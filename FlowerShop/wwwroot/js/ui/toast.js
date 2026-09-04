let lastToastSignature = '';
let lastToastTime = 0;
let isInitialized = false;

function normalizeToastPayload(payload) {
    if (!payload) return null;
    if (typeof payload === 'string') {
        return { message: payload, level: 'info', duration: 4000, title: null };
    }

    let target = payload;
    if (target.value && typeof target.value === 'object') {
        target = target.value;
    } else if (target.value && typeof target.value === 'string') {
        return {
            message: target.value,
            level: target.level || 'info',
            duration: target.duration !== undefined ? target.duration : 4000,
            title: target.title || null
        };
    }

    const message = target.message || target.Message;
    if (!message) return null;

    const level = target.level || target.Level || 'info';
    const title = target.title || target.Title || null;
    const duration = target.duration !== undefined ? target.duration : (target.Duration !== undefined ? target.Duration : 4000);

    return { message, level, title, duration };
}

export function showToast(rawPayload) {
    const data = normalizeToastPayload(rawPayload);
    if (!data || !data.message) return;

    const now = Date.now();
    const signature = `${data.level}_${data.title || ''}_${data.message}`;
    if (signature === lastToastSignature && (now - lastToastTime) < 400) {
        return;
    }
    lastToastSignature = signature;
    lastToastTime = now;

    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'fixed bottom-5 right-5 z-50 flex flex-col gap-3 pointer-events-none max-w-sm w-full px-4 sm:px-0';
        document.body.appendChild(container);
    }

    const levelNormalized = (data.level || 'info').toLowerCase();

    let borderClass = 'bg-sky-600/80 text-sky-100 shadow-sky-100';
    let iconClass = 'fa-solid fa-circle-info text-sky-100';
    let defaultTitle = 'Obaveštenje';

    switch (levelNormalized) {
        case 'success':
            borderClass = 'bg-green-600/80 text-green-100 shadow-green-100';
            iconClass = 'fa-solid fa-circle-check text-green-100';
            defaultTitle = 'Uspešno';
            break;
        case 'error':
            borderClass = 'bg-red-600/80 text-red-100 shadow-red-100';
            iconClass = 'fa-solid fa-circle-exclamation text-red-100';
            defaultTitle = 'Greška';
            break;
        case 'warning':
            borderClass = 'bg-amber-600/80 text-amber-100 shadow-amber-100';
            iconClass = 'fa-solid fa-triangle-exclamation text-amber-100';
            defaultTitle = 'Upozorenje';
            break;
    }

    const finalTitle = data.title || defaultTitle;

    const toast = document.createElement('div');
    toast.className = `toast-item pointer-events-auto transform translate-y-4 opacity-0 transition-all duration-300 ease-out flex items-start gap-3 p-4 rounded-xl shadow-xl backdrop-blur-md ${borderClass}`;
    toast.innerHTML = `
        <div class="text-xl flex-shrink-0 mt-0.5">
            <i class="${iconClass}"></i>
        </div>
        <div class="flex-1 text-sm">
            <h5 class="font-bold text-slate-100">${escapeHtml(finalTitle)}</h5>
            <p class="text-gray-100 mt-0.5 text-xs sm:text-sm">${escapeHtml(data.message)}</p>
        </div>
        <button type="button" class="toast-close text-slate-100 hover:text-slate-600 transition-colors p-1">
            <i class="fa-solid fa-xmark text-sm"></i>
        </button>
    `;

    const closeBtn = toast.querySelector('.toast-close');
    const dismiss = () => {
        toast.classList.remove('translate-y-0', 'opacity-100');
        toast.classList.add('translate-y-4', 'opacity-0');
        setTimeout(() => {
            if (toast.parentElement) {
                toast.parentElement.removeChild(toast);
            }
        }, 300);
    };

    closeBtn.addEventListener('click', dismiss);

    container.appendChild(toast);

    requestAnimationFrame(() => {
        toast.classList.remove('translate-y-4', 'opacity-0');
        toast.classList.add('translate-y-0', 'opacity-100');
    });

    if (data.duration > 0) {
        setTimeout(dismiss, data.duration);
    }
}

function escapeHtml(unsafe) {
    if (!unsafe) return '';
    return String(unsafe)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

function initExistingToasts() {
    document.querySelectorAll('#toast-container .toast-item').forEach(toast => {
        if (toast.dataset.initialized === 'true') return;
        toast.dataset.initialized = 'true';

        const duration = parseInt(toast.getAttribute('data-duration') || '4000', 10);
        const dismiss = () => {
            toast.classList.remove('translate-y-0', 'opacity-100');
            toast.classList.add('translate-y-4', 'opacity-0');
            setTimeout(() => {
                if (toast.parentElement) {
                    toast.parentElement.removeChild(toast);
                }
            }, 300);
        };

        const closeBtn = toast.querySelector('.toast-close');
        if (closeBtn) {
            closeBtn.onclick = null;
            closeBtn.addEventListener('click', dismiss);
        }

        if (duration > 0) {
            setTimeout(dismiss, duration);
        }
    });
}

export function initToasts() {
    initExistingToasts();

    if (isInitialized) return;
    isInitialized = true;

    document.addEventListener('showToast', (event) => {
        if (event.detail) {
            showToast(event.detail);
        }
    });

    window.addEventListener('showToast', (event) => {
        if (event.detail) {
            showToast(event.detail);
        }
    });

    const handleHtmxResponse = (event) => {
        const xhr = event.detail?.xhr;
        if (!xhr) return;

        const triggerHeader = xhr.getResponseHeader('HX-Trigger') ||
                              xhr.getResponseHeader('HX-Trigger-After-Swap') ||
                              xhr.getResponseHeader('HX-Trigger-After-Settle');
        if (triggerHeader) {
            try {
                const parsed = JSON.parse(triggerHeader);
                if (parsed.showToast) {
                    showToast(parsed.showToast);
                }
            } catch {
            }
        }
    };

    document.addEventListener('htmx:afterOnLoad', handleHtmxResponse);
    document.addEventListener('htmx:afterRequest', handleHtmxResponse);
    document.addEventListener('htmx:afterSettle', handleHtmxResponse);
}
