

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

// A function responsible for toggling dark overlays over the body
// It can take specific disabled buttons as an argumnent and enable all of them
// After the overlay was closed
export function toggleOverlay(element, translateClass, optionalDisabledButtons = []) {
    const overlay = document.createElement("div")
    document.body.appendChild(overlay);
    document.body.classList.add("overflow-y-hidden");
    document.body.classList.add("pointer-events-none");
    overlay.classList.add("overlay","fixed", "top-0", "w-screen", "h-screen", "bg-gray-900/40", "z-1");
    
    document.addEventListener("click", (e) => {
        if(e.target !== element && !element.contains(e.target))
        {
            element.classList.add(translateClass);
            overlay.remove();
            document.body.classList.remove("overflow-y-hidden");
            if(optionalDisabledButtons.length > 0) {
                optionalDisabledButtons.forEach(o => {
                    o.disabled = false;
                })
            }
        }
    })
}