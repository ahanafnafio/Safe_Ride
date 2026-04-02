function logoutUser() {
    localStorage.removeItem('userRole');
    localStorage.removeItem('userEmail');

    window.location.href = 'login.html';
}
