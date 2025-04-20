// db-inspector-fixed.mjs
import express from 'express';
import mysql from 'mysql2/promise';

const app = express();
const port = 8080;
const ipAddress = 'localhost';

app.use(express.json());

// Función de conexión a la base de datos
async function dbConnect() {
    return await mysql.createConnection({
        host: 'cryptochicks-db.c7uwe4okm9f3.us-east-2.rds.amazonaws.com',
        user: "admin",
        password: "Ivanyaduermete",
        database: 'Cryptopia',
        multipleStatements: true
    });
}

// Ruta para probar la conexión básica
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
        
        statusInfo.success = true;
        
    } catch (err) {
        console.error('❌ ERROR DE CONEXIÓN:', err);
        statusInfo.error = {
            name: err.name,
            message: err.message,
            code: err.code,
            sqlState: err.sqlState || 'N/A'
        };
    } finally {
        if (connection) {
            await connection.end();
            console.log('Conexión cerrada');
        }
    }
    
    // Enviar resultados como JSON
    res.json(statusInfo);
});

// Ruta para inspeccionar la estructura de todas las tablas (excepto vistas problemáticas)
app.get('/inspect-tables', async (req, res) => {
    let connection;
    let tablesInfo = {};
    let problemTables = []; // Para almacenar tablas que dan error

    try {
        connection = await dbConnect();
        
        // Obtener lista de tablas
        const [tables] = await connection.query('SHOW TABLES');
        const tableNames = tables.map(t => Object.values(t)[0]);
        
        // Lista de tablas a excluir porque son vistas con problemas
        const excludeTables = ['usercryptographyprogress']; 
        
        // Para cada tabla, obtener su estructura
        for (const tableName of tableNames) {
            // Saltarse las tablas excluidas
            if (excludeTables.includes(tableName)) {
                problemTables.push({
                    tableName: tableName,
                    reason: "Vista con referencias inválidas"
                });
                continue;
            }
            
            try {
                const [columns] = await connection.query(`DESCRIBE ${tableName}`);
                const [rowCount] = await connection.query(`SELECT COUNT(*) as count FROM ${tableName}`);
                
                tablesInfo[tableName] = {
                    columns: columns,
                    rowCount: rowCount[0].count
                };
            } catch (err) {
                problemTables.push({
                    tableName: tableName,
                    error: err.message
                });
                console.error(`Error al inspeccionar tabla ${tableName}:`, err.message);
            }
        }
        
        res.json({
            tables: tablesInfo,
            problemTables: problemTables
        });
        
    } catch (err) {
        console.error('Error al inspeccionar tablas:', err);
        res.status(500).json({
            error: err.message,
            code: err.code
        });
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Ruta para ver todas las preguntas de trivia con sus respuestas
app.get('/inspect-trivia', async (req, res) => {
    let connection;
    let triviaData = {
        success: false,
        preguntas: [],
        error: null
    };

    try {
        connection = await dbConnect();
        
        // Obtener todas las preguntas
        const [questions] = await connection.execute('SELECT * FROM preguntatrivia');
        
        // Para cada pregunta, obtener sus respuestas
        for (const question of questions) {
            const [answers] = await connection.execute(
                'SELECT * FROM respuestastrivia WHERE idPregunta = ?', 
                [question.idPregunta]
            );
            
            triviaData.preguntas.push({
                pregunta: question,
                respuestas: answers
            });
        }
        
        triviaData.success = true;
        res.json(triviaData);
        
    } catch (err) {
        console.error('Error al inspeccionar preguntas de trivia:', err);
        triviaData.error = {
            message: err.message,
            code: err.code
        };
        res.status(500).json(triviaData);
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Ruta para ver palabras de criptografía
app.get('/inspect-cryptography', async (req, res) => {
    let connection;
    let cryptoData = {
        success: false,
        palabras: [],
        error: null
    };

    try {
        connection = await dbConnect();
        
        // Obtener todas las palabras criptográficas
        const [words] = await connection.execute('SELECT * FROM palabracriptografia');
        cryptoData.palabras = words;
        cryptoData.success = true;
        
        res.json(cryptoData);
        
    } catch (err) {
        console.error('Error al inspeccionar palabras criptográficas:', err);
        cryptoData.error = {
            message: err.message,
            code: err.code
        };
        res.status(500).json(cryptoData);
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Ruta para ver usuarios (corregida con los campos que realmente existen)
app.get('/inspect-users', async (req, res) => {
    let connection;
    let userData = {
        success: false,
        usuarios: [],
        wallets: [],
        error: null
    };

    try {
        connection = await dbConnect();
        
        // Primero, veamos la estructura de la tabla usuario para conocer sus columnas
        const [userColumns] = await connection.query('DESCRIBE usuario');
        console.log('Columnas de la tabla usuario:', userColumns.map(col => col.Field));
        
        // Obtener usuarios (sin contraseñas por seguridad)
        // Vamos a excluir la contraseña y obtener todas las demás columnas
        let userFields = userColumns
            .map(col => col.Field)
            .filter(field => field !== 'contrasena')
            .join(', ');
        
        const [users] = await connection.execute(`SELECT ${userFields} FROM usuario`);
        userData.usuarios = users;
        
        // Obtener estructura de wallet para saber qué columnas tiene
        const [walletColumns] = await connection.query('DESCRIBE wallet');
        console.log('Columnas de la tabla wallet:', walletColumns.map(col => col.Field));
        
        // Obtener wallets
        const [wallets] = await connection.execute('SELECT * FROM wallet');
        userData.wallets = wallets;
        
        userData.success = true;
        res.json(userData);
        
    } catch (err) {
        console.error('Error al inspeccionar usuarios:', err);
        userData.error = {
            message: err.message,
            code: err.code
        };
        res.status(500).json(userData);
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Ruta para probar procedimientos almacenados
app.get('/test-stored-procedures', async (req, res) => {
    let connection;
    let proceduresData = {
        success: false,
        procedures: [],
        testResults: {},
        error: null
    };

    try {
        connection = await dbConnect();
        
        // Listar procedimientos almacenados
        const [procedures] = await connection.query(`
            SELECT ROUTINE_NAME 
            FROM INFORMATION_SCHEMA.ROUTINES 
            WHERE ROUTINE_TYPE = 'PROCEDURE' 
            AND ROUTINE_SCHEMA = 'Cryptopia'
        `);
        
        proceduresData.procedures = procedures.map(p => p.ROUTINE_NAME);
        
        // Intentar obtener información sobre los procedimientos específicos mencionados en el código
        const proceduresToCheck = ['RegistrateCryptographyGame', 'RegistrateTriviagame'];
        
        for (const proc of proceduresToCheck) {
            try {
                const [paramInfo] = await connection.query(`
                    SELECT PARAMETER_NAME, DATA_TYPE, PARAMETER_MODE
                    FROM INFORMATION_SCHEMA.PARAMETERS
                    WHERE SPECIFIC_NAME = ?
                    AND ROUTINE_SCHEMA = 'Cryptopia'
                    ORDER BY ORDINAL_POSITION
                `, [proc]);
                
                proceduresData.testResults[proc] = {
                    exists: true,
                    parameters: paramInfo
                };
                
            } catch (err) {
                proceduresData.testResults[proc] = {
                    exists: false,
                    error: err.message
                };
            }
        }
        
        proceduresData.success = true;
        res.json(proceduresData);
        
    } catch (err) {
        console.error('Error al probar procedimientos almacenados:', err);
        proceduresData.error = {
            message: err.message,
            code: err.code
        };
        res.status(500).json(proceduresData);
    } finally {
        if (connection) {
            await connection.end();
        }
    }
});

// Página principal con enlaces a todas las herramientas
app.get('/', (req, res) => {
    res.send(`
        <!DOCTYPE html>
        <html>
        <head>
            <title>Inspector de Base de Datos - Cryptopia</title>
            <style>
                body {
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    line-height: 1.6;
                    color: #333;
                    max-width: 900px;
                    margin: 0 auto;
                    padding: 20px;
                }
                
                h1 {
                    color: #2c3e50;
                    text-align: center;
                    margin-bottom: 30px;
                    padding-bottom: 10px;
                    border-bottom: 2px solid #3498db;
                }
                
                .tools-container {
                    display: grid;
                    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
                    gap: 20px;
                }
                
                .tool-card {
                    background-color: #f8f9fa;
                    border-radius: 8px;
                    padding: 20px;
                    box-shadow: 0 2px 5px rgba(0,0,0,0.1);
                    transition: transform 0.3s ease, box-shadow 0.3s ease;
                }
                
                .tool-card:hover {
                    transform: translateY(-5px);
                    box-shadow: 0 5px 15px rgba(0,0,0,0.1);
                }
                
                h2 {
                    color: #3498db;
                    margin-top: 0;
                }
                
                a {
                    display: inline-block;
                    margin-top: 10px;
                    color: #fff;
                    background-color: #3498db;
                    padding: 8px 16px;
                    border-radius: 4px;
                    text-decoration: none;
                    transition: background-color 0.3s ease;
                }
                
                a:hover {
                    background-color: #2980b9;
                }
                
                p {
                    margin-bottom: 15px;
                }
                
                .footer {
                    margin-top: 40px;
                    text-align: center;
                    font-size: 0.9em;
                    color: #7f8c8d;
                }
            </style>
        </head>
        <body>
            <h1>Inspector de Base de Datos - Cryptopia</h1>
            
            <div class="tools-container">
                <div class="tool-card">
                    <h2>Prueba de Conexión</h2>
                    <p>Verifica la conexión básica a la base de datos y muestra las tablas disponibles.</p>
                    <a href="/test-connection" target="_blank">Probar Conexión</a>
                </div>
                
                <div class="tool-card">
                    <h2>Estructura de Tablas</h2>
                    <p>Inspecciona la estructura de todas las tablas y muestra el número de registros en cada una.</p>
                    <a href="/inspect-tables" target="_blank">Ver Estructura</a>
                </div>
                
                <div class="tool-card">
                    <h2>Preguntas de Trivia</h2>
                    <p>Muestra todas las preguntas de trivia con sus respuestas asociadas.</p>
                    <a href="/inspect-trivia" target="_blank">Ver Trivia</a>
                </div>
                
                <div class="tool-card">
                    <h2>Palabras Criptográficas</h2>
                    <p>Muestra todas las palabras utilizadas en el juego de criptografía.</p>
                    <a href="/inspect-cryptography" target="_blank">Ver Palabras</a>
                </div>
                
                <div class="tool-card">
                    <h2>Usuarios y Wallets</h2>
                    <p>Inspecciona los usuarios registrados y sus carteras asociadas.</p>
                    <a href="/inspect-users" target="_blank">Ver Usuarios</a>
                </div>
                
                <div class="tool-card">
                    <h2>Procedimientos Almacenados</h2>
                    <p>Prueba los procedimientos almacenados utilizados por la aplicación.</p>
                    <a href="/test-stored-procedures" target="_blank">Probar Procedimientos</a>
                </div>
            </div>
            
            <div class="footer">
                <p>Inspector de Base de Datos para Cryptopia | Servidor corriendo en: http://${ipAddress}:${port}</p>
            </div>
        </body>
        </html>
    `);
});

// Iniciar el servidor
app.listen(port, ipAddress, () => {
    console.log(`Servidor inspector iniciado en: http://${ipAddress}:${port}`);
    console.log(`Abre el navegador y visita la dirección anterior para comenzar la inspección.`);
});