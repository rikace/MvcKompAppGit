Sys.Mvc.ValidatorRegistry.validators.price = function (rule) {
    var minValue = rule.ValidationParameters["min"];

    return function (value, context) {

        if (!value || !value.length) {
            return true;
        }

        if (context.eventName != 'blur') {
            return true;
        }

        if (value > minValue) {
            var cents = value - Math.floor(value);
            if (cents >= 0.99 && cents < 0.995) {
                return true;
            }
        }

        return false;
    };
};

