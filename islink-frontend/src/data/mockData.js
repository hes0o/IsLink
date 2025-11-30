/**
 * Mock Data for IsLink Development
 * This data simulates what we'll get from the backend API
 */

// Categories
export const categories = [
  {
    id: "cat_1",
    name: "Graphics & Design",
    slug: "graphics-design",
    icon: "🎨",
    image: "/images/categories/design.jpg",
    subcategories: [
      { id: "sub_1", name: "Logo Design", slug: "logo-design" },
      { id: "sub_2", name: "Brand Style Guides", slug: "brand-style" },
      { id: "sub_3", name: "Business Cards", slug: "business-cards" },
      { id: "sub_4", name: "Illustration", slug: "illustration" }
    ]
  },
  {
    id: "cat_2",
    name: "Programming & Tech",
    slug: "programming-tech",
    icon: "💻",
    image: "/images/categories/programming.jpg",
    subcategories: [
      { id: "sub_5", name: "Web Development", slug: "web-development" },
      { id: "sub_6", name: "Mobile Apps", slug: "mobile-apps" },
      { id: "sub_7", name: "E-Commerce", slug: "e-commerce" },
      { id: "sub_8", name: "Desktop Apps", slug: "desktop-apps" }
    ]
  },
  {
    id: "cat_3",
    name: "Digital Marketing",
    slug: "digital-marketing",
    icon: "📈",
    image: "/images/categories/marketing.jpg",
    subcategories: [
      { id: "sub_9", name: "Social Media Marketing", slug: "social-media" },
      { id: "sub_10", name: "SEO", slug: "seo" },
      { id: "sub_11", name: "Content Marketing", slug: "content-marketing" }
    ]
  },
  {
    id: "cat_4",
    name: "Writing & Translation",
    slug: "writing-translation",
    icon: "✍️",
    image: "/images/categories/writing.jpg",
    subcategories: [
      { id: "sub_12", name: "Articles & Blog Posts", slug: "articles" },
      { id: "sub_13", name: "Translation", slug: "translation" },
      { id: "sub_14", name: "Copywriting", slug: "copywriting" }
    ]
  },
  {
    id: "cat_5",
    name: "Video & Animation",
    slug: "video-animation",
    icon: "🎬",
    image: "/images/categories/video.jpg",
    subcategories: [
      { id: "sub_15", name: "Video Editing", slug: "video-editing" },
      { id: "sub_16", name: "Animation", slug: "animation" },
      { id: "sub_17", name: "Motion Graphics", slug: "motion-graphics" }
    ]
  },
  {
    id: "cat_6",
    name: "Music & Audio",
    slug: "music-audio",
    icon: "🎵",
    image: "/images/categories/music.jpg",
    subcategories: [
      { id: "sub_18", name: "Voice Over", slug: "voice-over" },
      { id: "sub_19", name: "Mixing & Mastering", slug: "mixing-mastering" },
      { id: "sub_20", name: "Podcast Editing", slug: "podcast-editing" }
    ]
  },
  {
    id: "cat_7",
    name: "Business",
    slug: "business",
    icon: "💼",
    image: "/images/categories/business.jpg",
    subcategories: [
      { id: "sub_21", name: "Virtual Assistant", slug: "virtual-assistant" },
      { id: "sub_22", name: "Data Entry", slug: "data-entry" },
      { id: "sub_23", name: "Business Plans", slug: "business-plans" }
    ]
  },
  {
    id: "cat_8",
    name: "AI Services",
    slug: "ai-services",
    icon: "🤖",
    image: "/images/categories/ai.jpg",
    subcategories: [
      { id: "sub_24", name: "AI Applications", slug: "ai-applications" },
      { id: "sub_25", name: "AI Chatbots", slug: "ai-chatbots" },
      { id: "sub_26", name: "AI Content", slug: "ai-content" }
    ]
  }
];

// Sample Users (Sellers)
export const users = [
  {
    id: "user_1",
    username: "designpro",
    email: "design@example.com",
    fullName: "Sarah Johnson",
    avatar: "https://randomuser.me/api/portraits/women/44.jpg",
    role: "seller",
    country: "United States",
    memberSince: "2023-03-15",
    bio: "Award-winning graphic designer with 8+ years of experience in branding and visual identity.",
    skills: ["Logo Design", "Branding", "UI/UX", "Illustration"],
    languages: [
      { name: "English", level: "Native" },
      { name: "Spanish", level: "Conversational" }
    ],
    rating: 4.9,
    reviewCount: 234,
    completedOrders: 312,
    isOnline: true
  },
  {
    id: "user_2",
    username: "codemaster",
    email: "code@example.com",
    fullName: "Ahmed Hassan",
    avatar: "https://randomuser.me/api/portraits/men/32.jpg",
    role: "seller",
    country: "Egypt",
    memberSince: "2022-08-20",
    bio: "Full-stack developer specializing in React, Node.js, and cloud solutions. Building scalable web applications.",
    skills: ["React", "Node.js", "Python", "AWS", "MongoDB"],
    languages: [
      { name: "Arabic", level: "Native" },
      { name: "English", level: "Fluent" }
    ],
    rating: 5.0,
    reviewCount: 189,
    completedOrders: 201,
    isOnline: true
  },
  {
    id: "user_3",
    username: "wordsmith",
    email: "word@example.com",
    fullName: "Emily Chen",
    avatar: "https://randomuser.me/api/portraits/women/68.jpg",
    role: "seller",
    country: "Canada",
    memberSince: "2023-01-10",
    bio: "Professional content writer and copywriter. I help brands tell their story through compelling content.",
    skills: ["Blog Writing", "SEO Writing", "Copywriting", "Editing"],
    languages: [
      { name: "English", level: "Native" },
      { name: "French", level: "Fluent" }
    ],
    rating: 4.8,
    reviewCount: 156,
    completedOrders: 178,
    isOnline: false
  }
];

// Sample Gigs
export const gigs = [
  {
    id: "gig_1",
    sellerId: "user_1",
    seller: users[0],
    title: "I will design a modern minimalist logo for your brand",
    slug: "modern-minimalist-logo-design",
    description: "Get a unique, professional logo that perfectly represents your brand. I create clean, modern designs that stand out and leave a lasting impression.",
    categoryId: "cat_1",
    subcategoryId: "sub_1",
    images: [
      "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/103228970/original/a8a518449f2cc0a9e9cadb5e9df3a5bc10e53c0c/design-3-unique-minimalist-logo-with-source-file.jpg",
      "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs2/103228970/original/6dfcd8a95dbdc49141f8cd0de6e8469e5c4e9ef1/design-3-unique-minimalist-logo-with-source-file.jpg"
    ],
    packages: {
      basic: {
        name: "Basic",
        price: 35,
        deliveryDays: 5,
        revisions: 2,
        features: ["1 Logo Concept", "PNG File", "Vector File"]
      },
      standard: {
        name: "Standard",
        price: 75,
        deliveryDays: 3,
        revisions: 5,
        features: ["3 Logo Concepts", "PNG + Vector Files", "Source File", "Social Media Kit"]
      },
      premium: {
        name: "Premium",
        price: 150,
        deliveryDays: 2,
        revisions: "Unlimited",
        features: ["5 Logo Concepts", "All File Formats", "Source File", "Social Media Kit", "Brand Guidelines", "Stationery Design"]
      }
    },
    tags: ["logo", "minimalist", "modern", "branding", "business"],
    rating: 4.9,
    reviewCount: 127,
    ordersInQueue: 5,
    createdAt: "2024-01-15",
    isActive: true
  },
  {
    id: "gig_2",
    sellerId: "user_2",
    seller: users[1],
    title: "I will build a responsive React website for your business",
    slug: "responsive-react-website",
    description: "Get a fast, modern, and fully responsive website built with React. Clean code, SEO-friendly, and optimized for performance.",
    categoryId: "cat_2",
    subcategoryId: "sub_5",
    images: [
      "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/299377270/original/fdacb8ff12c4d9a9a993892579dcaae24efbc621/be-your-frontend-developer-using-html-css-tailwind-react.png"
    ],
    packages: {
      basic: {
        name: "Basic",
        price: 150,
        deliveryDays: 7,
        revisions: 2,
        features: ["1-3 Pages", "Responsive Design", "Contact Form"]
      },
      standard: {
        name: "Standard",
        price: 350,
        deliveryDays: 10,
        revisions: 4,
        features: ["4-6 Pages", "Responsive Design", "Contact Form", "Basic SEO", "Analytics Setup"]
      },
      premium: {
        name: "Premium",
        price: 600,
        deliveryDays: 14,
        revisions: "Unlimited",
        features: ["7-10 Pages", "Responsive Design", "Contact Form", "Advanced SEO", "Analytics", "Admin Panel", "Database Integration"]
      }
    },
    tags: ["react", "website", "frontend", "responsive", "javascript"],
    rating: 5.0,
    reviewCount: 89,
    ordersInQueue: 3,
    createdAt: "2024-02-01",
    isActive: true
  },
  {
    id: "gig_3",
    sellerId: "user_3",
    seller: users[2],
    title: "I will write SEO blog posts and articles for your website",
    slug: "seo-blog-posts-articles",
    description: "Engaging, well-researched content that ranks. I write SEO-optimized articles that drive traffic and convert readers into customers.",
    categoryId: "cat_4",
    subcategoryId: "sub_12",
    images: [
      "https://fiverr-res.cloudinary.com/images/t_main1,q_auto,f_auto,q_auto,f_auto/gigs/197abordar5971/original/c5c5d624e43ad6bdfa5eb2e0a56e1a1d4c2a4b07/write-1500-words-seo-article-or-blog-post.jpg"
    ],
    packages: {
      basic: {
        name: "Basic",
        price: 25,
        deliveryDays: 2,
        revisions: 1,
        features: ["500 Words", "SEO Optimized", "1 Keyword"]
      },
      standard: {
        name: "Standard",
        price: 50,
        deliveryDays: 3,
        revisions: 2,
        features: ["1000 Words", "SEO Optimized", "3 Keywords", "Meta Description"]
      },
      premium: {
        name: "Premium",
        price: 100,
        deliveryDays: 4,
        revisions: 3,
        features: ["2000 Words", "SEO Optimized", "5 Keywords", "Meta Description", "Images", "Internal Links"]
      }
    },
    tags: ["writing", "blog", "seo", "content", "articles"],
    rating: 4.8,
    reviewCount: 203,
    ordersInQueue: 8,
    createdAt: "2024-01-28",
    isActive: true
  }
];

// Sample Reviews
export const reviews = [
  {
    id: "review_1",
    orderId: "order_1",
    gigId: "gig_1",
    buyerId: "user_buyer_1",
    buyerName: "Michael R.",
    buyerAvatar: "https://randomuser.me/api/portraits/men/75.jpg",
    buyerCountry: "United Kingdom",
    sellerId: "user_1",
    rating: 5,
    comment: "Absolutely fantastic work! Sarah understood my vision perfectly and delivered a stunning logo. Communication was excellent throughout the project. Highly recommended!",
    sellerResponse: "Thank you so much Michael! It was a pleasure working with you. Best of luck with your new brand! 🎉",
    createdAt: "2024-03-10"
  },
  {
    id: "review_2",
    orderId: "order_2",
    gigId: "gig_1",
    buyerId: "user_buyer_2",
    buyerName: "Lisa T.",
    buyerAvatar: "https://randomuser.me/api/portraits/women/23.jpg",
    buyerCountry: "Australia",
    sellerId: "user_1",
    rating: 5,
    comment: "Professional, creative, and fast! The logo exceeded my expectations. Will definitely work with Sarah again.",
    sellerResponse: "",
    createdAt: "2024-03-08"
  },
  {
    id: "review_3",
    orderId: "order_3",
    gigId: "gig_2",
    buyerId: "user_buyer_3",
    buyerName: "David K.",
    buyerAvatar: "https://randomuser.me/api/portraits/men/45.jpg",
    buyerCountry: "Germany",
    sellerId: "user_2",
    rating: 5,
    comment: "Ahmed is an excellent developer! He built exactly what I needed and even added some features I hadn't thought of. The code is clean and well-documented.",
    sellerResponse: "Thank you David! Great working with you. Let me know if you need any updates! 🚀",
    createdAt: "2024-03-05"
  }
];

