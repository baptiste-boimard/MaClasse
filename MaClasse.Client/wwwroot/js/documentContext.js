window.documents = {
    dotNetInstance: null,

    setInstance: function (instance) {
        window.documents.dotNetInstance = instance;
    },

    enableHorizontalWheel: function () {
        if (window.documents.__horizontalWheelHandler) {
            return;
        }

        window.documents.__horizontalWheelHandler = function (e) {
            const scroller = e.target.closest(".file-explorer-list-wrapper");
            if (!scroller) {
                return;
            }

            const canScrollHorizontally = scroller.scrollWidth > scroller.clientWidth;
            if (!canScrollHorizontally) {
                return;
            }

            const delta = Math.abs(e.deltaY) >= Math.abs(e.deltaX) ? e.deltaY : e.deltaX;
            if (delta === 0) {
                return;
            }

            scroller.scrollLeft += delta;
            e.preventDefault();
        };

        document.addEventListener("wheel", window.documents.__horizontalWheelHandler, { passive: false, capture: true });
    },

    disableHorizontalWheel: function () {
        if (!window.documents.__horizontalWheelHandler) {
            return;
        }

        document.removeEventListener("wheel", window.documents.__horizontalWheelHandler, { capture: true });
        delete window.documents.__horizontalWheelHandler;
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
        
            const menu = document.querySelector(".file-explorer-context-menu");
            if (menu && !menu.contains(e.target)) {
                if (window.documents.dotNetInstance) {
                    window.documents.dotNetInstance.invokeMethodAsync("CloseDocumentMenu");
                }
            }
       
        });
    },

    focusContextMenuFirstItem: function (firstItemId, menuId) {
        const tryFocus = (attempt = 0) => {
            const firstItem = firstItemId ? document.getElementById(firstItemId) : null;
            if (firstItem instanceof HTMLElement && !firstItem.hasAttribute("disabled")) {
                firstItem.focus();
                return;
            }

            const menu = menuId ? document.getElementById(menuId) : null;
            if (menu instanceof HTMLElement) {
                const fallbackItem = menu.querySelector(".file-explorer-menu-item:not(:disabled)");
                if (fallbackItem instanceof HTMLElement) {
                    fallbackItem.focus();
                    return;
                }

                menu.focus();
                return;
            }

            if (attempt < 6) {
                window.setTimeout(() => tryFocus(attempt + 1), 30);
            }
        };

        window.requestAnimationFrame(() => tryFocus());
    },

    focusElementById: function (elementId) {
        const tryFocus = (attempt = 0) => {
            const element = elementId ? document.getElementById(elementId) : null;
            if (element instanceof HTMLElement) {
                element.focus();
                return;
            }

            if (attempt < 6) {
                window.setTimeout(() => tryFocus(attempt + 1), 30);
            }
        };

        window.requestAnimationFrame(() => tryFocus());
    }
    
    
};
