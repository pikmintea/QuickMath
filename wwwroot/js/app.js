window.__sound = (() => {
    let ctx = null;
    function getCtx() {
        if (!ctx) {
            const C = window.AudioContext || window.webkitAudioContext;
            if (C) ctx = new C();
        }
        return ctx;
    }
    function tone(freq, duration, type = 'sine', vol = 0.25) {
        const c = getCtx();
        if (!c) return;
        const osc = c.createOscillator();
        const gain = c.createGain();
        osc.type = type;
        osc.connect(gain);
        gain.connect(c.destination);
        osc.frequency.setValueAtTime(freq, c.currentTime);
        gain.gain.setValueAtTime(vol, c.currentTime);
        gain.gain.exponentialRampToValueAtTime(0.001, c.currentTime + duration);
        osc.start(c.currentTime);
        osc.stop(c.currentTime + duration);
    }
    return {
        correct() {
            const c = getCtx();
            if (!c) return;
            const o1 = c.createOscillator();
            const o2 = c.createOscillator();
            const g = c.createGain();
            o1.connect(g); o2.connect(g); g.connect(c.destination);
            o1.frequency.setValueAtTime(523, c.currentTime);
            o1.frequency.setValueAtTime(659, c.currentTime + 0.08);
            o2.frequency.setValueAtTime(784, c.currentTime + 0.16);
            g.gain.setValueAtTime(0.2, c.currentTime);
            g.gain.exponentialRampToValueAtTime(0.001, c.currentTime + 0.4);
            o1.start(c.currentTime); o1.stop(c.currentTime + 0.4);
            o2.start(c.currentTime + 0.16); o2.stop(c.currentTime + 0.4);
        },
        wrong() {
            tone(180, 0.3, 'sawtooth', 0.15);
        },
        gameOver() {
            const c = getCtx();
            if (!c) return;
            [300, 250, 200, 150].forEach((f, i) => {
                const o = c.createOscillator();
                const g = c.createGain();
                o.type = 'sine';
                o.connect(g); g.connect(c.destination);
                o.frequency.setValueAtTime(f, c.currentTime + i * 0.15);
                g.gain.setValueAtTime(0.2, c.currentTime + i * 0.15);
                g.gain.exponentialRampToValueAtTime(0.001, c.currentTime + i * 0.15 + 0.2);
                o.start(c.currentTime + i * 0.15);
                o.stop(c.currentTime + i * 0.15 + 0.2);
            });
        },
        streak(level) {
            const c = getCtx();
            if (!c) return;
            const base = [440, 523, 659, 784];
            const note = base[Math.min(level - 1, base.length - 1)];
            const o = c.createOscillator();
            const g = c.createGain();
            o.type = 'sine';
            o.connect(g); g.connect(c.destination);
            o.frequency.setValueAtTime(note, c.currentTime);
            o.frequency.setValueAtTime(note * 1.25, c.currentTime + 0.1);
            g.gain.setValueAtTime(0.25, c.currentTime);
            g.gain.exponentialRampToValueAtTime(0.001, c.currentTime + 0.3);
            o.start(c.currentTime); o.stop(c.currentTime + 0.3);
        },
        countdownTick() {
            tone(440, 0.1, 'sine', 0.1);
        },
        countdownGo() {
            tone(880, 0.3, 'sine', 0.25);
        }
    };
})();

window.__effects = (() => {
    const style = document.createElement('style');
    style.textContent = `
        @keyframes __confettiFall {
            0% { transform: translateY(0) scale(1); opacity: 1; }
            100% { transform: translateY(100vh) scale(0.3); opacity: 0; }
        }
    `;
    document.head.appendChild(style);

    return {
        confetti(count = 20) {
            const body = document.body;
            const colors = ['#3b82f6', '#16a34a', '#d97706', '#dc2626', '#8b5cf6', '#ec4899'];
            for (let i = 0; i < count; i++) {
                const el = document.createElement('div');
                const color = colors[Math.floor(Math.random() * colors.length)];
                const size = 6 + Math.random() * 6;
                const x = Math.random() * window.innerWidth;
                const dur = 0.6 + Math.random() * 0.6;
                const r = Math.random() * 360;
                el.style.cssText = `
                    position: fixed; top: -10px; left: ${x}px; z-index: 9999;
                    width: ${size}px; height: ${size * (0.4 + Math.random() * 0.6)}px;
                    background: ${color}; border-radius: 2px;
                    pointer-events: none;
                    transform: rotate(${r}deg);
                    animation: __confettiFall ${dur}s ease-out forwards;
                    animation-delay: ${Math.random() * 0.2}s;
                `;
                body.appendChild(el);
                setTimeout(() => el.remove(), (dur + 0.3) * 1000);
            }
        }
    };
})();
