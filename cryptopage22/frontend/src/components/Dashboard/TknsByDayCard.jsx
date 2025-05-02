import React from 'react';
import { Bar } from 'react-chartjs-2';

const TknsByDayCard = ({ data }) => {
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
        label: 'TKNs Earned',
        data: data.map(item => item.totalTKNs),
        backgroundColor: '#36a2eb',
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
      },
      tooltip: {
        backgroundColor: '#333',
        titleColor: '#fff',
        bodyColor: '#fff',
        borderColor: '#36a2eb',
        borderWidth: 1
      }
    }
  };
  
  // Calcular el total de TKNs ganados
  const totalTkns = data.reduce((acc, curr) => acc + curr.totalTKNs, 0);
  
  return (
    <div className="card tkns-card">
      <h2>TKNs Earned Per Day</h2>
      <div className="tkns-total">
        <h3>{totalTkns.toLocaleString()}</h3>
        <p>Total TKNs</p>
      </div>
      <div className="chart-container">
        <Bar data={chartData} options={options} />
      </div>
    </div>
  );
};

export default TknsByDayCard;