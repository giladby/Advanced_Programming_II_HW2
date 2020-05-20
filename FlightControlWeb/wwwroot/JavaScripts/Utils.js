async function printError(error) {

    console.log("enter");

    var msg = document.getElementById("myErrorMsg");
    msg.style.opacity = "1";
    msg.innerHTML = error;
    let width = msg.offsetWidth;
    let offset = -0.5 * width;
    let offsetString = offset.toString() + "px"
    msg.style.marginLeft = offsetString;
    msg.style.visibility = "visible";

    await Sleep(5000);

    // Set the opacity of div to 0 (transparent)
    msg.style.opacity = "0";

    msg.style.visibility = "hidden";
}

function Sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}