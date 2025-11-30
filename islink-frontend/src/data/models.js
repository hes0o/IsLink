/**
 * IsLink Data Models
 * These define the structure of data we'll use throughout the app
 */

/**
 * User Model
 * Represents both buyers and sellers on the platform
 * 
 * Example:
 * {
 *   id: "user_123",
 *   username: "johndesigner",
 *   email: "john@example.com",
 *   fullName: "John Doe",
 *   avatar: "https://...",
 *   role: "seller",
 *   country: "United States",
 *   memberSince: "2024-01-15",
 *   bio: "Professional graphic designer...",
 *   skills: ["Logo Design", "Branding", "UI/UX"],
 *   languages: [{ name: "English", level: "Native" }],
 *   rating: 4.9,
 *   reviewCount: 127,
 *   completedOrders: 156,
 *   isOnline: true
 * }
 */
export const UserModel = {
  id: "",
  username: "",
  email: "",
  fullName: "",
  avatar: "",
  role: "buyer", // "buyer" | "seller" | "admin"
  country: "",
  memberSince: "",
  bio: "",
  skills: [],
  languages: [],
  rating: 0,
  reviewCount: 0,
  completedOrders: 0,
  isOnline: false
};

/**
 * Category Model
 * Service categories on the platform
 * 
 * Example:
 * {
 *   id: "cat_1",
 *   name: "Graphics & Design",
 *   slug: "graphics-design",
 *   icon: "palette",
 *   image: "https://...",
 *   subcategories: [
 *     { id: "sub_1", name: "Logo Design", slug: "logo-design" },
 *     { id: "sub_2", name: "Brand Style Guides", slug: "brand-style" }
 *   ]
 * }
 */
export const CategoryModel = {
  id: "",
  name: "",
  slug: "",
  icon: "",
  image: "",
  subcategories: []
};

/**
 * Gig Model (Service)
 * Services offered by sellers
 * 
 * Example:
 * {
 *   id: "gig_456",
 *   sellerId: "user_123",
 *   title: "I will design a modern logo for your brand",
 *   slug: "modern-logo-design",
 *   description: "Professional logo design...",
 *   categoryId: "cat_1",
 *   subcategoryId: "sub_1",
 *   images: ["https://...", "https://..."],
 *   packages: {
 *     basic: { name: "Basic", price: 25, deliveryDays: 3, revisions: 1, features: ["1 Logo Concept"] },
 *     standard: { name: "Standard", price: 50, deliveryDays: 2, revisions: 3, features: ["3 Logo Concepts", "Source Files"] },
 *     premium: { name: "Premium", price: 100, deliveryDays: 1, revisions: "Unlimited", features: ["5 Logo Concepts", "Source Files", "Social Media Kit"] }
 *   },
 *   tags: ["logo", "branding", "modern"],
 *   rating: 4.9,
 *   reviewCount: 89,
 *   ordersInQueue: 3,
 *   createdAt: "2024-01-20",
 *   isActive: true
 * }
 */
export const GigModel = {
  id: "",
  sellerId: "",
  title: "",
  slug: "",
  description: "",
  categoryId: "",
  subcategoryId: "",
  images: [],
  packages: {
    basic: { name: "Basic", price: 0, deliveryDays: 0, revisions: 0, features: [] },
    standard: { name: "Standard", price: 0, deliveryDays: 0, revisions: 0, features: [] },
    premium: { name: "Premium", price: 0, deliveryDays: 0, revisions: 0, features: [] }
  },
  tags: [],
  rating: 0,
  reviewCount: 0,
  ordersInQueue: 0,
  createdAt: "",
  isActive: true
};

/**
 * Order Model
 * Represents a transaction between buyer and seller
 * 
 * Status flow: pending -> in_progress -> delivered -> completed
 *              pending -> cancelled
 *              delivered -> revision_requested -> in_progress
 */
export const OrderModel = {
  id: "",
  gigId: "",
  buyerId: "",
  sellerId: "",
  packageType: "basic", // "basic" | "standard" | "premium"
  price: 0,
  serviceFee: 0,
  totalPrice: 0,
  status: "pending", // "pending" | "in_progress" | "delivered" | "completed" | "cancelled" | "revision_requested"
  requirements: "",
  deliveryDate: "",
  createdAt: "",
  completedAt: "",
  deliveries: [] // Array of delivery submissions
};

/**
 * Review Model
 * Feedback left after order completion
 */
export const ReviewModel = {
  id: "",
  orderId: "",
  gigId: "",
  buyerId: "",
  sellerId: "",
  rating: 0, // 1-5
  comment: "",
  sellerResponse: "",
  createdAt: ""
};

/**
 * Message Model
 * Direct messages between users
 */
export const MessageModel = {
  id: "",
  conversationId: "",
  senderId: "",
  receiverId: "",
  content: "",
  attachments: [],
  isRead: false,
  createdAt: ""
};

/**
 * Conversation Model
 * A conversation thread between two users
 */
export const ConversationModel = {
  id: "",
  participants: [], // [userId1, userId2]
  lastMessage: "",
  lastMessageAt: "",
  unreadCount: 0
};

