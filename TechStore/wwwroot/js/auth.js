(function () {

    function initPasswordToggles() {
        const toggles = document.querySelectorAll('[data-password-toggle]');

        toggles.forEach(button => {
            button.addEventListener('click', function () {

                const wrapper = this.closest('.field__password');
                if (!wrapper) {
                    return;
                }

                const input = wrapper.querySelector('input');
                const icon = this.querySelector('i');

                if (!input) {
                    return;
                }

                const isHidden = input.type === 'password';

                input.type = isHidden ? 'text' : 'password';

                if (icon) {
                    icon.classList.toggle('bi-eye', !isHidden);
                    icon.classList.toggle('bi-eye-slash', isHidden);
                }

                this.setAttribute('aria-label', isHidden ? 'Hide password' : 'Show password');
            });
        });
    }

    document.addEventListener('DOMContentLoaded', initPasswordToggles);

})();
