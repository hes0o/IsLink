import { Link } from 'react-router-dom';
import './NotFound.css';

function NotFound() {
  return (
    <div className="not-found-page">
      <div className="not-found-content">
        <h1 className="error-code">404</h1>
        <h2 className="error-title">Page Not Found</h2>
        <p className="error-message">
          Oops! The page you're looking for doesn't exist or has been moved.
        </p>
        <div className="error-actions">
          <Link to="/" className="btn-home">
            Go to Homepage
          </Link>
          <Link to="/gigs" className="btn-explore">
            Explore Services
          </Link>
        </div>
      </div>
    </div>
  );
}

export default NotFound;
