import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './Header.css';

function Header() {
  const [searchQuery, setSearchQuery] = useState('');
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const navigate = useNavigate();

  const handleSearch = (e) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      navigate(`/gigs?search=${encodeURIComponent(searchQuery)}`);
    }
  };

  return (
    <header className="header">
      <div className="header-container">
        {/* Logo */}
        <Link to="/" className="logo">
          <div className="logo-icon">
            <svg viewBox="0 0 40 40" fill="none" xmlns="http://www.w3.org/2000/svg">
              <rect width="40" height="40" rx="10" fill="url(#logoGradient)"/>
              <path d="M12 20C12 16 14 14 18 14C20 14 21.5 15 22 16" stroke="white" strokeWidth="2.5" strokeLinecap="round"/>
              <path d="M28 20C28 24 26 26 22 26C20 26 18.5 25 18 24" stroke="white" strokeWidth="2.5" strokeLinecap="round"/>
              <circle cx="18" cy="14" r="2" fill="white"/>
              <circle cx="22" cy="26" r="2" fill="white"/>
              <defs>
                <linearGradient id="logoGradient" x1="0" y1="0" x2="40" y2="40">
                  <stop stopColor="#1dbf73"/>
                  <stop offset="1" stopColor="#14a35d"/>
                </linearGradient>
              </defs>
            </svg>
          </div>
          <span className="logo-text">IsLink</span>
        </Link>

        {/* Search Bar */}
        <form className="search-bar" onSubmit={handleSearch}>
          <input
            type="text"
            placeholder="What service are you looking for?"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
          <button type="submit" className="search-btn">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <circle cx="11" cy="11" r="8" />
              <path d="M21 21l-4.35-4.35" />
            </svg>
          </button>
        </form>

        {/* Navigation */}
        <nav className={`nav ${isMenuOpen ? 'nav-open' : ''}`}>
          <Link to="/gigs" className="nav-link">Explore</Link>
          <Link to="/gigs" className="nav-link">Become a Seller</Link>
          <div className="nav-divider"></div>
          <Link to="/auth/login" className="nav-link">Sign In</Link>
          <Link to="/auth/register" className="btn btn-primary">Join</Link>
        </nav>

        {/* Mobile Menu Toggle */}
        <button 
          className="menu-toggle"
          onClick={() => setIsMenuOpen(!isMenuOpen)}
          aria-label="Toggle menu"
        >
          <span></span>
          <span></span>
          <span></span>
        </button>
      </div>

      {/* Categories Bar */}
      <div className="categories-bar">
        <div className="categories-container">
          <Link to="/gigs?category=graphics-design">Graphics & Design</Link>
          <Link to="/gigs?category=programming-tech">Programming & Tech</Link>
          <Link to="/gigs?category=digital-marketing">Digital Marketing</Link>
          <Link to="/gigs?category=writing-translation">Writing & Translation</Link>
          <Link to="/gigs?category=video-animation">Video & Animation</Link>
          <Link to="/gigs?category=music-audio">Music & Audio</Link>
          <Link to="/gigs?category=business">Business</Link>
          <Link to="/gigs?category=ai-services">AI Services</Link>
        </div>
      </div>
    </header>
  );
}

export default Header;
