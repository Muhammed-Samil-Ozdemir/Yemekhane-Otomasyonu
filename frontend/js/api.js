export class ApiError extends Error {
    constructor(message, status) {
        super(message);
        this.status = status;
    }
}

const LOGIN_PAGE = 'giris.html';

async function request(method, path, body) {
    let response;

    try {
        response = await fetch(`/api/${path}`, {
            method,
            headers: body === undefined ? {} : { 'Content-Type': 'application/json' },
            body: body === undefined ? undefined : JSON.stringify(body),
            credentials: 'same-origin'
        });
    } catch {
        throw new ApiError('Sunucuya ulaşılamadı. Bağlantınızı kontrol edip tekrar deneyin.', 0);
    }

    const result = await response.json().catch(() => null);

    if (response.status === 401 && !location.pathname.endsWith(LOGIN_PAGE)) {
        location.replace(LOGIN_PAGE);
        return new Promise(() => {});
    }

    if (!response.ok || !result?.isSuccess) {
        const message = result?.errorMessages?.length
            ? result.errorMessages.join(' ')
            : 'Beklenmeyen bir hata oluştu. Lütfen tekrar deneyin.';

        throw new ApiError(message, response.status);
    }

    return result.data;
}

export const api = {
    get: path => request('GET', path),
    post: (path, body = {}) => request('POST', path, body),
    put: (path, body) => request('PUT', path, body)
};
