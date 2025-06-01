window.documents = {
    dotNetInstance: null,

    setInstance: function (instance) {
        window.documents.dotNetInstance = instance;
    },

    handleDocumentClickFromBlazor : function (documentId, x, y) {
     
        if (window.documents.dotNetInstance) {
            window.documents.dotNetInstance.invokeMethodAsync('ShowDocumentMenu', documentId, x, y);
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
