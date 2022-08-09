import { ICamera } from '../../../models/camera';

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
    deviceType: number;
    timeZone?: string;
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
    deviceType: number;

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

export enum DeviceTypeEnum {
    PeopleRecognition = 0,
    Traffic = 1,
    Alpr = 2
}

export interface IDeviceFilter {
    sort: string;
    clientId: number;
    search: string;
    filterBy: number;
}

export class DeviceFilter implements IDeviceFilter {
    sort: string;
    clientId: number;
    search: string;
    filterBy: number;
}
