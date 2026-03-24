window.classToolsSync = (function () {
    const STORAGE_KEY = "maClasse.classToolsState.v1";
    const listeners = new Map();

    function safeParse(raw) {
        if (!raw) {
            return null;
        }

        try {
            return JSON.parse(raw);
        } catch {
            return null;
        }
    }

    function getState() {
        try {
            const raw = localStorage.getItem(STORAGE_KEY);
            return safeParse(raw);
        } catch {
            return null;
        }
    }

    function setState(state) {
        try {
            const payload = {
                ...state,
                updatedAt: Date.now()
            };

            localStorage.setItem(STORAGE_KEY, JSON.stringify(payload));
            window.dispatchEvent(new CustomEvent("class-tools-sync-local"));
        } catch {
            // Ignore storage failures.
        }
    }

    function subscribe(dotNetRef) {
        const id = `${Date.now()}-${Math.random().toString(36).slice(2)}`;

        const notify = () => {
            try {
                dotNetRef.invokeMethodAsync("OnExternalStateChanged");
            } catch {
                // Ignore notify errors.
            }
        };

        const onStorage = (event) => {
            if (event.key === STORAGE_KEY) {
                notify();
            }
        };

        const onLocal = () => notify();

        window.addEventListener("storage", onStorage);
        window.addEventListener("class-tools-sync-local", onLocal);

        listeners.set(id, { onStorage, onLocal });
        return id;
    }

    function unsubscribe(id) {
        const entry = listeners.get(id);
        if (!entry) {
            return;
        }

        window.removeEventListener("storage", entry.onStorage);
        window.removeEventListener("class-tools-sync-local", entry.onLocal);
        listeners.delete(id);
    }

    return {
        getState,
        setState,
        subscribe,
        unsubscribe
    };
})();
