export interface ICameraType {
    id: number;
    name: string;
    rtspTemplate: string;
    isActive: boolean;
    cameraVendorId: number;
}

export class CameraType implements ICameraType {
    cameraVendorId: number;
    id: number;
    isActive: boolean;
    name: string;
    rtspTemplate: string;

    constructor(obj?: ICameraType) {
        if (obj) Object.assign(this, obj);
    }
}

export interface ICameraVendor {
    id: number;
    name: string;
    isActive: boolean;
}

export class CameraVendor implements ICameraVendor {
    id: number;
    isActive: boolean;
    name: string;

    constructor(obj?: ICameraVendor) {
        if (obj) Object.assign(this, obj);
    }
}
