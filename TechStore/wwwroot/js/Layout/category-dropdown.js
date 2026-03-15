const categoryToggle = document.getElementById("categoryToggle");
const categoryDropdown = document.getElementById("categoryDropdown");

if (categoryToggle) {

    categoryToggle.addEventListener("click", () => {

        categoryDropdown.classList.toggle("category-dropdown--open");

    });
}

document.addEventListener("click", (e) => {

    if (!categoryDropdown.contains(e.target)) {

        categoryDropdown.classList.remove("category-dropdown--open");

    }
});