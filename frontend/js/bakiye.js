import { api } from './api.js';
import {
    escapeHtml,
    formatDateTime,
    formatMoney,
    setBusy,
    showMessage,
    startPage,
    updateBalance
} from './ortak.js';

const balanceValue = document.getElementById('bakiye');
const movementList = document.getElementById('hareketler');
const form = document.getElementById('yukleme-formu');
const submitButton = form.querySelector('button[type="submit"]');

const typeClasses = {
    yukleme: 'cip-basari',
    rezervasyon: '',
    iade: 'cip-uyari'
};

function showBalance(balance) {
    balanceValue.textContent = formatMoney(balance);
    updateBalance(balance);
}

function renderMovements(movements) {
    if (movements.length === 0) {
        movementList.innerHTML = '<div class="bos-durum"><strong>Henüz hareket yok</strong></div>';
        return;
    }

    movementList.innerHTML = `
        <table class="tablo">
            <thead>
                <tr>
                    <th scope="col">Tarih</th>
                    <th scope="col">Tür</th>
                    <th scope="col">Açıklama</th>
                    <th scope="col" class="sayi">Tutar</th>
                </tr>
            </thead>
            <tbody>
                ${movements.map(movement => `
                    <tr>
                        <td data-etiket="Tarih">${formatDateTime(movement.createdAt)}</td>
                        <td data-etiket="Tür"><span class="cip ${typeClasses[movement.type] ?? ''}">${escapeHtml(movement.typeText)}</span></td>
                        <td data-etiket="Açıklama">${escapeHtml(movement.description)}</td>
                        <td data-etiket="Tutar" class="sayi ${movement.amount > 0 ? 'arti' : 'eksi'}">${movement.amount > 0 ? '+' : ''}${formatMoney(movement.amount)}</td>
                    </tr>`).join('')}
            </tbody>
        </table>`;
}

async function loadBalance({ includeBalance = true } = {}) {
    try {
        const data = await api.get('balance');

        if (includeBalance) {
            showBalance(data.balance);
        }

        renderMovements(data.movements);
    } catch (error) {
        movementList.innerHTML = `<div class="bos-durum"><strong>Hareketler gösterilemiyor</strong><p>${escapeHtml(error.message)}</p></div>`;
    }
}

await startPage();

form.addEventListener('click', event => {
    const button = event.target.closest('button[data-tutar]');
    if (button) {
        form.elements.amount.value = button.dataset.tutar;
        form.elements.amount.focus();
    }
});

form.addEventListener('submit', async event => {
    event.preventDefault();
    setBusy(submitButton, true);

    try {
        const amount = Number(String(form.elements.amount.value).replace(',', '.'));
        const receipt = await api.post('balance/load', { amount });

        showBalance(receipt.balance);
        showMessage(`${receipt.message} Güncel bakiyeniz: ${formatMoney(receipt.balance)}`);
        form.reset();

        await loadBalance({ includeBalance: false });
    } catch (error) {
        showMessage(error.message, 'hata');
    } finally {
        setBusy(submitButton, false);
    }
});

await loadBalance();
