import "./ui/header.js"
import "./ui/catalogue.js"
import {initFilters} from "./helpers.js";
import {initToasts} from "./ui/toast.js";

function initApp() {
    initFilters();
    initToasts();
}

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initApp);
} else {
    initApp();
}

document.addEventListener("htmx:afterSettle", () => {
    initFilters();
    initToasts();
});