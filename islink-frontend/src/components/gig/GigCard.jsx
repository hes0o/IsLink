import { useState } from 'react';
import { Link } from 'react-router-dom';
import './GigCard.css';

function GigCard({ gig }) {
  const [isLiked, setIsLiked] = useState(false);
  const [currentImage, setCurrentImage] = useState(0);

  // Normalize gig data to handle both camelCase and PascalCase from API
  const normalizedGig = {
    id: gig.id || gig.Id,
    slug: gig.slug || gig.Slug,
    title: gig.title || gig.Title,
    images: gig.images || gig.Images || [],
    rating: gig.rating || gig.Rating || 0,
    reviewCount: gig.reviewCount || gig.ReviewCount || 0,
    seller: {
      username: gig.seller?.username || gig.seller?.Username || gig.Seller?.username || gig.Seller?.Username,
      avatar: gig.seller?.avatar || gig.seller?.avatarUrl || gig.seller?.AvatarUrl || gig.Seller?.avatar || gig.Seller?.avatarUrl || gig.Seller?.AvatarUrl,
      rating: gig.seller?.rating || gig.seller?.Rating || gig.Seller?.rating || gig.Seller?.Rating || 0
    },
    packages: {
      basic: {
        price: gig.packages?.basic?.price || gig.packages?.Basic?.price || gig.packages?.Basic?.Price || gig.Packages?.basic?.price || gig.Packages?.Basic?.price || gig.Packages?.Basic?.Price || 0
      }
    }
  };

  const images = normalizedGig.images || [];
  const firstImage = images.length > 0 ? images[0] : 'https://via.placeholder.com/300x200?text=IsLink+Gig';

  const handleLike = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setIsLiked(!isLiked);
  };

  const nextImage = (e) => {
    e.preventDefault();
    e.stopPropagation();
    if (images.length > 1) {
      setCurrentImage((prev) => (prev + 1) % images.length);
    }
  };

  const prevImage = (e) => {
    e.preventDefault();
    e.stopPropagation();
    if (images.length > 1) {
      setCurrentImage((prev) => (prev - 1 + images.length) % images.length);
    }
  };

  return (
    <Link to={`/gig/${normalizedGig.slug}`} className="gig-card">
      {/* Image Slider */}
      <div className="gig-card-image">
        <img 
          src={images[currentImage] || firstImage} 
          alt={normalizedGig.title}
          onError={(e) => {
            e.target.src = 'https://via.placeholder.com/300x200?text=IsLink+Gig';
          }}
        />
        
        {/* Navigation arrows for multiple images */}
        {images.length > 1 && (
          <>
            <button className="image-nav image-nav-prev" onClick={prevImage}>
              ‹
            </button>
            <button className="image-nav image-nav-next" onClick={nextImage}>
              ›
            </button>
            <div className="image-dots">
              {images.map((_, index) => (
                <span 
                  key={index} 
                  className={`dot ${index === currentImage ? 'active' : ''}`}
                />
              ))}
            </div>
          </>
        )}

        {/* Like button */}
        <button 
          className={`like-btn ${isLiked ? 'liked' : ''}`}
          onClick={handleLike}
          aria-label={isLiked ? 'Unlike' : 'Like'}
        >
          <svg viewBox="0 0 24 24" fill={isLiked ? 'currentColor' : 'none'} stroke="currentColor" strokeWidth="2">
            <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
          </svg>
        </button>
      </div>

      {/* Card Body */}
      <div className="gig-card-body">
        {/* Seller Info */}
        <div className="gig-seller">
          <img 
            src={normalizedGig.seller.avatar || 'https://via.placeholder.com/40x40?text=U'} 
            alt={normalizedGig.seller.username}
            className="seller-avatar"
            onError={(e) => {
              e.target.src = 'https://via.placeholder.com/40x40?text=U';
            }}
          />
          <div className="seller-info">
            <span className="seller-name">{normalizedGig.seller.username || 'Unknown'}</span>
            {normalizedGig.seller.rating >= 4.7 && (
              <span className="seller-badge">Top Rated</span>
            )}
          </div>
        </div>

        {/* Gig Title */}
        <h3 className="gig-title">{normalizedGig.title}</h3>

        {/* Rating */}
        <div className="gig-rating">
          <span className="star">★</span>
          <span className="rating-value">{normalizedGig.rating.toFixed(1)}</span>
          <span className="rating-count">({normalizedGig.reviewCount})</span>
        </div>
      </div>

      {/* Card Footer */}
      <div className="gig-card-footer">
        <span className="starting-at">Starting at</span>
        <span className="gig-price">${normalizedGig.packages.basic.price.toFixed(2)}</span>
      </div>
    </Link>
  );
}

export default GigCard;
