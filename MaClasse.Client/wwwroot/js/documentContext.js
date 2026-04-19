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

    isFocusInsideElement: function (elementId) {
        const el = document.getElementById(elementId);
        if (!el) return false;
        return el.contains(document.activeElement);
    },

    getActiveElementMenuPosition: function () {
        const el = document.activeElement;
        const rect = el ? el.getBoundingClientRect() : { left: 0, bottom: 0 };
        const clamp = (value, min, max) => Math.max(min, Math.min(value, max));
        const margin = 8;
        const menu = document.getElementById("custom-context-menu");
        const menuRect = menu ? menu.getBoundingClientRect() : null;
        const menuWidth = Math.ceil(menuRect?.width ?? 180);
        const menuHeight = Math.ceil(menuRect?.height ?? 140);
        const maxX = Math.max(margin, window.innerWidth - menuWidth - margin);
        const maxY = Math.max(margin, window.innerHeight - menuHeight - margin);
        return {
            x: Math.round(clamp(rect.left, margin, maxX)),
            y: Math.round(clamp(rect.bottom, margin, maxY))
        };
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
        if (window.documents.__outsideClickHandler) {
            return;
        }

        window.documents.__outsideClickHandler = function (e) {
            const target = e.target;
            if (!(target instanceof Element)) {
                return;
            }

            const clickedCard = target.closest(".file-explorer-card");
            const clickedMenu = target.closest(".file-explorer-context-menu");
            const clickedDialog = target.closest(".mud-dialog-container, .mud-overlay, .mud-dialog");

            if (clickedMenu || clickedDialog) {
                return;
            }

            if (!window.documents.dotNetInstance || clickedCard) {
                return;
            }

            const hasOpenMenu = !!document.querySelector(".file-explorer-context-menu");
            const dotnet = window.documents.dotNetInstance;

            if (hasOpenMenu) {
                dotnet.invokeMethodAsync("CloseDocumentMenu").catch(() => {
                    if (window.documents.dotNetInstance === dotnet) {
                        window.documents.dotNetInstance = null;
                    }
                });
                return;
            }

            dotnet.invokeMethodAsync("ClearDocumentSelection").catch(() => {
                if (window.documents.dotNetInstance === dotnet) {
                    window.documents.dotNetInstance = null;
                }
            });
        };

        document.addEventListener("mousedown", window.documents.__outsideClickHandler);
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
    },

    __cancelFocusOutsideMenu: function () {
        if (window.documents.__focusOutsideMenuActivateHandler) {
            document.removeEventListener("keydown", window.documents.__focusOutsideMenuActivateHandler, true);
            window.documents.__focusOutsideMenuActivateHandler = null;
        }
        if (window.documents.__focusOutsideMenuHandler) {
            document.removeEventListener("focusin", window.documents.__focusOutsideMenuHandler, true);
            window.documents.__focusOutsideMenuHandler = null;
        }
    },

    watchFocusOutsideMenu: function (menuId, dotNetRef) {
        window.documents.__cancelFocusOutsideMenu();

        let suppressUntil = 0;

        const menuActivateHandler = function (e) {
            if (e.key !== "Enter" && e.key !== " ") return;
            const target = e.target;
            if (!(target instanceof HTMLElement)) return;
            if (!target.classList.contains("file-explorer-menu-item")) return;
            if (target.disabled || target.hasAttribute("disabled")) return;
            const menu = document.getElementById(menuId);
            if (!menu || !menu.contains(target)) return;
            e.preventDefault();
            suppressUntil = Date.now() + 300;
            target.click();
        };

        const focusinHandler = function (e) {
            if (Date.now() < suppressUntil) return;
            const menu = document.getElementById(menuId);
            if (!menu || !menu.contains(e.target)) {
                window.documents.__cancelFocusOutsideMenu();
                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync("CloseMenuOnFocusOut").catch(() => {});
                }
            }
        };

        window.documents.__focusOutsideMenuActivateHandler = menuActivateHandler;
        window.documents.__focusOutsideMenuHandler = focusinHandler;

        document.addEventListener("keydown", menuActivateHandler, true);
        document.addEventListener("focusin", focusinHandler, true);
    },

    cancelFocusOutsideMenu: function () {
        window.documents.__cancelFocusOutsideMenu();
    }


};
