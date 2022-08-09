export class Page {
    // The number of elements in the page
    size = 0;

    total = 0;

    // The current page number
    pageNumber = 0;

    constructor(pageNumber: number = 0, size: number = 10) {
        this.pageNumber = pageNumber;
        this.size = size;
    }
}
