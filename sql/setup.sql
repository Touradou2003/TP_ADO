-- Création de la base EHODSI_DB et de la table Etudiants
-- À exécuter une seule fois dans SSMS sur le serveur EHODHFMASSE

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'EHODSI_DB')
    CREATE DATABASE EHODSI_DB;
GO

USE EHODSI_DB;
GO

-- Table Etudiants (étape 1 : sans DateNaissance, ajoutée à l'étape 2)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Etudiants')
BEGIN
    CREATE TABLE Etudiants (
        Numero        INT           PRIMARY KEY IDENTITY(1,1),
        NomEtudiant   NVARCHAR(100) NOT NULL,
        MtBourse      DECIMAL(10,2) NOT NULL,
        DateNaissance DATE          NULL   -- ajoutée à l'étape 2
    );
END
GO

-- Données initiales (numéros conformes au sujet)
IF NOT EXISTS (SELECT 1 FROM Etudiants)
BEGIN
    INSERT INTO Etudiants (NomEtudiant, MtBourse, DateNaissance) VALUES
        ('Modou Diop',   140000, '2004-02-14'),
        ('Doudou Fall',  150000, '1999-08-04'),
        ('Ismaila Sarr',  50000, NULL),
        ('Abdoulaye Sy',  90000, '2000-06-01'),
        ('Mariama Ba',    85000, '1996-05-31'),
        ('Talla Sylla',   48000, NULL);
END
GO
