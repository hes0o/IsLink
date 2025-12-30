import { useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context';
import './Header.css';

function Header() {
  const [searchQuery, setSearchQuery] = useState('');
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [showUserMenu, setShowUserMenu] = useState(false);
  const navigate = useNavigate();
  const location = useLocation(); // Hook for checking current page
  const { user, isAuthenticated, logout } = useAuth();

  const isHomePage = location.pathname === '/';

  const handleSearch = (e) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      navigate(`/gigs?search=${encodeURIComponent(searchQuery)}`);
    }
  };

  const handleLogout = async () => {
    await logout();
    setShowUserMenu(false);
    navigate('/');
  };

  return (
    <>
      <header className={`header ${isHomePage ? 'header-transparent' : ''}`}>
        <div className="header-container">
          {/* Logo */}
          <Link to="/" className="logo">
            <div className="logo-icon">
              <svg viewBox="0 0 40 40" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect width="40" height="40" rx="10" fill="url(#logoGradient)" />
                <path d="M12 20C12 16 14 14 18 14C20 14 21.5 15 22 16" stroke="white" strokeWidth="2.5" strokeLinecap="round" />
                <path d="M28 20C28 24 26 26 22 26C20 26 18.5 25 18 24" stroke="white" strokeWidth="2.5" strokeLinecap="round" />
                <circle cx="18" cy="14" r="2" fill="white" />
                <circle cx="22" cy="26" r="2" fill="white" />
                <defs>
                  <linearGradient id="logoGradient" x1="0" y1="0" x2="40" y2="40">
                    <stop stopColor="#1dbf73" />
                    <stop offset="1" stopColor="#14a35d" />
                  </linearGradient>
                </defs>
              </svg>
            </div>
            <span className="logo-text">IsLink</span>
          </Link>

          {/* Search Bar (Hidden on Home hero, visible elsewhere or scrolling) */}
          <form className={`search-bar ${isHomePage ? 'hidden-on-desktop' : ''}`} onSubmit={handleSearch}>
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
            <Link to="/linkerai" className="nav-link special-link">🤖 LinkerAI</Link>

            {isAuthenticated ? (
              <>
                <Link to="/dashboard" className="nav-link">Dashboard</Link>
                <Link to="/messages" className="nav-link">Messages</Link>

                {/* User Menu */}
                <div className="user-menu-container">
                  <button
                    className="user-menu-trigger"
                    onClick={() => setShowUserMenu(!showUserMenu)}
                  >
                    <img
                      src={user?.avatarUrl || 'https://via.placeholder.com/40?text=U'}
                      alt={user?.username}
                      className="user-avatar"
                    />
                    <span className="user-name">{user?.username || user?.Username}</span>
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="dropdown-icon">
                      <path d="M6 9l6 6 6-6" />
                    </svg>
                  </button>

                  {showUserMenu && (
                    <div className="user-dropdown">
                      <Link to={`/profile/${user?.username || user?.Username}`} onClick={() => setShowUserMenu(false)}>
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                          <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                          <circle cx="12" cy="7" r="4" />
                        </svg>
                        Profile
                      </Link>
                      <Link to="/dashboard" onClick={() => setShowUserMenu(false)}>
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                          <rect x="3" y="3" width="7" height="7" />
                          <rect x="14" y="3" width="7" height="7" />
                          <rect x="14" y="14" width="7" height="7" />
                          <rect x="3" y="14" width="7" height="7" />
                        </svg>
                        Dashboard
                      </Link>
                      {user?.role === 'seller' && (
                        <Link to="/dashboard?tab=gigs" onClick={() => setShowUserMenu(false)}>
                          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                            <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z" />
                          </svg>
                          My Gigs
                        </Link>
                      )}
                      <div className="dropdown-divider"></div>
                      <button onClick={handleLogout} className="logout-btn">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                          <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
                          <polyline points="16 17 21 12 16 7" />
                          <line x1="21" y1="12" x2="9" y2="12" />
                        </svg>
                        Logout
                      </button>
                    </div>
                  )}
                </div>
              </>
            ) : (
              <>
                <Link to="/gigs" className="nav-link">Become a Seller</Link>
                <div className="nav-divider"></div>
                <Link to="/auth/login" className="nav-link">Sign In</Link>
                <Link to="/auth/register" className="btn btn-primary">Join</Link>
              </>
            )}
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
      </header>

      {/* Categories Bar - ONLY ON HOME PAGE */}
      {isHomePage && (
        <div className="categories-wrapper">
          <div className="categories-bar glass-bar">
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
        </div>
      )}
    </>
  );
}

export default Header;
