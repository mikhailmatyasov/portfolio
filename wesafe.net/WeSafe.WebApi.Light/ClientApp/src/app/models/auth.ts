export class LoginModel {
    userName: string;
    password: string;
}

export enum Roles {
    Administrators = 'Administrators',
    Operators = 'Operators',
    Users = 'Users'
}

export interface ITokenResponse {
    accessToken: string;
    userName: string;
    displayName: string;
    role: string;
    expiresAt: string;
    demo: boolean;
}

export class SignUpModel {
    deviceToken: string;
    deviceType: number;
    userName: string;
    password: string;
    name: string;
    phone: string;
}

export class ProfileModel {
    displayName: string;
    oldPassword: string;
    password: string;
    confirmPassword: string;
}
