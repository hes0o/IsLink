import { Outlet } from 'react-router-dom';
import Header from './Header';
import Footer from './Footer';
import { ChatBot } from '../common';
import './Layout.css';

function Layout() {
  return (
    <div className="layout">
      <Header />
      <main className="main-content">
        <Outlet />
      </main>
      <Footer />
      <ChatBot />
    </div>
  );
}

export default Layout;
