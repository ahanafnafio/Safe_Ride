function logoutUser() {
    localStorage.removeItem('userRole');
    localStorage.removeItem('userEmail');
    localStorage.removeItem('sessionID');

    window.location.href = '../../logout.html';
}
