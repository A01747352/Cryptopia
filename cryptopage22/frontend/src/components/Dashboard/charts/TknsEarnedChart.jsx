import React from 'react';
import { Bar } from 'react-chartjs-2';
import ChartConfigs from '../ChartConfigs';

/**
 * TknsEarnedChart Component
 * 
 * Bar chart displaying tokens earned by day
 * 
 * @param {Object} props
 * @param {Array} props.data - Array of token earning data by day
 */
const TknsEarnedChart = ({ data }) => {
  if (!data || data.length === 0) return <div className="empty-chart">No data available</div>;
  
  // Format dates for display
  const labels = data.map(item => {
    const date = new Date(item.dia);
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
  });
  
  const chartData = {
    labels,
    datasets: [
      {
        label: 'TKNs Earned',
        data: data.map(item => item.totalTKNs),
        backgroundColor: '#ff9800',
        borderRadius: 5,
      }
    ]
  };
  
  return <Bar data={chartData} options={ChartConfigs.barChartOptions} />;
};

export default TknsEarnedChart;