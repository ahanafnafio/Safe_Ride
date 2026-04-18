-- Script for creating the TripHistory table in the Saferide database
CREATE TABLE TripHistory
(
    tripID INTEGER PRIMARY KEY AUTOINCREMENT,
    
    -- Links to the Rider (the person who requested the ride)
    riderID INTEGER NOT NULL,
    
    -- Links to the Driver (the person who drove the car)
    driverID INTEGER NOT NULL,
    
    -- Links to the specific vehicle used (the Rider's car)
    vehicleID INTEGER NOT NULL,
    
    pickupLocation TEXT NOT NULL,
    destination TEXT NOT NULL,
    
    -- Using DATETIME to track when the ride happened
    -- DEFAULT CURRENT_TIMESTAMP automatically saves the time of the entry
    rideDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    -- Status could be 'Completed', 'Cancelled', or 'In-Progress'
    status TEXT NOT NULL CHECK(status IN ('Completed', 'Cancelled', 'In-Progress')),

    -- Foreign Key Constraints
    FOREIGN KEY (riderID) REFERENCES Users(userID),
    FOREIGN KEY (driverID) REFERENCES Users(userID),
    FOREIGN KEY (vehicleID) REFERENCES Vehicles(vehicleID)
);