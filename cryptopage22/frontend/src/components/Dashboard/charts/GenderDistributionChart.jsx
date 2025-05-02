import React from 'react';
import { Doughnut } from 'react-chartjs-2';
import ChartConfigs from '../ChartConfigs';

/**
 * GenderDistributionChart Component
 * 
 * Doughnut chart displaying the gender distribution of users
 * 
 * @param {Object} props
 * @param {Object} props.data - Object containing gender distribution data
 */
const GenderDistributionChart = ({ data }) => {
  if (!data) return <div className="empty-chart">No data available</div>;
  
  const totalUsers = data.total || 0;
  const malePercentage = totalUsers > 0 ? Math.round((data.masculino / totalUsers) * 100) : 0;
  const femalePercentage = totalUsers > 0 ? Math.round((data.femenino / totalUsers) * 100) : 0;
  const otherPercentage = totalUsers > 0 ? Math.round((data.otro / totalUsers) * 100) : 0;
  
  const chartData = {
    labels: ['Male', 'Female', 'Other'],
    datasets: [
      {
        data: [malePercentage, femalePercentage, otherPercentage],
        backgroundColor: ['#3f51b5', '#e91e63', '#4caf50'],
        borderWidth: 0,
      }
    ]
  };
  
  // Customize doughnut options
  const options = {
    ...ChartConfigs.doughnutOptions,
    plugins: {
      ...ChartConfigs.doughnutOptions.plugins,
      tooltip: {
        ...ChartConfigs.doughnutOptions.plugins.tooltip,
        callbacks: {
          label: function(context) {
            return `${context.label}: ${context.raw}%`;
          }
        }
      }
    }
  };
  
  return <Doughnut data={chartData} options={options} />;
};

export default GenderDistributionChart;