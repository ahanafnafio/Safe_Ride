/*namespace Saferide.Database;

public string Fetccusertriphistory(){
    return @"
            SELECT t.tripID, t.pickupLocation, t.destination, t.rideDate, v.make, v.model
            FROM TripHistory t
            JOIN Vehicles v ON t.vehicleID = v.vehicleID
            WHERE t.riderID = $userID OR t.driverID = $userID
            ORDER BY t.rideDate DESC;
            ";
}
I am commenting this cause im not sure if this file would break the code or not*/