import React from 'react';
import { Bar } from 'react-chartjs-2';

const AgeDistribution = ({ data }) => {
  if (!data) return null;
  
  const chartData = {
    labels: ['18-17', '18-24', '25-34', '35-44', '45+'],
    datasets: [
      {
        label: 'Usuarios por edad',
        data: [
          data.grupo_18_17 || 0,
          data.grupo_18_24 || 0,
          data.grupo_25_34 || 0,
          data.grupo_35_44 || 0,
          data.grupo_45_plus || 0
        ],
        backgroundColor: '#6c63ff',
        borderRadius: 5,
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
    <div className="card">
      <h2>Age</h2>
      <div className="chart-container">
        <Bar data={chartData} options={options} />
      </div>
    </div>
  );
};

export default AgeDistribution;