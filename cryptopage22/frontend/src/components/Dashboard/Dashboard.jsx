import React, { useState, useEffect } from 'react';
import { Chart as ChartJS } from 'chart.js';
import AnalyticsService from '../../services/analytics-service';
import ChartConfigs from './ChartConfigs';
import { FaUsers, FaCoins, FaExchangeAlt, FaGamepad } from 'react-icons/fa';
import './Dashboard.css';

// Component imports
import KeyMetricCard from './components/KeyMetricCard';
import ChartCard from './components/ChartCard';
import TableCard from './components/TableCard';
import ActiveUsersChart from './charts/ActiveUsersChart';
import AgeDistributionChart from './charts/AgeDistributionChart';
import GenderDistributionChart from './charts/GenderDistributionChart';
import TknsEarnedChart from './charts/TknsEarnedChart';
import TopPowerUpsChart from './charts/TopPowerUpsChart';
import AccuracyMetric from './components/AccuracyMetric';

// Apply global Chart.js configurations
ChartConfigs.applyGlobalConfig();

const Dashboard = () => {
  const [loading, setLoading] = useState(true);
  const [data, setData] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        // Fetch all required data
        const [
          dashboardRes,
          triviaRes,
          retentionRes,
          accuracyRes,
          activeUsersRes,
          powerUpsRes,
          rankingRes,
          tradingRes,
          tknsRes,
          cryptosRes,
          failedQuestionsRes
        ] = await Promise.all([
          AnalyticsService.getDashboardData(),
          AnalyticsService.getTriviaStats(),
          AnalyticsService.getRetentionData(),
          AnalyticsService.getAverageAccuracy(),
          AnalyticsService.getActiveUsersByDay(),
          AnalyticsService.getTopPowerUps(),
          AnalyticsService.getUserRanking(),
          AnalyticsService.getTradingStats(),
          AnalyticsService.getTknsByDay(),
          AnalyticsService.getTopCryptocurrencies(),
          AnalyticsService.getMostFailedQuestions()
        ]);

        // Organize data in a structured format
        setData({
          metrics: {
            totalUsers: dashboardRes?.genero?.total || 2845,
            activeSessions: dashboardRes?.sesiones || 5840,
            totalTrades: tradingRes?.totalTrades || 1256,
            totalRevenue: dashboardRes?.ingresos || 12495,
          },
          accuracy: accuracyRes?.promedioAciertos || 68.5,
          genderDistribution: dashboardRes?.genero || {
            masculino: 65,
            femenino: 28,
            otro: 7,
            total: 100
          },
          ageDistribution: dashboardRes?.edad || {
            grupo_0_17: 5,
            grupo_18_24: 32,
            grupo_25_34: 45,
            grupo_35_44: 12,
            grupo_45_plus: 6
          },
          dailyActiveUsers: activeUsersRes || [],
          topCryptocurrencies: cryptosRes || [],
          topPowerUps: powerUpsRes || [],
          userRanking: rankingRes || [],
          tknsEarned: tknsRes || [],
          failedQuestions: failedQuestionsRes || [],
          retentionData: retentionRes || []
        });
        
        setLoading(false);
      } catch (error) {
        console.error('Error fetching dashboard data:', error);
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return (
      <div className="dashboard-container">
        <div className="dashboard-loading">
          {/* KPI Skeleton Loading */}
          <div className="dashboard-row kpi-row">
            {[...Array(4)].map((_, i) => (
              <div key={i} className="loading-card">
                <div className="loading-title"></div>
                <div className="loading-content"></div>
              </div>
            ))}
          </div>
          
          {/* Main Grid Skeleton Loading */}
          <div className="dashboard-row main-row">
            {[...Array(9)].map((_, i) => (
              <div key={i} className="loading-card">
                <div className="loading-title"></div>
                <div className="loading-content"></div>
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      {/* KPI Summary row */}
      <div className="dashboard-row kpi-row">
        <KeyMetricCard 
          icon={<FaUsers />} 
          title="Total Users" 
          value={data.metrics.totalUsers.toLocaleString()} 
          color="primary"
        />
        <KeyMetricCard 
          icon={<FaGamepad />} 
          title="Active Sessions" 
          value={data.metrics.activeSessions.toLocaleString()} 
          color="secondary"
        />
        <KeyMetricCard 
          icon={<FaExchangeAlt />} 
          title="Total Trades" 
          value={data.metrics.totalTrades.toLocaleString()} 
          color="success"
        />
        <KeyMetricCard 
          icon={<FaCoins />} 
          title="Total Revenue" 
          value={`$${data.metrics.totalRevenue.toLocaleString()}`} 
          color="warning"
        />
      </div>

      {/* Main dashboard grid */}
      <div className="dashboard-row main-row">
        <ChartCard 
          title="Active Users" 
          subtitle={`${Math.round(data.dailyActiveUsers.reduce((acc, item) => acc + item.usuarios, 0) / Math.max(1, data.dailyActiveUsers.length))} avg. daily users`}
        >
          <ActiveUsersChart data={data.dailyActiveUsers} />
        </ChartCard>
        
        <ChartCard title="Age Distribution">
          <AgeDistributionChart data={data.ageDistribution} />
        </ChartCard>
        
        <ChartCard title="Gender Distribution">
          <GenderDistributionChart data={data.genderDistribution} />
        </ChartCard>
        
        <ChartCard 
          title="TKNs Earned" 
          subtitle={`${data.tknsEarned.reduce((acc, item) => acc + item.totalTKNs, 0).toLocaleString()} total TKNs`}
        >
          <TknsEarnedChart data={data.tknsEarned} />
        </ChartCard>
        
        <ChartCard title="Most Used PowerUps">
          <TopPowerUpsChart data={data.topPowerUps} />
        </ChartCard>
        
        <TableCard 
          title="Top Cryptocurrencies" 
          headers={['Name', 'Total']} 
          data={data.topCryptocurrencies.map(item => ({
            name: item.nombre,
            total: parseFloat(item.total).toFixed(2)
          }))}
        />
        
        <TableCard 
          title="User Ranking" 
          headers={['Username', 'Score']} 
          data={data.userRanking.map(item => ({
            username: item.username,
            score: item.totalPuntaje ? item.totalPuntaje.toLocaleString() : '0'
          }))}
        />
        
        <TableCard 
          title="Most Failed Questions" 
          headers={['Question', 'Errors']} 
          data={data.failedQuestions.map(item => ({
            question: item.pregunta?.length > 30 ? item.pregunta.substring(0, 30) + '...' : item.pregunta,
            errors: item.errores
          }))}
        />

        <AccuracyMetric value={data.accuracy} />
      </div>
    </div>
  );
};

export default Dashboard;