import axios from 'axios';

const BASE_URL = 'http://localhost:8081/api';

const AnalyticsService = {
  // Endpoints existentes
  getDashboardData: async () => {
    const response = await axios.get(`${BASE_URL}/analytics/dashboard`);
    return response.data;
  },

  getTriviaStats: async () => {
    const response = await axios.get(`${BASE_URL}/trivia/estadisticas`);
    return response.data;
  },

  getRetentionData: async () => {
    const response = await axios.get(`${BASE_URL}/analytics/retencion`);
    return response.data;
  },

  // Nuevos endpoints para las nuevas tarjetas
  getAverageAccuracy: async () => {
    // Este endpoint debería implementarse en el backend
    try {
      const response = await axios.get(`${BASE_URL}/analytics/average-accuracy`);
      return response.data;
    } catch (error) {
      console.error('Error fetching average accuracy:', error);
      // Datos ficticios para pruebas si el endpoint no existe
      return { promedioAciertos: 68.5 };
    }
  },

  getActiveUsersByDay: async () => {
    // Este endpoint debería implementarse en el backend
    try {
      const response = await axios.get(`${BASE_URL}/analytics/active-users`);
      return response.data;
    } catch (error) {
      console.error('Error fetching active users:', error);
      // Datos ficticios para pruebas
      return [
        { dia: '2023-04-15', usuarios: 45 },
        { dia: '2023-04-16', usuarios: 52 },
        { dia: '2023-04-17', usuarios: 48 },
        { dia: '2023-04-18', usuarios: 60 },
        { dia: '2023-04-19', usuarios: 55 },
        { dia: '2023-04-20', usuarios: 70 },
        { dia: '2023-04-21', usuarios: 65 }
      ];
    }
  },

  getTopPowerUps: async () => {
    // Este endpoint debería implementarse en el backend
    try {
      const response = await axios.get(`${BASE_URL}/analytics/top-powerups`);
      return response.data;
    } catch (error) {
      console.error('Error fetching top power-ups:', error);
      // Datos ficticios para pruebas
      return [
        { nombre: 'Double Points', veces_usado: 320 },
        { nombre: 'Time Boost', veces_usado: 245 },
        { nombre: 'Skip Question', veces_usado: 198 },
        { nombre: 'Extra Life', veces_usado: 156 },
        { nombre: 'Coin Multiplier', veces_usado: 134 }
      ];
    }
  },

  getUserRanking: async () => {
    // Este endpoint debería implementarse en el backend
    try {
      const response = await axios.get(`${BASE_URL}/analytics/user-ranking`);
      return response.data;
    } catch (error) {
      console.error('Error fetching user ranking:', error);
      // Datos ficticios para pruebas
      return [
        { username: 'crypto_master', totalPuntaje: 12500 },
        { username: 'blockchain_guru', totalPuntaje: 10800 },
        { username: 'hodl_king', totalPuntaje: 9750 },
        { username: 'satoshi_fan', totalPuntaje: 8600 },
        { username: 'nft_collector', totalPuntaje: 7900 }
      ];
    }
  },

  getTradingStats: async () => {
    // Este endpoint debería implementarse en el backend
    try {
      const response = await axios.get(`${BASE_URL}/analytics/trading-stats`);
      return response.data;
    } catch (error) {
      console.error('Error fetching trading stats:', error);
      // Datos ficticios para pruebas
      return { totalTrades: 1256, totalVolumen: 45678.90 };
    }
  },

  getTknsByDay: async () => {
    // Este endpoint debería implementarse en el backend
    try {
      const response = await axios.get(`${BASE_URL}/analytics/tkns-by-day`);
      return response.data;
    } catch (error) {
      console.error('Error fetching TKNs by day:', error);
      // Datos ficticios para pruebas
      return [
        { dia: '2023-04-15', totalTKNs: 1250 },
        { dia: '2023-04-16', totalTKNs: 1450 },
        { dia: '2023-04-17', totalTKNs: 1320 },
        { dia: '2023-04-18', totalTKNs: 1580 },
        { dia: '2023-04-19', totalTKNs: 1680 },
        { dia: '2023-04-20', totalTKNs: 1850 },
        { dia: '2023-04-21', totalTKNs: 1720 }
      ];
    }
  },

  getTopCryptocurrencies: async () => {
    // Este endpoint debería implementarse en el backend
    try {
      const response = await axios.get(`${BASE_URL}/analytics/top-cryptocurrencies`);
      return response.data;
    } catch (error) {
      console.error('Error fetching top cryptocurrencies:', error);
      // Datos ficticios para pruebas
      return [
        { nombre: 'Bitcoin', total: 125.35 },
        { nombre: 'Ethereum', total: 458.72 },
        { nombre: 'Cardano', total: 3450.21 },
        { nombre: 'Solana', total: 892.45 },
        { nombre: 'Polkadot', total: 765.33 }
      ];
    }
  },

  getMostFailedQuestions: async () => {
    // Este endpoint debería implementarse en el backend
    try {
      const response = await axios.get(`${BASE_URL}/analytics/most-failed-questions`);
      return response.data;
    } catch (error) {
      console.error('Error fetching most failed questions:', error);
      // Datos ficticios para pruebas
      return [
        { pregunta: '¿Qué es un hash en blockchain?', errores: 125 },
        { pregunta: '¿Qué es un smart contract?', errores: 98 },
        { pregunta: '¿Qué significa PoW?', errores: 87 },
        { pregunta: '¿Cuál fue la primera criptomoneda?', errores: 65 },
        { pregunta: '¿Qué es un token NFT?', errores: 54 }
      ];
    }
  }
};

export default AnalyticsService;