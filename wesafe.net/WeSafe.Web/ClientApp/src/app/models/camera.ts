export interface ICamera {
    id: number;
    cameraName: string;
    ip: string;
    port: string;
    login: string;
    password: string;
    isForRecognition: boolean;
    direction: number;
    directionLeft: number;
    directionRight: number;
    isActive: boolean;
    timeStartRecord: number;
    timeStopRecord: number;
    separateIndex?: number;
    roi: string;
    schedule: string;
    specificRtcpConnectionString: string;
    deviceId: number;
    lastImagePath: string;
    status: string;
    recognitionSettings: string;
    networkStatus: string;
}

export class Camera implements ICamera {
    cameraName: string;
    direction: number;
    directionLeft: number;
    directionRight: number;
    id: number;
    ip: string;
    isActive: boolean;
    isForRecognition: boolean;
    login: string;
    password: string;
    port: string;
    roi: string;
    schedule: string;
    separateIndex?: number;
    specificRtcpConnectionString: string;
    timeStartRecord: number;
    timeStopRecord: number;
    deviceId: number;
    lastImagePath: string;
    status: string;
    recognitionSettings: string;
    networkStatus: string;

    constructor(value?: ICamera) {
        if (value) Object.assign(this, value);
    }
}

export interface ICameraStat {
    count: number;
    activeCount: number;
    maxActiveCameras: number;
}

export class RecognitionSettings {
    confidence: number;
    sensitivity: number;
    alertFrequency: number;

    constructor() {
        this.confidence = 90;
        this.sensitivity = 7;
        this.alertFrequency = 30;
    }

    public static parse(value: string): RecognitionSettings {
        const result = new RecognitionSettings();

        if (value) {
            const obj = JSON.parse(value);

            if (obj.confidence) result.confidence = obj.confidence;
            if (obj.sensitivity) result.sensitivity = obj.sensitivity;
            if (obj.alertFrequency) result.alertFrequency = obj.alertFrequency;
        }

        return result;
    }
}
