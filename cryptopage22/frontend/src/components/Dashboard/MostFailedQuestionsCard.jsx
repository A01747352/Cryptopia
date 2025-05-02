import React from 'react';

const MostFailedQuestionsCard = ({ data }) => {
  if (!data || data.length === 0) return null;
  
  return (
    <div className="card failed-questions-card">
      <h2>Most Missed Questions</h2>
      <div className="failed-questions-list">
        <table className="failed-questions-table">
          <thead>
            <tr>
              <th>Question</th>
              <th>Errors</th>
            </tr>
          </thead>
          <tbody>
            {data.map((question, index) => (
              <tr key={index}>
                <td className="question-text">
                  {question.pregunta.length > 40 
                    ? question.pregunta.substring(0, 40) + '...' 
                    : question.pregunta}
                </td>
                <td className="error-count">{question.errores}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default MostFailedQuestionsCard;