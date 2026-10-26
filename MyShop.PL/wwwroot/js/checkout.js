(function () {
    var summary = document.querySelector('[data-checkout-summary]');
    var mobileLayout = window.matchMedia('(max-width: 767px)');

    if (summary) {
        summary.open = !mobileLayout.matches;
        mobileLayout.addEventListener('change', function (event) {
            summary.open = !event.matches;
        });
    }

    var form = document.querySelector('.checkout-layout');
    var continueButton = form && form.querySelector('[data-checkout-continue]');

    if (form && continueButton) {
        form.addEventListener('submit', function (event) {
            if (event.submitter !== continueButton || continueButton.disabled) return;

            continueButton.disabled = true;
            continueButton.innerHTML = '<span class="store-button-loader" aria-hidden="true"></span><span>Saving delivery details...</span>';
        });
    }

    var cards = document.querySelectorAll('.paymob-card-preview');
    var reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    var finePointer = window.matchMedia('(pointer: fine)').matches;

    if (!reducedMotion && finePointer) {
        cards.forEach(function (card) {
            var frame = 0;

            card.addEventListener('pointermove', function (event) {
                var bounds = card.getBoundingClientRect();
                var x = Math.min(1, Math.max(0, (event.clientX - bounds.left) / bounds.width));
                var y = Math.min(1, Math.max(0, (event.clientY - bounds.top) / bounds.height));

                window.cancelAnimationFrame(frame);
                frame = window.requestAnimationFrame(function () {
                    card.style.setProperty('--mx', (x * 100) + '%');
                    card.style.setProperty('--my', (y * 100) + '%');
                    card.style.setProperty('--rx', ((0.5 - y) * 16) + 'deg');
                    card.style.setProperty('--ry', ((x - 0.5) * 16) + 'deg');
                });
            }, { passive: true });

            card.addEventListener('pointerleave', function () {
                window.cancelAnimationFrame(frame);
                frame = window.requestAnimationFrame(function () {
                    card.style.setProperty('--mx', '50%');
                    card.style.setProperty('--my', '50%');
                    card.style.setProperty('--rx', '0deg');
                    card.style.setProperty('--ry', '0deg');
                });
            }, { passive: true });
        });
    }
})();
