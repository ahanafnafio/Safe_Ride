const pickup = document.getElementById("pickup");
const destination = document.getElementById("destination");
const vehicle = document.getElementById("vehicle");
const addVehicleBtn = document.getElementById("addVehicleBtn");
const savedVehicleSelect = document.getElementById("savedVehicleSelect");
const vehicleFeedback = document.getElementById("vehicleFeedback");
const pickupTime = document.getElementById("pickupTime");
const contactNumber = document.getElementById("contactNumber");

const previewPickup = document.getElementById("previewPickup");
const previewDestination = document.getElementById("previewDestination");
const previewStatus = document.getElementById("previewStatus");
const tripFormStatus = document.getElementById("tripFormStatus");

const status = document.getElementById("status");
const driver = document.getElementById("driver");
const eta = document.getElementById("eta");
const statusBadge = document.getElementById("statusBadge");

const savedVehicle = document.getElementById("savedVehicle");
const accountUser = document.getElementById("accountUser");
const welcomeUserName = document.getElementById("welcomeUserName");
const activityBadge = document.getElementById("activityBadge");
const recentActivityText = document.getElementById("recentActivityText");
const mapText = document.querySelector(".map-overlay-text p");
const mapSubtext = document.querySelector(".map-overlay-text span");

const locationBtn = document.getElementById("locationBtn");
const requestBtn = document.getElementById("requestBtn");
const themeToggle = document.getElementById("themeToggle");
const dashboardStatusMessage = document.getElementById("dashboardStatusMessage");

const arrivedBtn = document.getElementById("arrivedBtn");
const completeBtn = document.getElementById("completeBtn");

let map;
let directionsService;
let directionsRenderer;
let routeLine;
let geocoder;
let currentRideId = null;

const API_BASE_URL = "http://localhost:5044/api";

function getSessionId() {
  let sessionId = localStorage.getItem("sessionId");

  if (!sessionId) {
    sessionId = `demo-session-${Date.now()}`;
    localStorage.setItem("sessionId", sessionId);
  }

  return sessionId;
}

async function addVehicleToApi(vehicleName) {
  try {
    const response = await fetch(`${API_BASE_URL}/Match/vehicle/add`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        sessionId: getSessionId(),
        make: vehicleName,
        model: "",
        color: "",
        plate: "",
        notes: ""
      })
    });

    if (!response.ok) {
      throw new Error("Vehicle API request failed");
    }

    return await response.json();
  } catch (error) {
    console.warn("Vehicle API unavailable.", error);
    return null;
  }
}

async function getVehiclesFromApi() {
  try {
    const response = await fetch(
      `${API_BASE_URL}/Match/myvehicles?sessionId=${getSessionId()}`
    );

    if (!response.ok) {
      throw new Error("Failed to load vehicles");
    }

    return await response.json();
  } catch (error) {
    console.error("Failed to fetch vehicles:", error);
    return [];
  }
}

async function requestRideFromApi(rideRequest) {
  try {
    const response = await fetch(`${API_BASE_URL}/Match/ride/request`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(rideRequest)
    });

    if (!response.ok) {
      throw new Error("Ride request API failed");
    }

    return await response.json();
  } catch (error) {
    console.warn("Ride request API unavailable. Ride saved locally for demo.", error);
    return null;
  }
}

function loadUserName() {
  const storedName = localStorage.getItem("saferideUserName");
  if (storedName && storedName.trim()) {
    accountUser.textContent = storedName;
    welcomeUserName.textContent = storedName;
  }
}

function getSavedVehicles() {
  return JSON.parse(localStorage.getItem("saferideVehicles")) || [];
}

function saveVehicles(vehicles) {
  localStorage.setItem("saferideVehicles", JSON.stringify(vehicles));
}

async function renderSavedVehicles() {
  const vehicles = await getVehiclesFromApi();

  savedVehicleSelect.innerHTML =
    '<option value="">No saved vehicle selected</option>';

  vehicles.forEach((vehicleItem) => {
    const option = document.createElement("option");

    option.value = vehicleItem.vehicleId;
    option.textContent = vehicleItem.make;

    savedVehicleSelect.appendChild(option);
  });
}

async function getSelectedVehicleObject() {
  const vehicles = await getVehiclesFromApi();

  return vehicles.find(
    (vehicleItem) => vehicleItem.vehicleId == savedVehicleSelect.value
  );
}

function applySavedTheme() {
  const savedTheme = localStorage.getItem("saferideTheme");

  if (savedTheme === "dark") {
    document.body.classList.add("dark-mode");
    themeToggle.textContent = "☀️";
  } else {
    document.body.classList.remove("dark-mode");
    themeToggle.textContent = "🌙";
  }
}

function announceDashboardUpdate(message) {
  if (dashboardStatusMessage) {
    dashboardStatusMessage.textContent = message;
  }
}

themeToggle.addEventListener("click", () => {
  document.body.classList.toggle("dark-mode");

  const isDarkMode = document.body.classList.contains("dark-mode");
  localStorage.setItem("saferideTheme", isDarkMode ? "dark" : "light");
  themeToggle.textContent = isDarkMode ? "☀️" : "🌙";

  announceDashboardUpdate(isDarkMode ? "Dark mode enabled." : "Light mode enabled.");
});

function initMap() {
  const mapElement = document.getElementById("map");

  if (!mapElement || !window.google || !window.google.maps) {
    return;
  }

  map = new google.maps.Map(mapElement, {
    center: { lat: 32.7767, lng: -96.7970 },
    zoom: 11,
    mapTypeControl: false,
    streetViewControl: false,
    fullscreenControl: false
  });

  directionsService = new google.maps.DirectionsService();
  directionsRenderer = new google.maps.DirectionsRenderer({
    map,
    suppressMarkers: false
  });

  mapElement.classList.add("map-ready");

  if (mapText) {
    mapText.textContent = "Enter a pickup and destination to preview your route";
  }

  if (mapSubtext) {
    mapSubtext.textContent = "Map connected. Waiting for trip details.";
  }
  geocoder = new google.maps.Geocoder();
}

function geocodeAddress(address) {
  return new Promise((resolve, reject) => {
    geocoder.geocode({ address: address }, (results, statusResult) => {
      if (statusResult === "OK" && results[0]) {
        resolve({
          address: results[0].formatted_address,
          lat: results[0].geometry.location.lat(),
          lon: results[0].geometry.location.lng()
        });
      } else {
        reject(`Could not geocode address: ${address}`);
      }
    });
  });
}

window.initMap = initMap;

function updatePreview() {
  previewPickup.textContent = pickup.value.trim() || "Not set";
  previewDestination.textContent = destination.value.trim() || "Not set";

  if (pickup.value.trim() || destination.value.trim()) {
    previewStatus.textContent = "Preview Ready";
    tripFormStatus.textContent = "Editing";
  } else {
    previewStatus.textContent = "Waiting";
    tripFormStatus.textContent = "Ready";
  }
}

function resetRideState() {
  status.textContent = "No active ride";
  driver.textContent = "Not assigned";
  eta.textContent = "--";
  statusBadge.textContent = "Inactive";
  activityBadge.textContent = "No activity";
  recentActivityText.textContent = "No recent ride requests yet.";

  if (mapText) {
    mapText.textContent = "Enter a pickup and destination to preview your route";
  }

  if (mapSubtext) {
    mapSubtext.textContent = "Map connected. Waiting for trip details.";
  }
}

addVehicleBtn.addEventListener("click", async () => {
  const vehicleName = vehicle.value.trim();

  if (!vehicleName) {
    vehicleFeedback.textContent = "Enter a vehicle before adding it.";
    announceDashboardUpdate("Enter a vehicle before adding it.");
    return;
  }

  const result = await addVehicleToApi(vehicleName);

  if (result == null) {
    vehicleFeedback.textContent = "Vehicle could not be added.";
    announceDashboardUpdate("Vehicle could not be added.");
    return;
  }

  vehicleFeedback.textContent = `${vehicleName} added to saved vehicles.`;
  savedVehicle.textContent = vehicleName;
  announceDashboardUpdate(`${vehicleName} added to saved vehicles.`);

  await renderSavedVehicles();
});

savedVehicleSelect.addEventListener("change", async () => {
  const selectedVehicle = await getSelectedVehicleObject();

  if (selectedVehicle) {
    vehicle.value = selectedVehicle.make;
    savedVehicle.textContent = selectedVehicle.make;
    vehicleFeedback.textContent = `${selectedVehicle.make} selected for this ride.`;
    announceDashboardUpdate(`${selectedVehicle.make} selected for this ride.`);
  } else {
    savedVehicle.textContent = "Not set";
    vehicleFeedback.textContent = "No saved vehicle selected.";
  }
});

locationBtn.addEventListener("click", () => {
  if (!navigator.geolocation) {
    alert("Geolocation is not supported by your browser.");
    return;
  }

  locationBtn.disabled = true;
  locationBtn.textContent = "Getting Location...";
  announceDashboardUpdate("Getting your current location.");

  navigator.geolocation.getCurrentPosition(
    (position) => {
      const userLocation = {
        lat: position.coords.latitude,
        lng: position.coords.longitude
      };

      pickup.value = `${userLocation.lat}, ${userLocation.lng}`;
      updatePreview();

      if (map) {
        map.setCenter(userLocation);
        map.setZoom(14);

        new google.maps.Marker({
          position: userLocation,
          map,
          title: "Your Current Location"
        });
      }

      if (mapText) {
        mapText.textContent = "Pickup set to your current location";
      }

      if (mapSubtext) {
        mapSubtext.textContent = "Enter a destination to preview your route.";
      }

      locationBtn.disabled = false;
      locationBtn.textContent = "Use Current Location";
      announceDashboardUpdate("Pickup location updated to your current location.");
    },
    () => {
      alert("Location access was denied or unavailable.");
      locationBtn.disabled = false;
      locationBtn.textContent = "Use Current Location";
      announceDashboardUpdate("Location access was denied or unavailable.");
    }
  );
});

pickup.addEventListener("input", updatePreview);
destination.addEventListener("input", updatePreview);

requestBtn.addEventListener("click", async () => {
  const pickupValue = pickup.value.trim();
  const destinationValue = destination.value.trim();
  const vehicleValue = vehicle.value.trim();
  const pickupTimeValue = pickupTime.value.trim();
  const contactValue = contactNumber.value.trim();
  if (!pickupValue || !destinationValue || !vehicleValue || !pickupTimeValue || !contactValue) {
    alert("Please complete all trip fields before requesting a driver.");
    return;
  }

  requestBtn.disabled = true;
  requestBtn.textContent = "Finding Driver...";

  status.textContent = "Driver requested";
  driver.textContent = "Searching...";
  eta.textContent = "--";
  statusBadge.textContent = "Requested";

  let pickupLocation;
  let dropoffLocation;

  try {
    pickupLocation = await geocodeAddress(pickupValue);
    dropoffLocation = await geocodeAddress(destinationValue);
  } catch (err) {
    alert("Could not find one of the addresses.");
    console.error(err);
    requestBtn.disabled = false;
    requestBtn.textContent = "Request Driver";
    return;
  }

  console.log("Geocoded pickup:", pickupLocation);
  console.log("Geocoded dropoff:", dropoffLocation);

  const rideRequest = {
    sessionId: getSessionId(),

    pickupAddress: pickupLocation.address,
    pickupLat: pickupLocation.lat,
    pickupLon: pickupLocation.lon,

    dropoffAddress: dropoffLocation.address,
    dropoffLat: dropoffLocation.lat,
    dropoffLon: dropoffLocation.lon,

    notes: `Pickup time: ${pickupTimeValue}. Contact: ${contactValue}`,
    vehicleId: 1
  };

console.log("Sending ride request:", rideRequest);

const result = await requestRideFromApi(rideRequest);

if (!result) {
  alert("Ride request failed.");
  requestBtn.disabled = false;
  requestBtn.textContent = "Request Driver";
  return;
}

console.log("Backend ride result:", result);
console.log("Backend encodedPolyline:", result.encodedPolyline);

currentRideId = result.rideId;
console.log("Stored rideId:", currentRideId);


if (result.encodedPolyline && google.maps.geometry) {
  console.log("Drawing route from BACKEND polyline");

  if (routeLine) {
    routeLine.setMap(null);
  }

  const decodedPath = google.maps.geometry.encoding.decodePath(result.encodedPolyline);

  routeLine = new google.maps.Polyline({
    path: decodedPath,
    map: map,
    strokeWeight: 5,
    strokeColor: "#4285F4",
    strokeOpacity: 1.0
  });

  const bounds = new google.maps.LatLngBounds();
  decodedPath.forEach(point => bounds.extend(point));
  map.fitBounds(bounds);

  if (directionsRenderer) {
    directionsRenderer.setDirections({ routes: [] });
  }
}

  status.textContent = result.rideStatus;
  driver.textContent = `${result.driverFirstName} ${result.driverLastName}`;
  eta.textContent = `${Math.round(result.driverEtaSeconds / 60)} min`;
  statusBadge.textContent = "Matched";

  requestBtn.disabled = false;
  requestBtn.textContent = "Driver Requested";
});

renderSavedVehicles();
applySavedTheme();
loadUserName();
updatePreview();
resetRideState();


document.getElementById("logoutBtn").addEventListener("click", () => {
  window.location.href = "logout.html";
});

arrivedBtn.addEventListener("click", async () => {
  if (!currentRideId) {
    alert("No active ride yet.");
    return;
  }

  const response = await fetch(`${API_BASE_URL}/Match/ride/arrived`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ rideId: currentRideId })
  });

  const result = await response.json();

  if (routeLine) {
    routeLine.setMap(null);
  }

  const decodedPath = google.maps.geometry.encoding.decodePath(result.encodedPolyline);

  routeLine = new google.maps.Polyline({
    path: decodedPath,
    map: map,
    strokeWeight: 5,
    strokeColor: "#4285F4",
    strokeOpacity: 1.0

  });

  const bounds = new google.maps.LatLngBounds();
  decodedPath.forEach(point => bounds.extend(point));
  map.fitBounds(bounds);

  status.textContent = result.rideStatus;
  statusBadge.textContent = "In Progress";
  const seconds = parseInt(result.routeDuration.replace("s", ""));
  const minutes = Math.round(seconds / 60);
  eta.textContent = `${minutes} min`;
});

completeBtn.addEventListener("click", async () => {
  if (!currentRideId) {
    alert("No active ride yet.");
    return;
  }

  const response = await fetch(`${API_BASE_URL}/Match/ride/complete`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ rideId: currentRideId })
  });

  const result = await response.json();

  status.textContent = result.rideStatus;
  statusBadge.textContent = "Completed";

  window.location.href = "rating.html";
});