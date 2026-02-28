document.addEventListener("DOMContentLoaded", function () {

    const autoSubmitSelect = document.querySelector("[data-auto-submit]");

    if (autoSubmitSelect) {
        autoSubmitSelect.addEventListener("change", function () {
            this.form.submit();
        });
    }

});