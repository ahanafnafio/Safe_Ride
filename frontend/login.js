const loginForm = document.getElementById("loginForm");
const emailInput = document.getElementById("email");
const passwordInput = document.getElementById("password");
const emailError = document.getElementById("emailError");
const passwordError = document.getElementById("passwordError");
const togglePassword = document.getElementById("togglePassword");

togglePassword.addEventListener("click", function () {
  const isPassword = passwordInput.type === "password";
  passwordInput.type = isPassword ? "text" : "password";
  togglePassword.textContent = isPassword ? "Hide" : "Show";
});

function isValidEmail(email) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

loginForm.addEventListener("submit", function (e) {
  e.preventDefault();

  emailError.textContent = "";
  passwordError.textContent = "";

  const email = emailInput.value.trim();
  const password = passwordInput.value.trim();

  let hasError = false;

  if (!email) {
    emailError.textContent = "Email is required.";
    hasError = true;
  } else if (!isValidEmail(email)) {
    emailError.textContent = "Enter a valid email address.";
    hasError = true;
  }

  if (!password) {
    passwordError.textContent = "Password is required.";
    hasError = true;
  } else if (password.length < 6) {
    passwordError.textContent = "Password must be at least 6 characters.";
    hasError = true;
  }

  if (hasError) return;

  console.log("Login submitted:", { email, password });

  // Later this is where you'll connect to backend
  alert("Login submitted successfully.");
});