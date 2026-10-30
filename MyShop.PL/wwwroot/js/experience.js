(function () {
    'use strict';

    var favoritesKey = 'myshop-favorites';
    var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    function track(eventName, details) {
        var payload = Object.assign({ event: eventName, myshop_event: 'myshop_' + eventName }, details || {});
        window.dataLayer = window.dataLayer || [];
        window.dataLayer.push(payload);
        document.dispatchEvent(new CustomEvent('myshop:analytics', { detail: payload }));
    }

    window.myShopTrack = track;

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
        var productName = button.dataset.productName || (document.documentElement.lang === 'ar' ? 'المنتج' : 'product');
        button.setAttribute('aria-label', document.documentElement.lang === 'ar'
            ? (isActive ? 'إزالة ' + productName + ' من المفضلة' : 'إضافة ' + productName + ' إلى المفضلة')
            : (isActive ? 'Remove ' + productName + ' from favorites' : 'Add ' + productName + ' to favorites'));
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
            amount: Number(button.dataset.productAmount || (card && card.dataset.productAmount)) || 0,
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
        card.dataset.productCard = '';
        card.dataset.productId = String(item.id);
        card.dataset.productName = item.name || 'Product';
        card.dataset.productCategory = item.category || '';
        card.dataset.productPrice = item.price || '';
        card.dataset.productAmount = String(Number(item.amount) || 0);

        var media = element('div', 'store-product-media');
        if (item.image) {
            var image = document.createElement('img');
            image.src = sameOriginPath(item.image);
            image.alt = item.name;
            image.width = 640;
            image.height = 480;
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
        heart.dataset.productCategory = item.category || '';
        heart.dataset.productPrice = item.price || '';
        heart.dataset.productAmount = String(Number(item.amount) || 0);
        heart.dataset.productImage = item.image || '';
        heart.dataset.productDetailsUrl = item.detailsUrl || '';
        heart.dataset.addToCartUrl = item.addToCartUrl || fallbackCartUrl;
        heart.setAttribute('aria-pressed', 'true');
        heart.setAttribute('aria-label', 'Remove ' + (item.name || 'product') + ' from favorites');
        heart.appendChild(element('i', 'fas fa-heart'));
        media.appendChild(heart);

        var category = element('span', 'product-category-badge', item.category || 'Featured');
        category.setAttribute('data-no-translate', '');
        media.appendChild(category);

        var body = element('div', 'store-product-body');
        if (Number(item.reviewCount) > 0) {
            var rating = element('div', 'store-rating');
            rating.setAttribute('aria-label', 'Rated ' + Number(item.averageRating).toFixed(1) + ' out of 5 from ' + item.reviewCount + ' reviews');
            for (var star = 1; star <= 5; star += 1) {
                var iconName = Number(item.averageRating) >= star ? 'fas fa-star' : Number(item.averageRating) >= star - .5 ? 'fas fa-star-half-alt' : 'far fa-star';
                rating.appendChild(element('i', iconName));
            }
            rating.appendChild(element('span', '', Number(item.averageRating).toFixed(1) + ' (' + item.reviewCount + ')'));
            body.appendChild(rating);
        }
        var title = element('h2', 'store-product-title', item.name || 'Product');
        title.setAttribute('data-no-translate', '');
        body.appendChild(title);
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
        var app = document.querySelector('.store-app');
        var authenticated = !!app && app.dataset.userAuthenticated === 'true';
        var favoritePage = document.querySelector('[data-favorites-page]');
        var grid = favoritePage && favoritePage.querySelector('[data-favorites-grid]');
        var empty = favoritePage && favoritePage.querySelector('[data-favorites-empty]');
        var loading = favoritePage && favoritePage.querySelector('[data-favorites-loading]');
        var serverLoading = authenticated && !!app.dataset.favoritesReadUrl;
        var savedToken = document.querySelector('[data-favorites-antiforgery] input[name="__RequestVerificationToken"]')
            || document.querySelector('.favorites-token-source input[name="__RequestVerificationToken"]');

        function token() { return savedToken ? savedToken.value : ''; }

        function request(url, method, data) {
            if (!url) return Promise.reject(new Error('The favorites service is unavailable.'));
            var headers = { 'X-Requested-With': 'XMLHttpRequest' };
            if (method !== 'GET') {
                headers['Content-Type'] = 'application/json';
                if (token()) headers.RequestVerificationToken = token();
            }
            return fetch(url, {
                method: method,
                credentials: 'same-origin',
                headers: headers,
                body: method === 'GET' ? undefined : JSON.stringify(data || {})
            }).then(function (response) {
                return response.json().catch(function () { return {}; }).then(function (payload) {
                    if (!response.ok) throw new Error(payload.message || 'Could not update your favorites.');
                    return payload;
                });
            });
        }

        function normalizeServerFavorites(payload) {
            var result = payload && (payload.favorites || payload.Favorites);
            if (!Array.isArray(result)) return [];
            return result.map(function (item) {
                var id = item.id || item.Id;
                var price = item.price == null ? item.Price : item.price;
                var category = item.categoryName || item.CategoryName || '';
                var image = item.imageUrl || item.ImageUrl || '';
                var rating = item.averageRating == null ? item.AverageRating : item.averageRating;
                var reviewCount = item.reviewCount == null ? item.ReviewCount : item.reviewCount;
                return {
                    id: String(id),
                    name: item.name || item.Name || 'Product',
                    category: category,
                    price: priceText(price),
                    amount: Number(price) || 0,
                    image: sameOriginPath(image),
                    detailsUrl: '/Store/Details/' + encodeURIComponent(id),
                    addToCartUrl: app.dataset.favoritesAddToCartUrl || '/Cart/AddToCart',
                    averageRating: Number(rating) || 0,
                    reviewCount: Number(reviewCount) || 0
                };
            }).filter(function (item) { return item.id !== 'undefined' && item.id !== 'null'; });
        }

        function priceText(value) {
            var amount = Number(value);
            if (!Number.isFinite(amount)) return String(value || '');
            try {
                return new Intl.NumberFormat(document.documentElement.lang === 'ar' ? 'ar-EG' : 'en-EG', {
                    style: 'currency', currency: 'EGP', maximumFractionDigits: 2
                }).format(amount);
            } catch (_) {
                return amount.toFixed(2) + ' EGP';
            }
        }

        function render() {
            if (!favoritePage || !grid) return;
            if (loading) loading.hidden = !serverLoading;
            grid.hidden = serverLoading;
            grid.replaceChildren();
            var fallbackCartUrl = favoritePage.dataset.addToCartUrl || '/Cart/AddToCart';
            items.forEach(function (item, index) {
                var card = buildFavoriteCard(item, token(), fallbackCartUrl);
                card.style.setProperty('--reveal-delay', Math.min(index * 55, 330) + 'ms');
                grid.appendChild(card);
            });
            if (empty) empty.hidden = serverLoading || items.length > 0;
        }

        function persistFromButton(button) {
            var product = productFromButton(button);
            if (!product.id) return Promise.resolve();
            var active = button.classList.contains('active');
            var existingIndex = items.findIndex(function (item) { return String(item.id) === product.id; });
            var previousItems = items.slice();

            if (active) {
                if (existingIndex >= 0) items[existingIndex] = Object.assign({}, items[existingIndex], product);
                else items.unshift(product);
                if (!authenticated && !writeFavorites(items)) {
                    items = items.filter(function (item) { return String(item.id) !== product.id; });
                    setFavoriteState(button, false);
                    notifyFavorite('danger', 'Could not save favorite', 'Allow site storage and try again.');
                    return Promise.resolve();
                }
            } else if (existingIndex >= 0) {
                items.splice(existingIndex, 1);
                if (!authenticated) writeFavorites(items);
                var renderedCard = button.closest('[data-favorite-card]');
                if (renderedCard) {
                    renderedCard.classList.add('is-removing');
                    window.setTimeout(render, reduceMotion ? 0 : 220);
                }
            }

            updateFavoriteBadges(items);
            syncFavoriteButtons(items);
            if (favoritePage && !button.hasAttribute('data-favorite-rendered')) render();

            if (!authenticated) {
                notifyFavorite(active ? 'success' : 'info', active ? 'Saved to favorites' : 'Removed from favorites',
                    active ? product.name + ' is ready when you come back.' : product.name + ' was removed from your list.');
                track('favorite_' + (active ? 'add' : 'remove'), { product_id: Number(product.id) || null });
                return Promise.resolve();
            }

            var endpoint = active ? app.dataset.favoritesAddUrl : app.dataset.favoritesRemoveUrl;
            button.disabled = true;
            return request(endpoint, 'POST', { productId: Number(product.id) })
                .then(function () {
                    writeFavorites(items);
                    notifyFavorite(active ? 'success' : 'info', active ? 'Saved to favorites' : 'Removed from favorites',
                        active ? product.name + ' is saved to your account.' : product.name + ' was removed from your account.');
                    track('favorite_' + (active ? 'add' : 'remove'), { product_id: Number(product.id) || null });
                })
                .catch(function (error) {
                    items = previousItems;
                    syncFavoriteButtons(items);
                    updateFavoriteBadges(items);
                    if (favoritePage) render();
                    notifyFavorite('danger', 'Could not update favorites', error.message);
                })
                .finally(function () { button.disabled = false; });
        }

        document.querySelectorAll('[data-wishlist-button][data-product-id]').forEach(function (button) {
            button.setAttribute('aria-pressed', button.classList.contains('active') ? 'true' : 'false');
        });

        syncFavoriteButtons(items);
        updateFavoriteBadges(items);
        render();

        if (authenticated && app.dataset.favoritesReadUrl) {
            var guestIds = items.map(function (item) { return Number(item.id); }).filter(function (id) { return id > 0; });
            var merge = guestIds.length
                ? request(app.dataset.favoritesSyncUrl, 'POST', { productIds: guestIds })
                : Promise.resolve();
            merge.then(function () {
                return request(app.dataset.favoritesReadUrl, 'GET');
            }).then(function (payload) {
                items = normalizeServerFavorites(payload);
                writeFavorites(items);
                serverLoading = false;
                syncFavoriteButtons(items);
                updateFavoriteBadges(items);
                render();
                track('favorites_sync', { favorite_count: items.length });
            }).catch(function (error) {
                serverLoading = false;
                render();
                notifyFavorite('danger', 'Favorites could not be loaded', error.message);
            });
        }

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

    function initializeAnalyticsEvents() {
        var products = Array.prototype.slice.call(document.querySelectorAll('[data-product-card][data-product-id]'));
        if (products.length) {
            track('view_item_list', {
                item_count: products.length,
                items: products.map(function (card) {
                    return {
                        item_id: card.dataset.productId,
                        item_name: card.dataset.productName || '',
                        item_category: card.dataset.productCategory || '',
                        price: Number(card.dataset.productAmount) || undefined
                    };
                })
            });
        }

        var detail = document.querySelector('[data-product-view]');
        if (detail) {
            var productPrice = Number(detail.dataset.productAmount) || 0;
            track('view_item', {
                currency: 'EGP',
                value: productPrice,
                items: [{
                    item_id: detail.dataset.productId,
                    item_name: detail.dataset.productName || '',
                    item_category: detail.dataset.productCategory || '',
                    price: productPrice,
                    quantity: 1
                }]
            });
        }

        var checkout = document.querySelector('[data-analytics-checkout]');
        if (checkout) {
            track('begin_checkout', {
                currency: 'EGP',
                value: Number(checkout.dataset.checkoutValue) || 0,
                item_count: Number(checkout.dataset.checkoutItems) || 0
            });
        }

        if (new URLSearchParams(window.location.search).has('searchTerm')) {
            track('search', { has_search_term: true });
        }

        document.addEventListener('submit', function (event) {
            var form = event.target;
            if (!(form instanceof HTMLFormElement)) return;
            if (form.matches('.auth-form')) {
                track(form.action.toLowerCase().indexOf('register') >= 0 ? 'sign_up_attempt' : 'login_attempt');
            }
        });

        document.addEventListener('click', function (event) {
            var target = event.target.closest('[data-paymob-start], [data-stripe-submit]');
            if (target) track('select_payment_method', { provider: target.hasAttribute('data-paymob-start') ? 'paymob' : 'stripe' });
        });
    }

    initializeFavorites();
    initializeScrollReveal();
    initializePageProgress();
    initializeAnalyticsEvents();
})();
