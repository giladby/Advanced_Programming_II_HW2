// drag and drop functionality, also does image visibility toggle
function dragAndDropFileController(selector, onDropCallback) {
    let el_ = document.getElementById("flightsList");
    let image = document.getElementById("dragAndDropImage");
    this.dragenter = function (e) {
        dragEnter(e, image);
    };
    this.dragover = function (e) {
        dragOver(e);
    };
    this.dragleave = function (e) {
        dragLeave(e, image);
    };
    this.drop = function (e) {
        dragDrop(e, image, onDropCallback);
    };
    el_.addEventListener('dragenter', this.dragenter, false);
    image.addEventListener('dragover', this.dragover, false);
    image.addEventListener('dragleave', this.dragleave, false);
    image.addEventListener('drop', this.drop, false);
}

function dragEnter(e, image) {
    e.stopPropagation();
    e.preventDefault();
    image.style.visibility = "visible";
    image.classList.add('dropping');
}

function dragOver(e) {
    e.stopPropagation();
    e.preventDefault();
}

function dragLeave(e, image) {
    e.stopPropagation();
    e.preventDefault();
    image.style.visibility = "hidden";
}

function dragDrop(e, image, onDropCallback) {
    e.stopPropagation();
    e.preventDefault();
    image.style.visibility = "hidden";
    image.classList.remove('dropping');
    onDropCallback(e.dataTransfer.files, e);
}

// get a file into the drag and drop zone and send it to the server
function dragAndDropFunc() {
    new dragAndDropFileController('body', function (files) {
        analyzeFiles(files);
    });
}

// read the file and add it as flight plan if the file is valid
function analyzeFiles(files) {
    let f = files[0];
    if (!f) {
        return;
    }
    if (f.type.match('application/json')) {
        let reader = new FileReader();
        reader.onloadend = function (e) {
            tryAddFlightPlan(this.result);
        };
        reader.readAsText(f);
    } else {
        printError("The given file is not a JSON file");
    }
}

// add flight plan unless invalid
function tryAddFlightPlan(data) {
    try {
        let result = JSON.parse(data);
        addFlightPlanFunc(result);
    } catch {
        printError("Received invalid JSON file");
    }
}
