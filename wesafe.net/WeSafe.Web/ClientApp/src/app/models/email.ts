export interface IEmail {
    id: number;
    mailAddress: string;
    notifyServerException: boolean;
}

export class Email implements IEmail {
    id: number;
    mailAddress: string;
    notifyServerException: boolean;
}
