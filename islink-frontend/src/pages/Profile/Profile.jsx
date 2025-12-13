import { useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context';
import { users, gigs, reviews } from '../../data/mockData';
import GigCard from '../../components/gig/GigCard';
import './Profile.css';

function Profile() {
  const { username } = useParams();
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const [activeTab, setActiveTab] = useState('gigs');
  const [showContactModal, setShowContactModal] = useState(false);
  const [contactMessage, setContactMessage] = useState('');

  // Find user - fallback to first user for demo
  const user = users.find(u => u.username === username) || users[0];
  
  // Get user's gigs
  const userGigs = gigs.filter(g => g.sellerId === user.id);
  
  // Get user's reviews
  const userReviews = reviews.filter(r => r.sellerId === user.id);

  return (
    <div className="profile-page">
      <div className="container">
        <div className="profile-layout">
          {/* Sidebar */}
          <aside className="profile-sidebar">
            <div className="profile-card">
              {/* Avatar & Basic Info */}
              <div className="profile-header">
                <div className="avatar-container">
                  <img 
                    src={user.avatar} 
                    alt={user.fullName}
                    className="profile-avatar"
                  />
                  {user.isOnline && <span className="online-badge"></span>}
                </div>
                <h1 className="profile-name">{user.fullName}</h1>
                <p className="profile-username">@{user.username}</p>
                
                {/* Rating */}
                <div className="profile-rating">
                  <span className="star">★</span>
                  <span className="rating-value">{user.rating}</span>
                  <span className="rating-count">({user.reviewCount} reviews)</span>
                </div>

                {/* Badges */}
                <div className="profile-badges">
                  {user.rating >= 4.7 && (
                    <span className="badge top-rated">⭐ Top Rated</span>
                  )}
                  {user.completedOrders >= 100 && (
                    <span className="badge verified">✓ Verified Pro</span>
                  )}
                </div>
              </div>

              {/* Actions */}
              <div className="profile-actions">
                <button 
                  className="btn-contact-profile"
                  onClick={() => setShowContactModal(true)}
                >
                  Contact Me
                </button>
              </div>

              {/* Info */}
              <div className="profile-info">
                <div className="info-item">
                  <span className="info-label">From</span>
                  <span className="info-value">{user.country}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Member since</span>
                  <span className="info-value">
                    {new Date(user.memberSince).toLocaleDateString('en-US', { 
                      month: 'short', 
                      year: 'numeric' 
                    })}
                  </span>
                </div>
                <div className="info-item">
                  <span className="info-label">Avg. response time</span>
                  <span className="info-value">1 hour</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Last delivery</span>
                  <span className="info-value">About 2 hours</span>
                </div>
              </div>

              {/* Description */}
              <div className="profile-description">
                <h3>Description</h3>
                <p>{user.bio}</p>
              </div>

              {/* Languages */}
              <div className="profile-languages">
                <h3>Languages</h3>
                <ul>
                  {user.languages.map((lang, idx) => (
                    <li key={idx}>
                      <span className="lang-name">{lang.name}</span>
                      <span className="lang-level">{lang.level}</span>
                    </li>
                  ))}
                </ul>
              </div>

              {/* Skills */}
              <div className="profile-skills">
                <h3>Skills</h3>
                <div className="skills-list">
                  {user.skills.map((skill, idx) => (
                    <span key={idx} className="skill-tag">{skill}</span>
                  ))}
                </div>
              </div>

              {/* Stats */}
              <div className="profile-stats">
                <div className="stat-box">
                  <span className="stat-value">{user.completedOrders}</span>
                  <span className="stat-label">Orders Completed</span>
                </div>
                <div className="stat-box">
                  <span className="stat-value">{userGigs.length}</span>
                  <span className="stat-label">Active Gigs</span>
                </div>
              </div>
            </div>
          </aside>

          {/* Main Content */}
          <main className="profile-main">
            {/* Tabs */}
            <div className="profile-tabs">
              <button 
                className={`tab ${activeTab === 'gigs' ? 'active' : ''}`}
                onClick={() => setActiveTab('gigs')}
              >
                Gigs ({userGigs.length})
              </button>
              <button 
                className={`tab ${activeTab === 'reviews' ? 'active' : ''}`}
                onClick={() => setActiveTab('reviews')}
              >
                Reviews ({user.reviewCount})
              </button>
            </div>

            {/* Tab Content */}
            <div className="tab-content">
              {activeTab === 'gigs' && (
                <div className="gigs-section">
                  {userGigs.length > 0 ? (
                    <div className="profile-gigs-grid">
                      {userGigs.map(gig => (
                        <GigCard key={gig.id} gig={gig} />
                      ))}
                    </div>
                  ) : (
                    <div className="empty-state">
                      <p>No gigs available yet.</p>
                    </div>
                  )}
                </div>
              )}

              {activeTab === 'reviews' && (
                <div className="reviews-section">
                  {/* Reviews Summary */}
                  <div className="reviews-overview">
                    <div className="overall-rating">
                      <span className="big-rating">{user.rating}</span>
                      <div className="rating-stars">
                        {'★'.repeat(Math.round(user.rating))}
                        {'☆'.repeat(5 - Math.round(user.rating))}
                      </div>
                      <span className="total-reviews">{user.reviewCount} reviews</span>
                    </div>
                    <div className="rating-breakdown">
                      <div className="breakdown-row">
                        <span>Seller communication</span>
                        <div className="mini-stars">★★★★★</div>
                        <span>5.0</span>
                      </div>
                      <div className="breakdown-row">
                        <span>Service as described</span>
                        <div className="mini-stars">★★★★★</div>
                        <span>4.9</span>
                      </div>
                      <div className="breakdown-row">
                        <span>Would recommend</span>
                        <div className="mini-stars">★★★★★</div>
                        <span>5.0</span>
                      </div>
                    </div>
                  </div>

                  {/* Reviews List */}
                  <div className="reviews-list">
                    {reviews.map(review => (
                      <div key={review.id} className="review-item">
                        <div className="review-header">
                          <img 
                            src={review.buyerAvatar} 
                            alt={review.buyerName}
                            className="reviewer-avatar"
                          />
                          <div className="reviewer-info">
                            <span className="reviewer-name">{review.buyerName}</span>
                            <span className="reviewer-country">{review.buyerCountry}</span>
                          </div>
                          <div className="review-meta">
                            <div className="review-stars">
                              {'★'.repeat(review.rating)}
                              {'☆'.repeat(5 - review.rating)}
                            </div>
                            <span className="review-date">
                              {new Date(review.createdAt).toLocaleDateString('en-US', {
                                month: 'short',
                                day: 'numeric',
                                year: 'numeric'
                              })}
                            </span>
                          </div>
                        </div>
                        <p className="review-text">{review.comment}</p>
                        {review.sellerResponse && (
                          <div className="seller-response">
                            <span className="response-label">Seller's Response</span>
                            <p>{review.sellerResponse}</p>
                          </div>
                        )}
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          </main>
        </div>
      </div>

      {/* Contact Modal */}
      {showContactModal && (
        <div className="modal-overlay" onClick={() => setShowContactModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <button className="modal-close" onClick={() => setShowContactModal(false)}>×</button>
            {isAuthenticated ? (
              <>
                <h3>Contact {user.username}</h3>
                <textarea 
                  placeholder="Hi! I would like to discuss a project with you..."
                  rows={5}
                  value={contactMessage}
                  onChange={(e) => setContactMessage(e.target.value)}
                ></textarea>
                <button 
                  className="btn-send"
                  onClick={() => {
                    alert('Message sent! (This will work when the database is connected)');
                    setShowContactModal(false);
                    setContactMessage('');
                    navigate('/messages');
                  }}
                  disabled={!contactMessage.trim()}
                >
                  Send Message
                </button>
              </>
            ) : (
              <>
                <h3>Please Log In</h3>
                <p className="modal-text">You need to be logged in to contact sellers.</p>
                <div className="modal-actions">
                  <Link to="/auth/login" className="btn-send">Sign In</Link>
                  <Link to="/auth/register" className="btn-secondary">Create Account</Link>
                </div>
              </>
            )}
          </div>
        </div>
      )}
    </div>
  );
}

export default Profile;
