/* ============================================================================
   Script de base de datos - Sistema de Gestión de Rutinas de Gimnasio
   ============================================================================
   PARTE 1: creación de tablas. Generada con:
              dotnet ef migrations script --idempotent -o ../database/script.sql
            "Idempotente" = se puede ejecutar varias veces sin romper nada: cada
            bloque pregunta primero si la migración ya está aplicada.
   PARTE 2: datos de ejemplo (escrita a mano). Cada rutina se inserta solo si
            no existe otra con el mismo nombre.

   Uso (la base tiene que existir; -f 65001 = leer el archivo como UTF-8):
     sqlcmd -S .\SQLEXPRESS -E -Q "CREATE DATABASE GymRutinasDb"
     sqlcmd -S .\SQLEXPRESS -E -d GymRutinasDb -f 65001 -i database\script.sql

   NOTA: no hace falta correr este script si usás la API: al arrancar crea la
   base y las tablas sola. Sirve para crear todo desde SQL o para cargar datos.
   ============================================================================ */


/* ============================== PARTE 1: TABLAS ============================ */

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260911182541_Inicial'
)
BEGIN
    CREATE TABLE [Rutinas] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        CONSTRAINT [PK_Rutinas] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260911182541_Inicial'
)
BEGIN
    CREATE TABLE [Ejercicios] (
        [Id] int NOT NULL IDENTITY,
        [RutinaId] int NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Dia] nvarchar(10) NOT NULL,
        [Series] int NOT NULL,
        [Repeticiones] int NOT NULL,
        [Peso] decimal(6,2) NULL,
        [Notas] nvarchar(500) NULL,
        [Orden] int NOT NULL,
        CONSTRAINT [PK_Ejercicios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Ejercicios_Rutinas_RutinaId] FOREIGN KEY ([RutinaId]) REFERENCES [Rutinas] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260911182541_Inicial'
)
BEGIN
    CREATE INDEX [IX_Ejercicios_RutinaId] ON [Ejercicios] ([RutinaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260911182541_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Rutinas_Nombre] ON [Rutinas] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260911182541_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260911182541_Inicial', N'10.0.12');
END;

COMMIT;
GO


/* ========================= PARTE 2: DATOS DE EJEMPLO ======================= */
/* SCOPE_IDENTITY() devuelve el Id que SQL Server acaba de generar para la
   rutina, y lo usamos como RutinaId (clave foránea) de sus ejercicios.
   Los días van sin tilde porque así los guarda la API (enum DiaSemana).      */

SET NOCOUNT ON;
GO

-- Rutina 1: tres días (Lunes, Miércoles, Viernes)
IF NOT EXISTS (SELECT 1 FROM [Rutinas] WHERE [Nombre] = N'Full Body Principiante')
BEGIN
    DECLARE @IdRutina int;

    INSERT INTO [Rutinas] ([Nombre], [Descripcion], [FechaCreacion])
    VALUES (N'Full Body Principiante', N'Cuerpo completo tres veces por semana para empezar a entrenar.', GETDATE());

    SET @IdRutina = SCOPE_IDENTITY();

    INSERT INTO [Ejercicios] ([RutinaId], [Nombre], [Dia], [Series], [Repeticiones], [Peso], [Notas], [Orden])
    VALUES
        (@IdRutina, N'Sentadilla',          N'Lunes',     3, 12, 40.00, N'Bajar hasta la paralela.', 1),
        (@IdRutina, N'Press de banca',      N'Lunes',     3, 10, 30.00, NULL,                        2),
        (@IdRutina, N'Remo con mancuerna',  N'Lunes',     3, 12, 14.00, NULL,                        3),
        (@IdRutina, N'Peso muerto rumano',  N'Miercoles', 3, 10, 40.00, N'Espalda recta.',           1),
        (@IdRutina, N'Press militar',       N'Miercoles', 3, 10, 20.00, NULL,                        2),
        (@IdRutina, N'Plancha',             N'Miercoles', 3, 1,  NULL,  N'Mantener 40 segundos.',    3),
        (@IdRutina, N'Zancadas',            N'Viernes',   3, 12, 10.00, N'12 por pierna.',           1),
        (@IdRutina, N'Dominadas asistidas', N'Viernes',   3, 8,  NULL,  N'Con banda elástica.',      2);
END;
GO

-- Rutina 2: solo Martes y Jueves
IF NOT EXISTS (SELECT 1 FROM [Rutinas] WHERE [Nombre] = N'Torso / Pierna')
BEGIN
    DECLARE @IdRutina int;

    INSERT INTO [Rutinas] ([Nombre], [Descripcion], [FechaCreacion])
    VALUES (N'Torso / Pierna', N'Dos días: torso el martes y pierna el jueves.', GETDATE());

    SET @IdRutina = SCOPE_IDENTITY();

    INSERT INTO [Ejercicios] ([RutinaId], [Nombre], [Dia], [Series], [Repeticiones], [Peso], [Notas], [Orden])
    VALUES
        (@IdRutina, N'Press de banca',     N'Martes', 4, 8,  60.00,  NULL,                  1),
        (@IdRutina, N'Dominadas',          N'Martes', 4, 6,  NULL,   N'Peso corporal.',     2),
        (@IdRutina, N'Fondos en paralelas', N'Martes', 3, 10, NULL,  NULL,                  3),
        (@IdRutina, N'Sentadilla',         N'Jueves', 5, 5,  90.00,  NULL,                  1),
        (@IdRutina, N'Prensa',             N'Jueves', 4, 10, 150.00, NULL,                  2),
        (@IdRutina, N'Gemelos de pie',     N'Jueves', 4, 15, 50.00,  N'Pausa arriba 1 seg.', 3);
END;
GO

-- Rutina 3: un solo día (Sábado)
IF NOT EXISTS (SELECT 1 FROM [Rutinas] WHERE [Nombre] = N'Cardio y Core')
BEGIN
    DECLARE @IdRutina int;

    INSERT INTO [Rutinas] ([Nombre], [Descripcion], [FechaCreacion])
    VALUES (N'Cardio y Core', NULL, GETDATE());

    SET @IdRutina = SCOPE_IDENTITY();

    INSERT INTO [Ejercicios] ([RutinaId], [Nombre], [Dia], [Series], [Repeticiones], [Peso], [Notas], [Orden])
    VALUES
        (@IdRutina, N'Burpees',          N'Sabado', 4, 15, NULL, NULL,              1),
        (@IdRutina, N'Abdominales rueda', N'Sabado', 3, 12, NULL, NULL,             2),
        (@IdRutina, N'Soga',             N'Sabado', 5, 1,  NULL, N'1 minuto cada serie.', 3);
END;
GO
