import mysql from 'mysql2/promise';

async function connectDb() {
  try {
    // Configuración mejorada para la conexión
    const connection = await mysql.createConnection({
      host: 'database-3.c7uwe4okm9f3.us-east-2.rds.amazonaws.com',
      user: "admin",
      password: "Ivanyaduermete",
      database: 'Cryptopia',
      // Añadiendo opciones para mejorar la estabilidad
      connectTimeout: 20000, // 20 segundos de timeout
      waitForConnections: true,
      // Desactivar el modo estricto para evitar problemas con datos NULL
      charset: 'utf8mb4',
      // Configuración de SSL para conexiones a RDS
      ssl: {
        rejectUnauthorized: false // Para entornos de desarrollo
      }
    });
    
    console.log("Conexión a la BD exitosa");
    return connection;
  } catch (err) {
    console.error('Error al conectar con la base de datos:', err);
    throw err;
  }
}

export default connectDb;