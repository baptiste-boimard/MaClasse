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
