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
        localStorage.setItem('myshop-theme', dark ? 'dark' : 'light');
        var themeColor = document.querySelector('meta[name="theme-color"]');
        if (themeColor) themeColor.setAttribute('content', dark ? '#0b1120' : '#f5f7fb');
        document.querySelectorAll('[data-theme-toggle]').forEach(function (button) {
            button.setAttribute('aria-pressed', dark ? 'true' : 'false');
            button.setAttribute('aria-label', root.lang === 'ar'
                ? (dark ? 'التبديل إلى الوضع الفاتح' : 'التبديل إلى الوضع الداكن')
                : (dark ? 'Switch to light theme' : 'Switch to dark theme'));
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

    function syncThemeEvent(event) {
        if (event.detail && event.detail.theme) paint(event.detail.theme);
    }
    document.addEventListener('store-theme-change', syncThemeEvent);
    window.addEventListener('store-theme-change', syncThemeEvent);
})();
