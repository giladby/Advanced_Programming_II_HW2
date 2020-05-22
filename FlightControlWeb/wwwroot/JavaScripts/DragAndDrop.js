function DragAndDropFileController(selector, onDropCallback) {
    let el_ = document.getElementById("flightsList");
    let image = document.getElementById("dragAndDropImage");
    this.dragenter = function (e) {
        e.stopPropagation();
        e.preventDefault();
        image.style.visibility = "visible";
        image.classList.add('dropping');
    };
    this.dragover = function (e) {
        e.stopPropagation();
        e.preventDefault();
    };
    this.dragleave = function (e) {
        e.stopPropagation();
        e.preventDefault();
        image.style.visibility = "hidden";
        
    };
    this.drop = function (e) {
        e.stopPropagation();
        e.preventDefault();
        image.style.visibility = "hidden";
        image.classList.remove('dropping');
        onDropCallback(e.dataTransfer.files, e);
    };
    el_.addEventListener('dragenter', this.dragenter, false);
    image.addEventListener('dragover', this.dragover, false);
    image.addEventListener('dragleave', this.dragleave, false);
    image.addEventListener('drop', this.drop, false);
}

function DragAndDropFunc() {
    new DragAndDropFileController('body', function (files) {
        let f = files[0];
        if (!f) {
            return;
        }
        if (f.type.match('application/json')) {
            let reader = new FileReader();
            reader.onloadend = function (e) {
                try {
                    let result = JSON.parse(this.result);
                    AddFlightPlanFunc(result);
                } catch {
                    printError("Received invalid JSON file");
                }
            };
            reader.readAsText(f);
        } else {
            printError("The given file is not a JSON file");
        }
    });
}
