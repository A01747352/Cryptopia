import React from 'react';
import { Line } from 'react-chartjs-2';
import ChartConfigs from '../ChartConfigs';

/**
 * ActiveUsersChart Component
 * 
 * Line chart displaying active users over time
 * 
 * @param {Object} props
 * @param {Array} props.data - Array of active user data objects
 */
const ActiveUsersChart = ({ data }) => {
  if (!data || data.length === 0) return <div className="empty-chart">No data available</div>;
  
  // Format dates for the chart
  const labels = data.map(item => {
    const date = new Date(item.dia);
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
  });
  
  const chartData = {
    labels,
    datasets: [
      {
        label: 'Active Users',
        data: data.map(item => item.usuarios),
        fill: false,
        borderColor: '#4caf50',
        backgroundColor: 'rgba(76, 175, 80, 0.1)',
        tension: 0.4,
        pointBackgroundColor: '#4caf50'
      }
    ]
  };
  
  return <Line data={chartData} options={ChartConfigs.lineChartOptions} />;
};

export default ActiveUsersChart;