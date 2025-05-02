import React from 'react';
import { FaTrophy, FaMedal, FaAward } from 'react-icons/fa';

const UserRankingCard = ({ data }) => {
  if (!data || data.length === 0) return null;
  
  // Iconos para los primeros tres lugares
  const getRankIcon = (index) => {
    switch(index) {
      case 0: return <FaTrophy style={{ color: '#FFD700' }} />;
      case 1: return <FaMedal style={{ color: '#C0C0C0' }} />;
      case 2: return <FaAward style={{ color: '#CD7F32' }} />;
      default: return <span className="rank-number">{index + 1}</span>;
    }
  };
  
  return (
    <div className="card ranking-card">
      <h2>User Ranking</h2>
      <div className="ranking-list">
        <table className="ranking-table">
          <thead>
            <tr>
              <th>Rank</th>
              <th>Username</th>
              <th>Score</th>
            </tr>
          </thead>
          <tbody>
            {data.map((user, index) => (
              <tr key={index} className={index < 3 ? 'top-rank' : ''}>
                <td className="rank-icon">{getRankIcon(index)}</td>
                <td className="username">{user.username}</td>
                <td className="score">{user.totalPuntaje.toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default UserRankingCard;