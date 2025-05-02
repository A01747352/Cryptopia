import React from 'react';
import { Bar } from 'react-chartjs-2';
import ChartConfigs from '../ChartConfigs';

/**
 * TopPowerUpsChart Component
 * 
 * Horizontal bar chart displaying the most used power-ups
 * 
 * @param {Object} props
 * @param {Array} props.data - Array of power-up usage data
 */
const TopPowerUpsChart = ({ data }) => {
  if (!data || data.length === 0) return <div className="empty-chart">No data available</div>;
  
  const chartData = {
    labels: data.map(item => item.nombre),
    datasets: [
      {
        label: 'Usage Count',
        data: data.map(item => item.veces_usado),
        backgroundColor: '#3f51b5',
        borderRadius: 5,
      }
    ]
  };
  
  // Customize options for horizontal bar chart
  const options = {
    ...ChartConfigs.barChartOptions,
    indexAxis: 'y',
    scales: {
      ...ChartConfigs.barChartOptions.scales,
      x: {
        ...ChartConfigs.barChartOptions.scales.y,
        grid: {
          color: 'rgba(255, 255, 255, 0.05)'
        }
      },
      y: {
        ...ChartConfigs.barChartOptions.scales.x,
        grid: {
          display: false
        }
      }
    }
  };
  
  return <Bar data={chartData} options={options} />;
};

export default TopPowerUpsChart;