(() => {
    try {
        const tema = localStorage.getItem('tema');

        if (tema === 'acik' || tema === 'koyu') {
            document.documentElement.dataset.tema = tema;
        }
    } catch {
        document.documentElement.removeAttribute('data-tema');
    }
})();
