"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ConfirmationModel = /** @class */ (function () {
    function ConfirmationModel(message, yesButtonText, noButtonText) {
        if (message === void 0) { message = 'Are you sure?'; }
        if (yesButtonText === void 0) { yesButtonText = "Yes"; }
        if (noButtonText === void 0) { noButtonText = "No"; }
        this.message = message;
        this.yesButtonText = yesButtonText;
        this.noButtonText = noButtonText;
        this.message = message;
        this.yesButtonText = yesButtonText;
        this.noButtonText = noButtonText;
    }
    return ConfirmationModel;
}());
exports.ConfirmationModel = ConfirmationModel;
//# sourceMappingURL=confirmation.js.map