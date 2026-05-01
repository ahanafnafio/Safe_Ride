const driverName = document.getElementById("driverName");
const ratingPickup = document.getElementById("ratingPickup");
const ratingDestination = document.getElementById("ratingDestination");
const starButtons = document.querySelectorAll(".starBtn");
const ratingText = document.getElementById("ratingText");
const ratingComment = document.getElementById("ratingComment");
const submitRatingBtn = document.getElementById("submitRatingBtn");
const ratingMessage = document.getElementById("ratingMessage");

let selectedRating = 0;

const rideRequest = JSON.parse(localStorage.getItem("saferideRideRequest")) || {};

driverName.textContent = rideRequest.driver || "Kendra M.";
ratingPickup.textContent = rideRequest.pickup || "Not set";
ratingDestination.textContent = rideRequest.destination || "Not set";

starButtons.forEach((button) => {
  button.addEventListener("click", () => {
    selectedRating = Number(button.dataset.rating);

    starButtons.forEach((star) => {
      const starValue = Number(star.dataset.rating);
      star.classList.toggle("selected", starValue <= selectedRating);
    });

    ratingText.textContent = `${selectedRating} out of 5 stars selected`;
    ratingMessage.textContent = "";
  });
});

submitRatingBtn.addEventListener("click", () => {
  if (selectedRating === 0) {
    ratingMessage.textContent = "Please select a rating before submitting.";
    return;
  }

const ratingData = {
  sessionId: rideRequest.sessionId,
  driver:
    rideRequest.driver ||
    `${rideRequest.driverFirstName || ""} ${rideRequest.driverLastName || ""}`.trim() ||
    "Kendra M.",
  pickup: rideRequest.pickup || rideRequest.pickupAddress,
  destination: rideRequest.destination || rideRequest.dropoffAddress,
  rating: selectedRating,
  comment: ratingComment.value.trim(),
  submittedAt: new Date().toISOString()
};

  localStorage.setItem("saferideDriverRating", JSON.stringify(ratingData));

  ratingMessage.textContent = "Thank you! Your rating has been submitted.";

  setTimeout(() => {
    window.location.href = "rider-dashboard.html";
  }, 1200);
});