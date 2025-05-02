import express from 'express';
import cors from 'cors';
import usersRoutes from './routes/users.mjs';
import triviaRoutes from './routes/trivia.mjs';
import analyticsRoutes from './routes/analytics.mjs';
import cryptoRoutes from './routes/crypto.mjs';

const app = express();
const port = process.env.PORT || 8081;

// Middleware
app.use(cors());
app.use(express.json());

// Rutas
app.use('/api/users', usersRoutes);
app.use('/api/trivia', triviaRoutes);
app.use('/api/analytics', analyticsRoutes);
app.use('/api/crypto', cryptoRoutes);

// Ruta base
app.get('/', (req, res) => {
  res.json({ message: 'Bienvenido a la API de Cryptopia' });
});

// Manejo de rutas no encontradas
app.use((req, res) => {
  res.status(404).json({ error: 'Ruta no encontrada' });
});

// Iniciar servidor
app.listen(port, () => {
  console.log(`Servidor ejecutándose en http://localhost:${port}`);
});