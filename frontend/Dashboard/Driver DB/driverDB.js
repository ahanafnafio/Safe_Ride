const driverStatus = document.getElementById("driverStatus");
const accountStatus = document.getElementById("accountStatus");

const availableButton = document.getElementById("availableButton");
const offlineButton = document.getElementById("offlineButton");

const themeToggle = document.getElementById("themeToggle");
themeToggle.addEventListener("click", function() {
    document.body.classList.toggle("darkMode");

    if (document.body.classList.contains("darkMode")) {
        themeToggle.textContent = "Light Mode";
    } else {
        themeToggle.textContent = "Dark Mode";
    }
});

availableButton.addEventListener("click", function() {
    driverStatus.textContent = "Online";
    driverStatus.classList.remove("offline");
    driverStatus.classList.add("online");
    accountStatus.textContent = "Online";
});

offlineButton.addEventListener("click", function() {
    driverStatus.textContent = "Offline";
    driverStatus.classList.remove("online");
    driverStatus.classList.add("offline");
    accountStatus.textContent = "Offline";
});

let acceptButton1 = document.getElementById("acceptButton1");
let declineButton1 = document.getElementById("declineButton1");
let rideStatus1 = document.getElementById("rideStatus1");
let ride1 = document.getElementById("ride1");

let acceptButton2 = document.getElementById("acceptButton2");
let declineButton2 = document.getElementById("declineButton2");
let rideStatus2 = document.getElementById("rideStatus2");
let ride2 = document.getElementById("ride2");

acceptButton1.addEventListener("click", function() {
    rideStatus1.textContent = "Ride Accepted";
    ride1.classList.remove("rideDeclined");
    ride1.classList.add("rideAccepted");
});

declineButton1.addEventListener("click", function() {
    rideStatus1.textContent = "Ride Declined";
    ride1.classList.remove("rideAccepted");
    ride1.classList.add("rideDeclined");
});

acceptButton2.addEventListener("click", function() {
    rideStatus2.textContent = "Ride Accepted";
    ride2.classList.remove("rideDeclined");
    ride2.classList.add("rideAccepted");
});

declineButton2.addEventListener("click", function() {
    rideStatus2.textContent = "Ride Declined";
    ride2.classList.remove("rideAccepted");
    ride2.classList.add("rideDeclined");
});