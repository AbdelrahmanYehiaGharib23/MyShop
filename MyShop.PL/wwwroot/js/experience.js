(function () {
    'use strict';

    var favoritesKey = 'myshop-favorites';
    var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    function readFavorites() {
        try {
            var value = JSON.parse(localStorage.getItem(favoritesKey) || '[]');
            return Array.isArray(value) ? value.filter(function (item) { return item && item.id; }) : [];
        } catch (_) {
            return [];
        }
    }

    function writeFavorites(items) {
        try {
            localStorage.setItem(favoritesKey, JSON.stringify(items));
            return true;
        } catch (_) {
            return false;
        }
    }

    function pulseFavorite(button) {
        if (reduceMotion) return;
        button.classList.remove('is-burst');
        void button.offsetWidth;
        button.classList.add('is-burst');
        window.setTimeout(function () { button.classList.remove('is-burst'); }, 380);
    }

    function setFavoriteState(button, isActive, animate) {
        var wasActive = button.classList.contains('active');
        button.classList.toggle('active', isActive);
        button.setAttribute('aria-pressed', isActive ? 'true' : 'false');
        button.setAttribute('aria-label', (isActive ? 'Remove ' : 'Add ') + (button.dataset.productName || 'product') + ' ' + (isActive ? 'from' : 'to') + ' favorites');
        var icon = button.querySelector('i');
        if (icon) {
            icon.classList.toggle('fas', isActive);
            icon.classList.toggle('far', !isActive);
        }
        if (isActive && !wasActive && animate !== false) pulseFavorite(button);
    }

    function updateFavoriteBadges(items) {
        document.querySelectorAll('[data-favorites-count]').forEach(function (badge) {
            badge.textContent = items.length > 99 ? '99+' : String(items.length);
            badge.hidden = items.length === 0;
        });
        document.querySelectorAll('[data-favorites-total]').forEach(function (label) {
            label.textContent = String(items.length);
        });
    }

    function notifyFavorite(type, title, message) {
        var stack = document.querySelector('[data-store-toast-stack]');
        if (!stack) {
            stack = document.createElement('div');
            stack.className = 'store-toast-stack';
            stack.setAttribute('data-store-toast-stack', '');
            stack.setAttribute('role', 'status');
            stack.setAttribute('aria-live', 'polite');
            document.body.appendChild(stack);
        }
        var toast = element('div', 'store-toast ' + type);
        var icon = element('span', 'store-toast-icon');
        var content = document.createElement('div');
        var iconName = type === 'danger' ? 'fa-exclamation-circle' : type === 'info' ? 'fa-info-circle' : 'fa-check-circle';
        icon.appendChild(element('i', 'fas ' + iconName));
        content.appendChild(element('strong', '', title));
        content.appendChild(element('span', '', message));
        toast.appendChild(icon);
        toast.appendChild(content);
        stack.appendChild(toast);
        window.setTimeout(function () {
            toast.classList.add('is-hiding');
            window.setTimeout(function () { toast.remove(); }, 280);
        }, 3500);
    }

    function syncFavoriteButtons(items) {
        var ids = new Set(items.map(function (item) { return String(item.id); }));
        document.querySelectorAll('[data-wishlist-button][data-product-id]').forEach(function (button) {
            setFavoriteState(button, ids.has(String(button.dataset.productId)), false);
        });
    }

    function sameOriginPath(value) {
        if (!value) return '';
        try {
            var url = new URL(value, window.location.origin);
            return url.origin === window.location.origin ? url.pathname + url.search + url.hash : '';
        } catch (_) {
            return '';
        }
    }

    function productFromButton(button) {
        var card = button.closest('[data-product-card]');
        var image = card && card.querySelector('.store-product-media img');
        return {
            id: String(button.dataset.productId || (card && card.dataset.productId) || ''),
            name: button.dataset.productName || (card && card.dataset.productName) || 'Product',
            category: button.dataset.productCategory || (card && card.dataset.productCategory) || 'Featured',
            price: button.dataset.productPrice || (card && card.dataset.productPrice) || '',
            image: sameOriginPath(button.dataset.productImage || (card && card.dataset.productImage) || (image && image.getAttribute('src')) || ''),
            detailsUrl: sameOriginPath(button.dataset.productDetailsUrl || (card && card.dataset.productDetailsUrl) || ''),
            addToCartUrl: sameOriginPath(button.dataset.addToCartUrl || (card && card.dataset.addToCartUrl) || '')
        };
    }

    function element(tag, className, text) {
        var node = document.createElement(tag);
        if (className) node.className = className;
        if (text) node.textContent = text;
        return node;
    }

    function makeHiddenInput(name, value) {
        var input = document.createElement('input');
        input.type = 'hidden';
        input.name = name;
        input.value = value;
        return input;
    }

    function buildFavoriteCard(item, token, fallbackCartUrl) {
        var card = element('article', 'store-product-card store-fade-in');
        card.dataset.favoriteCard = item.id;

        var media = element('div', 'store-product-media');
        if (item.image) {
            var image = document.createElement('img');
            image.src = sameOriginPath(item.image);
            image.alt = item.name;
            image.loading = 'lazy';
            image.decoding = 'async';
            image.addEventListener('load', function () { media.classList.add('has-loaded'); }, { once: true });
            image.addEventListener('error', function () { media.classList.add('has-loaded'); }, { once: true });
            media.appendChild(image);
            if (image.complete) media.classList.add('has-loaded');
        } else {
            var fallback = element('div', 'store-product-fallback');
            var fallbackIcon = element('i', 'fas fa-box-open');
            fallbackIcon.setAttribute('aria-hidden', 'true');
            fallback.appendChild(fallbackIcon);
            media.appendChild(fallback);
        }

        var heart = element('button', 'wishlist-button active');
        heart.type = 'button';
        heart.dataset.wishlistButton = '';
        heart.dataset.favoriteRendered = '';
        heart.dataset.productId = String(item.id);
        heart.dataset.productName = item.name || 'Product';
        heart.setAttribute('aria-pressed', 'true');
        heart.setAttribute('aria-label', 'Remove ' + (item.name || 'product') + ' from favorites');
        heart.appendChild(element('i', 'fas fa-heart'));
        media.appendChild(heart);

        var category = element('span', 'product-category-badge', item.category || 'Featured');
        media.appendChild(category);

        var body = element('div', 'store-product-body');
        body.appendChild(element('h2', 'store-product-title', item.name || 'Product'));
        body.appendChild(element('p', 'store-product-subtitle', item.category ? 'Explore the ' + item.category + ' collection.' : 'Explore this product in our catalog.'));

        var meta = element('div', 'store-product-meta');
        meta.appendChild(element('span', 'store-price', item.price || ''));
        meta.appendChild(element('span', '', item.category || ''));
        body.appendChild(meta);

        var actions = element('div', 'store-product-actions');
        var form = document.createElement('form');
        form.className = 'store-cart-form';
        form.method = 'post';
        form.action = sameOriginPath(item.addToCartUrl || fallbackCartUrl);
        if (token) form.appendChild(makeHiddenInput('__RequestVerificationToken', token));
        form.appendChild(makeHiddenInput('productId', String(item.id)));
        form.appendChild(makeHiddenInput('quantity', '1'));
        var addButton = element('button', 'store-button-primary');
        addButton.type = 'submit';
        addButton.appendChild(element('i', 'fas fa-cart-plus'));
        addButton.appendChild(element('span', '', 'Add to Cart'));
        form.appendChild(addButton);
        actions.appendChild(form);

        var details = element('a', 'store-button-secondary store-details-button');
        details.href = sameOriginPath(item.detailsUrl || '/Store/Index');
        details.setAttribute('aria-label', 'View ' + (item.name || 'product') + ' details');
        details.appendChild(element('i', 'fas fa-eye'));
        actions.appendChild(details);
        body.appendChild(actions);

        card.appendChild(media);
        card.appendChild(body);
        return card;
    }

    function initializeFavorites() {
        var items = readFavorites();
        var favoritePage = document.querySelector('[data-favorites-page]');
        var grid = favoritePage && favoritePage.querySelector('[data-favorites-grid]');
        var empty = favoritePage && favoritePage.querySelector('[data-favorites-empty]');

        function render() {
            if (!favoritePage || !grid) return;
            grid.replaceChildren();
            var tokenInput = favoritePage.querySelector('input[name="__RequestVerificationToken"]');
            var token = tokenInput ? tokenInput.value : '';
            var fallbackCartUrl = favoritePage.dataset.addToCartUrl || '/Cart/AddToCart';
            items.forEach(function (item, index) {
                var card = buildFavoriteCard(item, token, fallbackCartUrl);
                card.style.setProperty('--reveal-delay', Math.min(index * 55, 330) + 'ms');
                grid.appendChild(card);
            });
            if (empty) empty.hidden = items.length > 0;
        }

        function persistFromButton(button) {
            var product = productFromButton(button);
            if (!product.id) return;
            var active = button.classList.contains('active');
            var existingIndex = items.findIndex(function (item) { return String(item.id) === product.id; });

            if (active) {
                if (existingIndex >= 0) items[existingIndex] = Object.assign({}, items[existingIndex], product);
                else items.unshift(product);
                if (!writeFavorites(items)) {
                    items = items.filter(function (item) { return String(item.id) !== product.id; });
                    setFavoriteState(button, false);
                    notifyFavorite('danger', 'Could not save favorite', 'Allow site storage and try again.');
                    return;
                }
                notifyFavorite('success', 'Saved to favorites', product.name + ' is ready when you come back.');
            } else if (existingIndex >= 0) {
                items.splice(existingIndex, 1);
                writeFavorites(items);
                var renderedCard = button.closest('[data-favorite-card]');
                if (renderedCard) {
                    renderedCard.classList.add('is-removing');
                    window.setTimeout(render, reduceMotion ? 0 : 220);
                }
                notifyFavorite('info', 'Removed from favorites', product.name + ' was removed from your list.');
            }

            updateFavoriteBadges(items);
            syncFavoriteButtons(items);
            if (favoritePage && !button.hasAttribute('data-favorite-rendered')) render();
        }

        document.querySelectorAll('[data-wishlist-button][data-product-id]').forEach(function (button) {
            button.setAttribute('aria-pressed', button.classList.contains('active') ? 'true' : 'false');
        });

        syncFavoriteButtons(items);
        updateFavoriteBadges(items);
        render();

        document.addEventListener('click', function (event) {
            var button = event.target.closest('[data-wishlist-button][data-product-id]');
            if (!button) return;
            if (button.hasAttribute('data-favorite-rendered')) {
                event.preventDefault();
                setFavoriteState(button, !button.classList.contains('active'));
            } else {
                var isActive = button.classList.contains('active');
                setFavoriteState(button, isActive, false);
                if (isActive) pulseFavorite(button);
            }
            persistFromButton(button);
        });
    }

    function initializeScrollReveal() {
        if (reduceMotion || !('IntersectionObserver' in window)) return;
        var targets = document.querySelectorAll('.shop-hero-copy, .shop-hero-media, .category-strip, .store-section-title, [data-product-card], .detail-layout > *, .checkout-hero, .checkout-panel, .checkout-summary, .cart-page-heading, .cart-list, .cart-summary, .orders-hero, .favorites-heading, [data-favorites-grid]');
        if (!targets.length) return;

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) return;
                entry.target.classList.add('is-revealed');
                observer.unobserve(entry.target);
            });
        }, { rootMargin: '0px 0px -48px 0px', threshold: 0.06 });

        targets.forEach(function (target, index) {
            target.classList.add('scroll-reveal');
            target.style.setProperty('--reveal-delay', Math.min(index % 8 * 48, 300) + 'ms');
            observer.observe(target);
        });
    }

    function initializePageProgress() {
        var bar = document.createElement('div');
        bar.className = 'store-page-progress';
        bar.setAttribute('aria-hidden', 'true');
        document.body.appendChild(bar);

        function start() { bar.classList.add('is-active'); }
        document.addEventListener('click', function (event) {
            if (event.defaultPrevented || event.button !== 0 || event.metaKey || event.ctrlKey || event.shiftKey || event.altKey) return;
            var link = event.target.closest('a[href]');
            if (!link || link.target === '_blank' || link.hasAttribute('download')) return;
            var href = link.getAttribute('href');
            if (!href || href.charAt(0) === '#' || link.hasAttribute('data-no-progress')) return;
            var url;
            try { url = new URL(href, window.location.href); } catch (_) { return; }
            if (url.origin !== window.location.origin || url.href === window.location.href) return;
            start();
        });

        document.addEventListener('submit', function (event) {
            if (event.defaultPrevented) return;
            var form = event.target;
            if (!(form instanceof HTMLFormElement) || form.target === '_blank' || form.noValidate) return;
            if (form.matches('[data-no-progress], [data-stripe-checkout], [data-paymob-checkout]')) return;
            start();
        });

        window.addEventListener('pageshow', function () { bar.classList.remove('is-active'); });
    }

    initializeFavorites();
    initializeScrollReveal();
    initializePageProgress();
})();
