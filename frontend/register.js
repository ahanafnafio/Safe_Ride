console.log("JS is connected");
document.getElementById("registerForm").addEventListener("submit", function(event){

    event.preventDefault();

    let password = document.getElementById("password").value;
    let confirmPassword = document.getElementById("confirmPassword").value;

    let passwordError = document.getElementById("passwordError");
    let confirmPasswordError = document.getElementById("confirmPasswordError");

    passwordError.textContent = "";
    confirmPasswordError.textContent = "";

    let hasError = false;

    //Check password requirements
    if(password.length < 8){
        passwordError.textContent = "Password must be at least 8 characters long.";
        hasError = true;
    }

    //Check passwords match
    if(password !== confirmPassword){
        confirmPasswordError.textContent = "Passwords do not match.";
        hasError = true;
    }

    if(hasError){
        return;
    }

    const payload = {
      firstName: document.getElementById("firstName").value.trim(),
      lastName: document.getElementById("lastName").value.trim(),
      email: document.getElementById("email").value.trim(),
      password: password,
      role: document.querySelector('input[name="userRole"]:checked').value
    };

    fetch("http://localhost:5044/api/auth/register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(payload)
    })
      .then(async (res) => {
        const text = await res.text();
        if (!res.ok) {
          throw new Error(text);
        }
        console.log("Success:", text);
      })
      .catch((err) => {
        console.error("Error:", err.message);
    });
  });

const roleInputs = document.querySelectorAll('input[name="userRole"]');
const driverFields = document.getElementById("driverFields");

roleInputs.forEach((input) => {
  input.addEventListener("change", function () {
    if (this.value === "Driver") {
      driverFields.classList.remove("hidden");
    } else {
      driverFields.classList.add("hidden");
    }
  });
});

const themeToggle = document.getElementById("themeToggle");

themeToggle.addEventListener("click", function() {
  document.body.classList.toggle("darkMode");

  if (document.body.classList.contains("darkMode")) {
    themeToggle.textContent = "Light Mode";
  } else {
    themeToggle.textContent = "Dark Mode";
  }
});