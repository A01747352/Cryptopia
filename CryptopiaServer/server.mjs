import express from 'express';
import mysql from 'mysql2/promise';


const app = express();
const port = 8080;
const ipAddress = 'localhost';

app.use(express.json());

/* Format for server url's.

Use the format /scene/function
The scene corresponds to the scene in Unity.
The urls can be more specific too but the format should be descriptive. 

*/

// Connect to Data Base

async function dbConnect() {
    return await mysql.createConnection({
        host: 'database-3.c7uwe4okm9f3.us-east-2.rds.amazonaws.com',
        user: "admin",
        password: "Ivanyaduermete",
        database: 'Cryptopia',
        multipleStatements: true
    });
}

async function loginVerification(user, password) {
    let connection;
    try {
        connection = await dbConnect();
        const [rows] = await connection.query('SELECT contrasena FROM usuario WHERE username = ?;', [user]);
        const correctPassword = rows[0].contrasena;

        if (rows.length > 0) {
            if (correctPassword === password) {
                return { result: "True" };
            }
            else {
                return { result: "False" };
            }
        }

    } catch (err) {
        console.error("Error al acceder a la base datos:", err);
        return { result: "errorServidor" };
    } finally {
        if (connection) {
            connection.end();
        }
    }
}

async function registration(user, password, firstName, lastName, dateOfBirth, gender, country, occupation) {
    let connection;

    try {
        connection = await dbConnect();
        const [existingUser] = await connection.query('SELECT username FROM usuario WHERE username = ?;', [user]);
        if (existingUser.length > 0) {
            return { result: "usuarioExiste" };
        }

        const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/;
        if (!passwordRegex.test(password)) {
            return { result: "contrasenaInvalida" };
        }

        // Convertir género a formato correcto (un carácter)
        let genderChar = null;
        if (gender) {
            if (gender.toLowerCase().startsWith('m')) {
                genderChar = 'M';
            } else if (gender.toLowerCase().startsWith('f')) {
                genderChar = 'F';
            } else {
                genderChar = 'O'; // Otro
            }
        }

        await connection.query('INSERT INTO usuario(username, contrasena, nombre, apellido, nacimiento, genero, pais, ocupacion) VALUES(?, ?, ?, ?, ?, ?, ?, ?);',
            [user, password, firstName, lastName, dateOfBirth, genderChar, country, occupation]);
        return { result: "registroExitoso" };

    } catch (err) {
        console.error("Error al acceder a la base datos:", err);
        return { result: "errorServidor" };
    } finally {
        if (connection) {
            connection.end();
        }
    }
}


// Login and Register Requests
app.post("/login", async (req, res) => {
    let { user, password } = req.body;
    try {
        const result = await loginVerification(user, password);
        res.status(200).json(result);

    } catch (error) {
        console.error("Error en el endpoint de autenticación:", error);
        res.status(500).json({ result: "errorServidor" });
    }
});

app.post("/register", async (req, res) => {
    let { user, password, firstName, lastName, dateOfBirth, gender, country, occupation } = req.body;
    try {
        const result = await registration(user, password, firstName, lastName, dateOfBirth, gender, country, occupation);
        res.status(200).json(result);

    } catch (error) {
        console.error("Error en el endpoint de autenticación:", error);
        res.status(500).json({ result: "errorServidor" });
    }
});

// Player Progress



// Minigame Cryptography Requests


// Endpoint para cargar palabras de encriptación
app.get('/cryptography/loadEncryption', async (req, res) => {
    let connection;
    try {
        connection = await dbConnect();
        const [rows] = await connection.execute('SELECT * FROM palabracriptografia ORDER BY RAND() LIMIT 1');
        res.send(rows);
    } catch (err) {
        console.error("Error cargando palabras de encriptación:", err);
        const { name, message } = err;
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});

// Endpoint para enviar respuesta de criptografía
app.post('/cryptography/submitAnswer', async (req, res) => {
    let { respuestaUsuario, idPalabra, idPartida } = req.body;
    console.log("Respuesta recibida:", req.body);

    let connection;
    try {
        connection = await dbConnect();
        await connection.query(
            'INSERT INTO respuestausuariocriptografia(respuestaUsuario, idPalabra, idPartida) VALUES(?, ?, ?);',
            [respuestaUsuario, idPalabra, idPartida]
        );
        res.send({ success: true });
    } catch (err) {
        console.error("Error guardando respuesta de criptografía:", err);
        const { name, message } = err;
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});

// Endpoint para guardar resultados del juego de criptografía
app.post('/cryptography/saveGame', async (req, res) => {
    let { idPartida, aciertos, errores, puntaje, TKNs, resultado, idMinijuego, idUsuario } = req.body;
    console.log("Guardando juego de criptografía:", req.body);

    let connection;
    try {
        connection = await dbConnect();
        await connection.execute(
            'UPDATE partidacriptografia SET aciertos = ?, errores = ?, puntaje = ?, TKNs = ?, resultado = ?, idMinijuego = ? WHERE idPartida = ?',
            [aciertos, errores, puntaje, TKNs, resultado, idMinijuego, idPartida]
        );
        await connection.execute(
            'UPDATE wallet SET cantidad = cantidad + ? WHERE idUsuario = ? AND idCriptomoneda = 1',
            [TKNs, idUsuario]
        );
        res.send({ success: true });
    } catch (err) {
        console.error("Error guardando resultados de criptografía:", err);
        const { name, message } = err;
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});



// Endpoint para registrar un nuevo juego de trivia
app.get('/cryptography/newGame/:userId', async (req, res) => {
    let userId = req.params.userId;
    let connection;
    try {
        connection = await dbConnect();
        const [rows] = await connection.query('CALL RegistrateCryptographyGame(?);', [userId]);
        const response = rows[0][0];
        res.send(response);
    } catch (err) {
        const { name, message } = err;
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});

// Minigame CryptoMine Requests 

app.get('/cryptomine/retrieveUserPowerUps/:userId', async (req, res) => {
    let connection;
    let userId = req.params.userId;
    try {
        connection = await dbConnect();
        const [rows] = await connection.execute('SELECT pu.nombre FROM powerupdesbloqueado AS pud LEFT JOIN powerup AS pu ON pud.idPowerUp = pu.idPowerUp WHERE pud.idUsuario = ?;', [userId]);
        const response = rows.map(row => row.nombre).join('-');
        res.send(response);
    }
    catch (err) {
        const { name, message } = err;
        res.status(500).send({ error: name, message });
    }
    finally {
        if (connection) {
            connection.end();
        }
    }
});

app.post('/cryptomine/saveSession/:userId', async (req, res) => {
    let userId = req.params.userId;
    let { TKNs, startSession, endSession, minedBlocks, clicks, score } = req.body;
    let connection;
    try {
        connection = await dbConnect();
        console.log("Conexion exitosa");
        await connection.execute('INSERT INTO sesioncryptomine(idUsuario, TKNs, bloquesMinados, clicks, puntaje, inicioSesion, terminaSesion, idMinijuego) VALUES(?, ?, ?, ?, ?, ?, ?, 3);', [userId, TKNs, minedBlocks, clicks, score, startSession, endSession]);
        console.log("Consulta exitosa");
        await connection.execute('UPDATE wallet SET cantidad = cantidad + ? WHERE idUsuario = ? AND idCriptomoneda = 1', [TKNs, userId]);
        res.send({ success: true });
    }
    catch (err) {
        const { name, message } = err;
        res.status(500).send({ error: name, message });
    }
    finally {
        if (connection) {
            connection.end();
        }
    }
});

app.get('/cryptomine/loadUserData/:userId', async (req, res) => {
    let userId = req.params.userId;
    let connection;
    try {
        connection = await dbConnect();
        const [rows] = await connection.execute('SELECT cantidad FROM wallet WHERE idUsuario = ? AND idCriptomoneda = 1;', [userId]);
        const TKNs = rows[0].cantidad || 0;
        const [rows2] = await connection.execute('SELECT SUM(bloquesMinados) AS totalBloquesMinados FROM sesioncryptomine WHERE idUsuario = ?;', [userId]);
        const totalBloquesMinados = rows2[0].totalBloquesMinados || 0;
        const response = {
            TKNs: TKNs,
            totalBloquesMinados: totalBloquesMinados
        };
        res.send(response);
    }
    catch (err) {
        const { name, message } = err;
        res.status(500).send({ error: name, message });
    }
    finally {
        if (connection) {
            connection.end();
        }
    }
});


app.get('/trivia/newGame/:userId', async (req, res) => {
    let userId = req.params.userId;
    let connection;
    try {
        connection = await dbConnect();
        const [rows] = await connection.query('CALL RegistrateTriviagame(?);', [userId]);
        const response = rows[0][0];
        res.send(response);
    } catch (err) {
        const { name, message } = err;
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});

// Endpoint para obtener todas las preguntas con sus respuestas
app.get('/trivia/getAllQuestions', async (req, res) => {
    let connection;
    try {
        connection = await dbConnect();

        // Obtenemos todas las preguntas de trivia
        const [questions] = await connection.execute('SELECT * FROM preguntatrivia');

        if (questions.length === 0) {
            res.status(404).send({ error: "No se encontraron preguntas" });
            return;
        }

        // Preparamos un array para almacenar cada pregunta con sus respuestas
        const questionsWithAnswers = [];

        // Para cada pregunta, obtenemos sus respuestas
        for (const question of questions) {
            const [answers] = await connection.execute(
                'SELECT * FROM respuestastrivia WHERE idPregunta = ?',
                [question.idPregunta]
            );

            // Mezclamos las respuestas para que no siempre estén en el mismo orden
            const shuffledAnswers = shuffleArray(answers);

            // Agregamos la pregunta con sus respuestas al array
            questionsWithAnswers.push({
                question: question,
                answers: shuffledAnswers
            });
        }

        res.send(questionsWithAnswers);
    } catch (err) {
        console.error("Error obteniendo preguntas de trivia:", err);
        const { name, message } = err;
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});

// Función auxiliar para mezclar un array (algoritmo Fisher-Yates)
function shuffleArray(array) {
    const newArray = [...array];
    for (let i = newArray.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [newArray[i], newArray[j]] = [newArray[j], newArray[i]];
    }
    return newArray;
}

// Endpoint para enviar respuesta de usuario (versión personalizada)
// Endpoint para enviar respuesta de usuario (versión personalizada)
app.post('/trivia/submitAnswer', async (req, res) => {
    let { respuestaUsuario, idPregunta, esCorrecta, idPartida } = req.body;
    console.log("Respuesta de trivia recibida:", req.body);

    let connection;
    try {
        connection = await dbConnect();
        await connection.query(
            'INSERT INTO respuestausuariotrivia(respuestaUsuario, idPregunta, esCorrecta, idPartida) VALUES(?, ?, ?, ?);',
            [respuestaUsuario, idPregunta, esCorrecta, idPartida]
        );
        res.send({ success: true });
    } catch (err) {
        console.error("Error guardando respuesta de trivia:", err);
        const { name, message } = err;
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});


// Endpoint para guardar resultados del juego
app.post('/trivia/saveGame', async (req, res) => {
    let { idPartida, resultado, porcentajeAciertos, puntaje, TKNs, idMinijuego, idUsuario } = req.body;
    console.log("Guardando juego de trivia:", req.body);

    let connection;
    try {
        connection = await dbConnect();

        // Actualizar la partida de trivia
        await connection.execute(
            'UPDATE partidatrivia SET resultado = ?, porcentajeAciertos = ?, puntaje = ?, TKNs = ? WHERE idPartida = ?',
            [resultado, porcentajeAciertos, puntaje, TKNs, idPartida]
        );

        // Añadir TKNs al wallet del usuario
        await connection.execute(
            'UPDATE wallet SET cantidad = cantidad + ? WHERE idUsuario = ? AND idCriptomoneda = 1',
            [TKNs, idUsuario]
        );

        res.send({ success: true });
    } catch (err) {
        console.error("Error guardando resultados de trivia:", err);
        const { name, message } = err;
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});


app.use((req, res) => {
    const url = req.originalUrl;
    res.status(404).render('not_found', { url });
});

app.listen(port, () => {
    console.log(`Servidor esperando en: http://${ipAddress}:${port}`);
});