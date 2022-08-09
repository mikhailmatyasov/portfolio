class CameraManufactor {
    id: number;
    manufactor: string;
    cameraMarks: Array<CameraMark>;
}

class CameraMark {
    id: number;
    model: string;
    rtspPaths: Array<CameraPath>;
}

class CameraPath {
    id: number;
    path:string;
}
