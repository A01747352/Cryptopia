import React from 'react';

/**
 * ChartCard Component
 * 
 * A container for chart visualizations with title and optional subtitle
 * 
 * @param {Object} props
 * @param {string} props.title - Card title
 * @param {string} [props.subtitle] - Optional subtitle or additional information
 * @param {React.ReactNode} props.children - Chart component or other content
 */
const ChartCard = ({ title, subtitle, children }) => {
  return (
    <div className="card">
      <div className="card-title">
        {title}
        {subtitle && <span className="card-subtitle">{subtitle}</span>}
      </div>
      <div className="chart-container">
        {children}
      </div>
    </div>
  );
};

export default ChartCard;