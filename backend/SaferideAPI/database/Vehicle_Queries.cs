namespace Saferide.Database;

public class VehicleQueries
{
    // Check if a vehicle with this license plate already exists
    public string LicensePlateLookupQuery()
    {
        return @"  
                SELECT vehicleID FROM Vehicles
                WHERE licensePlate = $licensePlate;
                ";
    }

    // Store a new vehicle linked to a specific user (the owner/rider)
    public string StoreNewVehicleQuery()
    {
        return @" 
                INSERT INTO Vehicles (make, model, year, licensePlate, color, ownerID)
                VALUES ($make, $model, $year, $licensePlate, $color, $ownerID);
                ";
    }

    // Fetch all vehicles owned by a specific user
    public string FetchVehiclesByOwnerQuery()
    {
        return @"
                SELECT vehicleID, make, model, year, licensePlate, color
                FROM Vehicles
                WHERE ownerID = $ownerID;
                ";
    }

    // Update vehicle information (if the user gets a new car or changes plates)
    public string UpdateVehicleQuery()
    {
        return @"
                UPDATE Vehicles
                SET make = $make, model = $model, year = $year, color = $color
                WHERE vehicleID = $vehicleID AND ownerID = $ownerID;
                ";
    }
}
