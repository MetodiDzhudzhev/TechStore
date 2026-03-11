document.addEventListener("DOMContentLoaded", () => {

    const toggles = document.querySelectorAll(".js-product-toggle");

    toggles.forEach(btn => {

        btn.addEventListener("click", e => {

            if (e.target.closest(".product-card__actions")) {
                return;
            }

            const targetId = btn.dataset.target;

            const target = document.getElementById(targetId);

            target.classList.toggle("is-open");
        });
    });
});