namespace Saferide.Database;

public class HistoryQueries
{
    // Fetches the full history for a specific user (Rider or Driver)
    public string FetchUserTripHistory()
    {
        return @"
            SELECT t.tripID, t.pickupLocation, t.destination, t.rideDate, t.status, v.make, v.model
            FROM TripHistory t
            JOIN Vehicles v ON t.vehicleID = v.vehicleID
            WHERE t.riderID = $userID OR t.driverID = $userID
            ORDER BY t.rideDate DESC;
            ";
    }

    // Creates a new trip entry when a ride is initiated
    public string CreateTripEntry()
    {
        return @"
            INSERT INTO TripHistory (riderID, driverID, vehicleID, pickupLocation, destination, status)
            VALUES ($riderID, $driverID, $vehicleID, $pickupLocation, $destination, 'In-Progress');
            ";
    }

    // Updates the status (e.g., 'In-Progress' -> 'Completed')
    public string UpdateTripStatus()
    {
        return @"
            UPDATE TripHistory
            SET status = $status
            WHERE tripID = $tripID;
            ";
    }

    // Gets specific details for one trip
    public string GetTripById()
    {
        return @"
            SELECT * FROM TripHistory
            WHERE tripID = $tripID;
            ";
    }
}