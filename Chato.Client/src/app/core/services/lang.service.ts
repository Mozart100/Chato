import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

const SELECTED_LANG = 'selected.language';

@Injectable({ providedIn: 'root' })
export class LangService {

    readonly langs = [
        { text: 'Hebrew', flag: 'assets/images/flags/israel.png', lang: 'he' },
        { text: 'English', flag: 'assets/images/flags/us.jpg', lang: 'en' }
    ];

    constructor(private translate: TranslateService) {
        this.loadSelectedLanguage();
    }

    private loadSelectedLanguage() {
        const lang = localStorage.getItem(SELECTED_LANG);
        if (lang) {
            this.setLanguage(lang);
        }
    }

    /**
     * Set language
     * @param lang language
     */
    setLanguage(lang: string) {
        this.translate.use(lang);
        this.setDirection(lang);
    }

    /**
     * Set page direction
     * @param lang language
     */
    setDirection(lang: string) {
        document.documentElement.dir = lang === 'he' ? 'rtl' : 'ltr';
        localStorage.setItem(SELECTED_LANG, lang);
    }


    get lang() {
        return this.translate.currentLang;
    }
}
