using DatabaseBuilder.DatabaseBuilders;
using System;
using System.Configuration;

namespace DatabaseBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running Migrations");

            var connectionString = ConfigurationManager.AppSettings["connectionString"];

            //var databaseBuilder = SqlServerDatabaseBuilder.

            Console.WriteLine("Completed. Press any key to close console");
            Console.ReadLine();
        }
    }
}
