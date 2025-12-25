import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './context';
import { Layout } from './components/layout';
import Home from './pages/Home';
import Gigs from './pages/Gigs';
import GigDetails from './pages/GigDetails';
import CreateGig from './pages/CreateGig';
import { Login, Register } from './pages/Auth';
import Profile from './pages/Profile';
import Dashboard from './pages/Dashboard';
import Messages from './pages/Messages';
import LinkerAI from './pages/LinkerAI';
import NotFound from './pages/NotFound';
import './styles/variables.css';
import './App.css';

// Protected Route Component
function ProtectedRoute({ children }) {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="loading-screen">
        <div className="spinner"></div>
        <p>Loading...</p>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/auth/login" replace />;
  }

  return children;
}

function App() {
  return (
    <Routes>
      {/* Main Layout Routes */}
      <Route path="/" element={<Layout />}>
        <Route index element={<Home />} />
        <Route path="gigs" element={<Gigs />} />
        <Route path="gig/:slug" element={<GigDetails />} />
        <Route path="profile/:username" element={<Profile />} />
        <Route path="linkerai" element={
          <ProtectedRoute>
            <LinkerAI />
          </ProtectedRoute>
        } />
        <Route path="gigs/create" element={
          <ProtectedRoute>
            <CreateGig />
          </ProtectedRoute>
        } />

        {/* Protected Routes */}
        <Route path="dashboard" element={
          <ProtectedRoute>
            <Dashboard />
          </ProtectedRoute>
        } />
        <Route path="messages" element={
          <ProtectedRoute>
            <Messages />
          </ProtectedRoute>
        } />
        <Route path="messages/:conversationId" element={
          <ProtectedRoute>
            <Messages />
          </ProtectedRoute>
        } />
      </Route>

      {/* Auth Routes (No Layout - Full Page) */}
      <Route path="/auth/login" element={<Login />} />
      <Route path="/auth/register" element={<Register />} />

      {/* 404 Route */}
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
}

export default App;
