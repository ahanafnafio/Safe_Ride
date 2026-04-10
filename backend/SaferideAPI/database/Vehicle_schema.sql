-- Script for creating the Vehicles table in the Saferide database
-- In this model, the Vehicle belongs to the User/Rider who needs a driver.
CREATE TABLE Vehicles
(
    vehicleID INTEGER PRIMARY KEY AUTOINCREMENT,
    make TEXT NOT NULL,
    model TEXT NOT NULL,
    year INTEGER NOT NULL,
    licensePlate TEXT NOT NULL UNIQUE,
    color TEXT NOT NULL,
    
    -- Foreign Key: Links the vehicle to the OWNER (the Rider)
    ownerID INTEGER NOT NULL,
    
    FOREIGN KEY (ownerID) REFERENCES Users(userID) 
        ON DELETE CASCADE
);