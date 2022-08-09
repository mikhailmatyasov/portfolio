export interface IUnhandledException {
    userName: string;
    errorMessage: string;
    stackTrace: number;
    dateTime: string;
}

export class UnhandledException implements IUnhandledException {
    userName: string;
    errorMessage: string;
    stackTrace: number;
    dateTime: string;
}

export interface IUnhandledExceptionFilter {
    userName: string;
    fromDate: string;
    toDate: string;
    searchText: string;
}

export class UnhandledExceptionFilter implements IUnhandledExceptionFilter {
    userName: string;
    fromDate: string;
    toDate: string;
    searchText: string;
}
