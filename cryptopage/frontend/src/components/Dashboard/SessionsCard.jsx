import React from 'react';
import { FaStar } from 'react-icons/fa';

const SessionsCard = ({ sessionsCount }) => {
  return (
    <div className="card sessions-card">
      <h2>Sessions</h2>
      <div className="sessions-count">
        <h2>{sessionsCount || '5,840'}</h2>
      </div>
      <div className="rating">
        <FaStar className="star filled" />
        <FaStar className="star filled" />
        <FaStar className="star filled" />
        <FaStar className="star half" />
        <FaStar className="star" />
        <span>4.7</span>
      </div>
    </div>
  );
};

export default SessionsCard;