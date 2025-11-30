import { useState } from 'react';
import { Link } from 'react-router-dom';
import './GigCard.css';

function GigCard({ gig }) {
  const [isLiked, setIsLiked] = useState(false);
  const [currentImage, setCurrentImage] = useState(0);

  const handleLike = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setIsLiked(!isLiked);
  };

  const nextImage = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setCurrentImage((prev) => (prev + 1) % gig.images.length);
  };

  const prevImage = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setCurrentImage((prev) => (prev - 1 + gig.images.length) % gig.images.length);
  };

  return (
    <Link to={`/gig/${gig.slug}`} className="gig-card">
      {/* Image Slider */}
      <div className="gig-card-image">
        <img 
          src={gig.images[currentImage]} 
          alt={gig.title}
          onError={(e) => {
            e.target.src = 'https://via.placeholder.com/300x200?text=IsLink+Gig';
          }}
        />
        
        {/* Navigation arrows for multiple images */}
        {gig.images.length > 1 && (
          <>
            <button className="image-nav image-nav-prev" onClick={prevImage}>
              ‹
            </button>
            <button className="image-nav image-nav-next" onClick={nextImage}>
              ›
            </button>
            <div className="image-dots">
              {gig.images.map((_, index) => (
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
            src={gig.seller?.avatar} 
            alt={gig.seller?.username}
            className="seller-avatar"
            onError={(e) => {
              e.target.src = 'https://via.placeholder.com/40x40?text=U';
            }}
          />
          <div className="seller-info">
            <span className="seller-name">{gig.seller?.username}</span>
            {gig.seller?.rating >= 4.7 && (
              <span className="seller-badge">Top Rated</span>
            )}
          </div>
        </div>

        {/* Gig Title */}
        <h3 className="gig-title">{gig.title}</h3>

        {/* Rating */}
        <div className="gig-rating">
          <span className="star">★</span>
          <span className="rating-value">{gig.rating.toFixed(1)}</span>
          <span className="rating-count">({gig.reviewCount})</span>
        </div>
      </div>

      {/* Card Footer */}
      <div className="gig-card-footer">
        <span className="starting-at">Starting at</span>
        <span className="gig-price">${gig.packages.basic.price}</span>
      </div>
    </Link>
  );
}

export default GigCard;
