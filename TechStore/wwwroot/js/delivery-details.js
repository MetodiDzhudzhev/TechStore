document.addEventListener("DOMContentLoaded", () => {
    const editBtn = document.getElementById("editBtn");
    const saveBtn = document.getElementById("saveBtn");
    const cancelBtn = document.getElementById("cancelBtn");
    const inputs = Array.from(document.querySelectorAll(".editable"));

    if (!editBtn || !saveBtn || !cancelBtn || inputs.length === 0) {
        return;
    }

    let initialValues = inputs.map(i => (i.value ?? "").trim());

    function hasChanges() {
        return inputs.some((input, idx) => (input.value ?? "").trim() !== initialValues[idx]);
    }

    function updateSaveState() {
        saveBtn.disabled = !hasChanges();
    }

    function enableEditMode() {
        inputs.forEach(i => i.removeAttribute("readonly"));

        editBtn.hidden = true;
        saveBtn.hidden = false;
        cancelBtn.hidden = false;

        updateSaveState();

        inputs[0].focus();
        inputs[0].select?.();
    }
    function disableEditMode({ restoreValues } = { restoreValues: false }) {
        if (restoreValues) {
            inputs.forEach((input, idx) => {
                input.value = initialValues[idx];
            });
        }

        inputs.forEach(i => i.setAttribute("readonly", "readonly"));

        editBtn.hidden = false;
        saveBtn.hidden = true;
        cancelBtn.hidden = true;

        saveBtn.disabled = true;
    }

    inputs.forEach(input => {
        input.addEventListener("input", updateSaveState);
        input.addEventListener("change", updateSaveState);
    });

    editBtn.addEventListener("click", enableEditMode);

    cancelBtn.addEventListener("click", () => {
        disableEditMode({ restoreValues: true });
    });
});