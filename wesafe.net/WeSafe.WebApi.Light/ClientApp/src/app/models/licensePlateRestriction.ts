export interface ILicensePlateRestriction {
    id: number;
    licensePlate: string;
    licensePlateType: string;
}

export class LicensePlateRestriction implements ILicensePlateRestriction {
    id: number;
    licensePlate: string;
    licensePlateType: string;
}

export interface ILicensePlateType {
    id: number;
    value: string;
}

export class LicensePlateType implements ILicensePlateType {
    id: number;
    value: string;
}
