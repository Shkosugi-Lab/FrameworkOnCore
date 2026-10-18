// FrameworkOnCore Studio's page: Cloudscape (the AWS console's design system) with its global styles and fonts.
import { createRoot } from 'react-dom/client';
import I18nProvider from '@cloudscape-design/components/i18n';
import enMessages from '@cloudscape-design/components/i18n/messages/all.en';
import jaMessages from '@cloudscape-design/components/i18n/messages/all.ja';
import '@cloudscape-design/global-styles/index.css';
import { lang } from './i18n.js';
import App from './App.jsx';

createRoot(document.getElementById('root')).render(
  <I18nProvider locale={lang} messages={[enMessages, jaMessages]}>
    <App />
  </I18nProvider>,
);
