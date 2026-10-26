(function () {
    var filters = Array.from(document.querySelectorAll('[data-order-filter]'));
    var cards = Array.from(document.querySelectorAll('[data-order-status]'));
    var count = document.querySelector('[data-orders-count]');
    var empty = document.querySelector('[data-orders-empty]');

    filters.forEach(function (filter) {
        filter.addEventListener('click', function () {
            var selected = filter.getAttribute('data-order-filter');
            var visible = 0;
            cards.forEach(function (card) {
                var show = selected === 'all' || card.getAttribute('data-order-status') === selected;
                card.hidden = !show;
                if (show) visible++;
            });
            filters.forEach(function (item) {
                var active = item === filter;
                item.classList.toggle('active', active);
                item.setAttribute('aria-pressed', active ? 'true' : 'false');
            });
            if (count) count.textContent = visible + (visible === 1 ? ' order' : ' orders');
            if (empty) empty.hidden = visible > 0;
        });
    });

    document.querySelectorAll('[data-order-items-toggle]').forEach(function (button) {
        button.addEventListener('click', function () {
            var container = button.closest('.order-items');
            if (!container) return;
            var expand = button.getAttribute('aria-expanded') !== 'true';
            container.querySelectorAll('[data-order-extra="true"]').forEach(function (item) {
                item.hidden = !expand;
            });
            button.setAttribute('aria-expanded', expand ? 'true' : 'false');
            button.textContent = expand ? 'Show fewer items' : 'Show more items';
        });
    });
})();
