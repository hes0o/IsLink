import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context';
import './Auth.css';

function Register() {
  const navigate = useNavigate();
  const { register, isAuthenticated } = useAuth();
  const [step, setStep] = useState(1);
  const [formData, setFormData] = useState({
    email: '',
    username: '',
    password: '',
    confirmPassword: '',
    fullName: '',
    accountType: 'buyer',
    agreeToTerms: false
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

  const validateStep1 = () => {
    const newErrors = {};
    if (!formData.email) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Please enter a valid email';
    }
    if (!formData.username) {
      newErrors.username = 'Username is required';
    } else if (formData.username.length < 3) {
      newErrors.username = 'Username must be at least 3 characters';
    }
    return newErrors;
  };

  const validateStep2 = () => {
    const newErrors = {};
    if (!formData.password) {
      newErrors.password = 'Password is required';
    } else if (formData.password.length < 8) {
      newErrors.password = 'Password must be at least 8 characters';
    }
    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
    }
    if (!formData.fullName) {
      newErrors.fullName = 'Full name is required';
    }
    if (!formData.agreeToTerms) {
      newErrors.agreeToTerms = 'You must agree to the terms';
    }
    return newErrors;
  };

  const handleNextStep = () => {
    const newErrors = validateStep1();
    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }
    setStep(2);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const newErrors = validateStep2();
    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    setIsLoading(true);
    setApiError('');

    const result = await register({
      email: formData.email,
      username: formData.username,
      password: formData.password,
      fullName: formData.fullName,
      accountType: formData.accountType
    });

    setIsLoading(false);

    if (result.success) {
      navigate('/dashboard');
    } else {
      setApiError(result.message || 'Registration failed. Please try again.');
    }
  };

  const getPasswordStrength = () => {
    const password = formData.password;
    if (!password) return { strength: 0, label: '' };
    let strength = 0;
    if (password.length >= 8) strength++;
    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
    if (/\d/.test(password)) strength++;
    if (/[^a-zA-Z0-9]/.test(password)) strength++;
    
    const labels = ['', 'Weak', 'Fair', 'Good', 'Strong'];
    return { strength, label: labels[strength] };
  };

  const passwordStrength = getPasswordStrength();

  return (
    <div className="auth-page">
      <div className="auth-container">
        {/* Left Side - Branding */}
        <div className="auth-branding register-branding">
          <div className="branding-content">
            <Link to="/" className="auth-logo">
              <svg viewBox="0 0 40 40" fill="none" xmlns="http://www.w3.org/2000/svg" className="auth-logo-icon">
                <rect width="40" height="40" rx="10" fill="url(#authLogoGrad2)"/>
                <path d="M12 20C12 16 14 14 18 14C20 14 21.5 15 22 16" stroke="white" strokeWidth="2.5" strokeLinecap="round"/>
                <path d="M28 20C28 24 26 26 22 26C20 26 18.5 25 18 24" stroke="white" strokeWidth="2.5" strokeLinecap="round"/>
                <circle cx="18" cy="14" r="2" fill="white"/>
                <circle cx="22" cy="26" r="2" fill="white"/>
                <defs>
                  <linearGradient id="authLogoGrad2" x1="0" y1="0" x2="40" y2="40">
                    <stop stopColor="#1dbf73"/>
                    <stop offset="1" stopColor="#14a35d"/>
                  </linearGradient>
                </defs>
              </svg>
              <span className="logo-text">IsLink</span>
            </Link>
            <h1>Join IsLink Today</h1>
            <p>Start your freelancing journey or find the perfect talent for your projects.</p>
            <div className="features-list">
              <div className="feature">
                <span className="feature-icon">🌍</span>
                <span>Connect with clients globally</span>
              </div>
              <div className="feature">
                <span className="feature-icon">💰</span>
                <span>Secure payment protection</span>
              </div>
              <div className="feature">
                <span className="feature-icon">⭐</span>
                <span>Build your reputation</span>
              </div>
              <div className="feature">
                <span className="feature-icon">🚀</span>
                <span>Grow your business</span>
              </div>
            </div>
          </div>
        </div>

        {/* Right Side - Form */}
        <div className="auth-form-section">
          <div className="auth-form-container">
            <h2>Create Account</h2>
            <p className="auth-subtitle">
              {step === 1 ? 'Enter your email and choose a username' : 'Set your password and complete registration'}
            </p>

            {apiError && (
              <div className="api-error">
                {apiError}
              </div>
            )}

            {/* Progress Steps */}
            <div className="progress-steps">
              <div className={`progress-step ${step >= 1 ? 'active' : ''}`}>
                <span className="step-number">1</span>
                <span className="step-label">Account</span>
              </div>
              <div className="progress-line"></div>
              <div className={`progress-step ${step >= 2 ? 'active' : ''}`}>
                <span className="step-number">2</span>
                <span className="step-label">Security</span>
              </div>
            </div>

            {step === 1 ? (
              <>
                <div className="divider">
                  <span>Register with email</span>
                </div>

                {/* Step 1 Form */}
                <form className="auth-form" onSubmit={(e) => { e.preventDefault(); handleNextStep(); }}>
                  <div className={`form-group ${errors.email ? 'error' : ''}`}>
                    <label htmlFor="email">Email</label>
                    <input
                      type="email"
                      id="email"
                      name="email"
                      placeholder="name@example.com"
                      value={formData.email}
                      onChange={handleChange}
                    />
                    {errors.email && <span className="error-message">{errors.email}</span>}
                  </div>

                  <div className={`form-group ${errors.username ? 'error' : ''}`}>
                    <label htmlFor="username">Username</label>
                    <input
                      type="text"
                      id="username"
                      name="username"
                      placeholder="Choose a username"
                      value={formData.username}
                      onChange={handleChange}
                    />
                    {errors.username && <span className="error-message">{errors.username}</span>}
                    <span className="input-hint">This will be your public profile name</span>
                  </div>

                  {/* Account Type Selection */}
                  <div className="form-group">
                    <label>I want to</label>
                    <div className="account-type-options">
                      <label className={`type-option ${formData.accountType === 'buyer' ? 'selected' : ''}`}>
                        <input
                          type="radio"
                          name="accountType"
                          value="buyer"
                          checked={formData.accountType === 'buyer'}
                          onChange={handleChange}
                        />
                        <span className="type-icon">🛒</span>
                        <span className="type-label">Hire for projects</span>
                        <span className="type-desc">I'm looking for freelancers</span>
                      </label>
                      <label className={`type-option ${formData.accountType === 'seller' ? 'selected' : ''}`}>
                        <input
                          type="radio"
                          name="accountType"
                          value="seller"
                          checked={formData.accountType === 'seller'}
                          onChange={handleChange}
                        />
                        <span className="type-icon">💼</span>
                        <span className="type-label">Work as freelancer</span>
                        <span className="type-desc">I want to offer services</span>
                      </label>
                    </div>
                  </div>

                  <button type="submit" className="btn-submit">
                    Continue
                  </button>
                </form>
              </>
            ) : (
              /* Step 2 Form */
              <form className="auth-form" onSubmit={handleSubmit}>
                <div className={`form-group ${errors.fullName ? 'error' : ''}`}>
                  <label htmlFor="fullName">Full Name</label>
                  <input
                    type="text"
                    id="fullName"
                    name="fullName"
                    placeholder="Enter your full name"
                    value={formData.fullName}
                    onChange={handleChange}
                    disabled={isLoading}
                  />
                  {errors.fullName && <span className="error-message">{errors.fullName}</span>}
                </div>

                <div className={`form-group ${errors.password ? 'error' : ''}`}>
                  <label htmlFor="password">Password</label>
                  <div className="password-input">
                    <input
                      type={showPassword ? 'text' : 'password'}
                      id="password"
                      name="password"
                      placeholder="Create a password"
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
                  {formData.password && (
                    <div className="password-strength">
                      <div className="strength-bars">
                        {[1, 2, 3, 4].map(level => (
                          <div 
                            key={level}
                            className={`bar ${passwordStrength.strength >= level ? `level-${passwordStrength.strength}` : ''}`}
                          ></div>
                        ))}
                      </div>
                      <span className={`strength-label level-${passwordStrength.strength}`}>
                        {passwordStrength.label}
                      </span>
                    </div>
                  )}
                  {errors.password && <span className="error-message">{errors.password}</span>}
                </div>

                <div className={`form-group ${errors.confirmPassword ? 'error' : ''}`}>
                  <label htmlFor="confirmPassword">Confirm Password</label>
                  <input
                    type="password"
                    id="confirmPassword"
                    name="confirmPassword"
                    placeholder="Confirm your password"
                    value={formData.confirmPassword}
                    onChange={handleChange}
                    disabled={isLoading}
                  />
                  {errors.confirmPassword && <span className="error-message">{errors.confirmPassword}</span>}
                </div>

                <div className={`form-group ${errors.agreeToTerms ? 'error' : ''}`}>
                  <label className="checkbox-label">
                    <input
                      type="checkbox"
                      name="agreeToTerms"
                      checked={formData.agreeToTerms}
                      onChange={handleChange}
                    />
                    <span className="checkmark"></span>
                    I agree to IsLink's Terms of Service and Privacy Policy
                  </label>
                  {errors.agreeToTerms && <span className="error-message">{errors.agreeToTerms}</span>}
                </div>

                <div className="form-buttons">
                  <button type="button" className="btn-back" onClick={() => setStep(1)} disabled={isLoading}>
                    Back
                  </button>
                  <button type="submit" className="btn-submit" disabled={isLoading}>
                    {isLoading ? 'Creating Account...' : 'Create Account'}
                  </button>
                </div>
              </form>
            )}

            <p className="auth-footer">
              Already have an account? <Link to="/auth/login">Sign in</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Register;
