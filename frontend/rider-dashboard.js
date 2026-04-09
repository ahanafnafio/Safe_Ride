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

const locationBtn = document.getElementById("locationBtn");
const requestBtn = document.getElementById("requestBtn");

function loadUserName() {
  const storedName = localStorage.getItem("saferideUserName");
  if (storedName && storedName.trim()) {
    accountUser.textContent = storedName;
    welcomeUserName.textContent = storedName;
  }
}

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
}

locationBtn.addEventListener("click", () => {
  pickup.value = "Current Location";
  updatePreview();
});

pickup.addEventListener("input", updatePreview);
destination.addEventListener("input", updatePreview);

requestBtn.addEventListener("click", () => {
  if (
    !pickup.value.trim() ||
    !destination.value.trim() ||
    !vehicle.value.trim() ||
    !pickupTime.value.trim() ||
    !contactNumber.value.trim()
  ) {
    alert("Please complete all trip fields before requesting a driver.");
    return;
  }

  savedVehicle.textContent = vehicle.value.trim();

  status.textContent = "Driver requested";
  driver.textContent = "Searching...";
  eta.textContent = "5-8 min";
  statusBadge.textContent = "Requested";
  activityBadge.textContent = "Trip requested";
  recentActivityText.textContent = `Trip requested from ${pickup.value.trim()} to ${destination.value.trim()}.`;
  tripFormStatus.textContent = "Submitted";

  setTimeout(() => {
    status.textContent = "Driver assigned";
    driver.textContent = "James Carter";
    eta.textContent = "4 min";
    statusBadge.textContent = "Matched";
    activityBadge.textContent = "Driver assigned";
    recentActivityText.textContent = `James Carter is on the way to ${pickup.value.trim()}.`;
  }, 1500);

  setTimeout(() => {
    status.textContent = "En route";
    eta.textContent = "2 min";
    statusBadge.textContent = "En Route";
    activityBadge.textContent = "Driver en route";
    recentActivityText.textContent = `Your driver is en route to ${pickup.value.trim()}.`;
  }, 3500);
});

loadUserName();
updatePreview();
resetRideState();


document.getElementById("logoutBtn").addEventListener("click", () => {
    window.location.href = "logout.html";
  });