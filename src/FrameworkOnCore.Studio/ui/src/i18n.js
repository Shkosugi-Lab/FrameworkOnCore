// The language of the page: the browser's first language Studio has (navigator.languages, in the user's order), else
// English. Every text of the page is in messages.js, each in both languages; the server's (its errors, the catalog's
// titles) follow the Accept-Language the page sends (api.js).
import { M, KNOWN_EN } from './messages.js';

const SUPPORTED = ['ja', 'en'];

export const lang = (navigator.languages?.length ? navigator.languages : [navigator.language])
  .map(l => String(l ?? '').toLowerCase().split('-')[0])
  .find(l => SUPPORTED.includes(l)) ?? 'en';

export const locale = lang === 'ja' ? 'ja-JP' : 'en-US';

document.documentElement.lang = lang;

/** The text of a key in the page's language; a function's text is given the arguments. */
export function t(key, ...args) {
  const entry = M[key];
  if (!entry) return key;
  const text = entry[lang === 'ja' ? 0 : 1];
  return typeof text === 'function' ? text(...args) : text;
}

export const fmt = n => Number(n ?? 0).toLocaleString(locale);
export const when = (value, options) => new Date(value).toLocaleString(locale, options);

/** How long since a time (or between two): seconds, else minutes and seconds. */
export function duration(from, to) {
  const s = Math.max(0, Math.round(((to ? new Date(to) : new Date()) - new Date(from)) / 1000));
  return s < 60 ? t('time.seconds', s) : t('time.minutes', Math.floor(s / 60), s % 60);
}

/** A text the server wrote in Japanese (the report's section titles, the analysis' notes), in the page's language. */
export function known(text) {
  if (lang === 'ja' || !text) return text;
  let out = KNOWN_EN[text] ?? text;
  for (const [ja, en] of Object.entries(KNOWN_EN)) if (out.includes(ja)) out = out.replace(ja, en);
  return out;
}
export const reportTitle = known;
