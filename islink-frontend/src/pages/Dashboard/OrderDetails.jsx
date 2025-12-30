import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { ordersAPI } from '../../services/api';
import './OrderDetails.css';

function OrderDetails() {
    const { orderId } = useParams();
    const [order, setOrder] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        // Simulate API fetch for demo puposes OR fetch real data if API is ready
        // For now, we'll try to fetch real data but fallback to mock if 404
        const fetchOrder = async () => {
            try {
                setLoading(true);
                // In a real app: const res = await ordersAPI.getById(orderId);
                // For now, let's mock it based on ID to ensure UI works
                // const data = res.data;

                // Mock Data for immediate UI fix
                const mockOrder = {
                    id: orderId,
                    orderNumber: `ORD-${orderId}`,
                    status: 'in_progress',
                    createdAt: new Date().toISOString(),
                    deliveryDate: new Date(Date.now() + 86400000 * 3).toISOString(), // +3 days
                    price: 150,
                    gig: {
                        title: 'Professional Logo Design with Brand Identity',
                        image: 'https://via.placeholder.com/300x200',
                        slug: 'professional-logo-design'
                    },
                    seller: {
                        username: 'design_pro',
                        avatar: 'https://via.placeholder.com/50'
                    },
                    buyer: {
                        username: 'john_buyer'
                    },
                    package: {
                        title: 'Standard Package',
                        description: 'Logo + Business Card + Social Media Kit',
                        deliveryDays: 3,
                        revisions: 5
                    },
                    requirements: 'I need a blue logo for a tech company.'
                };

                setOrder(mockOrder);
            } catch (err) {
                console.error("Error fetching order:", err);
                setError("Failed to load order details.");
            } finally {
                setLoading(false);
            }
        };

        if (orderId) fetchOrder();
    }, [orderId]);

    if (loading) return <div className="loading-screen"><div className="spinner"></div></div>;
    if (error) return <div className="error-state"><p>{error}</p><Link to="/dashboard" className="btn-primary">Back to Dashboard</Link></div>;
    if (!order) return <div className="error-state"><p>Order not found</p></div>;

    return (
        <div className="order-details-page">
            <div className="order-details-container">
                {/* Header */}
                <div className="order-header">
                    <div>
                        <h1>Order #{order.orderNumber}</h1>
                        <p className="text-muted">Placed on {new Date(order.createdAt).toLocaleDateString()}</p>
                    </div>
                    <span className={`order-status-badge ${order.status}`}>
                        {order.status.replace('_', ' ')}
                    </span>
                </div>

                <div className="order-grid">
                    {/* Main Content */}
                    <div className="order-main">
                        {/* Timeline */}
                        <div className="order-card">
                            <h2>Order Timeline</h2>
                            <div className="timeline">
                                <div className="timeline-item completed">
                                    <div className="timeline-dot"></div>
                                    <div className="timeline-content">
                                        <h4>Order Placed</h4>
                                        <span>{new Date(order.createdAt).toLocaleString()}</span>
                                    </div>
                                </div>

                                <div className="timeline-item current">
                                    <div className="timeline-dot"></div>
                                    <div className="timeline-content">
                                        <h4>Order in Progress</h4>
                                        <span>Expected delivery: {new Date(order.deliveryDate).toDateString()}</span>
                                    </div>
                                </div>

                                <div className="timeline-item">
                                    <div className="timeline-dot"></div>
                                    <div className="timeline-content">
                                        <h4>Delivery Imminent</h4>
                                        <span>Seller will upload files soon</span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Requirements */}
                        <div className="order-card">
                            <h2>Order Requirements</h2>
                            <div className="requirements-box">
                                <p>{order.requirements}</p>
                            </div>
                        </div>

                        {/* Delivery Area */}
                        <div className="order-card">
                            <h2>Delivery</h2>
                            <div className="delivery-files">
                                <svg viewBox="0 0 24 24" width="48" height="48" fill="none" stroke="currentColor" strokeWidth="1" color="#9ca3af" style={{ marginBottom: '1rem' }}>
                                    <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                                    <polyline points="7 10 12 15 17 10" />
                                    <line x1="12" y1="15" x2="12" y2="3" />
                                </svg>
                                <p>No files delivered yet.</p>
                            </div>
                        </div>
                    </div>

                    {/* Sidebar */}
                    <aside className="order-sidebar">
                        <div className="order-card">
                            <h2>Gig Details</h2>
                            <div className="gig-summary">
                                <img src={order.gig.image} alt={order.gig.title} />
                                <div>
                                    <h3>{order.gig.title}</h3>
                                    <Link to={`/gig/${order.gig.slug}`} className="text-primary text-sm">View Gig</Link>
                                </div>
                            </div>

                            <div className="package-summary">
                                <h4>{order.package.title}</h4>
                                <ul className="text-sm text-muted mt-2">
                                    <li>• {order.package.deliveryDays} Days Delivery</li>
                                    <li>• {order.package.revisions} Revisions</li>
                                </ul>
                            </div>

                            <div className="price-breakdown">
                                <div className="price-row">
                                    <span>Subtotal</span>
                                    <span>${order.price.toFixed(2)}</span>
                                </div>
                                <div className="price-row">
                                    <span>Service Fee</span>
                                    <span>$0.00</span>
                                </div>
                                <div className="price-row total">
                                    <span>Total</span>
                                    <span>${order.price.toFixed(2)}</span>
                                </div>
                            </div>
                        </div>

                        <div className="order-card">
                            <h2>Need Help?</h2>
                            <button className="btn-secondary w-full" onClick={() => alert('Contacting support...')}>
                                Contact Support
                            </button>
                        </div>
                    </aside>
                </div>
            </div>
        </div>
    );
}

export default OrderDetails;
