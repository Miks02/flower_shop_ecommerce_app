import {toggleOverlay} from "../helpers.js";

const header = document.querySelector('header');
const authMenu = document.getElementById("auth-menu");

header.addEventListener("click", handleAuthMenu)

function handleAuthMenu (e) {
    if(e.target.closest(`button[data-menu="auth-open"]`) || e.target.closest(`button[data-menu="auth-close"]`))
    {
        e.preventDefault();
        e.stopPropagation();
        toggleOverlay(authMenu, "translate-x-full");
        
    }
    
}


