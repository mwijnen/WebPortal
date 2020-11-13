using DatabaseBuilder.MigrationManagers;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DatabaseBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Migration Manager");

            var connectionString = ConfigurationManager.AppSettings["connectionString"];
            var connection = new SqlConnection(connectionString);

            Console.WriteLine("Enter option to run scripts");
            Console.WriteLine("1: Run database migrations");
            Console.WriteLine("2: Roll back database migrations");

            var option = Console.ReadLine();

            IMigrationManager migrationManager = new MigrationManager();
            switch (option)
            {
                case "1":
                    migrationManager.RunMigrations(connection);
                    break;
                case "2":
                    migrationManager.RollBackMigrations(connection);
                    break;
                default:
                    break;
            }
            
            Console.WriteLine("Program completed. Press any key to close console");
            Console.ReadLine();
        }
    }
}
