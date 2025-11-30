import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { categories, gigs } from '../../data/mockData';
import GigCard from '../../components/gig/GigCard';
import './Home.css';

function Home() {
  const [searchQuery, setSearchQuery] = useState('');
  const navigate = useNavigate();

  const handleSearch = (e) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      navigate(`/gigs?search=${encodeURIComponent(searchQuery)}`);
    }
  };

  const popularTags = [
    'Website Design',
    'Logo Design',
    'SEO',
    'Video Editing',
    'Social Media',
  ];

  return (
    <div className="home">
      {/* Hero Section */}
      <section className="hero">
        <div className="hero-content">
          <h1 className="hero-title">
            Find the perfect <span className="highlight">freelance</span> services for your business
          </h1>
          <p className="hero-subtitle">
            Connect with talented professionals ready to bring your ideas to life
          </p>
          
          <form className="hero-search" onSubmit={handleSearch}>
            <input
              type="text"
              placeholder="Try 'logo design' or 'website development'"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
            <button type="submit">Search</button>
          </form>

          <div className="popular-tags">
            <span>Popular:</span>
            {popularTags.map((tag) => (
              <Link key={tag} to={`/gigs?search=${encodeURIComponent(tag)}`}>
                {tag}
              </Link>
            ))}
          </div>
        </div>

        {/* Animated background elements */}
        <div className="hero-bg">
          <div className="hero-shape hero-shape-1"></div>
          <div className="hero-shape hero-shape-2"></div>
          <div className="hero-shape hero-shape-3"></div>
        </div>
      </section>

      {/* Trusted By Section */}
      <section className="trusted-by">
        <div className="container">
          <span className="trusted-label">Trusted by:</span>
          <div className="trusted-logos">
            <span className="company-logo">Meta</span>
            <span className="company-logo">Google</span>
            <span className="company-logo">Netflix</span>
            <span className="company-logo">P&G</span>
            <span className="company-logo">PayPal</span>
          </div>
        </div>
      </section>

      {/* Categories Section */}
      <section className="categories-section">
        <div className="container">
          <h2 className="section-title">Explore Popular Categories</h2>
          <div className="categories-grid">
            {categories.map((category) => (
              <Link
                key={category.id}
                to={`/gigs?category=${category.slug}`}
                className="category-card"
              >
                <span className="category-icon">{category.icon}</span>
                <h3 className="category-name">{category.name}</h3>
                <p className="category-count">
                  {Math.floor(Math.random() * 1000) + 200}+ services
                </p>
              </Link>
            ))}
          </div>
        </div>
      </section>

      {/* Featured Gigs Section */}
      <section className="featured-section">
        <div className="container">
          <div className="section-header">
            <h2 className="section-title">Popular Services</h2>
            <Link to="/gigs" className="view-all">
              View All →
            </Link>
          </div>
          <div className="gigs-grid">
            {gigs.map((gig) => (
              <GigCard key={gig.id} gig={gig} />
            ))}
          </div>
        </div>
      </section>

      {/* How It Works Section */}
      <section className="how-it-works">
        <div className="container">
          <h2 className="section-title">How IsLink Works</h2>
          <div className="steps-grid">
            <div className="step-card">
              <div className="step-number">1</div>
              <div className="step-icon">🔍</div>
              <h3>Find a Service</h3>
              <p>Browse through thousands of services or search for exactly what you need</p>
            </div>
            <div className="step-card">
              <div className="step-number">2</div>
              <div className="step-icon">💬</div>
              <h3>Contact Seller</h3>
              <p>Discuss your requirements and get a custom quote from the seller</p>
            </div>
            <div className="step-card">
              <div className="step-number">3</div>
              <div className="step-icon">💳</div>
              <h3>Place Order</h3>
              <p>Make a secure payment and the seller will start working on your project</p>
            </div>
            <div className="step-card">
              <div className="step-number">4</div>
              <div className="step-icon">✅</div>
              <h3>Get Delivered</h3>
              <p>Receive your completed work and release payment when satisfied</p>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="cta-section">
        <div className="container">
          <div className="cta-content">
            <h2>Ready to Start Selling?</h2>
            <p>Join thousands of freelancers and start earning on IsLink</p>
            <Link to="/auth/register" className="cta-button">
              Become a Seller
            </Link>
          </div>
        </div>
      </section>
    </div>
  );
}

export default Home;
