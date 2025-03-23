function initializeGoogleLogin(dotNetObjRef, clientId) {
        if (!window.google) {
        console.error("Google SDK not loaded!");
        return;
    }

    google.accounts.id.initialize({
        client_id: clientId,
        callback: (response) => {
            dotNetObjRef.invokeMethodAsync('ReceiveGoogleToken', response.credential);
        },
        auto_select: false,  // 🔹 Désactive la connexion automatique
        prompt: "select_account",  // 🔹 Force la sélection du compte
        itp_support: true // Active FedCM pour une meilleure intégration


    });

    google.accounts.id.renderButton(
        document.getElementById("google-button"),
        { theme: "outline", size: "large", width: "400" }
    );

    google.accounts.id.prompt(); // 🔹 Ouvre le pop-up de sélection de compte si possible

}

function openGoogleAccountMenu() {
    window.open(
        // "https://accounts.google.com/SignOutOptions?hl=fr&continue=https://myaccount.google.com/%3Futm_source%3Dsign_in_no_continue&ec=GBRAwAE",
        // "https://accounts.google.com/AccountChooser?hl=fr",
        // "https://accounts.google.com/SignOutOptions?hl=fr",
        "GoogleAccountChooser",
        "width=500,height=600"
    );
}
window.onload = function () {
    google.accounts.id.renderButton(
        document.querySelector(".g_id_signin"),
        { theme: "outline", size: "large" } // Personnalisation
    );
    console.log("Bouton Google initialisé");
};