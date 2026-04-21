namespace Saferide.Database;

public class DatabaseQueries
{
    // Check whether email exists in Riders table
    public string LookupRiderEmail()
    {
        return @"  
                SELECT riderID FROM Riders
                WHERE email = $email;
                ";
    }

    // Check whether email exists in Drivers table
    public string LookupDriverEmail()
    {
        return @"  
                SELECT driverID FROM Drivers
                WHERE email = $email;
                ";
    }

    // Store new user in Riders table
    public string StoreNewRider()
    {
        return @" 
                INSERT INTO Riders (firstName, lastName, email, passwordHash)
                VALUES ($firstName, $lastName, $email, $passwordHash);
                ";
    }

    // Store new user in Drivers table
    public string StoreNewDriver()
    {
        return @" 
                INSERT INTO Drivers (firstName, lastName, email, passwordHash)
                VALUES ($firstName, $lastName, $email, $passwordHash);
                ";
    }

    // Fetch stored Rider data
    public string FetchRider()
    {
        return @"
                SELECT riderID, firstName, lastName, email, passwordHash
                FROM Riders
                WHERE email = $email;
                ";
    }

    // Fetch stored Driver data
    public string FetchDriver()
    {
        return @"
                SELECT driverID, firstName, lastName, email, passwordHash
                FROM Drivers
                WHERE email = $email;
                ";
    }
}