window.focusHelpers = window.focusHelpers || {};

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
