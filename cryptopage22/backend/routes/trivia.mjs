import express from 'express';
import connectDb from '../db/db.mjs';

const router = express.Router();

// Obtener todas las preguntas
router.get('/preguntas', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute('SELECT * FROM preguntatrivia');
    res.json(rows);
  } catch (err) {
    console.error('Error al obtener preguntas:', err);
    res.status(500).json({ error: 'Error al obtener preguntas', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// Obtener respuestas para una pregunta
router.get('/preguntas/:id/respuestas', async (req, res) => {
  const { id } = req.params;
  let connection;
  try {
    connection = await connectDb();
    const [rows] = await connection.execute('SELECT * FROM respuestastrivia WHERE idPregunta = ?', [id]);
    res.json(rows);
  } catch (err) {
    console.error('Error al obtener respuestas:', err);
    res.status(500).json({ error: 'Error al obtener respuestas', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

// Obtener estadísticas de preguntas (para el dashboard)
router.get('/estadisticas', async (req, res) => {
  let connection;
  try {
    connection = await connectDb();
    
    // Preguntas más acertadas
    const [top] = await connection.execute(`
      SELECT p.idPregunta, p.pregunta, COUNT(r.idRespuestaUsuario) as totalCorrectas
      FROM preguntatrivia p
      JOIN respuestausuariotrivia r ON p.idPregunta = r.idPregunta
      WHERE r.esCorrecta = 1
      GROUP BY p.idPregunta
      ORDER BY totalCorrectas DESC
      LIMIT 3
    `);
    
    // Preguntas menos acertadas
    const [bottom] = await connection.execute(`
      SELECT p.idPregunta, p.pregunta, COUNT(r.idRespuestaUsuario) as totalIncorrectas
      FROM preguntatrivia p
      JOIN respuestausuariotrivia r ON p.idPregunta = r.idPregunta
      WHERE r.esCorrecta = 0
      GROUP BY p.idPregunta
      ORDER BY totalIncorrectas DESC
      LIMIT 3
    `);
    
    // Porcentajes de respuestas correctas
    const [correctStats] = await connection.execute(`
      SELECT p.idPregunta, p.pregunta, 
             COUNT(CASE WHEN r.esCorrecta = 1 THEN 1 END) as correctas,
             COUNT(r.idRespuestaUsuario) as total,
             ROUND((COUNT(CASE WHEN r.esCorrecta = 1 THEN 1 END) / COUNT(r.idRespuestaUsuario)) * 100) as porcentaje
      FROM preguntatrivia p
      JOIN respuestausuariotrivia r ON p.idPregunta = r.idPregunta
      GROUP BY p.idPregunta
      ORDER BY porcentaje DESC
    `);
    
    res.json({
      topCorrectas: top,
      bottomCorrectas: bottom,
      estadisticasPorcentajes: correctStats
    });
  } catch (err) {
    console.error('Error al obtener estadísticas:', err);
    res.status(500).json({ error: 'Error al obtener estadísticas', details: err.message });
  } finally {
    if (connection) await connection.end();
  }
});

export default router;