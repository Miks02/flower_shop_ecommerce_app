
const filterMenu = document.getElementById('filter-menu');
const filterClose = document.getElementById('filter-close');
const filterOpen = document.getElementById('filter-open');
const dropdown = document.querySelectorAll('.dropdown');

const priceRange = document.getElementById('price-range');
const priceTag = document.getElementById("price-tag")
const priceRangeMobile = document.getElementById('price-range-mobile');
const priceTagMobile = document.getElementById("price-tag-mobile")

priceRange.addEventListener("input", (e) => {
    priceTag.innerHTML = e.target.value;
})
priceRangeMobile.addEventListener("input", (e) => {
    priceTagMobile.innerHTML = e.target.value;
})

filterOpen.addEventListener('click', () => {
    filterMenu.classList.remove('-translate-x-full');
    document.body.classList.add('overflow-y-auto');
})

filterClose.addEventListener('click', () => {
    filterMenu.classList.add('-translate-x-full');
    document.body.classList.remove('overflow-y-auto');
})

const filterButtons = document.querySelectorAll('.filter-link');

filterButtons.forEach(button => {
    const parentLi = button.closest('li');
    const dropdown = parentLi.querySelector('.dropdown');
    const arrow = button.querySelector('.arrow');

    button.addEventListener('click', (event) => {
        event.preventDefault();
        dropdown.classList.toggle('hidden');
        arrow.classList.toggle('rotate-90');
    });
    
});

console.log("catalogue")