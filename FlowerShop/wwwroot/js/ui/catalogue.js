
const filterMenu = document.getElementById('filter-menu');
const filterClose = document.getElementById('filter-close');
const filterOpen = document.getElementById('filter-open');
const dropdown = document.querySelectorAll('.dropdown');



filterOpen.addEventListener('click', () => {
    filterMenu.classList.remove('-translate-x-full');
    document.body.classList.add('overflow-y-hidden');
})

filterClose.addEventListener('click', () => {
    filterMenu.classList.add('-translate-x-full');
    document.body.classList.remove('overflow-y-hidden');

})

const filterButtons = document.querySelectorAll('.filter-link');

filterButtons.forEach(button => {
    const parentLi = button.closest('li');
    const dropdown = parentLi.querySelector('.dropdown');
    const arrow = button.querySelector('.arrow');

    if (dropdown && arrow) {
        if (dropdown.classList.contains('dropdown-default-open')) {
            dropdown.style.maxHeight = dropdown.scrollHeight + 'px';
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







