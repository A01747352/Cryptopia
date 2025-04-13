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

async function loginVerification(credentials) {
    let user = credentials.user;
    let password = credentials.password;
    let connection;

    try {
        connection = await mysql.createConnection({
            host: 'localhost',
            user: process.env.LOCALHOST_MYSQL_USER,
            password: process.env.LOCALHOST_MYSQL_PASSWORD,
            database: 'Cryptopia',
        });
        const [rows] = await connection.query('SELECT contrasena FROM Usuario WHERE id = ?;', [user]);
        const correctPassword = rows.length > 0 ? rows[0].contrasena : null;

        if (correctPassword === password) {
            logInTimestamp = new Date();
            return { result: "Correct Login" };
        }

        else if (contrasenaValida !== null) {
            return { result: "Invalid Password" };
        }
        else {
            return { result: "Invalid Credentials" };
        }

    } catch (err) {
        console.error("Error al acceder a la base datos:", err);
        return { result: "errorServidor" };
    } finally {
        if (connection) {
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
        console.log(req.body);
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
app.use((req, res) => {
    const url = req.originalUrl;
    res.status(404).render('not_found', { url });
  });
  
  app.listen(port, () => {
    console.log(`Servidor esperando en: http://${ ipAddress }:${ port }`);
  });

  // Login and Register Requests
app.post("/login", async (req, res) => {
    try {
        console.log("req.body:", req.body);
        const credentials = req.body;
        let resultAutentication = await loginVerification(credentials);

        res.setHeader("Content-Type", "application/json");
        res.json(resultAutentication);
    } catch (error) {
        console.error("Error en el endpoint de autenticación:", error);
        res.status(500).json({ result: "errorServidor" });
    }
});



app.post('/register', async (req, res) =>
{
    let {user, password} = req.body;
    let connection;
    try
    {
        connection = await dbConnect();
        const [rows] = await connection.query('CALL Register(?, ?);', [user, password]);
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
