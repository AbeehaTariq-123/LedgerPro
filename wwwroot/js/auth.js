/**
 * LedgerPro - Auth Helper Scripts
 * Handles password visibility toggle and basic client-side form UX.
 */

/**
 * Toggle password field between text and password.
 * @param {string} inputId - The id of the input field
 * @param {HTMLButtonElement} btn - The toggle button element
 */
function togglePassword(inputId, btn) {
    const input = document.getElementById(inputId);
    if (!input) return;

    if (input.type === "password") {
        input.type = "text";
        btn.textContent = "🙈";
        btn.setAttribute("aria-label", "Hide password");
    } else {
        input.type = "password";
        btn.textContent = "👁";
        btn.setAttribute("aria-label", "Show password");
    }
}

/**
 * Simple live validation: remove error state when user starts typing.
 */
document.addEventListener("DOMContentLoaded", function () {
    const inputs = document.querySelectorAll(".form-control");
    inputs.forEach(function (input) {
        input.addEventListener("input", function () {
            // Clear the associated validation span text (ASP.NET renders it)
            const errorSpan = input.parentElement.querySelector(".field-error");
            if (errorSpan && errorSpan.textContent.trim() !== "") {
                errorSpan.textContent = "";
            }
            // Remove any inline red border we set
            input.style.borderColor = "";
        });
    });
});