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
