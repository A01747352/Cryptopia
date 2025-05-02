import React from 'react';

const QuestionsCard = ({ triviaStats }) => {
  if (!triviaStats) return null;

  // Extraer las preguntas más y menos acertadas
  const topQuestions = triviaStats.estadisticasPorcentajes.slice(0, 4);
  const bottomQuestions = [...triviaStats.estadisticasPorcentajes]
    .sort((a, b) => a.porcentaje - b.porcentaje)
    .slice(0, 4);

  return (
    <div className="card questions-card">
      <h2>Questions</h2>
      
      <div className="questions-container">
        <div className="top-questions">
          <h3>Top Correct</h3>
          <ul>
            {topQuestions.map(q => (
              <li key={q.idPregunta}>
                <span>{q.pregunta.length > 30 ? q.pregunta.substring(0, 30) + '...' : q.pregunta}</span>
                <span className="percentage">{q.porcentaje}%</span>
              </li>
            ))}
          </ul>
        </div>
        
        <div className="bottom-questions">
          <h3>Bottom Correct</h3>
          <ul>
            {bottomQuestions.map(q => (
              <li key={q.idPregunta}>
                <span>{q.pregunta.length > 30 ? q.pregunta.substring(0, 30) + '...' : q.pregunta}</span>
                <span className="percentage">{q.porcentaje}%</span>
              </li>
            ))}
          </ul>
        </div>
      </div>
    </div>
  );
};

export default QuestionsCard;