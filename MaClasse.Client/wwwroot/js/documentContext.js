// Rend le canvas scheduler inaccessible au clavier et aux lecteurs d'écran
(function () {
    const applyCanvasIsolation = (canvas) => {
        canvas.setAttribute('aria-hidden', 'true');
        canvas.querySelectorAll('[tabindex]:not([tabindex="-1"]), a[href], button, input, select, textarea')
            .forEach(el => el.setAttribute('tabindex', '-1'));
    };

    const canvasObserver = new MutationObserver((_, obs) => {
        const canvas = document.getElementById('scheduler-canvas');
        if (!canvas) return;
        applyCanvasIsolation(canvas);
    });

    // Attend que Blazor rende #scheduler-canvas dans le DOM
    const bodyObserver = new MutationObserver(() => {
        const canvas = document.getElementById('scheduler-canvas');
        if (!canvas) return;
        bodyObserver.disconnect();
        applyCanvasIsolation(canvas);
        canvasObserver.observe(canvas, { childList: true, subtree: true, attributes: true, attributeFilter: ['aria-hidden'] });
    });

    bodyObserver.observe(document.body, { childList: true, subtree: true });

    // Cas où le canvas existe déjà
    const canvas = document.getElementById('scheduler-canvas');
    if (canvas) {
        bodyObserver.disconnect();
        applyCanvasIsolation(canvas);
        canvasObserver.observe(canvas, { childList: true, subtree: true, attributes: true, attributeFilter: ['aria-hidden'] });
    }
})();

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
        const rect = el ? el.getBoundingClientRect() : { right: 0, top: 0 };
        const clamp = (value, min, max) => Math.max(min, Math.min(value, max));
        const margin = 8;
        const menuWidth = 180;
        const menuHeight = 140;
        const maxX = Math.max(margin, window.innerWidth - menuWidth - margin);
        const maxY = Math.max(margin, window.innerHeight - menuHeight - margin);
        return {
            x: Math.round(clamp(rect.right + margin, margin, maxX)),
            y: Math.round(clamp(rect.top, margin, maxY))
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
    },

    openContextDialog: function (dialogId, dotNetRef, focusFirst, anchorX, anchorY) {
        const dialog = document.getElementById(dialogId);
        if (!(dialog instanceof HTMLDialogElement)) return;

        const margin = 8;
        const estimatedW = dialog.offsetWidth || 180;
        const estimatedH = dialog.offsetHeight || 140;
        const maxLeft = window.innerWidth - estimatedW - margin;
        const maxTop = window.innerHeight - estimatedH - margin;
        const left = Math.min(anchorX + margin, maxLeft);
        const top = Math.min(anchorY, maxTop);

        dialog.style.cssText = `position: fixed; margin: 0; top: ${top}px; left: ${left}px;`;
        dialog.showModal();

        if (focusFirst) {
            const first = dialog.querySelector('button:not([disabled])');
            if (first instanceof HTMLElement) first.focus();
        }

        dialog.addEventListener('cancel', function onCancel() {
            dialog.removeEventListener('cancel', onCancel);
            if (dotNetRef) dotNetRef.invokeMethodAsync('CloseMenuOnFocusOut').catch(() => {});
        }, { once: true });

        dialog.addEventListener('click', function onBackdropClick(e) {
            if (e.target === dialog) {
                dialog.removeEventListener('click', onBackdropClick);
                if (dotNetRef) dotNetRef.invokeMethodAsync('CloseMenuOnFocusOut').catch(() => {});
            }
        });
    },

    closeContextDialog: function (dialogId) {
        const dialog = document.getElementById(dialogId);
        if (dialog instanceof HTMLDialogElement && dialog.open) {
            dialog.close();
        }
    }

};
