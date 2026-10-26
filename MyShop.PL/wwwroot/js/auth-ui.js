(function () {
    document.querySelectorAll('.auth-form').forEach(function (form) {
        form.addEventListener('submit', function () {
            if (!form.checkValidity()) return;
            var button = form.querySelector('.auth-btn[type="submit"]');
            if (!button || button.classList.contains('is-submitting')) return;
            button.classList.add('is-submitting');
            button.setAttribute('aria-busy', 'true');
            button.setAttribute('aria-label', button.textContent.trim());
            form.closest('.auth-card')?.classList.add('is-submitting');
        });
    });

    document.querySelectorAll('.auth-form .input-group input:not([type="hidden"])').forEach(function (input) {
        var wrapper = input.closest('.input-group');
        if (wrapper && input.type === 'password' && !wrapper.querySelector('.auth-password-toggle')) {
            var toggle = document.createElement('button');
            toggle.type = 'button';
            toggle.className = 'auth-password-toggle';
            toggle.setAttribute('aria-label', 'Show password');
            toggle.setAttribute('aria-pressed', 'false');
            toggle.innerHTML = '<i class="fas fa-eye" aria-hidden="true"></i>';
            wrapper.appendChild(toggle);
            toggle.addEventListener('click', function () {
                var reveal = input.type === 'password';
                input.type = reveal ? 'text' : 'password';
                toggle.setAttribute('aria-pressed', reveal ? 'true' : 'false');
                toggle.setAttribute('aria-label', reveal ? 'Hide password' : 'Show password');
                toggle.innerHTML = '<i class="fas fa-' + (reveal ? 'eye-slash' : 'eye') + '" aria-hidden="true"></i>';
            });
        }
    });
})();
