export interface IClientSubscriber {
    id: number;
    phone: string;
    clientId: number;
    permissions: string;
    name: string;
    isActive: boolean;
    createdAt: string;
}

export class ClientSubscriber implements IClientSubscriber {
    id: number;
    clientId: number;
    permissions: string;
    phone: string;
    name: string;
    isActive: boolean;
    createdAt: string;
}
