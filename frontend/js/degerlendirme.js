import { api } from './api.js';
import { escapeHtml, formatDate, setBusy, showMessage, startPage } from './ortak.js';

const list = document.getElementById('liste');
const pendingChip = document.getElementById('bekleyen');
const scores = [5, 4, 3, 2, 1];

function renderStars(score) {
    return `<span class="puan" aria-label="${score} puan">${'★'.repeat(score)}${'☆'.repeat(5 - score)}</span>`;
}

function renderForm(item) {
    const stars = scores.map(score => `
        <input type="radio" id="puan-${item.menuId}-${score}" name="puan" value="${score}">
        <label for="puan-${item.menuId}-${score}" title="${score} puan" aria-label="${score} puan">★</label>`).join('');

    return `
        <form class="form" data-menu-id="${item.menuId}">
            <fieldset class="yildizlar" aria-label="Puanınız">${stars}</fieldset>
            <label class="alan">
                <span>Yorum (isteğe bağlı)</span>
                <textarea name="yorum" rows="3"></textarea>
            </label>
            <button type="submit" class="dugme dugme-ana">Gönder</button>
        </form>`;
}

function renderItem(item) {
    const result = item.canEvaluate
        ? renderForm(item)
        : `<div class="etiket-deger">
               ${renderStars(item.score)}
               ${item.comment ? `<p>${escapeHtml(item.comment)}</p>` : '<p class="ikincil">Yorum yazılmadı.</p>'}
           </div>`;

    return `
        <article class="kart">
            <div class="bolum-baslik">
                <div class="etiket-deger">
                    <span class="ust-etiket">${escapeHtml(item.cafeteriaName)} · ${formatDate(item.date, { day: 'numeric', month: 'long', weekday: 'short' })}</span>
                    <h2>${escapeHtml(item.mealName)}</h2>
                </div>
                ${item.canEvaluate ? '<span class="cip cip-uyari">Bekliyor</span>' : '<span class="cip cip-basari">Değerlendirildi</span>'}
            </div>
            <p class="ikincil">${escapeHtml(item.foods ?? '')}</p>
            ${result}
        </article>`;
}

async function loadEvaluations() {
    try {
        const items = await api.get('evaluations');
        const pendingCount = items.filter(item => item.canEvaluate).length;

        pendingChip.hidden = pendingCount === 0;
        pendingChip.textContent = `${pendingCount} menü değerlendirmenizi bekliyor`;

        list.innerHTML = items.length === 0
            ? '<div class="bos-durum"><strong>Henüz kullandığınız bir menü yok</strong><p>Turnikeden geçtiğiniz menüler burada değerlendirmeye açılır.</p></div>'
            : items.map(renderItem).join('');
    } catch (error) {
        list.innerHTML = `<div class="bos-durum"><strong>Menüler gösterilemiyor</strong><p>${escapeHtml(error.message)}</p></div>`;
    }
}

await startPage();

list.addEventListener('submit', async event => {
    event.preventDefault();

    const form = event.target;
    const button = form.querySelector('button[type="submit"]');
    const data = new FormData(form);

    setBusy(button, true);

    try {
        const message = await api.post('evaluations', {
            menuId: Number(form.dataset.menuId),
            score: Number(data.get('puan') ?? 0),
            comment: data.get('yorum')
        });

        showMessage(message);
        await loadEvaluations();
    } catch (error) {
        showMessage(error.message, 'hata');
        setBusy(button, false);
    }
});

await loadEvaluations();
