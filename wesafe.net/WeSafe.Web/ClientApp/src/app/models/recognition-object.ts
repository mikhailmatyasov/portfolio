export interface IRecognitionObject {
    id: number;
    name: string;
    description: string;
    isActive: boolean;
}

export class RecognitionObject implements  IRecognitionObject {
    description: string;
    id: number;
    isActive: boolean;
    name: string;
}
