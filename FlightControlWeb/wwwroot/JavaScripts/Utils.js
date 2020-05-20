async function printError(error) {

    var msg = document.getElementById("myErrorMsg");
    msg.innerHTML = error;

    $("#myErrorMsg").fadeIn(0);
    let width = msg.offsetWidth;
    let offset = -0.5 * width;
    let offsetString = offset.toString() + "px";
    msg.style.marginLeft = offsetString;
    $("#myErrorMsg").fadeOut(0);
    
    $("#myErrorMsg").fadeIn();
    await Sleep(3000);
    $("#myErrorMsg").fadeOut();
}

function Sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}