import { useState, useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import { linkerAIAPI } from '../../services/api';
import ErrorBoundary from '../../components/common/ErrorBoundary';
import './LinkerAI.v3.css';

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

  // Gemini-like "Starter Chips"
  const starterChips = [
    { label: "Build a new e-commerce site", prompt: "I want to build a new e-commerce website for my clothing brand." },
    { label: "Create a marketing plan", prompt: "I need a marketing plan for my mobile app launch." },
    { label: "Fix bugs in my React app", prompt: "I have some bugs in my React application that need fixing." },
    { label: "Design a logo", prompt: "I need a professional logo design for a tech startup." }
  ];

  useEffect(() => {
    // Only load history on mount. 
    loadHistory();
  }, []);

  useEffect(() => {
    if (messages.length > 0) {
      scrollToBottom();
    }
  }, [messages, loading]);

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
    if (sessId === sessionId) return;

    try {
      setLoading(true);
      setErrorBanner('');
      const resp = await linkerAIAPI.getSession(sessId);

      const success = safeGet(resp, 'success');
      const data = safeGet(resp, 'data');

      if (success && data) {
        setSessionId(safeGet(data, 'sessionId'));

        // Transform messages
        const msgs = safeGet(data, 'messages') || [];
        setMessages(msgs.map(m => ({
          role: safeGet(m, 'role'),
          content: safeGet(m, 'content'),
          timestamp: safeGet(m, 'timestamp')
        })));

        // Handle recommendations
        const isComplete = safeGet(data, 'isComplete');
        const recs = safeGet(data, 'recommendations');
        if (isComplete && recs) {
          setRecommendations(recs);
        } else {
          setRecommendations(null);
        }
      }
    } catch (err) {
      console.error('Failed to load session details', err);
      setErrorBanner('Failed to load conversation history');
    } finally {
      if (window.innerWidth < 768) {
        setShowHistory(false);
      }
      setLoading(false);
    }
  };

  // Improved Image URL handling
  const getImageUrl = (url) => {
    if (!url) return 'https://placehold.co/600x400?text=Service+Image';
    if (url.startsWith('http')) return url;
    let cleanUrl = url.replace(/\\/g, '/');
    if (cleanUrl.startsWith('/')) {
      cleanUrl = cleanUrl.slice(1);
    }
    const cleanBase = (import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5001').replace(/\/$/, '');
    return `${cleanBase}/${cleanUrl}`;
  };

  const safeGet = (obj, key) => {
    if (!obj) return undefined;
    if (obj[key] !== undefined) return obj[key];
    const camel = key.charAt(0).toLowerCase() + key.slice(1);
    if (obj[camel] !== undefined) return obj[camel];
    const pascal = key.charAt(0).toUpperCase() + key.slice(1);
    if (obj[pascal] !== undefined) return obj[pascal];
    return undefined;
  };

  const safeFixed = (val, digits = 2) => {
    const num = Number(val);
    if (isNaN(num)) return '0.00';
    return num.toFixed(digits);
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
  };

  const handleNewChat = () => {
    setSessionId(null);
    setMessages([]);
    setRecommendations(null);
  };

  const handleStarterClick = (prompt) => {
    setInputMessage(prompt);
    // Slight delay to ensure state update before submit if we wanted auto-send, 
    // but better to just pre-fill or directly trigger.
    // Let's directly trigger send logic for smooth UX
    triggerSend(prompt);
  };

  // Separate send logic to allow external calls (chips)
  const triggerSend = async (msgText) => {
    if (!msgText.trim() || loading) return;

    // Auto-start session if needed
    let currentSessionId = sessionId;
    if (!currentSessionId) {
      try {
        setLoading(true);
        const response = await linkerAIAPI.start();
        if (!safeGet(response, 'success')) throw new Error('Start failed');
        currentSessionId = safeGet(response, 'sessionId');
        setSessionId(currentSessionId);

        // Note: We ignore the welcome message from start() since user is already "sending" their intent
        // Or we insert it as the first message silently. 
        // Gemini usually just starts answering the prompt.
        // Let's silently start and then send the user message.
      } catch (error) {
        console.error(error);
        setErrorBanner('Failed to start new conversation');
        setLoading(false);
        return;
      }
    }

    // Now send the user message
    const messageToSend = msgText.trim();
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
      const response = await linkerAIAPI.chat(currentSessionId, messageToSend);
      if (!safeGet(response, 'success')) {
        throw new Error(safeGet(response, 'message') || 'LinkerAI request failed');
      }

      setMessages(prev => [...prev, {
        role: 'assistant',
        content: safeGet(response, 'message'),
        timestamp: new Date()
      }]);

      const isComplete = safeGet(response, 'isComplete');
      const recs = safeGet(response, 'recommendations');
      if (isComplete && recs) {
        setRecommendations(recs);
      }

      loadHistory();
    } catch (error) {
      console.error('Error sending message:', error);
      setErrorBanner(error.message || 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  const sendMessage = (e) => {
    e.preventDefault();
    triggerSend(inputMessage);
  };

  const getRecProp = (obj, key) => safeGet(obj, key);

  // Render Welcome Screen if no messages
  const renderContent = () => {
    if (messages.length === 0) {
      return (
        <div className="welcome-screen">
          <div className="welcome-gradient">Hello, User</div>
          <div className="welcome-subtitle">How can I help you today?</div>
          <div className="starter-chips">
            {starterChips.map((chip, idx) => (
              <button
                key={idx}
                className="starter-chip"
                onClick={() => handleStarterClick(chip.prompt)}
              >
                <p>{chip.label}</p>
              </button>
            ))}
          </div>
        </div>
      );
    }

    return (
      <div className="chat-messages">
        {messages.map((msg, index) => (
          <div key={index} className={`message ${msg.role}`}>
            <div className="message-avatar">
              {msg.role === 'assistant' ?
                <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                  {/* Assistant Icon */}
                  <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 14.5l-1 1-1-1-2.5-1 2.5-1 1-1 1 1 2.5 1-2.5 1zm5.5-5.5l-2.5 1 2.5 1 1 1 1-1 2.5-1-2.5-1-1-1-1 1z" />
                </svg>
                : '👤'}
            </div>
            <div className="message-content">
              <div className="message-name">{msg.role === 'assistant' ? 'LinkerAI' : 'You'}</div>
              <div className="message-text">{msg.content}</div>
            </div>
          </div>
        ))}

        {loading && (
          <div className="message assistant">
            <div className="message-avatar">✨</div>
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

        {/* IN-STREAM RECOMMENDATIONS WIDGET */}
        {recommendations && (
          <div className="message assistant" style={{ marginTop: '1rem' }}>
            <div className="message-avatar">✨</div>
            <div className="message-content rec-message-container">
              <div className="rec-header">
                <h3>Recommended Plan</h3>
                <p style={{ color: '#64748b' }}>{safeGet(safeGet(recommendations, 'projectSummary'), 'description')}</p>

                <div className="rec-metrics">
                  <div className="rec-metric-item">
                    <span className="rec-metric-label">Project Type</span>
                    <span className="rec-metric-value">{safeGet(safeGet(recommendations, 'projectSummary'), 'projectType')}</span>
                  </div>
                  <div className="rec-metric-item">
                    <span className="rec-metric-label">Budget</span>
                    <span className="rec-metric-value">${safeFixed(safeGet(safeGet(recommendations, 'budget'), 'totalBudget'))}</span>
                  </div>
                  <div className="rec-metric-item">
                    <span className="rec-metric-label">Remaining</span>
                    <span className="rec-metric-value" style={{ color: safeGet(safeGet(recommendations, 'budget'), 'remaining') >= 0 ? 'var(--neutral-800)' : 'var(--error)' }}>
                      ${safeFixed(safeGet(safeGet(recommendations, 'budget'), 'remaining'))}
                    </span>
                  </div>
                </div>
              </div>

              <h4 style={{ marginBottom: '1rem', color: 'var(--neutral-700)' }}>Suggested Services</h4>
              <div className="rec-services-grid">
                {(safeGet(recommendations, 'services') || []).map((service) => (
                  <div className="rec-cinematic-card" key={safeGet(service, 'gigId')}>
                    {safeGet(service, 'imageUrl') ? (
                      <img
                        src={getImageUrl(safeGet(service, 'imageUrl'))}
                        alt={safeGet(service, 'title')}
                        className="rec-cine-img"
                        onError={(e) => {
                          e.target.onerror = null;
                          e.target.src = 'https://placehold.co/600x400?text=Service';
                        }}
                      />
                    ) : (
                      <div className="rec-cine-img" style={{ backgroundColor: '#1a1a1a' }}></div>
                    )}

                    <div className="rec-cine-overlay">
                      <div className="rec-cine-title">{safeGet(service, 'title')}</div>

                      <div className="rec-cine-details">
                        <div className="rec-cine-metrics">
                          <span className="rec-cine-price">${safeFixed(safeGet(service, 'price'))}</span>
                          <span className="rec-cine-rating">
                            <span style={{ color: '#facc15', fontSize: '1.2rem' }}>★</span>
                            {safeFixed(safeGet(service, 'rating'), 1)}
                          </span>
                        </div>
                        <Link to={`/gig/${safeGet(service, 'slug')}`} className="rec-cine-btn">
                          View Details
                        </Link>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        )}

        <div ref={messagesEndRef} />
      </div>
    );
  };

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
                <span>{safeGet(sess, 'title') || safeGet(sess, 'lastMessage') || 'New Chat'}</span>
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

          {renderContent()}

          <div className="chat-input-container">
            <form className="chat-input-form" onSubmit={sendMessage}>
              <input
                type="text"
                value={inputMessage}
                onChange={(e) => setInputMessage(e.target.value)}
                placeholder={recommendations ? "Conversation completed." : "Message LinkerAI..."}
                disabled={loading || !!recommendations}
                className="chat-input"
              />
              <button
                type="submit"
                disabled={loading || !inputMessage.trim() || !!recommendations}
                className="chat-send-button"
              >
                {/* Send Icon */}
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <line x1="22" y1="2" x2="11" y2="13"></line>
                  <polygon points="22 2 15 22 11 13 2 9 22 2"></polygon>
                </svg>
              </button>
            </form>
          </div>
        </div>
      </div>
      {/* Sidebar Removed */}
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
