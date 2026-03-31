// Telling the code where to find the SQLite package in the project
using Microsoft.Data.Sqlite;

// Importing User class from Models folder
using Saferide.Models;

// Puts the class in the database namespace, helpful for other .cs files accessing it from elsewhere in the project
namespace Saferide.Database;

public class SQLiteCommunication
{
    // *** Members ***

    // DatabaseQueries object to hold the SQL query strings
    private DatabaseQueries dbQueries = new DatabaseQueries();



    // *** Methods ***

    // Establish a connection to the database, execute email lookup query, return true or false
    public bool ExecuteEmailLookupQuery(User user)
    {
        // Get the SQL query string from DatabaseQueries.cs
        string query = dbQueries.EmailLookupQuery();

        // Connect to the SQLite Database file
        using var connection = new SqliteConnection("Data Source=saferide.db");
        connection.Open();

        // Create a command object with the query and open connection
        using var command = new SqliteCommand(query, connection);

        // Use a parameter instead of directly inserting email into SQL query, helping prevent SQL injection
        command.Parameters.AddWithValue("$email", user.GetEmail());

        // Parse through returned entries using the reader method
        using var reader = command.ExecuteReader();

        // If the reader has at least one row, then the email associated with an existing user in the database was found
        if (reader.HasRows)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Establish a connection to the database, execute store new user query, return true or false
    public bool ExecuteStoreNewUserQuery(User user)
    {
        // Get the SQL query string from DatabaseQueries.cs
        string query = dbQueries.StoreNewUserQuery();

        // Connect to the SQLite Database file
        using var connection = new SqliteConnection("Data Source=saferide.db");
        connection.Open();

        // Create a command object with the query and open connection
        using var command = new SqliteCommand(query, connection);

        // Use parameters instead of directly inserting user attributes into the SQL query, helping prevent SQL injection
        command.Parameters.AddWithValue("$firstName", user.GetFirstName());
        command.Parameters.AddWithValue("$lastName", user.GetLastName());
        command.Parameters.AddWithValue("$email", user.GetEmail());
        command.Parameters.AddWithValue("$passwordHash", user.GetPasswordHash());
        command.Parameters.AddWithValue("$userRole", user.GetRole());


        // ExecuteNonQuery method instead of reader, because we are inserting, not looking up entries
        var rowsAffected = command.ExecuteNonQuery();

        // If at least one row was affected, the query was successful
        if (rowsAffected > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Establish a connection to the database, execute fetch user query, return User object with fetched member values if found, otherwise return null
    public User? ExecuteFetchUserQuery(User user)
    {
        // Create new user object to hold fetched members, for use in Authentication.cs
        User foundUser = new User();

        // Get the SQL query string from DatabaseQueries.cs
        string query = dbQueries.FetchUserQuery();

        // Connect to the SQLite Database file
        using var connection = new SqliteConnection("Data Source=saferide.db");
        connection.Open();

        // Create a command object with the query and open connection
        using var command = new SqliteCommand(query, connection);

        // Use a parameter instead of directly inserting email into SQL query, helping prevent SQL injection
        command.Parameters.AddWithValue("$email", user.GetEmail());

        // Parse through returned entries using the reader method
        using var reader = command.ExecuteReader();

        // Read the first row, if it exists
        if (reader.Read())
        {
            // Grab fetched values from database, storing in placeholder variables one at a time
            var id = reader.GetInt32(0);
            var passwordHash = reader.GetString(1);                
            var userRole = reader.GetString(2);

            // Store the fetched values in the foundUser object to be returned
            foundUser.SetUserID(id);
            foundUser.SetPasswordHash(passwordHash);
            foundUser.SetUserRole(userRole);

            return foundUser;
        }

        // Return null if no matching email was found
        else
        {
            return null;
        }
    }
}