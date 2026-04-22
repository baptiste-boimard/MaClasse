window.appointments = {
    dotNetInstance: null,

    setInstance: function (instance) {
        window.appointments.dotNetInstance = instance;
    },

    handleAppointmentClick: function (e, appointmentId) {
        e.preventDefault();

        const clamp = (value, min, max) => Math.max(min, Math.min(value, max));
        const margin = 8;
        const menu = document.getElementById("custom-context-menu");
        const menuRect = menu ? menu.getBoundingClientRect() : null;
        const menuWidth = Math.ceil(menuRect?.width ?? 180);
        const menuHeight = Math.ceil(menuRect?.height ?? 120);
        const maxX = Math.max(margin, window.innerWidth - menuWidth - margin);
        const maxY = Math.max(margin, window.innerHeight - menuHeight - margin);
        const x = Math.round(clamp(e.clientX, margin, maxX));
        const y = Math.round(clamp(e.clientY, margin, maxY));

        if (window.appointments.dotNetInstance) {
            window.appointments.dotNetInstance.invokeMethodAsync('ShowCustomMenu', appointmentId, x, y);
        }
    }
};

window.appointments.appointmentsData = [];
window.appointments.currentViewDate = null;
window.appointments.currentViewIndex = 0;
window.appointments.__canvasNavWired = false;

window.appointments.setAppointmentsData = function (data) {
    window.appointments.appointmentsData = Array.isArray(data) ? data : [];
};

window.appointments.setCurrentView = function (dateIso, viewIndex) {
    window.appointments.currentViewDate = new Date(dateIso);
    window.appointments.currentViewIndex = viewIndex;
};

window.appointments.getEventsFromCanvas = function (canvas) {
    return Array.from(canvas.querySelectorAll(".rz-event"))
        .map(function (el) {
            const content = el.querySelector(".rz-event-content");
            if (!content) return null;
            const match = /'([^']+)'\s*\)/.exec(content.getAttribute("onmousedown") || "");
            const id = match ? match[1] : null;
            const data = id ? window.appointments.appointmentsData.find(function (a) { return a.id === id; }) : null;
            if (!data) return null;
            return { el: el, id: id, start: new Date(data.start), end: new Date(data.end) };
        })
        .filter(Boolean)
        .sort(function (a, b) { return a.start - b.start; });
};

window.appointments.focusEventById = function (appointmentId, canvasId) {
    const canvas = document.getElementById(canvasId || "scheduler-canvas");
    if (!canvas) return false;
    const contents = Array.from(canvas.querySelectorAll(".rz-event-content"));
    for (const content of contents) {
        const match = /'([^']+)'\s*\)/.exec(content.getAttribute("onmousedown") || "");
        if (match && match[1] === appointmentId) {
            const el = content.closest(".rz-event");
            if (el) {
                el.setAttribute("tabindex", "0");
                el.focus({ preventScroll: false });
                return true;
            }
        }
    }
    return false;
};

window.appointments.focusFirstEventInCanvas = function (canvasId) {
    const canvas = document.getElementById(canvasId || "scheduler-canvas");
    if (!canvas) return false;
    const events = window.appointments.getEventsFromCanvas(canvas);
    if (events.length === 0) return false;
    events[0].el.setAttribute("tabindex", "0");
    events[0].el.focus({ preventScroll: false });
    return true;
};

window.appointments.focusEventByIdWhenReady = function (appointmentId, canvasId, maxAttempts) {
    maxAttempts = maxAttempts || 40;
    let attempts = 0;
    function tryFocus() {
        if (window.appointments.focusEventById(appointmentId, canvasId)) return;
        attempts++;
        if (attempts >= maxAttempts) {
            window.appointments.focusFirstEventInCanvas(canvasId);
            return;
        }
        requestAnimationFrame(tryFocus);
    }
    requestAnimationFrame(tryFocus);
};

window.appointments.isInCurrentView = function (date) {
    const viewDate = window.appointments.currentViewDate;
    const viewIndex = window.appointments.currentViewIndex;
    if (!viewDate) return true;
    if (viewIndex === 0) {
        return date.toDateString() === viewDate.toDateString();
    }
    const day = viewDate.getDay();
    const startOfWeek = new Date(viewDate);
    startOfWeek.setDate(viewDate.getDate() - ((day + 6) % 7));
    startOfWeek.setHours(0, 0, 0, 0);
    const endOfWeek = new Date(startOfWeek);
    endOfWeek.setDate(startOfWeek.getDate() + 7);
    return date >= startOfWeek && date < endOfWeek;
};

window.appointments.navigateToAppointment = async function (appointmentId, canvasId) {
    const data = window.appointments.appointmentsData.find(function (a) { return a.id === appointmentId; });
    if (!data) return;
    const found = window.appointments.focusEventById(appointmentId, canvasId || "scheduler-canvas");
    if (!found && window.appointments.dotNetInstance) {
        const apptDate = new Date(data.start);
        await window.appointments.dotNetInstance.invokeMethodAsync("NavigateToDate", appointmentId, apptDate.toISOString());
    }
};

window.appointments.wireCanvasNavigation = function (canvasId) {
    if (window.appointments.__canvasNavWired) return;
    window.appointments.__canvasNavWired = true;

    const getCanvas = function () { return document.getElementById(canvasId); };

    const getEventId = function (el) {
        const content = el.querySelector(".rz-event-content");
        if (!content) return null;
        const match = /'([^']+)'\s*\)/.exec(content.getAttribute("onmousedown") || "");
        return match ? match[1] : null;
    };

    // Enter/Space sur scheduler-canvas → focus event courant/suivant
    document.addEventListener("keydown", function (e) {
        const canvas = getCanvas();
        if (!canvas || document.activeElement !== canvas) return;
        if (e.key !== "Enter" && e.key !== " " && e.key !== "Space" && e.key !== "Spacebar") return;
        e.preventDefault();
        e.stopImmediatePropagation();
        const events = window.appointments.getEventsFromCanvas(canvas);
        if (events.length === 0) return;
        const now = new Date();
        let target = events.find(function (i) { return i.start <= now && i.end > now; });
        if (!target) target = events.find(function (i) { return i.start > now; });
        if (!target) target = events[events.length - 1];
        if (target) {
            target.el.setAttribute("tabindex", "0");
            target.el.focus({ preventScroll: false });
        }
    }, true);

    // Tab/Shift+Tab entre events
    document.addEventListener("keydown", function (e) {
        if (e.key !== "Tab") return;
        const canvas = getCanvas();
        if (!canvas) return;
        const active = document.activeElement;
        if (!active || !active.classList.contains("rz-event") || !canvas.contains(active)) return;
        const events = window.appointments.getEventsFromCanvas(canvas);
        let currentIndex = events.findIndex(function (i) { return i.el === active; });
        if (currentIndex < 0) {
            // Fallback : l'élément DOM a peut-être été recréé, on cherche par ID d'appointment
            const content = active.querySelector(".rz-event-content");
            const match = content ? /'([^']+)'\s*\)/.exec(content.getAttribute("onmousedown") || "") : null;
            if (match) currentIndex = events.findIndex(function (i) { return i.id === match[1]; });
        }
        if (currentIndex < 0) return;
        const isRenderable = function (isoStart) {
            const m = new Date(isoStart).getHours() * 60 + new Date(isoStart).getMinutes();
            const end = window.appointments._viewEndMinutes ?? 1140;
            return m < end;
        };
        e.preventDefault();
        e.stopImmediatePropagation();
        if (!e.shiftKey) {
            if (currentIndex < events.length - 1) {
                const next = events[currentIndex + 1];
                next.el.setAttribute("tabindex", "0");
                next.el.focus({ preventScroll: false });
            } else {
                const currentStart = events[currentIndex].start;
                const future = window.appointments.appointmentsData
                    .filter(function (a) { return new Date(a.start) > currentStart && isRenderable(a.start); })
                    .sort(function (a, b) { return new Date(a.start) - new Date(b.start); });
                if (future.length > 0) {
                    window.appointments.navigateToAppointment(future[0].id, canvasId);
                } else {
                    canvas.focus();
                }
            }
        } else {
            if (currentIndex > 0) {
                const prev = events[currentIndex - 1];
                prev.el.setAttribute("tabindex", "0");
                prev.el.focus({ preventScroll: false });
            } else {
                const currentStart = events[currentIndex].start;
                const past = window.appointments.appointmentsData
                    .filter(function (a) { return new Date(a.start) < currentStart && isRenderable(a.start); })
                    .sort(function (a, b) { return new Date(b.start) - new Date(a.start); });
                if (past.length > 0) {
                    window.appointments.navigateToAppointment(past[0].id, canvasId);
                } else {
                    canvas.focus();
                }
            }
        }
    }, true);

    // Escape sur event → retour au canvas
    document.addEventListener("keydown", function (e) {
        if (e.key !== "Escape") return;
        const canvas = getCanvas();
        if (!canvas) return;
        const active = document.activeElement;
        if (!active || !active.classList.contains("rz-event") || !canvas.contains(active)) return;
        e.preventDefault();
        canvas.focus();
    }, true);

    // Enter/Space sur event → ouvrir menu contextuel
    document.addEventListener("keydown", function (e) {
        if (e.key !== "Enter" && e.key !== " " && e.key !== "Space" && e.key !== "Spacebar") return;
        const canvas = getCanvas();
        if (!canvas) return;
        const active = document.activeElement;
        if (!active || !active.classList.contains("rz-event") || !canvas.contains(active)) return;
        e.preventDefault();
        const id = getEventId(active);
        if (!id || !window.appointments.dotNetInstance) return;
        const rect = active.getBoundingClientRect();
        const x = Math.round(rect.left + rect.width / 2);
        const y = Math.round(rect.top + rect.height / 2);
        window.appointments.dotNetInstance.invokeMethodAsync("ShowCustomMenu", id, x, y);
    }, true);

    // Masquer le focus sur clic souris dans le canvas
    document.addEventListener("mousedown", function (e) {
        const canvas = getCanvas();
        if (!canvas || !canvas.contains(e.target)) return;
        requestAnimationFrame(function () {
            if (document.activeElement === canvas) canvas.blur();
        });
    }, true);
};

window.appointments.registerEscapeListener = function (dotNetRef) {
    if (window.appointments.__escapeHandler) return;
    window.appointments.__escapeHandler = function (e) {
        if (e.key !== 'Escape') return;
        e.stopImmediatePropagation();
        dotNetRef.invokeMethodAsync('EscapePressed');
    };
    document.addEventListener('keydown', window.appointments.__escapeHandler, true);
};

window.appointments.unregisterEscapeListener = function () {
    if (!window.appointments.__escapeHandler) return;
    document.removeEventListener('keydown', window.appointments.__escapeHandler, true);
    window.appointments.__escapeHandler = null;
};

window.appointments.registerOutsideClick = function () {
    document.addEventListener("mousedown", function (e) {
        const menu = document.getElementById("custom-context-menu");
        if (menu && !menu.contains(e.target)) {
            if (window.appointments.dotNetInstance) {
                window.appointments.dotNetInstance.invokeMethodAsync("CloseCustomMenu");
            }
        }
    });
};

window.appointments.scrollSchedulerToCurrentTime = function (startMinutes, endMinutes) {
    window.appointments._viewStartMinutes = startMinutes;
    window.appointments._viewEndMinutes = endMinutes;
    const canvas = document.querySelector(".scheduler-canvas");
    if (!canvas || endMinutes <= startMinutes) {
        return;
    }

    const findScrollableContainer = () => {
        const preferred = canvas.querySelector(
            ".rz-scheduler-content, .rz-scheduler-view-content, .rz-scheduler-view"
        );
        if (preferred && preferred.scrollHeight > preferred.clientHeight) {
            return preferred;
        }

        const candidates = Array.from(canvas.querySelectorAll("div"));
        return candidates.find(el => el.scrollHeight > el.clientHeight + 10) || null;
    };

    const scrollable = findScrollableContainer();
    if (!scrollable) {
        return;
    }

    const now = new Date();
    const nowPlusOneHourMinutes = now.getHours() * 60 + now.getMinutes() + 60;
    const clampedMinutes = Math.min(endMinutes, Math.max(startMinutes, nowPlusOneHourMinutes));
    const ratio = (clampedMinutes - startMinutes) / (endMinutes - startMinutes);
    const target = Math.max(
        0,
        ratio * scrollable.scrollHeight - (scrollable.clientHeight / 2)
    );

    // Run after layout to make sure Radzen has rendered time slots.
    window.requestAnimationFrame(() => {
        scrollable.scrollTop = target;
    });
};
