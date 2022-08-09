export interface IDeviceLog{
    clientName: string;
    deviceName: string;
    cameraName: string;
    logLevel: string;
    errorMessage: string;
    dataTime: string;
}

export class DeviceLog implements IDeviceLog {
    clientName: string;
    deviceName: string;
    cameraName: string;
    logLevel: string;
    errorMessage: string;
    dataTime: string;
}

export interface IDeviceLogFilter {
    logLevels: Array<string>;
    clientId: number;
    cameraId: number;
    deviceId: number;
    fromDate: string;
    toDate: string;
    searchText: string;
}

export class DeviceLogFilter implements IDeviceLogFilter {
    logLevels: Array<string>;
    clientId: number;
    cameraId: number;
    deviceId: number;
    fromDate: string;
    toDate: string;
    searchText: string;
}

export interface ILogLevel {
    id: number;
    value: string;
}

export class LogLevel implements ILogLevel {
    id: number;
    value: string;
}
