-- Script for creating the Riders and Drivers tables in the Saferide database
CREATE TABLE IF NOT EXISTS Riders
(
    riderID INTEGER PRIMARY KEY AUTOINCREMENT,
    firstName TEXT NOT NULL,
    lastName TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE, -- sets the constraint that the email atrribute must differ from other entries in the table
    passwordHash TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Drivers
(
    driverID INTEGER PRIMARY KEY AUTOINCREMENT,
    firstName TEXT NOT NULL,
    lastName TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE, -- sets the constraint that the email atrribute must differ from other entries in the table
    passwordHash TEXT NOT NULL
);