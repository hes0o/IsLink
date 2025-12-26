import { useState, useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import { linkerAIAPI } from '../../services/api';
import ErrorBoundary from '../../components/common/ErrorBoundary';
import './LinkerAI.css';

function LinkerAI() {
  const [sessionId, setSessionId] = useState(null);
  const [messages, setMessages] = useState([]);
  const [inputMessage, setInputMessage] = useState('');
  const [loading, setLoading] = useState(false);
  const [recommendations, setRecommendations] = useState(null);
  const [errorBanner, setErrorBanner] = useState('');
  const [history, setHistory] = useState([]);
  const [showHistory, setShowHistory] = useState(true); // Default open on desktop
  const messagesEndRef = useRef(null);
  const chatContainerRef = useRef(null);

  useEffect(() => {
    // Only load history on mount. 
    // We DO NOT auto-load the active session anymore to give user control.
    loadHistory();
  }, []);

  useEffect(() => {
    if (messages.length > 0) {
      scrollToBottom();
    }
  }, [messages, loading]);

  // Removed checkActiveSession from auto-startup
  // It is now only internal or triggered by explicit action if needed.
  const checkActiveSession = async () => {
    // Kept for reference or explicit "Resume" button if we add one later
    // For now, we rely on loadSession(id) from the sidebar.
    try {
      setLoading(true);
      const session = await linkerAIAPI.getActiveSession();
      if (session && (session.Success ?? session.success)) {
        // Restore session
        setSessionId(session.SessionId || session.sessionId);
        const lastMsg = session.Message || session.message;

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
    // TODO: Implement actual session fetch
  };

  // FIX: Improved Image URL handling
  const getImageUrl = (url) => {
    if (!url) return 'https://placehold.co/600x400?text=Service+Image';

    // If it's already a full URL (http/https), use it
    if (url.startsWith('http')) return url;

    // Handle relative paths
    // Replace backslashes with forward slashes for web compatibility
    let cleanUrl = url.replace(/\\/g, '/');

    // Remove leading slash if present to avoid double slashes with base URL
    if (cleanUrl.startsWith('/')) {
      cleanUrl = cleanUrl.slice(1);
    }

    // Construct full URL pointing to the API public folder (or wherever images are served)
    const cleanBase = (import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5001').replace(/\/$/, '');

    return `${cleanBase}/${cleanUrl}`;
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

  // NEW: Safe number formatting
  const safeFixed = (val, digits = 2) => {
    const num = Number(val);
    if (isNaN(num)) return '0.00';
    return num.toFixed(digits);
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
  };

  // New Chat Functionality
  const handleNewChat = () => {
    setSessionId(null);
    setMessages([]);
    setRecommendations(null);
    startConversation();
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

      // Refresh history sidebar to show new message/session
      loadHistory();
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
      {/* 1. Left Sidebar: History */}
      <div className={`linkerai-sidebar ${!showHistory ? 'closed' : ''}`}>
        <div className="sidebar-header">
          <button className="new-chat-btn" onClick={handleNewChat}>
            <span>+</span> New Chat
          </button>
        </div>

        <div className="sidebar-content">
          <div className="history-group-title">Recent</div>
          <div className="history-list">
            {history.length === 0 && <p style={{ padding: '0.5rem', color: '#9ca3af', fontSize: '0.9rem' }}>No history</p>}
            {history.map(sess => (
              <div
                key={safeGet(sess, 'sessionId')}
                className={`history-item ${safeGet(sess, 'sessionId') === sessionId ? 'active' : ''}`}
                onClick={() => loadSession(safeGet(sess, 'sessionId'))}
              >
                <span>💬</span>
                <span>{safeGet(sess, 'lastMessage') || 'New Conversation'}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* 2. Main Chat Area */}
      <div className="linkerai-main">
        <header className="linkerai-header">
          <div className="header-left">
            <button className="toggle-sidebar-btn" onClick={() => setShowHistory(!showHistory)}>
              {showHistory ? '◀' : '▶'}
            </button>
            <div className="model-selector">
              LinkerAI <span style={{ fontSize: '0.8rem', color: '#6b7280', fontWeight: 'normal' }}>Pro</span>
            </div>
          </div>
        </header>

        <div className="linkerai-chat-wrapper" ref={chatContainerRef}>
          {errorBanner && (
            <div className="linkerai-error-banner">
              <strong>Error:</strong> {errorBanner}
            </div>
          )}

          <div className="chat-messages">
            {messages.map((msg, index) => (
              <div key={index} className={`message ${msg.role}`}>
                <div className="message-avatar">
                  {msg.role === 'assistant' ? '🤖' : '👤'}
                </div>
                <div className="message-content">
                  <div className="message-name">{msg.role === 'assistant' ? 'LinkerAI' : 'You'}</div>
                  <div className="message-text">{msg.content}</div>
                </div>
              </div>
            ))}

            {loading && (
              <div className="message assistant">
                <div className="message-avatar">🤖</div>
                <div className="message-content">
                  <div className="message-name">LinkerAI</div>
                  <div className="message-text">
                    <div className="typing-indicator">
                      <span></span><span></span><span></span>
                    </div>
                  </div>
                </div>
              </div>
            )}
            <div ref={messagesEndRef} />
          </div>

          <div className="chat-input-container">
            <form className="chat-input-form" onSubmit={sendMessage}>
              <input
                type="text"
                value={inputMessage}
                onChange={(e) => setInputMessage(e.target.value)}
                placeholder="Message LinkerAI..."
                disabled={loading || !sessionId}
                className="chat-input"
              />
              <button
                type="submit"
                disabled={loading || !sessionId || !inputMessage.trim()}
                className="chat-send-button"
              >
                ➤
              </button>
            </form>
          </div>
        </div>
      </div>

      {/* 3. Right Panel: Recommendations (Conditional) */}
      {recommendations && (
        <div className="linkerai-recommendations">
          <h2>Recommendations</h2>

          {/* Project Summary */}
          <div className="rec-card">
            <h3>Project Summary</h3>
            <div className="project-summary">
              <p><strong>Type:</strong> {getRecProp(getRecProp(recommendations, 'projectSummary'), 'projectType')}</p>
              {getRecProp(getRecProp(recommendations, 'projectSummary'), 'brandName') && (
                <p><strong>Brand:</strong> {getRecProp(getRecProp(recommendations, 'projectSummary'), 'brandName')}</p>
              )}
              <p style={{ fontSize: '0.9rem' }}>{getRecProp(getRecProp(recommendations, 'projectSummary'), 'description')}</p>
            </div>
          </div>

          {/* Budget */}
          <div className="rec-card">
            <h3>Budget Analysis</h3>
            <div className="budget-breakdown">
              <div className="budget-item">
                <span>Total:</span>
                <strong>${safeFixed(getRecProp(getRecProp(recommendations, 'budget'), 'totalBudget'))}</strong>
              </div>
              <div className="budget-item highlight">
                <span>Rem:</span>
                <strong className={getRecProp(getRecProp(recommendations, 'budget'), 'remaining') >= 0 ? 'positive' : 'negative'}>
                  ${safeFixed(getRecProp(getRecProp(recommendations, 'budget'), 'remaining'))}
                </strong>
              </div>
            </div>
          </div>

          {/* Recommended Services */}
          <div className="rec-card">
            <h3>Services</h3>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
              {(getRecProp(recommendations, 'services') || []).map((service) => (
                <Link key={getRecProp(service, 'gigId')} to={`/gig/${getRecProp(service, 'slug')}`} className="recommended-service-card">
                  {getRecProp(service, 'imageUrl') && (
                    <img
                      src={getImageUrl(getRecProp(service, 'imageUrl'))}
                      alt={getRecProp(service, 'title')}
                      className="service-image"
                      onError={(e) => {
                        e.target.onerror = null; // Prevent infinite loop
                        e.target.src = 'https://placehold.co/600x400?text=Service'; // Reliable fallback
                      }}
                    />
                  )}
                  <div className="service-info">
                    <h4>{getRecProp(service, 'title')}</h4>
                    <div className="service-price">${safeFixed(getRecProp(service, 'price'))}</div>
                    <div className="service-rating">⭐ {safeFixed(getRecProp(service, 'rating'), 1)}</div>
                  </div>
                </Link>
              ))}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

function LinkerAIWrapper() {
  return (
    <ErrorBoundary>
      <LinkerAI />
    </ErrorBoundary>
  );
}

export default LinkerAIWrapper;
