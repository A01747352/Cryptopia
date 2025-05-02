import express from 'express';
import connectDb from '../db/db.mjs';

const router = express.Router();

// Obtener todas las criptomonedas
router.get('/monedas', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute('SELECT * FROM criptomoneda');
    res.json(rows);
  } catch (err) {
    console.error('Error al obtener criptomonedas:', err);
    res.status(500).json({ error: 'Error al obtener criptomonedas', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// Obtener datos de trading
router.get('/trading', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    // Utilizando los nombres correctos de las columnas según tu esquema
    const [rows] = await connection.execute(`
      SELECT t.*, 
        c1.nombre as monedaIntercambiada, 
        c2.nombre as monedaRecibida
      FROM trading t
      JOIN criptomoneda c1 ON t.cryptoIntercambiado = c1.idCriptomoneda
      JOIN criptomoneda c2 ON t.cryptoRecibido = c2.idCriptomoneda
      ORDER BY t.fecha DESC 
      LIMIT 50
    `);
    res.json(rows);
  } catch (err) {
    console.error('Error al obtener datos de trading:', err);
    res.status(500).json({ error: 'Error al obtener datos de trading', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

export default router;