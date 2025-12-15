import { useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context';
import { gigs, reviews, categories } from '../../data/mockData';
import './GigDetails.css';

function GigDetails() {
  const { slug } = useParams();
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const [selectedPackage, setSelectedPackage] = useState('basic');
  const [currentImage, setCurrentImage] = useState(0);
  const [showContactModal, setShowContactModal] = useState(false);
  const [contactMessage, setContactMessage] = useState('');

  // Find the gig
  const gig = gigs.find(g => g.slug === slug) || gigs[0];
  const category = categories.find(cat => cat.id === gig.categoryId);
  const gigReviews = reviews.filter(r => r.gigId === gig.id);

  const packages = [
    { key: 'basic', ...gig.packages.basic },
    { key: 'standard', ...gig.packages.standard },
    { key: 'premium', ...gig.packages.premium }
  ];

  const selectedPkg = gig.packages[selectedPackage];

  return (
    <div className="gig-details-page">
      {/* Breadcrumb */}
      <div className="breadcrumb">
        <div className="container">
          <Link to="/">Home</Link>
          <span className="separator">/</span>
          <Link to={`/gigs?category=${category?.slug}`}>{category?.name}</Link>
          <span className="separator">/</span>
          <span className="current">{gig.title}</span>
        </div>
      </div>

      <div className="gig-details-content">
        <div className="container">
          <div className="gig-layout">
            {/* Main Content */}
            <div className="gig-main">
              {/* Title */}
              <h1 className="gig-title">{gig.title}</h1>

              {/* Seller Info Bar */}
              <div className="seller-bar">
                <Link to={`/profile/${gig.seller?.username}`} className="seller-info">
                  <img 
                    src={gig.seller?.avatarUrl || gig.seller?.AvatarUrl || 'https://via.placeholder.com/40?text=U'} 
                    alt={gig.seller?.username || gig.seller?.Username}
                    className="seller-avatar"
                    onError={(e) => {
                      e.target.src = 'https://via.placeholder.com/40?text=U';
                    }}
                  />
                  <div className="seller-details">
                    <span className="seller-name">{gig.seller?.username}</span>
                    {gig.seller?.rating >= 4.7 && (
                      <span className="seller-badge">Top Rated</span>
                    )}
                  </div>
                </Link>
                <div className="gig-stats">
                  <div className="stat">
                    <span className="star">★</span>
                    <span className="value">{gig.rating}</span>
                    <span className="label">({gig.reviewCount})</span>
                  </div>
                  <div className="stat">
                    <span className="value">{gig.ordersInQueue}</span>
                    <span className="label">Orders in Queue</span>
                  </div>
                </div>
              </div>

              {/* Image Gallery */}
              <div className="image-gallery">
                <div className="main-image">
                  <img 
                    src={gig.images[currentImage]} 
                    alt={gig.title}
                    onError={(e) => {
                      e.target.src = 'https://via.placeholder.com/800x500?text=IsLink+Gig';
                    }}
                  />
                </div>
                {gig.images.length > 1 && (
                  <div className="thumbnail-list">
                    {gig.images.map((img, idx) => (
                      <button
                        key={idx}
                        className={`thumbnail ${idx === currentImage ? 'active' : ''}`}
                        onClick={() => setCurrentImage(idx)}
                      >
                        <img src={img} alt={`Preview ${idx + 1}`} />
                      </button>
                    ))}
                  </div>
                )}
              </div>

              {/* Description */}
              <section className="gig-section">
                <h2>About This Gig</h2>
                <div className="description">
                  <p>{gig.description}</p>
                  <p>
                    With years of experience and hundreds of satisfied clients, I deliver 
                    high-quality work that exceeds expectations. My process includes:
                  </p>
                  <ul>
                    <li>Initial consultation to understand your needs</li>
                    <li>Research and concept development</li>
                    <li>First draft delivery within timeline</li>
                    <li>Revisions based on your feedback</li>
                    <li>Final delivery with all source files</li>
                  </ul>
                </div>
              </section>

              {/* About Seller */}
              <section className="gig-section">
                <h2>About The Seller</h2>
                <div className="seller-card">
                  <Link to={`/profile/${gig.seller?.username}`} className="seller-profile">
                    <img 
                      src={gig.seller?.avatarUrl || gig.seller?.AvatarUrl || 'https://via.placeholder.com/80?text=U'} 
                      alt={gig.seller?.username || gig.seller?.Username}
                      className="seller-avatar-lg"
                      onError={(e) => {
                        e.target.src = 'https://via.placeholder.com/80?text=U';
                      }}
                    />
                    <div className="seller-info-detailed">
                      <h3>{gig.seller?.fullName}</h3>
                      <p className="seller-title">{gig.seller?.skills?.slice(0, 2).join(' | ')}</p>
                      <div className="seller-rating">
                        <span className="star">★</span>
                        <span>{gig.seller?.rating}</span>
                        <span className="count">({gig.seller?.reviewCount})</span>
                      </div>
                    </div>
                  </Link>
                  <button 
                    className="btn-contact"
                    onClick={() => setShowContactModal(true)}
                  >
                    Contact Me
                  </button>
                </div>
                <div className="seller-bio">
                  <p>{gig.seller?.bio}</p>
                </div>
                <div className="seller-stats-grid">
                  <div className="stat-item">
                    <span className="label">From</span>
                    <span className="value">{gig.seller?.country}</span>
                  </div>
                  <div className="stat-item">
                    <span className="label">Member since</span>
                    <span className="value">{new Date(gig.seller?.memberSince).toLocaleDateString('en-US', { month: 'short', year: 'numeric' })}</span>
                  </div>
                  <div className="stat-item">
                    <span className="label">Avg. response time</span>
                    <span className="value">1 hour</span>
                  </div>
                  <div className="stat-item">
                    <span className="label">Last delivery</span>
                    <span className="value">About 2 hours</span>
                  </div>
                </div>
              </section>

              {/* Reviews */}
              <section className="gig-section">
                <h2>Reviews</h2>
                <div className="reviews-summary">
                  <div className="rating-big">
                    <span className="number">{gig.rating}</span>
                    <div className="stars">
                      {'★'.repeat(Math.round(gig.rating))}
                      {'☆'.repeat(5 - Math.round(gig.rating))}
                    </div>
                    <span className="count">{gig.reviewCount} reviews</span>
                  </div>
                  <div className="rating-bars">
                    {[5, 4, 3, 2, 1].map(num => (
                      <div key={num} className="rating-bar">
                        <span className="label">{num} Stars</span>
                        <div className="bar">
                          <div 
                            className="fill" 
                            style={{ width: `${num === 5 ? 85 : num === 4 ? 12 : 3}%` }}
                          ></div>
                        </div>
                        <span className="percent">{num === 5 ? 85 : num === 4 ? 12 : 3}%</span>
                      </div>
                    ))}
                  </div>
                </div>

                <div className="reviews-list">
                  {reviews.slice(0, 3).map(review => (
                    <div key={review.id} className="review-card">
                      <div className="review-header">
                        <img 
                          src={review.buyer?.avatarUrl || review.buyer?.AvatarUrl || review.buyerAvatar || 'https://via.placeholder.com/40?text=U'} 
                          alt={review.buyer?.fullName || review.buyer?.FullName || review.buyerName || 'Buyer'}
                          className="reviewer-avatar"
                          onError={(e) => {
                            e.target.src = 'https://via.placeholder.com/40?text=U';
                          }}
                        />
                        <div className="reviewer-info">
                          <span className="reviewer-name">{review.buyerName}</span>
                          <span className="reviewer-country">{review.buyerCountry}</span>
                        </div>
                        <div className="review-rating">
                          <span className="star">★</span>
                          <span>{review.rating}</span>
                        </div>
                      </div>
                      <p className="review-comment">{review.comment}</p>
                      {review.sellerResponse && (
                        <div className="seller-response">
                          <span className="response-label">Seller's Response:</span>
                          <p>{review.sellerResponse}</p>
                        </div>
                      )}
                      <span className="review-date">
                        {new Date(review.createdAt).toLocaleDateString('en-US', { 
                          month: 'long', 
                          day: 'numeric', 
                          year: 'numeric' 
                        })}
                      </span>
                    </div>
                  ))}
                </div>
              </section>
            </div>

            {/* Sidebar - Packages */}
            <div className="gig-sidebar">
              <div className="packages-card">
                {/* Package Tabs */}
                <div className="package-tabs">
                  {packages.map(pkg => (
                    <button
                      key={pkg.key}
                      className={`tab ${selectedPackage === pkg.key ? 'active' : ''}`}
                      onClick={() => setSelectedPackage(pkg.key)}
                    >
                      {pkg.name}
                    </button>
                  ))}
                </div>

                {/* Package Details */}
                <div className="package-content">
                  <div className="package-header">
                    <h3>{selectedPkg.name}</h3>
                    <span className="package-price">${selectedPkg.price}</span>
                  </div>
                  
                  <p className="package-description">
                    {selectedPackage === 'basic' && 'Perfect for small projects and quick needs.'}
                    {selectedPackage === 'standard' && 'Great balance of features and value.'}
                    {selectedPackage === 'premium' && 'Complete package with all premium features.'}
                  </p>

                  <div className="package-meta">
                    <div className="meta-item">
                      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <circle cx="12" cy="12" r="10"/>
                        <path d="M12 6v6l4 2"/>
                      </svg>
                      <span>{selectedPkg.deliveryDays} Day{selectedPkg.deliveryDays > 1 ? 's' : ''} Delivery</span>
                    </div>
                    <div className="meta-item">
                      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <path d="M4 12v8a2 2 0 002 2h12a2 2 0 002-2v-8"/>
                        <polyline points="16,6 12,2 8,6"/>
                        <line x1="12" y1="2" x2="12" y2="15"/>
                      </svg>
                      <span>{selectedPkg.revisions} Revision{selectedPkg.revisions !== 1 ? 's' : ''}</span>
                    </div>
                  </div>

                  <ul className="package-features">
                    {selectedPkg.features.map((feature, idx) => (
                      <li key={idx}>
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                          <polyline points="20,6 9,17 4,12"/>
                        </svg>
                        {feature}
                      </li>
                    ))}
                  </ul>

                  <button 
                    className="btn-order"
                    onClick={() => {
                      if (!isAuthenticated) {
                        navigate('/auth/login');
                      } else {
                        alert(`Order placed for ${selectedPkg.name} package ($${selectedPkg.price})!\n\nThis will work when the database is connected.`);
                      }
                    }}
                  >
                    Continue (${selectedPkg.price})
                  </button>

                  <button className="btn-compare">
                    Compare Packages
                  </button>
                </div>
              </div>

              {/* Contact Seller */}
              <div className="contact-card">
                <button 
                  className="btn-contact-seller"
                  onClick={() => setShowContactModal(true)}
                >
                  Contact Seller
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Contact Modal */}
      {showContactModal && (
        <div className="modal-overlay" onClick={() => setShowContactModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <button className="modal-close" onClick={() => setShowContactModal(false)}>×</button>
            {isAuthenticated ? (
              <>
                <h3>Contact {gig.seller?.username}</h3>
                <textarea 
                  placeholder="Hi! I'm interested in your service. I would like to discuss..."
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

export default GigDetails;
