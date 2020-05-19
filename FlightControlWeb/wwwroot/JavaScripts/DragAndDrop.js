function DragAndDropFileController(selector, onDropCallback) {
    var el_ = document.getElementById("flightsList");
    var image = document.getElementById("dragAndDropImage");
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
        var f = files[0];
        if (!f) {
            return;
        }
        if (f.type.match('application/json')) {
            var reader = new FileReader();
            reader.onloadend = function (e) {
                try {
                    var result = JSON.parse(this.result);
                    AddFlightPlanFunc(result);
                } catch {
                    alert('Please enter a valid JSON file');
                }
            };
            reader.readAsText(f);
        } else {
            alert('Please enter a JSON file!');
        }
    });
}
