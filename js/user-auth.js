let user = JSON.parse(localStorage.getItem('userData'));

if (!user) {
    alert("Виникли проблеми під час входу в акаунт.");
}
else {
    let usernameField = document.getElementById("username-span");
    usernameField.textContent = user.login;
}
