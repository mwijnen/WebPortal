using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebPortal.Databases
{
    public class SqlServerDatabaseBuilder : IDatabaseBuilder
    {
        public void CreateDatabase()
        {
            var file = @"C:\Users\marce\Desktop\code\WebPortal\WebPortal\Databases\Initializations\AspNetRoleClaims.sql";
            var script = File.ReadAllText(file);

            //for each table check if table exists in database, if not create using script

            var connectionString = ""
        }
    }
}
