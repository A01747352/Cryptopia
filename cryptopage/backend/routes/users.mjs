import express from 'express';
import connectDb from '../db/db.mjs';

const router = express.Router();

// Obtener todos los usuarios
router.get('/', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute('SELECT * FROM usuario');
    res.json(rows);
  } catch (err) {
    console.error('Error al obtener usuarios:', err);
    res.status(500).json({ error: 'Error al obtener usuarios', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// Obtener un usuario por ID
router.get('/:id', async (req, res) => {
  const { id } = req.params;
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute('SELECT * FROM usuario WHERE Id = ?', [id]);
    if (rows.length === 0) {
      return res.status(404).json({ error: 'Usuario no encontrado' });
    }
    res.json(rows[0]);
  } catch (err) {
    console.error('Error al obtener usuario:', err);
    res.status(500).json({ error: 'Error al obtener usuario', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// Obtener wallet de un usuario
router.get('/:id/wallet', async (req, res) => {
  const { id } = req.params;
  let connection;
  try {
    connection = await connectDb();
    // Ajustando para usar 'nombre' y 'abreviatura' de la tabla 'criptomoneda'
    const [rows] = await connection.execute(`
      SELECT w.*, c.nombre, c.abreviatura 
      FROM wallet w 
      JOIN criptomoneda c ON w.idCriptomoneda = c.idCriptomoneda
      WHERE w.idUsuario = ?`, [id]);
    res.json(rows);
  } catch (err) {
    console.error('Error al obtener wallet:', err);
    res.status(500).json({ error: 'Error al obtener wallet', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

export default router;