import { ICamera } from './camera';

export interface IDevice {
    id: number;
    serialNumber: string;
    hwVersion: string;
    swVersion: string;
    nvidiaSn: string;
    macAddress: string;
    clientId: number;
    clientName: string;
    clientNetworkIp: string;
    activationDate: string;
    assemblingDate: string;
    createdBy: string;
    info: string;
    token: string;
    status: string;
    isArmed: boolean;
    camerasNumber: number;
    name: string;
    maxActiveCameras: number;
    cameras?: Array<ICamera>;
}

export class Device implements IDevice {
    id: number;
    activationDate: string;
    assemblingDate: string;
    clientId: number;
    clientName: string;
    clientNetworkIp: string;
    createdBy: string;
    hwVersion: string;
    info: string;
    macAddress: string;
    nvidiaSn: string;
    swVersion: string;
    serialNumber: string;
    token: string;
    status: string;
    isArmed: boolean;
    camerasNumber: number;
    name: string;
    maxActiveCameras: number;

    constructor(obj?: IDevice) {
        if (obj) Object.assign(this, obj);
    }
}

export interface IDeviceType {
    id: number;
    value: string;
}

export class DeviceType implements IDeviceType {
    id: number;
    value: string;
}
