
CREATE DATABASE Cryptopia;
USE Cryptopia;
CREATE TABLE Usuario (
Id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
username VARCHAR(25) UNIQUE NOT NULL,
nombre VARCHAR(25) NOT NULL,
apellido VARCHAR(25) NOT NULL,
contrasena VARCHAR(25) NOT NULL,
nacimiento DATE NOT NULL,
genero CHAR(1) CHECK (genero IN ('M', 'F', 'O')),
fechaRegistro TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
-- B) Minijuego
CREATE TABLE Minijuego (
idMinijuego INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
nombre VARCHAR(25) UNIQUE NOT NULL,
modulo VARCHAR(25) NOT NULL
);
-- E) Recompensa (creada antes para referencias)
CREATE TABLE Recompensa (
idRecompensa INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
tipo VARCHAR(25) NOT NULL,
referencia INT NULL,
cantidad DOUBLE NOT NULL
);
-- C) SmartContract
CREATE TABLE SmartContract (
idSmartContract INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
nombre VARCHAR(25) NOT NULL,
condicion JSON NOT NULL,
idRecompensa INT NOT NULL,
repetible BOOLEAN NOT NULL DEFAULT 0,
FOREIGN KEY (idRecompensa) REFERENCES Recompensa(idRecompensa)
);
-- D) SmartContractCumplido
CREATE TABLE SmartContractCumplido (
idSmartContract INT NOT NULL,
idUsuario INT NOT NULL,
fecha TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
PRIMARY KEY (idSmartContract, idUsuario),
FOREIGN KEY (idSmartContract) REFERENCES SmartContract(idSmartContract),
FOREIGN KEY (idUsuario) REFERENCES Usuario(ID)
);
-- F) PartidaTrivia
CREATE TABLE PartidaTrivia (
idPartida INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
resultado ENUM('victoria', 'derrota') NOT NULL,
porcentajeAciertos DECIMAL(3,2) NOT NULL CHECK (porcentajeAciertos
BETWEEN 0 AND 100),
puntaje INT NOT NULL DEFAULT 0,
fechaPartida TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
TKNs DOUBLE NOT NULL DEFAULT 0,
idSmartContract INT NULL,
idMinijuego INT NOT NULL,
FOREIGN KEY (idSmartContract) REFERENCES SmartContract(idSmartContract),
FOREIGN KEY (idMinijuego) REFERENCES Minijuego(idMinijuego)
);
-- G) PreguntaTrivia
CREATE TABLE PreguntaTrivia (
idPregunta INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
pregunta TEXT NOT NULL,
puntos INT NOT NULL DEFAULT 10
);
-- H) RespuestasTrivia
CREATE TABLE RespuestasTrivia (
idRespuesta INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
respuesta TEXT NOT NULL,
retroalimentacion TEXT NOT NULL,
resultado BOOLEAN NOT NULL,
idPregunta INT NOT NULL,
FOREIGN KEY (idPregunta) REFERENCES PreguntaTrivia(idPregunta)
);
-- I) RespuestaUsuarioTrivia
CREATE TABLE RespuestaUsuarioTrivia (
idRespuestaUsuario INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
idRespuesta INT NOT NULL,
idPartida INT NOT NULL,
FOREIGN KEY (idRespuesta) REFERENCES RespuestasTrivia(idRespuesta),
FOREIGN KEY (idPartida) REFERENCES PartidaTrivia(idPartida)
);
-- J) PartidaCriptografia
CREATE TABLE PartidaCriptografia (
idPartida INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
aciertos INT NOT NULL DEFAULT 0,
errores INT NOT NULL DEFAULT 0,
puntaje INT NOT NULL DEFAULT 0,
TKNs DOUBLE NOT NULL DEFAULT 0,
resultado BOOLEAN NOT NULL,
idMinijuego INT NOT NULL,
idUsuario INT NOT NULL,
FOREIGN KEY (idMinijuego) REFERENCES Minijuego(idMinijuego),
FOREIGN KEY (idUsuario) REFERENCES Usuario(Id)
);
-- K) PalabraCriptografia
CREATE TABLE PalabraCriptografia (
idPalabra INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
palabra VARCHAR(25) NOT NULL,
respuestaCorrecta VARCHAR(25) NOT NULL,
encriptacion VARCHAR(25) NOT NULL,
puntos INT NOT NULL DEFAULT 10,
idMinijuego INT NOT NULL,
FOREIGN KEY (idMinijuego) REFERENCES Minijuego(idMinijuego)
);
-- L) RespuestaUsuarioCriptografia
CREATE TABLE RespuestaUsuarioCriptografia (
idRespuestaUsuario INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
respuestaUsuario VARCHAR(25) NOT NULL,
idPalabra INT NOT NULL,
idPartida INT NOT NULL,
FOREIGN KEY (idPalabra) REFERENCES PalabraCriptografia(idPalabra),
FOREIGN KEY (idPartida) REFERENCES PartidaCriptografia(idPartida)
);
-- M) SesionCryptoMine
CREATE TABLE SesionCryptoMine (
idUsuario INT NOT NULL,
TKNs DOUBLE NOT NULL DEFAULT 0,
bloquesMinados INT NOT NULL DEFAULT 0,
clicks INT NOT NULL DEFAULT 0,
puntaje INT NOT NULL DEFAULT 0,
inicioSesion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
terminaSesion TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
idMinijuego INT NOT NULL,
PRIMARY KEY (idUsuario),
FOREIGN KEY (idUsuario) REFERENCES Usuario(ID),
FOREIGN KEY (idMinijuego) REFERENCES Minijuego(idMinijuego)
);
-- N) PowerUp
CREATE TABLE PowerUp (
idPowerUp INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
nombre VARCHAR(25) UNIQUE NOT NULL,
descripcion TEXT NOT NULL
);
-- O) PowerUpDesbloqueado
CREATE TABLE PowerUpDesbloqueado (
idPowerUp INT NOT NULL,
idUsuario INT NOT NULL,
activado BOOLEAN NOT NULL DEFAULT 0,
idSmartContract INT NOT NULL,
PRIMARY KEY (idPowerUp, idUsuario),
FOREIGN KEY (idPowerUp) REFERENCES PowerUp(idPowerUp),
FOREIGN KEY (idUsuario) REFERENCES Usuario(ID),
FOREIGN KEY (idSmartContract) REFERENCES SmartContract(idSmartContract)
);
-- P) CryptoRig
CREATE TABLE CryptoRig (
idRig INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
nombre VARCHAR(25) UNIQUE NOT NULL,
descripcion TEXT NOT NULL,
costo DOUBLE NOT NULL
);
-- Q) CryptoRigUsuario
CREATE TABLE CryptoRigUsuario (
idRig INT NOT NULL,
idUsuario INT NOT NULL,
estado VARCHAR(25) NOT NULL DEFAULT 'comprado',
activado BOOLEAN NOT NULL DEFAULT 0,
PRIMARY KEY (idRig, idUsuario),
FOREIGN KEY (idRig) REFERENCES CryptoRig(idRig),
FOREIGN KEY (idUsuario) REFERENCES Usuario(ID)
);
-- R) Criptomoneda
CREATE TABLE Criptomoneda (
idCriptomoneda INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
nombre VARCHAR(25) UNIQUE NOT NULL,
abreviatura VARCHAR(4) UNIQUE NOT NULL
);
-- S) Trading
CREATE TABLE Trading (
idTrade INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
idUsuario INT NOT NULL,
cryptoIntercambiado INT NOT NULL,
cryptoRecibido INT NOT NULL,
cantidadIntercambiada DOUBLE NOT NULL CHECK (cantidadIntercambiada > 0),
cantidadRecibida DOUBLE NOT NULL CHECK (cantidadRecibida > 0),
fecha TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
FOREIGN KEY (idUsuario) REFERENCES Usuario(Id),
FOREIGN KEY (cryptoIntercambiado) REFERENCES
Criptomoneda(idCriptomoneda),
FOREIGN KEY (cryptoRecibido) REFERENCES Criptomoneda(idCriptomoneda)
);

-- S) Wallet
DROP TABLE Wallet;
CREATE TABLE Wallet (
idUsuario INT NOT NULL,
idCriptomoneda INT NOT NULL,
cantidad DECIMAL(10,2) NOT NULL,
PRIMARY KEY (idUsuario, idCriptomoneda),
FOREIGN KEY (idUsuario) REFERENCES Usuario(Id),
FOREIGN KEY (idCriptomoneda) REFERENCES Criptomoneda(idCriptomoneda)
);


DELIMITER $$
CREATE PROCEDURE RegistrateCryptographyGame(IN userId INT)
BEGIN
    DECLARE idMinigame INT;
    DECLARE idGame INT;
    
   DECLARE EXIT HANDLER FOR SQLEXCEPTION
   BEGIN
		ROLLBACK;
		RESIGNAL SQLSTATE '50000' SET MESSAGE_TEXT = 'Se produjo un error con la transaccion';
	END;
    
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
    START TRANSACTION;
		SELECT idMinijuego INTO idMinigame FROM Minijuego WHERE nombre = 'Cryptography';
        
        INSERT INTO PartidaCriptografia(aciertos, errores, puntaje, TKNs, resultado, idMinijuego, idUsuario) 
        VALUES (0, 0, 0, 0, FALSE, idMinigame, userId);
        
        SET idGame = LAST_INSERT_ID();
    COMMIT;
    
    SELECT idGame;
END $$
DELIMITER ;

USE Cryptopia;
INSERT INTO Minijuego(nombre, modulo) VALUES ("Cryptography", "Criptografia");
INSERT INTO PalabraCriptografia(palabra, respuestaCorrecta, encriptacion, puntos, idMinijuego) VALUES ("4dr322", "address", "leetspeak", 100, 1);
UPDATE PalabraCriptografia SET palabra = "4ddr322" WHERE idPalabra = 1;
ALTER TABLE PartidaTrivia ADD idUsuario INT, ADD FOREIGN KEY (idUsuario) REFERENCES Usuario(Id);
ALTER TABLE Usuario ADD pais VARCHAR(50), ADD edad INT, ADD ocupacion VARCHAR(20);
INSERT INTO Usuario(username, nombre, apellido, contrasena, nacimiento, genero) VALUES("El_mas_capito", "Emilio", "De León", "elmascapito", "2004/05/12", "o");
INSERT INTO Criptomoneda(nombre, abreviatura) VALUES ("Token", "TKN");
INSERT INTO Wallet(idUsuario, idCriptomoneda, cantidad) VALUES (1, 1, 0);
UPDATE Wallet SET cantidad = cantidad + 1.5 WHERE idUsuario = 1 AND idCriptomoneda = 1;

CREATE VIEW UserCryptographyProgress AS
SELECT 
  u.Id,
  u.username,
  COUNT(pc.idPartida) AS partidasJugadas,
  SUM(pc.aciertos) AS totalAciertos,
  SUM(pc.errores) AS totalErrores,
  SUM(pc.puntaje) AS puntajeTotal
FROM Usuario u
JOIN PartidaCriptografia pc ON pc.idUsuario = u.Id
GROUP BY u.Id;
DROP VIEW userprogress;
CREATE VIEW UserProgress AS
SELECT 
  u.Id,
  u.username,
  IFNULL(pc.partidas, 0) + IFNULL(pt.partidas, 0) AS partidasJugadas,
  IFNULL(pc.puntaje, 0) + IFNULL(pt.puntaje, 0) + IFNULL(scm.puntaje, 0) AS puntajeTotal,
  IFNULL(pc.TKNs, 0) + IFNULL(pt.TKNs, 0) + IFNULL(scm.TKNs, 0) AS TotalEarnedTKNs
FROM Usuario u
LEFT JOIN (
    SELECT idUsuario, COUNT(*) AS partidas, SUM(puntaje) AS puntaje, SUM(TKNs) AS TKNs
    FROM PartidaCriptografia
    GROUP BY idUsuario
) pc ON pc.idUsuario = u.Id
LEFT JOIN (
    SELECT idUsuario, COUNT(*) AS partidas, SUM(puntaje) AS puntaje, SUM(TKNs) AS TKNs
    FROM PartidaTrivia
    GROUP BY idUsuario
) pt ON pt.idUsuario = u.Id
LEFT JOIN (
    SELECT idUsuario, SUM(puntaje) AS puntaje, SUM(TKNs) AS TKNs
    FROM SesionCryptoMine
    GROUP BY idUsuario
) scm ON scm.idUsuario = u.Id;

