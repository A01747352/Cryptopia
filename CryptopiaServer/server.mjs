import express from 'express';
import mysql from 'mysql2/promise';


const app = express();
const port = 8080;
const ipAddress = 'localhost'; 

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



// Player Progress



// Minigame Cryptography Requests


app.get('/cryptography/newGame', async (req, res) => 
{

    let connection;
    try
    {
        connection = await dbConnect();
        const [rows] = await connection.query('CALL RegistrateCryptographyGame();');
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

app.use((req, res) => {
    const url = req.originalUrl;
    res.status(404).render('not_found', { url });
  });
  
  app.listen(port, () => {
    console.log(`Servidor esperando en: http://${ ipAddress }:${ port }`);
  });






