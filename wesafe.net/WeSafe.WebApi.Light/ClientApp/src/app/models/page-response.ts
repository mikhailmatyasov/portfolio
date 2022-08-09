export interface IPageResponse<T> {
    items: Array<T>;
    total: number;
}

export class PageResponse<T> implements IPageResponse<T>{
    items: Array<T>;
    total: number;

    constructor(items: Array<T>, total: number) {
        this.items = items;
        this.total = total;
    }
}
