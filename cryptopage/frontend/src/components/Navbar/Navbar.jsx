import React from 'react';
import { FaSearch, FaBell } from 'react-icons/fa';
import './Navbar.css';

const Navbar = () => {
  return (
    <div className="navbar">
      <div className="search-bar">
        <FaSearch />
        <input type="text" placeholder="Search..." />
      </div>
      <div className="nav-right">
        <div className="notifications">
          <FaBell />
        </div>
        <div className="user-profile">
          <img src="https://i.pravatar.cc/100" alt="User Profile" />
        </div>
      </div>
    </div>
  );
};

export default Navbar;