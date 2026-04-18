namespace Saferide.Database;

public class DatabaseQueries
{
    // After user enters a valid email address, need to verify there's not an entry in the database with that email already
    public string EmailLookupQuery()
    {
        return @"  
                SELECT userID FROM Users
                WHERE email = $email;
                ";
    }

    // If email is unregistered, need to store the input in the database
    public string StoreNewUserQuery()
    {
        return @" 
                INSERT INTO Users (firstName, lastName, email, passwordHash, userRole)
                VALUES ($firstName, $lastName, $email, $passwordHash, $userRole);
                ";
    }

    // check that user-entered email exists in database, and return the userID, passwordHash, and userRole
    public string FetchUserQuery()
    {
        return @"
                SELECT userID, passwordHash, userRole
                FROM Users
                WHERE email = $email;
                ";
    }
}