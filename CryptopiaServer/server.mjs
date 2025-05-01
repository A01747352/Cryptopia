import express from 'express';
import mysql from 'mysql2/promise';
import axios from 'axios';


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
        const [rows] = await connection.query('SELECT Id, contrasena FROM usuario WHERE username = ?;', [user]);

        if (rows.length > 0) {
            const correctPassword = rows[0].contrasena;
            const userId = rows[0].Id;

            if (correctPassword === password) {
                return { result: "True", userId: userId };
            } else {
                return { result: "False" };
            }
        } else {
            return { result: "False" };
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

        await connection.query('CALL InsertUsuario(?, ?, ?, ?, ?, ?, ?, ?);',
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
            'CALL InsertRespuestaUsuarioCriptografia(?, ?, ?);',
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
            'CALL saveGameCryptography(?, ?, ?, ?, ?, ?, ?);',
            [idPartida, idUsuario, aciertos, errores, puntaje, TKNs, resultado]
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
app.post('/trivia/submitAnswer', async (req, res) => {
    let { respuestaUsuario, idPregunta, esCorrecta, idPartida } = req.body;
    console.log("Respuesta de trivia recibida:", req.body);

    let connection;
    try {
        connection = await dbConnect();
        await connection.query(
            'CALL InsertRespuestaUsuarioTrivia(?, ?, ?, ?);',
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
            'CALL saveGameTrivia(?, ?, ?, ?, ?, ?);',
            [idPartida, idUsuario, resultado, porcentajeAciertos, puntaje, TKNs]
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

// Minigame CryptoMine Requests 

app.get('/cryptomine/retrieveUserPowerUps/:userId', async (req, res) => {
    let connection;
    let userId = req.params.userId;
    try {
        connection = await dbConnect();
        const [rows] = await connection.execute('SELECT pu.nombre, pu.descripcion FROM powerupdesbloqueado AS pud LEFT JOIN powerup AS pu ON pud.idPowerUp = pu.idPowerUp WHERE pud.idUsuario = ?;', [userId]);
        res.send(rows);
        console.log(rows);
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
        
        await connection.execute('CALL InsertSesionCryptoMine(?, ?, ?, ?, ?, ?, ?);', [userId, TKNs, minedBlocks, clicks, score, startSession, endSession]);

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
        console.log("Conexion exitosa");
        const [rows] = await connection.execute('SELECT cantidad FROM wallet WHERE idUsuario = ? AND idCriptomoneda = 1;', [userId]);
        const TKNs = rows.length > 0 ? rows[0].cantidad : 0; // Default to 0 if no rows are returned
        const [rows2] = await connection.execute('SELECT SUM(sc.bloquesMinados) AS totalBloquesMinados, up.puntajeTotal AS PuntajeTotal FROM sesioncryptomine AS sc INNER JOIN userprogress AS up WHERE idUsuario = ?;', [userId]);
        const totalBloquesMinados = rows2[0].totalBloquesMinados || 0;
        const puntajeTotal = rows2[0].PuntajeTotal || 0;
        const response = {
            TKNs: TKNs,
            totalBloquesMinados: totalBloquesMinados,
            puntajeTotal: puntajeTotal
        };
        res.send(response);
        console.log("User Data: ", response);
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

// Trading Endpoints

app.get('/trading/getUserWallet/:userId', async (req, res) => {
    let userId = req.params.userId;
    let connection;
    try {
        connection = await dbConnect();
        const [rows] = await connection.execute('SELECT c.nombre, c.tokenId, c.abreviatura, w.cantidad FROM criptomoneda AS c LEFT JOIN wallet AS w ON c.idCriptomoneda = w.idCriptomoneda AND w.idUsuario = 1;', [userId]);
        const idTokens = rows.filter(row => row.tokenId !== 0).map(row => row.tokenId).join(',');
        const options = 
        {
            method: 'GET',
            headers: {accept: 'application/json', api_key: 'tm-d6883ffa-b6ba-4805-876c-923d32308570'}
        };
        const queryParams = `token_id=${idTokens}`;
        const url = `https://api.tokenmetrics.com/v2/price?${queryParams}`;


        try {
            const response = await axios.get(url, options);
            
            
            //Add CURRENT_PRICE to matching rows
        for (const row of rows) 
        {
            
            if (row.cantidad === undefined || row.cantidad === null) 
            {
                row.cantidad = 0; // Default value if cantidad is undefined or null
            }
            if (row.tokenId === 0)
            {
                row.precio = 1;

            }
            else
            {
                
                const tokenData = response.data.data.find(token => token.TOKEN_ID === row.tokenId);
                if (tokenData) 
                {
                    row.precio = tokenData.CURRENT_PRICE;
                } 
                else 
                {
                    console.log(`Token ID ${row.tokenId} not found in API response.`);
                    row.precio = 0; 
                }
            }
        }

            res.send(rows); 
        } catch (axiosError) {
            console.error("Error fetching data from Token Metrics API:", axiosError);
            res.status(500).send({ error: "Error fetching data from external API", message: axiosError.message });
        }
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

app.post('/trading/registerTransaction/:userId', async (req, res) => 
    {
    let userId = req.params.userId;
    let {cryptoSold, cryptoBought, amountSold, amountBought} = req.body;
    let connection;
    try {
        connection = await dbConnect();
        console.log("Conexion exitosa");
        console.log("Transaccion: ", req.body);
        const query = `CALL registerTrade(${userId}, ${cryptoSold}, ${amountSold}, ${cryptoBought}, ${amountBought});`;
        console.log("Query: ", query);
        await connection.execute('CALL registerTrade(?, ?, ?, ?, ?);', [userId, cryptoSold, amountSold, cryptoBought, amountBought]);
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


// Endpoint para Obtener Contratos Aleatorios
app.get('/smartcontracts/random', async (req, res) => {
    let connection;
    try {
        connection = await dbConnect();
        const [rows] = await connection.query(
            'SELECT idSmartContract, descripcion, JSON_UNQUOTE(JSON_EXTRACT(condicion, "$")) AS condicion, idRecompensa FROM smartcontract ORDER BY RAND() LIMIT 3'
        );
        res.send(rows); // Devuelve los contratos aleatorios
    } catch (err) {
        console.error("Error en /smartcontracts/random:", err); // Registra el error completo
        res.status(500).send({ error: err.name, message: err.message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});

//Endpoint para Procesar Recompensas
app.get('/reward/:rewardId/:userId', async (req, res) => {
    const { rewardId, userId } = req.params;
    let connection;
    try {
        connection = await dbConnect();
        const [reward] = await connection.query(
            'SELECT tipo, referencia, cantidad FROM recompensa WHERE idRecompensa = ?',
            [rewardId]
        );

        if (reward.length === 0) {
            res.status(404).send({ error: "Recompensa no encontrada" });
            return;
        }

        const { tipo, referencia, cantidad } = reward[0];

        if (tipo === "PowerUp") {
            // Desbloquear PowerUp
            await connection.query(
                'INSERT INTO powerupdesbloqueado (idPowerUp, idUsuario, activado, idSmartContract) VALUES (?, ?, 0, ?)',
                [referencia, userId, rewardId]
            );
            res.send({ success: true, message: "PowerUp desbloqueado" });
        } else if (tipo === "Tkns") {
            // Añadir Tkns al Wallet
            await connection.query(
                'UPDATE wallet SET cantidad = cantidad + ? WHERE idUsuario = ? AND idCriptomoneda = 1',
                [cantidad, userId]
            );
            res.send({ success: true, message: `${cantidad} Tkns añadidos al Wallet` });
        } else {
            res.status(400).send({ error: "Tipo de recompensa desconocido" });
        }
    } catch (err) {
        const { name, message } = err;
        console.error("Error en /reward:", message);
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});

app.get('/reward/:rewardId/:userId', async (req, res) => {
    const rewardId = req.params.rewardId;
    const userId = req.params.userId;
    let connection;

    try {
        connection = await dbConnect();
        await connection.query(
            'INSERT INTO recompensas (idRecompensa, idUsuario) VALUES (?, ?)', 
            [rewardId, userId]
        );
        res.send({ success: true, message: "Recompensa asignada correctamente" });
    } catch (err) {
        const { name, message } = err;
        console.error("Error al asignar recompensa:", message);
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});

// Endpoint para verificar las condiciones de los contratos
app.get('/smartcontracts/check/:userId', async (req, res) => {
    const userId = req.params.userId;
    let connection;
    try {
        connection = await dbConnect();
        const [contracts] = await connection.query(
            'SELECT sc.idSmartContract, sc.descripcion, sc.condicion, sc.idRecompensa FROM smartcontract sc ' +
            'LEFT JOIN smartcontractcumplido scc ON sc.idSmartContract = scc.idSmartContract AND scc.idUsuario = ? ' +
            'WHERE scc.idSmartContract IS NULL', 
            [userId]
        );

        const contractStatuses = [];
        for (const contract of contracts) {
            const condition = JSON.parse(contract.condicion); // Parsear la condición como JSON
            contractStatuses.push({
                idSmartContract: contract.idSmartContract,
                descripcion: contract.descripcion,
                condicion: condition, // Incluir la condición como JSON
                idRecompensa: contract.idRecompensa
            });
        }

        res.send(contractStatuses); // Devolver los contratos con sus condiciones
    } catch (err) {
        const { name, message } = err;
        console.error("Error en /smartcontracts/check:", message);
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});

app.post('/smartcontracts/registerCompleted/:idSmartContract', async (req, res) => {
    const idSmartContract = req.params.idSmartContract;
    const userId = req.body.userId; // Asegúrate de enviar el userId en el cuerpo de la solicitud
    let connection;

    try {
        connection = await dbConnect();
        await connection.query(
            'INSERT INTO smartcontractcumplido (idSmartContract, idUsuario) VALUES (?, ?)', 
            [idSmartContract, userId]
        );
        res.send({ success: true, message: "Contrato registrado como cumplido" });
    } catch (err) {
        const { name, message } = err;
        console.error("Error al registrar contrato cumplido:", message);
        res.status(500).send({ error: name, message });
    } finally {
        if (connection) {
            connection.end();
        }
    }
});

app.use((req, res) => {
    const url = req.originalUrl;
    res.status(404).send({ error: "Endpoint no encontrado", url });
});

app.listen(port, () => {
    console.log(`Servidor esperando en: http://${ipAddress}:${port}`);
});


