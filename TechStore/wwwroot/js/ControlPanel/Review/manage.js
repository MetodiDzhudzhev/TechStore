document.addEventListener("DOMContentLoaded", () => {

    const buttons = document.querySelectorAll(".js-review-toggle");

    buttons.forEach(btn => {

        const targetId = btn.dataset.target;
        const target = document.getElementById(targetId);
        const text = btn.querySelector(".btn-text");

        btn.addEventListener("click", () => {

            target.classList.toggle("is-open");

            if (target.classList.contains("is-open")) {
                text.textContent = "Hide details";
            }
            else {
                text.textContent = "Details";
            }
        });
    });
});