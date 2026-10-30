(function () {
    'use strict';

    var page = document.querySelector('[data-payment-status]');
    if (!page || !window.fetch) return;

    var title = page.querySelector('[data-status-title]');
    var message = page.querySelector('[data-status-message]');
    var help = page.querySelector('[data-status-help]');
    var icon = page.querySelector('[data-status-icon]');
    var retry = page.querySelector('[data-status-retry]');
    var refresh = page.querySelector('[data-status-refresh]');
    var statusUrl = page.dataset.statusUrl;
    var orderId = page.dataset.orderId;
    var checks = 0;
    var maxAutomaticChecks = 16;
    var checking = false;
    var labels = {
        pending: {
            title: 'We’re verifying your payment',
            message: 'The payment provider is confirming your transaction. This can take a few moments.',
            help: 'We’ll check for an update automatically. You can also check again manually.',
            icon: 'fa-spinner fa-spin',
            className: 'is-pending'
        },
        paid: {
            title: 'Payment confirmed',
            message: 'Your payment has been verified and your order is confirmed.',
            help: 'You can view delivery updates and order details in My Orders.',
            icon: 'fa-check',
            className: 'is-success'
        },
        failed: {
            title: 'Payment was not confirmed',
            message: 'Your payment was not confirmed. You can review the order status in your account.',
            help: 'If you need help with this order, check its details in My Orders.',
            icon: 'fa-exclamation',
            className: 'is-failed'
        },
        cancelled: {
            title: 'This order was cancelled',
            message: 'This order is no longer available for payment.',
            help: 'If you need help with this order, check its details in My Orders.',
            icon: 'fa-ban',
            className: 'is-failed'
        }
    };

    function trackPurchase() {
        var key = 'myshop-purchase-confirmed-' + orderId;
        try {
            if (localStorage.getItem(key) === '1') return;
            localStorage.setItem(key, '1');
        } catch (_) { }

        if (typeof window.myShopTrack === 'function') {
            window.myShopTrack('purchase', {
                transaction_id: String(orderId),
                currency: 'EGP',
                value: Number(page.dataset.total) || 0,
                item_count: Number(page.dataset.itemCount) || 0
            });
        }
    }

    function render(status) {
        var normalized = String(status || 'pending').toLowerCase();
        var state = labels[normalized] || labels.pending;
        page.dataset.status = normalized;
        title.textContent = state.title;
        message.textContent = state.message;
        icon.className = 'payment-status-icon ' + state.className;
        icon.innerHTML = '<i class="fas ' + state.icon + '" aria-hidden="true"></i>';
        retry.hidden = normalized !== 'pending';
        refresh.hidden = normalized !== 'pending';

        if (normalized === 'paid') trackPurchase();
        if (normalized !== 'pending') help.textContent = state.help;
    }

    function checkStatus(isManual) {
        if (!statusUrl || checking || page.dataset.status !== 'pending') return;
        checking = true;
        checks += 1;
        if (isManual) {
            refresh.disabled = true;
            refresh.setAttribute('aria-busy', 'true');
        }

        fetch(statusUrl, {
            method: 'GET',
            credentials: 'same-origin',
            cache: 'no-store',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(function (response) {
            if (!response.ok) throw new Error('Could not refresh the order status.');
            return response.json();
        }).then(function (payload) {
            render(payload.status);
            if (page.dataset.status === 'pending') {
                help.textContent = checks >= maxAutomaticChecks
                    ? 'Payment is still pending. Check again in a moment or return to payment.'
                    : labels.pending.help;
            }
        }).catch(function () {
            help.textContent = 'Could not reach the order status service. Check your connection and try again.';
        }).finally(function () {
            checking = false;
            refresh.disabled = false;
            refresh.removeAttribute('aria-busy');
            if (page.dataset.status === 'pending' && checks < maxAutomaticChecks) {
                window.setTimeout(function () { checkStatus(false); }, 2500);
            }
        });
    }

    if (refresh) refresh.addEventListener('click', function () { checkStatus(true); });
    if (page.dataset.status === 'paid') trackPurchase();
    else if (page.dataset.status === 'pending') window.setTimeout(function () { checkStatus(false); }, 1200);
})();
