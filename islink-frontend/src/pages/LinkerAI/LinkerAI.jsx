import { useState, useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import { linkerAIAPI } from '../../services/api';
import './LinkerAI.css';

function LinkerAI() {
  const [sessionId, setSessionId] = useState(null);
  const [messages, setMessages] = useState([]);
  const [inputMessage, setInputMessage] = useState('');
  const [loading, setLoading] = useState(false);
  const [recommendations, setRecommendations] = useState(null);
  const [errorBanner, setErrorBanner] = useState('');
  const [history, setHistory] = useState([]);
  const [showHistory, setShowHistory] = useState(false);
  const messagesEndRef = useRef(null);
  const chatContainerRef = useRef(null);

  useEffect(() => {
    // Check for active session on mount
    checkActiveSession();
    // Load history
    loadHistory();
  }, []);

  useEffect(() => {
    if (messages.length > 0) {
      scrollToBottom();
    }
  }, [messages, loading]);

  const checkActiveSession = async () => {
    try {
      setLoading(true);
      const session = await linkerAIAPI.getActiveSession();
      if (session && (session.Success ?? session.success)) {
        // Restore session
        setSessionId(session.SessionId || session.sessionId);
        const lastMsg = session.Message || session.message;

        // In a real app we would load full history, here we just show the last state
        // or a "Welcome back" to indicate continuity if history is partial.
        // For now, let's just set the session and if there was a last message, show it.
        if (lastMsg) {
          setMessages([{
            role: 'assistant',
            content: lastMsg,
            timestamp: new Date()
          }]);
        }

        // Restore recommendations if available
        const isComplete = session.IsComplete ?? session.isComplete;
        const recs = session.Recommendations || session.recommendations;
        if (isComplete && recs) {
          setRecommendations(recs);
        }
      } else {
        // No active session, start new
        await startConversation();
      }
    } catch (error) {
      console.error('Failed to restore session:', error);
      // Fallback to new session
      await startConversation();
    } finally {
      setLoading(false);
    }
  };

  const loadHistory = async () => {
    try {
      const resp = await linkerAIAPI.getSessions();
      if (resp?.Success ?? resp?.success) {
        setHistory(resp.Data || resp.data || []);
      }
    } catch (err) {
      console.error('Failed to load history', err);
    }
  };

  const loadSession = async (sessId) => {
    // For MVP, just switching the active highlight visually
    // In full implementation, we'd fetch the specific session details
    console.log("Switching to session", sessId);
  };



  const getImageUrl = (url) => {
    if (!url) return 'https://placehold.co/600x400?text=No+Image';
    if (url.startsWith('http')) return url;
    return `${import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5001'}/${url}`;
  };

  // Helper to safely access properties regardless of case
  const safeGet = (obj, key) => {
    if (!obj) return undefined;
    // Try exact, camel, Pascal
    if (obj[key] !== undefined) return obj[key];
    const camel = key.charAt(0).toLowerCase() + key.slice(1);
    if (obj[camel] !== undefined) return obj[camel];
    const pascal = key.charAt(0).toUpperCase() + key.slice(1);
    if (obj[pascal] !== undefined) return obj[pascal];
    return undefined;
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
  };

  const startConversation = async () => {
    try {
      setLoading(true);
      setErrorBanner('');
      const response = await linkerAIAPI.start();
      const success = safeGet(response, 'success');
      if (!success) {
        throw new Error(safeGet(response, 'message') || 'Failed to start LinkerAI session');
      }

      setSessionId(safeGet(response, 'sessionId'));
      setMessages([{
        role: 'assistant',
        content: safeGet(response, 'message'),
        timestamp: new Date()
      }]);
    } catch (error) {
      console.error('Error starting conversation:', error);
      const msg = error?.message || 'Unknown error';
      setErrorBanner(msg);
      setMessages([{
        role: 'assistant',
        content: `⚠️ LinkerAI error: ${msg}`,
        timestamp: new Date()
      }]);
    } finally {
      setLoading(false);
    }
  };

  const sendMessage = async (e) => {
    e.preventDefault();
    if (!inputMessage.trim() || !sessionId || loading) return;

    const messageToSend = inputMessage.trim();
    setErrorBanner('');
    const userMessage = {
      role: 'user',
      content: messageToSend,
      timestamp: new Date()
    };

    setMessages(prev => [...prev, userMessage]);
    setInputMessage('');
    setLoading(true);

    try {
      const response = await linkerAIAPI.chat(sessionId, messageToSend);
      const success = safeGet(response, 'success');
      if (!success) {
        throw new Error(safeGet(response, 'message') || 'LinkerAI request failed');
      }

      setMessages(prev => [...prev, {
        role: 'assistant',
        content: safeGet(response, 'message'),
        timestamp: new Date()
      }]);

      // If recommendations are ready, set them
      const isComplete = safeGet(response, 'isComplete');
      const recs = safeGet(response, 'recommendations');
      if (isComplete && recs) {
        setRecommendations(recs);
      }
    } catch (error) {
      console.error('Error sending message:', error);
      const msg = error?.message || 'Unknown error';
      setErrorBanner(msg);
      setMessages(prev => [...prev, {
        role: 'assistant',
        content: `⚠️ LinkerAI error: ${msg}`,
        timestamp: new Date()
      }]);
    } finally {
      setLoading(false);
    }
  };

  // Helper to normalize recommendation access in JSX
  const getRecProp = (obj, key) => safeGet(obj, key);

  return (
    <div className="linkerai-page">
      {/* Header */}
      <div className="linkerai-header">
        <div className="linkerai-header-content">
          <div className="linkerai-logo">
            <span className="ai-icon">🤖</span>
            <h1>LinkerAI</h1>
          </div>
          <p>Your intelligent project assistant</p>
        </div>
      </div>

      <div className="linkerai-main">
        {/* History Sidebar */}
        <div className={`linkerai-sidebar ${showHistory ? 'open' : ''}`}>
          <div className="sidebar-header">
            <h3>History</h3>
            <button onClick={() => setShowHistory(false)} className="close-sidebar">×</button>
          </div>
          <div className="history-list">
            {history.length === 0 && <p className="no-history">No past conversations</p>}
            {history.map(sess => (
              <div key={safeGet(sess, 'sessionId')} className={`history-item ${safeGet(sess, 'sessionId') === sessionId ? 'active' : ''}`} onClick={() => loadSession(safeGet(sess, 'sessionId'))}>
                <div className="history-date">{new Date(safeGet(sess, 'lastActivityAt')).toLocaleDateString()}</div>
                <div className="history-preview">{safeGet(sess, 'lastMessage')}</div>
              </div>
            ))}
          </div>
        </div>

        {/* Chat Section - Centered */}
        <div className="linkerai-chat-wrapper">
          <button className="toggle-history-btn" onClick={() => setShowHistory(!showHistory)}>
            📜 History
          </button>
          <div className="linkerai-chat" ref={chatContainerRef}>
            {errorBanner && (
              <div className="linkerai-error-banner">
                <strong>LinkerAI error:</strong> {errorBanner}
              </div>
            )}
            <div className="chat-messages">
              {messages.map((msg, index) => (
                <div key={index} className={`message ${msg.role}`}>
                  <div className="message-content">
                    {msg.role === 'assistant' && (
                      <div className="message-avatar">🤖</div>
                    )}
                    <div className="message-text">{msg.content}</div>
                    {msg.role === 'user' && (
                      <div className="message-avatar">👤</div>
                    )}
                  </div>
                </div>
              ))}
              {loading && (
                <div className="message assistant">
                  <div className="message-content">
                    <div className="message-avatar">🤖</div>
                    <div className="message-text">
                      <div className="typing-indicator">
                        <span></span>
                        <span></span>
                        <span></span>
                      </div>
                    </div>
                  </div>
                </div>
              )}
              <div ref={messagesEndRef} />
            </div>

            <form className="chat-input-form" onSubmit={sendMessage}>
              <input
                type="text"
                value={inputMessage}
                onChange={(e) => setInputMessage(e.target.value)}
                placeholder="Tell me about your project..."
                disabled={loading || !sessionId}
                className="chat-input"
              />
              <button
                type="submit"
                disabled={loading || !sessionId || !inputMessage.trim()}
                className="chat-send-button"
              >
                Send
              </button>
            </form>
          </div>
        </div>

        {/* Recommendations Section - Sidebar */}
        {recommendations && (
          <div className="linkerai-recommendations">
            <h2>📋 Your Personalized Recommendations</h2>

            {/* Project Summary */}
            <div className="recommendation-section">
              <h3>Project Summary</h3>
              <div className="project-summary">
                <p><strong>Type:</strong> {getRecProp(getRecProp(recommendations, 'projectSummary'), 'projectType')}</p>
                {getRecProp(getRecProp(recommendations, 'projectSummary'), 'brandName') && (
                  <p><strong>Brand:</strong> {getRecProp(getRecProp(recommendations, 'projectSummary'), 'brandName')}</p>
                )}
                <p><strong>Description:</strong> {getRecProp(getRecProp(recommendations, 'projectSummary'), 'description')}</p>
                {(getRecProp(getRecProp(recommendations, 'projectSummary'), 'keyRequirements') || []).length > 0 && (
                  <div>
                    <strong>Key Requirements:</strong>
                    <ul>
                      {getRecProp(getRecProp(recommendations, 'projectSummary'), 'keyRequirements').map((req, idx) => (
                        <li key={idx}>{req}</li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>
            </div>

            {/* Budget Breakdown */}
            <div className="recommendation-section">
              <h3>💰 Budget Breakdown</h3>
              <div className="budget-breakdown">
                <div className="budget-item">
                  <span>Total Budget:</span>
                  <strong>${getRecProp(getRecProp(recommendations, 'budget'), 'totalBudget')?.toFixed(2)}</strong>
                </div>
                <div className="budget-item">
                  <span>Total Cost:</span>
                  <strong>${getRecProp(getRecProp(recommendations, 'budget'), 'totalCost')?.toFixed(2)}</strong>
                </div>
                <div className="budget-item highlight">
                  <span>Remaining:</span>
                  <strong className={getRecProp(getRecProp(recommendations, 'budget'), 'remaining') >= 0 ? 'positive' : 'negative'}>
                    ${getRecProp(getRecProp(recommendations, 'budget'), 'remaining')?.toFixed(2)}
                  </strong>
                </div>
                <div className="service-costs">
                  <strong>Service Costs:</strong>
                  <ul>
                    {(getRecProp(getRecProp(recommendations, 'budget'), 'serviceCosts') || []).map((cost, idx) => (
                      <li key={idx}>
                        {getRecProp(cost, 'serviceName')}: <strong>${getRecProp(cost, 'cost')?.toFixed(2)}</strong>
                      </li>
                    ))}
                  </ul>
                </div>
              </div>
            </div>

            {/* Timeline */}
            <div className="recommendation-section">
              <h3>📅 Timeline</h3>
              <div className="timeline-info">
                <p><strong>Estimated Delivery:</strong> {getRecProp(getRecProp(recommendations, 'timeline'), 'totalDays')} days</p>
                <p><strong>Your Deadline:</strong> {getRecProp(getRecProp(recommendations, 'timeline'), 'deadlineDays')} days</p>
                <p className={getRecProp(getRecProp(recommendations, 'timeline'), 'isFeasible') ? 'feasible' : 'not-feasible'}>
                  <strong>Status:</strong> {getRecProp(getRecProp(recommendations, 'timeline'), 'isFeasible') ? '✅ Feasible' : '⚠️ Tight Timeline'}
                </p>
                {(getRecProp(getRecProp(recommendations, 'timeline'), 'items') || []).length > 0 && (
                  <div className="timeline-items">
                    {getRecProp(getRecProp(recommendations, 'timeline'), 'items').map((item, idx) => (
                      <div key={idx} className="timeline-item">
                        <strong>{getRecProp(item, 'serviceName')}</strong>
                        <span>Days {getRecProp(item, 'startDay')}-{getRecProp(item, 'endDay')} ({getRecProp(item, 'days')} days)</span>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>

            {/* Recommended Services */}
            <div className="recommendation-section">
              <h3>🎯 Recommended Services</h3>
              <div className="recommended-services">
                {(getRecProp(recommendations, 'services') || []).map((service) => (
                  <div key={getRecProp(service, 'gigId')} className="recommended-service-card">
                    <Link to={`/gig/${getRecProp(service, 'slug')}`} className="service-card-link">
                      {getRecProp(service, 'imageUrl') && (
                        <img src={getImageUrl(getRecProp(service, 'imageUrl'))} alt={getRecProp(service, 'title')} className="service-image"
                          onError={(e) => e.target.src = 'https://placehold.co/600x400?text=Image+Error'} />
                      )}
                      <div className="service-info">
                        <h4>{getRecProp(service, 'title')}</h4>
                        <p className="service-category">{getRecProp(service, 'category')}</p>
                        <p className="service-seller">by {getRecProp(service, 'sellerName')}</p>
                        <div className="service-rating">
                          ⭐ {getRecProp(service, 'rating')?.toFixed(1)} ({getRecProp(service, 'reviewCount')} reviews)
                        </div>
                        <p className="service-price">${getRecProp(service, 'price')?.toFixed(2)}</p>
                        <p className="service-delivery">Delivery: {getRecProp(service, 'deliveryDays')} days</p>
                        <p className="service-reason"><em>Why: {getRecProp(service, 'reason')}</em></p>
                        <span className="btn-view">View Service</span>
                      </div>
                    </Link>
                  </div>
                ))}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default LinkerAI;
