/// <reference path="jquery-1.7.1.js" />
/*globals $, window */

$(function () {
    $.ajaxSetup({ cache: false });
});

(function (window) {
    var ajaxUtil = window.ajaxUtil = {};

    ajaxUtil.modify = function (url, dataToProcess, httpVerb, successCallback, errorCallback) {
        $.ajax(url, {
            data: dataToProcess,
            type: httpVerb,
            dataType: 'json',
            contentType: 'application/json',
            success: function (data) {                
                if (successCallback !== undefined) {
                    successCallback(data);
                }
            },
            error: function () {
                if (errorCallback !== undefined)
                    errorCallback();
            }
        });
    };

    ajaxUtil.add = function (url, dataToProcess, successCallback, errorCallback) {
        this.modify(url, dataToProcess, "POST", successCallback, errorCallback);
    };

    ajaxUtil.update = function (url, dataToProcess, successCallback, errorCallback) {
        this.modify(url, dataToProcess, "PUT", "Item Updated.", successCallback, errorCallback);
    };

    ajaxUtil.remove = function (url) {
        this.modify(url, null, "DELETE");
    };
}(window));