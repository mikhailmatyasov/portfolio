import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AuthService } from '../../../services/auth.service';

@Component({
    selector: 'nav-header',
    templateUrl: 'nav-header.component.html',
    styleUrls: ['./nav-header.scss'],
    encapsulation: ViewEncapsulation.None
})

export class NavHeaderComponent implements OnInit {
    constructor(public authService: AuthService) {
    }

    ngOnInit() {
    }
}
