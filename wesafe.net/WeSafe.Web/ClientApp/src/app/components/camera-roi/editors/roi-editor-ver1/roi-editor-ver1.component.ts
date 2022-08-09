import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Location } from '@angular/common';

declare let init;

@Component({
    selector: 'app-roi-editor-ver1',
    templateUrl: './roi-editor-ver1.component.html',
    styleUrls: ['./roi-editor-ver1.component.scss']
})
export class RoiEditorVer1Component implements OnInit {
    @Input()
    roi: any = {};

    @Input()
    lastImagePath: string;

    @Output()
    save = new EventEmitter<any>();

    @Output()
    cancel = new EventEmitter();

    constructor(private _location: Location) {
    }

    ngOnInit() {
        this.init();
    }

    init() {
        this.roi.roi_ignore = this.roi.roi_ignore || [];
        this.roi.roi_perimeter = this.roi.roi_perimeter || [];
        this.roi.roi_pool = this.roi.roi_pool || [];
        const imagePath = this.lastImagePath; //"\\assets\\roi\\img\\1-1-1576327706.1619835.jpg";

        init(this.roi.roi_ignore, this.roi.roi_perimeter, this.roi.roi_pool, imagePath, changedArea => {
            // changedArea.roi contains your array for API.
            if (changedArea.key === 'ignore') this.roi.roi_ignore = changedArea.roi;
            else if (changedArea.key === 'perimeter') this.roi.roi_perimeter = changedArea.roi;
            else if (changedArea.key === 'pool') this.roi.roi_pool = changedArea.roi;
        });
    }

    clear(member) {
        if (this.roi) {
            if (this.roi[member]) {
                delete (this.roi[member]);
                this.init();
            }
        }
    }

    onSave() {
        if (this.save) this.save.emit(this.roi);
    }

    onCancel() {
        if (this.cancel) this.cancel.emit();
    }

    back() {
        this._location.back();
    }
}
