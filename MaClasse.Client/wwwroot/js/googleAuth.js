function initializeGoogeLogin(dotNetObjRef, clientId) {
    if (!window.google) {
        console.error("Google SDK not loaded!");
        return;
    }

    google.accounts.id.initialize({
        client_id: "419056052171-4stscg6up8etnu68m5clp4gi0m3im8ea.apps.googleusercontent.com",
        callback: (response) => {
            dotNetObjRef.invokeMethodAsync('ReceiveGoogleToken', response.credential);
        }
    });

    google.accounts.id.renderButton(
        document.getElementById("google-button"),
        { theme: "outline", size: "large" }
    );
}