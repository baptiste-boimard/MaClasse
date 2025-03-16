window.initializeGoogleSignIn = function () {
    google.accounts.id.initialize({
        client_id: 'YOUR_CLIENT_ID',
        callback: handleCredentialResponse
    });
    google.accounts.id.renderButton(
        document.querySelector(".g_id_signin"),
        { theme: "outline", size: "large" }
    );
    google.accounts.id.prompt();
};

function handleCredentialResponse(response) {
    fetch('/signin-google', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: 'credential=' + response.credential
    })
        .then(response => {
            if (response.ok) {
                window.location.href = '/';
            } else {
                console.error('Erreur de connexion Google');
            }
        });
}