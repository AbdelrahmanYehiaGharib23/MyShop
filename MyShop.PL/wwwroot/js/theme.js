(function () {
    var root = document.documentElement;
    var stored = localStorage.getItem('myshop-theme');
    var prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    var initial = stored === 'dark' || stored === 'light'
        ? stored
        : (prefersDark ? 'dark' : 'light');

    function paint(theme) {
        var dark = theme === 'dark';
        root.setAttribute('data-theme', dark ? 'dark' : 'light');
        root.setAttribute('data-store-theme', dark ? 'dark' : 'light');
        if (document.body) document.body.setAttribute('data-theme', dark ? 'dark' : 'light');
        localStorage.setItem('myshop-theme', dark ? 'dark' : 'light');
        document.querySelectorAll('[data-theme-toggle]').forEach(function (button) {
            button.setAttribute('aria-pressed', dark ? 'true' : 'false');
            button.setAttribute('aria-label', dark ? 'Switch to light theme' : 'Switch to dark theme');
            var icon = button.querySelector('i');
            if (icon) {
                icon.classList.toggle('fa-moon', !dark);
                icon.classList.toggle('fa-sun', dark);
            }
            var label = button.querySelector('span');
            if (label) label.textContent = dark ? 'Light Mode' : 'Dark Mode';
        });
    }

    paint(initial);

    document.querySelectorAll('[data-theme-toggle]:not([data-store-theme-toggle])').forEach(function (button) {
        button.addEventListener('click', function () {
            paint(root.getAttribute('data-theme') === 'dark' ? 'light' : 'dark');
            document.dispatchEvent(new CustomEvent('store-theme-change', {
                detail: { theme: root.getAttribute('data-theme') }
            }));
        });
    });

    var sidebarToggle = document.querySelector('[data-admin-sidebar-toggle]');
    if (sidebarToggle) {
        function setSidebarOpen(open) {
            document.body.classList.toggle('admin-sidebar-open', open);
            sidebarToggle.setAttribute('aria-expanded', open ? 'true' : 'false');
            sidebarToggle.setAttribute('aria-label', open ? 'Close navigation' : 'Open navigation');
        }

        sidebarToggle.addEventListener('click', function () {
            if (window.matchMedia('(max-width: 767.98px)').matches) {
                setSidebarOpen(!document.body.classList.contains('admin-sidebar-open'));
            }
        });

        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') setSidebarOpen(false);
        });

        document.addEventListener('click', function (event) {
            if (document.body.classList.contains('admin-sidebar-open')
                && !event.target.closest('#admin-sidebar')
                && !event.target.closest('[data-admin-sidebar-toggle]')) {
                setSidebarOpen(false);
            }
        });

        window.addEventListener('resize', function () {
            if (!window.matchMedia('(max-width: 767.98px)').matches) setSidebarOpen(false);
        });

        document.querySelectorAll('#admin-sidebar .admin-nav-link').forEach(function (link) {
            link.addEventListener('click', function () { setSidebarOpen(false); });
        });
    }

    function syncThemeEvent(event) {
        if (event.detail && event.detail.theme) paint(event.detail.theme);
    }
    document.addEventListener('store-theme-change', syncThemeEvent);
    window.addEventListener('store-theme-change', syncThemeEvent);
})();
