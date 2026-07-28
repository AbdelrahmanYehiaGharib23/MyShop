(function () {
    var root = document.documentElement;
    var prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    var themeToggle = document.querySelector('[data-store-theme-toggle]');
    var navbar = document.querySelector('.store-navbar');
    var searchInput = document.querySelector('.store-search-input');
    var categoryButtons = Array.prototype.slice.call(document.querySelectorAll('[data-category-filter]'));
    var productCards = Array.prototype.slice.call(document.querySelectorAll('[data-product-card]'));
    var noResults = document.querySelector('[data-store-no-results]');
    var cartTarget = document.querySelector('[data-store-cart-target]');
    var cartBadge = document.querySelector('[data-store-cart-badge]');
    var activeCategory = 'all';

    function normalize(value) {
        return (value || '').toString().trim().toLowerCase();
    }

    function ensureToastStack() {
        var stack = document.querySelector('[data-store-toast-stack]');

        if (!stack) {
            stack = document.createElement('div');
            stack.className = 'store-toast-stack';
            stack.setAttribute('data-store-toast-stack', '');
            stack.setAttribute('role', 'status');
            stack.setAttribute('aria-live', 'polite');
            document.body.appendChild(stack);
        }

        return stack;
    }

    function dismissToast(toast, delay) {
        window.setTimeout(function () {
            toast.classList.add('is-hiding');
            window.setTimeout(function () {
                toast.remove();
            }, 260);
        }, delay || 4200);
    }

    function toastIcon(type) {
        if (type === 'danger') {
            return 'fas fa-exclamation-circle';
        }

        if (type === 'info') {
            return 'fas fa-info-circle';
        }

        return 'fas fa-check-circle';
    }

    function showToast(type, title, message) {
        var toast = document.createElement('div');
        var content = document.createElement('div');
        var heading = document.createElement('strong');
        var body = document.createElement('span');
        var icon = document.createElement('span');

        toast.className = 'store-toast ' + (type || 'success');
        toast.setAttribute('data-store-toast', '');
        icon.className = 'store-toast-icon';
        icon.innerHTML = '<i class="' + toastIcon(type) + '"></i>';
        heading.textContent = title || 'Done';
        body.textContent = message || '';

        content.appendChild(heading);
        content.appendChild(body);
        toast.appendChild(icon);
        toast.appendChild(content);
        ensureToastStack().appendChild(toast);
        dismissToast(toast, 4200);
    }

    function updateThemeIcon(theme) {
        if (!themeToggle) {
            return;
        }

        var icon = themeToggle.querySelector('i');
        if (!icon) {
            return;
        }

        icon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
        themeToggle.setAttribute('aria-label', theme === 'dark' ? 'Switch to light theme' : 'Switch to dark theme');
    }

    function setTheme(theme) {
        root.setAttribute('data-store-theme', theme);
        localStorage.setItem('myshop-theme', theme);
        updateThemeIcon(theme);
    }

    function applyProductFilters() {
        var term = normalize(searchInput && searchInput.value);
        var category = normalize(activeCategory);
        var visibleCount = 0;

        productCards.forEach(function (card) {
            var name = normalize(card.getAttribute('data-product-name'));
            var cardCategory = normalize(card.getAttribute('data-product-category'));
            var matchesSearch = !term || name.indexOf(term) !== -1 || cardCategory.indexOf(term) !== -1;
            var matchesCategory = category === 'all' || cardCategory === category;
            var isVisible = matchesSearch && matchesCategory;

            card.classList.toggle('store-hidden', !isVisible);
            if (isVisible) {
                visibleCount += 1;
            }
        });

        if (noResults) {
            noResults.classList.toggle('store-hidden', visibleCount > 0 || productCards.length === 0);
        }
    }

    function updateNavbarState() {
        if (!navbar) {
            return;
        }

        navbar.classList.toggle('is-scrolled', window.scrollY > 10);
    }

    function addRipple(event) {
        var target = event.target.closest('.store-button-primary, .store-button-secondary, .store-button-danger, .store-button-ghost, .store-icon-button, .store-cart-button, .store-user-button, .category-chip');

        if (!target || target.disabled || prefersReducedMotion) {
            return;
        }

        var rect = target.getBoundingClientRect();
        var ripple = document.createElement('span');
        ripple.className = 'store-ripple';
        ripple.style.left = (event.clientX - rect.left) + 'px';
        ripple.style.top = (event.clientY - rect.top) + 'px';
        target.appendChild(ripple);

        window.setTimeout(function () {
            ripple.remove();
        }, 620);
    }

    function parseBadgeCount() {
        if (!cartBadge) {
            return 0;
        }

        var current = parseInt(cartBadge.textContent, 10);
        return Number.isNaN(current) ? 0 : current;
    }

    function pulseCart(quantity) {
        if (!cartBadge) {
            return;
        }

        cartBadge.textContent = parseBadgeCount() + quantity;
        cartBadge.classList.remove('is-pulsing');
        void cartBadge.offsetWidth;
        cartBadge.classList.add('is-pulsing');

        if (cartTarget) {
            cartTarget.classList.remove('cart-received', 'cart-bounce');
            void cartTarget.offsetWidth;
            cartTarget.classList.add('cart-received', 'cart-bounce');

            window.setTimeout(function () {
                cartTarget.classList.remove('cart-received', 'cart-bounce');
            }, 1050);
        }
    }

    function getProductName(form) {
        var card = form.closest('[data-product-card]');
        var detailTitle = document.querySelector('.detail-panel h1');

        if (card && card.getAttribute('data-product-name')) {
            return card.getAttribute('data-product-name');
        }

        if (detailTitle) {
            return detailTitle.textContent.trim();
        }

        return 'Product';
    }

    function getProductVisual(form) {
        var shell = form.closest('[data-product-card]');
        var detail = form.closest('.detail-layout');

        if (shell) {
            return shell.querySelector('.store-product-media img, .store-product-media .placeholder-illustration');
        }

        if (detail) {
            return detail.querySelector('.detail-media img, .detail-media .placeholder-illustration');
        }

        return null;
    }

    function flyToCart(source) {
        if (prefersReducedMotion || !source || !cartTarget || !source.getBoundingClientRect || !cartTarget.getBoundingClientRect) {
            return;
        }

        var sourceRect = source.getBoundingClientRect();
        var targetRect = cartTarget.getBoundingClientRect();
        var flyer = document.createElement('div');
        var startX = sourceRect.left + (sourceRect.width / 2) - 37;
        var startY = sourceRect.top + (sourceRect.height / 2) - 37;
        var endX = targetRect.left + (targetRect.width / 2) - 37;
        var endY = targetRect.top + (targetRect.height / 2) - 37;

        flyer.className = 'store-fly-image';

        if (source.tagName && source.tagName.toLowerCase() === 'img') {
            flyer.style.backgroundImage = 'url("' + (source.currentSrc || source.src) + '")';
        } else {
            flyer.innerHTML = source.outerHTML;
            flyer.classList.add('store-fly-placeholder');
        }

        flyer.style.left = startX + 'px';
        flyer.style.top = startY + 'px';
        document.body.appendChild(flyer);

        var animation = flyer.animate([
            { transform: 'translate3d(0, 0, 0) scale(1)', opacity: 1 },
            { transform: 'translate3d(' + ((endX - startX) * .55) + 'px, ' + ((endY - startY) - 48) + 'px, 0) scale(.72)', opacity: .92, offset: .58 },
            { transform: 'translate3d(' + (endX - startX) + 'px, ' + (endY - startY) + 'px, 0) scale(.28)', opacity: 0 }
        ], {
            duration: 880,
            easing: 'cubic-bezier(.2, .8, .2, 1)'
        });

        animation.onfinish = function () {
            flyer.remove();
        };
    }

    function restoreButton(button, html) {
        button.classList.remove('is-loading', 'is-added');
        button.disabled = false;
        button.innerHTML = html;
    }

    function enhanceCartForms() {
        if (!window.fetch || !window.FormData) {
            return;
        }

        document.querySelectorAll('.store-cart-form').forEach(function (form) {
            if (!/AddToCart/i.test(form.getAttribute('action') || '')) {
                return;
            }

            form.addEventListener('submit', function (event) {
                var button = form.querySelector('button[type="submit"]');
                var quantityInput = form.querySelector('input[name="quantity"]');
                var quantity = parseInt(quantityInput && quantityInput.value, 10);
                var originalHtml = button ? button.innerHTML : '';
                var productName = getProductName(form);

                if (!button || form.dataset.storeSubmitting === 'true') {
                    return;
                }

                event.preventDefault();
                form.dataset.storeSubmitting = 'true';
                button.disabled = true;
                button.classList.add('is-loading');
                button.innerHTML = '<span class="store-button-loader" aria-hidden="true"></span><span>Adding...</span>';

                flyToCart(getProductVisual(form));

                fetch(form.action, {
                    method: form.method || 'POST',
                    body: new FormData(form),
                    credentials: 'same-origin',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                }).then(function (response) {
                    if (!response.ok) {
                        throw new Error('Add to cart failed');
                    }

                    return response.text();
                }).then(function () {
                    pulseCart(Number.isNaN(quantity) ? 1 : quantity);
                    button.classList.remove('is-loading');
                    button.classList.add('is-added');
                    button.innerHTML = '<i class="fas fa-check"></i><span>Added</span>';
                    showToast('success', 'Added to cart', productName + ' is now in your cart.');

                    window.setTimeout(function () {
                        form.dataset.storeSubmitting = 'false';
                        restoreButton(button, originalHtml);
                    }, 1450);
                }).catch(function () {
                    form.dataset.storeSubmitting = 'false';
                    restoreButton(button, originalHtml);
                    showToast('danger', 'Could not add item', 'Please try adding the product again.');
                });
            });
        });
    }

    function animateCount(element) {
        if (element.dataset.storeAnimated === 'true') {
            return;
        }

        element.dataset.storeAnimated = 'true';

        var to = Number(element.getAttribute('data-count-to') || 0);
        var format = element.getAttribute('data-count-format');
        var start = performance.now();
        var duration = prefersReducedMotion ? 1 : 1050;

        function formatValue(value) {
            if (format === 'currency') {
                return new Intl.NumberFormat(undefined, {
                    style: 'currency',
                    currency: 'USD',
                    maximumFractionDigits: 2
                }).format(value);
            }

            return Math.round(value).toLocaleString();
        }

        function tick(now) {
            var progress = Math.min((now - start) / duration, 1);
            var eased = 1 - Math.pow(1 - progress, 3);

            element.textContent = formatValue(to * eased);

            if (progress < 1) {
                window.requestAnimationFrame(tick);
            } else {
                element.textContent = formatValue(to);
            }
        }

        window.requestAnimationFrame(tick);
    }

    function initCountAnimations() {
        var counters = Array.prototype.slice.call(document.querySelectorAll('[data-store-count]'));

        if (!counters.length) {
            return;
        }

        if (!('IntersectionObserver' in window)) {
            counters.forEach(animateCount);
            return;
        }

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    animateCount(entry.target);
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: .35 });

        counters.forEach(function (counter) {
            observer.observe(counter);
        });
    }

    function initImageLoaders() {
        document.querySelectorAll('.store-product-media, .detail-media, .cart-item-media').forEach(function (shell) {
            var image = shell.querySelector('img');

            if (!image) {
                shell.classList.add('has-loaded');
                return;
            }

            function markLoaded() {
                shell.classList.add('has-loaded');
            }

            if (image.complete) {
                markLoaded();
            } else {
                image.addEventListener('load', markLoaded, { once: true });
                image.addEventListener('error', markLoaded, { once: true });
            }
        });
    }

    updateThemeIcon(root.getAttribute('data-store-theme') || 'dark');
    updateNavbarState();
    initImageLoaders();
    initCountAnimations();
    enhanceCartForms();

    productCards.forEach(function (card, index) {
        card.style.animationDelay = Math.min(index * 70, 420) + 'ms';
    });

    if (themeToggle) {
        themeToggle.addEventListener('click', function () {
            var nextTheme = root.getAttribute('data-store-theme') === 'dark' ? 'light' : 'dark';
            setTheme(nextTheme);
        });
    }

    if (searchInput && productCards.length) {
        searchInput.addEventListener('input', applyProductFilters);
    }

    categoryButtons.forEach(function (button) {
        button.addEventListener('click', function () {
            activeCategory = button.getAttribute('data-category-filter') || 'all';

            categoryButtons.forEach(function (item) {
                item.classList.toggle('active', item === button);
            });

            applyProductFilters();
        });
    });

    document.querySelectorAll('[data-wishlist-button]').forEach(function (button) {
        button.setAttribute('aria-pressed', 'false');

        button.addEventListener('click', function () {
            var icon = button.querySelector('i');
            var isActive = !button.classList.contains('active');

            button.classList.toggle('active', isActive);
            button.setAttribute('aria-pressed', isActive ? 'true' : 'false');

            if (icon) {
                icon.classList.toggle('fas', isActive);
                icon.classList.toggle('far', !isActive);
            }
        });
    });

    document.addEventListener('click', addRipple);
    window.addEventListener('scroll', updateNavbarState, { passive: true });

    document.querySelectorAll('[data-store-toast]').forEach(function (toast, index) {
        dismissToast(toast, 4200 + (index * 450));
    });
})();
