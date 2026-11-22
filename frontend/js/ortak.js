import { api } from './api.js';

const moneyFormat = new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' });
let messageTimer;

export function escapeHtml(value) {
    const characters = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };

    return String(value ?? '').replace(/[&<>"']/g, character => characters[character]);
}

export function formatMoney(value) {
    return moneyFormat.format(value);
}

export function parseDate(value) {
    const [year, month, day] = value.slice(0, 10).split('-').map(Number);

    return new Date(year, month - 1, day);
}

export function formatDate(value, options = { day: 'numeric', month: 'long', year: 'numeric' }) {
    return new Intl.DateTimeFormat('tr-TR', options).format(parseDate(value));
}

export function formatDateTime(value) {
    return new Intl.DateTimeFormat('tr-TR', {
        day: 'numeric',
        month: 'short',
        hour: '2-digit',
        minute: '2-digit'
    }).format(new Date(value));
}

export function formatTime(value) {
    return value.slice(11, 16);
}

export function toIsoDate(date) {
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');

    return `${date.getFullYear()}-${month}-${day}`;
}

export function showMessage(text, type = 'basari') {
    const box = document.getElementById('bildirim');

    box.textContent = text;
    box.className = `bildirim bildirim-${type}`;
    box.hidden = false;

    clearTimeout(messageTimer);
    messageTimer = setTimeout(() => {
        box.hidden = true;
    }, 6000);
}

export function setBusy(button, busy) {
    button.disabled = busy;
    button.setAttribute('aria-busy', String(busy));
}

export function updateBalance(balance) {
    const chip = document.getElementById('ust-bakiye');

    if (chip) {
        chip.textContent = formatMoney(balance);
    }
}

export function fillSelect(select, items, getLabel = item => item.name) {
    select.innerHTML = items
        .map(item => `<option value="${item.id}">${escapeHtml(getLabel(item))}</option>`)
        .join('');
}

export function bindThemeButton() {
    const button = document.getElementById('tema-dugmesi');

    button?.addEventListener('click', () => {
        const current = document.documentElement.dataset.tema
            ?? (matchMedia('(prefers-color-scheme: light)').matches ? 'acik' : 'koyu');
        const next = current === 'acik' ? 'koyu' : 'acik';

        document.documentElement.dataset.tema = next;

        try {
            localStorage.setItem('tema', next);
        } catch {
            document.documentElement.dataset.tema = next;
        }
    });
}

function markCurrentPage() {
    const page = location.pathname.split('/').pop() || 'menu.html';

    document.querySelectorAll('.ana-menu a').forEach(link => {
        if (link.getAttribute('href') === page) {
            link.setAttribute('aria-current', 'page');
        }
    });
}

async function logout() {
    try {
        await api.post('auth/logout');
    } finally {
        location.replace('giris.html');
    }
}

export async function startPage({ requireAdmin = false } = {}) {
    bindThemeButton();
    markCurrentPage();

    let user;

    try {
        user = await api.get('auth/me');
    } catch (error) {
        if (error.status !== 401) {
            showMessage(error.message, 'hata');
        }

        throw error;
    }

    document.getElementById('ust-kullanici').textContent = user.fullName;
    updateBalance(user.balance);

    document.querySelectorAll('[data-yalniz-yonetici]').forEach(element => {
        element.hidden = user.role !== 'yonetici';
    });

    document.getElementById('cikis-dugmesi').addEventListener('click', logout);

    if (requireAdmin && user.role !== 'yonetici') {
        location.replace('menu.html');
    }

    return user;
}
