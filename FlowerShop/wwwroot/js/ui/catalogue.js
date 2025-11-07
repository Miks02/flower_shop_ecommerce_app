import {toggleOverlay} from "../helpers.js";

const filterMenu = document.getElementById('filter-menu');
const filterClose = document.getElementById('filter-close');
const filterOpen = document.getElementById('filter-open');

filterOpen.addEventListener('click', (e) => {
    e.preventDefault();
    e.stopPropagation()
    toggleOverlay(filterMenu, "-translate-x-full")
})

filterClose.addEventListener('click', (e) => {
    e.preventDefault();
    toggleOverlay(filterMenu, "-translate-x-full");

})

const filterButtons = document.querySelectorAll('.filter-link');

filterButtons.forEach(button => {
    const parentLi = button.closest('li');
    const dropdown = parentLi.querySelector('.dropdown');
    const arrow = button.querySelector('.arrow');

    if (dropdown && arrow) {
        if (dropdown.classList.contains('dropdown-default-open')) {
            dropdown.style.maxHeight = dropdown.scrollHeight + 'px';
            arrow.classList.add("rotate-90");
        } else {
            dropdown.style.maxHeight = '0px';
        }
        
        button.addEventListener('click', (event) => {
            event.preventDefault();
            
            const isOpen = dropdown.style.maxHeight && dropdown.style.maxHeight !== '0px';
            
            if (isOpen) {
                dropdown.style.maxHeight = '0px';
                arrow.classList.remove('rotate-90');
            } else {
                dropdown.style.maxHeight = dropdown.scrollHeight + 'px';
                arrow.classList.add('rotate-90');
            }
        });
    }
});







