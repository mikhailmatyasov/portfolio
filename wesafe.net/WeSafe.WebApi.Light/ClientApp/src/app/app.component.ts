import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
    dir = 'ltr';

    constructor(private _authService: AuthService) {
        // this language will be used as a fallback when a translation isn't found in the current language
        // translate.setDefaultLang('en');

        // the lang to use, if the lang isn't available, it will use the current loader to get them
        let selectedLang = localStorage.getItem('lang');

        if (!selectedLang) selectedLang = 'en';

        // translate.use(selectedLang);
    }

    ngOnInit(): void {
        this.setDir();
        window.addEventListener('storage', this.setDir, false);
    }

    setDir = () => {
        this.dir = this.getDir();
    }

    getDir = () => {
        const lang = localStorage.getItem('lang');

        if (!lang) return 'ltr';

        switch (lang) {
            case 'il': return 'rtl';
            case 'en': return 'ltr';
        }

        return 'ltr';
    }
}
