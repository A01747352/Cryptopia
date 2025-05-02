import React from 'react';
import { Doughnut } from 'react-chartjs-2';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';

ChartJS.register(ArcElement, Tooltip, Legend);

const GenderDistribution = ({ data }) => {
  if (!data) return null;
  
  const totalUsers = data.total || 0;
  const malePercentage = totalUsers > 0 ? Math.round((data.masculino / totalUsers) * 100) : 0;
  const femalePercentage = totalUsers > 0 ? Math.round((data.femenino / totalUsers) * 100) : 0;
  const otherPercentage = totalUsers > 0 ? Math.round((data.otro / totalUsers) * 100) : 0;
  
  const chartData = {
    labels: ['Male', 'Female', 'Other'],
    datasets: [
      {
        data: [malePercentage, femalePercentage, otherPercentage],
        backgroundColor: ['#6c63ff', '#ff6384', '#36a2eb'],
        borderWidth: 0,
      }
    ]
  };
  
  const options = {
    responsive: true,
    plugins: {
      legend: {
        position: 'right',
        labels: {
          color: '#a0a0a0',
          padding: 20,
          font: {
            size: 14
          }
        }
      }
    },
    cutout: '70%'
  };
  
  return (
    <div className="card">
      <h2>Gender</h2>
      <div className="gender-container">
        <div className="gender-chart">
          <Doughnut data={chartData} options={options} />
        </div>
        <div className="gender-percentage">
          <h2>{malePercentage}%</h2>
          <p>Male</p>
        </div>
      </div>
    </div>
  );
};

export default GenderDistribution;