import { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context';
import { usersAPI, gigsAPI, reviewsAPI } from '../../services/api';
import GigCard from '../../components/gig/GigCard';
import './Profile.css';

function Profile() {
  const { username } = useParams();
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const [activeTab, setActiveTab] = useState('gigs');
  const [showContactModal, setShowContactModal] = useState(false);
  const [contactMessage, setContactMessage] = useState('');
  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState(null);
  const [userGigs, setUserGigs] = useState([]);
  const [userReviews, setUserReviews] = useState([]);

  useEffect(() => {
    const loadProfileData = async () => {
      setLoading(true);
      try {
        // Fetch user data
        const userResponse = await usersAPI.getByUsername(username);
        if (userResponse?.Success && userResponse?.Data) {
          setUser(userResponse.Data);
        } else {
          // User not found
          setUser(null);
        }

        // Fetch user's gigs
        try {
          const gigsResponse = await gigsAPI.getBySeller(username);
          const gigsList = gigsResponse?.Data || gigsResponse?.data || gigsResponse || [];
          setUserGigs(Array.isArray(gigsList) ? gigsList : []);
        } catch (err) {
          console.log('Could not load gigs:', err);
          setUserGigs([]);
        }

        // Fetch user's reviews
        try {
          const reviewsResponse = await reviewsAPI.getBySeller(username);
          const reviewsData = reviewsResponse?.Data?.Reviews || reviewsResponse?.Data?.reviews || reviewsResponse?.Data || [];
          setUserReviews(Array.isArray(reviewsData) ? reviewsData : []);
        } catch (err) {
          console.log('Could not load reviews:', err);
          setUserReviews([]);
        }
      } catch (err) {
        console.error('Error loading profile:', err);
        setUser(null);
      } finally {
        setLoading(false);
      }
    };

    if (username) {
      loadProfileData();
    } else {
      setLoading(false);
    }
  }, [username]);

  if (loading) {
    return (
      <div className="profile-page">
        <div className="container">
          <div className="loading-state">
            <div className="spinner"></div>
            <p>Loading profile...</p>
          </div>
        </div>
      </div>
    );
  }

  if (!user) {
    return (
      <div className="profile-page">
        <div className="container">
          <div className="error-state">
            <p>User not found</p>
            <Link to="/" className="btn-primary">Go Home</Link>
          </div>
        </div>
      </div>
    );
  }

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
                    src={user.avatarUrl || user.AvatarUrl || 'https://via.placeholder.com/120?text=U'} 
                    alt={user.fullName || user.FullName}
                    className="profile-avatar"
                    onError={(e) => {
                      e.target.src = 'https://via.placeholder.com/120?text=U';
                    }}
                  />
                  {(user.isOnline || user.IsOnline) && <span className="online-badge"></span>}
                </div>
                <h1 className="profile-name">{user.fullName || user.FullName}</h1>
                <p className="profile-username">@{user.username || user.Username}</p>
                
                {/* Rating */}
                <div className="profile-rating">
                  <span className="star">★</span>
                  <span className="rating-value">{user.rating || user.Rating || 0}</span>
                  <span className="rating-count">({user.reviewCount || user.ReviewCount || 0} reviews)</span>
                </div>

                {/* Badges */}
                <div className="profile-badges">
                  {(user.rating || user.Rating || 0) >= 4.7 && (
                    <span className="badge top-rated">⭐ Top Rated</span>
                  )}
                  {(user.completedOrders || user.CompletedOrders || 0) >= 100 && (
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
                  <span className="info-value">{user.country || user.Country || 'N/A'}</span>
                </div>
                {user.memberSince || user.MemberSince ? (
                  <div className="info-item">
                    <span className="info-label">Member since</span>
                    <span className="info-value">
                      {new Date(user.memberSince || user.MemberSince).toLocaleDateString('en-US', { 
                        month: 'short', 
                        year: 'numeric' 
                      })}
                    </span>
                  </div>
                ) : null}
                <div className="info-item">
                  <span className="info-label">Avg. response time</span>
                  <span className="info-value">1 hour</span>
                </div>
              </div>

              {/* Description */}
              {(user.bio || user.Bio) && (
                <div className="profile-description">
                  <h3>Description</h3>
                  <p>{user.bio || user.Bio}</p>
                </div>
              )}

              {/* Languages */}
              {((user.languages || user.Languages) && (user.languages || user.Languages).length > 0) && (
                <div className="profile-languages">
                  <h3>Languages</h3>
                  <ul>
                    {(user.languages || user.Languages || []).map((lang, idx) => (
                      <li key={idx}>
                        <span className="lang-name">{lang.name || lang.Name}</span>
                        <span className="lang-level">{lang.level || lang.Level || lang.Proficiency}</span>
                      </li>
                    ))}
                  </ul>
                </div>
              )}

              {/* Skills */}
              {((user.skills || user.Skills) && (user.skills || user.Skills).length > 0) && (
                <div className="profile-skills">
                  <h3>Skills</h3>
                  <div className="skills-list">
                    {(user.skills || user.Skills || []).map((skill, idx) => (
                      <span key={idx} className="skill-tag">
                        {typeof skill === 'string' ? skill : (skill.skillName || skill.SkillName || skill)}
                      </span>
                    ))}
                  </div>
                </div>
              )}

              {/* Stats */}
              <div className="profile-stats">
                <div className="stat-box">
                  <span className="stat-value">{user.completedOrders || user.CompletedOrders || 0}</span>
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
                Reviews ({user.reviewCount || user.ReviewCount || userReviews.length})
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
                      <span className="big-rating">{user.rating || user.Rating || 0}</span>
                      <div className="rating-stars">
                        {'★'.repeat(Math.round(user.rating || user.Rating || 0))}
                        {'☆'.repeat(5 - Math.round(user.rating || user.Rating || 0))}
                      </div>
                      <span className="total-reviews">{user.reviewCount || user.ReviewCount || userReviews.length} reviews</span>
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
                    {userReviews.length > 0 ? (
                      userReviews.map((review, idx) => {
                        const reviewRating = review.rating || review.Rating || 0;
                        const buyer = review.buyer || review.Buyer || {};
                        return (
                          <div key={review.id || review.Id || idx} className="review-item">
                            <div className="review-header">
                              <img 
                                src={buyer.avatarUrl || buyer.AvatarUrl || 'https://via.placeholder.com/40?text=U'} 
                                alt={buyer.fullName || buyer.FullName || buyer.username || buyer.Username || 'Buyer'}
                                className="reviewer-avatar"
                                onError={(e) => {
                                  e.target.src = 'https://via.placeholder.com/40?text=U';
                                }}
                              />
                              <div className="reviewer-info">
                                <span className="reviewer-name">{buyer.fullName || buyer.FullName || buyer.username || buyer.Username || 'Buyer'}</span>
                                {buyer.country || buyer.Country ? (
                                  <span className="reviewer-country">{buyer.country || buyer.Country}</span>
                                ) : null}
                              </div>
                              <div className="review-meta">
                                <div className="review-stars">
                                  {'★'.repeat(reviewRating)}
                                  {'☆'.repeat(5 - reviewRating)}
                                </div>
                                <span className="review-date">
                                  {new Date(review.createdAt || review.CreatedAt || new Date()).toLocaleDateString('en-US', {
                                    month: 'short',
                                    day: 'numeric',
                                    year: 'numeric'
                                  })}
                                </span>
                              </div>
                            </div>
                            <p className="review-text">{review.comment || review.Comment}</p>
                            {(review.sellerResponse || review.SellerResponse) && (
                              <div className="seller-response">
                                <span className="response-label">Seller's Response</span>
                                <p>{review.sellerResponse || review.SellerResponse}</p>
                              </div>
                            )}
                          </div>
                        );
                      })
                    ) : (
                      <div className="empty-state">
                        <p>No reviews yet.</p>
                      </div>
                    )}
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
                <h3>Contact {user.username || user.Username}</h3>
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
