import { Link } from 'react-router-dom';
import './Footer.css';

function Footer() {
  // For pages that don't exist yet, show alert
  const handlePlaceholderClick = (e, pageName) => {
    e.preventDefault();
    alert(`"${pageName}" page coming soon!`);
  };

  return (
    <footer className="footer">
      <div className="footer-container">
        {/* Footer Main */}
        <div className="footer-main">
          {/* Categories */}
          <div className="footer-section">
            <h4>Categories</h4>
            <ul>
              <li><Link to="/gigs?category=graphics-design">Graphics & Design</Link></li>
              <li><Link to="/gigs?category=programming-tech">Programming & Tech</Link></li>
              <li><Link to="/gigs?category=digital-marketing">Digital Marketing</Link></li>
              <li><Link to="/gigs?category=writing-translation">Writing & Translation</Link></li>
              <li><Link to="/gigs?category=video-animation">Video & Animation</Link></li>
            </ul>
          </div>

          {/* About */}
          <div className="footer-section">
            <h4>About</h4>
            <ul>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'About IsLink')}>About IsLink</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Careers')}>Careers</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Press & News')}>Press & News</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Partnerships')}>Partnerships</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Privacy Policy')}>Privacy Policy</a></li>
            </ul>
          </div>

          {/* Support */}
          <div className="footer-section">
            <h4>Support</h4>
            <ul>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Help & Support')}>Help & Support</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Trust & Safety')}>Trust & Safety</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Selling on IsLink')}>Selling on IsLink</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Buying on IsLink')}>Buying on IsLink</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Contact Us')}>Contact Us</a></li>
            </ul>
          </div>

          {/* Community */}
          <div className="footer-section">
            <h4>Community</h4>
            <ul>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Events')}>Events</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Blog')}>Blog</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Forum')}>Forum</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Podcast')}>Podcast</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'Affiliates')}>Affiliates</a></li>
            </ul>
          </div>

          {/* More From IsLink */}
          <div className="footer-section">
            <h4>More From IsLink</h4>
            <ul>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'IsLink Business')}>IsLink Business</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'IsLink Pro')}>IsLink Pro</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'IsLink Studios')}>IsLink Studios</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'IsLink Guides')}>IsLink Guides</a></li>
              <li><a href="#" onClick={(e) => handlePlaceholderClick(e, 'IsLink Workspace')}>IsLink Workspace</a></li>
            </ul>
          </div>
        </div>

        {/* Footer Bottom */}
        <div className="footer-bottom">
          <div className="footer-brand">
            <Link to="/" className="footer-logo">
              <svg viewBox="0 0 40 40" fill="none" xmlns="http://www.w3.org/2000/svg" className="footer-logo-icon">
                <rect width="40" height="40" rx="10" fill="url(#footerLogoGradient)"/>
                <path d="M12 20C12 16 14 14 18 14C20 14 21.5 15 22 16" stroke="white" strokeWidth="2.5" strokeLinecap="round"/>
                <path d="M28 20C28 24 26 26 22 26C20 26 18.5 25 18 24" stroke="white" strokeWidth="2.5" strokeLinecap="round"/>
                <circle cx="18" cy="14" r="2" fill="white"/>
                <circle cx="22" cy="26" r="2" fill="white"/>
                <defs>
                  <linearGradient id="footerLogoGradient" x1="0" y1="0" x2="40" y2="40">
                    <stop stopColor="#1dbf73"/>
                    <stop offset="1" stopColor="#14a35d"/>
                  </linearGradient>
                </defs>
              </svg>
              <span className="logo-text">IsLink</span>
            </Link>
            <p className="copyright">© IsLink International Ltd. 2024</p>
          </div>

          <div className="footer-social">
            <a href="https://twitter.com" target="_blank" rel="noopener noreferrer" aria-label="Twitter">
              <svg viewBox="0 0 24 24" fill="currentColor">
                <path d="M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z"/>
              </svg>
            </a>
            <a href="https://facebook.com" target="_blank" rel="noopener noreferrer" aria-label="Facebook">
              <svg viewBox="0 0 24 24" fill="currentColor">
                <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z"/>
              </svg>
            </a>
            <a href="https://linkedin.com" target="_blank" rel="noopener noreferrer" aria-label="LinkedIn">
              <svg viewBox="0 0 24 24" fill="currentColor">
                <path d="M20.447 20.452h-3.554v-5.569c0-1.328-.027-3.037-1.852-3.037-1.853 0-2.136 1.445-2.136 2.939v5.667H9.351V9h3.414v1.561h.046c.477-.9 1.637-1.85 3.37-1.85 3.601 0 4.267 2.37 4.267 5.455v6.286zM5.337 7.433c-1.144 0-2.063-.926-2.063-2.065 0-1.138.92-2.063 2.063-2.063 1.14 0 2.064.925 2.064 2.063 0 1.139-.925 2.065-2.064 2.065zm1.782 13.019H3.555V9h3.564v11.452zM22.225 0H1.771C.792 0 0 .774 0 1.729v20.542C0 23.227.792 24 1.771 24h20.451C23.2 24 24 23.227 24 22.271V1.729C24 .774 23.2 0 22.222 0h.003z"/>
              </svg>
            </a>
            <a href="https://instagram.com" target="_blank" rel="noopener noreferrer" aria-label="Instagram">
              <svg viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 0C8.74 0 8.333.015 7.053.072 5.775.132 4.905.333 4.14.63c-.789.306-1.459.717-2.126 1.384S.935 3.35.63 4.14C.333 4.905.131 5.775.072 7.053.012 8.333 0 8.74 0 12s.015 3.667.072 4.947c.06 1.277.261 2.148.558 2.913.306.788.717 1.459 1.384 2.126.667.666 1.336 1.079 2.126 1.384.766.296 1.636.499 2.913.558C8.333 23.988 8.74 24 12 24s3.667-.015 4.947-.072c1.277-.06 2.148-.262 2.913-.558.788-.306 1.459-.718 2.126-1.384.666-.667 1.079-1.335 1.384-2.126.296-.765.499-1.636.558-2.913.06-1.28.072-1.687.072-4.947s-.015-3.667-.072-4.947c-.06-1.277-.262-2.149-.558-2.913-.306-.789-.718-1.459-1.384-2.126C21.319 1.347 20.651.935 19.86.63c-.765-.297-1.636-.499-2.913-.558C15.667.012 15.26 0 12 0zm0 2.16c3.203 0 3.585.016 4.85.071 1.17.055 1.805.249 2.227.415.562.217.96.477 1.382.896.419.42.679.819.896 1.381.164.422.36 1.057.413 2.227.057 1.266.07 1.646.07 4.85s-.015 3.585-.074 4.85c-.061 1.17-.256 1.805-.421 2.227-.224.562-.479.96-.899 1.382-.419.419-.824.679-1.38.896-.42.164-1.065.36-2.235.413-1.274.057-1.649.07-4.859.07-3.211 0-3.586-.015-4.859-.074-1.171-.061-1.816-.256-2.236-.421-.569-.224-.96-.479-1.379-.899-.421-.419-.69-.824-.9-1.38-.165-.42-.359-1.065-.42-2.235-.045-1.26-.061-1.649-.061-4.844 0-3.196.016-3.586.061-4.861.061-1.17.255-1.814.42-2.234.21-.57.479-.96.9-1.381.419-.419.81-.689 1.379-.898.42-.166 1.051-.361 2.221-.421 1.275-.045 1.65-.06 4.859-.06l.045.03zm0 3.678c-3.405 0-6.162 2.76-6.162 6.162 0 3.405 2.76 6.162 6.162 6.162 3.405 0 6.162-2.76 6.162-6.162 0-3.405-2.757-6.162-6.162-6.162zM12 16c-2.21 0-4-1.79-4-4s1.79-4 4-4 4 1.79 4 4-1.79 4-4 4zm7.846-10.405c0 .795-.646 1.44-1.44 1.44-.795 0-1.44-.646-1.44-1.44 0-.794.646-1.439 1.44-1.439.793-.001 1.44.645 1.44 1.439z"/>
              </svg>
            </a>
          </div>

          <div className="footer-extras">
            <select className="language-select">
              <option value="en">English</option>
              <option value="ar">العربية</option>
              <option value="fr">Français</option>
              <option value="es">Español</option>
            </select>
            <span className="currency">S£ SYP</span>
          </div>
        </div>
      </div>
    </footer>
  );
}

export default Footer;
