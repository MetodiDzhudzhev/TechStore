document.addEventListener("DOMContentLoaded", function () {

    const toggles = document.querySelectorAll(".js-toggle-details");

    toggles.forEach(btn => {
        btn.addEventListener("click", function () {

            const targetId = this.dataset.target;
            const details = document.getElementById(targetId);

            details.classList.toggle("is-open");
        });
    });

});