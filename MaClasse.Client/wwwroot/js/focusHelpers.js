window.focusHelpers = window.focusHelpers || {};

window.focusHelpers.wireTabSequenceByIds = function (ids) {
    if (!Array.isArray(ids) || ids.length === 0) {
        return;
    }

    const wireKey = ids.join("|");
    window.focusHelpers.__tabSequenceByIdsWired = window.focusHelpers.__tabSequenceByIdsWired || {};
    if (window.focusHelpers.__tabSequenceByIdsWired[wireKey]) {
        return;
    }
    window.focusHelpers.__tabSequenceByIdsWired[wireKey] = true;

    const isAvailable = function (el) {
        if (!el) return false;
        const rect = el.getBoundingClientRect();
        return rect.width > 0 || rect.height > 0;
    };

    const getFocusTarget = function (id) {
        const el = document.getElementById(id);
        if (!el || !isAvailable(el)) return null;
        if (el.tabIndex >= 0) return el;
        return el.querySelector("button:not([disabled]), [tabindex]:not([tabindex='-1']), input:not([disabled]), a[href]") || null;
    };

    const resolveTargets = function () {
        return ids
            .map((id) => ({ id, target: getFocusTarget(id), el: document.getElementById(id) }))
            .filter((item) => item.target !== null);
    };

    document.addEventListener("keydown", function (e) {
        if (e.key !== "Tab" || e.shiftKey || e.ctrlKey || e.metaKey || e.altKey) {
            return;
        }

        const active = document.activeElement;
        if (!(active instanceof HTMLElement)) {
            return;
        }

        const targets = resolveTargets();
        if (targets.length === 0) {
            return;
        }

        const currentIndex = targets.findIndex(
            (item) => item.target === active || item.el === active || (item.el && item.el.contains(active))
        );

        if (currentIndex < 0 || currentIndex === targets.length - 1) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();

        const nextItem = targets[currentIndex + 1];
        nextItem.target.focus({ preventScroll: true });
    }, true);
};

window.focusHelpers.focusElementById = function (id) {
    if (!id) {
        return;
    }

    const direct = document.getElementById(id);
    const fallback = direct ? direct.querySelector("button, [tabindex], input, textarea, select, a[href]") : null;
    const target = direct?.focus ? direct : fallback;

    if (!target || typeof target.focus !== "function") {
        return;
    }

    const focusNow = () => target.focus({ preventScroll: true });

    focusNow();
    requestAnimationFrame(focusNow);
    setTimeout(focusNow, 0);
};

window.focusHelpers.focusFirstInContainer = function (containerId) {
    if (!containerId) {
        return;
    }

    const container = document.getElementById(containerId);
    if (!container) {
        return;
    }

    const target = container.querySelector(
        "button,[role='tab'],[tabindex]:not([tabindex='-1']),input,textarea,select,a[href]"
    );

    if (!target || typeof target.focus !== "function") {
        return;
    }

    const focusNow = () => target.focus({ preventScroll: true });
    focusNow();
    requestAnimationFrame(focusNow);
    setTimeout(focusNow, 0);
};

window.focusHelpers.wireSuccessTabNavigation = function (successTabId, saveButtonId) {
    if (!successTabId || !saveButtonId) {
        return;
    }

    const successTab = document.getElementById(successTabId);
    if (!successTab || successTab.dataset.successTabNavigationWired === "1") {
        return;
    }

    successTab.dataset.successTabNavigationWired = "1";

    successTab.addEventListener("keydown", function (e) {
        if (e.key !== "Tab" || e.shiftKey) {
            return;
        }

        e.preventDefault();
        window.focusHelpers.focusElementById(saveButtonId);
    }, true);
};

window.focusHelpers.wireTabRedirect = function (fromElementId, toElementId) {
    if (!fromElementId || !toElementId) {
        return;
    }

    const fromElement = document.getElementById(fromElementId);
    if (!fromElement || fromElement.dataset.tabRedirectWired === "1") {
        return;
    }

    fromElement.dataset.tabRedirectWired = "1";

    fromElement.addEventListener("keydown", function (e) {
        if (e.key !== "Tab" || e.shiftKey) {
            return;
        }

        e.preventDefault();
        window.focusHelpers.focusElementById(toElementId);
    }, true);
};

window.focusHelpers.wireTabRedirectFromSelf = function (fromElementId, toElementId) {
    if (!fromElementId || !toElementId) {
        return;
    }

    const fromElement = document.getElementById(fromElementId);
    if (!fromElement || fromElement.dataset.tabRedirectFromSelfWired === "1") {
        return;
    }

    fromElement.dataset.tabRedirectFromSelfWired = "1";

    fromElement.addEventListener("keydown", function (e) {
        if (e.key !== "Tab" || e.shiftKey) {
            return;
        }

        if (e.target !== fromElement) {
            return;
        }

        e.preventDefault();
        window.focusHelpers.focusElementById(toElementId);
    }, true);
};

window.focusHelpers.wireEnterSpaceRedirectFromSelf = function (fromElementId, toElementId) {
    if (!fromElementId || !toElementId) {
        return;
    }

    const fromElement = document.getElementById(fromElementId);
    if (!fromElement || fromElement.dataset.enterSpaceRedirectFromSelfWired === "1") {
        return;
    }

    fromElement.dataset.enterSpaceRedirectFromSelfWired = "1";

    fromElement.addEventListener("keydown", function (e) {
        const isEnter = e.key === "Enter";
        const isSpace = e.key === " " || e.key === "Space" || e.key === "Spacebar";

        if (!isEnter && !isSpace) {
            return;
        }

        if (e.target !== fromElement) {
            return;
        }

        e.preventDefault();
        window.focusHelpers.focusElementById(toElementId);
    }, true);
};

window.focusHelpers.wireShiftTabRedirectFromSelf = function (fromElementId, toElementId) {
    if (!fromElementId || !toElementId) {
        return;
    }

    const fromElement = document.getElementById(fromElementId);
    if (!fromElement || fromElement.dataset.shiftTabRedirectFromSelfWired === "1") {
        return;
    }

    fromElement.dataset.shiftTabRedirectFromSelfWired = "1";

    fromElement.addEventListener("keydown", function (e) {
        if (e.key !== "Tab" || !e.shiftKey) {
            return;
        }

        if (e.target !== fromElement) {
            return;
        }

        e.preventDefault();
        window.focusHelpers.focusElementById(toElementId);
    }, true);
};

window.focusHelpers.wireAdvancedSearchTabFlow = function (searchButtonId, resultIdPrefix, fallbackElementId) {
    if (!searchButtonId || !resultIdPrefix || !fallbackElementId) {
        return;
    }

    const wireKey = `${searchButtonId}|${resultIdPrefix}|${fallbackElementId}`;
    window.focusHelpers.__advancedSearchTabFlowWired = window.focusHelpers.__advancedSearchTabFlowWired || {};
    if (window.focusHelpers.__advancedSearchTabFlowWired[wireKey]) {
        return;
    }
    window.focusHelpers.__advancedSearchTabFlowWired[wireKey] = true;

    document.addEventListener("keydown", function (e) {
        if (e.key !== "Tab" || e.shiftKey) {
            return;
        }

        const active = document.activeElement;
        if (!(active instanceof HTMLElement)) {
            return;
        }

        const resultCards = Array.from(document.querySelectorAll(`[id^='${resultIdPrefix}']`))
            .filter((node) => node instanceof HTMLElement);

        if (active.id === searchButtonId) {
            e.preventDefault();
            if (resultCards.length > 0) {
                resultCards[0].focus({ preventScroll: true });
            } else {
                window.focusHelpers.focusElementById(fallbackElementId);
            }
            return;
        }

        if (!active.id || !active.id.startsWith(resultIdPrefix)) {
            return;
        }

        const currentIndex = resultCards.findIndex((card) => card.id === active.id);
        if (currentIndex < 0) {
            return;
        }

        e.preventDefault();
        const nextCard = resultCards[currentIndex + 1];
        if (nextCard) {
            nextCard.focus({ preventScroll: true });
            return;
        }

        window.focusHelpers.focusElementById(fallbackElementId);
    }, true);
};

window.focusHelpers.wireAltFocusCycleByIds = function (ids) {
    if (!Array.isArray(ids) || ids.length === 0) {
        return;
    }

    const wireKey = ids.join("|");
    window.focusHelpers.__altFocusCycleByIdsWired = window.focusHelpers.__altFocusCycleByIdsWired || {};
    if (window.focusHelpers.__altFocusCycleByIdsWired[wireKey]) {
        return;
    }
    window.focusHelpers.__altFocusCycleByIdsWired[wireKey] = true;

    const isVisible = function (el) {
        if (!el) return false;
        const rect = el.getBoundingClientRect();
        return rect.width > 0 || rect.height > 0;
    };

    const resolveTargets = function () {
        return ids
            .map((id) => document.getElementById(id))
            .filter((el) => el instanceof HTMLElement && isVisible(el));
    };

    let altHandledOnKeyDown = false;

    document.addEventListener("keydown", function (e) {
        if (!e.altKey || e.ctrlKey || e.metaKey || e.shiftKey) {
            return;
        }
        if (e.key !== "Alt" && e.key !== "AltGraph" && e.key !== "") {
            return;
        }

        const active = document.activeElement;
        if (!(active instanceof HTMLElement)) {
            return;
        }

        const targets = resolveTargets();
        if (targets.length === 0) {
            return;
        }

        const currentIndex = targets.findIndex((t) => t === active || t.contains(active));
        if (currentIndex < 0) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();

        const nextTarget = targets[(currentIndex + 1) % targets.length];
        nextTarget.focus({ preventScroll: true });
        altHandledOnKeyDown = true;
    }, true);

    document.addEventListener("keyup", function (e) {
        if ((!e.altKey && e.key !== "Alt" && e.key !== "AltGraph" && e.key !== "") || !altHandledOnKeyDown) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();
        altHandledOnKeyDown = false;
    }, true);
};

window.focusHelpers.wireAltFocusCycleInContainer = function (containerSelector, targetSelectors) {
    if (!containerSelector || !Array.isArray(targetSelectors) || targetSelectors.length === 0) {
        return;
    }

    const container = document.querySelector(containerSelector);
    if (!(container instanceof HTMLElement)) {
        return;
    }

    const wireKey = `${containerSelector}|${targetSelectors.join("|")}`;
    window.focusHelpers.__altFocusCycleInContainerWired = window.focusHelpers.__altFocusCycleInContainerWired || {};
    if (window.focusHelpers.__altFocusCycleInContainerWired[wireKey]) {
        return;
    }
    window.focusHelpers.__altFocusCycleInContainerWired[wireKey] = true;

    const resolveTargets = function () {
        return targetSelectors
            .map((selector) => container.querySelector(selector))
            .filter((element) => element instanceof HTMLElement);
    };

    let altHandledOnKeyDown = false;

    document.addEventListener("keydown", function (e) {
        if (e.key !== "Alt" || e.ctrlKey || e.metaKey || e.shiftKey) {
            return;
        }

        const active = document.activeElement;
        if (!(active instanceof HTMLElement)) {
            return;
        }

        const targets = resolveTargets();
        if (targets.length === 0) {
            return;
        }

        const currentIndex = targets.findIndex((target) => target === active || target.contains(active));
        if (currentIndex < 0) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();

        const nextTarget = targets[(currentIndex + 1) % targets.length];
        nextTarget.focus({ preventScroll: true });
        altHandledOnKeyDown = true;
    }, true);

    document.addEventListener("keyup", function (e) {
        if (e.key !== "Alt" || !altHandledOnKeyDown) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();
        altHandledOnKeyDown = false;
    }, true);
};

window.focusHelpers.wireDocumentShiftTabRedirect = function (fromElementId, toElementId) {
    if (!fromElementId || !toElementId) {
        return;
    }

    const wireKey = fromElementId + "|doc-shift-tab|" + toElementId;
    window.focusHelpers.__documentShiftTabRedirectWired = window.focusHelpers.__documentShiftTabRedirectWired || {};
    if (window.focusHelpers.__documentShiftTabRedirectWired[wireKey]) {
        return;
    }
    window.focusHelpers.__documentShiftTabRedirectWired[wireKey] = true;

    document.addEventListener("keydown", function (e) {
        if (e.key !== "Tab" || !e.shiftKey || e.ctrlKey || e.metaKey || e.altKey) {
            return;
        }

        const active = document.activeElement;
        if (!(active instanceof HTMLElement)) {
            return;
        }

        const fromEl = document.getElementById(fromElementId);
        if (!fromEl) {
            return;
        }

        if (active !== fromEl && !fromEl.contains(active)) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();
        window.focusHelpers.focusElementById(toElementId);
    }, true);
};

window.focusHelpers.wireDocumentTabRedirect = function (fromElementId, toElementId) {
    if (!fromElementId || !toElementId) {
        return;
    }

    const wireKey = fromElementId + "|doc-tab|" + toElementId;
    window.focusHelpers.__documentTabRedirectWired = window.focusHelpers.__documentTabRedirectWired || {};
    if (window.focusHelpers.__documentTabRedirectWired[wireKey]) {
        return;
    }
    window.focusHelpers.__documentTabRedirectWired[wireKey] = true;

    document.addEventListener("keydown", function (e) {
        if (e.key !== "Tab" || e.shiftKey || e.ctrlKey || e.metaKey || e.altKey) {
            return;
        }

        const active = document.activeElement;
        if (!(active instanceof HTMLElement)) {
            return;
        }

        const fromEl = document.getElementById(fromElementId);
        if (!fromEl) {
            return;
        }

        if (active !== fromEl && !fromEl.contains(active)) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();
        window.focusHelpers.focusElementById(toElementId);
    }, true);
};

window.focusHelpers.wireTabButtonEnterToPanel = function (tabButtonId, panelFirstElementId) {
    if (!tabButtonId || !panelFirstElementId) {
        return;
    }

    const button = document.getElementById(tabButtonId);
    if (!button || button.dataset.tabButtonEnterToPanelWired === "1") {
        return;
    }

    button.dataset.tabButtonEnterToPanelWired = "1";

    button.addEventListener("keydown", function (e) {
        const isEnter = e.key === "Enter";
        const isSpace = e.key === " " || e.key === "Space" || e.key === "Spacebar";
        if (!isEnter && !isSpace) {
            return;
        }

        if (e.target !== button) {
            return;
        }

        // Ne pas preventDefault : le clic doit se propager pour que SelectTool soit appelé.
        // On attend le re-render Blazor avant de déplacer le focus.
        const tryFocus = function (remaining) {
            const target = document.getElementById(panelFirstElementId);
            if (target) {
                window.focusHelpers.focusElementById(panelFirstElementId);
                return;
            }
            if (remaining > 0) {
                requestAnimationFrame(function () { tryFocus(remaining - 1); });
            }
        };

        requestAnimationFrame(function () { tryFocus(15); });
    });
};

window.focusHelpers.wireKeyActivateToInnerButtonClick = function (wrapperId) {
    if (!wrapperId) {
        return;
    }

    const wrapper = document.getElementById(wrapperId);
    if (!wrapper || wrapper.dataset.keyActivateToInnerButtonClickWired === "1") {
        return;
    }

    wrapper.dataset.keyActivateToInnerButtonClickWired = "1";

    wrapper.addEventListener("keydown", function (e) {
        const isEnter = e.key === "Enter";
        const isSpace = e.key === " " || e.key === "Space" || e.key === "Spacebar";
        if (!isEnter && !isSpace) {
            return;
        }

        e.preventDefault();

        const btn = wrapper.querySelector("button:not([disabled])");
        if (btn) {
            btn.click();
        }
    });
};

window.focusHelpers.keepInnerButtonTabIndexNegative = function (wrapperId) {
    if (!wrapperId) {
        return;
    }

    const wireKey = "inner-btn-tabindex-" + wrapperId;
    window.focusHelpers.__innerBtnTabIndexWired = window.focusHelpers.__innerBtnTabIndexWired || {};
    if (window.focusHelpers.__innerBtnTabIndexWired[wireKey]) {
        return;
    }
    window.focusHelpers.__innerBtnTabIndexWired[wireKey] = true;

    setInterval(function () {
        const wrapper = document.getElementById(wrapperId);
        if (!wrapper) return;
        const btn = wrapper.querySelector("button");
        if (btn && btn.tabIndex !== -1) {
            btn.tabIndex = -1;
        }
    }, 150);
};

window.focusHelpers.wireTabToFirstInContainer = function (fromId, containerId) {
    if (!fromId || !containerId) {
        return;
    }

    const fromEl = document.getElementById(fromId);
    if (!fromEl || fromEl.dataset.tabToFirstInContainerWired === "1") {
        return;
    }

    fromEl.dataset.tabToFirstInContainerWired = "1";

    fromEl.addEventListener("keydown", function (e) {
        if (e.key !== "Tab" || e.shiftKey) {
            return;
        }

        if (e.target !== fromEl) {
            return;
        }

        e.preventDefault();
        window.focusHelpers.focusFirstInContainer(containerId);
    }, true);
};

window.focusHelpers.wireContainerExitTab = function (containerId, targetId) {
    if (!containerId || !targetId) {
        return;
    }

    const wireKey = containerId + "|container-exit-tab|" + targetId;
    window.focusHelpers.__containerExitTabWired = window.focusHelpers.__containerExitTabWired || {};
    if (window.focusHelpers.__containerExitTabWired[wireKey]) {
        return;
    }
    window.focusHelpers.__containerExitTabWired[wireKey] = true;

    const getFocusables = function (container) {
        return Array.from(container.querySelectorAll(
            "button:not([disabled]), [tabindex]:not([tabindex='-1']), input:not([disabled]), a[href]"
        )).filter(function (el) { return el instanceof HTMLElement; });
    };

    document.addEventListener("keydown", function (e) {
        if (e.key !== "Tab" || e.shiftKey || e.ctrlKey || e.metaKey || e.altKey) {
            return;
        }

        const active = document.activeElement;
        if (!(active instanceof HTMLElement)) {
            return;
        }

        const container = document.getElementById(containerId);
        if (!container || !container.contains(active)) {
            return;
        }

        const focusables = getFocusables(container);
        if (focusables.length === 0 || active !== focusables[focusables.length - 1]) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();
        window.focusHelpers.focusElementById(targetId);
    }, true);
};

window.focusHelpers.wirePdfScrollAreaEnter = function (scrollAreaId, iframeId) {
    if (!scrollAreaId || !iframeId) {
        return;
    }

    const wireKey = "pdf-scroll-area-enter-" + scrollAreaId;
    window.focusHelpers.__pdfScrollAreaEnterWired = window.focusHelpers.__pdfScrollAreaEnterWired || {};
    if (window.focusHelpers.__pdfScrollAreaEnterWired[wireKey]) {
        return;
    }
    window.focusHelpers.__pdfScrollAreaEnterWired[wireKey] = true;

    document.addEventListener("keydown", function (e) {
        const isEnter = e.key === "Enter";
        const isSpace = e.key === " " || e.key === "Space" || e.key === "Spacebar";
        if (!isEnter && !isSpace) {
            return;
        }

        const active = document.activeElement;
        if (!(active instanceof HTMLElement)) {
            return;
        }

        const scrollEl = document.getElementById(scrollAreaId);
        if (!scrollEl || active !== scrollEl) {
            return;
        }

        const iframe = document.getElementById(iframeId);
        if (!iframe) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();
        iframe.focus();
    }, true);
};

window.focusHelpers.wireImageScrollAreaKeyboard = function (scrollAreaId, imageId) {
    if (!scrollAreaId || !imageId) {
        return;
    }

    const wireKey = "image-scroll-area-keyboard-" + scrollAreaId;
    window.focusHelpers.__imageScrollAreaKeyboardWired = window.focusHelpers.__imageScrollAreaKeyboardWired || {};
    if (window.focusHelpers.__imageScrollAreaKeyboardWired[wireKey]) {
        return;
    }
    window.focusHelpers.__imageScrollAreaKeyboardWired[wireKey] = true;

    const scrollKeys = new Set(["ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "PageUp", "PageDown", "Home", "End"]);

    document.addEventListener("keydown", function (e) {
        if (!scrollKeys.has(e.key)) {
            return;
        }

        const active = document.activeElement;
        if (!(active instanceof HTMLElement)) {
            return;
        }

        const scrollEl = document.getElementById(scrollAreaId);
        if (!scrollEl || active !== scrollEl) {
            return;
        }

        const img = document.getElementById(imageId);
        if (!img) {
            return;
        }

        const scrollableParent = (function findScrollable(el) {
            let node = el.parentElement;
            while (node && node !== document.body) {
                const style = getComputedStyle(node);
                const overflowY = style.overflowY;
                const overflowX = style.overflowX;
                const canScrollY = (overflowY === "auto" || overflowY === "scroll") && node.scrollHeight > node.clientHeight;
                const canScrollX = (overflowX === "auto" || overflowX === "scroll") && node.scrollWidth > node.clientWidth;
                if (canScrollY || canScrollX) {
                    return node;
                }
                node = node.parentElement;
            }
            return scrollEl;
        })(img);

        const stepSmall = 80;
        const stepPage = scrollableParent.clientHeight * 0.85;

        e.preventDefault();
        e.stopPropagation();

        switch (e.key) {
            case "ArrowDown":  scrollableParent.scrollBy({ top: stepSmall, behavior: "smooth" }); break;
            case "ArrowUp":    scrollableParent.scrollBy({ top: -stepSmall, behavior: "smooth" }); break;
            case "ArrowRight": scrollableParent.scrollBy({ left: stepSmall, behavior: "smooth" }); break;
            case "ArrowLeft":  scrollableParent.scrollBy({ left: -stepSmall, behavior: "smooth" }); break;
            case "PageDown":   scrollableParent.scrollBy({ top: stepPage, behavior: "smooth" }); break;
            case "PageUp":     scrollableParent.scrollBy({ top: -stepPage, behavior: "smooth" }); break;
            case "Home":       scrollableParent.scrollTo({ top: 0, behavior: "smooth" }); break;
            case "End":        scrollableParent.scrollTo({ top: scrollableParent.scrollHeight, behavior: "smooth" }); break;
        }
    }, true);
};

window.focusHelpers.wireMudMenuCloseOnFocusOut = function (menuWrapperId, dotNetRef) {
    if (!menuWrapperId || !dotNetRef) {
        return;
    }

    const wireKey = "mud-menu-close-focusout-" + menuWrapperId;
    window.focusHelpers.__mudMenuCloseOnFocusOutWired = window.focusHelpers.__mudMenuCloseOnFocusOutWired || {};
    if (window.focusHelpers.__mudMenuCloseOnFocusOutWired[wireKey]) {
        return;
    }
    window.focusHelpers.__mudMenuCloseOnFocusOutWired[wireKey] = true;

    document.addEventListener("focusout", function (e) {
        const relatedTarget = e.relatedTarget;

        const openPopovers = Array.from(document.querySelectorAll(".mud-popover-open"));
        if (openPopovers.length === 0) {
            return;
        }

        const wrapper = document.getElementById(menuWrapperId);

        const isInsidePopover = openPopovers.some(function (p) {
            return relatedTarget && p.contains(relatedTarget);
        });
        const isInsideWrapper = wrapper && relatedTarget && wrapper.contains(relatedTarget);

        if (isInsidePopover || isInsideWrapper) {
            return;
        }

        dotNetRef.invokeMethodAsync("CloseMenuAsync").catch(function () {});
    }, true);
};

window.focusHelpers.wireMudMenuFocusFirstOnOpen = function (menuWrapperId) {
    // Géré côté Blazor via OnActivatorKeyDown + focusFirstOpenMudMenuItemDelayed
};

window.focusHelpers.focusFirstOpenMudMenuItemDelayed = function (delay) {
    setTimeout(function () {
        const openPopover = document.querySelector(".mud-popover-open");
        if (!openPopover) {
            return;
        }
        // MudBlazor 8 rend MudMenuItem comme <div tabindex="0" class="mud-menu-item ...">
        const firstItem =
            openPopover.querySelector(".mud-menu-item[tabindex='0']") ||
            openPopover.querySelector("div[tabindex='0']") ||
            openPopover.querySelector("[tabindex='0']");
        if (!firstItem) {
            return;
        }
        firstItem.focus({ preventScroll: true });
        // Marquer comme focus clavier pour le ring CSS (évite l'affichage à la souris)
        firstItem.classList.add("focus-keyboard");
        firstItem.addEventListener("blur", function () {
            firstItem.classList.remove("focus-keyboard");
        }, { once: true });

        // MudMenuItem est un <div> : pas d'activation native par Enter, brancher le clavier
        if (firstItem.tagName !== "BUTTON" && firstItem.tagName !== "A") {
            const popover = openPopover;
            if (popover.dataset.mudMenuItemKeyboardWired === "1") {
                return;
            }
            popover.dataset.mudMenuItemKeyboardWired = "1";
            popover.addEventListener("keydown", function (e) {
                const isEnter = e.key === "Enter";
                const isSpace = e.key === " " || e.key === "Space" || e.key === "Spacebar";
                if (!isEnter && !isSpace) {
                    return;
                }
                const active = document.activeElement;
                if (!(active instanceof HTMLElement) || !popover.contains(active)) {
                    return;
                }
                if (active.tagName === "BUTTON" || active.tagName === "A") {
                    return;
                }
                e.preventDefault();
                active.click();
            });
        }
    }, delay);
};

window.focusHelpers.wireContainerExitShiftTab = function (containerId, targetId) {
    if (!containerId || !targetId) {
        return;
    }

    const wireKey = containerId + "|container-exit-shift-tab|" + targetId;
    window.focusHelpers.__containerExitShiftTabWired = window.focusHelpers.__containerExitShiftTabWired || {};
    if (window.focusHelpers.__containerExitShiftTabWired[wireKey]) {
        return;
    }
    window.focusHelpers.__containerExitShiftTabWired[wireKey] = true;

    const getFocusables = function (container) {
        return Array.from(container.querySelectorAll(
            "button:not([disabled]), [tabindex]:not([tabindex='-1']), input:not([disabled]), a[href]"
        )).filter(function (el) { return el instanceof HTMLElement; });
    };

    document.addEventListener("keydown", function (e) {
        if (e.key !== "Tab" || !e.shiftKey || e.ctrlKey || e.metaKey || e.altKey) {
            return;
        }

        const active = document.activeElement;
        if (!(active instanceof HTMLElement)) {
            return;
        }

        const container = document.getElementById(containerId);
        if (!container || !container.contains(active)) {
            return;
        }

        const focusables = getFocusables(container);
        if (focusables.length === 0 || active !== focusables[0]) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();
        window.focusHelpers.focusElementById(targetId);
    }, true);
};
