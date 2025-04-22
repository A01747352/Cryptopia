import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { FaGamepad, FaChartBar, FaUsers, FaCog } from 'react-icons/fa';
import './Sidebar.css';

const Sidebar = () => {
  const location = useLocation();
  
  return (
    <div className="sidebar">
      <div className="logo">
        <FaGamepad />
        <h1>Cryptopia</h1>
      </div>
      <nav className="nav-menu">
        <Link to="/" className={location.pathname === '/' ? 'active' : ''}>
          <span>Dashboard</span>
        </Link>
        <Link to="/users" className={location.pathname === '/users' ? 'active' : ''}>
          <span>Users</span>
        </Link>
        <Link to="/analytics" className={location.pathname === '/analytics' ? 'active' : ''}>
          <span>Analytics</span>
        </Link>
        <Link to="/settings" className={location.pathname === '/settings' ? 'active' : ''}>
          <span>Settings</span>
        </Link>
      </nav>
    </div>
  );
};

export default Sidebar;