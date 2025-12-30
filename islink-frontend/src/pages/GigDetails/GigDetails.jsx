import { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context';
import { gigsAPI, ordersAPI, messagesAPI } from '../../services/api';
import './GigDetails.css';

// Helper for image URLs (consistent with LinkerAI)
const getImageUrl = (path) => {
  if (!path) return 'https://placehold.co/600x400?text=No+Image';
  if (path.startsWith('http')) return path;
  return `http://localhost:5001${path}`;
};

function GigDetails() {
  const { slug } = useParams();
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  const [gig, setGig] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [selectedPackage, setSelectedPackage] = useState('basic'); // 'basic', 'standard', 'premium'
  const [currentImage, setCurrentImage] = useState(0);
  const [showContactModal, setShowContactModal] = useState(false);
  const [contactMessage, setContactMessage] = useState('');
  const [orderProcessing, setOrderProcessing] = useState(false);

  // Fetch Gig Data
  useEffect(() => {
    const fetchGig = async () => {
      try {
        setLoading(true);
        const response = await gigsAPI.getBySlug(slug);
        if (response.success && response.data) {
          setGig(response.data);
        } else {
          setError('Gig not found');
        }
      } catch (err) {
        console.error("Error fetching gig:", err);
        setError(err.message || 'Failed to load gig details');
      } finally {
        setLoading(false);
      }
    };

    if (slug) fetchGig();
  }, [slug]);

  const handleOrder = async () => {
    if (!isAuthenticated) {
      navigate('/auth/login', { state: { from: `/gig/${slug}` } });
      return;
    }

    try {
      setOrderProcessing(true);
      // Backend expects: GigId, PackageType, Requirements
      const orderData = {
        gigId: gig.id,
        packageType: selectedPackage,
        requirements: `Order for ${selectedPackage} package` // Simple default for now
      };

      const response = await ordersAPI.create(orderData);

      if (response.success) {
        alert('🎉 Order placed successfully!');
        navigate('/dashboard?tab=orders'); // Redirect to dashboard orders tab
      } else {
        throw new Error(response.message || 'Failed to place order');
      }
    } catch (err) {
      alert(`Error placing order: ${err.message}`);
    } finally {
      setOrderProcessing(false);
    }
  };

  if (loading) {
    return (
      <div className="gig-details-page">
        <div className="container" style={{ padding: '4rem', textAlign: 'center' }}>
          <div className="loading-spinner"></div>
          <p>Loading service details...</p>
        </div>
      </div>
    );
  }

  if (error || !gig) {
    return (
      <div className="gig-details-page">
        <div className="container" style={{ padding: '4rem', textAlign: 'center' }}>
          <h2>Service not found</h2>
          <p>{error}</p>
          <Link to="/gigs" className="btn-primary" style={{ marginTop: '1rem', display: 'inline-block' }}>
            Browse All Services
          </Link>
        </div>
      </div>
    );
  }

  // Determine current package data safely
  // API returns packages as { basic: {...}, standard: {...}, premium: {...} }
  // keys in 'packages' object match our state 'basic', 'standard', 'premium' (lowercase)
  // HOWEVER, backend DTO properties are PascalCase (Basic, Standard, Premium) usually, 
  // but JS JSON usually camelCases them. Let's check api.js or DTO.
  // api.js response.json() will have properties as sent by backend default formatter.
  // .NET 8 WebAPI defaults to camelCase. So 'packages' should have 'basic', 'standard'.

  const packagesData = gig.packages || {};
  const currentPkg = packagesData[selectedPackage] || {};

  // Package mapping for display
  const packageList = [
    { key: 'basic', name: packagesData.basic?.name || 'Basic' },
    { key: 'standard', name: packagesData.standard?.name || 'Standard' },
    { key: 'premium', name: packagesData.premium?.name || 'Premium' }
  ];

  return (
    <div className="gig-details-page">
      {/* Breadcrumb */}
      <div className="breadcrumb">
        <div className="container">
          <Link to="/">Home</Link>
          <span className="separator">/</span>
          <Link to={`/gigs?category=${gig.category?.slug}`}>{gig.category?.name || 'Category'}</Link>
          <span className="separator">/</span>
          <span className="current">{gig.title}</span>
        </div>
      </div>

      <div className="gig-details-content">
        <div className="container">
          <div className="gig-layout">
            {/* Main Content */}
            <div className="gig-main">
              <h1 className="gig-title">{gig.title}</h1>

              {/* Seller Info */}
              <div className="seller-bar">
                <Link to={`/profile/${gig.seller?.username}`} className="seller-info">
                  <img
                    src={getImageUrl(gig.seller?.avatarUrl)}
                    alt={gig.seller?.username}
                    className="seller-avatar"
                    onError={(e) => { e.target.src = 'https://placehold.co/40x40?text=U'; }}
                  />
                  <div className="seller-details">
                    <span className="seller-name">{gig.seller?.username}</span>
                    <span className="seller-badge">Level 2 Seller</span>
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

              {/* Gallery */}
              <div className="image-gallery">
                <div className="main-image">
                  <img
                    src={getImageUrl(gig.images && gig.images[currentImage])}
                    alt={gig.title}
                    onError={(e) => { e.target.src = 'https://placehold.co/800x500?text=Full+Image'; }}
                  />
                </div>
                {gig.images && gig.images.length > 1 && (
                  <div className="thumbnail-list">
                    {gig.images.map((img, idx) => (
                      <button
                        key={idx}
                        className={`thumbnail ${idx === currentImage ? 'active' : ''}`}
                        onClick={() => setCurrentImage(idx)}
                      >
                        <img
                          src={getImageUrl(img)}
                          alt={`Thumbnail ${idx}`}
                          onError={(e) => { e.target.src = 'https://placehold.co/100x100?text=Img'; }}
                        />
                      </button>
                    ))}
                  </div>
                )}
              </div>

              {/* Description */}
              <section className="gig-section">
                <h2>About This Gig</h2>
                <div className="description" style={{ whiteSpace: 'pre-line' }}>
                  {gig.description}
                </div>
              </section>

              {/* About Seller (Simplified for now) */}
              <section className="gig-section">
                <h2>About The Seller</h2>
                <div className="seller-card">
                  <Link to={`/profile/${gig.seller?.username}`} className="seller-profile">
                    <img
                      src={getImageUrl(gig.seller?.avatarUrl)}
                      alt={gig.seller?.username}
                      className="seller-avatar-lg"
                      onError={(e) => { e.target.src = 'https://placehold.co/80x80?text=Available'; }}
                    />
                    <div className="seller-info-detailed">
                      <h3>{gig.seller?.fullName || gig.seller?.username}</h3>
                      <p className="seller-title">{gig.seller?.bio?.substring(0, 60)}...</p>
                    </div>
                  </Link>
                  <button className="btn-contact" onClick={() => setShowContactModal(true)}>Contact Me</button>
                </div>
              </section>

            </div>

            {/* Sidebar - Packages */}
            <div className="gig-sidebar">
              <div className="packages-card">
                {/* Tabs */}
                <div className="package-tabs">
                  {packageList.map(pkg => (
                    <button
                      key={pkg.key}
                      className={`tab ${selectedPackage === pkg.key ? 'active' : ''}`}
                      onClick={() => setSelectedPackage(pkg.key)}
                    >
                      {pkg.name}
                    </button>
                  ))}
                </div>

                {/* Content */}
                <div className="package-content">
                  <div className="package-header">
                    <h3>{currentPkg.name || selectedPackage}</h3>
                    <span className="package-price">${currentPkg.price}</span>
                  </div>

                  <p className="package-description">{currentPkg.description}</p>

                  <div className="package-meta">
                    <div className="meta-item">
                      <span>⏱ {currentPkg.deliveryDays} Days Delivery</span>
                    </div>
                    <div className="meta-item">
                      <span>🔄 {currentPkg.revisions} Revisions</span>
                    </div>
                  </div>

                  <ul className="package-features">
                    {(currentPkg.features || []).map((feature, idx) => (
                      <li key={idx}>✓ {feature}</li>
                    ))}
                  </ul>

                  <button
                    className="btn-order"
                    onClick={handleOrder}
                    disabled={orderProcessing}
                  >
                    {orderProcessing ? 'Processing...' : `Continue ($${currentPkg.price})`}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Contact Modal (Simplified) */}
      {showContactModal && (
        <div className="modal-overlay" onClick={() => setShowContactModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <button className="modal-close" onClick={() => setShowContactModal(false)}>×</button>
            {isAuthenticated ? (
              <>
                <h3>Contact {gig.seller?.username}</h3>
                <textarea
                  placeholder="Hi! I'm interested in your service..."
                  rows={5}
                  value={contactMessage}
                  onChange={(e) => setContactMessage(e.target.value)}
                  className="modal-textarea"
                ></textarea>
                <div className="modal-actions">
                  <button
                    className="btn-primary"
                    onClick={async () => {
                      try {
                        // 1. Get or Create Conversation
                        // The gig object has seller object, check for ID
                        const participantId = gig.seller?.id || gig.seller?.Id;
                        if (!participantId) {
                          alert("Error: Cannot contact seller (Missing ID)");
                          return;
                        }

                        const convResponse = await messagesAPI.getOrCreateConversation({
                          participantId: participantId,
                          relatedGig: gig.id
                        });

                        const conversationData = convResponse?.data || convResponse?.Data || convResponse;
                        const conversationId = conversationData?.id || conversationData?.Id;

                        if (!conversationId) {
                          throw new Error("Failed to start conversation");
                        }

                        // 2. Send Message
                        await messagesAPI.sendMessage(conversationId, {
                          content: contactMessage,
                          messageType: 'text'
                        });

                        setShowContactModal(false);
                        setContactMessage('');
                        navigate(`/messages/${conversationId}`);
                      } catch (err) {
                        console.error("Failed to send message:", err);
                        alert(`Error: ${err.message || "Failed to send message"}`);
                      }
                    }}
                    disabled={!contactMessage.trim()}
                  >
                    Send Message
                  </button>
                  <button className="btn-secondary" onClick={() => setShowContactModal(false)}>Cancel</button>
                </div>
              </>
            ) : (
              <>
                <h3>Please Log In</h3>
                <p>You need to be logged in to contact sellers.</p>
                <div className="modal-actions">
                  <Link to="/auth/login" state={{ from: `/gig/${slug}` }} className="btn-primary">Sign In</Link>
                  <button className="btn-secondary" onClick={() => setShowContactModal(false)}>Cancel</button>
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
