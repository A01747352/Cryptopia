import React from 'react';
import { Line } from 'react-chartjs-2';

const RetentionChart = ({ data }) => {
  if (!data) return null;
  
  const labels = data.map(item => item.dia.substring(0, 2));
  const values = data.map(item => item.usuarios);
  
  const chartData = {
    labels,
    datasets: [
      {
        label: 'Retención',
        data: values,
        fill: false,
        borderColor: '#6c63ff',
        tension: 0.4,
        pointBackgroundColor: '#6c63ff'
      }
    ]
  };
  
  const options = {
    responsive: true,
    scales: {
      y: {
        beginAtZero: true,
        grid: {
          color: 'rgba(255, 255, 255, 0.1)'
        },
        ticks: {
          color: '#a0a0a0'
        }
      },
      x: {
        grid: {
          display: false
        },
        ticks: {
          color: '#a0a0a0'
        }
      }
    },
    plugins: {
      legend: {
        display: false
      }
    }
  };
  
  return (
    <div>
      <h2>Retention</h2>
      <div className="retention-percentage">
        <h2>58%</h2>
      </div>
      <div className="chart-container">
        <Line data={chartData} options={options} />
      </div>
    </div>
  );
};

export default RetentionChart;