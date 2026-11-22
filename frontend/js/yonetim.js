import { api } from './api.js';
import {
    escapeHtml,
    fillSelect,
    formatDate,
    setBusy,
    showMessage,
    startPage,
    toIsoDate
} from './ortak.js';

const reportFilter = document.getElementById('rapor-filtresi');
const reportArea = document.getElementById('rapor');
const menuForm = document.getElementById('menu-formu');
const menuSection = document.getElementById('menu-bolumu');
const foodPicker = document.getElementById('yemek-secimi');
const selectionSummary = document.getElementById('secim-ozeti');
const newMenuButton = document.getElementById('yeni-menu');
const turnstileForm = document.getElementById('turnike-formu');
const percentFormat = new Intl.NumberFormat('tr-TR', { style: 'percent', maximumFractionDigits: 1 });

let editingMenuId = null;

function shiftDays(days) {
    const date = new Date();
    date.setDate(date.getDate() + days);

    return toIsoDate(date);
}

function renderScore(row) {
    if (row.averageScore === null) {
        return '<span class="ikincil">Değerlendirme yok</span>';
    }

    return `<span class="puan">★</span> ${row.averageScore.toFixed(1)} <span class="ikincil">(${row.evaluationCount} oy)</span>`;
}

function renderReport(rows) {
    if (rows.length === 0) {
        reportArea.innerHTML = '<div class="bos-durum"><strong>Bu aralıkta menü yok</strong></div>';
        return;
    }

    reportArea.innerHTML = `
        <table class="tablo">
            <thead>
                <tr>
                    <th scope="col">Tarih</th>
                    <th scope="col">Öğün</th>
                    <th scope="col">Yemekhane</th>
                    <th scope="col">Doluluk</th>
                    <th scope="col" class="sayi">Boş yer</th>
                    <th scope="col">Ortalama puan</th>
                    <th scope="col"><span class="ikincil">İşlem</span></th>
                </tr>
            </thead>
            <tbody>
                ${rows.map(row => `
                    <tr>
                        <td data-etiket="Tarih">${formatDate(row.date, { day: 'numeric', month: 'short', weekday: 'short' })}</td>
                        <td data-etiket="Öğün">${escapeHtml(row.mealName)}</td>
                        <td data-etiket="Yemekhane">${escapeHtml(row.cafeteriaName)}</td>
                        <td data-etiket="Doluluk" class="tam-satir">
                            <div class="doluluk">
                                <div class="doluluk-cubuk"><span style="width: ${Math.min(100, row.occupancyRate)}%"></span></div>
                                <span>${row.reservationCount} / ${row.capacity} · ${percentFormat.format(row.occupancyRate / 100)}</span>
                            </div>
                        </td>
                        <td data-etiket="Boş yer" class="sayi">${row.freeSeats}</td>
                        <td data-etiket="Ortalama puan">${renderScore(row)}</td>
                        <td data-etiket="İşlem"><button type="button" class="dugme" data-duzenle="${row.menuId}">Düzenle</button></td>
                    </tr>`).join('')}
            </tbody>
        </table>`;
}

async function loadReport() {
    const filter = new FormData(reportFilter);
    const query = new URLSearchParams();

    for (const [key, value] of filter) {
        if (value) {
            query.set(key, value);
        }
    }

    try {
        renderReport(await api.get(`admin/menus?${query}`));
    } catch (error) {
        reportArea.innerHTML = `<div class="bos-durum"><strong>Rapor gösterilemiyor</strong><p>${escapeHtml(error.message)}</p></div>`;
    }
}

function renderFoodPicker(foods) {
    const categories = Map.groupBy(foods, food => food.categoryName);

    foodPicker.innerHTML = [...categories].map(([category, items]) => `
        <fieldset>
            <legend>${escapeHtml(category)}</legend>
            ${items.map(food => `
                <label class="onay-kutusu">
                    <input type="checkbox" name="foodIds" value="${food.id}">
                    <span>${escapeHtml(food.name)}</span>
                    <small>${food.calories} kcal</small>
                </label>`).join('')}
        </fieldset>`).join('');
}

function updateSelectionSummary() {
    const count = menuForm.querySelectorAll('input[name="foodIds"]:checked').length;

    selectionSummary.textContent = count === 0 ? 'Seçili yemek yok' : `${count} yemek seçildi`;
}

function resetMenuForm() {
    editingMenuId = null;
    menuForm.reset();
    menuForm.elements.date.value = shiftDays(1);

    document.getElementById('form-etiket').textContent = 'Yeni menü';
    document.getElementById('form-baslik').textContent = 'Menü oluştur';
    newMenuButton.hidden = true;

    updateSelectionSummary();
}

async function editMenu(menuId) {
    try {
        const menu = await api.get(`admin/menus/${menuId}`);

        editingMenuId = menu.id;
        menuForm.elements.cafeteriaId.value = menu.cafeteriaId;
        menuForm.elements.date.value = menu.date;
        menuForm.elements.mealId.value = menu.mealId;
        menuForm.elements.studentPrice.value = menu.studentPrice;
        menuForm.elements.staffPrice.value = menu.staffPrice;

        menuForm.querySelectorAll('input[name="foodIds"]').forEach(input => {
            input.checked = menu.foodIds.includes(Number(input.value));
        });

        document.getElementById('form-etiket').textContent = `Menü #${menu.id}`;
        document.getElementById('form-baslik').textContent = 'Menüyü düzenle';
        newMenuButton.hidden = false;

        updateSelectionSummary();
        menuSection.scrollIntoView({ behavior: 'smooth' });
    } catch (error) {
        showMessage(error.message, 'hata');
    }
}

async function saveMenu(event) {
    event.preventDefault();

    const button = menuForm.querySelector('button[type="submit"]');
    const body = {
        cafeteriaId: Number(menuForm.elements.cafeteriaId.value),
        date: menuForm.elements.date.value,
        mealId: Number(menuForm.elements.mealId.value),
        studentPrice: Number(menuForm.elements.studentPrice.value),
        staffPrice: Number(menuForm.elements.staffPrice.value),
        foodIds: [...menuForm.querySelectorAll('input[name="foodIds"]:checked')].map(input => Number(input.value))
    };

    setBusy(button, true);

    try {
        if (editingMenuId) {
            showMessage(await api.put(`admin/menus/${editingMenuId}`, body));
        } else {
            const menuId = await api.post('admin/menus', body);
            showMessage(`Menü #${menuId} oluşturuldu.`);
        }

        resetMenuForm();
        await loadReport();
    } catch (error) {
        showMessage(error.message, 'hata');
    } finally {
        setBusy(button, false);
    }
}

async function passTurnstile(event) {
    event.preventDefault();

    const button = turnstileForm.querySelector('button[type="submit"]');

    setBusy(button, true);

    try {
        const message = await api.post('admin/turnstile', {
            email: turnstileForm.elements.email.value,
            cafeteriaId: Number(turnstileForm.elements.cafeteriaId.value),
            time: turnstileForm.elements.time.value || null
        });

        showMessage(message);
        turnstileForm.elements.email.value = '';
        await loadReport();
    } catch (error) {
        showMessage(error.message, 'hata');
    } finally {
        setBusy(button, false);
    }
}

await startPage({ requireAdmin: true });

try {
    const [cafeterias, meals, foods] = await Promise.all([
        api.get('cafeterias'),
        api.get('meals'),
        api.get('admin/foods')
    ]);

    fillSelect(reportFilter.elements.cafeteriaId, [{ id: '', name: 'Tüm yemekhaneler' }, ...cafeterias]);
    fillSelect(menuForm.elements.cafeteriaId, cafeterias);
    fillSelect(menuForm.elements.mealId, meals);
    fillSelect(turnstileForm.elements.cafeteriaId, cafeterias);
    renderFoodPicker(foods);

    reportFilter.elements.startDate.value = shiftDays(-7);
    reportFilter.elements.endDate.value = shiftDays(7);
    resetMenuForm();

    reportFilter.addEventListener('submit', event => {
        event.preventDefault();
        loadReport();
    });

    reportArea.addEventListener('click', event => {
        const button = event.target.closest('button[data-duzenle]');
        if (button) {
            editMenu(Number(button.dataset.duzenle));
        }
    });

    menuForm.addEventListener('change', updateSelectionSummary);
    menuForm.addEventListener('submit', saveMenu);
    newMenuButton.addEventListener('click', resetMenuForm);
    turnstileForm.addEventListener('submit', passTurnstile);

    await loadReport();
} catch (error) {
    showMessage(error.message, 'hata');
}
