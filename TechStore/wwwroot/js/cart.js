(function () {
    document.addEventListener("DOMContentLoaded", () => {
        const clearForm = document.querySelector("[data-cart-clear]");
        if (!clearForm) {
            return;
        }

        clearForm.addEventListener("submit", (e) => {
            const ok = confirm("Are you sure you want to clear your cart?");
            if (!ok) {
                e.preventDefault();
            }
        });
    });
})();
