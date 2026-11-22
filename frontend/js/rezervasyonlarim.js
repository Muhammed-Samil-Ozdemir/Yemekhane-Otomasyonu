import { api } from './api.js';
import {
    escapeHtml,
    formatDate,
    formatDateTime,
    formatMoney,
    setBusy,
    showMessage,
    startPage,
    updateBalance
} from './ortak.js';

const filterGroup = document.getElementById('filtre');
const list = document.getElementById('liste');

const filters = [
    { key: 'hepsi', label: 'Tümü' },
    { key: 'aktif', label: 'Aktif' },
    { key: 'kullanildi', label: 'Kullanıldı' },
    { key: 'iptal', label: 'İptal' }
];

const statusClasses = {
    aktif: 'cip-basari',
    kullanildi: '',
    iptal: 'cip-hata'
};

let reservations = [];
let activeFilter = 'hepsi';

function renderFilters() {
    filterGroup.innerHTML = filters.map(filter => {
        const count = filter.key === 'hepsi'
            ? reservations.length
            : reservations.filter(reservation => reservation.status === filter.key).length;

        return `
            <button type="button" class="dugme" data-filtre="${filter.key}" aria-pressed="${filter.key === activeFilter}">
                ${filter.label} (${count})
            </button>`;
    }).join('');
}

function renderAction(reservation) {
    if (reservation.canCancel) {
        return `<button type="button" class="dugme dugme-tehlike" data-iptal="${reservation.id}">İptal et</button>`;
    }

    return reservation.status === 'aktif'
        ? '<span class="ikincil">İptal süresi doldu</span>'
        : '<span class="ikincil">-</span>';
}

function renderList() {
    renderFilters();

    const rows = reservations.filter(reservation => activeFilter === 'hepsi' || reservation.status === activeFilter);

    if (rows.length === 0) {
        list.innerHTML = '<div class="bos-durum"><strong>Bu filtrede rezervasyon yok</strong><p>Haftalık menüden yeni bir rezervasyon yapabilirsiniz.</p></div>';
        return;
    }

    list.innerHTML = `
        <table class="tablo">
            <thead>
                <tr>
                    <th scope="col">Tarih</th>
                    <th scope="col">Öğün</th>
                    <th scope="col">Yemekhane</th>
                    <th scope="col" class="sayi">Ücret</th>
                    <th scope="col">Durum</th>
                    <th scope="col">Alındı</th>
                    <th scope="col"><span class="ikincil">İşlem</span></th>
                </tr>
            </thead>
            <tbody>
                ${rows.map(reservation => `
                    <tr>
                        <td data-etiket="Tarih">${formatDate(reservation.date, { day: 'numeric', month: 'long', weekday: 'short' })}</td>
                        <td data-etiket="Öğün">${escapeHtml(reservation.mealName)}</td>
                        <td data-etiket="Yemekhane">${escapeHtml(reservation.cafeteriaName)}</td>
                        <td data-etiket="Ücret" class="sayi">${formatMoney(reservation.price)}</td>
                        <td data-etiket="Durum"><span class="cip ${statusClasses[reservation.status] ?? ''}">${escapeHtml(reservation.statusText)}</span></td>
                        <td data-etiket="Alındı" class="ikincil">${formatDateTime(reservation.createdAt)}</td>
                        <td data-etiket="İşlem" class="tam-satir"${reservation.canCancel ? '' : ' data-bos'}>${renderAction(reservation)}</td>
                    </tr>`).join('')}
            </tbody>
        </table>`;
}

async function loadReservations() {
    try {
        reservations = await api.get('reservations');
        renderList();
    } catch (error) {
        list.innerHTML = `<div class="bos-durum"><strong>Rezervasyonlar gösterilemiyor</strong><p>${escapeHtml(error.message)}</p></div>`;
    }
}

async function cancel(button) {
    if (button.dataset.onay !== 'evet') {
        button.dataset.onay = 'evet';
        button.textContent = 'Emin misiniz? Onayla';

        setTimeout(() => {
            if (button.isConnected) {
                delete button.dataset.onay;
                button.textContent = 'İptal et';
            }
        }, 4000);

        return;
    }

    setBusy(button, true);

    try {
        const receipt = await api.post(`reservations/${button.dataset.iptal}/cancel`);

        updateBalance(receipt.balance);
        showMessage(`${receipt.message} Güncel bakiyeniz: ${formatMoney(receipt.balance)}`);

        await loadReservations();
    } catch (error) {
        showMessage(error.message, 'hata');
        setBusy(button, false);
    }
}

await startPage();

filterGroup.addEventListener('click', event => {
    const button = event.target.closest('button[data-filtre]');
    if (button) {
        activeFilter = button.dataset.filtre;
        renderList();
    }
});

list.addEventListener('click', event => {
    const button = event.target.closest('button[data-iptal]');
    if (button) {
        cancel(button);
    }
});

await loadReservations();
