CREATE DATABASE Salud;

GO

USE Salud;

GO

CREATE TABLE Pacientes (
    PacienteID INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    FechaNacimiento DATE,
    Genero CHAR(1),
    Direccion VARCHAR(200),
    Telefono VARCHAR(15),
    Email VARCHAR(100)
);

CREATE TABLE Medicos (
    MedicoID INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Especialidad VARCHAR(100),
    Telefono VARCHAR(15),
    Email VARCHAR(100)
);

CREATE TABLE Citas (
    CitaID INT PRIMARY KEY IDENTITY(1,1),
    PacienteID INT NOT NULL,
    MedicoID INT NOT NULL,
    FechaHora DATETIME NOT NULL,
    MotivoConsulta VARCHAR(500),
    FOREIGN KEY (PacienteID) REFERENCES Pacientes(PacienteID),
    FOREIGN KEY (MedicoID) REFERENCES Medicos(MedicoID)
);

CREATE TABLE Tratamientos (
    TratamientoID INT PRIMARY KEY IDENTITY(1,1),
    PacienteID INT NOT NULL,
    MedicoID INT NOT NULL,
    FechaTratamiento DATE NOT NULL,
    Descripcion VARCHAR(500),
    Costo DECIMAL(10,2),
    FOREIGN KEY (PacienteID) REFERENCES Pacientes(PacienteID),
    FOREIGN KEY (MedicoID) REFERENCES Medicos(MedicoID)
);

CREATE TABLE Facturacion (
    FacturaID INT PRIMARY KEY IDENTITY(1,1),
    PacienteID INT NOT NULL,
    TratamientoID INT NOT NULL,
    FechaFactura DATE NOT NULL,
    MontoTotal DECIMAL(10,2) NOT NULL,
    Estado VARCHAR(50),
    FOREIGN KEY (PacienteID) REFERENCES Pacientes(PacienteID),
    FOREIGN KEY (TratamientoID) REFERENCES Tratamientos(TratamientoID)
);

CREATE TABLE HistorialMedico (
    HistorialID INT PRIMARY KEY IDENTITY(1,1),
    PacienteID INT NOT NULL,
    FechaActualizacion DATETIME NOT NULL,
    Detalles VARCHAR(MAX),
    FOREIGN KEY (PacienteID) REFERENCES Pacientes(PacienteID)
);

GO

CREATE TRIGGER TR_ActualizarHistorialMedico
ON Tratamientos
AFTER INSERT
AS
BEGIN
    DECLARE @PacienteID INT, @Descripcion VARCHAR(500), @FechaTratamiento DATE;

    SELECT @PacienteID = i.PacienteID, @Descripcion = i.Descripcion, @FechaTratamiento = i.FechaTratamiento
    FROM inserted i;

    INSERT INTO HistorialMedico (PacienteID, FechaActualizacion, Detalles)
    VALUES (@PacienteID, GETDATE(), 'Nuevo tratamiento registrado: ' + @Descripcion + ' el ' + CONVERT(VARCHAR, @FechaTratamiento, 103));
END;
GO

CREATE PROCEDURE sp_ReporteFacturacion
AS
BEGIN
    SELECT 
        f.FacturaID,
        p.Nombre + ' ' + p.Apellido AS Paciente,
        f.FechaFactura,
        f.MontoTotal,
        f.Estado
    FROM 
        Facturacion f
        INNER JOIN Pacientes p ON f.PacienteID = p.PacienteID;
END;
GO

CREATE PROCEDURE sp_GestionarCitas
AS
BEGIN
    SELECT 
        c.CitaID,
        p.Nombre + ' ' + p.Apellido AS Paciente,
        m.Nombre + ' ' + m.Apellido AS Medico,
        c.FechaHora,
        c.MotivoConsulta
    FROM 
        Citas c
        INNER JOIN Pacientes p ON c.PacienteID = p.PacienteID
        INNER JOIN Medicos m ON c.MedicoID = m.MedicoID;
END;
GO

CREATE PROCEDURE sp_CalcularCostosTratamiento
AS
BEGIN
    SELECT 
        t.TratamientoID,
        p.Nombre + ' ' + p.Apellido AS Paciente,
        m.Nombre + ' ' + m.Apellido AS Medico,
        t.FechaTratamiento,
        t.Descripcion,
        t.Costo
    FROM 
        Tratamientos t
        INNER JOIN Pacientes p ON t.PacienteID = p.PacienteID
        INNER JOIN Medicos m ON t.MedicoID = m.MedicoID;
END;
GO


Ejemplo de insert
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Juan', 'Pérez', '1980-01-15', 'M', 'Calle Falsa 123', '555-1234', 'juan.perez@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Ana', 'García', '1992-03-22', 'F', 'Av. Siempre Viva 456', '555-2345', 'ana.garcia@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Carlos', 'Sánchez', '1985-07-14', 'M', 'Calle Sol 789', '555-3456', 'carlos.sanchez@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Lucía', 'Martínez', '1978-11-05', 'F', 'Av. Libertador 1011', '555-4567', 'lucia.martinez@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Marcos', 'Fernández', '1990-06-18', 'M', 'Calle Luna 1112', '555-5678', 'marcos.fernandez@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Sofía', 'Rodríguez', '1983-02-10', 'F', 'Av. Estrella 1314', '555-6789', 'sofia.rodriguez@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Mateo', 'Gómez', '1995-09-27', 'M', 'Calle Río 1516', '555-7890', 'mateo.gomez@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Valentina', 'López', '1988-04-03', 'F', 'Av. Bosque 1718', '555-8901', 'valentina.lopez@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Jorge', 'Díaz', '1975-12-22', 'M', 'Calle Montaña 1920', '555-9012', 'jorge.diaz@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Martina', 'Pérez', '1982-08-15', 'F', 'Av. Jardín 2122', '555-0123', 'martina.perez@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Lucas', 'Torres', '1994-11-30', 'M', 'Calle Flor 2324', '555-1234', 'lucas.torres@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Camila', 'Hernández', '1987-05-25', 'F', 'Av. Horizonte 2526', '555-2345', 'camila.hernandez@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Tomás', 'Ruiz', '1991-01-18', 'M', 'Calle Bosque 2728', '555-3456', 'tomas.ruiz@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Natalia', 'Ramírez', '1984-06-07', 'F', 'Av. Mar 2930', '555-4567', 'natalia.ramirez@example.com');
GO
INSERT INTO Pacientes (Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, Email)
VALUES ('Santiago', 'Morales', '1979-03-11', 'M', 'Calle Roca 3132', '555-5678', 'santiago.morales@example.com');
GO


INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dr. Ana', 'Gómez', 'Cardiología', '555-5678', 'ana.gomez@example.com');
GO
INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dr. Carlos', 'Fernández', 'Neurología', '555-6789', 'carlos.fernandez@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dra. Laura', 'Martínez', 'Pediatría', '555-7890', 'laura.martinez@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dr. Jorge', 'López', 'Dermatología', '555-8901', 'jorge.lopez@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dra. María', 'García', 'Ginecología', '555-9012', 'maria.garcia@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dr. Tomás', 'Ruiz', 'Ortopedia', '555-0123', 'tomas.ruiz@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dra. Paula', 'Sánchez', 'Oftalmología', '555-1234', 'paula.sanchez@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dr. Fernando', 'Díaz', 'Cardiología', '555-2345', 'fernando.diaz@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dra. Alicia', 'Morales', 'Pediatría', '555-3456', 'alicia.morales@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dr. Ricardo', 'Torres', 'Dermatología', '555-4567', 'ricardo.torres@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dra. Verónica', 'Ramírez', 'Ginecología', '555-5678', 'veronica.ramirez@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dr. Martín', 'Gómez', 'Ortopedia', '555-6789', 'martin.gomez@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dra. Lucía', 'Hernández', 'Neurología', '555-7890', 'lucia.hernandez@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dr. Pedro', 'Pérez', 'Cardiología', '555-8901', 'pedro.perez@example.com');
GO

INSERT INTO Medicos (Nombre, Apellido, Especialidad, Telefono, Email)
VALUES ('Dra. Valeria', 'Núñez', 'Oftalmología', '555-9012', 'valeria.nunez@example.com');
GO



INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (1, 1, '2024-08-22 10:00:00', 'Chequeo anual');
GO
INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (1, 2, '2024-08-22 10:30:00', 'Control de presión arterial');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (3, 1, '2024-08-22 11:00:00', 'Dolor de cabeza persistente');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (2, 3, '2024-08-22 10:00:00', 'Chequeo anual');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (4, 4, '2024-08-23 09:00:00', 'Dolor en las articulaciones');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (5, 2, '2024-08-23 09:30:00', 'Revisión de resultados de laboratorio');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (6, 5, '2024-08-24 08:00:00', 'Consulta sobre medicamento');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (7, 3, '2024-08-24 08:30:00', 'Consulta de seguimiento');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (8, 4, '2024-08-24 09:00:00', 'Dolor abdominal');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (9, 6, '2024-08-24 09:30:00', 'Chequeo de rutina');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (10, 1, '2024-08-24 10:00:00', 'Consulta por fiebre');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (11, 7, '2024-08-25 10:30:00', 'Revisión de dieta');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (12, 8, '2024-08-25 11:00:00', 'Chequeo anual');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (13, 5, '2024-08-25 11:30:00', 'Consulta de control');
GO

INSERT INTO Citas (PacienteID, MedicoID, FechaHora, MotivoConsulta)
VALUES (14, 9, '2024-08-25 12:00:00', 'Chequeo de rutina');
GO



INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (1, 1, '2024-08-22', 'Tratamiento de hipertensión', 1500.00);
GO
INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (2, 3, '2024-08-22', 'Terapia física para dolor de espalda', 1200.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (4, 2, '2024-08-23', 'Tratamiento para colesterol alto', 1800.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (3, 4, '2024-08-23', 'Tratamiento de asma', 1300.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (5, 1, '2024-08-24', 'Terapia de rehabilitación', 2000.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (6, 5, '2024-08-24', 'Tratamiento de diabetes', 1500.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (7, 3, '2024-08-24', 'Consulta de seguimiento para hipertensión', 1000.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (8, 4, '2024-08-25', 'Terapia ocupacional', 1900.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (9, 2, '2024-08-25', 'Tratamiento de anemia', 1400.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (10, 6, '2024-08-25', 'Tratamiento de alergias', 1100.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (11, 7, '2024-08-26', 'Revisión de medicación', 800.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (12, 8, '2024-08-26', 'Tratamiento de migrañas', 1600.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (13, 5, '2024-08-26', 'Tratamiento de bronquitis', 1700.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (14, 9, '2024-08-27', 'Terapia respiratoria', 1350.00);
GO

INSERT INTO Tratamientos (PacienteID, MedicoID, FechaTratamiento, Descripcion, Costo)
VALUES (15, 10, '2024-08-27', 'Tratamiento de ansiedad', 1800.00);
GO




INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (1, 1, '2024-08-22', 1500.00, 'Pagado');
GO
INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (2, 8, '2024-08-22', 1200.00, 'Pendiente');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (3, 8, '2024-08-23', 1800.00, 'Pagado');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (4, 3, '2024-08-23', 1300.00, 'Pendiente');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (5, 6, '2024-08-24', 2000.00, 'Pagado');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (6, 5, '2024-08-24', 1500.00, 'Pendiente');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (7, 7, '2024-08-24', 1000.00, 'Pagado');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (8, 8, '2024-08-25', 1900.00, 'Pendiente');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (9, 7, '2024-08-25', 1400.00, 'Pagado');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (10, 10, '2024-08-25', 1100.00, 'Pendiente');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (11, 11, '2024-08-26', 800.00, 'Pagado');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (12, 12, '2024-08-26', 1600.00, 'Pendiente');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (13, 13, '2024-08-26', 1700.00, 'Pagado');
GO

INSERT INTO Facturacion (PacienteID, TratamientoID, FechaFactura, MontoTotal, Estado)
VALUES (14, 10, '2024-08-27', 1350.00, 'Pendiente');
GO


