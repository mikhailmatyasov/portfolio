let areas = [];
let draw;
let imageRatio;
let changedAreaCallBack;

function init(ignore_roi, perimeter_roi, pool_roi, image_path, localChangedAreaCallBack) {
    let ignore_roi_clone = clone(ignore_roi);
    let perimeter_roi_clone = clone(perimeter_roi);
    let pool_roi_clone = clone(pool_roi);

    changedAreaCallBack = localChangedAreaCallBack;

    areas = [];
    areas.push({ key: 'ignore', roi: ignore_roi, color: 'rgba(255,0,255,0.2)', shape: null, completedManualDrawing: true });
    areas.push({ key: 'perimeter', roi: perimeter_roi, color: 'rgba(0,255, 0,0.2)', shape: null, completedManualDrawing: true });
    areas.push({ key: 'pool', roi: pool_roi, color: 'rgba(0,0,255,0.2)', shape: null, completedManualDrawing:  true });

    /*SVG.on(document, 'DOMContentLoaded', function() {
        loadImage(imagePath, imageLoaded);
    })*/
    loadImage(image_path, imageLoaded);
}


function imageLoaded(url, width, height) {
    let newSize = calculateAspectRatioFit(width, height);

    if (draw) draw.clear();
    $("#canvas").empty();
    draw = SVG('canvas').size(newSize.width, newSize.height);

    draw.image(url, newSize.width, newSize.height);

    drawAreas(imageRatio)

    setCallbacks(imageRatio);
}

function drawAreas(ratio) {
    areas.forEach(function(area) {
        drawArea(area, ratio);
    });
}

function drawArea(area, ratio, isManualDraw) {
    if (!isManualDraw)
        area.shape = draw.polygon(toRoiString(getNewPointsRatio(area.roi, ratio))).fill(area.color).stroke({ width: 1 });

    if (isManualDraw) {
        area.shape = draw.polygon().fill(area.color).stroke({ width: 1 }).draw();
        area.completedManualDrawing = false;
    }
}

function removeAreas(keys) {
    areas.forEach(function(area) {
        if (!area.shape)
            return;
        if (!keys) {
            area.shape.remove();
            area.shape = null;
        } else if (keys.indexOf(area.key) >= 0) {
            area.shape.remove();
            area.shape = null;
        }
    });
}

function setCallbacks() {
    $("button[data-key]").click(function() {
        let key = $(this).data('key')

        let area = findAreaByKey(key);
        removeAreas([area.key]);
        $("#tools").removeClass('d-flex');
        $("#tools").addClass('d-none');
        $("#controls").addClass('d-flex');
        $("#controls").removeClass('d-none');
        drawArea(area, imageRatio, true);
    });

    $("button[data-complete]").click(function(event) {
        var dataValue = $(this).data("complete");

        $("#controls").removeClass('d-flex');
        $("#controls").addClass('d-none');
        $("#tools").addClass('d-flex');
        $("#tools").removeClass('d-none');

        var activeArea = areas.filter(function(a) {
           return !a.completedManualDrawing;
        })[0];

        if (dataValue == "success") {
			finishDraw(activeArea);
			activeArea.roi = toNativeRoi(activeArea);
        } else
			finishDraw(activeArea);

        removeAreas();
        drawAreas(imageRatio);

        changedAreaCallBack(activeArea);

        event.stopImmediatePropagation();
        event.stopPropagation();
    });
}

function toNativeRoi(area){
	let roi = [];
	let localRatio = (!imageRatio)? 1: imageRatio;

	for(let i = 0; i < area.shape.node.points.length; i++){
		let arr = [Math.floor(area.shape.node.points[i].x / localRatio), Math.floor(area.shape.node.points[i].y / localRatio)]
		roi.push(arr)
	}

	return roi;
}

function finishDraw(area){
	area.shape.draw('done');
    area.shape.off('drawstart');
    area.completedManualDrawing = true;
}

function findAreaByKey(key) {
    let area = areas.filter(function(a) {
        return a.key == key;
    })[0];

    if (!area)
        throw Exception("Area with key = " + key + " not found.")

    return area;
}

function calculateAspectRatioFit(srcWidth, srcHeight) {
    var windowsWidth = $(window).width();
    if (windowsWidth < srcWidth) {
        imageRatio = Math.min(windowsWidth / srcWidth);

        return { width: srcWidth * imageRatio, height: srcHeight * imageRatio, ratio: imageRatio };
    }

    return { width: srcWidth, height: srcHeight, ratio: null };
}

function toRoiString(inputArray) {
    let roiString = "";
    for (var i = 0; i < inputArray.length; i++) {
        roiString += inputArray[i].join(',') + " ";
    }

    return roiString.trim();
}

function getNewPointsRatio(inputArray, ratio) {
    var arrayClone = clone(inputArray);

    if (!ratio)
        return arrayClone;

    for (var i = 0; i < arrayClone.length; i++) {
        arrayClone[i][0] = arrayClone[i][0] * ratio;
        arrayClone[i][1] = arrayClone[i][1] * ratio;
    }

    return arrayClone;
}

function loadImage(url, getImageSize) {
    var img = new Image();
    img.onload = function() {
        getImageSize(url, this.width, this.height);
    }
    img.src = url;
}

function clone(object) {
    return JSON.parse(JSON.stringify(object));
}
