let id = 0
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
    let objTag = $("#" + idString);
    let objElem = document.getElementById(idString);
    objTag.fadeIn(0);
    let width = objElem.offsetWidth;
    let offset = -0.5 * width;
    let offsetString = offset.toString() + "px";
    objElem.style.marginLeft = offsetString;
    objTag.fadeOut(0);
    objTag.fadeIn();
    await Sleep(3000);
    objTag.fadeOut();
}

function Sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function IsFlightValid(flight) {
    if (!flight) {
        return false;
    }
    if (!flight.flight_id || (flightPlan.longitude == null) || (flightPlan.latitude == null)
        || (flightPlan.passengers == null) || !flight.company_name
        || !flight.date_time || (flightPlan.is_external == null)) {
        return false;
    }
    return true;
}

function IsFlightPlanValid(flightPlan) {
    if (!flightPlan) {
        return false;
    }
    if ((flightPlan.passengers == null) || !flightPlan.company_name || !flightPlan.initial_location
        || (flightPlan.initial_location.longitude == null) || (flightPlan.initial_location.latitude == null)
        || !flightPlan.initial_location.date_time || !flightPlan.segments) {
        return false;
    } else {
        flightPlan.segments.forEach(function (segment) {
            if ((segment.longitude == null) || (segment.latitude == null) || (segment.timespan_seconds == null)) {
                return false;
            }
        });
    }
    return true;
}