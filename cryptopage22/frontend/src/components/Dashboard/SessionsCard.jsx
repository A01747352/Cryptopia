import React from 'react';
import { FaStar } from 'react-icons/fa';

const SessionsCard = ({ sessionsCount }) => {
  return (
    <div className=" sessions-card">
      <h2>Sessions</h2>
      <div className="sessions-count">
        <h2>{sessionsCount || '5,840'}</h2>
      </div>
    </div>
  );
};

export default SessionsCard;