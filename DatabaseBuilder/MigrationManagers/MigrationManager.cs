using Dapper;
using DatabaseBuilder.Constants;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace DatabaseBuilder.MigrationManagers
{
    public class MigrationManager : IMigrationManager
    {
        private const string _migrationsTableName = "Migrations";

        private const string _migrationsScriptFileNameColumn = "MigrationScriptFileName";

        private const string _migrationsRowDateTimeStampColumn = "MigrationRowDateTimeStamp";

        private const string _relativeMigrationPath = "Identity\\Migrations\\";

        private const string _relativeRollBackPath = "Identity\\RollBacks\\";

        public IMigrationManager CreateMigrationManager()
        {
            return new MigrationManager();
        }

        public void RunMigrations(object connection)
        {
            var sqlConnection = CastConnection(connection);

            CreateMigrationsTableIfNotPresent(sqlConnection);

            foreach (var scriptFilePath in GetMigrationScriptFilePaths())
            {
                var scriptFileName = GetMigrationFileNameFromPath(scriptFilePath);
                if (!MigrationTableContainsMigration(sqlConnection, scriptFileName))
                {
                    Console.WriteLine($"Performing migration: {scriptFileName}");
                    try
                    {
                        UpdateDatabase(sqlConnection, scriptFilePath);
                        AddMigrationToLog(sqlConnection, scriptFileName);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"Failed to perform migration with message {exception.Message}");
                        Console.WriteLine("Stopping program...");
                        return;
                    }
                }
            }
        }

        public void RollBackMigrations(object connection)
        {
            var sqlConnection = CastConnection(connection);

            if (!MigrationsTableExists(sqlConnection)) { return; }

            var scriptFilePathsInReversedOrder = GetMigrationScriptFilePaths().OrderByDescending(x => x);

            foreach (var scriptFilePath in scriptFilePathsInReversedOrder)
            {
                var scriptFileName = GetMigrationFileNameFromPath(scriptFilePath);
                if (MigrationTableContainsMigration(sqlConnection, scriptFileName))
                {
                    Console.WriteLine($"Performing migration roll back: {scriptFileName}");
                    try
                    {
                        string rollBackScriptFilePath = GetRollBackScriptPathFromMigrationScriptPath(scriptFilePath);
                        UpdateDatabase(sqlConnection, rollBackScriptFilePath);
                        DeleteMigrationFromLog(sqlConnection, scriptFileName);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"Failed to perform migration roll back with message {exception.Message}");
                        Console.WriteLine("Stopping program...");
                        return;
                    }
                }
            }
        }

        private static void UpdateDatabase(SqlConnection sqlConnection, string scriptFilePath)
        {
            var sql = File.ReadAllText(scriptFilePath).Replace(SqlKeywords.Go, "");
            sqlConnection.Query(sql);
        }

        static string GetRollBackScriptPathFromMigrationScriptPath(string scriptFilePath)
        {
            return scriptFilePath.Replace(MigrationKeywords.Migrate, MigrationKeywords.RollBack)
                                .Replace(_relativeMigrationPath, _relativeRollBackPath);
        }

        private SqlConnection CastConnection(object connection)
        {
            if (connection is SqlConnection)
            {
                return (SqlConnection)connection;
            }
            else
            {
                throw new Exception();
            }
        }

        private void CreateMigrationsTableIfNotPresent(SqlConnection sqlConnection)
        {
            if (!MigrationsTableExists(sqlConnection))
            {
                CreateMigrationsTable(sqlConnection);
            }
        }

        private bool MigrationsTableExists(SqlConnection sqlConnection)
        {
            var sql = $"SELECT OBJECT_ID('{_migrationsTableName}', 'U') AS Result;";
            var result = sqlConnection.Query<string>(sql).ToList();
            return (result.First() != null);
        }

        private void CreateMigrationsTable(SqlConnection sqlConnection)
        {
            var sql = $"CREATE TABLE[dbo].[{_migrationsTableName}]" +
                $"([{_migrationsScriptFileNameColumn}] NVARCHAR (256) NOT NULL," +
                $"[{_migrationsRowDateTimeStampColumn}] NVARCHAR(256) NOT NULL)";
            sqlConnection.Query(sql);
        }

        private List<string> GetMigrationScriptFilePaths()
        {
            var migrationScriptsFolder = GetApplicationRoot() + _relativeMigrationPath;

            return Directory.GetFiles(migrationScriptsFolder)
                .ToList();
        }

        private string GetMigrationFileNameFromPath(string scriptFilePath)
        {
            return scriptFilePath.Split("\\").Last();
        }

        private bool MigrationTableContainsMigration(SqlConnection sqlConnection, string migrationFileName)
        {
            var sql = $"SELECT COUNT(*) FROM [dbo].[{_migrationsTableName}]" +
                $"WHERE[{_migrationsScriptFileNameColumn}] = '{migrationFileName}'";
            var result = sqlConnection.Query<int>(sql).ToList();
            return (result.First() > 0);
        }

        private void AddMigrationToLog(SqlConnection sqlConnection, string scriptFileName, 
            bool success = true, string errorMessage = null)
        {
            var sql = $"INSERT INTO [dbo].{_migrationsTableName} ({_migrationsScriptFileNameColumn}, {_migrationsRowDateTimeStampColumn}) " +
                $"values ('{scriptFileName}','{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}');";
            sqlConnection.Query(sql);
        }

        private void DeleteMigrationFromLog(SqlConnection sqlConnection, string scriptFileName,
            bool success = true, string errorMessage = null)
        {
            var sql = $"DELETE FROM [dbo].{_migrationsTableName} WHERE {_migrationsScriptFileNameColumn} LIKE '%{scriptFileName}%';";
            sqlConnection.Query(sql);
        }

        private void DeleteMigrationTable(SqlConnection sqlConnection)
        {
            var sql = "DROP TABLE[dbo].[Migrations];";
            sqlConnection.Query(sql);
        }

        private static string GetApplicationRoot()
        {
            return AppDomain.CurrentDomain.BaseDirectory
                .Split("bin")
                .First();
        }
    }
}
