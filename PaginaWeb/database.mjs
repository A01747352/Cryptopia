import mysql from 'mysql2/promise';

// Función para conectar a la base de datos
async function connect() {
  return await mysql.createConnection({
    host: process.env.MYSQL_HOST,
    user: process.env.MYSQL_USER,
    password: process.env.MYSQL_PASSWORD,
    database: process.env.MYSQL_DATABASE
  });
}

// Estadísticas generales de usuarios
async function getUserStats(connection) {
  // En producción: const [rows] = await connection.execute('SELECT * FROM user_stats');
  return {
    totalUsers: 12543,
    activeUsers: 8721,
    newUsersThisMonth: 1245,
    averageSessionTime: 22.5, // en minutos
    completionRate: 68.3, // porcentaje
    userRetention: 87.2, // porcentaje
    averageScore: 78.4, // porcentaje
    userGrowth: 15.3 // porcentaje
  };
}

// Progreso del juego
async function getGameProgress(connection) {
  // En producción: const [rows] = await connection.execute('SELECT * FROM game_progress');
  return {
    completionRate: 68.3, // porcentaje
    averageLessonsPerUser: 32,
    topCompletedCourse: "Matemáticas Básicas",
    bestPerformingSkill: "Aritmética",
    totalExercisesCompleted: 827543,
    averageDailyActivity: 24.5, // minutos
    streakAverage: 8.3, // días
    challengesCompleted: 45928
  };
}

// Estadísticas por país
async function getCountryStats(connection) {
  // En producción: const [rows] = await connection.execute('SELECT country, count(*) as users FROM users GROUP BY country');
  return [
    { country: "México", users: 4235, completion: 72.1, active: 3542, growth: 14.2 },
    { country: "Colombia", users: 2567, completion: 68.5, active: 1896, growth: 18.7 },
    { country: "España", users: 1983, completion: 76.3, active: 1654, growth: 12.3 },
    { country: "Argentina", users: 1635, completion: 70.2, active: 1215, growth: 16.8 },
    { country: "Perú", users: 872, completion: 65.9, active: 701, growth: 21.5 },
    { country: "Chile", users: 658, completion: 71.8, active: 542, growth: 15.2 },
    { country: "Brasil", users: 843, completion: 69.5, active: 623, growth: 24.1 },
    { country: "Estados Unidos", users: 593, completion: 58.4, active: 401, growth: 8.9 }
  ];
}

// Distribución por edades
async function getAgeDistribution(connection) {
  // En producción: const [rows] = await connection.execute('SELECT age_range, count(*) as count FROM users GROUP BY age_range');
  return [
    { range: "5-10", count: 2567, percentage: 20.5, completion: 65.7, retention: 80.2 },
    { range: "11-15", count: 5231, percentage: 41.7, completion: 72.3, retention: 85.6 },
    { range: "16-20", count: 3142, percentage: 25.1, completion: 68.9, retention: 82.4 },
    { range: "21-30", count: 984, percentage: 7.8, completion: 61.2, retention: 75.8 },
    { range: "31+", count: 619, percentage: 4.9, completion: 59.8, retention: 72.3 }
  ];
}

// Listado de usuarios
async function getAllUsers(connection) {
  // En producción: const [rows] = await connection.execute('SELECT * FROM users LIMIT 50');
  return [
    { id: 1, username: "estudiante123", age: 14, country: "México", progress: 78, streak: 12, score: 8450, lastLogin: "2025-04-02" },
    { id: 2, username: "aprendo_feliz", age: 9, country: "Colombia", progress: 45, streak: 5, score: 3240, lastLogin: "2025-04-05" },
    { id: 3, username: "matematicas_pro", age: 16, country: "España", progress: 92, streak: 28, score: 12580, lastLogin: "2025-04-07" },
    { id: 4, username: "ciencia_divertida", age: 12, country: "Argentina", progress: 67, streak: 8, score: 5420, lastLogin: "2025-04-01" },
    { id: 5, username: "futuro_ingeniero", age: 15, country: "México", progress: 83, streak: 15, score: 9870, lastLogin: "2025-04-06" },
    { id: 6, username: "curiosity_kid", age: 10, country: "Chile", progress: 62, streak: 7, score: 4320, lastLogin: "2025-04-03" },
    { id: 7, username: "mente_brillante", age: 13, country: "Perú", progress: 71, streak: 9, score: 6840, lastLogin: "2025-04-04" },
    { id: 8, username: "explorador_espacial", age: 11, country: "Colombia", progress: 58, streak: 4, score: 3890, lastLogin: "2025-04-05" }
  ];
}

// Obtener datos de actividades recientes
async function getRecentActivities(connection) {
  // En producción: const [rows] = await connection.execute('SELECT * FROM user_activities ORDER BY created_at DESC LIMIT 10');
  return [
    { id: 1, user: "matematicas_pro", activity: "Completó curso de Geometría", time: "Hace 2 horas", points: 120 },
    { id: 2, user: "aprendo_feliz", activity: "Ganó medalla 'Explorador Curioso'", time: "Hace 4 horas", points: 50 },
    { id: 3, user: "futuro_ingeniero", activity: "Alcanzó racha de 15 días", time: "Hace 6 horas", points: 75 },
    { id: 4, user: "ciencia_divertida", activity: "Completó lección de Física", time: "Hace 8 horas", points: 40 },
    { id: 5, user: "curiosity_kid", activity: "Subió al nivel 12", time: "Hace 10 horas", points: 100 },
    { id: 6, user: "mente_brillante", activity: "Resolvió 50 problemas", time: "Hace 12 horas", points: 85 }
  ];
}

// Obtener datos de logros y recompensas
async function getAchievementData(connection) {
  // En producción: const [rows] = await connection.execute('SELECT * FROM achievements');
  return {
    popularAchievements: [
      { name: "Racha Increíble", users: 4582, icon: "streak" },
      { name: "Solucionador Maestro", users: 3241, icon: "star" },
      { name: "Explorador Curioso", users: 2876, icon: "compass" },
      { name: "Campeón de Matemáticas", users: 2103, icon: "calculator" }
    ],
    recentAchievements: [
      { name: "Compañero de Estudio", earned: 342, icon: "users" },
      { name: "Racha de 7 días", earned: 287, icon: "calendar" },
      { name: "Primer A+", earned: 213, icon: "award" },
      { name: "10 Lecciones Completadas", earned: 189, icon: "book" }
    ],
    totalAchievements: 45,
    totalBadges: 28,
    totalCertificates: 12
  };
}

// Obtener listado de todos los cursos
async function getAllCourses(connection) {
  // En producción: const [rows] = await connection.execute('SELECT * FROM courses');
  return [
    { id: 1, title: "Matemáticas Básicas", category: "Matemáticas", difficulty: "Principiante", users: 4235, completion: 78.3, rating: 4.8 },
    { id: 2, title: "Ciencias Naturales", category: "Ciencias", difficulty: "Intermedio", users: 3654, completion: 72.5, rating: 4.6 },
    { id: 3, title: "Historia Mundial", category: "Humanidades", difficulty: "Intermedio", users: 2987, completion: 65.8, rating: 4.5 },
    { id: 4, title: "Programación para Niños", category: "Tecnología", difficulty: "Principiante", users: 3421, completion: 82.1, rating: 4.9 },
    { id: 5, title: "Física Divertida", category: "Ciencias", difficulty: "Avanzado", users: 1845, completion: 58.3, rating: 4.7 },
    { id: 6, title: "Inglés Básico", category: "Idiomas", difficulty: "Principiante", users: 5243, completion: 76.8, rating: 4.7 }
  ];
}

// Estadísticas de cursos
async function getCourseStats(connection) {
  // En producción: const [rows] = await connection.execute('SELECT * FROM course_stats');
  return {
    totalCourses: 24,
    totalLessons: 543,
    totalExercises: 12876,
    completionByCategory: [
      { category: "Matemáticas", completion: 75.3 },
      { category: "Ciencias", completion: 68.5 },
      { category: "Humanidades", completion: 62.7 },
      { category: "Tecnología", completion: 81.2 },
      { category: "Idiomas", completion: 76.8 }
    ],
    popularTopics: [
      { topic: "Álgebra", popularity: 87 },
      { topic: "Programación", popularity: 92 },
      { topic: "Inglés", popularity: 85 },
      { topic: "Biología", popularity: 78 },
      { topic: "Historia", popularity: 72 }
    ]
  };
}

export default {
  connect,
  getUserStats,
  getGameProgress,
  getCountryStats,
  getAgeDistribution,
  getAllUsers,
  getRecentActivities,
  getAchievementData,
  getAllCourses,
  getCourseStats
};