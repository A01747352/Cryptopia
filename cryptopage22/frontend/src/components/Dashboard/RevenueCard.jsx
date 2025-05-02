import React from 'react';

const RevenueCard = ({ revenue }) => {
  // Formatear el valor como moneda
  const formattedRevenue = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(revenue || 12495);

  return (
    <div className="card revenue-card">
      <h2>Revenue</h2>
      <div className="revenue-amount">
        <h2>{formattedRevenue}</h2>
      </div>
    </div>
  );
};

export default RevenueCard;