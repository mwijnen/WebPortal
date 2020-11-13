using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseBuilder.Constants
{
    public static class SqlKeywords
    {
        public static string Go { get; set; } = "GO";
    }

    public static class MigrationKeywords
    {
        public static string Migrate { get; set; } = "Migrate";
        public static string RollBack { get; set; } = "RollBack";
    }
}
