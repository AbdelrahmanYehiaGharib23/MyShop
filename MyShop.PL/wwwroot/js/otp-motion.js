(function () {
    var root = document.querySelector('[data-otp-motion]');
    if (!root) return;

    var input = root.querySelector('.otp-native-input');
    var boxes = root.querySelector('.otp-boxes');
    var status = root.querySelector('[data-otp-status]');
    var statusText = root.querySelector('[data-otp-status-text]');
    var form = input.closest('form');
    var digitCount = 6;
    var focused = false;
    var submitting = false;
    var value = '';
    var elapsed = 0;
    var lastFrame = performance.now();
    var fullCodeTimer = 0;

    function spring(stiffness, damping) {
        return { x: 0, v: 0, t: 0, k: stiffness, c: damping };
    }

    function step(state, dt) {
        var acceleration = -state.k * (state.x - state.t) - state.c * state.v;
        state.v += acceleration * dt;
        state.x += state.v * dt;
    }

    function clamp(number, min, max) {
        return Math.max(min, Math.min(max, number));
    }

    function makeCell() {
        var element = document.createElement('div');
        var digit = document.createElement('i');
        var caret = document.createElement('span');

        element.className = 'otp-cell';
        digit.className = 'otp-cell-digit';
        caret.className = 'otp-cell-caret';
        element.appendChild(digit);
        element.appendChild(caret);
        boxes.appendChild(element);

        return {
            element: element,
            digit: digit,
            caret: caret,
            y: spring(520, 16),
            opacity: spring(400, 28),
            active: spring(300, 26)
        };
    }

    var cells = Array.from({ length: digitCount }, makeCell);
    var morph = spring(260, 24);
    var progress = spring(220, 28);
    var reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)');

    root.classList.add('otp-motion--ready');

    function setValue(nextValue) {
        var cleanValue = String(nextValue || '').replace(/[^0-9]/g, '').slice(0, digitCount);

        cells.forEach(function (cell, index) {
            var nextDigit = cleanValue[index] || '';
            var currentDigit = cell.digit.textContent;

            if (nextDigit && currentDigit !== nextDigit) {
                cell.digit.textContent = nextDigit;
                cell.y.x = -14;
                cell.y.v = 0;
                cell.opacity.x = 0;
                cell.opacity.t = 1;
            }

            if (!nextDigit) {
                cell.digit.textContent = '';
                cell.opacity.t = 0;
            }
        });

        value = cleanValue;
        if (input.value !== cleanValue) input.value = cleanValue;

        window.clearTimeout(fullCodeTimer);
        if (cleanValue.length === digitCount && !submitting) {
            fullCodeTimer = window.setTimeout(function () {
                morph.t = 1;
            }, 250);
        } else if (!submitting) {
            morph.t = 0;
            progress.t = 0;
        }
    }

    function onInput() {
        setValue(input.value);
    }

    function animate(now) {
        var dt = Math.min(Math.max((now - lastFrame) / 1000, 0), 1 / 30);
        lastFrame = now;
        elapsed += dt;

        var states = [];
        cells.forEach(function (cell, index) {
            cell.active.t = !submitting && focused && index === Math.min(value.length, digitCount - 1) ? 1 : 0;
            states.push(cell.y, cell.opacity, cell.active);
        });
        states.push(morph, progress);

        for (var substep = 0; substep < 4; substep += 1) {
            states.forEach(function (state) {
                if (reduceMotion.matches) {
                    state.x = state.t;
                    state.v = 0;
                } else {
                    step(state, dt / 4);
                }
            });
        }

        render();
        window.requestAnimationFrame(animate);
    }

    function render() {
        var width = boxes.clientWidth;
        if (!width) return;

        var baseGap = 8;
        var cellWidth = Math.min(48, (width - baseGap * (digitCount - 1)) / digitCount);
        var join = clamp(morph.x, 0, 1.1);
        var gap = baseGap * (1 - clamp(join, 0, 1));
        var total = cellWidth * digitCount + gap * (digitCount - 1);
        var start = (width - total) / 2;
        var submittingProgress = clamp(progress.x, 0, 1);

        cells.forEach(function (cell, index) {
            var active = clamp(cell.active.x, 0, 1);
            var digitOpacity = clamp(cell.opacity.x, 0, 1) * (1 - submittingProgress);
            var joinedWidth = cellWidth + (index < digitCount - 1 ? baseGap * clamp(join, 0, 1) + 1 : 0);

            cell.element.style.left = (start + index * (cellWidth + gap)).toFixed(2) + 'px';
            cell.element.style.width = Math.max(0, joinedWidth).toFixed(2) + 'px';
            cell.element.style.setProperty(
                '--otp-underline-color',
                submittingProgress > .01 || active > .01
                    ? 'color-mix(in srgb, var(--primary) ' + (Math.max(active, submittingProgress) * 100).toFixed(1) + '%, var(--border))'
                    : 'var(--border)'
            );
            cell.element.style.setProperty('--otp-underline-scale', (0.88 + active * 0.24).toFixed(3));
            cell.digit.style.opacity = digitOpacity.toFixed(3);
            cell.digit.style.transform = 'translateY(' + cell.y.x.toFixed(2) + 'px)';
            cell.digit.style.color = active > .01
                ? 'color-mix(in srgb, var(--primary) ' + (active * 30).toFixed(1) + '%, var(--text))'
                : '';
            cell.caret.style.opacity = active > .5 && !value[index] && (elapsed * 1.8) % 2 < 1.2 ? '1' : '0';
        });

        status.style.opacity = clamp(submittingProgress * 1.6 - .5, 0, 1).toFixed(3);
        status.style.transform = 'translateY(' + ((1 - submittingProgress) * 8).toFixed(2) + 'px)';
    }

    input.addEventListener('input', onInput);
    input.addEventListener('focus', function () { focused = true; });
    input.addEventListener('blur', function () { focused = false; });

    if (form) {
        form.addEventListener('submit', function () {
            if (!form.checkValidity() || submitting) return;

            submitting = true;
            window.clearTimeout(fullCodeTimer);
            morph.t = 1;
            progress.t = 1;
            statusText.textContent = document.documentElement.lang === 'ar' ? 'جارٍ التحقق...' : 'Checking code...';
        });
    }

    setValue(input.value);
    window.requestAnimationFrame(animate);
})();
