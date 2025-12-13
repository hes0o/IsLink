/**
 * API Service - Connects frontend to .NET backend
 */

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5001/api';

// Helper to get auth token
const getToken = () => localStorage.getItem('token');

// Helper for API requests
const apiRequest = async (endpoint, options = {}) => {
  const token = getToken();
  
  const config = {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
      ...options.headers,
    },
  };

  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
    const data = await response.json();
    
    if (!response.ok) {
      throw new Error(data.message || 'Something went wrong');
    }
    
    return data;
  } catch (error) {
    console.error('API Error:', error);
    throw error;
  }
};

// ============================================
// AUTH API
// ============================================

export const authAPI = {
  register: (data) => apiRequest('/auth/register', {
    method: 'POST',
    body: JSON.stringify(data),
  }),

  login: (data) => apiRequest('/auth/login', {
    method: 'POST',
    body: JSON.stringify(data),
  }),

  logout: () => apiRequest('/auth/logout', {
    method: 'POST',
  }),

  getMe: () => apiRequest('/auth/me'),
};

// ============================================
// USERS API
// ============================================

export const usersAPI = {
  getByUsername: (username) => apiRequest(`/users/${username}`),

  getSellers: (params = {}) => {
    const query = new URLSearchParams(params).toString();
    return apiRequest(`/users/sellers${query ? `?${query}` : ''}`);
  },

  updateProfile: (data) => apiRequest('/users/profile', {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
};

// ============================================
// GIGS API
// ============================================

export const gigsAPI = {
  getAll: (params = {}) => {
    const query = new URLSearchParams(params).toString();
    return apiRequest(`/gigs${query ? `?${query}` : ''}`);
  },

  getBySlug: (slug) => apiRequest(`/gigs/${slug}`),

  getBySeller: (username) => apiRequest(`/gigs/seller/${username}`),

  create: (data) => apiRequest('/gigs', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
};

// ============================================
// CATEGORIES API
// ============================================

export const categoriesAPI = {
  getAll: () => apiRequest('/categories'),

  getBySlug: (slug) => apiRequest(`/categories/${slug}`),
};

// ============================================
// ORDERS API
// ============================================

export const ordersAPI = {
  create: (data) => apiRequest('/orders', {
    method: 'POST',
    body: JSON.stringify(data),
  }),

  getAll: (params = {}) => {
    const query = new URLSearchParams(params).toString();
    return apiRequest(`/orders${query ? `?${query}` : ''}`);
  },

  getById: (id) => apiRequest(`/orders/${id}`),

  updateStatus: (id, status) => apiRequest(`/orders/${id}/status`, {
    method: 'PATCH',
    body: JSON.stringify({ status }),
  }),
};

// ============================================
// REVIEWS API
// ============================================

export const reviewsAPI = {
  getByGig: (gigId, params = {}) => {
    const query = new URLSearchParams(params).toString();
    return apiRequest(`/reviews/gig/${gigId}${query ? `?${query}` : ''}`);
  },

  getBySeller: (username, params = {}) => {
    const query = new URLSearchParams(params).toString();
    return apiRequest(`/reviews/seller/${username}${query ? `?${query}` : ''}`);
  },

  create: (data) => apiRequest('/reviews', {
    method: 'POST',
    body: JSON.stringify(data),
  }),

  addResponse: (reviewId, response) => apiRequest(`/reviews/${reviewId}/response`, {
    method: 'POST',
    body: JSON.stringify({ response }),
  }),
};

// ============================================
// MESSAGES API
// ============================================

export const messagesAPI = {
  getConversations: () => apiRequest('/messages/conversations'),

  getOrCreateConversation: (data) => apiRequest('/messages/conversations', {
    method: 'POST',
    body: JSON.stringify(data),
  }),

  getMessages: (conversationId, params = {}) => {
    const query = new URLSearchParams(params).toString();
    return apiRequest(`/messages/conversations/${conversationId}${query ? `?${query}` : ''}`);
  },

  sendMessage: (conversationId, data) => apiRequest(`/messages/conversations/${conversationId}`, {
    method: 'POST',
    body: JSON.stringify(data),
  }),
};

// ============================================
// NOTIFICATIONS API
// ============================================

export const notificationsAPI = {
  getAll: (params = {}) => {
    const query = new URLSearchParams(params).toString();
    return apiRequest(`/notifications${query ? `?${query}` : ''}`);
  },

  getUnreadCount: () => apiRequest('/notifications/unread-count'),

  markAllAsRead: () => apiRequest('/notifications/mark-all-read', {
    method: 'POST',
  }),
};

export default {
  auth: authAPI,
  users: usersAPI,
  gigs: gigsAPI,
  categories: categoriesAPI,
  orders: ordersAPI,
  reviews: reviewsAPI,
  messages: messagesAPI,
  notifications: notificationsAPI,
};
