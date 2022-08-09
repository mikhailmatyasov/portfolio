import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-alpr',
  templateUrl: './alpr.component.html',
  styleUrls: ['./alpr.component.scss']
})
export class AlprComponent implements OnInit {

    @Input()
    deviceId;

    constructor() { }

    ngOnInit() {
    }
}
