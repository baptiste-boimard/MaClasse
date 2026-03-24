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
