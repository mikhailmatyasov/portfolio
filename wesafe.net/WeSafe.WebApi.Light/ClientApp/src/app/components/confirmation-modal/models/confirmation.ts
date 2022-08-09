export class ConfirmationModel {

    constructor(public message: string = 'Are you sure?',
        public yesButtonText: string = "Yes",
        public noButtonText: string = "No") {
        this.message = message;
        this.yesButtonText = yesButtonText;
        this.noButtonText = noButtonText;
    }
}
