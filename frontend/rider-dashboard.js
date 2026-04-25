const pickup = document.getElementById("pickup");
const destination = document.getElementById("destination");
const vehicle = document.getElementById("vehicle");
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

let map;
let directionsService;
let directionsRenderer;

function loadUserName() {
  const storedName = localStorage.getItem("saferideUserName");
  if (storedName && storedName.trim()) {
    accountUser.textContent = storedName;
    welcomeUserName.textContent = storedName;
  }
}

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

locationBtn.addEventListener("click", () => {
  if (!navigator.geolocation) {
    alert("Geolocation is not supported by your browser.");
    return;
  }

  locationBtn.disabled = true;
  locationBtn.textContent = "Getting Location...";

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
    },
    () => {
      alert("Location access was denied or unavailable.");
      locationBtn.disabled = false;
      locationBtn.textContent = "Use Current Location";
    }
  );
});

pickup.addEventListener("input", updatePreview);
destination.addEventListener("input", updatePreview);

requestBtn.addEventListener("click", () => {
  const pickupValue = pickup.value.trim();
  const destinationValue = destination.value.trim();
  const vehicleValue = vehicle.value.trim();
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
    return;
  }

  savedVehicle.textContent = vehicleValue;

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
  }, 3500);
});

loadUserName();
updatePreview();
resetRideState();


document.getElementById("logoutBtn").addEventListener("click", () => {
  window.location.href = "logout.html";
});
