using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

        private const string _relativeMigrationPath = "\\Identity\\Migrations\\";

        private const string _relativeRollBackPath = "\\Identity\\RollBacks\\";

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
                if (!MigrationTableContainsMigration(sqlConnection, scriptFilePath))
                {
                    // run migration
                }
            }

            //select new migrations

            //run migrations
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
                .Skip(1)
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




        private void DeleteMigrationTable(SqlConnection sqlConnection)
        {
            var sql = "DROP TABLE[dbo].[Migrations];";
            sqlConnection.Query(sql);
        }



        public void RollBackMigrations(object connection)
        {
            var sqlConnection = CastConnection(connection);

            //list all stored migrations

            //for each migration find roll back script

            //roll back migrations
        }



        //ListMigrationQueue

        //Check if migration has been run
        // SELECT COUNT(*) FROM[web - portal - dev].[dbo].[Migrations]
        // WHERE[MigrationScriptFileName] = '000001-Migrate-AspNetRoles.sql'


        //private void InitializeMigrationTableIfNotPresent(SqlConnection connection)
        //{
        //    var migrationScriptFilePath = GetMigrationScriptFilePaths().First();

        //    if (!MigrationTableExists(connection))
        //    {
        //        RunMigrationScript(connection, migrationScriptFilePath);
        //    }
        //}



        private List<string> GetRollBackScriptFilePaths()
        {
            var rollBackScriptsFolder = GetApplicationRoot() + _relativeMigrationPath;

            return Directory.GetFiles(rollBackScriptsFolder)
                .Skip(1)
                .ToList();
        }

        private static string GetApplicationRoot()
        {
            return AppDomain.CurrentDomain.BaseDirectory
                .Split("bin")
                .First();
        }
    }
}
