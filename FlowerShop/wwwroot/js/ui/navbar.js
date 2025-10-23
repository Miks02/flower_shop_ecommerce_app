console.log("navbar")

const heroSection = document.querySelector(".hero");
const navbar = document.getElementById("navbar");
const mobileNavbar = document.getElementById("mobile-navbar");

const menuOpen = document.getElementById('navbar-open');
const menuClose = document.getElementById('navbar-close');
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

const handleIntersection = (entries) => {
    entries.forEach(entry => {
        if(entry.isIntersecting) {
            navbar.classList.remove("gradient-red");
            mobileNavbar.classList.remove("bg-red-900/85");
            if (entry.intersectionRatio < 0.95) {
                navbar.classList.add("backdrop-blur-3xl");
                navbar.classList.add("shadow-all");
                navbar.classList.add("bg-red-900/30");
            } else {
                navbar.classList.remove("backdrop-blur-3xl");
                navbar.classList.remove("shadow-all");
                navbar.classList.remove("bg-red-900/30");
            }
        } else {
            navbar.classList.add("gradient-red");
            mobileNavbar.classList.add("bg-red-900/85");
            mobileNavbar.classList.add("bg-opacity-50")
            
        }
    });
};

const observer = new IntersectionObserver(handleIntersection, {
    threshold: [0, 0.95],
    rootMargin: "0px",
    root: null
});
console.log(observer)
observer.observe(heroSection);

