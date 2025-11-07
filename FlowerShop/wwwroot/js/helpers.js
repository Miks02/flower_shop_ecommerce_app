

export function initFilters () {
    const filters = document.querySelectorAll("input[data-filter]");
    
    if(filters.length === 0)
        return;

    filters.forEach(filter => {
        filter.addEventListener("change", (e) => {
            const filterName = e.currentTarget.dataset.filter;
            const filterId = e.currentTarget.dataset.id;
            let currentValue;

            if(filter.getAttribute("type") === "range")
                currentValue = Number(filter.value);
            if(filter.getAttribute("type") === "checkbox")
                currentValue = filter.checked;

            syncFilters(filterName, filterId, currentValue);
        })
    })

    const syncFilters = (filterName, filterId, currentValue) => {
        const filters = document.querySelectorAll(`input[data-filter="${filterName}"][data-id="${filterId}"]`)

        if(filters.length === 0)
            return;

        if(typeof(currentValue) === "boolean")
        {
            filters.forEach(filter => {
                filter.checked = currentValue;
            })
            return;
        }
        if(typeof(currentValue) === "number")
        {
            const priceTag = document.getElementById("price-tag")
            const priceTagMobile = document.getElementById("price-tag-mobile")
            
            filters.forEach(filter => {
                filter.value = currentValue;
            })
            priceTag.textContent = currentValue
            priceTagMobile.textContent = currentValue;
            return;
        }

        console.log("Greška prilikom sinhronizacije filtera");
    }

}


export function toggleOverlay(element, translateClass) {
    let overlay = document.querySelector('.overlay')
    const isElementVisible = !element.classList.contains(translateClass);
    
    if(!overlay)
    {
        overlay = document.createElement("div")
        document.body.appendChild(overlay);
        document.body.classList.add("overflow-y-hidden");
        overlay.classList.add("overlay","fixed", "top-0", "w-screen", "h-screen", "bg-gray-900/40");
        
        overlay.addEventListener("click", () => {
            toggleOverlay(element, translateClass);
        })
        
    }
    
    if(isElementVisible) {
        element.classList.add(translateClass);
        overlay.remove();
        
        document.body.classList.remove("overflow-y-hidden");
        
        return;
    }
    
    element.classList.remove(translateClass);
    
    document.addEventListener("click", handleOutsideClick)
    
    function handleOutsideClick(e) {
        if(e.target !== element && !element.contains(e.target)) {
            element.classList.add(translateClass);
            overlay.remove();
            document.body.classList.remove("overflow-y-hidden");
            
            document.removeEventListener("click", handleOutsideClick);
        }
    }
    
}