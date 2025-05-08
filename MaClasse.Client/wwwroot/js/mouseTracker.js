window.appointments = {
    dotNetInstance: null,

    setInstance: function (instance) {
        window.appointments.dotNetInstance = instance;
    },

    handleAppointmentClick: function (e, appointmentId) {
        e.preventDefault();

        const x = e.clientX + window.scrollX;
        const y = e.clientY + window.scrollY;

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
