-- Esquema de la base de datos para el Dashboard del Videojuego Educativo

DROP DATABASE IF EXISTS cryptopia_db;

CREATE DATABASE cryptopia_db;

USE cryptopia_db;

-- Tabla de usuarios
CREATE TABLE users (
  id INT NOT NULL AUTO_INCREMENT,
  username VARCHAR(50) NOT NULL,
  full_name VARCHAR(100),
  email VARCHAR(100),
  age INT,
  country VARCHAR(50),
  registration_date DATE,
  last_login DATETIME,
  PRIMARY KEY (id)
);

-- Tabla de progreso de usuarios
CREATE TABLE user_progress (
  id INT NOT NULL AUTO_INCREMENT,
  user_id INT NOT NULL,
  course_id INT NOT NULL,
  lesson_id INT NOT NULL,
  completed BOOLEAN DEFAULT FALSE,
  score INT,
  time_spent INT, -- en segundos
  completed_at DATETIME,
  PRIMARY KEY (id)
);

-- Tabla de cursos
CREATE TABLE courses (
  id INT NOT NULL AUTO_INCREMENT,
  title VARCHAR(100) NOT NULL,
  description TEXT,
  difficulty VARCHAR(20),
  category VARCHAR(50),
  total_lessons INT,
  PRIMARY KEY (id)
);

-- Tabla de lecciones
CREATE TABLE lessons (
  id INT NOT NULL AUTO_INCREMENT,
  course_id INT NOT NULL,
  title VARCHAR(100) NOT NULL,
  sequence_order INT NOT NULL,
  type VARCHAR(50),
  points INT,
  PRIMARY KEY (id)
);

-- Tabla de sesiones de usuario
CREATE TABLE user_sessions (
  id INT NOT NULL AUTO_INCREMENT,
  user_id INT NOT NULL,
  start_time DATETIME NOT NULL,
  end_time DATETIME,
  device VARCHAR(50),
  platform VARCHAR(50),
  PRIMARY KEY (id)
);

-- Insertar algunos datos de ejemplo para pruebas
INSERT INTO courses (title, description, difficulty, category, total_lessons) VALUES
  ('Matemáticas Básicas', 'Fundamentos de aritmética y álgebra', 'Principiante', 'Matemáticas', 20),
  ('Ciencias Naturales', 'Introducción a la biología, química y física', 'Intermedio', 'Ciencias', 25),
  ('Historia Mundial', 'Eventos históricos importantes', 'Intermedio', 'Humanidades', 30),
  ('Programación para Niños', 'Conceptos básicos de programación', 'Principiante', 'Tecnología', 15);