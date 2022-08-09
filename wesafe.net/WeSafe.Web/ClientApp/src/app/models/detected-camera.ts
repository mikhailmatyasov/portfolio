export interface IDetectedCamera {
    id: number;
    name: string;
    ip: string;
    port: string;
    login: string;
    password: string;
    deviceId: number;
    state: number;
    detectingMethod: string;
    connectFailureText: string;
}

export class ConnectingDetectedCamera {
    id: number;
    login: string;
    password: string;
}
