//////////////////////////////////////////////////
// Global container of update helpers
var UpdateHelpers = UpdateHelpers || {};


//////////////////////////////////////////////////
// Add page specific helpers

// Knows how to refresh the ResultArea1 panel
UpdateHelpers.ResultAreaUpdater1 = function (data) {
    // Prefix #ResultArea ensures only child elements are retrieved.
    $("#ResultArea1 #Message1").html(data.Message);
};


// Knows how to refresh the ResultArea2 panel
UpdateHelpers.ResultAreaUpdater2 = function (data) {
    // Prefix #ResultArea ensures only child elements are retrieved.
    $("#ResultArea2 #Message2").html(data.Message);
};