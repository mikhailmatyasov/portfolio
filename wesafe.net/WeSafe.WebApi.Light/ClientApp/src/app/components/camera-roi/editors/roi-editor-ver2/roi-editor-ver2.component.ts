import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationModalComponent } from '../../../confirmation-modal/confirmation-modal.component';
import { ConfirmationModel } from '../../../confirmation-modal/models/confirmation';
import { AreaFormComponent } from './area-form/area-form.component';

declare let SVG;
declare let loadImage;

@Component({
    selector: 'app-roi-editor-ver2',
    templateUrl: './roi-editor-ver2.component.html',
    styleUrls: ['./roi-editor-ver2.component.scss']
})
export class RoiEditorVer2Component implements OnInit {
    @Input()
    roi: any = {};

    @Input()
    lastImagePath: string;

    @Output()
    save = new EventEmitter<any>();

    @Output()
    cancel = new EventEmitter();

    area: any;
    editing = false;
    adding = false;

    draw;
    shapes;
    shape;
    currentSize;
    editShape;

    // @ts-ignore
    @ViewChild('canvas')
    canvas;

    constructor(private dialog: MatDialog) {
    }

    ngOnInit() {
        this.draw = SVG('canvas');

        //"\\assets\\roi\\img\\1-1-1576327706.1619835.jpg"
        loadImage(this.lastImagePath, (url, width, height) => {
            const size = this.calculateAspectRatio(width, height);

            size.originalWidth = width;
            size.originalHeight = height;
            this.draw.size(size.width, size.height);
            this.draw.image(this.lastImagePath, size.width, size.height);
            this.shapes = this.drawAreas(this.roi.areas, size);
            this.currentSize = size;
        });
    }

    onSave() {
        if (this.save) this.save.emit(this.roi);
    }

    onCancel() {
        if (this.cancel) this.cancel.emit();
    }

    getAreaTypeName(area) {
        const areaNames = {
            ignore: 'Ignore',
            perimeter: 'Perimeter',
            pool: 'Pool',
            entryline: "Entry Line",
            exitline:"Exit Line"
        };

        return areaNames[area.type];
    }

    getAreaColor(area) {
        const areaColors = {
            ignore: 'rgba(255,0,255,0.2)',
            perimeter: 'rgba(0,255, 0,0.2)',
            pool: 'rgba(0,0,255,0.2)',
            entryline: 'rgba(180,17,17,1)',
            exitline: 'rgba(107,224,60,1)'
        };

        return areaColors[area.type];
    }

    addArea() {
        const dialogRef = this.dialog.open(AreaFormComponent, {
            panelClass: 'dialog',
            data: {
                create: true,
                area: {
                    name: '',
                    type: 'perimeter'
                }
            }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (!result)
                return;

            const area = {
                name: result.name,
                type: result.type,
                points: []
            };

            if (result.type === 'entryline' || result.type === 'exitline') {
                this.shape = this.draw
                    .polyline()
                    .fill('none')
                    .stroke({ width: 3, color: this.getAreaColor(area) })
                    .draw();
            } else {
                this.shape = this.draw
                    .polygon()
                    .fill(this.getAreaColor(area))
                    .stroke({ width: 1, color: '#000' })
                    .draw();
            }


            this.area = area;
            this.editing = true;
            this.adding = true;
        });
    }

    editArea(area) {
        const dialogRef = this.dialog.open(AreaFormComponent, {
            panelClass: 'dialog',
            data: {
                create: false,
                area: area
            }
        });

        dialogRef.afterClosed().subscribe(result => {
            area.name = result.name;
        });
    }

    deleteArea(area) {
        const dialogRef = this.dialog.open(ConfirmationModalComponent, {
            panelClass: 'dialog',
            data: new ConfirmationModel("Delete this area?", "yes, delete", "no, cancel")
        });

        dialogRef.afterClosed().subscribe(result => {
            if (!result)
                return;

            let i = this.roi.areas.indexOf(area);

            this.roi.areas.splice(i, 1);

            const shape = this.shapes.findIndex(item => item.area === area);

            if (shape !== -1) {
                this.shapes[shape].shape.remove();
                this.shapes.splice(shape, 1);
            }
        });
    }

    submit() {
        this.shape.draw('done');
        this.shape.off('drawstart');
        this.area.points = this.toRoi(this.shape);
        this.roi.areas.push(this.area);
        this.shapes.push({ shape: this.shape, area: this.area });
        //this.setupShape(this.shape, this.area);
        this.editing = false;
        this.adding = false;
    }

    close() {
        this.shape.draw('cancel');
        this.shape.off('drawstart');
        this.editing = false;
        this.adding = false;
    }

    closeEdit() {
        this.editing = false;
        this.adding = false;
    }

    private calculateAspectRatio(width, height) {
        const canvasWidth = this.canvas.nativeElement.clientWidth;
        const imageRatio = canvasWidth / width;

        return {
            width: width * imageRatio,
            height: height * imageRatio,
            ratio: imageRatio,
            originalWidth: width,
            originalHeight: height
        };
    }

    private drawAreas(areas, size) {
        return areas.map(area => {
            let shape = null;

            if (area.type === 'entryline' || area.type === 'exitline') {
                shape = this.draw
                    .polyline(this.getShapeSegments(this.adjustPoints(area.points, size)))
                    .fill('none')
                    .stroke({ width: 3, color: this.getAreaColor(area) });
            } else {
                shape = this.draw
                    .polygon(this.getShapeSegments(this.adjustPoints(area.points, size)))
                    .fill(this.getAreaColor(area))
                    .stroke({ width: 1, color: '#000' });
            }

            //this.setupShape(shape, area);

            return { shape, area };
        });
    }

    private setupShape(shape, area) {
        shape.click(event => {
            if (this.editing) return;

            this.area = area;
            this.editShape = shape;
            this.editing = true;
        });
    }

    private adjustPoints(points, size) {
        return points.map(point => {
            return [point[0] * size.ratio * size.originalWidth, point[1] * size.ratio * size.originalHeight];
        });
    }

    private getShapeSegments(points) {
        return points.map(point => {
            return point.join(',');
        }).join(' ');
    }

    private toRoi(shape) {
        const roi = [];

        for (let i = 0; i < shape.node.points.length; i++) {
            const arr = [
                (shape.node.points[i].x / this.currentSize.ratio / this.currentSize.originalWidth),
                (shape.node.points[i].y / this.currentSize.ratio / this.currentSize.originalHeight)
            ];

            roi.push(arr);
        }

        return roi;
    }
}
