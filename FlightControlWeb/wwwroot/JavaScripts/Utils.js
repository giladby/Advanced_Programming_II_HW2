var id = 0
async function printError(error) {
    id += 1;
    idString = id.toString() + "error";
    let objString = "<div class=\"h4 errorMessage\" id=\"" + idString + "\">" + error + "</div>";

    if (id > 1) {
        let prv = id - 1;
        let prvId = prv.toString() + "error";
        let previous = document.getElementById(prvId);
        previous.remove();
    }

    $("#errBox").append(objString);

    let obj = $("#" + idString);
    let objElem = document.getElementById(idString);

    obj.fadeIn(0);
    let width = objElem.offsetWidth;
    let offset = -0.5 * width;
    let offsetString = offset.toString() + "px";
    objElem.style.marginLeft = offsetString;
    obj.fadeOut(0);
    
    obj.fadeIn();
    await Sleep(3000);
    obj.fadeOut();
}

function Sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}