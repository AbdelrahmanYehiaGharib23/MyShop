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
        window.dispatchEvent(new CustomEvent('store-theme-change', {
            detail: {
                theme: theme
            }
        }));
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
        var target = event.target.closest('.store-button-primary, .store-button-secondary, .store-button-danger, .store-button-ghost, .store-icon-button, .store-cart-button, .store-user-button, .category-chip, .review-star-button, .review-delete-button, .review-action-button');

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
        document.querySelectorAll('.store-product-media, .detail-media, .cart-item-media, .checkout-item-media').forEach(function (shell) {
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

    function initCheckoutPreview() {
        var button = document.querySelector('[data-checkout-preview]');

        if (!button) {
            return;
        }

        var originalHtml = button.innerHTML;

        button.addEventListener('click', function () {
            button.disabled = true;
            button.classList.add('is-added');
            button.innerHTML = '<i class="fas fa-check"></i><span>Order Reviewed</span>';
            showToast('success', 'Order reviewed', 'Your checkout summary is ready.');

            window.setTimeout(function () {
                button.disabled = false;
                button.classList.remove('is-added');
                button.innerHTML = originalHtml;
            }, 1600);
        });
    }

    function loadStripeLibrary() {
        if (window.Stripe) {
            return Promise.resolve(window.Stripe);
        }

        return new Promise(function (resolve, reject) {
            var existingScript = document.querySelector('script[data-stripe-js]');

            if (existingScript) {
                existingScript.addEventListener('load', function () {
                    resolve(window.Stripe);
                }, { once: true });
                existingScript.addEventListener('error', reject, { once: true });
                return;
            }

            var script = document.createElement('script');
            script.src = 'https://js.stripe.com/v3/';
            script.async = true;
            script.setAttribute('data-stripe-js', 'true');
            script.addEventListener('load', function () {
                if (window.Stripe) {
                    resolve(window.Stripe);
                } else {
                    reject(new Error('Stripe could not be loaded.'));
                }
            }, { once: true });
            script.addEventListener('error', function () {
                reject(new Error('Stripe could not be loaded.'));
            }, { once: true });
            document.head.appendChild(script);
        });
    }

    function initStripeCheckout() {
        var form = document.querySelector('[data-stripe-checkout]');

        if (!form || form.getAttribute('data-stripe-enabled') !== 'true') {
            return;
        }

        var publishableKey = form.getAttribute('data-stripe-publishable-key');
        var intentUrl = form.getAttribute('data-payment-intent-url');
        var cardNumberMount = form.querySelector('[data-stripe-card-number]');
        var cardExpiryMount = form.querySelector('[data-stripe-card-expiry]');
        var cardCvcMount = form.querySelector('[data-stripe-card-cvc]');
        var errorBox = form.querySelector('[data-stripe-errors]');
        var button = form.querySelector('[data-stripe-submit]');
        var paymentIntentInput = form.querySelector('[data-payment-intent-id]');
        var originalButtonHtml = button ? button.innerHTML : '';
        var stripe = null;
        var cardNumber = null;
        var cardExpiry = null;
        var cardCvc = null;

        function setPaymentError(message) {
            if (errorBox) {
                errorBox.textContent = message || '';
            }
        }

        function fieldValue(name) {
            var input = form.querySelector('[name="' + name + '"]');
            return input ? input.value.trim() : '';
        }

        function requestPaymentIntent() {
            var token = form.querySelector('input[name="__RequestVerificationToken"]');
            var headers = {
                'X-Requested-With': 'XMLHttpRequest'
            };

            if (token) {
                headers.RequestVerificationToken = token.value;
            }

            return fetch(intentUrl, {
                method: 'POST',
                credentials: 'same-origin',
                headers: headers
            }).then(function (response) {
                return response.text().then(function (text) {
                    var payload = null;

                    if (text) {
                        try {
                            payload = JSON.parse(text);
                        } catch (parseError) {
                            payload = null;
                        }
                    }

                    if (!response.ok) {
                        throw new Error((payload && payload.message) || 'Could not start the payment.');
                    }

                    return payload || {};
                });
            });
        }

        function setPaymentLoading() {
            if (!button) {
                return;
            }

            button.disabled = true;
            button.classList.remove('is-paid');
            button.classList.add('is-loading');
            button.innerHTML = '<span class="store-button-loader" aria-hidden="true"></span><span>Processing...</span>';
        }

        function resetPaymentButton() {
            if (!button) {
                return;
            }

            button.disabled = false;
            button.classList.remove('is-loading', 'is-paid');
            button.innerHTML = originalButtonHtml;
        }

        function submitConfirmedOrder() {
            form.dataset.stripePaymentConfirmed = 'true';
            HTMLFormElement.prototype.submit.call(form);
        }

        function stripeElementStyle() {
            var styles = window.getComputedStyle(root);
            var stripeText = styles.getPropertyValue('--store-ink').trim() || '#142033';
            var stripeMuted = styles.getPropertyValue('--store-muted').trim() || '#75849a';
            var stripePrimary = styles.getPropertyValue('--store-primary').trim() || '#0ea5e9';
            var stripeDanger = styles.getPropertyValue('--store-danger').trim() || '#ef5b5b';

            return {
                base: {
                    color: stripeText,
                    fontFamily: 'Poppins, system-ui, sans-serif',
                    fontSize: '16px',
                    fontSmoothing: 'antialiased',
                    iconColor: stripePrimary,
                    '::placeholder': {
                        color: stripeMuted
                    }
                },
                invalid: {
                    color: stripeDanger,
                    iconColor: stripeDanger
                }
            };
        }

        if (!publishableKey || !intentUrl || !cardNumberMount || !cardExpiryMount || !cardCvcMount || !button || !window.fetch) {
            return;
        }

        form.addEventListener('submit', function (event) {
            if (form.dataset.stripePaymentConfirmed === 'true') {
                return;
            }

            event.preventDefault();

            if (form.dataset.stripeProcessing === 'true') {
                return;
            }

            if (!form.checkValidity()) {
                form.reportValidity();
                showToast('info', 'Delivery details needed', 'Add your phone and address before payment.');
                return;
            }

            if (!stripe || !cardNumber || !cardExpiry || !cardCvc) {
                setPaymentError('Payment form is still loading. Please try again in a moment.');
                showToast('info', 'Payment loading', 'Stripe is preparing the secure payment form.');
                return;
            }

            form.dataset.stripeProcessing = 'true';
            setPaymentError('');
            setPaymentLoading();

            requestPaymentIntent()
                .then(function (payload) {
                    if (!payload.clientSecret) {
                        throw new Error('Payment client secret was not returned.');
                    }

                    if (!payload.paymentIntentId) {
                        throw new Error('Payment intent id was not returned.');
                    }

                    return stripe.confirmCardPayment(payload.clientSecret, {
                        payment_method: {
                            card: cardNumber,
                            billing_details: {
                                phone: fieldValue('Phone'),
                                address: {
                                    line1: fieldValue('Address')
                                }
                            }
                        }
                    }).then(function (result) {
                        return {
                            result: result,
                            paymentIntentId: payload.paymentIntentId
                        };
                    });
                })
                .then(function (payload) {
                    if (payload.result.error) {
                        throw new Error(payload.result.error.message || 'Payment was not completed.');
                    }

                    if (paymentIntentInput) {
                        paymentIntentInput.value = payload.paymentIntentId;
                    }

                    button.classList.remove('is-loading');
                    button.classList.add('is-paid');
                    button.innerHTML = '<i class="fas fa-check"></i><span>Payment Complete</span>';
                    showToast('success', 'Payment complete', 'Your payment was accepted. Creating your order now.');

                    window.setTimeout(submitConfirmedOrder, prefersReducedMotion ? 50 : 700);
                })
                .catch(function (error) {
                    form.dataset.stripeProcessing = 'false';
                    resetPaymentButton();
                    setPaymentError(error.message || 'Payment failed. Please try again.');
                    showToast('danger', 'Payment failed', error.message || 'Please check your card details and try again.');
                });
        });

        loadStripeLibrary()
            .then(function (StripeConstructor) {
                var elements = null;

                stripe = StripeConstructor(publishableKey);
                elements = stripe.elements();

                cardNumber = elements.create('cardNumber', {
                    showIcon: true,
                    placeholder: '4242 4242 4242 4242',
                    style: stripeElementStyle()
                });
                cardExpiry = elements.create('cardExpiry', {
                    style: stripeElementStyle()
                });
                cardCvc = elements.create('cardCvc', {
                    style: stripeElementStyle()
                });

                cardNumber.mount(cardNumberMount);
                cardExpiry.mount(cardExpiryMount);
                cardCvc.mount(cardCvcMount);

                [cardNumber, cardExpiry, cardCvc].forEach(function (element) {
                    element.on('change', function (event) {
                        setPaymentError(event.error ? event.error.message : '');
                    });
                });

                cardNumber.on('ready', function () {
                    setPaymentError('');
                });
            })
            .catch(function (error) {
                setPaymentError(error.message || 'Stripe could not be loaded.');
                showToast('danger', 'Payment unavailable', 'Could not load the secure payment form.');
            });

        window.addEventListener('store-theme-change', function () {
            [cardNumber, cardExpiry, cardCvc].forEach(function (element) {
                if (element) {
                    element.update({
                        style: stripeElementStyle()
                    });
                }
            });
        });
    }

    function readValue(source, camelName) {
        if (!source) {
            return null;
        }

        if (Object.prototype.hasOwnProperty.call(source, camelName)) {
            return source[camelName];
        }

        var pascalName = camelName.charAt(0).toUpperCase() + camelName.slice(1);
        return Object.prototype.hasOwnProperty.call(source, pascalName) ? source[pascalName] : null;
    }

    function reviewErrorMessage(payload, fallback) {
        if (payload && payload.message) {
            return payload.message;
        }

        if (payload && payload.errors) {
            var keys = Object.keys(payload.errors);
            if (keys.length && payload.errors[keys[0]] && payload.errors[keys[0]].length) {
                return payload.errors[keys[0]][0];
            }
        }

        return fallback || 'Please try again.';
    }

    function requestReviewJson(url, options) {
        return fetch(url, options).then(function (response) {
            return response.text().then(function (text) {
                var payload = null;

                if (text) {
                    try {
                        payload = JSON.parse(text);
                    } catch (parseError) {
                        payload = null;
                    }
                }

                if (!response.ok) {
                    throw new Error(reviewErrorMessage(payload, 'Review request failed.'));
                }

                return payload;
            });
        });
    }

    function formatReviewDate(value) {
        if (!value) {
            return 'Just now';
        }

        var date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return 'Recently';
        }

        return new Intl.DateTimeFormat(undefined, {
            month: 'short',
            day: 'numeric',
            year: 'numeric'
        }).format(date);
    }

    function createReviewStars(rating) {
        var stars = document.createElement('span');
        stars.className = 'review-stars';
        stars.setAttribute('aria-label', 'Rated ' + rating + ' out of 5');

        for (var index = 1; index <= 5; index += 1) {
            var icon = document.createElement('i');
            icon.className = index <= rating ? 'fas fa-star' : 'far fa-star';
            stars.appendChild(icon);
        }

        return stars;
    }

    function initialsFromUser(userName, userId) {
        var source = (userName || '').trim();

        if (source) {
            return source.split(/\s+/).slice(0, 2).map(function (part) {
                return part.charAt(0).toUpperCase();
            }).join('');
        }

        return (userId || 'CU').slice(0, 2).toUpperCase();
    }

    function initReviewWidgets() {
        document.querySelectorAll('[data-review-widget]').forEach(function (widget) {
            var list = widget.querySelector('[data-review-list]');
            var empty = widget.querySelector('[data-review-empty]');
            var skeleton = widget.querySelector('[data-review-skeleton]');
            var averageNode = widget.querySelector('[data-review-average]');
            var countNode = widget.querySelector('[data-review-count]');
            var toolbarCopy = widget.querySelector('[data-review-toolbar-copy]');
            var liveBadge = widget.querySelector('[data-review-live-badge]');
            var form = widget.querySelector('[data-review-form]');
            var ratingInput = widget.querySelector('[data-review-rating-input]');
            var commentInput = widget.querySelector('[data-review-comment]');
            var charCount = widget.querySelector('[data-review-char-count]');
            var submitButton = widget.querySelector('[data-review-submit]');
            var deleteButton = widget.querySelector('[data-review-delete]');
            var formTitle = widget.querySelector('[data-review-form-title]');
            var formCopy = widget.querySelector('[data-review-form-copy]');
            var starPicker = widget.querySelector('[data-review-star-picker]');
            var currentUserId = widget.getAttribute('data-current-user-id') || '';
            var isAuthenticated = widget.getAttribute('data-is-authenticated') === 'true';
            var reviewsUrl = widget.getAttribute('data-reviews-url');
            var summaryUrl = widget.getAttribute('data-summary-url');
            var addUrl = widget.getAttribute('data-add-url');
            var editUrl = widget.getAttribute('data-edit-url');
            var deleteUrl = widget.getAttribute('data-delete-url');
            var loginUrl = widget.getAttribute('data-login-url') || '/Account/Login';
            var currentUserReview = null;

            if (!list || !reviewsUrl) {
                return;
            }

            function appendAntiForgeryToken(data) {
                var token = widget.querySelector('input[name="__RequestVerificationToken"]')
                    || document.querySelector('input[name="__RequestVerificationToken"]');

                if (token) {
                    data.set(token.name, token.value);
                }
            }

            function reviewId(review) {
                return readValue(review, 'id');
            }

            function reviewUserId(review) {
                return (readValue(review, 'userId') || '').toString();
            }

            function isOwnReview(review) {
                return isAuthenticated && currentUserId && reviewUserId(review) === currentUserId;
            }

            function setLiveBadge(iconClass, text) {
                if (!liveBadge) {
                    return;
                }

                liveBadge.innerHTML = '';

                var icon = document.createElement('i');
                icon.className = iconClass;
                liveBadge.appendChild(icon);
                liveBadge.appendChild(document.createTextNode(text));
            }

            function updateCharacterCount() {
                if (!commentInput || !charCount) {
                    return;
                }

                charCount.textContent = commentInput.value.length + ' / 1000';
            }

            function setRating(value) {
                var rating = Math.max(1, Math.min(5, parseInt(value, 10) || 5));

                if (ratingInput) {
                    ratingInput.value = rating;
                }

                if (!starPicker) {
                    return;
                }

                starPicker.querySelectorAll('[data-rating]').forEach(function (button) {
                    var buttonRating = parseInt(button.getAttribute('data-rating'), 10);
                    var active = buttonRating <= rating;
                    button.classList.toggle('active', active);
                    button.setAttribute('aria-checked', buttonRating === rating ? 'true' : 'false');
                });
            }

            function setFormMode(review) {
                if (!form) {
                    return;
                }

                currentUserReview = review || null;

                if (currentUserReview) {
                    setRating(readValue(currentUserReview, 'rating') || 5);
                    if (commentInput) {
                        commentInput.value = readValue(currentUserReview, 'comment') || '';
                    }
                    if (formTitle) {
                        formTitle.textContent = 'Update your review';
                    }
                    if (formCopy) {
                        formCopy.textContent = 'You already reviewed this product. Keep it fresh if your opinion changed.';
                    }
                    if (submitButton) {
                        submitButton.innerHTML = '<i class="fas fa-save"></i><span>Update Review</span>';
                    }
                    if (deleteButton) {
                        deleteButton.hidden = false;
                    }
                } else {
                    setRating(5);
                    if (commentInput) {
                        commentInput.value = '';
                    }
                    if (formTitle) {
                        formTitle.textContent = 'Write a review';
                    }
                    if (formCopy) {
                        formCopy.textContent = 'Rate the product and leave a helpful note for other shoppers.';
                    }
                    if (submitButton) {
                        submitButton.innerHTML = '<i class="fas fa-paper-plane"></i><span>Submit Review</span>';
                    }
                    if (deleteButton) {
                        deleteButton.hidden = true;
                    }
                }

                updateCharacterCount();
            }

            function renderSummary(summary) {
                var average = Number(readValue(summary, 'averageRating') || 0);
                var count = parseInt(readValue(summary, 'reviewCount'), 10) || 0;

                if (averageNode) {
                    averageNode.textContent = average.toFixed(1);
                }

                if (countNode) {
                    countNode.textContent = count ? count + ' review' + (count === 1 ? '' : 's') : 'No reviews yet';
                }
            }

            function loadSummary() {
                if (!summaryUrl) {
                    renderSummary(null);
                    return Promise.resolve();
                }

                return requestReviewJson(summaryUrl, {
                    method: 'GET',
                    credentials: 'same-origin',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                }).then(renderSummary).catch(function () {
                    renderSummary(null);
                });
            }

            function setSubmitLoading(isLoading) {
                if (!submitButton) {
                    return;
                }

                submitButton.disabled = isLoading;
                submitButton.classList.toggle('is-loading', isLoading);
                if (isLoading) {
                    submitButton.innerHTML = '<span class="store-button-loader" aria-hidden="true"></span><span>Saving...</span>';
                }
            }

            function renderReview(review) {
                var rating = parseInt(readValue(review, 'rating'), 10) || 0;
                var comment = readValue(review, 'comment') || '';
                var userName = readValue(review, 'userName') || '';
                var userId = reviewUserId(review);
                var own = isOwnReview(review);
                var card = document.createElement('article');
                var header = document.createElement('div');
                var author = document.createElement('div');
                var avatar = document.createElement('span');
                var authorText = document.createElement('div');
                var name = document.createElement('strong');
                var date = document.createElement('span');
                var body = document.createElement('p');

                card.className = 'review-card store-fade-in' + (own ? ' own-review' : '');
                header.className = 'review-card-header';
                author.className = 'review-author';
                avatar.className = 'review-avatar';
                avatar.textContent = initialsFromUser(userName, userId);
                name.textContent = own ? 'You' : (userName || 'Customer');
                date.textContent = formatReviewDate(readValue(review, 'createdOn'));
                body.textContent = comment;

                authorText.appendChild(name);
                authorText.appendChild(date);
                author.appendChild(avatar);
                author.appendChild(authorText);
                header.appendChild(author);
                header.appendChild(createReviewStars(rating));
                card.appendChild(header);
                card.appendChild(body);

                if (own) {
                    var actions = document.createElement('div');
                    var edit = document.createElement('button');
                    var remove = document.createElement('button');

                    actions.className = 'review-card-actions';
                    edit.type = 'button';
                    edit.className = 'review-action-button';
                    edit.innerHTML = '<i class="fas fa-edit"></i><span>Edit</span>';
                    edit.addEventListener('click', function () {
                        setFormMode(review);
                        if (form && form.scrollIntoView) {
                            form.scrollIntoView({ behavior: prefersReducedMotion ? 'auto' : 'smooth', block: 'center' });
                        }
                    });

                    remove.type = 'button';
                    remove.className = 'review-action-button danger';
                    remove.innerHTML = '<i class="fas fa-trash"></i><span>Delete</span>';
                    remove.addEventListener('click', function () {
                        deleteReview(review);
                    });

                    actions.appendChild(edit);
                    actions.appendChild(remove);
                    card.appendChild(actions);
                }

                return card;
            }

            function renderReviews(reviews, syncForm) {
                var items = Array.isArray(reviews) ? reviews : [];
                var total = items.length;
                var ownReview = items.find(isOwnReview) || null;

                list.innerHTML = '';
                items.forEach(function (review) {
                    list.appendChild(renderReview(review));
                });

                if (skeleton) {
                    skeleton.hidden = true;
                }
                if (empty) {
                    empty.hidden = total > 0;
                }
                if (toolbarCopy) {
                    toolbarCopy.textContent = total ? 'Showing verified customer feedback for this product.' : 'No customer reviews have been added yet.';
                }

                setLiveBadge(total ? 'fas fa-check-circle' : 'far fa-star', total ? 'Live reviews' : 'Waiting');

                if (syncForm !== false && form) {
                    setFormMode(ownReview);
                }
            }

            function loadReviews(syncForm) {
                if (skeleton) {
                    skeleton.hidden = false;
                }
                if (empty) {
                    empty.hidden = true;
                }
                setLiveBadge('fas fa-circle-notch fa-spin', 'Loading');

                requestReviewJson(reviewsUrl, {
                    method: 'GET',
                    credentials: 'same-origin',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                }).then(function (reviews) {
                    renderReviews(reviews, syncForm);
                    return loadSummary();
                }).catch(function (error) {
                    if (skeleton) {
                        skeleton.hidden = true;
                    }
                    setLiveBadge('fas fa-exclamation-circle', 'Error');
                    showToast('danger', 'Reviews unavailable', error.message || 'Could not load product reviews.');
                });
            }

            function submitReview(event) {
                event.preventDefault();

                if (!isAuthenticated) {
                    window.location.href = loginUrl;
                    return;
                }

                var comment = commentInput ? commentInput.value.trim() : '';
                var rating = parseInt(ratingInput && ratingInput.value, 10) || 0;

                if (rating < 1 || rating > 5) {
                    showToast('danger', 'Choose a rating', 'Please select between 1 and 5 stars.');
                    return;
                }

                if (!comment) {
                    showToast('danger', 'Write a review', 'Please add a short comment before submitting.');
                    return;
                }

                var data = new FormData(form);
                data.set('Rating', rating.toString());
                data.set('Comment', comment);

                var updating = Boolean(currentUserReview && reviewId(currentUserReview));
                if (updating) {
                    data.set('reviewId', reviewId(currentUserReview).toString());
                }

                setSubmitLoading(true);

                requestReviewJson(updating ? editUrl : addUrl, {
                    method: 'POST',
                    body: data,
                    credentials: 'same-origin',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                }).then(function () {
                    showToast('success', updating ? 'Review updated' : 'Review added', 'Thanks for sharing your product feedback.');
                    loadReviews(true);
                }).catch(function (error) {
                    showToast('danger', 'Review not saved', error.message || 'Please try again.');
                    loadReviews(false);
                }).finally(function () {
                    setSubmitLoading(false);
                    if (currentUserReview) {
                        setFormMode(currentUserReview);
                    }
                });
            }

            function deleteReview(review) {
                var targetReview = review || currentUserReview;
                var id = targetReview && reviewId(targetReview);

                if (!id) {
                    return;
                }

                var data = new FormData();

                data.set('reviewId', id.toString());
                appendAntiForgeryToken(data);

                if (deleteButton) {
                    deleteButton.disabled = true;
                }

                requestReviewJson(deleteUrl, {
                    method: 'POST',
                    body: data,
                    credentials: 'same-origin',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                }).then(function () {
                    showToast('success', 'Review deleted', 'Your review was removed from this product.');
                    currentUserReview = null;
                    loadReviews(true);
                }).catch(function (error) {
                    showToast('danger', 'Could not delete review', error.message || 'Please try again.');
                }).finally(function () {
                    if (deleteButton) {
                        deleteButton.disabled = false;
                    }
                });
            }

            if (starPicker) {
                starPicker.querySelectorAll('[data-rating]').forEach(function (button) {
                    button.addEventListener('click', function () {
                        setRating(button.getAttribute('data-rating'));
                    });
                });
            }

            if (commentInput) {
                commentInput.addEventListener('input', updateCharacterCount);
                updateCharacterCount();
            }

            if (form) {
                form.addEventListener('submit', submitReview);
            }

            if (deleteButton) {
                deleteButton.addEventListener('click', function () {
                    deleteReview(currentUserReview);
                });
            }

            loadReviews(true);
        });
    }

    updateThemeIcon(root.getAttribute('data-store-theme') || 'dark');
    updateNavbarState();
    initImageLoaders();
    initCountAnimations();
    enhanceCartForms();
    initCheckoutPreview();
    initStripeCheckout();
    initReviewWidgets();

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
