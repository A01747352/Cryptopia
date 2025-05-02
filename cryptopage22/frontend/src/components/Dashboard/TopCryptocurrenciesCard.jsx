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
          '#3b82f6', // Blue (primary)
          '#6366f1', // Indigo (secondary)
          '#10b981', // Emerald (success)
          '#f59e0b', // Amber (warning)
          '#8b5cf6'  // Violet
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
          color: 'var(--text-muted)',
          padding: 20,
          font: {
            size: 12
          }
        }
      },
      tooltip: {
        backgroundColor: 'var(--bg-card)',
        titleColor: 'var(--text-light)',
        bodyColor: 'var(--text-light)',
        borderColor: 'var(--primary)',
        borderWidth: 1,
        callbacks: {
          label: function(context) {
            const label = context.label || '';
            // Fix: Ensure the value is a number before calling toFixed
            let value = context.raw;
            let displayValue;
            
            // Check if value is a number or can be converted to one
            if (typeof value === 'number') {
              displayValue = value.toFixed(2);
            } else {
              // If it's not a number, try to convert it or use a default
              displayValue = parseFloat(value) ? parseFloat(value).toFixed(2) : '0.00';
            }
            
            return `${label}: ${displayValue}`;
          }
        }
      }
    },
    cutout: '65%'
  };
  
  // Fixed calculation to handle non-numeric values with parseFloat
  const totalCoins = data.reduce((acc, curr) => {
    // Make sure we're working with numbers
    const value = typeof curr.total === 'number' ? curr.total : parseFloat(curr.total) || 0;
    return acc + value;
  }, 0).toFixed(2);
  
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