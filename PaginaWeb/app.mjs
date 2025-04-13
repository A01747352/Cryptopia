import express from 'express';
import db from './database.mjs';
import * as dotenv from 'dotenv';

// Cargar variables de entorno
dotenv.config();

const app = express();
const port = process.env.PORT ?? 8080;
const ipAddress = process.env.C9_HOSTNAME ?? 'localhost';

// Configurar motor de plantillas EJS
app.set('view engine', 'ejs');
app.use(express.static('public'));
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Middleware para datos globales
app.use((req, res, next) => {
  res.locals.currentPath = req.path;
  next();
});

// Ruta principal - Dashboard
app.get('/', async (req, res) => {
  let connection;
  try {
    connection = await db.connect();
    
    // Obtener datos para el dashboard (en producción, estos vendrían de la BD)
    const userStats = await db.getUserStats(connection);
    const gameProgress = await db.getGameProgress(connection);
    const countryStats = await db.getCountryStats(connection);
    const ageDistribution = await db.getAgeDistribution(connection);
    const recentActivities = await db.getRecentActivities(connection);
    const achievementData = await db.getAchievementData(connection);
    
    res.render('dashboard', {
      userStats,
      gameProgress,
      countryStats,
      ageDistribution,
      recentActivities,
      achievementData,
      page: {
        title: 'Dashboard',
        description: 'Monitoreo de usuario y progreso del juego'
      }
    });
    
  } catch (err) {
    console.error('Error al obtener datos del dashboard:', err);
    res.status(500).render('error', { 
      message: 'Error al cargar el dashboard',
      error: err,
      page: {
        title: 'Error',
        description: 'Algo salió mal'
      }
    });
    
  } finally {
    if (connection) {
      await connection.end();
    }
  }
});

// Ruta para usuarios y progreso
app.get('/users', async (req, res) => {
  let connection;
  try {
    connection = await db.connect();
    
    const users = await db.getAllUsers(connection);
    const userStats = await db.getUserStats(connection);
    const ageDistribution = await db.getAgeDistribution(connection);
    
    res.render('users', {
      users,
      userStats,
      ageDistribution,
      page: {
        title: 'Usuarios',
        description: 'Información detallada de usuarios'
      }
    });
    
  } catch (err) {
    console.error('Error al obtener datos de usuarios:', err);
    res.status(500).render('error', { 
      message: 'Error al cargar la página de usuarios',
      error: err,
      page: {
        title: 'Error',
        description: 'Algo salió mal'
      }
    });
    
  } finally {
    if (connection) {
      await connection.end();
    }
  }
});

// Ruta para análisis de curso
app.get('/courses', async (req, res) => {
  let connection;
  try {
    connection = await db.connect();
    
    const courses = await db.getAllCourses(connection);
    const courseStats = await db.getCourseStats(connection);
    
    res.render('courses', {
      courses,
      courseStats,
      page: {
        title: 'Cursos',
        description: 'Análisis de rendimiento de cursos'
      }
    });
    
  } catch (err) {
    console.error('Error al obtener datos de cursos:', err);
    res.status(500).render('error', { 
      message: 'Error al cargar la página de cursos',
      error: err,
      page: {
        title: 'Error',
        description: 'Algo salió mal'
      }
    });
    
  } finally {
    if (connection) {
      await connection.end();
    }
  }
});

// Ruta para mapa global
app.get('/map', async (req, res) => {
  let connection;
  try {
    connection = await db.connect();
    
    const countryStats = await db.getCountryStats(connection);
    
    res.render('map', {
      countryStats,
      page: {
        title: 'Mapa Global',
        description: 'Distribución global de usuarios'
      }
    });
    
  } catch (err) {
    console.error('Error al obtener datos del mapa:', err);
    res.status(500).render('error', { 
      message: 'Error al cargar el mapa global',
      error: err,
      page: {
        title: 'Error',
        description: 'Algo salió mal'
      }
    });
    
  } finally {
    if (connection) {
      await connection.end();
    }
  }
});

// API para obtener datos de usuarios
app.get('/api/users', async (req, res) => {
  let connection;
  try {
    connection = await db.connect();
    const users = await db.getAllUsers(connection);
    res.json(users);
  } catch (err) {
    res.status(500).json({ error: err.message });
  } finally {
    if (connection) {
      await connection.end();
    }
  }
});

// API para obtener estadísticas por país
app.get('/api/countries', async (req, res) => {
  let connection;
  try {
    connection = await db.connect();
    const countries = await db.getCountryStats(connection);
    res.json(countries);
  } catch (err) {
    res.status(500).json({ error: err.message });
  } finally {
    if (connection) {
      await connection.end();
    }
  }
});

// API para obtener distribución por edades
app.get('/api/ages', async (req, res) => {
  let connection;
  try {
    connection = await db.connect();
    const ages = await db.getAgeDistribution(connection);
    res.json(ages);
  } catch (err) {
    res.status(500).json({ error: err.message });
  } finally {
    if (connection) {
      await connection.end();
    }
  }
});

// API para obtener logros y estadísticas
app.get('/api/achievements', async (req, res) => {
  let connection;
  try {
    connection = await db.connect();
    const achievements = await db.getAchievementData(connection);
    res.json(achievements);
  } catch (err) {
    res.status(500).json({ error: err.message });
  } finally {
    if (connection) {
      await connection.end();
    }
  }
});

// Página de recurso no encontrado (estatus 404)
app.use((req, res) => {
  const url = req.originalUrl;
  res.status(404).render('not_found', { 
    url,
    page: {
      title: 'No Encontrado',
      description: '404 - Recurso no encontrado'
    }
  });
});

// Iniciar el servidor
app.listen(port, () => {
  console.log(`Servidor del dashboard ejecutándose en: http://${ipAddress}:${port}`);
});