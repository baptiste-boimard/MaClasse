window.documents = {
    dotNetInstance: null,

    setInstance: function (instance) {
        window.documents.dotNetInstance = instance;
    },

    handleDocumentClickFromBlazor : function (documentId, x, y) {
        try {
            if (window.documents.dotNetInstance) {
                window.documents.dotNetInstance.invokeMethodAsync('ShowDocumentMenu', documentId, x, y);
            }
        } catch (err) {
            console.warn("🔴 JS→.NET call failed:", err);
        }
    },

    registerOutsideClick: function () {
        document.addEventListener("mousedown", function (e) {
            try {
                const menu = document.getElementById("custom-context-menu");
                if (menu && !menu.contains(e.target)) {
                    if (window.documents.dotNetInstance) {
                        window.documents.dotNetInstance.invokeMethodAsync("CloseDocumentMenu");
                    }
                }
            } catch (err) {
                console.warn("🔴 JS→.NET call failed (outsideClick):", err);
            }
        });
    }
    
    
};
