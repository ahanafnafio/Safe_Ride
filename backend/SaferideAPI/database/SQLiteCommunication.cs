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

    // Establish a connection to the database, execute lookup rider email query, return true or false
    public bool ExecuteLookupRiderEmail(string email)
    {
        // Get the SQL query string from DatabaseQueries.cs
        string query = dbQueries.LookupRiderEmail();

        // Connect to the SQLite Database file
        using var connection = new SqliteConnection("Data Source=database/saferide.db");
        connection.Open();

        // Create a command object with the query and open connection
        using var command = new SqliteCommand(query, connection);

        // Use a parameter instead of directly inserting email into SQL query, helping prevent SQL injection
        command.Parameters.AddWithValue("$email", email);

        // Parse through returned entries using the reader method
        using var reader = command.ExecuteReader();

        // If the reader has at least one row, then the email associated with an existing user in the database was found
        return reader.HasRows;
    }


    // Establish a connection to the database, execute lookup driver email query, return true or false
    public bool ExecuteLookupDriverEmail(string email)
    {
        // Get the SQL query string from DatabaseQueries.cs
        string query = dbQueries.LookupDriverEmail();

        // Connect to the SQLite Database file
        using var connection = new SqliteConnection("Data Source=database/saferide.db");
        connection.Open();

        // Create a command object with the query and open connection
        using var command = new SqliteCommand(query, connection);

        // Use a parameter instead of directly inserting email into SQL query, helping prevent SQL injection
        command.Parameters.AddWithValue("$email", email);

        // Parse through returned entries using the reader method
        using var reader = command.ExecuteReader();

        // If the reader has at least one row, then the email associated with an existing user in the database was found
        return reader.HasRows;
    }


    // Lookup email for both types of user for registration purposes, return true or false
    public bool EmailExistsAnywhere(string email)
    {
        return ExecuteLookupRiderEmail(email) || ExecuteLookupDriverEmail(email);
    }


    // Establish a connection to the database, execute store new rider query, return true or false
    public bool ExecuteStoreNewRider(Rider rider)
    {
        // Get the SQL query string from DatabaseQueries.cs
        string query = dbQueries.StoreNewRider();

        // Connect to the SQLite Database file
        using var connection = new SqliteConnection("Data Source=database/saferide.db");
        connection.Open();

        // Create a command object with the query and open connection
        using var command = new SqliteCommand(query, connection);

        // Use parameters instead of directly inserting user attributes into the SQL query, helping prevent SQL injection
        command.Parameters.AddWithValue("$firstName", rider.GetFirstName());
        command.Parameters.AddWithValue("$lastName", rider.GetLastName());
        command.Parameters.AddWithValue("$email", rider.GetEmail());
        command.Parameters.AddWithValue("$passwordHash", rider.GetPasswordHash());

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


    // Establish a connection to the database, execute store new driver query, return true or false
    public bool ExecuteStoreNewDriver(Driver driver)
    {
        // Get the SQL query string from DatabaseQueries.cs
        string query = dbQueries.StoreNewDriver();

        // Connect to the SQLite Database file
        using var connection = new SqliteConnection("Data Source=database/saferide.db");
        connection.Open();

        // Create a command object with the query and open connection
        using var command = new SqliteCommand(query, connection);

        // Use parameters instead of directly inserting user attributes into the SQL query, helping prevent SQL injection
        command.Parameters.AddWithValue("$firstName", driver.GetFirstName());
        command.Parameters.AddWithValue("$lastName", driver.GetLastName());
        command.Parameters.AddWithValue("$email", driver.GetEmail());
        command.Parameters.AddWithValue("$passwordHash", driver.GetPasswordHash());

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


    // Establish a connection to the database, execute fetch rider query, return Rider object with fetched member values if found, otherwise return null
    public Rider? ExecuteFetchRider(string email)
    {
        // Get the SQL query string from DatabaseQueries.cs
        string query = dbQueries.FetchRider();

        // Connect to the SQLite Database file
        using var connection = new SqliteConnection("Data Source=database/saferide.db");
        connection.Open();

        // Create a command object with the query and open connection
        using var command = new SqliteCommand(query, connection);

        // Use a parameter instead of directly inserting email into SQL query, helping prevent SQL injection
        command.Parameters.AddWithValue("$email", email);

        // Parse through returned entries using the reader method
        using var reader = command.ExecuteReader();

        // Read the first row, if it exists
        if (reader.Read())
        {
            // Grab fetched values from database, storing in placeholder variables one at a time
            var id = reader.GetInt32(0);
            var firstName = reader.GetString(1);
            var lastName = reader.GetString(2);
            var foundEmail = reader.GetString(3);
            var passwordHash = reader.GetString(4);

            // Store the fetched values in the foundUser object to be returned
            Rider foundRider = new Rider(firstName, lastName, foundEmail, passwordHash);
            foundRider.SetUserID(id);    // let database assign the ID, and populate foundRider object

            return foundRider;
        }

        // Return null if no matching email was found
        else
        {
            return null;
        }
    }


    // Establish a connection to the database, execute fetch driver query, return Driver object with fetched member values if found, otherwise return null
    public Driver? ExecuteFetchDriver(string email)
    {
        // Get the SQL query string from DatabaseQueries.cs
        string query = dbQueries.FetchDriver();

        // Connect to the SQLite Database file
        using var connection = new SqliteConnection("Data Source=database/saferide.db");
        connection.Open();

        // Create a command object with the query and open connection
        using var command = new SqliteCommand(query, connection);

        // Use a parameter instead of directly inserting email into SQL query, helping prevent SQL injection
        command.Parameters.AddWithValue("$email", email);

        // Parse through returned entries using the reader method
        using var reader = command.ExecuteReader();

        // Read the first row, if it exists
        if (reader.Read())
        {
            // Grab fetched values from database, storing in placeholder variables one at a time
            var id = reader.GetInt32(0);
            var firstName = reader.GetString(1);
            var lastName = reader.GetString(2);
            var foundEmail = reader.GetString(3);
            var passwordHash = reader.GetString(4);

            // Store the fetched values in the foundUser object to be returned
            Driver foundDriver = new Driver(firstName, lastName, foundEmail, passwordHash);
            foundDriver.SetUserID(id);    // let database assign the ID, and populate foundRider object

            return foundDriver;
        }

        // Return null if no matching email was found
        else
        {
            return null;
        }
    }
}