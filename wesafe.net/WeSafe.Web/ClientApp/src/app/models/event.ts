export interface IEvent {
    id: number;
    deviceId: number;
    deviceName: string;
    cameraId: number;
    cameraName: string;
    alert: boolean;
    parameters: string;
    message: string;
    time: string;
    entries: Array<IEventEntry>;
}

export interface IEventEntry {
    id: number;
    cameraLogId: number;
    typeKey: string;
    imageUrl: string;
    urlExpiration: string;
}

export interface IEventFilter {
    cameraId: number;
    deviceId: number;
    fromDate: string;
    toDate: string;
}

export class EventFilter implements IEventFilter {
    cameraId: number;
    deviceId: number;
    fromDate: string;
    toDate: string;
}
