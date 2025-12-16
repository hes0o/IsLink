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
  const messagesEndRef = useRef(null);
  const chatContainerRef = useRef(null);

  useEffect(() => {
    // Start conversation on mount
    startConversation();
  }, []);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const startConversation = async () => {
    try {
      setLoading(true);
      const response = await linkerAIAPI.start();
      if (response?.Success) {
        setSessionId(response.SessionId);
        setMessages([{
          role: 'assistant',
          content: response.Message,
          timestamp: new Date()
        }]);
      }
    } catch (error) {
      console.error('Error starting conversation:', error);
      setMessages([{
        role: 'assistant',
        content: 'Sorry, I encountered an error. Please try again.',
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
      if (response?.Success) {
        setMessages(prev => [...prev, {
          role: 'assistant',
          content: response.Message,
          timestamp: new Date()
        }]);

        // If recommendations are ready, set them
        if (response.IsComplete && response.Recommendations) {
          setRecommendations(response.Recommendations);
        }
      }
    } catch (error) {
      console.error('Error sending message:', error);
      setMessages(prev => [...prev, {
        role: 'assistant',
        content: 'Sorry, I encountered an error. Please try again.',
        timestamp: new Date()
      }]);
    } finally {
      setLoading(false);
    }
  };

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
        {/* Chat Section - Centered */}
        <div className="linkerai-chat-wrapper">
          <div className="linkerai-chat" ref={chatContainerRef}>
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
                  <p><strong>Type:</strong> {recommendations.ProjectSummary.ProjectType}</p>
                  {recommendations.ProjectSummary.BrandName && (
                    <p><strong>Brand:</strong> {recommendations.ProjectSummary.BrandName}</p>
                  )}
                  <p><strong>Description:</strong> {recommendations.ProjectSummary.Description}</p>
                  {recommendations.ProjectSummary.KeyRequirements.length > 0 && (
                    <div>
                      <strong>Key Requirements:</strong>
                      <ul>
                        {recommendations.ProjectSummary.KeyRequirements.map((req, idx) => (
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
                    <strong>${recommendations.Budget.TotalBudget.toFixed(2)}</strong>
                  </div>
                  <div className="budget-item">
                    <span>Total Cost:</span>
                    <strong>${recommendations.Budget.TotalCost.toFixed(2)}</strong>
                  </div>
                  <div className="budget-item highlight">
                    <span>Remaining:</span>
                    <strong className={recommendations.Budget.Remaining >= 0 ? 'positive' : 'negative'}>
                      ${recommendations.Budget.Remaining.toFixed(2)}
                    </strong>
                  </div>
                  <div className="service-costs">
                    <strong>Service Costs:</strong>
                    <ul>
                      {recommendations.Budget.ServiceCosts.map((cost, idx) => (
                        <li key={idx}>
                          {cost.ServiceName}: <strong>${cost.Cost.toFixed(2)}</strong>
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
                  <p><strong>Estimated Delivery:</strong> {recommendations.Timeline.TotalDays} days</p>
                  <p><strong>Your Deadline:</strong> {recommendations.Timeline.DeadlineDays} days</p>
                  <p className={recommendations.Timeline.IsFeasible ? 'feasible' : 'not-feasible'}>
                    <strong>Status:</strong> {recommendations.Timeline.IsFeasible ? '✅ Feasible' : '⚠️ Tight Timeline'}
                  </p>
                  {recommendations.Timeline.Items.length > 0 && (
                    <div className="timeline-items">
                      {recommendations.Timeline.Items.map((item, idx) => (
                        <div key={idx} className="timeline-item">
                          <strong>{item.ServiceName}</strong>
                          <span>Days {item.StartDay}-{item.EndDay} ({item.Days} days)</span>
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
                  {recommendations.Services.map((service) => (
                    <div key={service.GigId} className="recommended-service-card">
                      {service.ImageUrl && (
                        <img src={service.ImageUrl} alt={service.Title} className="service-image" />
                      )}
                      <div className="service-info">
                        <h4>{service.Title}</h4>
                        <p className="service-category">{service.Category}</p>
                        <p className="service-seller">by {service.SellerName}</p>
                        <div className="service-rating">
                          ⭐ {service.Rating.toFixed(1)} ({service.ReviewCount} reviews)
                        </div>
                        <p className="service-price">${service.Price.toFixed(2)}</p>
                        <p className="service-delivery">Delivery: {service.DeliveryDays} days</p>
                        <p className="service-reason"><em>Why: {service.Reason}</em></p>
                        <Link to={`/gig/${service.Slug}`} className="btn-primary">
                          View Service
                        </Link>
                      </div>
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
