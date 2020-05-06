function DragAndDropFunc() {
    function DnDFileController(selector, onDropCallback) {
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
    };

    new DnDFileController('body', function (files) {
        var f = files[0];

        if (!f.type.match('application/json')) {
            alert('Please enter a JSON file!');
        }

        var reader = new FileReader();
        reader.onloadend = function (e) {
            var result = JSON.parse(this.result);
            console.log(result);
            AddFlightPlan(result)
        };
        reader.readAsText(f);
        //GetFlightPlans();
    });
}
