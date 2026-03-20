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

    alert("Account created successfully!");

});