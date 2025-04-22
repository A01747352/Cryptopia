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

export default router;