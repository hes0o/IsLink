import { useState, useRef, useEffect } from 'react';
import './ChatBot.css';

function ChatBot() {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState([
    {
      id: 1,
      type: 'bot',
      text: "Hi! 👋 I'm IsLink AI Assistant. How can I help you today?",
      time: new Date()
    }
  ]);
  const [inputValue, setInputValue] = useState('');
  const [isTyping, setIsTyping] = useState(false);
  const messagesEndRef = useRef(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const quickReplies = [
    "How do I hire a freelancer?",
    "How do I become a seller?",
    "Payment methods?",
    "Contact support"
  ];

  const getBotResponse = (userMessage) => {
    const msg = userMessage.toLowerCase();
    
    if (msg.includes('hire') || msg.includes('freelancer') || msg.includes('find')) {
      return "To hire a freelancer:\n\n1. Browse services or search for what you need\n2. Check seller profiles and reviews\n3. Choose a package that fits your budget\n4. Place your order and describe your requirements\n5. Communicate with the seller and receive your delivery!\n\nWould you like me to help you find a specific service?";
    }
    
    if (msg.includes('seller') || msg.includes('sell') || msg.includes('offer')) {
      return "To become a seller on IsLink:\n\n1. Create an account and complete your profile\n2. Go to 'Become a Seller' in the menu\n3. Create your first gig with clear descriptions\n4. Set competitive prices and delivery times\n5. Start receiving orders!\n\n💡 Tip: Add portfolio samples to attract more buyers!";
    }
    
    if (msg.includes('payment') || msg.includes('pay') || msg.includes('money')) {
      return "IsLink supports multiple payment methods:\n\n💳 Credit/Debit Cards\n🏦 PayPal\n💰 IsLink Balance\n🔒 Bank Transfer\n\nAll payments are secure and protected. Funds are held safely until you approve the delivery.";
    }
    
    if (msg.includes('support') || msg.includes('contact') || msg.includes('help')) {
      return "You can reach our support team:\n\n📧 Email: support@islink.com\n💬 Live Chat: Available 24/7\n📞 Phone: +1 (555) 123-4567\n\nOur average response time is under 2 hours!";
    }
    
    if (msg.includes('price') || msg.includes('cost') || msg.includes('fee')) {
      return "IsLink pricing:\n\n• Buyers: Pay the listed price + 5.5% service fee\n• Sellers: Keep 80% of each order\n• No hidden fees!\n\nPrices start as low as $5 for basic services.";
    }

    if (msg.includes('hello') || msg.includes('hi') || msg.includes('hey')) {
      return "Hello! 😊 Great to meet you! I'm here to help with anything related to IsLink. What would you like to know?";
    }
    
    return "Thanks for your message! I can help you with:\n\n• Finding freelancers\n• Becoming a seller\n• Payment questions\n• Platform navigation\n• Account support\n\nWhat would you like to know more about?";
  };

  const handleSend = () => {
    if (!inputValue.trim()) return;

    const userMessage = {
      id: messages.length + 1,
      type: 'user',
      text: inputValue,
      time: new Date()
    };

    setMessages(prev => [...prev, userMessage]);
    setInputValue('');
    setIsTyping(true);

    // Simulate AI typing delay
    setTimeout(() => {
      const botResponse = {
        id: messages.length + 2,
        type: 'bot',
        text: getBotResponse(inputValue),
        time: new Date()
      };
      setMessages(prev => [...prev, botResponse]);
      setIsTyping(false);
    }, 1000 + Math.random() * 1000);
  };

  const handleQuickReply = (reply) => {
    setInputValue(reply);
    setTimeout(() => handleSend(), 100);
  };

  const handleKeyPress = (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  return (
    <div className="chatbot-container">
      {/* Chat Window */}
      {isOpen && (
        <div className="chat-window">
          {/* Header */}
          <div className="chat-header">
            <div className="chat-header-info">
              <div className="bot-avatar">
                <span>🤖</span>
                <span className="online-dot"></span>
              </div>
              <div className="bot-info">
                <span className="bot-name">IsLink AI Assistant</span>
                <span className="bot-status">Always online</span>
              </div>
            </div>
            <button className="close-btn" onClick={() => setIsOpen(false)}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M18 6L6 18M6 6l12 12"/>
              </svg>
            </button>
          </div>

          {/* Messages */}
          <div className="chat-messages">
            {messages.map((msg) => (
              <div key={msg.id} className={`message ${msg.type}`}>
                {msg.type === 'bot' && (
                  <div className="message-avatar">🤖</div>
                )}
                <div className="message-content">
                  <p>{msg.text}</p>
                  <span className="message-time">
                    {msg.time.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                  </span>
                </div>
              </div>
            ))}
            
            {isTyping && (
              <div className="message bot">
                <div className="message-avatar">🤖</div>
                <div className="message-content typing">
                  <div className="typing-indicator">
                    <span></span>
                    <span></span>
                    <span></span>
                  </div>
                </div>
              </div>
            )}
            
            <div ref={messagesEndRef} />
          </div>

          {/* Quick Replies */}
          {messages.length <= 2 && (
            <div className="quick-replies">
              {quickReplies.map((reply, idx) => (
                <button 
                  key={idx} 
                  className="quick-reply-btn"
                  onClick={() => handleQuickReply(reply)}
                >
                  {reply}
                </button>
              ))}
            </div>
          )}

          {/* Input */}
          <div className="chat-input">
            <input
              type="text"
              placeholder="Type your message..."
              value={inputValue}
              onChange={(e) => setInputValue(e.target.value)}
              onKeyPress={handleKeyPress}
            />
            <button 
              className="send-btn" 
              onClick={handleSend}
              disabled={!inputValue.trim()}
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M22 2L11 13M22 2l-7 20-4-9-9-4 20-7z"/>
              </svg>
            </button>
          </div>
        </div>
      )}

      {/* Toggle Button */}
      <button 
        className={`chat-toggle ${isOpen ? 'open' : ''}`}
        onClick={() => setIsOpen(!isOpen)}
      >
        {isOpen ? (
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M18 6L6 18M6 6l12 12"/>
          </svg>
        ) : (
          <>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M21 15a2 2 0 01-2 2H7l-4 4V5a2 2 0 012-2h14a2 2 0 012 2z"/>
            </svg>
            <span className="toggle-badge">AI</span>
          </>
        )}
      </button>
    </div>
  );
}

export default ChatBot;

