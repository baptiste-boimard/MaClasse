window.kbNav = {
    _zones: [],

    setZones: function (ids) {
        window.kbNav._zones = ids;
    },

    _isEditable: function (el) {
        if (!el) return false;
        const tag = el.tagName;
        return tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT' || el.isContentEditable;
    },

    _getContainingZoneIdx: function () {
        const active = document.activeElement;
        if (!active) return -1;
        const zones = window.kbNav._zones;
        for (let i = 0; i < zones.length; i++) {
            const el = document.getElementById(zones[i]);
            if (el && el.contains(active)) return i;
        }
        return -1;
    },

    _setup: function () {

        // Flèches → zone suivante / précédente
        document.addEventListener('keydown', function (e) {
            const forward = e.key === 'ArrowRight' || e.key === 'ArrowDown';
            const backward = e.key === 'ArrowLeft' || e.key === 'ArrowUp';
            if (!forward && !backward) return;
            if (window.kbNav._isEditable(document.activeElement)) return;

            const idx = window.kbNav._getContainingZoneIdx();
            if (idx < 0) return;

            e.preventDefault();
            const zones = window.kbNav._zones;
            const nextId = forward
                ? zones[(idx + 1) % zones.length]
                : zones[(idx - 1 + zones.length) % zones.length];

            const el = document.getElementById(nextId);
            if (el) el.focus({ preventScroll: true });
        }, true);

        // Échap → remonte sur l'élément zone parent
        document.addEventListener('keydown', function (e) {
            if (e.key !== 'Escape') return;
            const idx = window.kbNav._getContainingZoneIdx();
            if (idx < 0) return;
            const zoneEl = document.getElementById(window.kbNav._zones[idx]);
            if (!zoneEl || document.activeElement === zoneEl) return;
            e.preventDefault();
            zoneEl.focus({ preventScroll: true });
        }, true);
    }
};

window.kbNav._setup();
