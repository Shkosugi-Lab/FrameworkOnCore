// The server's HTTP API (/api): JSON both ways, its messages in the page's language (Accept-Language).
import { lang } from './i18n.js';

export async function api(path, options = {}) {
  const response = await fetch('/api' + path, {
    ...options,
    headers: { 'Content-Type': 'application/json', 'Accept-Language': lang, ...(options.headers ?? {}) },
  });
  const text = await response.text();
  const body = text ? JSON.parse(text) : null;
  if (!response.ok) throw Object.assign(new Error(body?.error ?? response.statusText), { body, status: response.status });
  return body;
}

/** A text resource (the conversion's report, a container's log); null when there is none. */
export async function text(path) {
  const response = await fetch('/api' + path, { headers: { 'Accept-Language': lang } });
  return response.ok ? response.text() : null;
}

export const post = (path, body) => api(path, { method: 'POST', body: JSON.stringify(body ?? {}) });
export const remove = path => api(path, { method: 'DELETE' });
