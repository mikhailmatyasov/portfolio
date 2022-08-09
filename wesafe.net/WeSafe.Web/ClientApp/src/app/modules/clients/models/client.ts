export interface IClient {
    id: number;
    name: string;
    phone: string;
    contractNumber: string;
    token: string;
    info: string;
    createdAt: string;
    isActive: boolean;
    sendToDevChat: boolean;
}

export class Client implements IClient {
    contractNumber: string;
    createdAt: string;
    id: number;
    info: string;
    isActive: boolean;
    name: string;
    phone: string;
    token: string;
    sendToDevChat: boolean;
}
