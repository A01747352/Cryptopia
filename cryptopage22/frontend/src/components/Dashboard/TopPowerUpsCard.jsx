import React from 'react';
import { Bar } from 'react-chartjs-2';

const TopPowerUpsCard = ({ data }) => {
  if (!data || data.length === 0) return null;
  
  const chartData = {
    labels: data.map(item => item.nombre),
    datasets: [
      {
        label: 'Veces usado',
        data: data.map(item => item.veces_usado),
        backgroundColor: [
          '#6c63ff',
          '#ff6384',
          '#36a2eb',
          '#ffce56',
          '#4caf50'
        ],
        borderRadius: 5,
      }
    ]
  };
  
  const options = {
    indexAxis: 'y',
    responsive: true,
    scales: {
      x: {
        beginAtZero: true,
        grid: {
          color: 'rgba(255, 255, 255, 0.1)'
        },
        ticks: {
          color: '#a0a0a0'
        }
      },
      y: {
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
      },
      tooltip: {
        backgroundColor: '#333',
        titleColor: '#fff',
        bodyColor: '#fff',
        borderColor: '#6c63ff',
        borderWidth: 1
      }
    }
  };
  
  return (
    <div className="card powerups-card">
      <h2>Most Used PowerUps</h2>
      <div className="chart-container">
        <Bar data={chartData} options={options} />
      </div>
    </div>
  );
};

export default TopPowerUpsCard;