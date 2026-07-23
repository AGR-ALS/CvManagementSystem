import i18n from 'i18next';
import {initReactI18next} from 'react-i18next';

import en from './locales/en/translation.json';
import ru from './locales/ru/translation.json';

const savedLng = localStorage.getItem('i18nextLng') || 'en';

i18n.use(initReactI18next).init({
    resources: {
        en: {translation: en},
        ru: {translation: ru},
    },
    lng: savedLng,
    fallbackLng: 'en',
    interpolation: {
        escapeValue: false,
    },
});

i18n.on('languageChanged', (lng) => {
    localStorage.setItem('i18nextLng', lng);
});

export default i18n;
