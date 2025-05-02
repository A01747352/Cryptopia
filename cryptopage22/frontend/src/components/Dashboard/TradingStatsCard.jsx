import React from 'react';
import { FaExchangeAlt, FaChartLine } from 'react-icons/fa';

const TradingStatsCard = ({ data }) => {
  if (!data) return null;
  
  // Formatear números
  const formattedTrades = data.totalTrades.toLocaleString();
  const formattedVolume = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(data.totalVolumen);
  
  return (
    <div className="card trading-stats-card">
      <h2>Trading Activity</h2>
      <div className="trading-stats-grid">
        <div className="trading-stat">
          <div className="stat-icon">
            <FaExchangeAlt />
          </div>
          <div className="stat-data">
            <h3>{formattedTrades}</h3>
            <p>Total Trades</p>
          </div>
        </div>
        <div className="trading-stat">
          <div className="stat-icon">
            <FaChartLine />
          </div>
          <div className="stat-data">
            <h3>{formattedVolume}</h3>
            <p>Trading Volume</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TradingStatsCard;