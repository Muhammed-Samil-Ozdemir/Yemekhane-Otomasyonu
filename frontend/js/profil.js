import { api } from './api.js';
import { escapeHtml, formatMoney, setBusy, showMessage, startPage } from './ortak.js';

const details = document.getElementById('bilgiler');
const form = document.getElementById('alerjen-formu');
const allergenList = document.getElementById('alerjenler');
const submitButton = form.querySelector('button[type="submit"]');

function renderDetails(user) {
    const rows = [
        ['Ad soyad', user.fullName],
        ['E-posta', user.email],
        ['Rol', user.roleText],
        ['Bakiye', formatMoney(user.balance)]
    ];

    if (user.studentNumber) {
        rows.splice(3, 0, ['Öğrenci numarası', user.studentNumber]);
    }

    details.innerHTML = rows
        .map(([label, value]) => `<div><dt>${label}</dt><dd>${escapeHtml(value)}</dd></div>`)
        .join('');
}

function renderAllergens(allergens) {
    allergenList.innerHTML = allergens.map(allergen => `
        <label class="onay-kutusu">
            <input type="checkbox" name="allergenIds" value="${allergen.id}"${allergen.isSelected ? ' checked' : ''}>
            <span>${escapeHtml(allergen.name)}</span>
        </label>`).join('');
}

const user = await startPage();
renderDetails(user);

try {
    renderAllergens(await api.get('profile/allergens'));
} catch (error) {
    allergenList.innerHTML = `<div class="bos-durum"><strong>Alerjenler gösterilemiyor</strong><p>${escapeHtml(error.message)}</p></div>`;
}

form.addEventListener('submit', async event => {
    event.preventDefault();
    setBusy(submitButton, true);

    try {
        const allergenIds = [...form.querySelectorAll('input[name="allergenIds"]:checked')]
            .map(input => Number(input.value));

        showMessage(await api.put('profile/allergens', { allergenIds }));
    } catch (error) {
        showMessage(error.message, 'hata');
    } finally {
        setBusy(submitButton, false);
    }
});
