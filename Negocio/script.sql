USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BIBLIOTECA_DB')
BEGIN
    CREATE DATABASE BIBLIOTECA_DB;
END
GO

USE BIBLIOTECA_DB;
GO


CREATE TABLE AUTORES (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Biografia VARCHAR(MAX) NULL
);

CREATE TABLE CATEGORIAS (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Descripcion VARCHAR(100) NOT NULL
);

CREATE TABLE EDITORIALES (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL
);

CREATE TABLE LIBROS (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo VARCHAR(200) NOT NULL,
    ISBN VARCHAR(50) NOT NULL,
    IdEditorial INT NOT NULL,
    AnioEdicion INT NOT NULL,
    CONSTRAINT FK_LIBROS_EDITORIALES FOREIGN KEY (IdEditorial) REFERENCES EDITORIALES(Id)
);

-- Tabla intermedia: Un libro puede tener uno o más autores
CREATE TABLE LIBROS_AUTORES (
    IdLibro INT NOT NULL,
    IdAutor INT NOT NULL,
    CONSTRAINT PK_LIBROS_AUTORES PRIMARY KEY (IdLibro, IdAutor),
    CONSTRAINT FK_LA_LIBROS FOREIGN KEY (IdLibro) REFERENCES LIBROS(Id),
    CONSTRAINT FK_LA_AUTORES FOREIGN KEY (IdAutor) REFERENCES AUTORES(Id)
);

-- Tabla intermedia: Un libro puede pertenecer a una o más categorías
CREATE TABLE LIBROS_CATEGORIAS (
    IdLibro INT NOT NULL,
    IdCategoria INT NOT NULL,
    CONSTRAINT PK_LIBROS_CATEGORIAS PRIMARY KEY (IdLibro, IdCategoria),
    CONSTRAINT FK_LC_LIBROS FOREIGN KEY (IdLibro) REFERENCES LIBROS(Id),
    CONSTRAINT FK_LC_CATEGORIAS FOREIGN KEY (IdCategoria) REFERENCES CATEGORIAS(Id)
);

CREATE TABLE EJEMPLARES (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IdLibro INT NOT NULL,
    CodigoInventario VARCHAR(50) NOT NULL UNIQUE,
    Estado VARCHAR(50) NOT NULL DEFAULT 'Disponible', -- Disponible, Prestado, Reservado, En reparación, Extraviado
    Observaciones VARCHAR(500) NULL,
    CONSTRAINT FK_EJEMPLARES_LIBROS FOREIGN KEY (IdLibro) REFERENCES LIBROS(Id)
);


CREATE TABLE POLITICAS_PRESTAMO (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Descripcion VARCHAR(100) NOT NULL, -- Ej: General, Estudiante, Docente
    DiasDuracion INT NOT NULL,
    CantidadMaximaEjemplares INT NOT NULL,
    LimiteRenovaciones INT NOT NULL
);

CREATE TABLE SOCIOS (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    DNI VARCHAR(20) NOT NULL UNIQUE,
    Direccion VARCHAR(200) NULL,
    Telefono VARCHAR(50) NULL,
    Email VARCHAR(150) NOT NULL,
    Habilitado BIT NOT NULL DEFAULT 1,
    IdPoliticaPrestamo INT NOT NULL,
    CONSTRAINT FK_SOCIOS_POLITICAS FOREIGN KEY (IdPoliticaPrestamo) REFERENCES POLITICAS_PRESTAMO(Id)
);


CREATE TABLE PRESTAMOS (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NumeroComprobante VARCHAR(50) NOT NULL UNIQUE,
    IdSocio INT NOT NULL,
    IdEjemplar INT NOT NULL,
    FechaInicio DATETIME NOT NULL DEFAULT GETDATE(),
    FechaPrevistaDevolucion DATETIME NOT NULL,
    FechaDevolucionReal DATETIME NULL,
    CantidadRenovaciones INT NOT NULL DEFAULT 0,
    Estado VARCHAR(50) NOT NULL DEFAULT 'Activo', -- Activo, Renovado, Devuelto a término, Devuelto con retraso, Vencido, Extraviado
    ObservacionesDevolucion VARCHAR(500) NULL,
    CONSTRAINT FK_PRESTAMOS_SOCIOS FOREIGN KEY (IdSocio) REFERENCES SOCIOS(Id),
    CONSTRAINT FK_PRESTAMOS_EJEMPLARES FOREIGN KEY (IdEjemplar) REFERENCES EJEMPLARES(Id)
);


CREATE TABLE USUARIOS (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    Clave VARCHAR(100) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    Perfil VARCHAR(50) NOT NULL, -- Administrador, Bibliotecario
    Activo BIT NOT NULL DEFAULT 1
);
GO


-- Usuarios del sistema
INSERT INTO USUARIOS (NombreUsuario, Clave, Nombre, Apellido, Email, Perfil, Activo) VALUES
('admin', 'admin123', 'Carlos', 'Gómez', 'admin@biblioteca.com', 'Administrador', 1),
('biblio', 'biblio123', 'Laura', 'Martínez', 'mostrador@biblioteca.com', 'Bibliotecario', 1);

-- Políticas de préstamo
INSERT INTO POLITICAS_PRESTAMO (Descripcion, DiasDuracion, CantidadMaximaEjemplares, LimiteRenovaciones) VALUES
('Socio General', 7, 2, 1),
('Estudiante Universitario', 14, 3, 2),
('Investigador / Docente', 30, 5, 3);

-- Socios
INSERT INTO SOCIOS (Nombre, Apellido, DNI, Direccion, Telefono, Email, Habilitado, IdPoliticaPrestamo) VALUES
('Juan', 'Pérez', '35111222', 'Av. Rivadavia 1234', '1144556677', 'juan.perez@email.com', 1, 1),
('María', 'López', '40333444', 'Belgrano 567', '1122334455', 'maria.lopez@email.com', 1, 2),
('Roberto', 'Sánchez', '28555666', 'San Martín 890', '1199887766', 'roberto.sanchez@email.com', 0, 1);

-- Autores
INSERT INTO AUTORES (Nombre, Apellido, Biografia) VALUES
('Jorge Luis', 'Borges', 'Escritor argentino de cuentos, ensayos y poemas.'),
('Julio', 'Cortázar', 'Escritor y traductor argentino, referente del boom latinoamericano.'),
('Robert C.', 'Martin', 'Ingeniero de software estadounidense, autor de Clean Code.');

-- Categorías
INSERT INTO CATEGORIAS (Descripcion) VALUES
('Ficción'),
('Informática'),
('Historia'),
('Ensayo');

-- Editoriales
INSERT INTO EDITORIALES (Nombre) VALUES
('Sudamericana'),
('Debolsillo'),
('Prentice Hall');

-- Libros
INSERT INTO LIBROS (Titulo, ISBN, IdEditorial, AnioEdicion) VALUES
('Ficciones', '978-987-566-647-4', 2, 2011),
('Rayuela', '978-84-204-3132-1', 1, 2019),
('Clean Code', '978-0-13-235088-4', 3, 2008);

-- Relaciones Libros - Autores
INSERT INTO LIBROS_AUTORES (IdLibro, IdAutor) VALUES
(1, 1), -- Ficciones -> Borges
(2, 2), -- Rayuela -> Cortázar
(3, 3); -- Clean Code -> Robert C. Martin

-- Relaciones Libros - Categorías
INSERT INTO LIBROS_CATEGORIAS (IdLibro, IdCategoria) VALUES
(1, 1), -- Ficciones -> Ficción
(2, 1), -- Rayuela -> Ficción
(3, 2); -- Clean Code -> Informática

-- Ejemplares físicos
INSERT INTO EJEMPLARES (IdLibro, CodigoInventario, Estado, Observaciones) VALUES
(1, 'INV-FIC-001', 'Disponible', 'Excelente estado'),
(1, 'INV-FIC-002', 'Prestado', 'Tapa con leve desgaste'),
(2, 'INV-RAY-001', 'Disponible', 'Nuevo'),
(3, 'INV-INF-001', 'Disponible', 'Edición en inglés'),
(3, 'INV-INF-002', 'En reparación', 'Hojas sueltas en el lomo');

-- Préstamo activo de ejemplo
INSERT INTO PRESTAMOS (NumeroComprobante, IdSocio, IdEjemplar, FechaInicio, FechaPrevistaDevolucion, FechaDevolucionReal, CantidadRenovaciones, Estado, ObservacionesDevolucion) VALUES
('COMP-0001', 1, 2, GETDATE(), DATEADD(DAY, 7, GETDATE()), NULL, 0, 'Activo', NULL);
GO    