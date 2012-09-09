
(function ($) {
    var _registeredElements = [];

    $.updater = function () {
        _methods = {
            setup: _setup,
            refresh: _refresh
        };

        return _methods;
    };


    var _setup = function (elementId, methodName) {
        _registeredElements[elementId] = methodName;
        return this;
    };

    var _refresh = function (elementId, data) {
        var func = _registeredElements[elementId];
        if (func != null)
            func(data);

        return this;
    };

})(jQuery);
