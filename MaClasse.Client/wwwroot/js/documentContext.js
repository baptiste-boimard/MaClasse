window.documents = {
    dotNetInstance: null,

    setInstance: function (instance) {
        window.documents.dotNetInstance = instance;
    },

    handleDocumentClickFromBlazor : function (documentId, x, y) {
        const clamp = (value, min, max) => Math.max(min, Math.min(value, max));
        const margin = 8;
        const menu = document.getElementById("custom-context-menu");
        const menuRect = menu ? menu.getBoundingClientRect() : null;
        const menuWidth = Math.ceil(menuRect?.width ?? 180);
        const menuHeight = Math.ceil(menuRect?.height ?? 140);
        const maxX = Math.max(margin, window.innerWidth - menuWidth - margin);
        const maxY = Math.max(margin, window.innerHeight - menuHeight - margin);
        const safeX = Math.round(clamp(x, margin, maxX));
        const safeY = Math.round(clamp(y, margin, maxY));

        if (window.documents.dotNetInstance) {
            window.documents.dotNetInstance.invokeMethodAsync('ShowDocumentMenu', documentId, safeX, safeY);
        }
    
    },

    registerOutsideClick: function () {
        document.addEventListener("mousedown", function (e) {
        
            const menu = document.getElementById("custom-context-menu");
            if (menu && !menu.contains(e.target)) {
                if (window.documents.dotNetInstance) {
                    window.documents.dotNetInstance.invokeMethodAsync("CloseDocumentMenu");
                }
            }
       
        });
    }
    
    
};
