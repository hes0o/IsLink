import { useState, useEffect, useMemo } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import { categories } from '../../data/mockData';
import { gigsAPI, categoriesAPI } from '../../services/api';
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
  const [gigs, setGigs] = useState([]);
  const [categoriesData, setCategoriesData] = useState([]);
  const [loading, setLoading] = useState(true);

  // Find current category
  const currentCategory = useMemo(() => {
    return categoriesData.find(cat => (cat.slug || cat.Slug) === categorySlug);
  }, [categoriesData, categorySlug]);

  // Fetch gigs from API
  useEffect(() => {
    const loadGigs = async () => {
      setLoading(true);
      try {
        const params = {
          limit: 50,
          sortBy: filters.sortBy === 'recommended' ? 'rating' : filters.sortBy
        };

        if (categorySlug) {
          params.category = categorySlug;
        }

        if (searchQuery) {
          params.search = searchQuery;
        }

        if (filters.minPrice) {
          params.minPrice = Number(filters.minPrice);
        }

        if (filters.maxPrice) {
          params.maxPrice = Number(filters.maxPrice);
        }

        if (filters.deliveryTime !== 'any') {
          params.deliveryTime = Number(filters.deliveryTime);
        }

        const gigsResponse = await gigsAPI.getAll(params);
        const gigsList = gigsResponse?.Data || gigsResponse?.data || [];
        setGigs(Array.isArray(gigsList) ? gigsList : []);
      } catch (error) {
        console.error('Error loading gigs:', error);
        setGigs([]);
      } finally {
        setLoading(false);
      }
    };

    loadGigs();
  }, [categorySlug, searchQuery, filters]);

  // Fetch categories on mount
  useEffect(() => {
    const loadCategories = async () => {
      try {
        const categoriesResponse = await categoriesAPI.getAll();
        if (categoriesResponse?.Data || categoriesResponse?.data) {
          const catsList = categoriesResponse.Data || categoriesResponse.data || [];
          setCategoriesData(Array.isArray(catsList) ? catsList : []);
        } else {
          setCategoriesData(categories);
        }
      } catch (error) {
        console.error('Error loading categories:', error);
        setCategoriesData(categories);
      }
    };

    loadCategories();
  }, []);

  // Filter and sort gigs (frontend filtering for sellerLevel if needed)
  const filteredGigs = useMemo(() => {
    let result = [...gigs];

    // Seller level filter (frontend only, as backend doesn't have this)
    if (filters.sellerLevel !== 'any') {
      if (filters.sellerLevel === 'top') {
        result = result.filter(gig => {
          const seller = gig.seller || gig.Seller;
          const rating = seller?.rating || seller?.Rating || gig.rating || gig.Rating;
          return rating >= 4.7;
        });
      }
    }

    return result;
  }, [gigs, filters]);

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
            <span className="current">{currentCategory.name || currentCategory.Name}</span>
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
                <span className="category-icon">{currentCategory.icon || currentCategory.Icon || '📁'}</span>
                {currentCategory.name || currentCategory.Name}
              </>
            ) : searchQuery ? (
              <>Results for "{searchQuery}"</>
            ) : (
              <>All Services</>
            )}
          </h1>
          {currentCategory && (
            <p className="category-description">
              Find the best {(currentCategory.name || currentCategory.Name).toLowerCase()} services for your project
            </p>
          )}
        </div>
      </div>

      {/* Subcategories */}
      {currentCategory && (currentCategory.subcategories || currentCategory.Subcategories)?.length > 0 && (
        <div className="subcategories">
          <div className="container">
            <div className="subcategory-list">
              {(currentCategory.subcategories || currentCategory.Subcategories || []).map(sub => (
                <Link 
                  key={sub.id || sub.Id} 
                  to={`/gigs?category=${currentCategory.slug || currentCategory.Slug}&sub=${sub.slug || sub.Slug}`}
                  className="subcategory-chip"
                >
                  {sub.name || sub.Name}
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
          {loading ? (
            <div className="loading-state">
              <div className="spinner"></div>
              <p>Loading services...</p>
            </div>
          ) : filteredGigs.length > 0 ? (
            <div className="gigs-grid">
              {filteredGigs.map(gig => (
                <GigCard key={gig.id || gig.Id} gig={gig} />
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
