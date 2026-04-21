using Microsoft.Data.Sqlite;
using System.IO;

namespace Saferide.Database
{
    public class DatabaseInitializer
    {
        public static void Initialize()
        {
            // Stores the path to the Database file
            string dbPath = "database/saferide.db";

            // Stores the path to the Schema file
            string schemaPath = "database/saferide_schema.sql";

            // Creating and opening a connection to the database
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            // Read the schema file into a string to pass to the command object
            string schemaSql = File.ReadAllText(schemaPath);

            // Create a command object and pass schema string and open connection
            using var command = new SqliteCommand(schemaSql, connection);

            // Execute the command to create database tables if they do not already exist
            command.ExecuteNonQuery();
        }
    }
}