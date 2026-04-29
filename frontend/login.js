const loginForm = document.getElementById("loginForm");
const emailInput = document.getElementById("email");
const passwordInput = document.getElementById("password");
const emailError = document.getElementById("emailError");
const passwordError = document.getElementById("passwordError");
const togglePassword = document.getElementById("togglePassword");
const forgotPasswordBtn = document.getElementById("forgotPasswordBtn");
const loginStatus = document.getElementById("loginStatus");
const passwordGroup = passwordInput.closest(".password-group");
const loginButton = loginForm.querySelector(".login-btn");
let isResetMode = false;
const API_BASE_URL = "http://localhost:5044/api";

const themeToggle = document.getElementById("themeToggle");

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

themeToggle.addEventListener("click", function () {
  document.body.classList.toggle("dark-mode");

  const isDarkMode = document.body.classList.contains("dark-mode");
  localStorage.setItem("saferideTheme", isDarkMode ? "dark" : "light");
  themeToggle.textContent = isDarkMode ? "☀️" : "🌙";
});

applySavedTheme();

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
  loginStatus.textContent = "";
  loginStatus.className = "status-message";

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

  if (!isResetMode) {
    if (!password) {
      passwordError.textContent = "Password is required.";
      hasError = true;
    } else if (password.length < 6) {
      passwordError.textContent = "Password must be at least 6 characters.";
      hasError = true;
    }
  }

  if (hasError) return;

  if (isResetMode) {
    loginStatus.className = "status-message status-success";
    loginStatus.textContent = `A reset code has been sent to ${email}.`;
    return;
  }

  loginStatus.textContent = "Checking your account...";

  fetch(`${API_BASE_URL}/Auth/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({
      email: email,
      password: password
    })
  })
    .then(async (response) => {
      const data = await response.json();
      return { ok: response.ok, data };
    })
    .then(({ ok, data }) => {
      console.log("Login response:", data);

      if (!ok || data.success === false || data.error) {
        const message = data.message || "Account not found or password is incorrect.";
        passwordError.textContent = message;
        loginStatus.className = "status-message status-error";
        loginStatus.innerHTML = `No user found under these credentials. <a href="register.html">Register here</a>.`;
        return;
      }

      const sessionId = data.sessionId || data.SessionId || data.sessionID;

      if (sessionId) {
        localStorage.setItem("sessionId", sessionId);
      }

      if (data.firstName) {
        localStorage.setItem("firstName", data.firstName);
        localStorage.setItem("saferideUserName", data.firstName);
      } else {
        localStorage.setItem("saferideUserName", email.split("@")[0] || "User");
      }

      loginStatus.className = "status-message status-success";
      loginStatus.textContent = "Login successful. Redirecting to your dashboard...";

      setTimeout(() => {
        const role = data.role || data.Role;

        if (role === "Driver") {
          window.location.href = "driverDB.html";
        } else {
          window.location.href = "rider-dashboard.html";
        }
      }, 700);
    })
    .catch((error) => {
      console.error("Login error:", error);
      loginStatus.className = "status-message status-error";
      loginStatus.innerHTML = `No user found under these credentials. <a href="register.html">Create account here</a>.`;
    });
});

forgotPasswordBtn.addEventListener("click", function () {
  isResetMode = !isResetMode;

  passwordInput.value = "";
  passwordError.textContent = "";
  loginStatus.className = "status-message status-info";

  if (isResetMode) {
    loginStatus.textContent = "Enter your email address and we will send you a reset code.";

    if (passwordGroup) {
      passwordGroup.style.display = "none";
    }

    loginButton.textContent = "Send Code";
    forgotPasswordBtn.textContent = "Back to login";
    emailInput.focus();
  } else {
    if (passwordGroup) {
      passwordGroup.style.display = "";
    }

    loginButton.textContent = "Log In";
    forgotPasswordBtn.textContent = "Forgot password?";
    loginStatus.textContent = "";
    loginStatus.className = "status-message";
  }
});
