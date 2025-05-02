import React from 'react';

/**
 * AccuracyMetric Component
 * 
 * Displays a circular visualization of the average trivia accuracy
 * 
 * @param {Object} props
 * @param {number} props.value - Accuracy percentage value
 */
const AccuracyMetric = ({ value }) => {
  return (
    <div className="card accuracy-metric">
      <div className="card-title">Trivia Accuracy</div>
      <div className="accuracy-circle">
        <span className="accuracy-value">{value}%</span>
      </div>
      <div className="accuracy-label">
        Average user accuracy on trivia questions
      </div>
    </div>
  );
};

export default AccuracyMetric;