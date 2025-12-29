import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../context';
import { ordersAPI, gigsAPI } from '../../services/api';
import './Dashboard.css';

function Dashboard() {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState('overview');
  const [orders, setOrders] = useState([]);
  const [myGigs, setMyGigs] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [stats, setStats] = useState({
    totalEarnings: 0,
    activeOrders: 0,
    completedOrders: 0,
    totalGigs: 0
  });

  useEffect(() => {
    if (user) {
      loadDashboardData();
    }
  }, [user]);

  const loadDashboardData = async () => {
    setIsLoading(true);
    setError(null);

    try {
      // Fetch orders - backend returns { success: true, data: [...] }
      const ordersResponse = await ordersAPI.getAll({ role: user?.role === 'seller' ? 'seller' : 'buyer' });
      const ordersList = ordersResponse?.Data || ordersResponse?.data || [];

      // Transform orders for display
      const formattedOrders = ordersList.map(order => ({
        id: order.id || order.Id,
        orderNumber: order.orderNumber || order.OrderNumber,
        gigTitle: order.gig?.title || order.Gig?.Title || 'Service Order',
        buyer: order.buyer?.username || order.Buyer?.Username || 'Buyer',
        seller: order.seller?.username || order.Seller?.Username || 'Seller',
        status: order.status || order.Status || 'pending',
        price: order.totalPrice || order.TotalPrice || 0,
        dueDate: order.deliveryDeadline ? new Date(order.deliveryDeadline).toLocaleDateString() : 'N/A'
      }));

      setOrders(formattedOrders);

      // Calculate stats from orders
      const activeOrders = formattedOrders.filter(o =>
        o.status === 'pending' || o.status === 'in_progress'
      ).length;
      const completedOrders = formattedOrders.filter(o => o.status === 'completed').length;
      const totalEarnings = formattedOrders
        .filter(o => o.status === 'completed')
        .reduce((sum, o) => sum + (o.price || 0), 0);

      setStats({
        totalEarnings: user?.balance || totalEarnings,
        activeOrders,
        completedOrders,
        totalGigs: myGigs.length
      });

    } catch (err) {
      console.error('Error loading dashboard data:', err);
      // Set default mock data for demo
      setOrders([
        { id: 1, orderNumber: 'ORD-001', gigTitle: 'Logo Design', buyer: 'john_doe', seller: user?.username, status: 'in_progress', price: 50, dueDate: '2024-12-20' },
        { id: 2, orderNumber: 'ORD-002', gigTitle: 'Website Development', buyer: 'jane_smith', seller: user?.username, status: 'pending', price: 200, dueDate: '2024-12-25' },
        { id: 3, orderNumber: 'ORD-003', gigTitle: 'SEO Optimization', buyer: 'mike_wilson', seller: user?.username, status: 'completed', price: 100, dueDate: '2024-12-10' },
      ]);
      setStats({
        totalEarnings: user?.balance || 2450,
        activeOrders: 2,
        completedOrders: 1,
        totalGigs: 3
      });
    }

    // Fetch user's gigs if seller
    if (user?.role === 'seller') {
      try {
        const gigsResponse = await gigsAPI.getBySeller(user.username);
        const gigsList = gigsResponse?.Data || gigsResponse?.data || gigsResponse || [];
        setMyGigs(Array.isArray(gigsList) ? gigsList : []);
        setStats(prev => ({ ...prev, totalGigs: Array.isArray(gigsList) ? gigsList.length : 0 }));
      } catch (err) {
        console.log('Could not load gigs:', err);
        setMyGigs([]);
      }
    }

    setIsLoading(false);
  };

  const getStatusBadge = (status) => {
    const statusMap = {
      pending: { label: 'Pending', className: 'status-pending' },
      in_progress: { label: 'In Progress', className: 'status-progress' },
      completed: { label: 'Completed', className: 'status-completed' },
      cancelled: { label: 'Cancelled', className: 'status-cancelled' }
    };
    const { label, className } = statusMap[status] || { label: status, className: '' };
    return <span className={`status-badge ${className}`}>{label}</span>;
  };

  // Show loading state
  if (isLoading) {
    return (
      <div className="dashboard-page">
        <div className="dashboard-loading">
          <div className="spinner"></div>
          <p>Loading your dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="dashboard-page">
      <div className="dashboard-container">
        {/* Sidebar */}
        <aside className="dashboard-sidebar">
          <div className="user-info">
            <img
              src={user?.avatarUrl || 'https://via.placeholder.com/80?text=U'}
              alt={user?.username || 'User'}
              className="user-avatar"
            />
            <h3>{user?.fullName || 'Demo User'}</h3>
            <p>@{user?.username || 'demo'}</p>
            <span className="user-level">Level 2 Seller</span>
          </div>

          <nav className="dashboard-nav">
            <button
              className={`nav-item ${activeTab === 'overview' ? 'active' : ''}`}
              onClick={() => setActiveTab('overview')}
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <rect x="3" y="3" width="7" height="7" />
                <rect x="14" y="3" width="7" height="7" />
                <rect x="14" y="14" width="7" height="7" />
                <rect x="3" y="14" width="7" height="7" />
              </svg>
              Overview
            </button>
            <button
              className={`nav-item ${activeTab === 'orders' ? 'active' : ''}`}
              onClick={() => setActiveTab('orders')}
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2" />
                <rect x="9" y="3" width="6" height="4" rx="2" />
                <line x1="9" y1="12" x2="15" y2="12" />
                <line x1="9" y1="16" x2="15" y2="16" />
              </svg>
              Orders
            </button>
            <button
              className={`nav-item ${activeTab === 'gigs' ? 'active' : ''}`}
              onClick={() => setActiveTab('gigs')}
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z" />
              </svg>
              My Gigs
            </button>
            <button
              className={`nav-item ${activeTab === 'earnings' ? 'active' : ''}`}
              onClick={() => setActiveTab('earnings')}
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <line x1="12" y1="1" x2="12" y2="23" />
                <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6" />
              </svg>
              Earnings
            </button>
          </nav>
        </aside>

        {/* Main Content */}
        <main className="dashboard-main">
          {activeTab === 'overview' && (
            <>
              <h1>Welcome back, {user?.fullName?.split(' ')[0] || 'User'}! 👋</h1>

              {/* Stats Cards */}
              <div className="stats-grid">
                <div className="stat-card">
                  <div className="stat-icon earnings">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                      <line x1="12" y1="1" x2="12" y2="23" />
                      <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6" />
                    </svg>
                  </div>
                  <div className="stat-info">
                    <h3>${stats.totalEarnings.toLocaleString()}</h3>
                    <p>Total Earnings</p>
                  </div>
                </div>

                <div className="stat-card">
                  <div className="stat-icon orders">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                      <circle cx="12" cy="12" r="10" />
                      <polyline points="12 6 12 12 16 14" />
                    </svg>
                  </div>
                  <div className="stat-info">
                    <h3>{stats.activeOrders}</h3>
                    <p>Active Orders</p>
                  </div>
                </div>

                <div className="stat-card">
                  <div className="stat-icon completed">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                      <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
                      <polyline points="22 4 12 14.01 9 11.01" />
                    </svg>
                  </div>
                  <div className="stat-info">
                    <h3>{stats.completedOrders}</h3>
                    <p>Completed</p>
                  </div>
                </div>

                <div className="stat-card">
                  <div className="stat-icon gigs">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                      <rect x="2" y="7" width="20" height="14" rx="2" ry="2" />
                      <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16" />
                    </svg>
                  </div>
                  <div className="stat-info">
                    <h3>{stats.totalGigs}</h3>
                    <p>Active Gigs</p>
                  </div>
                </div>
              </div>

              {/* Recent Orders */}
              <section className="dashboard-section">
                <div className="section-header">
                  <h2>Recent Orders</h2>
                  <button onClick={() => setActiveTab('orders')}>View All</button>
                </div>

                {isLoading ? (
                  <div className="loading">Loading...</div>
                ) : (
                  <div className="orders-table">
                    <table>
                      <thead>
                        <tr>
                          <th>Gig</th>
                          <th>Buyer</th>
                          <th>Status</th>
                          <th>Price</th>
                          <th>Due Date</th>
                        </tr>
                      </thead>
                      <tbody>
                        {orders.slice(0, 5).map(order => (
                          <tr key={order.id}>
                            <td>{order.gigTitle}</td>
                            <td>@{order.buyer}</td>
                            <td>{getStatusBadge(order.status)}</td>
                            <td>${order.price}</td>
                            <td>{order.dueDate}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
              </section>
            </>
          )}

          {activeTab === 'orders' && (
            <>
              <h1>Manage Orders</h1>
              <div className="orders-table full">
                <table>
                  <thead>
                    <tr>
                      <th>Order ID</th>
                      <th>Gig</th>
                      <th>Buyer</th>
                      <th>Status</th>
                      <th>Price</th>
                      <th>Due Date</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {orders.map(order => (
                      <tr key={order.id}>
                        <td>#{order.id}</td>
                        <td>{order.gigTitle}</td>
                        <td>@{order.buyer}</td>
                        <td>{getStatusBadge(order.status)}</td>
                        <td>${order.price}</td>
                        <td>{order.dueDate}</td>
                        <td>
                          <button className="btn-small">View</button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </>
          )}

          {activeTab === 'gigs' && (
            <>
              <div className="section-header">
                <h1>My Gigs</h1>
                <Link to="/gigs/create" className="btn-primary">+ Create New Gig</Link>
              </div>

              {myGigs.length === 0 ? (
                <div className="empty-state">
                  <p>You haven't created any gigs yet.</p>
                  <Link to="/gigs/create" className="btn-primary">Create Your First Gig</Link>
                </div>
              ) : (
                <div className="gigs-grid">
                  {myGigs.map(gig => {
                    const gigImage = (gig.images && gig.images[0]) || (gig.Images && gig.Images[0]) || 'https://via.placeholder.com/300x200';
                    // Backend DTO returns packages as an OBJECT { basic, standard, premium }, not an array
                    const pkgs = gig.packages || gig.Packages || {};
                    const basicPkg = pkgs.basic || pkgs.Basic || {};
                    const price = basicPkg.price || basicPkg.Price || 0;

                    return (
                      <div key={gig.id || gig.Id} className="gig-card-mini">
                        <img src={gigImage.startsWith('http') ? gigImage : `http://localhost:5001${gigImage}`} alt={gig.title || gig.Title}
                          onError={(e) => { e.target.onerror = null; e.target.src = 'https://placehold.co/300x200?text=Gig'; }} />
                        <div className="gig-card-content">
                          <h3>{gig.title || gig.Title}</h3>
                          <div className="gig-stats">
                            <span>⭐ {parseFloat(gig.rating || gig.Rating || 0).toFixed(1)}</span>
                            <span>📦 {gig.ordersInQueue || gig.OrdersInQueue || 0} in queue</span>
                          </div>
                          <p className="gig-price">From ${price}</p>
                          <div className="gig-actions">
                            <Link to={`/gig/${gig.slug || gig.Slug}`} className="btn-small">View</Link>
                            <button className="btn-small btn-outline">{gig.isActive || gig.IsActive ? 'Pause' : 'Activate'}</button>
                          </div>
                        </div>
                      </div>
                    );
                  })}
                </div>
              )}
            </>
          )}

          {activeTab === 'earnings' && (
            <>
              <h1>Earnings</h1>
              <div className="earnings-summary">
                <div className="earnings-card">
                  <h3>Available for Withdrawal</h3>
                  <p className="amount">$1,250.00</p>
                  <button className="btn-primary">Withdraw</button>
                </div>
                <div className="earnings-card">
                  <h3>Pending Clearance</h3>
                  <p className="amount">$450.00</p>
                  <span className="note">Cleared in 14 days</span>
                </div>
                <div className="earnings-card">
                  <h3>Total Earned (This Month)</h3>
                  <p className="amount">$750.00</p>
                  <span className="trend positive">+25% from last month</span>
                </div>
              </div>
            </>
          )}
        </main>
      </div>
    </div>
  );
}

export default Dashboard;
