function DragAndDropFileController(selector, onDropCallback) {
    var el_ = document.getElementById("flightsList");
    this.dragenter = function (e) {
        e.stopPropagation();
        e.preventDefault();
        el_.classList.add('dropping');
    };
    this.dragover = function (e) {
        e.stopPropagation();
        e.preventDefault();
    };
    this.dragleave = function (e) {
        e.stopPropagation();
        e.preventDefault();
    };
    this.drop = function (e) {
        e.stopPropagation();
        e.preventDefault();

        el_.classList.remove('dropping');

        onDropCallback(e.dataTransfer.files, e);
    };
    el_.addEventListener('dragenter', this.dragenter, false);
    el_.addEventListener('dragover', this.dragover, false);
    el_.addEventListener('dragleave', this.dragleave, false);
    el_.addEventListener('drop', this.drop, false);
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
                var result = JSON.parse(this.result);
                AddFlightPlanFunc(result);
            };
            reader.readAsText(f);
        } else {
            alert('Please enter a JSON file!');
        }
    });
}
