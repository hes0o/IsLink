import { useState, useEffect, useRef } from 'react';
import { useAuth } from '../../context';
import './Messages.css';

function Messages() {
  const { user } = useAuth();
  const [conversations, setConversations] = useState([]);
  const [activeConversation, setActiveConversation] = useState(null);
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const chatContainerRef = useRef(null);

  useEffect(() => {
    loadConversations();
  }, []);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const loadConversations = async () => {
    // Mock conversations for demo
    const mockConversations = [
      {
        id: '1',
        participant: { username: 'john_buyer', fullName: 'John Smith', avatar: 'https://via.placeholder.com/50?text=JS' },
        lastMessage: 'Thanks for the great work!',
        unreadCount: 2,
        updatedAt: new Date().toISOString(),
        relatedGig: 'Logo Design Service'
      },
      {
        id: '2',
        participant: { username: 'sarah_client', fullName: 'Sarah Johnson', avatar: 'https://via.placeholder.com/50?text=SJ' },
        lastMessage: 'Can you add some revisions?',
        unreadCount: 0,
        updatedAt: new Date(Date.now() - 3600000).toISOString(),
        relatedGig: 'Website Development'
      },
      {
        id: '3',
        participant: { username: 'mike_dev', fullName: 'Mike Wilson', avatar: 'https://via.placeholder.com/50?text=MW' },
        lastMessage: 'When can you deliver?',
        unreadCount: 1,
        updatedAt: new Date(Date.now() - 86400000).toISOString(),
        relatedGig: null
      }
    ];

    setConversations(mockConversations);
    setIsLoading(false);
  };

  const loadMessages = (conversationId) => {
    const mockMessages = [
      { id: '1', senderId: 'other', content: 'Hi! I saw your gig and I\'m interested.', createdAt: new Date(Date.now() - 7200000).toISOString() },
      { id: '2', senderId: 'me', content: 'Hello! Thank you for reaching out. What do you need?', createdAt: new Date(Date.now() - 7000000).toISOString() },
      { id: '3', senderId: 'other', content: 'I need a logo for my startup company.', createdAt: new Date(Date.now() - 6800000).toISOString() },
      { id: '4', senderId: 'me', content: 'Sure! Can you tell me more about your company and what style you\'re looking for?', createdAt: new Date(Date.now() - 6600000).toISOString() },
      { id: '5', senderId: 'other', content: 'It\'s a tech company called "TechFlow". I want something modern and clean.', createdAt: new Date(Date.now() - 6400000).toISOString() },
      { id: '6', senderId: 'me', content: 'Great! I can definitely help with that. I\'ll prepare some concepts for you.', createdAt: new Date(Date.now() - 3600000).toISOString() },
      { id: '7', senderId: 'other', content: 'Thanks for the great work!', createdAt: new Date().toISOString() },
    ];

    setMessages(mockMessages);
    const conv = conversations.find(c => c.id === conversationId);
    setActiveConversation(conv);
  };

  const scrollToBottom = () => {
    if (chatContainerRef.current) {
      const { scrollHeight, clientHeight } = chatContainerRef.current;
      chatContainerRef.current.scrollTop = scrollHeight - clientHeight;
    }
  };

  const handleSendMessage = (e) => {
    e.preventDefault();
    if (!newMessage.trim()) return;

    const newMsg = {
      id: Date.now().toString(),
      senderId: 'me',
      content: newMessage,
      createdAt: new Date().toISOString()
    };

    setMessages(prev => [...prev, newMsg]);
    setNewMessage('');
  };

  const formatTime = (dateString) => {
    const date = new Date(dateString);
    const now = new Date();
    const diff = now - date;

    if (diff < 60000) return 'Just now';
    if (diff < 3600000) return `${Math.floor(diff / 60000)}m ago`;
    if (diff < 86400000) return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    return date.toLocaleDateString([], { month: 'short', day: 'numeric' });
  };

  return (
    <div className="messages-page">
      <div className="messages-container">
        {/* Conversations List */}
        <aside className="conversations-list">
          <div className="conversations-header">
            <h2>Messages</h2>
            <button className="btn-compose">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M12 5v14M5 12h14" />
              </svg>
            </button>
          </div>

          <div className="conversations-search">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <circle cx="11" cy="11" r="8" />
              <path d="M21 21l-4.35-4.35" />
            </svg>
            <input type="text" placeholder="Search messages..." />
          </div>

          {isLoading ? (
            <div className="loading">Loading...</div>
          ) : (
            <div className="conversations">
              {conversations.map(conv => (
                <div
                  key={conv.id}
                  className={`conversation-item ${activeConversation?.id === conv.id ? 'active' : ''}`}
                  onClick={() => loadMessages(conv.id)}
                >
                  <img src={conv.participant.avatar} alt={conv.participant.fullName} className="conv-avatar" />
                  <div className="conv-info">
                    <div className="conv-header">
                      <h4>{conv.participant.fullName}</h4>
                      <span className="conv-time">{formatTime(conv.updatedAt)}</span>
                    </div>
                    <p className="conv-preview">{conv.lastMessage}</p>
                    {conv.relatedGig && (
                      <span className="conv-gig">📎 {conv.relatedGig}</span>
                    )}
                  </div>
                  {conv.unreadCount > 0 && (
                    <span className="unread-badge">{conv.unreadCount}</span>
                  )}
                </div>
              ))}
            </div>
          )}
        </aside>

        {/* Chat Area */}
        <main className="chat-area">
          {activeConversation ? (
            <>
              <div className="chat-header">
                <div className="chat-user">
                  <img src={activeConversation.participant.avatar} alt={activeConversation.participant.fullName} />
                  <div>
                    <h3>{activeConversation.participant.fullName}</h3>
                    <span className="user-status online">Online</span>
                  </div>
                </div>
                <div className="chat-actions">
                  <button className="action-btn">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                      <circle cx="12" cy="12" r="1" />
                      <circle cx="19" cy="12" r="1" />
                      <circle cx="5" cy="12" r="1" />
                    </svg>
                  </button>
                </div>
              </div>

              <div className="chat-messages" ref={chatContainerRef}>
                {messages.map(msg => (
                  <div key={msg.id} className={`message ${msg.senderId === 'me' ? 'sent' : 'received'}`}>
                    <div className="message-bubble">
                      <p>{msg.content}</p>
                      <span className="message-time">{formatTime(msg.createdAt)}</span>
                    </div>
                  </div>
                ))}
              </div>

              <form className="chat-input" onSubmit={handleSendMessage}>
                <button type="button" className="attach-btn">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M21.44 11.05l-9.19 9.19a6 6 0 0 1-8.49-8.49l9.19-9.19a4 4 0 0 1 5.66 5.66l-9.2 9.19a2 2 0 0 1-2.83-2.83l8.49-8.48" />
                  </svg>
                </button>
                <input
                  type="text"
                  placeholder="Type a message..."
                  value={newMessage}
                  onChange={(e) => setNewMessage(e.target.value)}
                />
                <button type="submit" className="send-btn" disabled={!newMessage.trim()}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <line x1="22" y1="2" x2="11" y2="13" />
                    <polygon points="22 2 15 22 11 13 2 9 22 2" />
                  </svg>
                </button>
              </form>
            </>
          ) : (
            <div className="no-conversation">
              <div className="empty-state">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
                  <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
                </svg>
                <h3>Select a conversation</h3>
                <p>Choose a conversation from the list to start chatting</p>
              </div>
            </div>
          )}
        </main>
      </div>
    </div>
  );
}

export default Messages;
