import { Injectable } from '@angular/core';

@Injectable()
export class LocalStorage {
    public getItem(key: string) {
        return JSON.parse(localStorage.getItem(key));
    }

    public setItem(key: string, value: any) {
        localStorage.setItem(key, JSON.stringify(value));
    }

    public remove(key: string) {
        localStorage.removeItem(key);
    }
}
