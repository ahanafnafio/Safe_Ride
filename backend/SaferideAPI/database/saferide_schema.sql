-- Script for creating the Users table in the Saferide database
CREATE TABLE Users
(
    userID INTEGER PRIMARY KEY AUTOINCREMENT,
    firstName TEXT NOT NULL,
    lastName TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE, -- sets the constraint that the email atrribute must differ from other entries in the table
    passwordHash TEXT NOT NULL,
    userRole TEXT NOT NULL CHECK(userRole IN ('Rider', 'Driver'))
);