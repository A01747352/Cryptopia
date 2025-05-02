import React from 'react';

/**
 * KeyMetricCard Component
 * 
 * Displays a single KPI (Key Performance Indicator) with an icon, title and value
 * 
 * @param {Object} props
 * @param {React.ReactNode} props.icon - Icon component to display
 * @param {string} props.title - Title of the metric
 * @param {string} props.value - Value to display
 * @param {string} props.color - Color theme (primary, secondary, success, warning, danger)
 */
const KeyMetricCard = ({ icon, title, value, color }) => {
  return (
    <div className="metric-card">
      <div className={`metric-icon ${color}`}>
        {icon}
      </div>
      <div className="metric-content">
        <div className="metric-title">{title}</div>
        <div className="metric-value">{value}</div>
      </div>
    </div>
  );
};

export default KeyMetricCard;