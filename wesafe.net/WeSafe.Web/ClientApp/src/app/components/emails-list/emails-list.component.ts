import { Component, OnInit } from '@angular/core';
import { IEmail, Email } from '../../models/email';
import { EmailService } from '../../services/email.service';

@Component({
  selector: 'app-emails-list',
  templateUrl: './emails-list.component.html',
    styleUrls: ['./emails-list.component.scss']
})
export class EmailsListComponent implements OnInit {

    emails: Array<IEmail> = [];
    adding: boolean = false;
    addedEmail: IEmail;

    constructor(private _emailService: EmailService) { }

    ngOnInit() {
        this.loadEmails();
    }

    loadEmails() {
        this._emailService.getEmails().subscribe(data => {
            this.emails = data;
        }, error => {
        });
    }

    deleteEmail(id: number) {
        this._emailService.deleteEmail(id).subscribe(data => {
            this.loadEmails();
        }, error => {
        });
    }

    addEmail() {
        this.adding = true;
        this.addedEmail = new Email();
    }

    changeNotifyServerException(id: number) {
        this._emailService.changeNotifyServerException(id).subscribe(data => {
            this.loadEmails();
        }, error => {
        });
    }

    cancel() {
        this.adding = false;
    }

    save() {
        this._emailService.createEmail(this.addedEmail).subscribe(data => {
            this.loadEmails();
            this.adding = false;
        }, error => {
        });
    }

}
