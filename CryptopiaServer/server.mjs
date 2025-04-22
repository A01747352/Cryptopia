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

async function dbConnect()
{
    return await mysql.createConnection({
        host: 'localhost',
        user: process.env.LOCALHOST_MYSQL_USER,
        password: process.env.LOCALHOST_MYSQL_PASSWORD,
        database: 'Cryptopia',
        multipleStatements: true
    });
}

// Login Verification
async function loginVerification(user,password){
    let connection;
    try {
        connection = await mysql.createConnection({
            host: 'localhost',
            user: process.env.LOCALHOST_MYSQL_USER,
            password: process.env.LOCALHOST_MYSQL_PASSWORD,
            database: 'Cryptopia',
        });
        const [rows] = await connection.query('SELECT contrasena FROM Usuario WHERE username = ?;', [user]);
        const correctPassword = rows[0].contrasena;

        if (rows.length > 0) {
            if (correctPassword === password) {
                return {result: "True"}; 
            } 
            else {
                return {result: "False"};
            }
        }

    } catch (err){
        console.error("Error al acceder a la base datos:", err);
        return { result: "errorServidor" };
    } finally {
        if (connection){
            await connection.end();
        }
    }
}




// Player Progress



// Minigame Cryptography Requests

app.get('/cryptography/newGame/:userId', async (req, res) => 
{
    let userId = req.params.userId;
    let connection;
    try
    {
        connection = await dbConnect();
        const [rows] = await connection.query('CALL RegistrateCryptographyGame(?);', [userId]);
        const response = rows[0][0];
        res.send(response);
    }
    catch (err)
    {
        const {name, message} = err;
        res.status(500).send({error: name, message});
    }
    finally 
    {
        if (connection)
        {
            connection.end();
        }
    }
});

app.get('/cryptography/loadEncryption', async (req, res) =>
{
    let connection;
    try
    {
        connection = await dbConnect();
        const [rows] = await connection.execute('SELECT * FROM PalabraCriptografia ORDER BY RAND() LIMIT 1');
        const response = JSON.stringify(rows);
        res.send(response);
    }
    catch (err)
    {
        const {name, message} = err;
        res.status(500).send({error: name, message});
    }
    finally 
    {
        if (connection)
        {
            connection.end();
        }
    }
});
app.post('/cryptography/submitAnswer', async (req, res) => 
    {
        let {respuestaUsuario, idPalabra, idPartida} = req.body;
        let connection;
        try
        {
            connection = await dbConnect();
            await connection.query('INSERT INTO RespuestaUsuarioCriptografia(respuestaUsuario, idPalabra, idPartida) VALUES(?, ?, ?);', [respuestaUsuario, idPalabra, idPartida]);
            res.send({ success: true});
        }
        catch (err)
        {
            const {name, message} = err;
            res.status(500).send({error: name, message});
        }
        finally 
        {
            if (connection)
            {
                connection.end();
            }
        }


    });

app.post('/cryptography/saveGame', async (req, res) =>
{
    let {idPartida, aciertos, errores, puntaje, TKNs, resultado, idMinijuego, idUsuario} = req.body;
    let connection;
    try
    {
        connection = await dbConnect();
        await connection.execute('UPDATE PartidaCriptografia SET aciertos = ?, errores = ?, puntaje = ?, TKNs = ?, resultado = ?, idMinijuego = ? WHERE idPartida = ?', [aciertos, errores, puntaje, TKNs, resultado, idMinijuego, idPartida]);
        await connection.execute('UPDATE Wallet SET cantidad = cantidad + ? WHERE idUsuario = ? AND idCriptomoneda = 1', [TKNs, idUsuario]);
        res.send({success: true});
    }
    catch (err)
        {
            const {name, message} = err;
            res.status(500).send({error: name, message});
        }
        finally 
        {
            if (connection)
            {
                connection.end();
            }
        }
});


  // Login and Register Requests
app.post("/login", async (req, res) => 
{
    let {user, password} = req.body;
    try {
        const result = await loginVerification(user, password);
        res.status(200).json(result);

    } catch (error) {
        console.error("Error en el endpoint de autenticación:", error);
        res.status(500).json({ result: "errorServidor" });
    }
});

// Endpoint para registrar un nuevo juego de trivia
app.get('/trivia/newGame/:userId', async (req, res) => 
    {
        let userId = req.params.userId;
        let connection;
        try
        {
            connection = await dbConnect();
            const [rows] = await connection.query('CALL RegistrateTriviagame(?);', [userId]);
            const response = rows[0][0];
            res.send(response);
        }
        catch (err)
        {
            const {name, message} = err;
            res.status(500).send({error: name, message});
        }
        finally 
        {
            if (connection)
            {
                connection.end();
            }
        }
    });

    // Endpoint para obtener todas las preguntas con sus respuestas
app.get('/trivia/getAllQuestions', async (req, res) => 
    {
        let connection;
        try
        {
            connection = await dbConnect();
            
            // Obtenemos todas las preguntas de trivia
            const [questions] = await connection.execute('SELECT * FROM PreguntaTrivia');
            
            if (questions.length === 0) {
                res.status(404).send({error: "No se encontraron preguntas"});
                return;
            }
            
            // Preparamos un array para almacenar cada pregunta con sus respuestas
            const questionsWithAnswers = [];
            
            // Para cada pregunta, obtenemos sus respuestas
            for (const question of questions) {
                const [answers] = await connection.execute(
                    'SELECT * FROM RespuestasTrivia WHERE idPregunta = ?', 
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
        }
        catch (err)
        {
            const {name, message} = err;
            res.status(500).send({error: name, message});
        }
        finally 
        {
            if (connection)
            {
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
app.post('/trivia/submitAnswer', async (req, res) => 
    {
        let {respuestaUsuario, idPregunta, esCorrecta, idPartida} = req.body;
        let connection;
        try
        {
            connection = await dbConnect();
            await connection.query(
                'INSERT INTO RespuestaUsuarioTrivia(respuestaUsuario, idPregunta, esCorrecta, idPartida) VALUES(?, ?, ?, ?);', 
                [respuestaUsuario, idPregunta, esCorrecta, idPartida]
            );
            res.send({ success: true});
        }
        catch (err)
        {
            const {name, message} = err;
            res.status(500).send({error: name, message});
        }
        finally 
        {
            if (connection)
            {
                connection.end();
            }
        }
    });


    // Endpoint para guardar resultados del juego
app.post('/trivia/saveGame', async (req, res) =>
    {
        let {idPartida, resultado, porcentajeAciertos, puntaje, TKNs, idMinijuego, idUsuario} = req.body;
        let connection;
        try
        {
            connection = await dbConnect();
            
            // Actualizar la partida de trivia
            await connection.execute(
                'UPDATE PartidaTrivia SET resultado = ?, porcentajeAciertos = ?, puntaje = ?, TKNs = ? WHERE idPartida = ?', 
                [resultado, porcentajeAciertos, puntaje, TKNs, idPartida]
            );
            
            // Añadir TKNs al wallet del usuario
            await connection.execute(
                'UPDATE Wallet SET cantidad = cantidad + ? WHERE idUsuario = ? AND idCriptomoneda = 1', 
                [TKNs, idUsuario]
            );
            
            res.send({success: true});
        }
        catch (err)
        {
            const {name, message} = err;
            res.status(500).send({error: name, message});
        }
        finally 
        {
            if (connection)
            {
                connection.end();
            }
        }
    });


// Minigame CryptoMine Requests 

app.get('/cryptomine/retrieveUserPowerUps/:userId', async (req, res) =>
{
    let connection;
    let userId = req.params.userId;
    try 
    {
        connection = await dbConnect();
        const [rows] = await connection.execute('SELECT pu.nombre FROM PowerUpDesbloqueado AS pud LEFT JOIN PowerUp AS pu ON pud.idPowerUp = pu.idPowerUp WHERE pud.idUsuario = ?;', [userId]);
        const response = rows.map(row => row.nombre).join('-');
        res.send(response);
    }
    catch (err)
    {
        const {name, message} = err;
        res.status(500).send({error: name, message});
    }
    finally 
    {
        if (connection)
        {
            connection.end();
        }
    }
});

app.post('/cryptomine/saveSession/:userId', async (req, res) =>
{
    let userId = req.params.userId;
    let {TKNs, startSession, endSession, minedBlocks, clicks, score} = req.body;
    let connection;
    try
    {
        connection = await dbConnect();
        console.log("Conexion exitosa");
        await connection.execute('INSERT INTO SesionCryptoMine(idUsuario, TKNs, bloquesMinados, clicks, puntaje, inicioSesion, terminaSesion, idMinijuego) VALUES(?, ?, ?, ?, ?, ?, ?, 3);', [userId, TKNs, minedBlocks, clicks, score, startSession, endSession]);
        console.log("Consulta exitosa");
        await connection.execute('UPDATE Wallet SET cantidad = cantidad + ? WHERE idUsuario = ? AND idCriptomoneda = 1', [TKNs, userId]);
        res.send({success: true});
    }
    catch (err)
    {
        const {name, message} = err;
        res.status(500).send({error: name, message});
    }
    finally 
    {
        if (connection)
        {
            connection.end();
        }
    }
});

app.get('/cryptomine/loadUserData/:userId', async (req, res) =>
{
    let userId = req.params.userId;
    let connection;
    try
    {
        connection = await dbConnect();
        const [rows] = await connection.execute('SELECT cantidad FROM Wallet WHERE idUsuario = ? AND idCriptomoneda = 1;', [userId]);
        const TKNs = rows[0].cantidad || 0;
        const [rows2] = await connection.execute('SELECT SUM(bloquesMinados) AS totalBloquesMinados FROM SesionCryptoMine WHERE idUsuario = ?;', [userId]);
        const totalBloquesMinados = rows2[0].totalBloquesMinados || 0;
        const response = {
            TKNs: TKNs,
            totalBloquesMinados: totalBloquesMinados
        };
        res.send(response);
    }
    catch (err)
    {
        const {name, message} = err;
        res.status(500).send({error: name, message});
    }
    finally 
    {
        if (connection)
        {
            connection.end();
        }
    }
});

app.get('/trading/retrieveCryptoPrices', async (req, res) =>
{
    try {
        const response = await axios.get('https://api.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum,litecoin&vs_currencies=usd');
        res.json(response.data);
        console.log("Precios de criptomonedas obtenidos:", response.data);
    } catch (error) {
        console.error("Error al obtener precios de criptomonedas:", error);
        res.status(500).json({ error: "Error al obtener precios de criptomonedas" });
    }
});


app.use((req, res) => {
    const url = req.originalUrl;
    res.status(404).json({ error: `No se encontró la URL: ${url}` });
  });
  
  app.listen(port, () => {
    console.log(`Servidor esperando en: http://${ ipAddress }:${ port }`);
  });
