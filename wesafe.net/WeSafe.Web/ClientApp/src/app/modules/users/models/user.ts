export interface IUser {
    id?: string;
    userName: string;
    displayName: string;
    phone: string;
    email: string;
    isActive: boolean;
    roleName: string;
    isLoked: boolean;
}

export interface IUpsertUser extends IUser {
    password: string;
}

export class UpsertUser implements IUpsertUser {
    isLoked: boolean;
    displayName: string;
    email: string;
    id: string;
    isActive: boolean;
    password: string;
    phone: string;
    roleName: string;
    userName: string;

    constructor(obj?: IUser) {
        if (obj) Object.assign(this, obj);
    }
}
