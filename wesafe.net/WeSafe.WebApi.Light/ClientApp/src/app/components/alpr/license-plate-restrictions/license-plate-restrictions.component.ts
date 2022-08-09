import { Component, OnInit, Input } from '@angular/core';
import { LicensePlateRestrictionService } from '../../../services/license-plate-restriction.service';
import { ILicensePlateType, ILicensePlateRestriction, LicensePlateRestriction } from '../../../models/licensePlateRestriction';

@Component({
  selector: 'app-license-plate-restrictions',
  templateUrl: './license-plate-restrictions.component.html',
  styleUrls: ['./license-plate-restrictions.component.scss']
})
export class LicensePlateRestrictionsComponent implements OnInit {

    @Input()
    deviceId;

    rows: Array<ILicensePlateRestriction> = [];
    licensePlate: string;
    licensePlateTypes: Array<ILicensePlateType> = [];
    selectedTypeId: number = 0;

    constructor(private _licensePlateRestrictionService: LicensePlateRestrictionService) { }

    ngOnInit() {
        this.loadLicensePlateRestrictions();
        this.getLicensePlateTypes();
    }

    loadLicensePlateRestrictions() {
        this._licensePlateRestrictionService.get(this.deviceId).subscribe(
          response => {
                this.rows = response;
          });
    }

    delete(id: number) {
        this._licensePlateRestrictionService.delete(id).subscribe(
            response => {
                this.loadLicensePlateRestrictions();
            });
    }

    add() {
        let license = new LicensePlateRestriction();
        license.licensePlate = this.licensePlate;
        license.licensePlateType = this.selectedTypeId.toString();

        this._licensePlateRestrictionService.create(this.deviceId, license).subscribe(
            response => {
                this.loadLicensePlateRestrictions();
            });
    }

    getLicensePlateTypes() {
        this._licensePlateRestrictionService.getLicensePlateTypes().subscribe(data => {
                this.licensePlateTypes = Object.keys(data).map(k => {
                    return { 'id': parseInt(k), 'value': data[k] };
                });
            },
            error => {
            });
    }

}
