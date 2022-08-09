import { Component, OnInit } from '@angular/core';
import { PermitedAdminIpService } from '../../services/permitted-admin-ip.service';
import { IPermittedAdminIp, PermittedAdminIp } from '../../models/permitted-admin-ip';

@Component({
  selector: 'app-ips-list',
  templateUrl: './ips-list.component.html',
  styleUrls: ['./ips-list.component.scss']
})
export class IpsListComponent implements OnInit {
    ips: Array<IPermittedAdminIp> = [];
    adding: boolean = false;
    addedIp: PermittedAdminIp;

    constructor(private _permitedAdminIpService: PermitedAdminIpService) { }

    ngOnInit() {
        this.loadIps();
  }

    loadIps() {
        this._permitedAdminIpService.getIps().subscribe(data => {
            this.ips = data;
        }, error => {
        });
    }

    deleteIp(id: number) {
        this._permitedAdminIpService.deleteIp(id).subscribe(data => {
            this.loadIps();
        }, error => {
        });     
    }

    addIp() {
        this.adding = true;
        this.addedIp = new PermittedAdminIp();
    }

    cancel() {
        this.adding = false;
    }

    save() {       
        this._permitedAdminIpService.createIp(this.addedIp).subscribe(data => {
            this.loadIps();
            this.adding = false;
        }, error => {
        });
    }
}
