import { Routes, Route } from 'react-router-dom';
import { Layout } from './components/layout';
import Home from './pages/Home';
import Gigs from './pages/Gigs';
import GigDetails from './pages/GigDetails';
import { Login, Register } from './pages/Auth';
import Profile from './pages/Profile';
import './styles/variables.css';
import './App.css';

function App() {
  return (
    <Routes>
      {/* Main Layout Routes */}
      <Route path="/" element={<Layout />}>
        <Route index element={<Home />} />
        <Route path="gigs" element={<Gigs />} />
        <Route path="gig/:slug" element={<GigDetails />} />
        <Route path="profile/:username" element={<Profile />} />
      </Route>

      {/* Auth Routes (No Layout - Full Page) */}
      <Route path="/auth/login" element={<Login />} />
      <Route path="/auth/register" element={<Register />} />
    </Routes>
  );
}

export default App;
