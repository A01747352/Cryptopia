import React from 'react';
import { Doughnut } from 'react-chartjs-2';
import ChartConfigs from '../ChartConfigs';

/**
 * AgeDistributionChart Component
 * 
 * Doughnut chart displaying the age distribution of users
 * 
 * @param {Object} props
 * @param {Object} props.data - Object containing age distribution data
 */
const AgeDistributionChart = ({ data }) => {
  if (!data) return <div className="empty-chart">No data available</div>;
  
  // Format data for the chart
  const chartData = {
    labels: ['0-17', '18-24', '25-34', '35-44', '45+'],
    datasets: [
      {
        data: [
          data.grupo_0_17 || 0,
          data.grupo_18_24 || 0,
          data.grupo_25_34 || 0,
          data.grupo_35_44 || 0,
          data.grupo_45_plus || 0
        ],
        backgroundColor: ChartConfigs.modernPalette.slice(0, 5),
        borderWidth: 0,
      }
    ]
  };
  
  return <Doughnut data={chartData} options={ChartConfigs.doughnutOptions} />;
};

export default AgeDistributionChart;