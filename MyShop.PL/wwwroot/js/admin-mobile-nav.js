(() => {
    const body = document.body;
    const toggle = document.querySelector('[data-admin-menu-toggle]');
    const closeButton = document.querySelector('[data-admin-menu-close]');
    const sidebar = document.getElementById('adminSidebar');
    if (!body || !toggle || !closeButton || !sidebar) return;

    const mobileQuery = window.matchMedia('(max-width: 991.98px)');
    const setOpen = (open) => {
        body.classList.toggle('admin-mobile-menu-open', open);
        closeButton.setAttribute('aria-hidden', String(!open));
        toggle.setAttribute('aria-expanded', String(open));
    };

    toggle.addEventListener('click', () => {
        if (mobileQuery.matches) {
            setOpen(!body.classList.contains('admin-mobile-menu-open'));
        } else {
            body.classList.toggle('sidebar-collapse');
            toggle.setAttribute('aria-expanded', String(!body.classList.contains('sidebar-collapse')));
        }
    });

    closeButton.addEventListener('click', () => setOpen(false));
    sidebar.querySelectorAll('.nav-sidebar .nav-link').forEach((link) => {
        link.addEventListener('click', () => {
            if (mobileQuery.matches) setOpen(false);
        });
    });
    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape' && body.classList.contains('admin-mobile-menu-open')) {
            setOpen(false);
            toggle.focus();
        }
    });
    mobileQuery.addEventListener('change', (event) => {
        if (!event.matches) setOpen(false);
    });
})();
