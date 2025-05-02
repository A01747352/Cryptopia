import React from 'react';
import { Line } from 'react-chartjs-2';

const ActiveUsersCard = ({ data }) => {
  if (!data || data.length === 0) return null;
  
  // Formatear las fechas para mostrar
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
        tension: 0.4,
        pointBackgroundColor: '#4caf50'
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
  
  // Calcular el promedio de usuarios activos
  const averageUsers = Math.round(
    data.reduce((acc, curr) => acc + curr.usuarios, 0) / data.length
  );
  
  return (
    <div className="card active-users-card">
      <h2>Active Users Per Day</h2>
      <div className="users-average">
        <h3>{averageUsers}</h3>
        <p>Avg. users</p>
      </div>
      <div className="chart-container">
        <Line data={chartData} options={options} />
      </div>
    </div>
  );
};

export default ActiveUsersCard;