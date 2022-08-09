import { Component, OnInit } from '@angular/core';
import { AuthService } from "../../../../services/auth.service";

@Component({
  selector: 'app-admin-root',
  templateUrl: './admin-root.component.html',
  styleUrls: ['./admin-root.component.scss']
})
export class AdminRootComponent implements OnInit {

    constructor(public authService: AuthService) { }

  ngOnInit() {
  }

}
