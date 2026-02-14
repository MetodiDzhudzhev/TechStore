document.addEventListener("DOMContentLoaded", () => {
    const buttons = document.querySelectorAll(".orderCard__toggle");

    buttons.forEach(btn => {
        const targetSelector = btn.getAttribute("data-target");
        const details = targetSelector ? document.querySelector(targetSelector) : null;
        const textSpan = btn.querySelector(".orderCard__toggleText");

        if (!details || !textSpan) {
            return;
        }

        details.hidden = true;

        btn.addEventListener("click", () => {
            const isOpen = !details.hidden;

            details.hidden = isOpen;
            btn.setAttribute("aria-expanded", !isOpen);
            textSpan.textContent = isOpen ? "Show details" : "Hide details";
        });
    });
});
