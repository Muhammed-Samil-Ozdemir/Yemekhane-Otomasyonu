import { api } from './api.js';
import {
    escapeHtml,
    fillSelect,
    formatDate,
    formatMoney,
    formatTime,
    showMessage,
    startPage,
    toIsoDate
} from './ortak.js';

const cafeteriaSelect = document.getElementById('yemekhane');
const mealSelect = document.getElementById('ogun');
const dateInput = document.getElementById('tarih');
const area = document.getElementById('menu-alani');

function pickCurrentMeal(meals) {
    const now = new Date().toTimeString().slice(0, 5);

    return meals.find(meal => meal.endTime > now) ?? meals[meals.length - 1];
}

function renderFood(food) {
    const allergens = food.allergens
        .map(allergen => `<span class="cip">${escapeHtml(allergen)}</span>`)
        .join('');

    return `
        <li class="yemek">
            <div class="yemek-ad">
                <span class="ust-etiket">${escapeHtml(food.categoryName)}</span>
                <strong>${escapeHtml(food.name)}</strong>
            </div>
            <div class="yemek-bilgi">
                ${food.isVegetarian ? '<span class="cip cip-basari">Vejetaryen</span>' : ''}
                ${allergens}
                <span class="kalori">${food.calories} kcal</span>
            </div>
        </li>`;
}

function renderMenu(menu) {
    const reserveLink = menu.isReservable
        ? `<a class="dugme dugme-ana" href="haftalik.html?yemekhane=${menu.cafeteriaId}&hafta=${menu.date}">Rezervasyon yap</a>`
        : '';

    return `
        <article class="kart">
            <div class="bolum-baslik">
                <div class="etiket-deger">
                    <span class="ust-etiket">${escapeHtml(menu.cafeteriaName)} · ${formatDate(menu.date, { day: 'numeric', month: 'long', weekday: 'long' })}</span>
                    <h2>${escapeHtml(menu.mealName)} menüsü</h2>
                </div>
                <div class="cipler">
                    ${menu.isVegetarian ? '<span class="cip cip-basari">Vejetaryen menü</span>' : ''}
                    ${menu.reservationNote ? `<span class="cip">${escapeHtml(menu.reservationNote)}</span>` : ''}
                </div>
            </div>
            <dl class="ozet">
                <div><dt>Toplam kalori</dt><dd>${menu.totalCalories} kcal</dd></div>
                <div><dt>Fiyatınız</dt><dd>${formatMoney(menu.price)}</dd></div>
                <div><dt>Boş yer</dt><dd>${menu.freeSeats ?? '-'} / ${menu.capacity ?? '-'}</dd></div>
                <div><dt>Son rezervasyon</dt><dd>${formatTime(menu.reservationDeadline)}</dd></div>
            </dl>
            <ul class="yemek-listesi">${menu.foods.map(renderFood).join('')}</ul>
            ${reserveLink}
        </article>`;
}

async function loadMenu() {
    area.innerHTML = '<p class="yukleniyor">Menü yükleniyor…</p>';

    try {
        const query = new URLSearchParams({
            cafeteriaId: cafeteriaSelect.value,
            mealId: mealSelect.value,
            date: dateInput.value
        });

        area.innerHTML = renderMenu(await api.get(`menus/daily?${query}`));
    } catch (error) {
        area.innerHTML = `
            <div class="bos-durum">
                <strong>Menü gösterilemiyor</strong>
                <p>${escapeHtml(error.message)}</p>
            </div>`;
    }
}

await startPage();

try {
    const [cafeterias, meals] = await Promise.all([api.get('cafeterias'), api.get('meals')]);

    fillSelect(cafeteriaSelect, cafeterias);
    fillSelect(mealSelect, meals, meal => `${meal.name} (${meal.startTime} - ${meal.endTime})`);

    mealSelect.value = String(pickCurrentMeal(meals).id);
    dateInput.value = toIsoDate(new Date());

    document.getElementById('filtre').addEventListener('change', loadMenu);

    await loadMenu();
} catch (error) {
    showMessage(error.message, 'hata');
}
