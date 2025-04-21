import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Line, Bar } from 'react-chartjs-2';
import { 
  Chart as ChartJS, 
  CategoryScale, 
  LinearScale, 
  PointElement, 
  LineElement,
  BarElement,
  Title, 
  Tooltip, 
  Legend 
} from 'chart.js';
import AgeDistribution from './AgeDistribution';
import GenderDistribution from './GenderDistribution';
import RetentionChart from './RetentionChart';
import SessionsCard from './SessionsCard';
import RevenueCard from './RevenueCard';
import QuestionsCard from './QuestionsCard';
import './Dashboard.css';

// Registrar componentes de Chart.js
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend
);

const Dashboard = () => {
  const [loading, setLoading] = useState(true);
  const [dashboardData, setDashboardData] = useState(null);
  const [triviaStats, setTriviaStats] = useState(null);
  const [retentionData, setRetentionData] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [dashboardRes, triviaRes, retentionRes] = await Promise.all([
          axios.get('http://localhost:8081/api/analytics/dashboard'),
          axios.get('http://localhost:8081/api/trivia/estadisticas'),
          axios.get('http://localhost:8081/api/analytics/retencion')
        ]);

        setDashboardData(dashboardRes.data);
        setTriviaStats(triviaRes.data);
        setRetentionData(retentionRes.data);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching dashboard data:', error);
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return <div className="loading">Cargando...</div>;
  }

  return (
    <div className="dashboard">
      <div className="dashboard-grid">
        <AgeDistribution data={dashboardData?.edad} />
        <GenderDistribution data={dashboardData?.genero} />
        <RetentionChart data={retentionData} />
      </div>
      
      <div className="dashboard-grid">
        <SessionsCard sessionsCount={dashboardData?.sesiones} />
        <RevenueCard revenue={dashboardData?.ingresos} />
        <QuestionsCard triviaStats={triviaStats} />
      </div>
    </div>
  );
};

export default Dashboard;