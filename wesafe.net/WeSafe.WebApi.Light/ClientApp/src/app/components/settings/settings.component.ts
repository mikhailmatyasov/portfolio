import { Component, OnInit } from '@angular/core';
import { IGlobalSettings } from '../../models/global-settings';
import { GlobalSettingsService } from '../../services/global-settings.service';

@Component({
    selector: 'app-settings',
    templateUrl: './settings.component.html',
    styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
    settings: IGlobalSettings = null;
    pending = false;

    constructor(private _settingsService: GlobalSettingsService) {
    }

    ngOnInit() {
        this.loadSettings();
    }

    update() {
        this.pending = true;
        this._settingsService.updateSettings(this.settings).subscribe(data => {
            this.pending = false;
            this.loadSettings();
        }, err => this.pending = false);
    }

    private loadSettings() {
        this.pending = true;
        this._settingsService.getSettings().subscribe(data => {
            this.settings = data;
            this.pending = false;
        }, err => this.pending = false);
    }
}
