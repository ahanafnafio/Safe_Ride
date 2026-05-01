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
const arrivedBtn = document.getElementById("arrivedBtn");
const completeBtn = document.getElementById("completeBtn");
const themeToggle = document.getElementById("themeToggle");
const dashboardStatusMessage = document.getElementById("dashboardStatusMessage");
const accountBtn = document.getElementById("accountBtn");
const accountDropdown = document.getElementById("accountDropdown");
const dropdownUserName = document.getElementById("dropdownUserName");
const dropdownVehicle = document.getElementById("dropdownVehicle");
const logoutBtn = document.getElementById("logoutBtn");

let map;
let directionsService;
let directionsRenderer;

const API_BASE_URL = "http://localhost:5044/api";

function getSessionId() {
  let sessionId = localStorage.getItem("sessionId");

  if (!sessionId) {
    sessionId = `demo-session-${Date.now()}`;
    localStorage.setItem("sessionId", sessionId);
  }

  return sessionId;
}

async function addVehicleToApi(vehicleItem) {
  try {
    const response = await fetch(`${API_BASE_URL}/Match/vehicle/add`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        sessionId: getSessionId(),
        vehicleId: vehicleItem.vehicleId,
        name: vehicleItem.name
      })
    });

    if (!response.ok) {
      throw new Error("Vehicle API request failed");
    }

    return await response.json();
  } catch (error) {
    console.warn("Vehicle API unavailable. Vehicle saved locally for demo.", error);
    return null;
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

function updateAccountDropdown() {
  const storedName = localStorage.getItem("saferideUserName") || localStorage.getItem("firstName") || "User";
  const selectedVehicle = getSelectedVehicleObject();
  const vehicleName = selectedVehicle ? selectedVehicle.name : savedVehicle.textContent || "Not set";

  dropdownUserName.textContent = storedName;
  dropdownVehicle.textContent = vehicleName;
}

function getSavedVehicles() {
  return JSON.parse(localStorage.getItem("saferideVehicles")) || [];
}

function saveVehicles(vehicles) {
  localStorage.setItem("saferideVehicles", JSON.stringify(vehicles));
}

function renderSavedVehicles() {
  const vehicles = getSavedVehicles();

  savedVehicleSelect.innerHTML = '<option value="">No saved vehicle selected</option>';

  vehicles.forEach((vehicleItem) => {
    const option = document.createElement("option");
    option.value = vehicleItem.vehicleId;
    option.textContent = vehicleItem.name;
    savedVehicleSelect.appendChild(option);
  });
}

function getSelectedVehicleObject() {
  const vehicles = getSavedVehicles();
  return vehicles.find((vehicleItem) => vehicleItem.vehicleId === savedVehicleSelect.value);
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

accountBtn.addEventListener("click", () => {
  updateAccountDropdown();
  accountDropdown.classList.toggle("show");

  const isOpen = accountDropdown.classList.contains("show");
  accountBtn.setAttribute("aria-expanded", isOpen ? "true" : "false");
  announceDashboardUpdate(isOpen ? "Account settings opened." : "Account settings closed.");
});

document.addEventListener("click", (event) => {
  if (!accountDropdown.contains(event.target) && !accountBtn.contains(event.target)) {
    accountDropdown.classList.remove("show");
    accountBtn.setAttribute("aria-expanded", "false");
  }
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

addVehicleBtn.addEventListener("click", () => {
  const vehicleName = vehicle.value.trim();

  if (!vehicleName) {
    vehicleFeedback.textContent = "Enter a vehicle before adding it.";
    announceDashboardUpdate("Enter a vehicle before adding it.");
    return;
  }

  const vehicles = getSavedVehicles();
  const newVehicle = {
    vehicleId: `vehicle-${Date.now()}`,
    sessionId: getSessionId(),
    name: vehicleName,
    createdAt: new Date().toISOString()
  };

  vehicles.push(newVehicle);
  saveVehicles(vehicles);
  renderSavedVehicles();

  savedVehicleSelect.value = newVehicle.vehicleId;
  savedVehicle.textContent = newVehicle.name;
  updateAccountDropdown();
  vehicleFeedback.textContent = `${newVehicle.name} added to saved vehicles.`;
  announceDashboardUpdate(`${newVehicle.name} added to saved vehicles.`);
  addVehicleToApi(newVehicle);
});

savedVehicleSelect.addEventListener("change", () => {
  const selectedVehicle = getSelectedVehicleObject();

  if (selectedVehicle) {
    vehicle.value = selectedVehicle.name;
    savedVehicle.textContent = selectedVehicle.name;
    vehicleFeedback.textContent = `${selectedVehicle.name} selected for this ride.`;
    announceDashboardUpdate(`${selectedVehicle.name} selected for this ride.`);
  } else {
    savedVehicle.textContent = "Not set";
    vehicleFeedback.textContent = "No saved vehicle selected.";
  }
  updateAccountDropdown();
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

requestBtn.addEventListener("click", () => {
  const pickupValue = pickup.value.trim();
  const destinationValue = destination.value.trim();
  const vehicleValue = vehicle.value.trim();
  const selectedVehicle = getSelectedVehicleObject();
  const selectedVehicleId = selectedVehicle ? selectedVehicle.vehicleId : null;
  const pickupTimeValue = pickupTime.value.trim();
  const contactValue = contactNumber.value.trim();

  if (
    !pickupValue ||
    !destinationValue ||
    !vehicleValue ||
    !pickupTimeValue ||
    !contactValue
  ) {
    alert("Please complete all trip fields before requesting a driver.");
    announceDashboardUpdate("Please complete all trip fields before requesting a driver.");
    return;
  }

  savedVehicle.textContent = selectedVehicle ? selectedVehicle.name : vehicleValue;
  updateAccountDropdown();

  const rideRequest = {
    sessionId: getSessionId(),
    pickup: pickupValue,
    destination: destinationValue,
    vehicle: selectedVehicle ? selectedVehicle.name : vehicleValue,
    vehicleId: selectedVehicleId,
    pickupTime: pickupTimeValue,
    contact: contactValue,
    status: "Requested",
    driver: "Not assigned",
    eta: "5-8 min"
  };

  localStorage.setItem("saferideRideRequest", JSON.stringify(rideRequest));
  requestRideFromApi(rideRequest);

  if (mapText) {
    mapText.textContent = `Route: ${pickupValue} → ${destinationValue}`;
  }

  if (mapSubtext) {
    mapSubtext.textContent = "Generating route preview and driver match...";
  }

  if (directionsService && directionsRenderer) {
    directionsService.route(
      {
        origin: pickupValue,
        destination: destinationValue,
        travelMode: google.maps.TravelMode.DRIVING
      },
      (result, statusResult) => {
        if (statusResult === "OK") {
          directionsRenderer.setDirections(result);

          const bounds = new google.maps.LatLngBounds();
          result.routes[0].legs.forEach((leg) => {
            bounds.extend(leg.start_location);
            bounds.extend(leg.end_location);
          });
          map.fitBounds(bounds);

          if (mapSubtext) {
            mapSubtext.textContent = "Live route loaded on map.";
          }
        } else {
          if (mapSubtext) {
            mapSubtext.textContent = "Route preview unavailable. Check location input.";
          }
        }
      }
    );
  }

  requestBtn.disabled = true;
  requestBtn.textContent = "Finding Driver...";
  announceDashboardUpdate("Ride request submitted. Finding a driver.");

  status.textContent = "Driver requested";
  driver.textContent = "Searching...";
  eta.textContent = "5-8 min";
  statusBadge.textContent = "Requested";
  activityBadge.textContent = "Trip requested";
  recentActivityText.textContent = `Trip requested from ${pickupValue} to ${destinationValue}.`;
  tripFormStatus.textContent = "Submitted";

  setTimeout(() => {
    status.textContent = "Driver assigned";
    driver.textContent = "James Carter";
    eta.textContent = "4 min";
    statusBadge.textContent = "Matched";
    activityBadge.textContent = "Driver assigned";
    recentActivityText.textContent = `James Carter is on the way to ${pickupValue}.`;
    announceDashboardUpdate("Driver assigned. James Carter is on the way.");

    if (mapSubtext) {
      mapSubtext.textContent = "Driver matched. Route active on map preview.";
    }
  }, 1500);

  setTimeout(() => {
    status.textContent = "En route";
    eta.textContent = "2 min";
    statusBadge.textContent = "En Route";
    activityBadge.textContent = "Driver en route";
    recentActivityText.textContent = `Your driver is en route to ${pickupValue}.`;

    requestBtn.textContent = "Driver Requested";
    requestBtn.disabled = false;
    announceDashboardUpdate("Driver is en route. Estimated arrival is 2 minutes.");
  }, 3500);
});

arrivedBtn.addEventListener("click", () => {
  status.textContent = "Driver arrived";
  eta.textContent = "Arrived";
  statusBadge.textContent = "Arrived";
  activityBadge.textContent = "Driver arrived";
  recentActivityText.textContent = "Your driver has arrived at the pickup location.";

  const rideRequest = JSON.parse(localStorage.getItem("saferideRideRequest")) || {};
  rideRequest.status = "Driver arrived";
  rideRequest.eta = "Arrived";
  localStorage.setItem("saferideRideRequest", JSON.stringify(rideRequest));

  announceDashboardUpdate("Driver has arrived at the pickup location.");
});

completeBtn.addEventListener("click", () => {
  status.textContent = "Ride completed";
  eta.textContent = "Completed";
  statusBadge.textContent = "Completed";
  activityBadge.textContent = "Ride completed";
  recentActivityText.textContent = "Ride completed successfully.";
  requestBtn.textContent = "Request Driver";
  requestBtn.disabled = false;

  const rideRequest = JSON.parse(localStorage.getItem("saferideRideRequest")) || {};
  rideRequest.status = "Ride completed";
  rideRequest.eta = "Completed";
  localStorage.setItem("saferideRideRequest", JSON.stringify(rideRequest));

  announceDashboardUpdate("Ride completed successfully.");
  window.location.href = "rating.html";
});

renderSavedVehicles();
applySavedTheme();
loadUserName();
updateAccountDropdown();

logoutBtn.addEventListener("click", () => {
  window.location.href = "logout.html";
});
