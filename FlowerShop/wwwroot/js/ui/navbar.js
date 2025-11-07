import {toggleOverlay} from "../helpers.js";

const heroSection = document.querySelector(".hero");
const navbar = document.getElementById("navbar");
const mobileNavbar = document.getElementById("mobile-navbar");
const loginMenu = document.getElementById("login-menu");

const menuOpen = document.getElementById('navbar-open');
const menuClose = document.getElementById('navbar-close');
const loginOpen = document.getElementById("login-open");
const loginClose = document.getElementById("login-close");
const searchButton = document.getElementById('searchButton');
const searchBar = document.getElementById('searchBar');
const searchInput = document.getElementById('searchInput');

menuOpen.addEventListener('click', () => {
    mobileNavbar.classList.remove('-translate-x-full');
    mobileNavbar.classList.add('translate-x-0');
});

menuClose.addEventListener('click', () => {
    mobileNavbar.classList.remove('translate-x-0');
    mobileNavbar.classList.add('-translate-x-full');
});


loginOpen.addEventListener("click", (e) => {
    e.preventDefault();
    e.stopPropagation();
    toggleOverlay(loginMenu, "translate-x-full");

})

loginClose.addEventListener("click", (e) => {
    e.preventDefault();
    toggleOverlay(loginMenu, "translate-x-full");

})

searchButton.addEventListener('click', () => {
    if(searchBar.classList.contains('invisible')) {
        searchBar.classList.remove('invisible');
        searchBar.classList.remove('h-0');
        searchBar.classList.add('h-[50px]');
        searchBar.classList.add('mb-4');
        searchInput.classList.remove('opacity-0');


    }
    else {
        searchBar.classList.add('invisible');
        searchBar.classList.add('h-0');
        searchBar.classList.remove('h-[50px]');
        searchBar.classList.remove('mb-4');
        searchInput.classList.add('opacity-0');
        navbar.classList.remove('bg-red-900/50');
    }

})

const updateNavbarClasses = (options) => {
    const { blur = false, shadow = false, bg = false } = options;

    navbar.classList.toggle("backdrop-blur-3xl", blur);
    navbar.classList.toggle("shadow-all", shadow);
    navbar.classList.toggle("bg-red-900/35", bg);
};

searchInput.querySelector('input').addEventListener('input', (e) => {
    const hasText = e.target.value.length > 0;
    updateNavbarClasses({ blur: hasText, shadow: hasText, bg: hasText });
});


const handleIntersection = (entries) => {
    entries.forEach(entry => {
        const isIntersecting = entry.isIntersecting;
        const ratioLow = entry.intersectionRatio < 0.95;
        const hasSearchText = searchInput.querySelector('input').value.length > 0;

        if (isIntersecting) {
            navbar.classList.remove("gradient-red");
            mobileNavbar.classList.remove("bg-red-900/85");
            
            updateNavbarClasses({
                blur: ratioLow || hasSearchText,
                shadow: ratioLow || hasSearchText,
                bg: ratioLow || hasSearchText
            });

        } else {
            navbar.classList.add("gradient-red");
            mobileNavbar.classList.add("bg-red-900/85", "bg-opacity-50");
            
            if (!hasSearchText) {
                updateNavbarClasses({ blur: false, shadow: false, bg: false });
            }
        }
    });
};

const observer = new IntersectionObserver(handleIntersection, {
    threshold: [0, 0.95],
    rootMargin: "0px",
    root: null
});
observer.observe(heroSection);

