// debug-db-express.js
import express from 'express';
import mysql from 'mysql2/promise';

const app = express();
const port = 8080;
const ipAddress = 'localhost';

app.use(express.json());

// Función de conexión a la base de datos (igual a la de tu código original)
async function dbConnect() {
    return await mysql.createConnection({
        host: 'cryptochicks-db.c7uwe4okm9f3.us-east-2.rds.amazonaws.com',
        user: "admin",
        password: "Ivanyaduermete",
        database: 'Cryptopia',
        multipleStatements: true
    });
}

// Ruta para probar la conexión
app.get('/test-connection', async (req, res) => {
    let connection;
    let statusInfo = {
        success: false,
        connectionTest: false,
        basicQuery: false,
        tablesFound: [],
        error: null
    };

    try {
        console.log('Intentando conectar a la base de datos...');
        
        // Intentar establecer conexión
        connection = await dbConnect();
        statusInfo.connectionTest = true;
        console.log('✅ Conexión establecida correctamente');
        
        // Probar una consulta simple
        console.log('Probando consulta simple...');
        const [result] = await connection.query('SELECT 1 + 1 AS solution');
        statusInfo.basicQuery = true;
        console.log(`Resultado de prueba: ${result[0].solution}`);
        
        // Listar las tablas disponibles
        console.log('Listando tablas disponibles:');
        const [tables] = await connection.query('SHOW TABLES');
        
        tables.forEach(table => {
            const tableName = Object.values(table)[0];
            statusInfo.tablesFound.push(tableName);
            console.log(`- ${tableName}`);
        });
        
        // Probar si podemos acceder a algunas tablas mencionadas en tu código
        console.log('Verificando tablas específicas mencionadas en el código...');
        
        const tablesToCheck = [
            'usuario', 
            'palabracriptografia', 
            'partidacriptografia',
            'respuestausuariocriptografia',
            'preguntatrivia',
            'respuestastrivia',
            'wallet'
        ];
        
        let tableStatus = {};
        
        for (const table of tablesToCheck) {
            try {
                // Intentar contar registros en cada tabla
                const [count] = await connection.query(`SELECT COUNT(*) as count FROM ${table}`);
                tableStatus[table] = {
                    exists: true,
                    count: count[0].count
                };
                console.log(`✅ Tabla ${table}: ${count[0].count} registros`);
            } catch (err) {
                tableStatus[table] = {
                    exists: false,
                    error: err.message
                };
                console.log(`❌ Error al acceder a tabla ${table}: ${err.message}`);
            }
        }
        
        statusInfo.tableDetails = tableStatus;
        statusInfo.success = true;
        
    } catch (err) {
        console.error('❌ ERROR DE CONEXIÓN:', err);
        statusInfo.error = {
            name: err.name,
            message: err.message,
            code: err.code,
            sqlState: err.sqlState || 'N/A'
        };
        
        // Agregar sugerencias según el tipo de error
        if (err.code === 'ECONNREFUSED') {
            statusInfo.suggestion = 'El servidor no está aceptando conexiones. Verifica la dirección del host, puerto y firewall.';
        } else if (err.code === 'ER_ACCESS_DENIED_ERROR') {
            statusInfo.suggestion = 'Credenciales incorrectas. Verifica el nombre de usuario y contraseña.';
        } else if (err.code === 'ER_BAD_DB_ERROR') {
            statusInfo.suggestion = 'La base de datos no existe. Verifica el nombre de la base de datos y tus permisos.';
        } else if (err.code === 'ETIMEDOUT') {
            statusInfo.suggestion = 'Tiempo de espera agotado. Verifica conectividad de red y reglas de seguridad.';
        }
        
    } finally {
        if (connection) {
            await connection.end();
            console.log('Conexión cerrada');
        }
    }
    
    // Enviar resultados como JSON
    res.json(statusInfo);
});

// Página de inicio simple para instrucciones
app.get('/', (req, res) => {
    res.send(`
        <html>
            <head>
                <title>Debug de Conexión a MySQL</title>
                <style>
                    body { font-family: Arial, sans-serif; margin: 40px; line-height: 1.6; }
                    h1 { color: #333; }
                    .container { max-width: 800px; margin: 0 auto; }
                    .info { background-color: #f0f0f0; padding: 20px; border-radius: 5px; }
                    .code { background-color: #f5f5f5; padding: 10px; border-radius: 3px; font-family: monospace; }
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>Debug de Conexión a MySQL</h1>
                    <div class="info">
                        <p>Este servidor ofrece una API para probar la conexión a la base de datos MySQL.</p>
                        <p>Para probar la conexión, visita la siguiente URL:</p>
                        <p class="code">http://${ipAddress}:${port}/test-connection</p>
                        <p>Los resultados se mostrarán como un objeto JSON con información detallada sobre la conexión.</p>
                    </div>
                </div>
            </body>
        </html>
    `);
});

// Iniciar el servidor
app.listen(port, ipAddress, () => {
    console.log(`Servidor de debug iniciado en: http://${ipAddress}:${port}`);
    console.log(`Para probar la conexión a la base de datos, visita: http://${ipAddress}:${port}/test-connection`);
});