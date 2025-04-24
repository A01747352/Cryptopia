import React from 'react';
import { Doughnut } from 'react-chartjs-2';

const TopCryptocurrenciesCard = ({ data }) => {
  if (!data || data.length === 0) return null;
  
  const chartData = {
    labels: data.map(item => item.nombre),
    datasets: [
      {
        data: data.map(item => item.total),
        backgroundColor: [
          '#ff6384',
          '#36a2eb',
          '#ffce56',
          '#4caf50',
          '#6c63ff'
        ],
        borderWidth: 0,
      }
    ]
  };
  
  const options = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'right',
        labels: {
          color: '#a0a0a0',
          padding: 20,
          font: {
            size: 12
          }
        }
      },
      tooltip: {
        backgroundColor: '#333',
        titleColor: '#fff',
        bodyColor: '#fff',
        borderColor: '#6c63ff',
        borderWidth: 1,
        callbacks: {
          label: function(context) {
            const label = context.label || '';
            const value = context.raw || 0;
            return `${label}: ${value.toFixed(2)}`;
          }
        }
      }
    },
    cutout: '65%'
  };
  
  // Calcular el total de criptomonedas
  const totalCoins = data.reduce((acc, curr) => acc + curr.total, 0).toFixed(2);
  
  return (
    <div className="card cryptocurrencies-card">
      <h2>Top Cryptocurrencies</h2>
      <div className="crypto-container">
        <div className="crypto-chart">
          <Doughnut data={chartData} options={options} />
        </div>
        <div className="crypto-total">
          <h3>{totalCoins}</h3>
          <p>Total Coins</p>
        </div>
      </div>
    </div>
  );
};

export default TopCryptocurrenciesCard;