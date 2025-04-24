import React from 'react';
import { CircularProgressbar, buildStyles } from 'react-circular-progressbar';
import 'react-circular-progressbar/dist/styles.css';

const AverageAccuracyCard = ({ accuracy }) => {
  const progressValue = parseFloat(accuracy) || 0;
  
  return (
    <div className="card average-accuracy-card">
      <h2>Average Accuracy</h2>
      <div className="progress-container">
        <div style={{ width: '150px', height: '150px', margin: '0 auto' }}>
        <CircularProgressbar
  value={progressValue}
  text={`${progressValue}%`}
  styles={buildStyles({
    textSize: '16px',
    pathColor: `#6c63ff`,
    textColor: '#ffffff',
    trailColor: 'rgba(51, 51, 51, 0.7)',
    pathTransition: 'stroke-dashoffset 0.5s ease-out'
  })}
/>
        </div>
        <div className="accuracy-metrics">
          <p>User performance in trivia questions</p>
        </div>
      </div>
    </div>
  );
};

export default AverageAccuracyCard;