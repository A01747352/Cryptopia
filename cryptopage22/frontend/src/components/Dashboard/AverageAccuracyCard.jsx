import React from 'react';
import LayeredPieChart from "../Graphs/LayeredPieChart";

const AverageAccuracyCard = ({ data }) => {
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
  return (
    <div >
      <h2>Average Accuracy</h2>
      <div className="chart-centered mt-4">
      <div className="chart-container flex items-center justify-center">
        <LayeredPieChart x={malePercentage} y={femalePercentage} z={80} />
      </div>
      <div>
        <ul>
          <p>Male {malePercentage} %</p>
          <p>Femal  {femalePercentage} %</p>
          <p>Other  {otherPercentage} %</p>
        </ul>
      </div>
        
      </div>
    </div>
  );
};

export default AverageAccuracyCard;
