import { api } from './api.js';
import { bindThemeButton, setBusy } from './ortak.js';

const form = document.getElementById('giris-formu');
const errorBox = document.getElementById('giris-hata');
const submitButton = form.querySelector('button[type="submit"]');

bindThemeButton();

api.get('auth/me')
    .then(() => location.replace('menu.html'))
    .catch(() => form.elements.email.focus());

form.addEventListener('submit', async event => {
    event.preventDefault();

    errorBox.hidden = true;
    setBusy(submitButton, true);

    try {
        await api.post('auth/login', {
            email: form.elements.email.value,
            password: form.elements.password.value
        });

        location.replace('menu.html');
    } catch (error) {
        errorBox.textContent = error.message;
        errorBox.hidden = false;
        setBusy(submitButton, false);
    }
});
