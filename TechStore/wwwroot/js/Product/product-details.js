function renderStars(avg) {
    const rounded = Math.round(avg);
    let html = '';

    for (let i = 1; i <= 5; i++) {
        if (i <= rounded) {
            html += '<span class="review__star review__star--filled">★</span>';
        } else {
            html += '<span class="review__star">☆</span>';
        }
    }

    return html + ` <span class="reviews__avg">${avg.toFixed(1)}/5</span>`;
}

(function () {

    const collapseEl = document.getElementById('reviewsCollapse');
    const container = document.getElementById('reviewsContainer');
    const toggleBtn = document.getElementById('reviewsToggleBtn');
    const summaryEl = document.getElementById('reviewsSummary');
    const chevron = document.querySelector('.reviews-panel__arrow');

    const pageSize = 5;
    const productId = toggleBtn.getAttribute('data-product-id');
    let loadedOnce = false;

    toggleBtn.addEventListener('click', async () => {

        collapseEl.classList.toggle('reviews-panel__collapse--open');

        if (chevron) {
            chevron.classList.toggle('reviews-panel__arrow--rotated');
        }

        if (!loadedOnce) {
            loadedOnce = true;
            await loadPage(1);
        }
    });

    async function loadSummary() {
        try {
            const res = await fetch(`/Review/Stats?productId=${productId}`, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (!res.ok) return;

            const data = await res.json();

            const count = data.totalCount ?? data.TotalCount;
            const avg = data.averageRating ?? data.AverageRating;

            if (summaryEl &&
                typeof count === 'number' &&
                typeof avg === 'number') {

                summaryEl.innerHTML =
                    `(${count}) • ${renderStars(avg)}`;
            }
        }
        catch {
        }
    }

    async function loadPage(page) {

        container.innerHTML =
            `<div class="reviews-panel__loader"></div>`;

        try {
            const res = await fetch(
                `/Review/Panel?productId=${productId}&page=${page}&pageSize=${pageSize}`,
                { headers: { 'X-Requested-With': 'XMLHttpRequest' } }
            );

            if (!res.ok) {
                container.innerHTML =
                    `<div class="reviews__message reviews__message--error">
                        Failed to load reviews.
                     </div>`;
                return;
            }

            container.innerHTML = await res.text();
        }
        catch {
            container.innerHTML =
                `<div class="reviews__message reviews__message--error">
                    Failed to load reviews.
                 </div>`;
        }
    }

    container.addEventListener('click', function (e) {

        const btn = e.target.closest('[data-page]');
        if (!btn) return;

        if (btn.classList.contains('reviews__page-btn--disabled') ||
            btn.classList.contains('reviews__page-btn--active')) {
            return;
        }

        const page = parseInt(btn.dataset.page);

        if (!Number.isNaN(page)) {
            loadPage(page);
        }
    });

    loadSummary();

})();