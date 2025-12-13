import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context';
import './Auth.css';

function Login() {
  const navigate = useNavigate();
  const { login, isAuthenticated } = useAuth();
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    rememberMe: false
  });
  const [showPassword, setShowPassword] = useState(false);
  const [errors, setErrors] = useState({});
  const [isLoading, setIsLoading] = useState(false);
  const [apiError, setApiError] = useState('');

  // Redirect if already logged in
  if (isAuthenticated) {
    navigate('/dashboard');
    return null;
  }

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
    setApiError('');
  };

  const validate = () => {
    const newErrors = {};
    if (!formData.email) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Please enter a valid email';
    }
    if (!formData.password) {
      newErrors.password = 'Password is required';
    }
    return newErrors;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const newErrors = validate();
    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    setIsLoading(true);
    setApiError('');

    const result = await login(formData.email, formData.password);
    
    setIsLoading(false);
    
    if (result.success) {
      navigate('/dashboard');
    } else {
      setApiError(result.message || 'Login failed. Please try again.');
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-container">
        {/* Left Side - Branding */}
        <div className="auth-branding">
          <div className="branding-content">
            <Link to="/" className="auth-logo">
              <svg viewBox="0 0 40 40" fill="none" xmlns="http://www.w3.org/2000/svg" className="auth-logo-icon">
                <rect width="40" height="40" rx="10" fill="url(#authLogoGrad1)"/>
                <path d="M12 20C12 16 14 14 18 14C20 14 21.5 15 22 16" stroke="white" strokeWidth="2.5" strokeLinecap="round"/>
                <path d="M28 20C28 24 26 26 22 26C20 26 18.5 25 18 24" stroke="white" strokeWidth="2.5" strokeLinecap="round"/>
                <circle cx="18" cy="14" r="2" fill="white"/>
                <circle cx="22" cy="26" r="2" fill="white"/>
                <defs>
                  <linearGradient id="authLogoGrad1" x1="0" y1="0" x2="40" y2="40">
                    <stop stopColor="#1dbf73"/>
                    <stop offset="1" stopColor="#14a35d"/>
                  </linearGradient>
                </defs>
              </svg>
              <span className="logo-text">IsLink</span>
            </Link>
            <h1>Welcome Back!</h1>
            <p>Sign in to continue your freelancing journey and connect with clients worldwide.</p>
            <div className="features-list">
              <div className="feature">
                <span className="feature-icon">💼</span>
                <span>Access your dashboard</span>
              </div>
              <div className="feature">
                <span className="feature-icon">💬</span>
                <span>Check your messages</span>
              </div>
              <div className="feature">
                <span className="feature-icon">📊</span>
                <span>Track your orders</span>
              </div>
            </div>
          </div>
        </div>

        {/* Right Side - Form */}
        <div className="auth-form-section">
          <div className="auth-form-container">
            <h2>Sign In</h2>
            <p className="auth-subtitle">Enter your credentials to access your account</p>

            {apiError && (
              <div className="api-error">
                {apiError}
              </div>
            )}

            {/* Demo Accounts Info */}
            <div className="demo-accounts">
              <p><strong>Demo Accounts:</strong></p>
              <small>design@example.com / password123 (Seller)</small><br/>
              <small>buyer@example.com / password123 (Buyer)</small>
            </div>

            <div className="divider">
              <span>or</span>
            </div>

            {/* Login Form */}
            <form onSubmit={handleSubmit} className="auth-form">
              <div className={`form-group ${errors.email ? 'error' : ''}`}>
                <label htmlFor="email">Email</label>
                <input
                  type="email"
                  id="email"
                  name="email"
                  placeholder="name@example.com"
                  value={formData.email}
                  onChange={handleChange}
                  disabled={isLoading}
                />
                {errors.email && <span className="error-message">{errors.email}</span>}
              </div>

              <div className={`form-group ${errors.password ? 'error' : ''}`}>
                <label htmlFor="password">Password</label>
                <div className="password-input">
                  <input
                    type={showPassword ? 'text' : 'password'}
                    id="password"
                    name="password"
                    placeholder="Enter your password"
                    value={formData.password}
                    onChange={handleChange}
                    disabled={isLoading}
                  />
                  <button 
                    type="button" 
                    className="toggle-password"
                    onClick={() => setShowPassword(!showPassword)}
                  >
                    {showPassword ? '👁️' : '👁️‍🗨️'}
                  </button>
                </div>
                {errors.password && <span className="error-message">{errors.password}</span>}
              </div>

              <div className="form-options">
                <label className="checkbox-label">
                  <input
                    type="checkbox"
                    name="rememberMe"
                    checked={formData.rememberMe}
                    onChange={handleChange}
                  />
                  <span className="checkmark"></span>
                  Remember me
                </label>
                <Link to="/auth/forgot-password" className="forgot-link">
                  Forgot Password?
                </Link>
              </div>

              <button type="submit" className="btn-submit" disabled={isLoading}>
                {isLoading ? 'Signing In...' : 'Sign In'}
              </button>
            </form>

            <p className="auth-footer">
              Don't have an account? <Link to="/auth/register">Join now</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Login;
