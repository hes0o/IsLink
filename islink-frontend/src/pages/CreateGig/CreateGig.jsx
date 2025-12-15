import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context';
import { gigsAPI, categoriesAPI } from '../../services/api';
import './CreateGig.css';

function CreateGig() {
  const navigate = useNavigate();
  const { isAuthenticated, user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [categories, setCategories] = useState([]);
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    categoryId: '',
    images: [''],
    tags: [''],
    packages: {
      basic: {
        name: 'Basic',
        price: 25,
        deliveryDays: 3,
        revisions: '2',
        description: 'Basic package',
        features: ['']
      },
      standard: {
        name: 'Standard',
        price: 50,
        deliveryDays: 5,
        revisions: '5',
        description: 'Standard package with more features',
        features: ['']
      },
      premium: {
        name: 'Premium',
        price: 100,
        deliveryDays: 7,
        revisions: 'Unlimited',
        description: 'Premium package with all features',
        features: ['']
      }
    }
  });

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/auth/login');
      return;
    }

    // Fetch categories
    categoriesAPI.getAll()
      .then(response => {
        if (response?.Success && response?.Data) {
          setCategories(response.Data);
        }
      })
      .catch(err => console.error('Error fetching categories:', err));
  }, [isAuthenticated, navigate]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handlePackageChange = (packageType, field, value) => {
    setFormData(prev => ({
      ...prev,
      packages: {
        ...prev.packages,
        [packageType]: {
          ...prev.packages[packageType],
          [field]: field === 'price' || field === 'deliveryDays' ? Number(value) : value
        }
      }
    }));
  };

  const handleArrayChange = (field, index, value) => {
    setFormData(prev => {
      const newArray = [...prev[field]];
      newArray[index] = value;
      return { ...prev, [field]: newArray };
    });
  };

  const addArrayItem = (field) => {
    setFormData(prev => ({
      ...prev,
      [field]: [...prev[field], '']
    }));
  };

  const removeArrayItem = (field, index) => {
    setFormData(prev => ({
      ...prev,
      [field]: prev[field].filter((_, i) => i !== index)
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!isAuthenticated) {
      alert('Please log in to create a gig');
      navigate('/auth/login');
      return;
    }

    setLoading(true);

    try {
      // Prepare request payload
      const payload = {
        title: formData.title,
        description: formData.description,
        categoryId: formData.categoryId || null,
        images: formData.images.filter(img => img.trim()),
        tags: formData.tags.filter(tag => tag.trim()),
        packages: {
          basic: {
            name: formData.packages.basic.name,
            price: formData.packages.basic.price,
            deliveryDays: formData.packages.basic.deliveryDays,
            revisions: formData.packages.basic.revisions,
            description: formData.packages.basic.description,
            features: formData.packages.basic.features.filter(f => f.trim())
          },
          standard: {
            name: formData.packages.standard.name,
            price: formData.packages.standard.price,
            deliveryDays: formData.packages.standard.deliveryDays,
            revisions: formData.packages.standard.revisions,
            description: formData.packages.standard.description,
            features: formData.packages.standard.features.filter(f => f.trim())
          },
          premium: {
            name: formData.packages.premium.name,
            price: formData.packages.premium.price,
            deliveryDays: formData.packages.premium.deliveryDays,
            revisions: formData.packages.premium.revisions,
            description: formData.packages.premium.description,
            features: formData.packages.premium.features.filter(f => f.trim())
          }
        }
      };

      const response = await gigsAPI.create(payload);
      
      if (response?.Success && response?.Data) {
        alert('Gig created successfully!');
        navigate(`/gig/${response.Data.Slug || response.Data.slug}`);
      } else {
        alert(response?.Message || 'Failed to create gig');
      }
    } catch (error) {
      console.error('Error creating gig:', error);
      alert('Failed to create gig. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  if (!isAuthenticated) {
    return null;
  }

  return (
    <div className="create-gig-page">
      <div className="container">
        <div className="create-gig-header">
          <h1>Create New Gig</h1>
          <p>Share your skills and start earning</p>
        </div>

        <form onSubmit={handleSubmit} className="create-gig-form">
          {/* Basic Information */}
          <section className="form-section">
            <h2>Basic Information</h2>
            
            <div className="form-group">
              <label htmlFor="title">Gig Title *</label>
              <input
                type="text"
                id="title"
                name="title"
                value={formData.title}
                onChange={handleInputChange}
                required
                maxLength={200}
                placeholder="e.g., I will design a modern logo for your business"
              />
            </div>

            <div className="form-group">
              <label htmlFor="description">Description *</label>
              <textarea
                id="description"
                name="description"
                value={formData.description}
                onChange={handleInputChange}
                required
                rows={6}
                placeholder="Describe what you will deliver..."
              />
            </div>

            <div className="form-group">
              <label htmlFor="categoryId">Category</label>
              <select
                id="categoryId"
                name="categoryId"
                value={formData.categoryId}
                onChange={handleInputChange}
              >
                <option value="">Select a category</option>
                {categories.map(cat => (
                  <option key={cat.Id || cat.id} value={cat.Id || cat.id}>
                    {cat.Name || cat.name}
                  </option>
                ))}
              </select>
            </div>
          </section>

          {/* Images */}
          <section className="form-section">
            <h2>Gig Images</h2>
            {formData.images.map((image, index) => (
              <div key={index} className="form-group">
                <label>Image URL {index + 1}</label>
                <div className="input-with-button">
                  <input
                    type="url"
                    value={image}
                    onChange={(e) => handleArrayChange('images', index, e.target.value)}
                    placeholder="https://example.com/image.jpg"
                  />
                  {formData.images.length > 1 && (
                    <button
                      type="button"
                      onClick={() => removeArrayItem('images', index)}
                      className="btn-remove"
                    >
                      Remove
                    </button>
                  )}
                </div>
              </div>
            ))}
            <button
              type="button"
              onClick={() => addArrayItem('images')}
              className="btn-add"
            >
              + Add Image
            </button>
          </section>

          {/* Tags */}
          <section className="form-section">
            <h2>Tags</h2>
            {formData.tags.map((tag, index) => (
              <div key={index} className="form-group">
                <label>Tag {index + 1}</label>
                <div className="input-with-button">
                  <input
                    type="text"
                    value={tag}
                    onChange={(e) => handleArrayChange('tags', index, e.target.value)}
                    placeholder="e.g., logo design"
                  />
                  {formData.tags.length > 1 && (
                    <button
                      type="button"
                      onClick={() => removeArrayItem('tags', index)}
                      className="btn-remove"
                    >
                      Remove
                    </button>
                  )}
                </div>
              </div>
            ))}
            <button
              type="button"
              onClick={() => addArrayItem('tags')}
              className="btn-add"
            >
              + Add Tag
            </button>
          </section>

          {/* Packages */}
          <section className="form-section">
            <h2>Packages</h2>
            
            {['basic', 'standard', 'premium'].map(packageType => (
              <div key={packageType} className="package-section">
                <h3>{packageType.charAt(0).toUpperCase() + packageType.slice(1)} Package</h3>
                
                <div className="package-grid">
                  <div className="form-group">
                    <label>Name *</label>
                    <input
                      type="text"
                      value={formData.packages[packageType].name}
                      onChange={(e) => handlePackageChange(packageType, 'name', e.target.value)}
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label>Price (SYP) *</label>
                    <input
                      type="number"
                      min="0"
                      step="0.01"
                      value={formData.packages[packageType].price}
                      onChange={(e) => handlePackageChange(packageType, 'price', e.target.value)}
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label>Delivery Days *</label>
                    <input
                      type="number"
                      min="1"
                      value={formData.packages[packageType].deliveryDays}
                      onChange={(e) => handlePackageChange(packageType, 'deliveryDays', e.target.value)}
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label>Revisions *</label>
                    <input
                      type="text"
                      value={formData.packages[packageType].revisions}
                      onChange={(e) => handlePackageChange(packageType, 'revisions', e.target.value)}
                      placeholder="e.g., 2, 5, or Unlimited"
                      required
                    />
                  </div>
                </div>

                <div className="form-group">
                  <label>Description</label>
                  <textarea
                    value={formData.packages[packageType].description}
                    onChange={(e) => handlePackageChange(packageType, 'description', e.target.value)}
                    rows={2}
                  />
                </div>
              </div>
            ))}
          </section>

          {/* Submit */}
          <div className="form-actions">
            <button
              type="button"
              onClick={() => navigate('/dashboard')}
              className="btn-secondary"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="btn-primary"
              disabled={loading}
            >
              {loading ? 'Creating...' : 'Create Gig'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default CreateGig;

