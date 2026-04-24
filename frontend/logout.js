function logoutUser() {
    const sessionId = localStorage.getItem('sessionId');
    fetch('http://localhost:5044/api/auth/logout', {
        method: 'POST',
        headers: {
            "ContentType" : "application/json"
        },
        body: JSON.stringify({ sessionId: sessionId })
    })
    .then(response => {
    localStorage.removeItem('userRole');
    localStorage.removeItem('userEmail');
    localStorage.removeItem('sessionId');
    localStorage.removeItem('firstName');

    window.location.href = '../../logout.html';
})
.catch(error => {
    console.error("Logout error:", error);

    localStorage.removeItem('userRole');
    localStorage.removeItem('userEmail');
    localStorage.removeItem('sessionId');
    localStorage.removeItem('firstName');
    window.location.href = '../../logout.html';
});
}
