const driverStatus = document.getElementById("driverStatus");
const accountStatus = document.getElementById("accountStatus");

const availableButton = document.getElementById("availableButton");
const offlineButton = document.getElementById("offlineButton");
const themeToggle = document.getElementById("themeToggle");

const assignRideButton = document.getElementById("assignRideButton");
const rideActionButton = document.getElementById("rideActionButton");

const passengerName = document.getElementById("passengerName");
const pickupLocation = document.getElementById("pickupLocation");
const destinationLocation = document.getElementById("destinationLocation");
const vehicleInfo = document.getElementById("vehicleInfo");
const rideStatus = document.getElementById("rideStatus");
const mapStatus = document.getElementById("mapStatus");

let map;
let directionsService;
let directionsRenderer;
let currentRideStep = "none";

let currentRide = null;

function getDemoRide() { // Return a hardcoded demo ride for testing purposes
    return {
        passenger: "Ben Nelson",
        pickup: "Lucky Lou's, Denton, TX",
        destination: "Discovery Park, Denton, TX",
        vehicle: "Blue Honda Civic ADB0023"
    };
}

function initMap() { // Initialize the Google Map
    const mapElement = document.getElementById("map");

    if (!mapElement || !window.google || !window.google.maps) {
        return;
    }

    map = new google.maps.Map(mapElement, {
        center: { lat: 33.2148, lng: -97.1331 },
        zoom: 13,
        mapTypeControl: false,
        streetViewControl: false,
        fullscreenControl: false
    });

    directionsService = new google.maps.DirectionsService();

    directionsRenderer = new google.maps.DirectionsRenderer({
        map: map,
        suppressMarkers: false
    });
}

window.initMap = initMap;

function showRoute(origin, destination) { // Display the route on the map
    if (!directionsService || !directionsRenderer) {
        return;
    }

    directionsService.route(
        {
            origin: origin,
            destination: destination,
            travelMode: google.maps.TravelMode.DRIVING
        },
        function(result, statusResult) { // Callback function to handle the directions result
            if (statusResult === "OK") {
                directionsRenderer.setDirections(result);
            } else {
                alert("Route could not be loaded. Check the location names.");
            }
        }
    );
}

function updateMapStatus(text, className) { // Update the map status text
    mapStatus.textContent = text;
    mapStatus.classList.remove("offline");
    mapStatus.classList.remove("online");
    mapStatus.classList.add(className);
}

themeToggle.addEventListener("click", function() { // Toggle dark mode class on body
    document.body.classList.toggle("darkMode");

    if (document.body.classList.contains("darkMode")) {
        themeToggle.textContent = "Light Mode";
    } else {
        themeToggle.textContent = "Dark Mode";
    }
});

availableButton.addEventListener("click", function() { // Set driver status to online and update UI
    driverStatus.textContent = "Online";
    driverStatus.classList.remove("offline");
    driverStatus.classList.add("online");

    accountStatus.textContent = "Online";
});

offlineButton.addEventListener("click", function() { // Set driver status to offline and update UI
    driverStatus.textContent = "Offline";
    driverStatus.classList.remove("online");
    driverStatus.classList.add("offline");

    accountStatus.textContent = "Offline";
});

assignRideButton.addEventListener("click", function() { // Assign a ride to the driver
    if (driverStatus.textContent !== "Online") {
        alert("Go available before accepting a ride.");
        return;
    }

    currentRide = getDemoRide();

    passengerName.textContent = currentRide.passenger;
    pickupLocation.textContent = currentRide.pickup;
    destinationLocation.textContent = currentRide.destination;
    vehicleInfo.textContent = currentRide.vehicle;

    rideStatus.textContent = "Driving to rider pickup location";
    updateMapStatus("To Pickup", "online");

    currentRideStep = "toPickup";

    assignRideButton.disabled = true;
    assignRideButton.classList.add("secondaryBtn");

    rideActionButton.disabled = false;
    rideActionButton.textContent = "Mark Picked Up";
    rideActionButton.classList.remove("secondaryBtn");

    showRoute("410 Avenue C #308j, Denton, TX 76201", currentRide.pickup);
});

rideActionButton.addEventListener("click", function() { // Handle ride progression based on current step
    if (currentRideStep === "toPickup") { // Mark the rider as picked up and update the UI to show route to destination
        currentRideStep = "toDestination";

        rideStatus.textContent = "Rider picked up. Driving to destination.";
        updateMapStatus("To Destination", "online");

        rideActionButton.textContent = "Complete Ride";

        showRoute(currentRide.pickup, currentRide.destination);
    } 
    else if (currentRideStep === "toDestination") {
        currentRideStep = "completed";

        // Show completion FIRST (briefly)
        rideStatus.textContent = "Ride completed. Rider and vehicle dropped off.";
        updateMapStatus("Completed", "online");

        rideActionButton.textContent = "Ride Complete";
        rideActionButton.disabled = true;
        rideActionButton.classList.add("secondaryBtn");

        assignRideButton.disabled = false;
        assignRideButton.classList.remove("secondaryBtn");

        // Reset everything AFTER
        setTimeout(() => {
            resetMap();

            updateMapStatus("Waiting", "offline");

            passengerName.textContent = "Not assigned";
            pickupLocation.textContent = "--";
            destinationLocation.textContent = "--";
            vehicleInfo.textContent = "--";
            rideStatus.textContent = "No active ride";
        }, 1500);
    }
});

    function resetMap() { // Clear the map and reset to default view
        if (!map || !directionsRenderer) 
            return;
        directionsRenderer.setDirections({ routes: [] });
        map.setCenter({ lat: 33.2148, lng: -97.1331 });
        map.setZoom(13);
    }
