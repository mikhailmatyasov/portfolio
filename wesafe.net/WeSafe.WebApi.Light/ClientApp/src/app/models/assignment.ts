export interface IAssignment {
    id: number;
    deviceId: number;
    deviceName: string;
    cameraId?: number;
    cameraName: string;
}

export class Assignment implements IAssignment{
    cameraId?: number;
    cameraName: string;
    deviceId: number;
    deviceName: string;
    id: number;

    constructor() {
    }
}
