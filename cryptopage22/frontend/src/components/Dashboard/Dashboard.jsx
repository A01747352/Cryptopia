import React, { useState, useEffect } from 'react';
import { 
  Chart as ChartJS, 
  CategoryScale, 
  LinearScale, 
  PointElement, 
  LineElement,
  BarElement,
  ArcElement,
  Title, 
  Tooltip, 
  Legend 
} from 'chart.js';
import AnalyticsService from '../../services/analytics-service';

// Componentes existentes
import AgeDistribution from './AgeDistribution';
import GenderDistribution from './GenderDistribution';
import RetentionChart from './RetentionChart';
import SessionsCard from './SessionsCard';
import RevenueCard from './RevenueCard';
import QuestionsCard from './QuestionsCard';

// Nuevos componentes
import AverageAccuracyCard from './AverageAccuracyCard';
import ActiveUsersCard from './ActiveUsersCard';
import TopPowerUpsCard from './TopPowerUpsCard';
import UserRankingCard from './UserRankingCard';
import TradingStatsCard from './TradingStatsCard';
import TknsByDayCard from './TknsByDayCard';
import TopCryptocurrenciesCard from './TopCryptocurrenciesCard';
import MostFailedQuestionsCard from './MostFailedQuestionsCard';

import './Dashboard.css';

// Registrar componentes de Chart.js
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend
);

// Configuración global para Chart.js
ChartJS.defaults.color = '#6c7a89';
ChartJS.defaults.font.family = "'Inter', 'Helvetica', 'Arial', sans-serif";

const Dashboard = () => {
  const [loading, setLoading] = useState(true);
  const [dashboardData, setDashboardData] = useState(null);
  const [triviaStats, setTriviaStats] = useState(null);
  const [retentionData, setRetentionData] = useState(null);
  
  // Estados para los nuevos datos
  const [averageAccuracy, setAverageAccuracy] = useState(null);
  const [activeUsers, setActiveUsers] = useState(null);
  const [topPowerUps, setTopPowerUps] = useState(null);
  const [userRanking, setUserRanking] = useState(null);
  const [tradingStats, setTradingStats] = useState(null);
  const [tknsByDay, setTknsByDay] = useState(null);
  const [topCryptos, setTopCryptos] = useState(null);
  const [failedQuestions, setFailedQuestions] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        // Simular tiempo de carga para prueba
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        // Cargar datos existentes
        const [
          dashboardRes, 
          triviaRes, 
          retentionRes,
          // Nuevos endpoints
          accuracyRes,
          activeUsersRes,
          powerUpsRes,
          rankingRes,
          tradingRes,
          tknsRes,
          cryptosRes,
          failedQuestionsRes
        ] = await Promise.all([
          // Endpoints existentes
          AnalyticsService.getDashboardData(),
          AnalyticsService.getTriviaStats(),
          AnalyticsService.getRetentionData(),
          // Nuevos endpoints
          AnalyticsService.getAverageAccuracy(),
          AnalyticsService.getActiveUsersByDay(),
          AnalyticsService.getTopPowerUps(),
          AnalyticsService.getUserRanking(),
          AnalyticsService.getTradingStats(),
          AnalyticsService.getTknsByDay(),
          AnalyticsService.getTopCryptocurrencies(),
          AnalyticsService.getMostFailedQuestions()
        ]);

        // Establecer datos existentes
        setDashboardData(dashboardRes);
        setTriviaStats(triviaRes);
        setRetentionData(retentionRes);
        
        // Establecer nuevos datos
        setAverageAccuracy(accuracyRes.promedioAciertos);
        setActiveUsers(activeUsersRes);
        setTopPowerUps(powerUpsRes);
        setUserRanking(rankingRes);
        setTradingStats(tradingRes);
        setTknsByDay(tknsRes);
        setTopCryptos(cryptosRes);
        setFailedQuestions(failedQuestionsRes);
        
        setLoading(false);
      } catch (error) {
        console.error('Error fetching dashboard data:', error);
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  return (
    <div className="dashboard light-theme">

<div className="dashboard-row">
<div className="low-card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <SessionsCard sessionsCount={dashboardData?.sesiones} />
          )}
        </div>


<div className="low-card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <SessionsCard sessionsCount={dashboardData?.sesiones} />
          )}
        </div>


        <div className="low-card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <SessionsCard sessionsCount={dashboardData?.sesiones} />
          )}
        </div>


        <div className="low-card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <SessionsCard sessionsCount={dashboardData?.sesiones} />
          )}
        </div>


  </div>


      {/* FILA DE KPIs PRINCIPALES */}
      <div className="dashboard-row">

      <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <SessionsCard sessionsCount={dashboardData?.sesiones} />
          )}
        </div>
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <AverageAccuracyCard data={dashboardData?.genero} />
          )}</div>
        
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <RetentionChart data={retentionData} />
          )}
        </div>
        </div>

      {/* FILA DEMOGRÁFICA */}
      <div className="dashboard-row">
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <AgeDistribution data={dashboardData?.edad} />
          )}
        </div>
        
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <GenderDistribution data={dashboardData?.genero} />
          )}
        </div>
        
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <ActiveUsersCard data={activeUsers} />
          )}
        </div>
      </div>

      {/* FILA DE PREGUNTAS Y POWERUPS */}
      <div className="dashboard-row">
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <QuestionsCard triviaStats={triviaStats} />
          )}
        </div>
        
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <TopPowerUpsCard data={topPowerUps} />
          )}
        </div>
        
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <MostFailedQuestionsCard data={failedQuestions} />
          )}
        </div>
      </div>

      {/* FILA DE ECONOMÍA Y RANKINGS */}
      <div className="dashboard-row">
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <UserRankingCard data={userRanking} />
          )}
        </div>
        
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <TradingStatsCard data={tradingStats} />
          )}
        </div>
      </div>

      {/* FILA DE TKNs Y CRIPTOMONEDAS */}
      <div className="dashboard-row">
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <TknsByDayCard data={tknsByDay} />
          )}
        </div>
        
        <div className="card">
          {loading ? (
            <div className="loading-card">
              <div className="loading-title"></div>
              <div className="loading-content"></div>
            </div>
          ) : (
            <TopCryptocurrenciesCard data={topCryptos} />
          )}
        </div>
      </div>
    </div>
  );
};

export default Dashboard;