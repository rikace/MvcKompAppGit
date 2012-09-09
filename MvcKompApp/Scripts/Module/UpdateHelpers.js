GLOBALS.namespace("GLOBALS.Helpers.ResultAreaUpdateHelper");

GLOBALS.Helpers.ResultAreaUpdateHelper = function (data) {
    // Prefix #ResultArea ensures only child elements are retrieved.
    $("#ResultArea #Message").html(data.Message);
};
