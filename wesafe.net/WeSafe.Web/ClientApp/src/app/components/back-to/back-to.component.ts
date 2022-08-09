import { Component, OnInit, Input } from '@angular/core';

@Component({
    selector: 'app-back-to',
    templateUrl: './back-to.component.html',
    styleUrls: ['./back-to.component.scss']
})
export class BackToComponent implements OnInit {
    @Input()
    url: any;

    @Input()
    title: string;

    constructor() {}

    ngOnInit() {
    }

}
