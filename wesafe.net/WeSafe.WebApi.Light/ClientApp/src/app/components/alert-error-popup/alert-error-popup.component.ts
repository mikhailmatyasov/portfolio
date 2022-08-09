import { Component, OnInit, ViewChild, TemplateRef, EmbeddedViewRef } from '@angular/core';
import { Alert } from '../../models/alert-error-popup';
import { AlertService } from '../../services/alert-error-popup.service';
import { MatSnackBar, MatSnackBarVerticalPosition, MatSnackBarHorizontalPosition, MatSnackBarRef } from '@angular/material';

@Component({
    selector: 'app-alert-error-popup',
    templateUrl: './alert-error-popup.component.html',
  styleUrls: ['./alert-error-popup.component.scss']
})

export class AlertErrorPopupComponent implements OnInit {
    alerts: Alert[] = [];
    horizontalPosition: MatSnackBarHorizontalPosition = 'center';
    verticalPosition: MatSnackBarVerticalPosition = 'top';
    @ViewChild('errorTemplate', { static: true }) public errorTemplate: TemplateRef<any>;

    private snackBarInstance: MatSnackBarRef<EmbeddedViewRef<any>>;

    constructor(private alertService: AlertService, private snackBar: MatSnackBar) { }

    ngOnInit() {
        this.alertService.getAlert().subscribe((alert: Alert) => {
            if (!alert) {
                // clear alerts when an empty alert is received
                this.alerts = [];
                return;
            }

            this.snackBarInstance = this.snackBar.openFromTemplate(this.errorTemplate,
                {
                    data: alert,
                    duration: 100000,
                    horizontalPosition: this.horizontalPosition,
                    verticalPosition: this.verticalPosition,
                    panelClass: ['error-snackbar'],
                });
        });
    }

    closeError() {
        this.snackBarInstance.dismiss();
    }

    removeAlert(alert: Alert) {
        this.alerts = this.alerts.filter(x => x !== alert);
    }
}
