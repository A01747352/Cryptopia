import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Line, Bar, Doughnut } from 'react-chartjs-2';
import './Analytics.css';

const Analytics = () => {
  const [analyticsData, setAnalyticsData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        // Obtener datos de diferentes endpoints para análisis completo
        const [dashboardRes, triviaRes, retentionRes] = await Promise.all([
          axios.get('http://localhost:8081/api/analytics/dashboard'),
          axios.get('http://localhost:8081/api/trivia/estadisticas'),
          axios.get('http://localhost:8081/api/analytics/retencion')
        ]);

        setAnalyticsData({
          dashboard: dashboardRes.data,
          trivia: triviaRes.data,
          retention: retentionRes.data
        });
        setLoading(false);
      } catch (error) {
        console.error('Error fetching analytics data:', error);
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return <div className="loading">Cargando datos de análisis...</div>;
  }

  // Configurar datos para gráficas aquí...

  return (
    <div className="analytics-container">
      <h1>Analytics</h1>
      
      <div className="analytics-grid">
        <div className="card analytics-card">
          <h2>User Growth</h2>
          <div className="chart-container">
            {/* Aquí iría una gráfica de crecimiento de usuarios */}
          </div>
        </div>
        
        <div className="card analytics-card">
          <h2>Engagement by Country</h2>
          <div className="chart-container">
            {/* Aquí iría una gráfica de compromiso por país */}
          </div>
        </div>
        
        <div className="card analytics-card">
          <h2>Trivia Performance</h2>
          <div className="chart-container">
            {/* Aquí iría una gráfica de rendimiento en trivias */}
          </div>
        </div>
        
        <div className="card analytics-card">
          <h2>Revenue Trends</h2>
          <div className="chart-container">
            {/* Aquí iría una gráfica de tendencias de ingresos */}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Analytics;