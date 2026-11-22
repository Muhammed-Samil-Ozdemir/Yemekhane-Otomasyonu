import { api } from './api.js';
import {
    escapeHtml,
    fillSelect,
    formatDate,
    formatMoney,
    formatTime,
    setBusy,
    showMessage,
    startPage,
    updateBalance
} from './ortak.js';

const cafeteriaSelect = document.getElementById('yemekhane');
const grid = document.getElementById('hafta');
const rangeLabel = document.getElementById('hafta-araligi');
const previousButton = document.getElementById('onceki-hafta');
const nextButton = document.getElementById('sonraki-hafta');
const params = new URLSearchParams(location.search);

let startDate = params.get('hafta');

function renderAction(menu) {
    if (!menu.isReservable) {
        return `<button type="button" class="dugme" disabled>${escapeHtml(menu.reservationNote ?? 'Rezervasyon kapalı')}</button>`;
    }

    const button = `<button type="button" class="dugme dugme-ana" data-menu-id="${menu.id}">Rezerve et</button>`;

    if (menu.halfPrice === null) {
        return button;
    }

    return `
        <div class="porsiyon-secimi">
            <label class="gorunmez" for="porsiyon-${menu.id}">Porsiyon</label>
            <select id="porsiyon-${menu.id}" data-porsiyon="${menu.id}">
                <option value="tam">Tam · ${formatMoney(menu.price)}</option>
                <option value="yarim">Yarım · ${formatMoney(menu.halfPrice)}</option>
            </select>
            ${button}
        </div>`;
}

function renderFoodNames(menu) {
    return menu.foods
        .map(food => food.hasAllergenWarning
            ? `<strong class="uyari-metni" title="${escapeHtml(food.allergenWarnings.join(', '))} içerir">${escapeHtml(food.name)}</strong>`
            : escapeHtml(food.name))
        .join(', ');
}

function renderSlot(slot) {
    if (!slot.menu) {
        return `
            <section class="ogun" aria-label="${escapeHtml(slot.mealName)}">
                <div class="ogun-ust"><h3>${escapeHtml(slot.mealName)}</h3></div>
                <p class="ogun-yemekler">Bu öğün için menü yayınlanmadı.</p>
            </section>`;
    }

    const menu = slot.menu;

    return `
        <section class="ogun" aria-label="${escapeHtml(slot.mealName)}">
            <div class="ogun-ust">
                <h3>${escapeHtml(slot.mealName)}</h3>
                <div class="etiket-deger">
                    <strong>${formatMoney(menu.price)}</strong>
                    ${menu.halfPrice !== null ? `<span class="yarim-fiyat">Yarım ${formatMoney(menu.halfPrice)}</span>` : ''}
                </div>
            </div>
            <p class="ogun-yemekler">${renderFoodNames(menu)}</p>
            <div class="cipler">
                ${menu.hasAllergenWarning ? `<span class="cip cip-hata">Alerjen: ${escapeHtml(menu.allergenWarnings.join(', '))}</span>` : ''}
                ${menu.isVegetarian ? '<span class="cip cip-basari">Vejetaryen</span>' : ''}
                <span class="cip">${menu.totalCalories} kcal</span>
                <span class="cip">${menu.freeSeats ?? '-'} boş yer</span>
            </div>
            <div class="ogun-alt">
                <span class="ikincil">Son saat ${formatTime(menu.reservationDeadline)}</span>
                ${renderAction(menu)}
            </div>
        </section>`;
}

function renderDay(day) {
    return `
        <article class="kart${day.isToday ? ' gun-bugun' : ''}">
            <div class="etiket-deger">
                <span class="ust-etiket">${formatDate(day.date, { weekday: 'long' })}${day.isToday ? ' · Bugün' : ''}</span>
                <h2>${formatDate(day.date, { day: 'numeric', month: 'long' })}</h2>
            </div>
            ${day.slots.map(renderSlot).join('')}
        </article>`;
}

async function loadWeek() {
    grid.setAttribute('aria-busy', 'true');

    try {
        const query = new URLSearchParams({ cafeteriaId: cafeteriaSelect.value });
        if (startDate) {
            query.set('startDate', startDate);
        }

        const week = await api.get(`menus/weekly?${query}`);

        startDate = week.startDate;
        previousButton.dataset.tarih = week.previousStartDate;
        nextButton.dataset.tarih = week.nextStartDate;
        rangeLabel.textContent = `${formatDate(week.startDate, { day: 'numeric', month: 'long' })} - ${formatDate(week.endDate)}`;

        history.replaceState(null, '', `?yemekhane=${cafeteriaSelect.value}&hafta=${week.startDate}`);

        grid.innerHTML = week.days.map(renderDay).join('');
    } catch (error) {
        grid.innerHTML = `
            <div class="bos-durum">
                <strong>Haftalık menü gösterilemiyor</strong>
                <p>${escapeHtml(error.message)}</p>
            </div>`;
    } finally {
        grid.removeAttribute('aria-busy');
    }
}

async function reserve(button) {
    setBusy(button, true);

    try {
        const portionSelect = grid.querySelector(`select[data-porsiyon="${button.dataset.menuId}"]`);

        const receipt = await api.post('reservations', {
            menuId: Number(button.dataset.menuId),
            portion: portionSelect?.value ?? 'tam'
        });

        updateBalance(receipt.balance);
        showMessage(`${receipt.message} Güncel bakiyeniz: ${formatMoney(receipt.balance)}`);

        await loadWeek();
    } catch (error) {
        showMessage(error.message, 'hata');
        setBusy(button, false);
    }
}

await startPage();

try {
    fillSelect(cafeteriaSelect, await api.get('cafeterias'));

    if (params.has('yemekhane')) {
        cafeteriaSelect.value = params.get('yemekhane');
    }

    cafeteriaSelect.addEventListener('change', loadWeek);

    [previousButton, nextButton].forEach(button => button.addEventListener('click', () => {
        startDate = button.dataset.tarih;
        loadWeek();
    }));

    grid.addEventListener('click', event => {
        const button = event.target.closest('button[data-menu-id]');
        if (button) {
            reserve(button);
        }
    });

    await loadWeek();
} catch (error) {
    showMessage(error.message, 'hata');
}
