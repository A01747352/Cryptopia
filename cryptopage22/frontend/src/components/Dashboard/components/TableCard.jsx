import React from 'react';

/**
 * TableCard Component
 * 
 * Displays tabular data in a card with a scrollable container
 * 
 * @param {Object} props
 * @param {string} props.title - Card title
 * @param {Array<string>} props.headers - Table column headers
 * @param {Array<Object>} props.data - Array of objects to display in the table
 * @param {string} [props.valueSuffix=''] - Optional suffix for the last column values
 */
const TableCard = ({ title, headers, data, valueSuffix = '' }) => {
  if (!data || data.length === 0) {
    return (
      <div className="card">
        <div className="card-title">{title}</div>
        <div className="empty-state">No data available</div>
      </div>
    );
  }

  return (
    <div className="card">
      <div className="card-title">{title}</div>
      <div className="table-container">
        <table className="data-table">
          <thead>
            <tr>
              {headers.map((header, index) => (
                <th key={index}>{header}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {data.map((row, rowIndex) => {
              const values = Object.values(row);
              return (
                <tr key={rowIndex}>
                  {values.map((value, cellIndex) => (
                    <td key={cellIndex}>
                      {cellIndex === values.length - 1 && valueSuffix
                        ? `${value}${valueSuffix}`
                        : value}
                    </td>
                  ))}
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default TableCard;