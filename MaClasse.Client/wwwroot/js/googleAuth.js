window.initializeGoogleLogin = function (dotNetObjRef, clientId) {
    // Attendre que le SDK Google soit bien dispo
    function waitForGoogleSDK(callback) {
        if (window.google && google.accounts && google.accounts.id) {
            callback();
        } else {
            // Réessaie dans 100ms
            setTimeout(() => waitForGoogleSDK(callback), 100);
        }
    }

    waitForGoogleSDK(() => {
        console.log("ID Client Google utilisé par JavaScript:", clientId);
        google.accounts.id.initialize({
            client_id: clientId,
            callback: (response) => {
                if (dotNetObjRef && typeof dotNetObjRef.invokeMethodAsync === 'function') {
                    dotNetObjRef.invokeMethodAsync('ReceiveGoogleToken', response.credential);
                }
            },
            auto_select: false,
            prompt_parent_id: "google-button",
            itp_support: true
        });

        const button = document.getElementById("google-button");
        if (button) {
            google.accounts.id.renderButton(button, {
                theme: "outline",
                size: "large",
                width: "400"
            });
        }
    });
};
