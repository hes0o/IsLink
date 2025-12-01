import { useState, useMemo } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import { categories, gigs } from '../../data/mockData';
import GigCard from '../../components/gig/GigCard';
import './Gigs.css';

function Gigs() {
  const [searchParams] = useSearchParams();
  const categorySlug = searchParams.get('category');
  const searchQuery = searchParams.get('search');

  const [filters, setFilters] = useState({
    minPrice: '',
    maxPrice: '',
    deliveryTime: 'any',
    sellerLevel: 'any',
    sortBy: 'recommended'
  });

  const [showFilters, setShowFilters] = useState(false);

  // Find current category
  const currentCategory = categories.find(cat => cat.slug === categorySlug);

  // Filter and sort gigs
  const filteredGigs = useMemo(() => {
    let result = [...gigs];

    // Filter by category
    if (categorySlug) {
      result = result.filter(gig => {
        const gigCategory = categories.find(cat => cat.id === gig.categoryId);
        return gigCategory?.slug === categorySlug;
      });
    }

    // Filter by search query
    if (searchQuery) {
      const query = searchQuery.toLowerCase();
      result = result.filter(gig => 
        gig.title.toLowerCase().includes(query) ||
        gig.tags.some(tag => tag.toLowerCase().includes(query))
      );
    }

    // Filter by price range
    if (filters.minPrice) {
      result = result.filter(gig => gig.packages.basic.price >= Number(filters.minPrice));
    }
    if (filters.maxPrice) {
      result = result.filter(gig => gig.packages.basic.price <= Number(filters.maxPrice));
    }

    // Filter by delivery time
    if (filters.deliveryTime !== 'any') {
      const maxDays = Number(filters.deliveryTime);
      result = result.filter(gig => gig.packages.basic.deliveryDays <= maxDays);
    }

    // Sort
    switch (filters.sortBy) {
      case 'price_low':
        result.sort((a, b) => a.packages.basic.price - b.packages.basic.price);
        break;
      case 'price_high':
        result.sort((a, b) => b.packages.basic.price - a.packages.basic.price);
        break;
      case 'rating':
        result.sort((a, b) => b.rating - a.rating);
        break;
      case 'reviews':
        result.sort((a, b) => b.reviewCount - a.reviewCount);
        break;
      default:
        // recommended - keep original order
        break;
    }

    return result;
  }, [categorySlug, searchQuery, filters]);

  const handleFilterChange = (key, value) => {
    setFilters(prev => ({ ...prev, [key]: value }));
  };

  const clearFilters = () => {
    setFilters({
      minPrice: '',
      maxPrice: '',
      deliveryTime: 'any',
      sellerLevel: 'any',
      sortBy: 'recommended'
    });
  };

  return (
    <div className="gigs-page">
      {/* Breadcrumb */}
      <div className="breadcrumb">
        <div className="container">
          <Link to="/">Home</Link>
          <span className="separator">/</span>
          {currentCategory ? (
            <span className="current">{currentCategory.name}</span>
          ) : searchQuery ? (
            <span className="current">Search: "{searchQuery}"</span>
          ) : (
            <span className="current">All Services</span>
          )}
        </div>
      </div>

      {/* Page Header */}
      <div className="page-header">
        <div className="container">
          <h1>
            {currentCategory ? (
              <>
                <span className="category-icon">{currentCategory.icon}</span>
                {currentCategory.name}
              </>
            ) : searchQuery ? (
              <>Results for "{searchQuery}"</>
            ) : (
              <>All Services</>
            )}
          </h1>
          {currentCategory && (
            <p className="category-description">
              Find the best {currentCategory.name.toLowerCase()} services for your project
            </p>
          )}
        </div>
      </div>

      {/* Subcategories */}
      {currentCategory && currentCategory.subcategories.length > 0 && (
        <div className="subcategories">
          <div className="container">
            <div className="subcategory-list">
              {currentCategory.subcategories.map(sub => (
                <Link 
                  key={sub.id} 
                  to={`/gigs?category=${currentCategory.slug}&sub=${sub.slug}`}
                  className="subcategory-chip"
                >
                  {sub.name}
                </Link>
              ))}
            </div>
          </div>
        </div>
      )}

      <div className="gigs-content">
        <div className="container">
          {/* Toolbar */}
          <div className="toolbar">
            <div className="results-count">
              <span className="count">{filteredGigs.length}</span> services available
            </div>

            <div className="toolbar-actions">
              <button 
                className={`filter-toggle ${showFilters ? 'active' : ''}`}
                onClick={() => setShowFilters(!showFilters)}
              >
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M4 21v-7M4 10V3M12 21v-9M12 8V3M20 21v-5M20 12V3M1 14h6M9 8h6M17 16h6"/>
                </svg>
                Filters
              </button>

              <div className="sort-dropdown">
                <label>Sort by:</label>
                <select 
                  value={filters.sortBy}
                  onChange={(e) => handleFilterChange('sortBy', e.target.value)}
                >
                  <option value="recommended">Recommended</option>
                  <option value="rating">Best Rating</option>
                  <option value="reviews">Most Reviews</option>
                  <option value="price_low">Price: Low to High</option>
                  <option value="price_high">Price: High to Low</option>
                </select>
              </div>
            </div>
          </div>

          {/* Filters Panel */}
          {showFilters && (
            <div className="filters-panel">
              <div className="filter-group">
                <label>Budget</label>
                <div className="price-inputs">
                  <input
                    type="number"
                    placeholder="Min"
                    value={filters.minPrice}
                    onChange={(e) => handleFilterChange('minPrice', e.target.value)}
                  />
                  <span>to</span>
                  <input
                    type="number"
                    placeholder="Max"
                    value={filters.maxPrice}
                    onChange={(e) => handleFilterChange('maxPrice', e.target.value)}
                  />
                </div>
              </div>

              <div className="filter-group">
                <label>Delivery Time</label>
                <select
                  value={filters.deliveryTime}
                  onChange={(e) => handleFilterChange('deliveryTime', e.target.value)}
                >
                  <option value="any">Any</option>
                  <option value="1">Up to 24 hours</option>
                  <option value="3">Up to 3 days</option>
                  <option value="7">Up to 7 days</option>
                  <option value="14">Up to 14 days</option>
                </select>
              </div>

              <div className="filter-group">
                <label>Seller Level</label>
                <select
                  value={filters.sellerLevel}
                  onChange={(e) => handleFilterChange('sellerLevel', e.target.value)}
                >
                  <option value="any">Any Level</option>
                  <option value="top">Top Rated</option>
                  <option value="level2">Level 2</option>
                  <option value="level1">Level 1</option>
                  <option value="new">New Seller</option>
                </select>
              </div>

              <button className="clear-filters" onClick={clearFilters}>
                Clear All
              </button>
            </div>
          )}

          {/* Gigs Grid */}
          {filteredGigs.length > 0 ? (
            <div className="gigs-grid">
              {filteredGigs.map(gig => (
                <GigCard key={gig.id} gig={gig} />
              ))}
            </div>
          ) : (
            <div className="no-results">
              <div className="no-results-icon">🔍</div>
              <h3>No services found</h3>
              <p>Try adjusting your filters or search terms</p>
              <button className="btn-primary" onClick={clearFilters}>
                Clear Filters
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default Gigs;
