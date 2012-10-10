/// <reference path="jquery-1.7.1-vsdoc.js" />
/// <reference path="jquery.validate-vsdoc.js" />
/// <reference path="jquery.validate.unobtrusive.js" />

jQuery.validator.addMethod("greater", function (value, element, param) {
    return Date.parse(value) > Date.parse($(param).val());
});

jQuery.validator.unobtrusive.adapters.add("greater", ["other"], function (options) {
    options.rules["greater"] = "#" + options.params.other;
    options.messages["greater"] = options.message;
});