window.soundMeter = {
    audioContext: null,
    analyser: null,
    source: null,
    stream: null,
    dataArray: null,
    smoothedDb: -60,

    async start() {
        try {
            if (this.analyser) {
                return true;
            }

            const stream = await navigator.mediaDevices.getUserMedia({
                audio: {
                    echoCancellation: true,
                    noiseSuppression: true,
                    autoGainControl: true
                }
            });

            const AudioContextRef = window.AudioContext || window.webkitAudioContext;
            if (!AudioContextRef) {
                this.stop();
                return false;
            }

            this.stream = stream;
            this.audioContext = new AudioContextRef();
            this.analyser = this.audioContext.createAnalyser();
            this.analyser.fftSize = 2048;
            this.analyser.smoothingTimeConstant = 0.85;
            this.source = this.audioContext.createMediaStreamSource(stream);
            this.source.connect(this.analyser);
            this.dataArray = new Float32Array(this.analyser.fftSize);
            this.smoothedDb = -60;
            return true;
        } catch {
            this.stop();
            return false;
        }
    },

    getDbfs() {
        if (!this.analyser || !this.dataArray) {
            return -60;
        }

        this.analyser.getFloatTimeDomainData(this.dataArray);

        let sumSquares = 0;
        for (let i = 0; i < this.dataArray.length; i += 1) {
            const sample = this.dataArray[i];
            sumSquares += sample * sample;
        }

        const rms = Math.sqrt(sumSquares / this.dataArray.length);
        const minRms = 1e-7;
        const dbfs = 20 * Math.log10(Math.max(rms, minRms));
        const clampedDbfs = Math.max(-60, Math.min(0, dbfs));

        this.smoothedDb = (this.smoothedDb * 0.82) + (clampedDbfs * 0.18);
        return this.smoothedDb;
    },

    stop() {
        if (this.stream) {
            this.stream.getTracks().forEach(track => track.stop());
        }

        if (this.audioContext) {
            try {
                this.audioContext.close();
            } catch {
                // no-op
            }
        }

        this.audioContext = null;
        this.analyser = null;
        this.source = null;
        this.stream = null;
        this.dataArray = null;
        this.smoothedDb = -60;
    }
};
