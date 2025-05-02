import express from 'express';
import connectDb from '../db/db.mjs';

const router = express.Router();

// Obtener estadísticas generales para el dashboard
router.get('/dashboard', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    
    // Estadísticas de edad - Corregido para coincidir con tu columna 'edad' en la tabla 'usuario'
    const [edadStats] = await connection.execute(`
      SELECT
        COUNT(CASE WHEN edad < 18 THEN 1 END) as grupo_0_17,
        COUNT(CASE WHEN edad BETWEEN 18 AND 24 THEN 1 END) as grupo_18_24,
        COUNT(CASE WHEN edad BETWEEN 25 AND 34 THEN 1 END) as grupo_25_34,
        COUNT(CASE WHEN edad BETWEEN 35 AND 44 THEN 1 END) as grupo_35_44,
        COUNT(CASE WHEN edad >= 45 THEN 1 END) as grupo_45_plus
      FROM usuario
    `);
    
    // Estadísticas de género - Corregido para coincidir con tu columna 'genero'
    const [generoStats] = await connection.execute(`
      SELECT
        COUNT(CASE WHEN genero = 'M' THEN 1 END) as masculino,
        COUNT(CASE WHEN genero = 'F' THEN 1 END) as femenino,
        COUNT(CASE WHEN genero NOT IN ('M', 'F') OR genero IS NULL THEN 1 END) as otro,
        COUNT(*) as total
      FROM usuario
    `);
    
    // Estadísticas de sesiones - Corregido para usar la tabla 'sesioncryptomine'
    const [sesionesStats] = await connection.execute(`
      SELECT COUNT(*) as total FROM sesioncryptomine
    `);
    
    // Estadísticas de ingresos - Usando datos de 'trading'
    // Esta es una aproximación, ajústala según cómo quieras calcular los ingresos
    const [ingresosStats] = await connection.execute(`
      SELECT COALESCE(SUM(cantidadRecibida), 0) as total 
      FROM trading
    `);
    
    res.json({
      edad: edadStats[0],
      genero: generoStats[0],
      sesiones: sesionesStats[0].total || 0,
      ingresos: parseFloat(ingresosStats[0].total || 0)
    });
  } catch (err) {
    console.error('Error en dashboard:', err);
    res.status(500).json({ 
      error: 'Error al obtener estadísticas', 
      details: err.message 
    });
  } finally {
    if (connection) await connection.end();
  }
});

// Obtener datos de retención por día de la semana
router.get('/retencion', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute(`
      SELECT 
        DAYNAME(fechaRegistro) as dia,
        COUNT(*) as usuarios
      FROM usuario
      GROUP BY DAYNAME(fechaRegistro)
      ORDER BY FIELD(DAYNAME(fechaRegistro), 'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday')
    `);
    res.json(rows);
  } catch (err) {
    console.error('Error en retención:', err);
    res.status(500).json({ error: 'Error al obtener datos de retención', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// NUEVOS ENDPOINTS

// Promedio de aciertos por usuario
router.get('/average-accuracy', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute(`
      SELECT AVG(porcentajeAciertos) AS promedioAciertos FROM partidatrivia
    `);
    res.json({
      promedioAciertos: parseFloat(rows[0].promedioAciertos || 0).toFixed(1)
    });
  } catch (err) {
    console.error('Error en promedio de aciertos:', err);
    res.status(500).json({ error: 'Error al obtener promedio de aciertos', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// Usuarios activos por día
router.get('/active-users', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute(`
      SELECT 
        DATE(fechaPartida) AS dia, 
        COUNT(DISTINCT idUsuario) AS usuarios 
      FROM partidatrivia 
      GROUP BY dia 
      ORDER BY dia DESC 
      LIMIT 7
    `);
    res.json(rows);
  } catch (err) {
    console.error('Error en usuarios activos:', err);
    res.status(500).json({ error: 'Error al obtener usuarios activos', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// PowerUps más usados
router.get('/top-powerups', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute(`
      SELECT 
        p.nombre, 
        COUNT(*) AS veces_usado 
      FROM powerupdesbloqueado pd 
      JOIN powerup p ON pd.idPowerUp = p.idPowerUp 
      WHERE pd.activado = 1 
      GROUP BY p.nombre 
      ORDER BY veces_usado DESC 
      LIMIT 5
    `);
    res.json(rows);
  } catch (err) {
    console.error('Error en top powerups:', err);
    res.status(500).json({ error: 'Error al obtener powerups más usados', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// Ranking de usuarios por puntaje
router.get('/user-ranking', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute(`
      
select username, puntajeTotal  AS totalPuntaje from userprogress ORDER BY totalPuntaje DESC LIMIT 5;
    `);
    res.json(rows);
  } catch (err) {
    console.error('Error en ranking de usuarios:', err);
    res.status(500).json({ error: 'Error al obtener ranking de usuarios', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// Estadísticas de trading
router.get('/trading-stats', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute(`
      SELECT 
        COUNT(*) AS totalTrades, 
        SUM(cantidadIntercambiada) AS totalVolumen 
      FROM trading
    `);
    res.json(rows[0]);
  } catch (err) {
    console.error('Error en estadísticas de trading:', err);
    res.status(500).json({ error: 'Error al obtener estadísticas de trading', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// TKNs ganados por día
router.get('/tkns-by-day', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute(`
      SELECT 
        DATE(fechaPartida) AS dia, 
        SUM(TKNs) AS totalTKNs 
      FROM partidatrivia 
      GROUP BY dia 
      ORDER BY dia DESC 
      LIMIT 7
    `);
    res.json(rows);
  } catch (err) {
    console.error('Error en TKNs por día:', err);
    res.status(500).json({ error: 'Error al obtener TKNs por día', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// Criptomonedas más almacenadas
router.get('/top-cryptocurrencies', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute(`
      SELECT 
        c.nombre, 
        SUM(w.cantidad) AS total 
      FROM wallet w 
      JOIN criptomoneda c ON w.idCriptomoneda = c.idCriptomoneda 
      GROUP BY c.nombre 
      ORDER BY total DESC
      LIMIT 5
    `);
    res.json(rows);
  } catch (err) {
    console.error('Error en top criptomonedas:', err);
    res.status(500).json({ error: 'Error al obtener criptomonedas más almacenadas', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// Preguntas con más errores
router.get('/most-failed-questions', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute(`
      SELECT 
        pt.pregunta, 
        COUNT(*) AS errores 
      FROM respuestausuariotrivia rut 
      JOIN preguntatrivia pt ON rut.idPregunta = pt.idPregunta 
      WHERE esCorrecta = 0 
      GROUP BY pt.pregunta 
      ORDER BY errores DESC 
      LIMIT 5
    `);
    res.json(rows);
  } catch (err) {
    console.error('Error en preguntas con más errores:', err);
    res.status(500).json({ error: 'Error al obtener preguntas con más errores', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

export default router;