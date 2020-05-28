// sleep for the given milliseconds
function sleep(ms) {
    return new Promise((resolve) => setTimeout(resolve, ms));
}

let id = 0;
// prints the given error message to the user page
async function printError(error) {
    id += 1;
    let idString = id.toString() + "error";
    // creates the error div string
    var objString = "<div class=\"h4 errorMessage\" id=\"" + idString + "\">" +
        error + "</div>";
    // if this is not the first error
    if (id > 1) {
        let prv = id - 1;
        let prvId = prv.toString() + "error";
        let previous = document.getElementById(prvId);
        // removes the previous error message
        previous.remove();
    }
    $("#errBox").append(objString);
    let objTag = $("#" + idString);
    let objElem = document.getElementById(idString);
    objTag.fadeIn(0);
    // get the message width in order to center it
    let width = objElem.offsetWidth;
    let offset = -0.5 * width;
    let offsetString = offset.toString() + "px";
    objElem.style.marginLeft = offsetString;
    objTag.fadeOut(0);
    // display the error message for 3 seconds with fade in and out
    objTag.fadeIn();
    await sleep(3000);
    objTag.fadeOut();
}

// check if the given flight is valid
function isFlightValid(flight) {
    if (!flight) {
        return false;
    }
    if (!flight.flight_id || (flight.longitude == null)
        || (flight.latitude == null) || (flight.passengers == null)
        || !flight.company_name || !flight.date_time
        || (flight.is_external == null)) {
        return false;
    }
    return true;
}

// check if the given flight plan is valid
function isFlightPlanValid(flightPlan) {
    if (!flightPlan) {
        return false;
    }
    if ((flightPlan.passengers == null) || !flightPlan.company_name
        || !flightPlan.initial_location
        || (flightPlan.initial_location.longitude == null)
        || (flightPlan.initial_location.latitude == null)
        || !flightPlan.initial_location.date_time
        || !flightPlan.segments) {
        return false;
    } else {
        let segment;
        for (segment of flightPlan.segments) {
            if ((segment.longitude == null) || (segment.latitude == null)
                || (segment.timespan_seconds == null)) {
                return false;
            }
        }
    }
    return true;
}

// get the current date and time in UTC
function makeDateAndTime() {
    let date = new Date();
    let getTime = date.getTime();
    let dateAndTime = new Date(getTime).toISOString();
    let length = dateAndTime.length - 5;
    dateAndTime = dateAndTime.substr(0, length) + "Z";
    return dateAndTime;
}

// convert the given time and adds the given seconds to it
function convertTime(time, secondsToAdd) {
    time = new Date(time);
    time = new Date(time.getTime() + 1000 * secondsToAdd);
    let getTime = time.getTime();
    if (isNaN(getTime)) {
        return "Ending time exceeded last date possible";
    }
    time = new Date(getTime).toISOString();
    let length = time.length - 5;
    time = time.substr(0, length) + "Z";
    return time;
}
