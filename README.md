# TP ADO.NET — CRUD Etudiants

Application console C# avec ADO.NET réalisant les opérations CRUD sur une table `Etudiants` dans SQL Server.

## Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download) ou supérieur
- SQL Server (Express ou autre)
- SQL Server Management Studio (SSMS) ou sqlcmd

## Installation de la base de données

1. Ouvrir SSMS ou sqlcmd et se connecter à votre instance SQL Server locale.
2. Exécuter le script de création :

   **Avec SSMS :** Ouvrir `sql/setup.sql` et exécuter (F5).

   **Avec sqlcmd :**
   ```
   sqlcmd -S . -E -i sql/setup.sql
   ```

   Ce script crée la base `EHODSI_DB`, la table `Etudiants` et insère 6 enregistrements de test.

## Configuration de la connexion

Dans `Program.cs`, modifier la constante `CONN_STR` selon votre instance SQL Server :

| Instance | Valeur du `Server` |
|---|---|
| Instance par défaut (MSSQLSERVER) | `Server=.` |
| Instance nommée (ex: SQLEXPRESS) | `Server=.\SQLEXPRESS` |
| Serveur distant | `Server=NOM_SERVEUR` |

Exemple pour une instance par défaut locale :
```csharp
const string CONN_STR = "Server=.;TrustServerCertificate=True;Database=EHODSI_DB;Integrated Security=True";
```

## Lancement

```bash
dotnet run
```

## Fonctionnalités

- Affichage au démarrage des étudiants avec bourse > 50 000 F (ordre décroissant)
- Menu interactif :
  - **C** — Créer un étudiant
  - **R** — Rechercher par numéro
  - **U** — Modifier nom et/ou bourse
  - **D** — Supprimer (avec confirmation)
  - **L** — Lister tous les étudiants
  - **Q** — Quitter
