/* ****************************************************************************
*
* Copyright (c) Francesco Abbruzzese. All rights reserved.
* francesco@dotnet-programming.com
* http://www.dotnet-programming.com/
* 
* This software is subject to the the license at http://mvccontrolstoolkit.codeplex.com/license  
* and included in the license.txt file of this distribution.
* 
* You must not remove this notice, or any other, from this software.
*
* ***************************************************************************/
//utilities
function MvcControlsToolkit_Trim(stringToTrim) {
    return stringToTrim.replace(/^\s+|\s+$/g, "");
}

function GlobalEvalScriptInElementId(element) {
    var allScriptText = "";
    if (element.tagName == "SCRIPT") {
        allScriptText = element.text;
    }
    else {
        var scripts = $(element).find("script");
        for (var i = 0; i < scripts.length; i++) {
            allScriptText += scripts[i].text;
        }
    }
    jQuery.globalEval(allScriptText);

}
function GlobalEvalScriptAndDestroy(element) {
    var allScriptText = CollectScriptAndDestroy(element);
    jQuery.globalEval(allScriptText);

}
function CollectScriptAndDestroy(element) {
    var allScriptText = "";
    if (element.tagName == "SCRIPT") {
        allScriptText = element.text;
        $(element).remove();
    }
    else {
        var scripts = $(element).find("script");
        for (var i = 0; i < scripts.length; i++) {
            allScriptText += scripts[i].text;
        }
        scripts.remove();
    }
    return allScriptText;

}

function GlobalEvalScriptInElementIdById(id) {
    var scripts = $("#" + id).find("script");
    var allScriptText = "";
    for (var i = 0; i < scripts.length; i++) {
        allScriptText += scripts[i].text;
    }
    jQuery.globalEval(allScriptText);
}

function CollectAllScriptsInelement(id) {
    var scripts = $("#" + id).find("script");
    var allScriptText = "";
    for (var i = 0; i < scripts.length; i++) {
        allScriptText += scripts[i].text;
    }
    return allScriptText;
}
///////////////////Json serialization/////////////////////////////
(function ($) {

    // JSON RegExp
    var rvalidchars = /^[\],:{}\s]*$/;
    var rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g;
    var rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g;
    var rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g;
    var dateISO = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:[.,]\d+)?Z/i;
    var dateNet = /\/Date\((\d+)(?:-\d+)?\)\//i;

    // replacer RegExp
    var replaceISO = /"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})(?:[.,](\d+))?Z"/i;
    var replaceNet = /"\\\/Date\((\d+)(?:-\d+)?\)\\\/"/i;

    // determine JSON native support
    var nativeJSON = (window.JSON && window.JSON.parse) ? true : false;
    var extendedJSON = nativeJSON && window.JSON.parse('{"x":9}', function (k, v) { return "Y"; }) === "Y";

    var jsonDateConverter = function (key, value) {
        if (typeof (value) === "string") {
            if (dateISO.test(value)) {
                return new Date(value);
            }
            if (dateNet.test(value)) {
                return new Date(parseInt(dateNet.exec(value)[1], 10));
            }
        }
        return value;
    };

    $.extend({
        parseJSON: function (data) {
            /// <summary>Takes a well-formed JSON string and returns the resulting JavaScript object.</summary>
            /// <param name="data" type="String">The JSON string to parse.</param>
            /// <param name="convertDates" optional="true" type="Boolean">Set to true when you want ISO/Asp.net dates to be auto-converted to dates.</param>
            var convertDates=true;
            if (typeof data !== "string" || !data) {
                return null;
            }

            // Make sure leading/trailing whitespace is removed (IE can't handle it)
            data = $.trim(data);

            // Make sure the incoming data is actual JSON
            // Logic borrowed from http://json.org/json2.js
            if (rvalidchars.test(data
                .replace(rvalidescape, "@")
                .replace(rvalidtokens, "]")
                .replace(rvalidbraces, ""))) {
                // Try to use the native JSON parser
                if (extendedJSON || (nativeJSON && convertDates !== true)) {
                    return window.JSON.parse(data, convertDates === true ? jsonDateConverter : undefined);
                }
                else {
                    data = convertDates === true ?
                        data.replace(replaceISO, "new Date(parseInt('$1',10),parseInt('$2',10)-1,parseInt('$3',10),parseInt('$4',10),parseInt('$5',10),parseInt('$6',10),(function(s){return parseInt(s,10)||0;})('$7'))")
                            .replace(replaceNet, "new Date($1)") :
                        data;
                    return (new Function("return " + data))();
                }
            } else {
                $.error("Invalid JSON: " + data);
            }
        }
    });
})(jQuery);



//Generic validation functions
var ValidationType_StandardClient = "StandardClient";
var ValidationType_UnobtrusiveClient = "UnobtrusiveClient";
var ValidationType_Server = "Server";

//validate single field return value of true if valid
function MvcControlsToolkit_Validate(fieldName, validationType) {
    if (validationType == ValidationType_StandardClient) {
        if (typeof document.getElementById(fieldName)[MvcControlsToolkit_FieldContext_Tag] === 'undefined') return;
        var errorMessages = null;
        try{
            errorMessages=document.getElementById(fieldName)[MvcControlsToolkit_FieldContext_Tag].validate('blur');
        }
        catch(e){
        }
        if (errorMessages && errorMessages.length) {
            return false;
        }
        else {
            return true;
        }
    }
    else if (validationType == ValidationType_UnobtrusiveClient) {
        var selector = '#' + fieldName;
        var res = true;
        try{
            res=$(selector).parents('form').validate().element(selector);
        }
        catch(e){
        }
        return  res;
    }
    return true;
}
//validate whole form with return value of true if valid
function MvcControlsToolkit_FormIsValid(elementField, validationType)
{
    if (validationType == ValidationType_StandardClient) {
            var formContext = null;
            $('#' + elementField).parents('form').each(function (i) { formContext = this[MvcControlsToolkit_FieldContext_formValidationTag]; })
            if (formContext == null) return true;
            var errorMessages = formContext.validate('submit');
            if (errorMessages && errorMessages.length) {
                return false;
            }
            else {
                return true;
            }
        }
        else if (validationType == ValidationType_UnobtrusiveClient) {
        return $('#' + elementField).parents('form').validate().form();
        }
        else {
            return true;
        }
    }
 ////////////////////////////STANDARD VALIDATION///////////////////////////////////////////////////////////

// Constants used in validation
var MvcControlsToolkit_FieldContext_hasValidationFiredTag = '__MVC_HasValidationFired';
var MvcControlsToolkit_FieldContext_formValidationTag = '__MVC_FormValidation';
var MvcControlsToolkit_FieldContext_Tag = '__MVC_FieldContext';
var MvcControlsToolkit_SpecialFormName = '_Template_Data_';



// Validation Patch, to avoid that deleted input fields be validated
function MvcControlsToolkit_FormContext$_isElementInHierarchy(parent, child) {
    if (child == null) return false;
    while (child) {
        if (parent === child) {
            return true;
        }
        child = child.parentNode;
    }
    return false;
}
function MvcControlsToolkit_FieldContext$validate(eventName){
if (typeof Sys === 'undefined' || Sys === null || typeof Sys.Mvc === 'undefined' ||  Sys.Mvc === null || typeof Sys.Mvc.FormContext === 'undefined' || Sys.Mvc.FormContext === null) return;
    for (var j = 0; j < this.elements.length; j++) {
        if (!MvcControlsToolkit_FormContext$_isElementInHierarchy(document.body, this.elements[j])) {
            this.clearErrors();
            return [];
        }
    }
    return this.baseValidate(eventName);
}

function MvcControlsToolkit_FieldContext$enableDynamicValidation() {
    for (var i = 0; i < this.elements.length; i++) {
            var element = this.elements[i];
            element[MvcControlsToolkit_FieldContext_Tag]=this;
    }
    this.baseEnableDynamicValidation();
}

if (typeof Sys !== 'undefined' && Sys !== null && typeof Sys.Mvc !== 'undefined' && Sys.Mvc !== null && typeof Sys.Mvc.FormContext !== 'undefined' && Sys.Mvc.FormContext !== null) {

    Sys.Mvc.FieldContext.prototype.baseValidate = Sys.Mvc.FieldContext.prototype.validate;
    Sys.Mvc.FieldContext.prototype.validate = MvcControlsToolkit_FieldContext$validate;

    Sys.Mvc.FieldContext.prototype.baseEnableDynamicValidation= Sys.Mvc.FieldContext.prototype.enableDynamicValidation;
    Sys.Mvc.FieldContext.prototype.enableDynamicValidation = MvcControlsToolkit_FieldContext$enableDynamicValidation;

    Sys.Mvc.FieldContext.prototype._dependsOn = new Array();

    Sys.Mvc.FieldContext.prototype._dependencyOnBlur = function (e) {
        this.validate('blur');
    };
    
    Sys.Mvc.FieldContext.prototype.takeDynamicValue = function (fieldName) {
        var fieldToVerify = null;
        if (this.elements.length > 0) fieldToVerify = this.elements[0];
        if (fieldToVerify == null) return null;
        var name = fieldToVerify.name;
        var index = name.lastIndexOf('.');
        if (index >= 0) {
            var toCut = name.substring(index + 1);
            var thisId = fieldToVerify.id;
            thisId = thisId.substring(0, thisId.lastIndexOf(toCut));
            fieldName = thisId + fieldName;
        }
        var element = document.getElementById(fieldName);
        if (element == null) return null;
        var toInsert = false;
        if (this._dependsOn == null) {
            this._dependsOn = new Array();
            this._dependsOn[fieldName] = element;
            toInsert = true;
        }
        else {
            if (typeof this._dependsOn[fieldName] === 'undefined') {
                this._dependsOn[fieldName] = element;
                toInsert = true;
            }
        }
        if (toInsert) {
            Sys.UI.DomEvent.addHandler(element, 'blur', Function.createDelegate(this, this._dependencyOnBlur));
        }
        return element.value;
    };

     
    /////Multy field rules handling

    ///rule registration for standard validation//////

    ///Dynamic Range rule////
    Sys.Mvc.ValidatorRegistry.validators["dynamicrange"] = function (rule) {
        // initialization code can go here.

        var minValue = rule.ValidationParameters["min"];
        var maxValue = rule.ValidationParameters["max"];

        // we return the function that actually does the validation 
        return function (value, context) {
            if (!value || !value.length) return true; /*success*/
            var convertedValue = Number.parseLocale(value);
            if (!isNaN(convertedValue) &&
                (minValue == null || minValue <= convertedValue) &&
                (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return rule.ErrorMessage;
        };
    };

    ///Client Dynamic Range rule////
    Sys.Mvc.ValidatorRegistry.validators["clientdynamirange"] = function (rule) {
        // initialization code can go here.
        var nminValue = rule.ValidationParameters["min"];
        var nmaxValue = rule.ValidationParameters["max"];

        var minDelay = rule.ValidationParameters["mindelay"];
        var maxDelay = rule.ValidationParameters["maxdelay"];


        // we return the function that actually does the validation 
        return function (value, context) {
            if (!value || !value.length) return true; /*success*/
            var convertedValue = Number.parseLocale(value);
            var minValue = null;
            var maxValue = null;

            if (nminValue != null) {
                var sminValue = context.fieldContext.takeDynamicValue(nminValue);
                if (sminValue != null) {
                    minValue = Number.parseLocale(sminValue);
                    if (isNaN(minValue)) {
                        minValue = null;
                    }
                    else if (minDelay != null) {
                        minValue = minValue+minDelay;
                    }
                }

            }

            if (nmaxValue != null) {
                var smaxValue = context.fieldContext.takeDynamicValue(nmaxValue);
                if (smaxValue != null) {
                    maxValue = Number.parseLocale(smaxValue);
                    if (isNaN(maxValue)) {
                        maxValue = null;
                    }
                    else if (maxDelay != null) {
                        maxValue = maxValue+maxDelay;
                    }
                }

            }

            if (!isNaN(convertedValue) &&
                (minValue == null || minValue <= convertedValue) &&
                (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return rule.ErrorMessage;
        };
    };

    ///Date Range rule////
    Sys.Mvc.ValidatorRegistry.validators["daterange"] = function (rule) {
        // initialization code can go here.
        var sminValue = rule.ValidationParameters["min"];
        var smaxValue = rule.ValidationParameters["max"];
        var minValue = null;
        var maxValue = null;
        if (sminValue != null) {
            sminValue = "new "+sminValue.substring(1, sminValue.length - 1);
            minValue = eval(sminValue);
        }
        if (smaxValue != null) {
            smaxValue = "new "+smaxValue.substring(1, smaxValue.length - 1);
            maxValue = eval(smaxValue);
        }

        // we return the function that actually does the validation 
        return function (value, context) {
            if (!value || !value.length) return true; /*success*/
            var convertedValue = Date.parseLocale(value);
            if (convertedValue != null && 
                (minValue == null || minValue <= convertedValue) &&
                (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return rule.ErrorMessage;
        };
    };

    ///Client Dynamic Date Range rule////
    Sys.Mvc.ValidatorRegistry.validators["clientdynamicdaterange"] = function (rule) {
        // initialization code can go here.
        var nminValue = rule.ValidationParameters["min"];
        var nmaxValue = rule.ValidationParameters["max"];

        var minDelay = rule.ValidationParameters["mindelay"];
        var maxDelay = rule.ValidationParameters["maxdelay"];
        var minValue = null;
        var maxValue = null;
        


        // we return the function that actually does the validation 
        return function (value, context) {
            if (!value || !value.length) return true; /*success*/
            var convertedValue = Date.parseLocale(value);

            if (nminValue != null) {
                var sminValue = context.fieldContext.takeDynamicValue(nminValue);
                if (sminValue != null) {
                    minValue = Date.parseLocale(sminValue);
                    
                    if (minDelay != null) {
                        minValue = new Date(minValue.getTime() + minDelay);
                    }
                }

            }

            if (nmaxValue != null) {
                var smaxValue = context.fieldContext.takeDynamicValue(nmaxValue);
                if (smaxValue != null) {
                    maxValue = Date.parseLocale(smaxValue);
                    
                    if (maxDelay != null) {
                        maxValue = new Date(maxValue.getTime() + maxDelay);
                    }
                }

            }

            if (convertedValue != null &&
                (minValue == null || minValue <= convertedValue) &&
                (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return rule.ErrorMessage;
        };
    };

}
////////////////END STANDARD VALIDATION/////////////////////////////////////////////////

///////// UNOBTRUSIVE VALIDATION//////////////////////////////////



if (typeof $ !== 'undefined' && $ !== null && typeof $.validator !== 'undefined' && $.validator !== null && typeof $.validator.unobtrusive !== 'undefined' && $.validator.unobtrusive !== null) {

    
    (function ($) {
        ///////Support function for rules involving other fields(I call them client dynamic/////////////////////
        $.validator.takeDynamicValue = function(fieldToVerify, fieldName){

            if (fieldToVerify == null) return null;

            var name = fieldToVerify.name;
            var index = name.lastIndexOf('.');
            if (index >= 0) {
                var toCut = name.substring(index + 1);
                var thisId = fieldToVerify.id;
                thisId = thisId.substring(0, thisId.lastIndexOf(toCut));
                fieldName = thisId + fieldName;
            }
            var element = document.getElementById(fieldName);
            if (element == null) return null;
            var toInsert = false;
            var _dependsOn=jQuery.data(fieldToVerify, "_dependsOn");
            if (typeof _dependsOn == 'undefined' || _dependsOn == null) {
                _dependsOn = new Array();
                _dependsOn[fieldName] = element;
                jQuery.data(fieldToVerify, "_dependsOn", _dependsOn);
                toInsert = true;
            }
            else {
                if (typeof _dependsOn[fieldName] === 'undefined') {
                    _dependsOn[fieldName] = element;
                    toInsert = true;
                }
            }
            if (toInsert) {
               $(element).blur(function() {
                   $(fieldToVerify).parents('form').first().validate().element(fieldToVerify);
               });
            }
            return element.value;
        };
        //////////////parsing input elements parsing functions ///////////////////////////
        $.validator.unobtrusive.clearAndParse = function (selector) {
            var form = $(selector).parents("form");
            if (form.length != 0) {
                form.removeData("validator");
            }
            else {
                $(selector).removeData("validator");
            }
            $.validator.unobtrusive.parse(selector);
        }
        $.validator.unobtrusive.parseExt = function (selector) {

            $.validator.unobtrusive.parse(selector);


            var form = $(selector).first().closest('form');


            var unobtrusiveValidation = form.data('unobtrusiveValidation');
            var validator = form.validate();

            $.each(unobtrusiveValidation.options.rules, function (elname, elrules) {
                if (validator.settings.rules[elname] == undefined) {
                    var args = {};
                    $.extend(args, elrules);
                    args.messages = unobtrusiveValidation.options.messages[elname];
                    $('[name= "' + elname + '"]').rules("add", args);
                } else {
                    $.each(elrules, function (rulename, data) {
                        if (validator.settings.rules[elname][rulename] == undefined) {
                            var args = {};
                            args[rulename] = data;
                            args.messages = unobtrusiveValidation.options.messages[elname][rulename];
                            $('[name= "' + elname + '"]').rules("add", args);
                        }
                    });
                }
            });
        }
    })($);

    /// mandatory attribute
    $.validator.unobtrusive.adapters.addBool("mandatory", "required");
    /////Multy field rules handling

    ///Dynamic Range rule////
    $.validator.addMethod(
        "dynamicrange",
        function (value, element, param) {

            var minValue = param[0]; if (minValue == '') minValue = null;
            var maxValue = param[1]; if (maxValue == '') maxValue = null;


            if ((!value || !value.length) && this.optional(element)) return true; /*success*/
            var convertedValue = null;
            if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                convertedValue = jQuery.global.parseFloat(value);
            }
            else {
                convertedValue = parseFloat(value);
            }
            if (!isNaN(convertedValue) &&
            (minValue == null || minValue <= convertedValue) &&
            (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return false;
        },
    "value is not in the required range");
        jQuery.validator.unobtrusive.adapters.add("dynamicrange", ["min", "max"], function (options) {
            var min = options.params.min,
                max = options.params.max;

            options.rules["dynamicrange"] = [min, max];
            if (options.message) {
                options.messages["dynamicrange"] = options.message;
            }
        });
  ///Client Dynamic Range rule////
        $.validator.addMethod(
        "clientdynamirange",
        function (value, element, param) {
            var nminValue = param[0]; if (nminValue == '') nminValue = null;
            var nmaxValue = param[2]; if (nmaxValue == '') nmaxValue = null;

            var minDelay = param[1]; if (minDelay == '') minDelay = null;
            var maxDelay = param[3]; if (maxDelay == '') maxDelay = null;
            var minValue = null;
            var maxValue = null;
            if (minDelay != null) minDelay = parseFloat(minDelay);
            if (maxDelay != null) maxDelay = parseFloat(maxDelay);


            if ((!value || !value.length) && this.optional(element)) return true; /*success*/
            var convertedValue = null;
            if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                convertedValue = jQuery.global.parseFloat(value);
            }
            else {
                convertedValue = parseFloat(value);
            }
            

            if (nminValue != null) {
                var sminValue = $.validator.takeDynamicValue(element, nminValue);
                if (sminValue != null) {
                    minValue = null;
                    if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                        minValue = jQuery.global.parseFloat(sminValue);
                    }
                    else {
                        minValue = parseFloat(sminValue);
                    }
                    if (isNaN(minValue)) {
                        minValue = null;
                    }
                    else if (minDelay != null) {
                        minValue = minValue + minDelay;
                    }
                }

            }

            if (nmaxValue != null) {
                var smaxValue = $.validator.takeDynamicValue(element, nmaxValue);
                if (smaxValue != null) {
                    maxValue = null;
                    if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                        maxValue = jQuery.global.parseFloat(smaxValue);
                    }
                    else {
                        maxValue = parseFloat(smaxValue);
                    }
                    if (isNaN(maxValue)) {
                        maxValue = null;
                    }
                    else if (maxDelay != null) {
                        maxValue = maxValue + maxDelay;
                    }
                }

            }

            if (!isNaN(convertedValue) &&
            (minValue == null || minValue <= convertedValue) &&
            (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return false;

        },
        "value is not in the required range");
        jQuery.validator.unobtrusive.adapters.add("clientdynamirange", ["min", "mindelay", "max", "maxdelay"], function (options) {
            var min = options.params.min,
                mindelay = options.params.mindelay;
                max = options.params.max,
                maxdelay = options.params.maxdelay;

                options.rules["clientdynamirange"] = [min, mindelay, max, maxdelay];
            if (options.message) {
                options.messages["clientdynamirange"] = options.message;
            }
        });


        //////Date validation///////////////
        $.validator.addMethod(
        "daterange",
        function (value, element, param) {

            var minValue = param[0]; if (minValue == '') minValue = null;
            var maxValue = param[1]; if (maxValue == '') maxValue = null;

            if (minValue != null) minValue = new Date(minValue);
            if (maxValue != null) maxValue = new Date(maxValue);

            if ((!value || !value.length) && this.optional(element)) return true; /*success*/
            var convertedValue = null;
            if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                convertedValue = new Date(value);
                convertedValue = jQuery.global.parseDate(value);
            }
            else {
                convertedValue = new Date(value);
            }
            if (!isNaN(convertedValue) &&
            (minValue == null || minValue <= convertedValue) &&
            (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return false;
        },
    "date is not in the required range");
        jQuery.validator.unobtrusive.adapters.add("daterange", ["min", "max"], function (options) {
            var min = options.params.min,
                max = options.params.max;
            options.rules["daterange"] = [min, max];
            if (options.message) {
                options.messages["daterange"] = options.message;
            }
        });

        $.validator.addMethod(
        "clientdynamicdateRange",
        function (value, element, param) {
            var nminValue = param[0]; if (nminValue == '') nminValue = null;
            var nmaxValue = param[2]; if (nmaxValue == '') nmaxValue = null;

            var minDelay = param[1]; if (minDelay == '') minDelay = null;
            var maxDelay = param[3]; if (maxDelay == '') maxDelay = null;
            var minValue = null;
            var maxValue = null;
            if (minDelay != null) minDelay = parseInt(minDelay);
            if (maxDelay != null) maxDelay = parseInt(maxDelay);


            if ((!value || !value.length) && this.optional(element)) return true; /*success*/
            var convertedValue = null;
            if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                convertedValue = jQuery.global.parseDate(value);
            }
            else {
                convertedValue = parseDate(value);
            }


            if (nminValue != null) {
                var sminValue = $.validator.takeDynamicValue(element, nminValue);
                if (sminValue != null) {
                    minValue = null;
                    if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                        minValue = jQuery.global.parseDate(sminValue);
                    }
                    else {
                        minValue = parseDate(sminValue);
                    }
                    if (isNaN(minValue)) {
                        minValue = null;
                    }
                    else if (minDelay != null) {
                        minValue = new Date(minValue.getTime() + minDelay);
                    }
                }

            }

            if (nmaxValue != null) {
                var smaxValue = $.validator.takeDynamicValue(element, nmaxValue);
                if (smaxValue != null) {
                    maxValue = null;
                    if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                        maxValue = jQuery.global.parseDate(smaxValue);
                    }
                    else {
                        maxValue = parseDate(smaxValue);
                    }
                    if (isNaN(maxValue)) {
                        maxValue = null;
                    }
                    else if (maxDelay != null) {
                        maxValue = new Date(maxValue.getTime() + maxDelay);
                    }
                }

            }

            if (!isNaN(convertedValue) &&
            (minValue == null || minValue <= convertedValue) &&
            (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return false;

        },
        "date is not in the required range");

        jQuery.validator.unobtrusive.adapters.add("clientdynamicdateRange", ["min", "mindelay", "max", "maxdelay"], function (options) {
            var min = options.params.min,
                mindelay = options.params.mindelay;
            max = options.params.max,
                maxdelay = options.params.maxdelay;

            options.rules["clientdynamicdateRange"] = [min, mindelay, max, maxdelay];
            if (options.message) {
                options.messages["clientdynamicdateRange"] = options.message;
            }
        });
    
}

////////// END UNOBTRUSIVE VALIDATION //////////////////

// DUAL SELECT

var DualSelect_Separator = ";;;";
var DualSelect_SelectAvial = "_AvialSelect";
var DualSelect_SelectSelected = "_SelSelect";
var DualSelect_HiddenSelectedItemsVal = "";



var DualSelect_TempObjSource, DualSelect_TempObjDestination;



function DualSelect_SetObjects(dualSelectId, bDoSelected) {
    if (bDoSelected) {
        DualSelect_TempObjSource =
			document.getElementById(dualSelectId + DualSelect_SelectAvial);
        DualSelect_TempObjDestination =
			document.getElementById(dualSelectId + DualSelect_SelectSelected);
    }
    else {
        DualSelect_TempObjSource =
			document.getElementById(dualSelectId + DualSelect_SelectSelected);
        DualSelect_TempObjDestination =
			document.getElementById(dualSelectId + DualSelect_SelectAvial);
    }
}



function DualSelect_GetIndexForInsert(oSelect, oNode) {
    if (oSelect.autosort == "false")
        return oSelect.length + 1;

    if (oSelect.length == 0) return 0;

    for (var i = 0; i < oSelect.length; i++)
        if (oSelect[i].text > oNode.text)
            return i;
    return oSelect.length;
}



function DualSelect_MoveElement(dualSelectId, bDoSelected) {
    DualSelect_SetObjects(dualSelectId, bDoSelected);

    if (DualSelect_TempObjSource.length == 0) return;

    iLast = 0;

    for (var i = 0; i < DualSelect_TempObjSource.length; i++) {
        if (DualSelect_TempObjSource[i].selected) {
            iLast = i;
            var oNode = DualSelect_TempObjSource[i];
            DualSelect_TempObjSource.remove(i);
            nPos = (DualSelect_TempObjDestination.length + 1);
            DualSelect_TempObjDestination.options.add(oNode,
				DualSelect_GetIndexForInsert(DualSelect_TempObjDestination, oNode));

            i--;
        }
    }

    DualSelect_SaveSelection(dualSelectId);

    if (DualSelect_TempObjSource.length > 0 && iLast == 0)
        DualSelect_TempObjSource.selectedIndex = 0;
    else if (DualSelect_TempObjSource.length - 1 >= iLast)
        DualSelect_TempObjSource.selectedIndex = iLast;
    else if (DualSelect_TempObjSource.length >= 1)
        DualSelect_TempObjSource.selectedIndex = iLast - 1;

    DualSelect_ClearSelection(DualSelect_TempObjSource);
    DualSelect_TempObjSource.focus;
}

function DualSelect_Move_Up(dualSelectId, bDoSelected) {
    DualSelect_SetObjects(dualSelectId, bDoSelected);

    if (DualSelect_TempObjSource.length == 0) return;
    if (DualSelect_TempObjSource[0].selected) return;
    for (var i = 1; i < DualSelect_TempObjSource.length; i++) {
        if (DualSelect_TempObjSource[i].selected) {

            var tempValue = DualSelect_TempObjSource[i];
            var tempValue1 = DualSelect_TempObjSource[i - 1];
            DualSelect_TempObjSource.remove(i);
            DualSelect_TempObjSource.remove(i-1);
            DualSelect_TempObjSource.options.add(tempValue, i - 1);
            DualSelect_TempObjSource.options.add(tempValue1, i); 
           
            i--;
        }
    }

    DualSelect_SaveSelection(dualSelectId);
}

function DualSelect_Move_Down(dualSelectId, bDoSelected) {
    DualSelect_SetObjects(dualSelectId, bDoSelected);

    if (DualSelect_TempObjSource.length == 0) return;
    if (DualSelect_TempObjSource[DualSelect_TempObjSource.length - 1].selected) return;
    for (var i = DualSelect_TempObjSource.length-2; i > -1 ; i--) {
        if (DualSelect_TempObjSource[i].selected) {
            var tempValue = DualSelect_TempObjSource[i];
            var tempValue1 = DualSelect_TempObjSource[i + 1];
            DualSelect_TempObjSource.remove(i + 1);
            DualSelect_TempObjSource.remove(i);
            DualSelect_TempObjSource.options.add(tempValue1, i);
            DualSelect_TempObjSource.options.add(tempValue, i+1);
            i++;
        }
    }

    DualSelect_SaveSelection(dualSelectId);
}

function DualSelect_MoveAll(dualSelectId, bDoSelected) {
    DualSelect_SetObjects(dualSelectId, bDoSelected);

    while (DualSelect_TempObjSource.length > 0) {
        oNode = DualSelect_TempObjSource[0];

        DualSelect_TempObjSource.remove(DualSelect_TempObjSource[0]);
        DualSelect_TempObjDestination.options.add(oNode,
			DualSelect_GetIndexForInsert(DualSelect_TempObjDestination, oNode));
    }

    DualSelect_SaveSelection(dualSelectId);
}



function DualSelect_ClearSelection(oSelect) {
    for (var i = 0; i < oSelect.length; i++)
        oSelect[i].selected = false;
}



function DualSelect_SaveSelection(dualSelectId) {
    var oSelect = document.getElementById(
		dualSelectId + DualSelect_SelectSelected);
    var sValues = "";
    var sTexts = "";

    for (var i = 0; i < oSelect.length; i++) {
        sValues += oSelect[i].value + DualSelect_Separator;
        
    }

    document.getElementById(
		dualSelectId + DualSelect_HiddenSelectedItemsVal).value = sValues;

}

// DATE AND TIME FUNCTIONS

var defaultYear = 1970 + 0;
var defaultMonth = 0+0;
var defaultDay = 1+0;
var defaultHour = 0 + 0;
var defaultMinute = 0 + 0;
var defaultSecond = 0 + 0;


function DateInput_Initialize(id) 
{
    if (document.getElementById(id + "_Year") != null) 
    {
        document.getElementById(id + "_Year").onkeypress = DateInputYearKeyVerify;
        document.getElementById(id + "_Year").onpaste = DateInputYearHandlePaste;
        document.getElementById(id + "_Year").ondrop = DateInputYearHandlePaste;
        document.getElementById(id + "_Year").onchange = DateInputChanged;
    }
    if (document.getElementById(id + "_Month") != null)
        document.getElementById(id + "_Month").onchange = DateInputChanged;

    if (document.getElementById(id + "_Day") != null)
        document.getElementById(id + "_Day").onchange = DateInputChanged;

    if (document.getElementById(id + "_Hours") != null)
        document.getElementById(id + "_Hours").onchange = DateInputChanged;

    if (document.getElementById(id + "_Minutes") != null)
        document.getElementById(id + "_Minutes").onchange = DateInputChanged;

    if (document.getElementById(id + "_Seconds") != null)
        document.getElementById(id + "_Seconds").onchange = DateInputChanged;

    if (eval(id + "_DateInCalendar")) {
        var options = eval(id + "_CalendarOptions");
        
        $('#' + id + '_Calendar').datepicker(options);
        DateInputChanged(null, id, true);
        
        
    }

    

}

function DateInputGetNumDays(M, curYear) {
    M = M + 1;
    if (curYear % 4 == 0) {
        return (M == 9 || M == 4 || M == 6 || M == 11) ? 30 : (M == 2) ? 29 : 31;
    } else {
        return (M == 9 || M == 4 || M == 6 || M == 11) ? 30 : (M == 2) ? 28 : 31;
    }
}

function DateTimeAdjustYears(cmbInput, min, max) {
    if (cmbInput == null) return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    var oldVar = cmbInput.value;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        if (i < 10)
            cmbInput.options[j] = new Option("   " + i, i);
        else if (i < 100)
            cmbInput.options[j] = new Option("  " + i, i);
        else if (i < 1000)
            cmbInput.options[j] = new Option(" " + i, i);
        else
            cmbInput.options[j] = new Option("" + i, i);
        j++;
    }
    MvcControlsToolKit_SetDateElement(cmbInput.id, oldVar);
}

function DateTimeAdjustMonthes(cmbInput, min, max) {
    if (cmbInput == null) return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    var oldVar = cmbInput.value;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        cmbInput.options[j] = new Option(DateTimeMonthes[i], i + 1);
        j++;
    }
    MvcControlsToolKit_SetDateElement(cmbInput.id, oldVar);
}

function DateTimeAdjustDays(cmbInput, min, max) {
    if (cmbInput == null) return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    var oldVar = cmbInput.value;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        if (i < 10)
            cmbInput.options[j] = new Option(" " + i, i);
        else
            cmbInput.options[j] = new Option("" + i, i);
        j++;
    }
    MvcControlsToolKit_SetDateElement(cmbInput.id, oldVar);
}
function DateTimeAdjustTimeElement(cmbInput, min, max) {
    if (cmbInput == null) return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    var oldVar = cmbInput.value;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        if (i < 10)
            cmbInput.options[j] = new Option("0" + i, i);
        else
            cmbInput.options[j] = new Option("" + i, i);
        j++;
    }
    MvcControlsToolKit_SetDateElement(cmbInput.id, oldVar);
}

function DateInputYearHandlePaste(evt) {

    evt = (evt) ? (evt) : ((window.event) ? (window.event) : null);
    if (evt == null) return true;

    var sorg = (evt.target) ? (evt.target) : ((event.srcElement) ? (event.srcElement) : null);
    if (sorg == null) return true;

    var val;
    if (evt.type == "paste")
        val = window.clipboardData.getData("Text");
    else if (evt.type == "drop")
        val = evt.dataTransfer.getData("Text");
    else
        return true;


    for (i = 0; i < val.length; i++) {
        keyCode = val.charCodeAt(i);

        if (keyCode == 13 || keyCode == 8)
            continue;

        if ((keyCode >= 48) && (keyCode <= 57))
            continue;
        else
            return false;

    }
    sorg.value = val;
    return false;
}

function DateInputYearKeyVerify(evt) {
    evt = (evt) ? (evt) : ((window.event) ? (window.event) : null);
    if (evt == null) return true;

    var sorg = (evt.target) ? (evt.target) : ((event.srcElement) ? (event.srcElement) : null);
    if (sorg == null) return true;

    var keyCode = ((evt.charCode || evt.initEvent) ? evt.charCode : ((evt.which) ? evt.which : evt.keyCode));


    if (keyCode == 0 || keyCode == 13 || keyCode == 8)
        return true;
    if ((keyCode >= 48) && (keyCode <= 57))
        return true;
    return false;
    var val = sorg.value;
}

function DateTimeInput_UpdateCalendar(clientId) {
    Nanno = document.getElementById(clientId + "_Year").value;
    Nmese = document.getElementById(clientId + "_Month").value;
    Ngiorno = document.getElementById(clientId + "_Day").value;

    var newDate = new Date(Nanno, Nmese - 1, Ngiorno);
    var dateHost = $('#' + clientId + "_Calendar");
    var format = dateHost.datepicker("option", "dateFormat");
    if (format == null) format = 'mm/dd/yy';
    
    dateHost.datepicker( "setDate" , $.datepicker.formatDate(format, newDate));

}

function DateTimeInput_UpdateFromCalendar(stringDate, clientId) {
    var dateHost = $('#' + clientId + "_Calendar");

    
    if (stringDate == null) return;

    var format = dateHost.datepicker("option", "dateFormat");
    if (format == null) format = 'mm/dd/yy';
    var date = null;
    try{
        date = $.datepicker.parseDate(format, stringDate);
    }
    catch (e){
        date = new Date();
    }

    var stringDateMin = dateHost.datepicker("option", "minDate");
    var stringDateMax = dateHost.datepicker("option", "maxDate");

    var dateMin = null;
    var dateMax = null;

    if (stringDateMin != null) dateMin = $.datepicker.parseDate(format, stringDateMin);
    if (stringDateMax != null) dateMax = $.datepicker.parseDate(format, stringDateMax);
    
    if (dateMin != null && date < dateMin) {
        date = dateMin;
    }
    if (dateMax != null && date > dateMax) {
        date = dateMax;
    }
    

    document.getElementById(clientId + "_Year").value = date.getFullYear();
    document.getElementById(clientId + "_Month").value = date.getMonth() + 1;
    document.getElementById(clientId + "_Day").value = date.getDate();

    DateInputChanged(null, clientId, true);
    


}

function DateTimeInput_UpdateCalendarMinMax(clientId, minDate, maxDate) {
    var dateHost = $('#' + clientId + "_Calendar");
    var format = dateHost.datepicker("option", "dateFormat");
    if (format == null) format = 'mm/dd/yy';

    if (minDate != null) {
        dateHost.datepicker("option", "minDate", $.datepicker.formatDate(format, minDate));
    }
    else {
        dateHost.datepicker("option", "minDate", null);
    }
    if (maxDate != null) {
        dateHost.datepicker("option", "maxDate", $.datepicker.formatDate(format, maxDate));
    }
    else {
        dateHost.datepicker("option", "maxDate", null);
    }
}

function DateInputChanged(evt, cid, update) {

    var clientID;
    if (cid == null) {


        evt = (evt) ? (evt) : ((window.event) ? (window.event) : null);
        if (evt == null) {

            return false;
        }

        var sorg = (evt.target) ? (evt.target) : ((event.srcElement) ? (event.srcElement) : null);
        if (sorg == null) {

            return false;
        }
        clientID = sorg.id.substring(0, sorg.id.lastIndexOf("_"));
    }
    else {
        clientID = cid;
    }
    if (eval(clientID + "Recursive") == true) return;
    eval(clientID + "Recursive = true;");


    var Nanno;
    var Nmese;
    var Ngiorno;
    var NHours;
    var NMinutes;
    var NSeconds;
    var CurrDate = eval(clientID + "_Curr");
    var CurrDay = CurrDate.getDate();
    var CurrMonth = CurrDate.getMonth();
    var CurrYear = CurrDate.getFullYear();
    var CurrHours = CurrDate.getHours();
    var CurrMinutes = CurrDate.getMinutes();
    var CurrSeconds = CurrDate.getSeconds();

    var currMin = eval(clientID + "_MinDate");
    var currMax = eval(clientID + "_MaxDate");

    var dynamicMin = null;
    if (eval("(typeof " + clientID + "_ClientDynamicMin !== 'undefined') && (" + clientID + "_ClientDynamicMin != null)") == true) dynamicMin = eval(clientID + "_ClientDynamicMin()");

    var dynamicMax = null;
    if (eval("(typeof " + clientID + "_ClientDynamicMax !== 'undefined') && (" + clientID + "_ClientDynamicMax != null)") == true) dynamicMax = eval(clientID + "_ClientDynamicMax()");

    if (dynamicMin != null && (currMin == null || dynamicMin > currMin)) {
        if (dynamicMin > currMax)
            currMin = currMax;
        else
            currMin = dynamicMin;
    }
    if (dynamicMax != null && (currMax == null || dynamicMax < currMax)) {
        if (dynamicMax < currMin)
            currMax = currMin;
        else
            currMax = dynamicMax;
    }

    if (document.getElementById(clientID + "_Year") != null) {
        Nanno = document.getElementById(clientID + "_Year").value;
    }
    else {
        Nanno = CurrYear;
    }

    if (document.getElementById(clientID + "_Month") != null)
        Nmese = document.getElementById(clientID + "_Month").value;
    else
        Nmese = CurrMonth;

    if (document.getElementById(clientID + "_Day") != null)
        Ngiorno = document.getElementById(clientID + "_Day").value;
    else
        Ngiorno = CurrDay;

    if (document.getElementById(clientID + "_Hours") != null)
        NHours = document.getElementById(clientID + "_Hours").value;
    else
        NHours = CurrHours;

    if (document.getElementById(clientID + "_Minutes") != null)
        NMinutes = document.getElementById(clientID + "_Minutes").value;
    else
        NMinutes = CurrMinutes;

    if (document.getElementById(clientID + "_Seconds") != null)
        NSeconds = document.getElementById(clientID + "_Seconds").value;
    else
        NSeconds = CurrSeconds;

    var TempNewDate = new Date(Nanno, Nmese-1, Ngiorno, NHours, NMinutes, NSeconds);

    if (currMax != null && currMax < TempNewDate) TempNewDate = currMax;
    if (currMin != null && currMin > TempNewDate) TempNewDate = currMin;

    Nanno = TempNewDate.getFullYear()+"";
    Nmese = (TempNewDate.getMonth()+1)+"";
    Ngiorno = TempNewDate.getDate()+"";
    NHours = TempNewDate.getHours()+"";
    NMinutes = TempNewDate.getMinutes()+"";
    NSeconds = TempNewDate.getSeconds()+"";

    var NewYear;
    var NewMonth;
    var NewDay;
    var NewHours;
    var NewMinutes;
    var NewSeconds;
    var MaxYear;
    var MinYear;
    var MaxMonth;
    var MinMonth;
    var MinDay;
    var MaxDay;
    var MinHours;
    var MaxHours;
    var MinMinutes;
    var MaxMinutes;
    var MinSeconds;
    var MaxSeconds;
    eval(clientID + "_Valid = true");

    
    //year processing
    NewYear = parseInt(Nanno);
    if (!isNaN(NewYear)) {
        if  (currMax == null) {
            MaxYear = null;
        }
        else {
            MaxYear = currMax.getFullYear();
        }
        if (currMin == null) {
            MinYear = null;
        }
        else {
            MinYear = currMin.getFullYear();
        }
        if (MaxYear != null && MaxYear < NewYear) NewYear = MaxYear;
        if (MinYear != null && MinYear > NewYear) NewYear = MinYear;
        if (document.getElementById(clientID + "_Year") != null && !eval(clientID + "_DateHidden") && eval(clientID + "_YearCombo") && !eval(clientID + "_DateInCalendar"))
            DateTimeAdjustYears(document.getElementById(clientID + "_Year"), MinYear, MaxYear);
        
        if ((MaxYear == null || MaxYear >= NewYear) && (MinYear == null || MinYear <= NewYear)) {

            //Month Processing
            MaxMonth = 11;
            MinMonth = 0;
            if (MaxYear == NewYear) {
                MaxMonth = currMax.getMonth();
            }
            if (MinYear == NewYear) {
                MinMonth = currMin.getMonth();
            }
            NewMonth = parseInt(Nmese);
            if (!isNaN(NewMonth)) {
                NewMonth = NewMonth - 1;
                if (MinMonth > NewMonth) {
                    NewMonth = MinMonth;
                }
                if (MaxMonth < NewMonth) {
                    NewMonth = MaxMonth;
                }
                if (CurrYear == MinYear || CurrYear == MaxYear || NewYear == MinYear || NewYear == MaxYear)
                    if (document.getElementById(clientID + "_Month") != null && !eval(clientID + "_DateHidden") && !eval(clientID + "_DateInCalendar"))
                        DateTimeAdjustMonthes(document.getElementById(clientID + "_Month"), MinMonth, MaxMonth);
                //day processing
                MinDay = 1;
                MaxDay = DateInputGetNumDays(NewMonth, NewYear);
                if (MaxYear == NewYear && MaxMonth == NewMonth) {
                    MaxDay = currMax.getDate();

                }
                if (MinYear == NewYear && MinMonth == NewMonth) {
                    MinDay = currMin.getDate();
                }
                NewDay = parseInt(Ngiorno);
                if (!isNaN(NewDay)) {
                    if (MinDay > NewDay) {
                        NewDay = MinDay;
                    }
                    if (MaxDay < NewDay) {
                        NewDay = MaxDay;
                    }
                    if (document.getElementById(clientID + "_Day") != null && !eval(clientID + "_DateHidden") && !eval(clientID + "_DateInCalendar"))
                        DateTimeAdjustDays(document.getElementById(clientID + "_Day"), MinDay, MaxDay);
                    // Hours Processing
                    MinHours = 0;
                    MaxHours = 23;
                    if (MaxYear == NewYear && MaxMonth == NewMonth && NewDay == MaxDay) {
                        MaxHours = currMax.getHours();
                    }
                    if (MinYear == NewYear && MinMonth == NewMonth && NewDay == MinDay) {
                        MinHours = currMin.getHours();
                    }
                    NewHours = parseInt(NHours);
                    if (!isNaN(NewHours)) {
                        if (MaxHours < NewHours) NewHours = MaxHours;
                        if (NewHours < MinHours) NewHours = MinHours;
                        if (document.getElementById(clientID + "_Hours") != null)
                            DateTimeAdjustTimeElement(document.getElementById(clientID + "_Hours"), MinHours, MaxHours);
                        // Minutes Processing
                        MinMinutes = 0;
                        MaxMinutes = 59;
                        if (MaxYear == NewYear && MaxMonth == NewMonth && NewDay == MaxDay && MaxHours == NewHours)
                            MaxMinutes = currMax.getMinutes();
                        if (MinYear == NewYear && MinMonth == NewMonth && NewDay == MinDay && MinHours == NewHours)
                            MinMinutes = currMin.getMinutes();
                        NewMinutes = parseInt(NMinutes);
                        if (!isNaN(NewMinutes)) {
                            if (MaxMinutes < NewMinutes) NewMinutes = MaxMinutes;
                            if (MinMinutes > NewMinutes) NewMinutes = MinMinutes;
                            if (document.getElementById(clientID + "_Minutes") != null)
                                DateTimeAdjustTimeElement(document.getElementById(clientID + "_Minutes"), MinMinutes, MaxMinutes);
                            // Seconds Processing
                            MinSeconds = 0;
                            MaxSeconds = 59;
                            if (MaxYear == NewYear && MaxMonth == NewMonth && NewDay == MaxDay && MaxHours == NewHours && MaxMinutes == NewMinutes)
                                MaxSeconds = currMax.getSeconds();
                            if (MinYear == NewYear && MinMonth == NewMonth && NewDay == MinDay && MinHours == NewHours && MinMinutes == NewMinutes)
                                MinSeconds = currMin.getSeconds();
                            NewSeconds = parseInt(NSeconds);
                            if (!isNaN(NewSeconds)) {
                                if (MaxSeconds < NewSeconds) NewSeconds = MaxSeconds;
                                if (NewSeconds < MinSeconds) NewSeconds = MinSeconds;
                                if (document.getElementById(clientID + "_Seconds") != null)
                                    DateTimeAdjustTimeElement(document.getElementById(clientID + "_Seconds"), MinSeconds, MaxSeconds);
                            }
                            else {
                                eval(clientID + "_Valid = false");
                            }
                        }

                        else {
                            eval(clientID + "_Valid = false");
                        }
                    }
                    else {
                        eval(clientID + "_Valid = false");
                    }
                }
                else {
                    eval(clientID + "_Valid = false");
                }
            }
            else {
                eval(clientID + "_Valid = false");
            }
        }
    }
    else {
        eval(clientID + "_Valid = false");
    }
    if (eval(clientID + "_DateInCalendar")) {
        DateTimeInput_UpdateCalendarMinMax(
        clientID,
        MinYear == null ? null: new Date(MinYear, MinMonth, MinDay),
        MaxYear == null ? null: new Date(MaxYear, MaxMonth, MaxDay));
    }
    var AChange = false;
    if (eval(clientID + "_Valid")) {
        if (update == true || (cid == null  && 
            (CurrYear != NewYear || CurrMonth != NewMonth || CurrDay != NewDay ||
             CurrHours != NewHours || CurrMinutes != NewMinutes || CurrSeconds != NewSeconds))) 
               AChange = true;
        CurrYear = NewYear;
        CurrMonth = NewMonth;
        CurrDay = NewDay;
        CurrHours = NewHours;
        CurrMinutes = NewMinutes;
        CurrSeconds = NewSeconds;
    }
    if (!AChange) {
        eval(clientID + "Recursive = false;")
        return true;
    }

    eval(clientID + "_Curr = new Date(CurrYear, CurrMonth, CurrDay, CurrHours, CurrMinutes, CurrSeconds)");
    
    if (document.getElementById(clientID + "_Year") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Year", CurrYear);
        
    }
    if (document.getElementById(clientID + "_Month") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Month", CurrMonth + 1);
    }
    if (document.getElementById(clientID + "_Day") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Day", CurrDay);
    }
    if (eval(clientID + "_DateInCalendar")) {
        DateTimeInput_UpdateCalendar(clientID);
    }
    if (document.getElementById(clientID + "_Hours") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Hours", CurrHours);
    }
    if (document.getElementById(clientID + "_Minutes") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Minutes", CurrMinutes);
    }
    if (document.getElementById(clientID + "_Seconds") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Seconds", CurrSeconds);
    }
    
    var currDate = eval(clientID + "_Curr");
    RefreshDependencies(clientID);
    eval(clientID + "_ClientDateChanged(" + currDate.getTime() + ")");
    $("#"+clientID + "_Hidden").trigger("DateTimeInput_Changed");
    eval(clientID + "Recursive = false;");
    return true;
}
function MvcControlsToolKit_SetDateElement(id, value) {
    var element = document.getElementById(id);
    if (element.tagName == 'SELECT') {
        for (var i = element.options.length - 1; i >= 0; i--) {
            if (element.options[i].value <= value) {
                element.selectedIndex = i;
                return;
            }
        }
        element.selectedIndex = 0;
    } else {
        if ((value === null) || (value === undefined))
            value = "";
        element.value = value;
    }
}
function SetDateInput(id, value, cType) {
    if (eval("typeof " + id + "_Curr === 'undefined'") == true) return;
    var currDate = eval(id + "_Curr");
    
    if (currDate == null) return;
    var currDateInMilliseconds = currDate.getTime();

    if (cType == 1 && value >= currDateInMilliseconds) 
    {
        return;
    }
    if (cType == 2 && value <= currDateInMilliseconds) {
       return;
    }
   var DateInFormat = new Date(value);
   var currMin = eval(id + "_MinDate");
   var currMax = eval(id + "_MaxDate");
   if (currMin != null && DateInFormat < currMin) DateInFormat = currMin;
   if (currMax != null && DateInFormat > currMax) DateInFormat = currMax;
   eval(id + "_Curr = DateInFormat;");
    if (document.getElementById(id + "_Hours") != null) {
        if (document.getElementById(id + "_Year") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Year", DateInFormat.getFullYear());
            DateInputChanged(null, id, false);
        }
        if (document.getElementById(id + "_Month") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Month", DateInFormat.getMonth() + 1);
            DateInputChanged(null, id, false);
        }
        if (document.getElementById(id + "_Day") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Day", DateInFormat.getDate());
            DateInputChanged(null, id, false);
        }
        if (document.getElementById(id + "_Hours") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Hours", DateInFormat.getHours());
            DateInputChanged(null, id, false);
        }
        if (document.getElementById(id + "_Minutes") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Minutes", DateInFormat.getMinutes());
            DateInputChanged(null, id, false);
        }
        if (document.getElementById(id + "_Seconds") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Seconds", DateInFormat.getSeconds());
            DateInputChanged(null, id, true);
        }
       }
    else {
            if (document.getElementById(id + "_Year") != null) {
                MvcControlsToolKit_SetDateElement(id + "_Year", DateInFormat.getFullYear());
                DateInputChanged(null, id, false);
            }
            if (document.getElementById(id + "_Month") != null) {
                MvcControlsToolKit_SetDateElement(id + "_Month", DateInFormat.getMonth() + 1);
                DateInputChanged(null, id, false);
            }
            if (document.getElementById(id + "_Day") != null) {
                MvcControlsToolKit_SetDateElement(id + "_Day", DateInFormat.getDate());
                DateInputChanged(null, id, true);
            }
    }
    if (eval(id + "_DateInCalendar")) {
        DateTimeInput_UpdateCalendar(id);
    }
    
}

function GetDateInput(id) {
    return eval(id + "_Curr");
}

function MvcControlsToolkit_DateTimeInput_SetString(sorg, value) {
    clientID = sorg.id.substring(0, sorg.id.lastIndexOf("_"));
    var ob = null;
    if (value == null || value == "") {
        ob = new Date();
    }
    else {
        ob = MvcControlsToolkit_Parse(value, MvcControlsToolkit_DataType_DateTime);
    }
    SetDateInput(clientID, ob.getTime(), 3);
}

function MvcControlsToolkit_DateTimeInput_Set(sorg, value, format, valueType){
    if ($('#' + sorg.id).length == 0) {
        var retry = function () { MvcControlsToolkit_DateTimeInput_Set(sorg, value, format, valueType); };
        setTimeout(retry, 0);
        return;
    }
    clientID = sorg.id.substring(0, sorg.id.lastIndexOf("_"));
    if(value == null || value == "") value = new Date();
    SetDateInput(clientID, value.getTime(), 3);
}

function MvcControlsToolkit_DateTimeInput_Get(sorg, valueType){
    clientID = sorg.id.substring(0, sorg.id.lastIndexOf("_"));
    return eval(clientID + "_Curr");
}
function MvcControlsToolkit_DateTimeInput_GetString(sorg) {
    clientID = sorg.id.substring(0, sorg.id.lastIndexOf("_"));
    return MvcControlsToolkit_Format(GetDateInput(clientID), 's', MvcControlsToolkit_DataType_DateTime, '', '');
}

function AddToUpdateList(id, toAdd) 
{
    if (id == null || toAdd == null) return;
    
    var currIndex = eval(id+"_UpdateListIndex");
    eval(id + "_UpdateList[" + currIndex + "] = '" + toAdd + "';");
    currIndex++;
    eval(id + "_UpdateListIndex = "+currIndex+";");
}

function RefreshDependencies(id) 
{
    if (eval("typeof " + id + "_UpdateListIndex === 'undefined'") == true) return;
    var length = eval(id + "_UpdateListIndex");
    if (length == null) return;
    for (var i = 0; i < length; i++) {
        DateInputChanged(null, eval(id + "_UpdateList[" + i + "]"), true);
    }
}


//////////////////////// DATAGRID /////////////////////////////

var DataButtonCancel = "DataButtonCancel";
var DataButtonDelete = "DataButtonDelete";
var DataButtonUndelete = "DataButtonUndelete";
var DataButtonEdit = "DataButtonEdit";
var DataButtonInsert = "DataButtonInsert";
var DataButtonResetRow = "DataButtonResetRow";
var DisplayPostfix ="_Display";
var EditPostfix = "_Edit";
var OldEditPostfix = "_OldEdit";
var UndeletePostfix = "_Undelete";
var ChangedExternallyPostfix = "_ChangedExternally";
var SavePostFix = "_Save";
var SavePostFixCurr = "_SaveCurr";
var DatagridFielsdPostfix = "_Datagrid_Fields";
var SavePostFixD = "_SaveD";
var SavePostFixU = "_SaveU";
var SavePostFixC = "_SaveC";
var DeletedPostFix = "_Deleted";
var ContainerPostFix = "_Container";
var VarPostfix = "_Var";
var AllNormalPostfix = "_AllNormal";
var ChandedPostfix = "_Changed";

var TemplateVarsPostfix = '_templateVars';
var TemplatePreparePostfix = '_templatePrepare';
var PlaceHolderPostfix = '_placeHolder';
var ChangedHiddenPostfix = '_changedHidden';
var TemplateSymbolPostfix='_templateSymbol';
var LastIndexPostfix='_lastIndex';
var DataGrid_ValidationTypePostfix = '_validationType';
var MinLastIndexPostfix = '_minLastIndex';
var LastVisibleIndexPostfix = '_lastVisibleIndex';
var TemplateEditHtmlPostfix='_editHtml';
var TemplateDisplayHtmlPostfix = '_displayHtml';
var TemplateAllJavascriptPostfix = '_allJavascript';
var MvcControlsToolkit_DatagridCssPostfix = '_Css';
var MvcControlsToolkit_DatagridAltCssPostfix = '_AltCss';
var MvcControlsToolkit_DatagridFatherItemsPostfix = '_FatherItems';

function DataGrid_Field(original, current, validationType) {
    this.Original = original;
    this.Current = current;
    this.ValidationType = validationType;
}
DataGrid_Field.prototype = {
    Original: null,
    Current: null,
    ValidationType: null,
    Reset: function () {
        if (this.Original == null) return;
        if (this.Current == null) return;
        if (this.Original.nodeName.toLowerCase() == "input") {
            if (this.Original.getAttribute('type').toLowerCase() == 'checkbox' ||
                this.Original.getAttribute('type').toLowerCase() == 'radiobutton') {
                var unchanged = this.Current.checked == this.Original.checked;
                this.Current.checked = this.Original.checked;
                if (!unchanged) 
                    $(this.Current).trigger('change');
            }
            else if (this.Original.getAttribute('type').toLowerCase() == 'text' ||
                     this.Original.getAttribute('type').toLowerCase() == 'password') {
                $(this.Current).trigger('focus');
                this.Current.value = this.Original.value;
                $(this.Current).trigger('blur');
            }
            
            else {
                this.Current.value = this.Original.value;
            }
        }
        else if (this.Original.nodeName.toLowerCase() == "textarea") {
            $(this.Current).trigger('focus');
            this.Current.value = this.Original.value;
            $(this.Current).trigger('blur');
        }
        else if (this.Original.nodeName.toLowerCase() == "select") {
            var unchanged = (this.Current.value == this.Original.value);
            this.Current.value = this.Original.value;
            if (!unchanged)  
                $(this.Current).trigger('change');
        }
        MvcControlsToolkit_Validate(this.Current.id, this.ValidationType);
    }
}

function DataGrid_EditRowFields(validationType) {
    this.Names = new Array();
    this.Dictionary = new Array();
    this.ValidationType=validationType;
}

DataGrid_EditRowFields.prototype = {
    Names: null,
    Dictionary: null,
    ValidationType: null,
    AddOriginal: function (fieldName, field) {
        if (typeof this.Dictionary[fieldName] === 'undefined') {
            this.Names.push(fieldName);
            this.Dictionary[fieldName] = new DataGrid_Field(field, null, this.ValidationType);
        }
        else {
            this.Dictionary[fieldName].Original = field;
        }
    },
    AddCurrent: function (fieldName, field) {
        if (typeof this.Dictionary[fieldName] === 'undefined') {
            this.Names.push(fieldName);
            this.Dictionary[fieldName] = new DataGrid_Field(null, field, this.ValidationType);
        }
        else {
            this.Dictionary[fieldName].Current = field;
        }
    },
    Reset: function(){
        var fieldName=null;
        for(var i=0; i < this.Names.length; i++){
            fieldName=this.Names[i];
            if (typeof this.Dictionary[fieldName] !== 'undefined') {
                this.Dictionary[fieldName].Reset();
            }
        }
    }
}

function DataGrid_ResetRow(itemRoot) {
    var fields = null;
    var temp = null;
    var validationType=null;
    var root = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    root = root.substring(0, root.lastIndexOf('___'));
    validationType = eval(root+DataGrid_ValidationTypePostfix);
    fields = eval(itemRoot + DatagridFielsdPostfix);
    if (fields == null) {
        fields = new DataGrid_EditRowFields(validationType);
        eval(itemRoot + DatagridFielsdPostfix + " = fields;");
        
        temp = eval(itemRoot + SavePostFix);
        temp.find('input').each(function (i) {
            fields.AddOriginal(this.id, this);
        });
        temp.find('textarea').each(function (i) {
            fields.AddOriginal(this.id, this);
        });
        temp.find('select').each(function (i) {
            fields.AddOriginal(this.id, this);
        });
        temp = eval(itemRoot + SavePostFixCurr);
        temp.find('input').each(function (i) {
            fields.AddCurrent(this.id, this);
        });
        temp.find('textarea').each(function (i) {
            fields.AddCurrent(this.id, this);
        });
        temp.find('select').each(function (i) {
            fields.AddCurrent(this.id, this);
        });

        eval(itemRoot + SavePostFix+ " = null;");
    }
    fields.Reset();
}
function DataGrid_Remove_Edit_Item(itemRoot) {
    var temp = null;
    temp = $('#' + itemRoot + EditPostfix + ContainerPostFix).detach();
    if (temp.length != 0) {
        eval(itemRoot + SavePostFixCurr + " = temp;");
    }
}

function DataGrid_Display_Edit_Item(itemRoot) {
    var temp = null;
    temp =  eval(itemRoot + SavePostFixCurr);   
    $('#' + itemRoot + DisplayPostfix + ContainerPostFix).before(temp);
    DataGrid_ResetRow(itemRoot);
}

function DataGrid_ItemRoot_AtIndex(itemRoot, index) {
    var right = itemRoot.substring(itemRoot.lastIndexOf('___'));
    var left = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    left = left.substring(0, left.lastIndexOf('_') + 1);
    return left + index + right;
}
function DataGrid_ItemRoot_Index(itemRoot) {
    itemRoot = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    itemRoot = itemRoot.substring(itemRoot.lastIndexOf('_') + 1);
    return parseInt(itemRoot);
}

function DataGrid_ChecKInsertNewItem(itemRoot) {
    var root = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    root = root.substring(0, root.lastIndexOf('___'));
    if (eval("typeof " + root + LastIndexPostfix + " === 'undefined'")) return;
    var index = DataGrid_ItemRoot_Index(itemRoot);
    var visibleIndex = eval(root + LastVisibleIndexPostfix);
    if (index != visibleIndex) return;

    var lastIndex = eval(root + LastIndexPostfix);
    
    if (lastIndex == index) {
        index++;
        var indexStr = index + '';
        var templateSymbol = eval(root + TemplateSymbolPostfix);
        var displayTemplate = eval(root + TemplateDisplayHtmlPostfix).replace(templateSymbol, indexStr);
        var editTemplate = eval(root + TemplateEditHtmlPostfix).replace(templateSymbol, indexStr);
        var changed = $('<div>').append($('#' + eval(root + ChangedHiddenPostfix)).clone()).remove().html().replace(templateSymbol, indexStr);
        var placeHolderName = eval(root + PlaceHolderPostfix);
        var placeHolder = $('<div>').append($('#' + placeHolderName).clone()).remove().html().replace(templateSymbol, indexStr);

        var tableRoot = $('#' + itemRoot + EditPostfix + ContainerPostFix).parent();
        $(displayTemplate).appendTo(tableRoot);
        $(editTemplate).appendTo(tableRoot);
        
        
        

        jQuery.globalEval(eval(root + TemplateAllJavascriptPostfix).replace(templateSymbol, indexStr));

        var hiddenElementsFather = $('#' + placeHolderName).parent();

        hiddenElementsFather.append(placeHolder);
        hiddenElementsFather.append(changed);
        
        var newItemName = DataGrid_ItemRoot_AtIndex(itemRoot, index) + EditPostfix + ContainerPostFix;
        if (typeof $ !== 'undefined' && $ !== null && typeof $.validator !== 'undefined' && $.validator !== null && typeof $.validator.unobtrusive !== 'undefined' && $.validator.unobtrusive !== null) {

            jQuery.validator.unobtrusive.parseExt('#' + newItemName);
        }

        var initVars = eval(root + TemplateVarsPostfix).replace(templateSymbol, indexStr);
        jQuery.globalEval(initVars);
        var initCall = eval(root + TemplatePreparePostfix).replace(templateSymbol, indexStr);
        jQuery.globalEval(initCall);

        

        visibleIndex++;
        lastIndex++;
        eval(root + LastVisibleIndexPostfix + ' = visibleIndex;');
        eval(root + LastIndexPostfix + ' = lastIndex;');
    }
    else {
        index++;
        var nextItem = DataGrid_ItemRoot_AtIndex(itemRoot, index);
        var temp = eval(nextItem + SavePostFixD);
        $('#' + nextItem + DisplayPostfix + ContainerPostFix).replaceWith(temp.clone(true));
        eval(root + LastVisibleIndexPostfix + ' = index;');
    }

}

function DataGrid_ChecKDisappearItem(itemRoot) {
    var root = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    root = root.substring(0, root.lastIndexOf('___'));
    if (eval("typeof  " + root + LastIndexPostfix + " === 'undefined'")) return;

    var index = DataGrid_ItemRoot_Index(itemRoot);
    var visibleIndex = eval(root + LastVisibleIndexPostfix);
    var minIndex = eval(root + MinLastIndexPostfix);

    if (index < minIndex) return;
    if (index != visibleIndex) {
        if (index == visibleIndex - 1) {
            index++;
            itemRoot = DataGrid_ItemRoot_AtIndex(itemRoot, index);
            DataGrid_ChecKDisappearItem(itemRoot);
            return;
            
        }
        else {
            return;
        }
    }
    if (index <= minIndex) return;
    var display = $('#' + itemRoot + DisplayPostfix + ContainerPostFix);
    if (display.length == 0) return;
    var prevItem = DataGrid_ItemRoot_AtIndex(itemRoot, index - 1);
    if ($('#' + prevItem + DisplayPostfix + ContainerPostFix).length == 0) return;
    display.css('display', 'none');
    visibleIndex--;
    eval(root + LastVisibleIndexPostfix+' = visibleIndex');
    index--;
    var prevtItem = DataGrid_ItemRoot_AtIndex(itemRoot, index);
    DataGrid_ChecKDisappearItem(prevItem, root);

}

function DataButton_Click(itemRoot, itemChanged, dataButtonType) {
    if (dataButtonType == DataButtonDelete) {
        var undel = eval(itemRoot + SavePostFixU);
        if (undel != null) {
            $('#' + itemRoot + DisplayPostfix + ContainerPostFix).before(undel.clone(true));
            $('#' + itemRoot + DisplayPostfix + ContainerPostFix).remove();
        }
        else {
            $('#' + itemRoot + DisplayPostfix + ContainerPostFix).css('display', 'none');
        }
        DataGrid_Remove_Edit_Item(itemRoot);

        $('#' + itemChanged).val('True');
        eval(itemRoot + DeletedPostFix + " = true;");
    }
    else if (dataButtonType == DataButtonEdit || dataButtonType == DataButtonInsert) {
        DataGrid_Display_Edit_Item(itemRoot);
        $('#' + itemRoot + DisplayPostfix + ContainerPostFix).remove();
        $('#' + itemChanged).val('True');
        DataGrid_ChecKInsertNewItem(itemRoot);

    }
    else if (dataButtonType == DataButtonCancel) {
        var temp = eval(itemRoot + SavePostFixD);
        $('#' + itemRoot + EditPostfix + ContainerPostFix).before(temp.clone(true));
        DataGrid_Remove_Edit_Item(itemRoot);
        $('#' + itemChanged).val('False');
        DataGrid_ChecKDisappearItem(itemRoot);
    }
    else if (dataButtonType == DataButtonResetRow) {
        var temp = eval(itemRoot + SavePostFixD);
        $('#' + itemRoot + EditPostfix + ContainerPostFix).before(temp.clone(true));
        DataGrid_Remove_Edit_Item(itemRoot);
        DataGrid_Display_Edit_Item(itemRoot);
        $('#' + itemRoot + DisplayPostfix + ContainerPostFix).remove();
    }
    else if (dataButtonType == DataButtonUndelete) {
        var undel = eval(itemRoot + SavePostFixU);
        var temp = eval(itemRoot + SavePostFixD);
        if (undel != null) {
            
            $('#' + itemRoot + UndeletePostfix + ContainerPostFix).before(temp.clone(true));
            $('#' + itemRoot + UndeletePostfix + ContainerPostFix).remove();
        }
        else {
            $('#' + itemRoot + DisplayPostfix+ ContainerPostFix).replaceWith(temp.clone(true));
        }
        eval(itemRoot + DeletedPostFix + " = false;");
        $('#' + itemChanged).val('False');
    }
    MvcControlsToolkit_DataGridApplyStylesItem(itemRoot);

}



function DataGrid_Prepare_Template(itemRoot, itemChanged, deleted, root) 
    {
        var allJavascript = CollectAllScriptsInelement(itemRoot + EditPostfix + ContainerPostFix) + CollectAllScriptsInelement(itemRoot + DisplayPostfix + ContainerPostFix);
        eval(root + TemplateAllJavascriptPostfix + ' = allJavascript;');
        var editTemplateElement=$('#' + itemRoot + EditPostfix + ContainerPostFix);
        var displayTemplateElement = $('#' + itemRoot + DisplayPostfix + ContainerPostFix);
        editTemplateElement.find('script').remove();
        displayTemplateElement.find('script').remove();

        var temp = null;
        if (editTemplateElement.hasClass("MVCCT_EncodedTemplate")) {
            temp = editTemplateElement.text();
        }
        else {
            temp = $('<div>').append(editTemplateElement.clone()).remove().html();
        }
        eval(root + TemplateEditHtmlPostfix + ' = temp;');
        if (displayTemplateElement.hasClass("MVCCT_EncodedTemplate")) {
            temp = displayTemplateElement.text();
        }
        else {
            temp = $('<div>').append(displayTemplateElement.clone()).remove().html();

        } 
        eval(root + TemplateDisplayHtmlPostfix + ' = temp;');



        editTemplateElement.remove();
        displayTemplateElement.remove();
    }

function MvcControlsToolkit_DataGridApplyStylesItem(itemRoot){
    var root = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    root = root.substring(0, root.lastIndexOf('___'));
    MvcControlsToolkit_DataGridApplyStyles(root);
}

function MvcControlsToolkit_DataGridApplyStyles(rootName){
    var root = eval(rootName+MvcControlsToolkit_DatagridFatherItemsPostfix);
    if (root == null) return;
    var css = eval (rootName + MvcControlsToolkit_DatagridCssPostfix);
    var altCss = eval(rootName + MvcControlsToolkit_DatagridAltCssPostfix);
    var alt = false;
     for (i = 0; i < root.childNodes.length; i++) {
         var nodeId = root.childNodes[i].id;
         if (nodeId == null) continue;
        var end_prefix = nodeId.lastIndexOf("_");
        if (end_prefix < 0) continue;
        var ending = nodeId.substring(end_prefix );
        if (ending != ContainerPostFix) continue;
        if ($(root.childNodes[i]).css('display') == 'none') continue;
        if (alt) {
            if (css != '') $(root.childNodes[i]).removeClass(css);
            if (altCss != '') $(root.childNodes[i]).addClass(altCss);
        }
        else {
            if (altCss != '') $(root.childNodes[i]).removeClass(altCss);
            if (css != '') $(root.childNodes[i]).addClass(css);
        }
        alt=!alt;
    }
}

function DataGrid_Prepare_Item(itemRoot, itemChanged, deleted, root) {
    var temp = eval(root + AllNormalPostfix);
    if (temp == null) {
        temp = new Array();
    }
    temp.push(itemRoot);
    eval(root + AllNormalPostfix + " = temp");
    temp = $('#' + itemRoot + OldEditPostfix + ContainerPostFix).find('script').remove();
    temp=$('#' + itemRoot + OldEditPostfix + ContainerPostFix).detach();
    $('#' + itemRoot + EditPostfix + ContainerPostFix).find('script').remove();
    if (temp.length == 0)
        temp = $('#' + itemRoot + EditPostfix + ContainerPostFix).clone(true);

    eval(itemRoot + SavePostFix + " = temp;");

    temp = $('#' + itemRoot + DisplayPostfix + ContainerPostFix);
    temp.find('script').remove();
    var parent = temp.parent();
    if (parent.length > 0) {
        parent=parent[0];
        eval(root+MvcControlsToolkit_DatagridFatherItemsPostfix+" = parent;")
    }
    temp=temp.clone(true);
    eval(itemRoot + SavePostFixD + " = temp;");

    temp = $('#' + itemRoot + ChangedExternallyPostfix + ContainerPostFix).find('script').remove();
    temp=temp.clone(true);

    if (temp != null && temp.size() == 0)
        temp = null;

    eval(itemRoot + SavePostFixC + " = temp;");

    $('#' + itemRoot + ChangedExternallyPostfix + ContainerPostFix).remove();

    temp = $('#' + itemRoot + UndeletePostfix + ContainerPostFix).find('script').remove();
    temp=temp.clone(true)

    if (temp != null && temp.size() == 0)
        temp = null;

    eval(itemRoot + SavePostFixU + " = temp;");

    if (deleted) {
        DataGrid_Remove_Edit_Item(itemRoot);
        if (temp == null)
            $('#' + itemRoot + DisplayPostfix + ContainerPostFix).css('display', 'none');
        else {
            $('#' + itemRoot + DisplayPostfix + ContainerPostFix).remove();
        }
        eval(itemRoot + DeletedPostFix + " = true;");
    }
    else {
        $('#' + itemRoot + UndeletePostfix + ContainerPostFix).remove();

        if (eval(itemChanged + VarPostfix)) {
            DataGrid_Remove_Edit_Item(itemRoot);
        }
        else {
            $('#' + itemRoot + DisplayPostfix + ContainerPostFix).remove();
        }
        eval(itemRoot + DeletedPostFix + " = false;");
    }
}
////////////////////////////////////SORTABLE and PERMUTATIONS////////////////////////////////
var SortableList_PermutationUpdateRootPrefix = '_PermutationUpdateRoot';
var SortableList_CanSortPrefix = '_CanSort';
var SortableList_ElementsNumberPrefix = '_ElementsNumber';
var SortableList_TemplateSymbolPrefix = '_TemplateSymbol';
var SortableList_TemplateSriptPrefix = '_TemplateSript';
var SortableList_TemplateHtmlPrefix = '_TemplateHtml';
var SortableList_PermutationPrefix = '_Permutation';
var SortableList_PermutationNamePrefix = '.Permutation';
var SortableList_UpdateModelPrefix = '___';
var SortableList_UpdateModelNamePrefix = '.$$';
var SortableList_UpdateModelFieldsPrefix = '_f_ields';
var SortableList_UpdateModelFieldsNamePrefix = '.f$ields';
var SortableList_ItemsContainerPrefix = '_ItemsContainer';
var SortableList_OriginalElementsNumber = '_OriginalElementsNumber';
var SortableList_TemplateHiddenPrefix = '_TemplateHidden';
var SortableList_TemplateHiddenHtmlPrefix = '_TemplateHiddenHtml';
var SortableList_NamePrefixPrefix = '_NamePrefix';
var SortableList_CssPostFix = "_Css";
var SortableList_AltCssPostFix = "_AltCss";

function MvcControlsToolkit_SortableList_PrepareTemplate(root, templateId) {
    MvcControlsToolkit_SortableList_PrepareTemplates(root, [templateId]);
}
function MvcControlsToolkit_SortableList_PrepareTemplates(root, templatesId) {
    eval(root + SortableList_TemplateSriptPrefix + ' = new Array();');
    eval(root + SortableList_TemplateHtmlPrefix + ' = new Array();');
    eval(root + SortableList_TemplateHiddenHtmlPrefix + ' = new Array();');
    for (var i = 0; i < templatesId.length; i++) {
        var templateId = templatesId[i];
        var templateElement = $('#' + templateId);
        
        var allJavascript = CollectAllScriptsInelement(templateId);
        eval(root + SortableList_TemplateSriptPrefix + '[i] = allJavascript;');

        $('#' + templateId).find('script').remove();
        var temp=null;
        if (templateElement.hasClass("MVCCT_EncodedTemplate")) 
        {
            temp = templateElement.text();
        }
        else{
            temp = $('<div>').append(templateElement.clone()).remove().html();
        }
        eval(root + SortableList_TemplateHtmlPrefix + '[i] = temp;');

        var hidden = eval(root + SortableList_TemplateHiddenPrefix);
        if (hidden.constructor == Array) {
            hidden = eval(root + SortableList_TemplateHiddenPrefix + '[i]');
        }
        
        temp = $('<div>').append($('#' + hidden).clone()).remove().html();
        eval(root + SortableList_TemplateHiddenHtmlPrefix + '[i] = temp;');

        $('#' + templateId).remove();
    }

    var canSort = eval(root + SortableList_CanSortPrefix);
    if (canSort){
        var elementNumber = eval(root + SortableList_ElementsNumberPrefix);

        var updateModel = document.getElementById(root + SortableList_UpdateModelPrefix + elementNumber );
        updateModel.setAttribute('id', updateModel.id+'_');


        var updateModelFields = document.getElementById(root + SortableList_UpdateModelPrefix + elementNumber  + SortableList_UpdateModelFieldsPrefix);
        updateModelFields.setAttribute('id', updateModelFields.id+'_'); 
    }
   
}
function MvcControlsToolkit_SortableList_AddNew(root, item) {
    MvcControlsToolkit_SortableList_AddNewChoice(root, 0, item);
}
function MvcControlsToolkit_SortableList_AddNewChoice(root, choice, item) {
    if (eval("typeof  " + root + SortableList_ElementsNumberPrefix + " === 'undefined'")) return;

    var elementNumber = eval(root + SortableList_ElementsNumberPrefix);
    var originalElementNumber = eval(root + SortableList_OriginalElementsNumber);
    var templateSymbol = eval(root + SortableList_TemplateSymbolPrefix);
    if (templateSymbol.constructor == Array) {
        templateSymbol = eval(root + SortableList_TemplateSymbolPrefix+'[choice]');
    }
    var hidden = eval(root + SortableList_TemplateHiddenPrefix);
    if (hidden.constructor == Array) {
        hidden = eval(root + SortableList_TemplateHiddenPrefix + '[choice]');
    }
    var allJavascript = eval(root + SortableList_TemplateSriptPrefix + '[choice]').replace(templateSymbol, elementNumber + '');
    var allHtml = eval(root + SortableList_TemplateHtmlPrefix + '[choice]').replace(templateSymbol, elementNumber + '');
    var hiddenElementFather = $('#' + hidden).parent();
    var hiddenElement = eval(root + SortableList_TemplateHiddenHtmlPrefix + '[choice]').replace(templateSymbol, elementNumber + '');
    var canSort = eval(root + SortableList_CanSortPrefix);
    var namePrefix = eval(root + SortableList_NamePrefixPrefix);

    elementNumber++;
    eval(root + SortableList_ElementsNumberPrefix + ' = elementNumber;');

    if (canSort){
        var permutation = document.getElementById(root + SortableList_PermutationPrefix);
        permutation.setAttribute('name', namePrefix + SortableList_UpdateModelNamePrefix + elementNumber +".$"+ SortableList_PermutationNamePrefix);

        var updateModel = document.getElementById(root + SortableList_UpdateModelPrefix + originalElementNumber + '_');
        updateModel.setAttribute('name', namePrefix + SortableList_UpdateModelNamePrefix + elementNumber);


        var updateModelFields = document.getElementById(root + SortableList_UpdateModelPrefix + originalElementNumber + SortableList_UpdateModelFieldsPrefix + '_');
        updateModelFields.setAttribute('name', namePrefix + SortableList_UpdateModelNamePrefix + elementNumber + SortableList_UpdateModelFieldsNamePrefix);
    }

    hiddenElementFather.append(hiddenElement);
    var result;
    if (item == null) {
        var hasFooter = false;
        var lastChild = $('#' + root + "_Footer");
        if (lastChild.length > 0) {
            hasFooter = true;
        }
        if (hasFooter) {
            result = $(allHtml).insertBefore(lastChild);
        }
        else {
            result = $(allHtml).appendTo('#' + root + SortableList_ItemsContainerPrefix);
        }
    }
    else {
        result = $(allHtml).insertBefore(item);
        MvcControlsToolkit_Bind(item, result[0]);
        $(item).remove();
    }
    
    if (typeof $ !== 'undefined' && $ !== null && typeof $.validator !== 'undefined' && $.validator !== null && typeof $.validator.unobtrusive !== 'undefined' && $.validator.unobtrusive !== null) {
        jQuery.validator.unobtrusive.parseExt('#' + result[0].id)
    }

    jQuery.globalEval(allJavascript);
    Update_Permutations_Root(root);
    if (canSort){
       
       $('#' + root + SortableList_ItemsContainerPrefix).sortable("refresh");
    }
    return result;
}

function Update_Permutations(itemName) {
    var place = itemName.lastIndexOf("___");
    if (place < 0) return;
    var rootName = itemName.substring(0, place);
    place = rootName.lastIndexOf("___");
    rootName = rootName.substring(0, place);
    if (place < 0) return;
    return Update_Permutations_Root(rootName);
}

function Update_Permutations_Root(rootName) {
    var root = document.getElementById(rootName + '_ItemsContainer');
    if (root == null) return;
    if (root.childNodes.length > 0 &&
        root.childNodes[root.childNodes.length - 1].id == "") {
        root = root.childNodes[root.childNodes.length - 1];
    }
    var field = document.getElementById(rootName + '_Permutation');
    var alt = false;
    var css = eval(rootName+SortableList_CssPostFix);
    var altCss = eval(rootName+SortableList_AltCssPostFix);
    for (i = 0; i < root.childNodes.length; i++) {
        var nodeId = root.childNodes[i].getAttribute('id');
        if (nodeId == null) continue;
        var end_prefix = nodeId.lastIndexOf("_");
        if (end_prefix < 0) continue;
        var ending = nodeId.substring(end_prefix + 1);
        if (ending == 'Header' || ending == 'Footer') continue;
        if (alt) {
            if (css != '') $(root.childNodes[i]).removeClass(css);
            if (altCss != '') $(root.childNodes[i]).addClass(altCss);
        }
        else {
            if (altCss != '') $(root.childNodes[i]).removeClass(altCss);
            if (css != '') $(root.childNodes[i]).addClass(css);
        }
        alt=!alt;
    }
    if (field == null) {
        
        return;
    }
    
    var res = '';
    for (i = 0; i < root.childNodes.length; i++) {

        var nodeId = root.childNodes[i].getAttribute('id');
        var end_prefix = nodeId.lastIndexOf("_");
        if (end_prefix < 0) continue;
        var deleteName = nodeId.substring(0, end_prefix + 1) + "Deleted";
        var deletedHidden = document.getElementById(deleteName);
        if (deletedHidden != null && deletedHidden.value == "True") continue;
        var end = nodeId.lastIndexOf("___");
        var order = nodeId.substring(0, end);
        var start = order.lastIndexOf("_") + 1;
        order = order.substring(start);
        if (i > 0) res = res + ',';
        res = res + order;
    }
    field.value = res;
}

///////////////////////////////////MANIPULATION BUTTONS/////////////////////////

var ManipulationButtonRemove = "ManipulationButtonRemove";
var ManipulationButtonHide = "ManipulationButtonHide";
var ManipulationButtonShow = "ManipulationButtonShow";
var ManipulationButtonResetGrid = "ManipulationButtonResetGrid";
var ManipulationButtonCustom = "ManipulationButtonCustom";

function ManipulationButton_Click(target, dataButtonType) {
    if (dataButtonType == ManipulationButtonCustom) {
        eval(target);
        return;
    }
    var end_prefix = target.lastIndexOf("_");
    var deleteName = target.substring(0, end_prefix);
    end_prefix = deleteName.lastIndexOf("_");
    deleteName = deleteName.substring(0, end_prefix + 1) + "Deleted";
    var deletedHidden = document.getElementById(deleteName);
    
    if (dataButtonType == ManipulationButtonRemove) {
        $('#' + target).remove();
        Update_Permutations(target);
    }
    else if (dataButtonType == ManipulationButtonHide) {

        $('#' + target).css('visibility', 'hidden');
        if (deletedHidden != null) deletedHidden.value = "True";
        Update_Permutations(target);
    }
    else if (dataButtonType == ManipulationButtonShow) {

        $('#' + target).css('visibility', 'visible');
        if (deletedHidden != null) deletedHidden.value = "False";
        Update_Permutations(target);
    }
    else if (dataButtonType == ManipulationButtonResetGrid) {
        var toUndo = eval(target + AllNormalPostfix);
        if (toUndo != null) {
            for (var i = 0; i < toUndo.length; i++) {
                var vChanged = toUndo[i].substring(0, toUndo[i].lastIndexOf("_")) + ChandedPostfix;
                var deleted = eval(toUndo[i] + DeletedPostFix);
                if (deleted != null && deleted == true)
                    DataButton_Click(toUndo[i], vChanged, DataButtonUndelete);
                else
                    DataButton_Click(toUndo[i], vChanged, DataButtonCancel);
            }
        }

    }
    

}
function MvcControlsToolkit_Button_AdjustText(buttonName, newText) {
    var button = document.getElementById(buttonName);

    var nodeTag = button.nodeName.toLowerCase();

    if (nodeTag == 'input') button.value = newText;
    else if (nodeTag == 'img') button.setAttribute('src', newText);
    else if (nodeTag == 'a') button.firstChild.nodeValue = newText;

}

///////////////////////////////////TREEVIEWS/////////////////////////
var MvcControlsToolkit_TreeView_ButtonModePostfix = '_ButtonMode';
var MvcControlsToolkit_TreeView_SaveDisplayPostfix = '_SaveDisplay';
var MvcControlsToolkit_TreeView_SaveEditPostfix = '_SaveEdit';
var MvcControlsToolkit_TreeView_ContainerDisplayPostfix = '___Choice1___flattened_ItemsContainer';
var MvcControlsToolkit_TreeView_ContainerEditPostfix = '___Choice2___flattened_ItemsContainer';
var MvcControlsToolkit_TreeView_ToggleEditPostfix = '_ToggleEditButton';
var MvcControlsToolkit_TreeView_IsEditPostFix = '___IsChoice2';
var MvcControlsToolkit_TreeView_RootNamePostfix = "_RootNamePostfix";
var MvcControlsToolkit_TreeView_ClosedPostfix = "___Closed";
var MvcControlsToolkit_TreeView_ItemsCountPostfix = "___ItemsCount";
var MvcControlsToolkit_TreeView_TemplatesPostfix = "_Templates";

function MvcControlsToolkit_TreeView_StartDrag(item, jQueryRoot) {
    var failure = true;
    var currClass = jQueryRoot.attr("class");
    if (currClass != null) {
        var currClass = currClass.split(' ');
        if (currClass != null && currClass.length > 0) {
            currClass = currClass[0];
            if (currClass != null && currClass.length > 0 && currClass.substring(currClass.length - 1) != "_")
                return;
            else {
                currClass = currClass.substring(0, currClass.length - 1);
                var selectedElement = $('.'+currClass);
                if (selectedElement.length > 0) {
                    var res = MvcControlsToolkit_FormContext$_isElementInHierarchy(item[0], selectedElement[0]);
                    if (res) {
                        selectedElement.removeClass(currClass);
                        selectedElement.attr("class", currClass+"__ "+selectedElement.attr("class"));
                    }
                }
            }
        }
    }
    
    jQueryRoot.sortable('option', 'items', '');
    jQueryRoot.sortable('refresh');
    
}
function MvcControlsToolkit_TreeView_StopDrag(item, jQueryRoot) {
    var currClass = jQueryRoot.attr("class");
    if (currClass != null) {
        var currClass = currClass.split(' ');
        if (currClass != null && currClass.length > 0) {
            currClass = currClass[0];
            if (currClass != null && currClass.length > 0 && currClass.substring(currClass.length - 1) == "_")
                currClass = currClass +'_';
            else {
                currClass = currClass + '__';
            }
            var selectedElement = $('.' + currClass);
            if (selectedElement.length > 0) {
                selectedElement.removeClass(currClass);
                currClass = currClass.substring(0, currClass.length - 2);
                selectedElement.attr("class", currClass + ' ' + selectedElement.attr("class"));
            }
        }
    }
    jQueryRoot.sortable('option', 'items', '> *');
    jQueryRoot.sortable('refresh');
}

function MvcControlsToolkit_TreeView_SelectLevel(target, selector)
{
    var root = $(target).parent().find('>ul');
    var currClass = root.attr("class");
    var arr = null;
    if (currClass != null) {
        var arr = currClass.split(' ');
        if (arr!= null && arr.length > 0) {
            currClass = arr[0];
        }
        else {
            arr = new Array();
            arr.push(currClass);
        }
        currClass=arr[0];
    }
    if (target.checked) {
        if (currClass != null && currClass.length > 0 && currClass.substring(currClass.length - 1) == "_")
            currClass=currClass.substring(0, currClass.length - 1);
                    
    }
    else {

        if (currClass != null && currClass.length > 0 && currClass.substring(currClass.length - 1) != "_") 
            currClass=currClass + '_';
    }
    for(var i = 1; i< arr.length; i++){
                currClass=currClass + ' ' + arr[i];
            }
            root.attr("class", currClass);
            if (target.checked) {
                $(selector).each(function () {
                    if (this == target) return;
                    if (!this.checked) return;
                    this.checked = false;
                    MvcControlsToolkit_TreeView_SelectLevel(this, selector);
                });
            }
}

function MvcControlsToolkit_TreeView_PrepareTemplates(root, templatesId) {
    eval(root + SortableList_TemplateSriptPrefix + ' = new Array();');
    eval(root + SortableList_TemplateHtmlPrefix + ' = new Array();');

    for (var i = 0; i < templatesId.length; i++) {
        var templateId = templatesId[i];
        var templateElement = $('#' + templateId);
        var allJavascript = CollectAllScriptsInelement(templateId);
        eval(root + SortableList_TemplateSriptPrefix + '[i] = allJavascript;');

        templateElement.find('script').remove();

        var temp = null;
        if (templateElement.hasClass("MVCCT_EncodedTemplate")) {
            temp = templateElement.text();
        }
        else {
            temp = $('<div>').append(templateElement.children().clone()).remove().html();
        }
        eval(root + SortableList_TemplateHtmlPrefix + '[i] = temp;');

        

        
    }
    $('#' + root + MvcControlsToolkit_TreeView_TemplatesPostfix).remove();
    

}

function MvcControlsToolkit_TreeView_AddNewChoice(rootName, templateChosen) {

    if (eval("typeof  " + rootName + MvcControlsToolkit_TreeView_RootNamePostfix + " === 'undefined'")) return;
    var root = eval(rootName + MvcControlsToolkit_TreeView_RootNamePostfix);
    
    var elementNumber = parseInt(document.getElementById(root+MvcControlsToolkit_TreeView_ItemsCountPostfix).value);

    templateSymbol = eval(root + SortableList_TemplateSymbolPrefix) + templateChosen;



    var allJavascript = eval(root + SortableList_TemplateSriptPrefix + '[templateChosen]').replace(templateSymbol, elementNumber + '');
    var allHtml = eval(root + SortableList_TemplateHtmlPrefix + '[templateChosen]').replace(templateSymbol, elementNumber + '');
    
    var canSort = eval(root + SortableList_CanSortPrefix);
    

    document.getElementById(root+MvcControlsToolkit_TreeView_ItemsCountPostfix).value= (elementNumber+1) +'' ;


    
    var result = $(allHtml).appendTo('#' + rootName + SortableList_ItemsContainerPrefix);

    var initFields = result.find('.MvcCT_init_info_'+root).detach().children();
    initFields.insertAfter('#'+root+SortableList_ItemsContainerPrefix);
    result.find('.level-select_'+root).change(
        function (event) {
            
                MvcControlsToolkit_TreeView_SelectLevel(event.target, '.level-select_'+root);
        }
    );
    jQuery.globalEval(allJavascript);
    if (typeof $ !== 'undefined' && $ !== null && typeof $.validator !== 'undefined' && $.validator !== null && typeof $.validator.unobtrusive !== 'undefined' && $.validator.unobtrusive !== null) {
        jQuery.validator.unobtrusive.parseExt('#' + result[0].id)
    }

    $('#' + root + SortableList_ItemsContainerPrefix).treeview({
        add: result
    });
    MvcControlsToolkit_TreeView_UpdateFather(result[0], rootName + SortableList_ItemsContainerPrefix);
    if (canSort) {

        $('#' + rootName + SortableList_ItemsContainerPrefix).sortable("refresh");
    }
    return result;
}

function MvcControlsToolkit_TreeViewToggle(item)
{
    if (item == null) return;
    var closedId = item.id.substring(0, item.id.lastIndexOf('_')) + MvcControlsToolkit_TreeView_ClosedPostfix;
    var closedStore = document.getElementById(closedId);
    if (closedStore == null) return;
    closedStore.value = $(item).hasClass($.treeview.classes.expandable) ? "True" : "False";
     
}

function MvcControlsToolkit_TreeView_UpdatePermutations(item) {
    if (item == null || item.length==0) return;
    var nodeName = item.attr('id');
    if (nodeName == null) return;
    var place = nodeName.lastIndexOf("_");
    if (place < 0) return;
    nodeName = nodeName.substring(0, place);
    var rootName=document.getElementById(nodeName + '___FatherOriginalId');
    MvcControlsToolkit_TreeView_UpdateFather(item[0], rootName.value.replace(/[\$\.]/g, '_') + SortableList_ItemsContainerPrefix);
    MvcControlsToolkit_TreeView_UpdateFather(item[0], null);
}

function MvcControlsToolkit_TreeView_AdjustToggleButton(
    id,
    textOrUrlEdit, cssClassEdit,
    textOrUrlUndoEdit, cssClassUndoEdit,
    textOrUrlRedoEdit, cssClassRedoEdit) {

    var button = $('#' + id + MvcControlsToolkit_TreeView_ToggleEditPostfix);
    if (button.length == 0) return;

    if (eval("typeof " + id + MvcControlsToolkit_TreeView_ButtonModePostfix + " === 'undefined'"))
        return;

    var buttonMode = eval(id + MvcControlsToolkit_TreeView_ButtonModePostfix);
    if (buttonMode == 0) {
        button.removeClass(cssClassUndoEdit);
        button.removeClass(cssClassRedoEdit);
        button.addClass(cssClassEdit);
        MvcControlsToolkit_Button_AdjustText(
            id + MvcControlsToolkit_TreeView_ToggleEditPostfix,
            textOrUrlEdit);
    }
    else if (buttonMode == 1) {
        button.removeClass(cssClassUndoEdit);
        button.removeClass(cssClassEdit);
        button.addClass(cssClassRedoEdit);
        MvcControlsToolkit_Button_AdjustText(
            id + MvcControlsToolkit_TreeView_ToggleEditPostfix,
            textOrUrlRedoEdit);
    }
    else {
        button.removeClass(cssClassRedoEdit);
        button.removeClass(cssClassEdit);
        button.addClass(cssClassUndoEdit);
        MvcControlsToolkit_Button_AdjustText(
            id + MvcControlsToolkit_TreeView_ToggleEditPostfix,
            textOrUrlUndoEdit);
    }

}

function MvcControlsToolkit_TreeView_ToggleEdit(
    id,
    textOrUrlEdit, cssClassEdit,
    textOrUrlUndoEdit, cssClassUndoEdit,
    textOrUrlRedoEdit, cssClassRedoEdit) {

    if (eval("typeof " + id + MvcControlsToolkit_TreeView_ButtonModePostfix + " === 'undefined'"))
        return;

    var buttonMode = eval(id + MvcControlsToolkit_TreeView_ButtonModePostfix);


    if (buttonMode == 0 || buttonMode == 1) {
        var edit = eval(id + MvcControlsToolkit_TreeView_SaveEditPostfix);
        $('#' +
            id + MvcControlsToolkit_TreeView_ContainerDisplayPostfix).before(edit);
        var display = $('#' +
            id + MvcControlsToolkit_TreeView_ContainerDisplayPostfix).detach();
        eval(id + MvcControlsToolkit_TreeView_SaveDisplayPostfix + ' = display;');
        eval(id + MvcControlsToolkit_TreeView_ButtonModePostfix + ' = 2;');


        document.getElementById(id + MvcControlsToolkit_TreeView_IsEditPostFix).value = 'True';
    }
    else {
        var display = eval(id + MvcControlsToolkit_TreeView_SaveDisplayPostfix);
        $('#' +
            id + MvcControlsToolkit_TreeView_ContainerEditPostfix).before(display);
        var edit = $('#' +
            id + MvcControlsToolkit_TreeView_ContainerEditPostfix).detach();
        eval(id + MvcControlsToolkit_TreeView_SaveEditPostfix + ' = edit;');
        eval(id + MvcControlsToolkit_TreeView_ButtonModePostfix + ' = 1;');

        document.getElementById(id + MvcControlsToolkit_TreeView_IsEditPostFix).value = 'False';
    }
    MvcControlsToolkit_TreeView_AdjustToggleButton(
    id,
    textOrUrlEdit, cssClassEdit,
    textOrUrlUndoEdit, cssClassUndoEdit,
    textOrUrlRedoEdit, cssClassRedoEdit);
}

function MvcControlsToolkit_TreeView_UpdateFather(item, fatherId) {
    if (item == null && fatherId == null) return;
    var root = null;
    var parentId = fatherId;
    if (parentId == null) {
        root = item.parentNode;
        parentId = root.getAttribute('id');
    }
    else {
        root = document.getElementById(parentId);
    }
    var place = parentId.lastIndexOf("_");
    if (place < 0) return;
    var parentName = parentId.substring(0, place);
    var itemsHandleName = parentName + "_handle";

    var countSonsField = document.getElementById(parentName + '___SonNumber');
    if (countSonsField == null) return;
    countSonsField.value = root.childNodes.length + '';

    var originaIdField = document.getElementById(parentName + '___OriginalId');
    if (originaIdField == null) return;
    var rootName = originaIdField.value;

    var nodeName = null;
    var placeAsSonField = null;
    var fatherNameField = null;
    for (i = 0; i < root.childNodes.length; i++) {

        var nodeId = root.childNodes[i].getAttribute('id');
        place = nodeId.lastIndexOf("_");
        if (place < 0) continue;
        nodeName = nodeId.substring(0, place);

        placeAsSonField = document.getElementById(nodeName + '___PositionAsSon');
        placeAsSonField.value = i + '';

        fatherNameField = document.getElementById(nodeName + '___FatherOriginalId');
        fatherNameField.value = rootName;
        if (root.childNodes[i] == item) {
            place = item.id.lastIndexOf("_");
            
            
            var innerContainer =$(document.getElementById(item.id.substring(0, place)+"___Item_SubContainer"));
            
            if (!innerContainer.hasClass(itemsHandleName)) {
                innerContainer.removeClass();
                innerContainer.addClass(itemsHandleName);
            }
            var current = $(item);
            if (i != root.childNodes.length - 1
                ) {
                current
                .removeClass($.treeview.classes.last)
				.removeClass($.treeview.classes.lastCollapsable)
				.removeClass($.treeview.classes.lastExpandable)
			    .find(">.hitarea")
				    .replaceClass($.treeview.classes.lastCollapsableHitarea, $.treeview.classes.collapsableHitarea)
				    .replaceClass($.treeview.classes.lastExpandableHitarea, $.treeview.classes.expandableHitarea);
                
            }
        }
    }
    if (root.childNodes.length > 0) {
        var last = $(root.childNodes[root.childNodes.length - 1]);
        if (!last.hasClass($.treeview.classes.last)) {
            last.addClass($.treeview.classes.last)
				.filter("." + $.treeview.classes.expandable).replaceClass($.treeview.classes.last, $.treeview.classes.lastExpandable).end()
				.find(">.hitarea").replaceClass($.treeview.classes.expandableHitarea, $.treeview.classes.lastExpandableHitarea).end()
				.filter("." + $.treeview.classes.collapsable).replaceClass($.treeview.classes.last, $.treeview.classes.lastCollapsable).end()
				.find(">.hitarea").replaceClass($.treeview.classes.collapsableHitarea, $.treeview.classes.lastCollapsableHitarea);
        }
    }
    if (root.childNodes.length > 1) {
        var semiLast = $(root.childNodes[root.childNodes.length - 2]);

        semiLast
            .removeClass($.treeview.classes.last)
				.removeClass($.treeview.classes.lastCollapsable)
				.removeClass($.treeview.classes.lastExpandable)
			    .find(">.hitarea")
				    .replaceClass($.treeview.classes.lastCollapsableHitarea, $.treeview.classes.collapsableHitarea)
				    .replaceClass($.treeview.classes.lastExpandableHitarea, $.treeview.classes.expandableHitarea);
        
    }
    

}

function MvcControlsToolkit_TreeView_AddNew(rootName, templateChosen) {

    var place = rootName.lastIndexOf("___");
    if (place < 0) return;
    var rootName = rootName.substring(0, place) ;

    var newElement = MvcControlsToolkit_TreeView_AddNewChoice(rootName, templateChosen); 


    
}

function MvcControlsToolkit_TreeView_Delete(itemName)
{
    
    var place = itemName.lastIndexOf("___");
    if (place < 0) return;
    var itemName = itemName.substring(0, place);
   
    var item = document.getElementById(itemName + ContainerPostFix);
    var rootName = document.getElementById(itemName + '___FatherOriginalId');
    $(item).remove();
    MvcControlsToolkit_TreeView_UpdateFather(item, rootName.value.replace(/[\$\.]/g, '_') + SortableList_ItemsContainerPrefix);
    
}

///////////////////////////////PAGER///////////////////////////////////


function PageButton_Click(pageField, pageValue, pageUrl, targetId, validationType) {
    if (pageUrl == '') {
        if (!MvcControlsToolkit_FormIsValid(pageField, validationType)) return;
        
        var field = document.getElementById(pageField);
        field.value = pageValue;
        $('#' + pageField).parents('form').submit();
    }
    else if (targetId != '') {
        $.ajax({
            type: 'GET',
            url: pageUrl,
            success: function (data) {
                $('#' + targetId).html(data);
            }
        });
    }
    else {
        window.location.href = pageUrl;
    }
}

///////////////////////////////SORTING///////////////////////////////////

function Sort_Handler(field, buttonName, initialize, causePostback, clientOrderChanged, sortField, pageField, cssNoSort, cssAscending, cssDescending, validationType) {
    if (!initialize && causePostback && !MvcControlsToolkit_FormIsValid(sortField, validationType)) return;
    var order = $('#' + sortField).val();
    var hasAscending = order.indexOf(' ' + field + '#+;');
    var hasDescending = order.indexOf(' ' + field + '#-;');
    var prevOrder = '';
    var actualOrder = '';
    if (!initialize) {
        if (hasDescending >= 0) {
            order = order.replace(' ' + field + '#-;', '');
            $('#' + sortField).val(order);
            $('#' + buttonName).removeClass(cssDescending);
            $('#' + buttonName).addClass(cssNoSort);
            prevOrder = '-';
        }
        else if (hasAscending >= 0) {
            order = order.replace(' ' + field + '#+;', ' ' + field + '#-;');
            $('#' + sortField).val(order);
            $('#' + buttonName).removeClass(cssAscending);
            $('#' + buttonName).addClass(cssDescending);
            prevOrder = '+';
            actualOrder = '-';
        }
        else {
            order = order + ' ' + field + '#+;';
            $('#' + sortField).val(order);
            $('#' + buttonName).removeClass(cssNoSort);
            $('#' + buttonName).addClass(cssAscending);
            actualOrder = '+';
        }
        if (pageField != null) $('#' + pageField).val('1');
        if (clientOrderChanged != null) eval(clientOrderChanged + "('" + field + "', '" + prevOrder + "', '" + actualOrder + "')");
        if (causePostback) $('#' + sortField).parents('form').submit();
    }
    else {
        if (hasDescending >= 0) {
            $('#' + buttonName).addClass(cssDescending);
        }
        else if (hasAscending >= 0) {
            $('#' + buttonName).addClass(cssAscending);
        }
        else {
            $('#' + buttonName).addClass(cssNoSort);
        }
    }
}

////////////////////////////////DETAIL FORM///////////////////////////////////////////////


function Setup_Ajax_ClientValidation(formId, validationType) {
    if (validationType == ValidationType_StandardClient) {
        var allFormOptions = window.mvcClientValidationMetadata;
        if (allFormOptions) {
            for (var i = 0; i < allFormOptions.length; i++) {
                var thisFormOptions = allFormOptions[i];
                thisFormOptions.FormId = formId;
            }
        }
        Sys.Mvc.FormContext._Application_Load();
    }
    else {
        if (typeof $ !== 'undefined' && $ !== null && typeof $.validator !== 'undefined' && $.validator !== null && typeof $.validator.unobtrusive !== 'undefined' && $.validator.unobtrusive !== null) {

            $.validator.unobtrusive.clearAndParse('#' + formId);
        }
    }
}

function OnBeginDetailForm(baseName, detailType, rowId, loadingElementId, validationType) {
    detailBusy = eval(baseName + '_DetailBusy');
    if (detailBusy) return false;
    eval(baseName + '_DetailBusy = true;');
    eval(baseName + "_TypeDetail = '" + detailType + "';");
    if (rowId != null) {//rowId null means re-posting on the same column
        if (detailType == 'Insert') {
            eval(baseName + "_CurrentRow = null;"); // grid need not be updated on success
        }
        else {
            eval(baseName + "_CurrentRow = '" + rowId + "';");
        }
    }
    else {
        var res = MvcControlsToolkit_FormIsValid(loadingElementId, validationType);
        return res;
    }
    return true;
}

function OnFailureDetailForm(baseName, displayExecute, editExecute) {
    var detailType = eval(baseName + '_TypeDetail');
    if (detailType == 'Display') {
        if (displayExecute != null) (displayExecute + '();');
    }
    else {
        if (editExecute != null) (editExecute + '();');
    }
    eval(baseName + '_DetailBusy = false;');
}

function TrueValue(value, fieldName) {
    var trueValue = value;
    try {
        trueValue = eval(fieldName + "_True");
    }
    catch (e) {

    }
    return trueValue;
}

function FormattedValue(value, fieldName) {
    var fValue = value;
    try {
        fValue = eval(fieldName + "_Format");
    }
    catch (e) {

    }
    var companionField = document.getElementById(fieldName + "_hidden");
    if (companionField != null) {
        fValue = companionField.value; 
    }
    return fValue;
}



function OnSuccessDetailForm(baseName, displayExecute, editExecute, ajaxContext, formId, validationType, unobtrusiveAjaxOn, updateTargetId) {
    if (!unobtrusiveAjaxOn) GlobalEvalScriptInElementId(ajaxContext.get_updateTarget());
    else $('#' + updateTargetId).html(ajaxContext);
    if (validationType != ValidationType_Server) Setup_Ajax_ClientValidation(formId, validationType);
    var detailType = eval(baseName + '_TypeDetail');
    var changedFieldCss = eval(baseName + '_ChangedFieldCss');
    var deletedRecordCss = eval(baseName + '_DeletedRecordCss');
    var isValid = false;
    var hiddenIsValid = document.getElementById("IsValid");
    if (hiddenIsValid != null && hiddenIsValid.value == "True") isValid = true;
    if (isValid && (detailType == 'FirstEdit' || detailType == 'Edit' || detailType == 'Display')) {
        var itemRoot = eval(baseName + "_CurrentRow");
        var fieldsToUpdate = eval(baseName + "_FieldsToUpdate").split(",");
        var detailRoot = eval(baseName + "_DetailPrefix");
        if (itemRoot != null) {
            var oldItemRoot = itemRoot.substring(0, itemRoot.lastIndexOf('Value')) + 'OldValue';
            var isInsert = true;
            var isExternalDelete = true;
            var changedDisplay = eval(itemRoot + SavePostFixC);
            var changedDisplayAvailable = false;
            if (changedDisplay == null) changedDisplay = eval(itemRoot + SavePostFixD);
            else changedDisplayAvailable = true;
            for (var i = 0; i < fieldsToUpdate.length; i++) {
                var oldField = document.getElementById(oldItemRoot + "_" + fieldsToUpdate[i]);
                if (oldField != null) isInsert = false;
                var newFieldName = null;
                if (detailRoot.length == 0) {
                    newFieldName = fieldsToUpdate[i];
                }
                else {
                    newFieldName = oldItemRoot + "_" + fieldsToUpdate[i];
                }
                var newField = document.getElementById(newFieldName);
                if (newField != null) isExternalDelete = false;
                if (oldField != null) {
                    var newValue = null;
                    var newFormattedValue = null;
                    if (newField != null && newField.getAttribute('type') != null && (newField.getAttribute('type').toLowerCase() == 'checkbox' || newField.getAttribute('type').toLowerCase() == 'radiobutton')) {
                        if (newField.getAttribute('checked') != null && newField.checked == true) newValue = 'True';
                        else newValue = 'False';
                    }
                    else {
                        if (newField != null) {
                            if (newField.nodeName != null && (newField.nodeName.toLowerCase() == "input" || newField.nodeName.toLowerCase() == "textarea" || newField.nodeName.toLowerCase() == "select")) {
                                newValue = TrueValue(newField.value, newFieldName);
                                newFormattedValue = FormattedValue(newField.value, newFieldName);
                            }
                            else {
                                newValue = TrueValue(newField.childNodes(0).nodeValue, newFieldName);
                                newFormattedValue = FormattedValue(newField.childNodes(0).nodeValue, newFieldName);
                            }
                        }
                        else {
                            newValue = TrueValue(null, newFieldName);
                            newFormattedValue = FormattedValue(null, newFieldName);
                        }
                    }
                    var itemToUpdate = changedDisplay.find('#' + itemRoot + '_' + fieldsToUpdate[i]);
                    if (changedFieldCss != null) {
                        if (newValue != null && oldField.value != newValue) {
                            itemToUpdate.addClass(changedFieldCss);

                        }
                        else {
                            itemToUpdate.removeClass(changedFieldCss);
                        }
                    }
                    if (newValue != null && oldField.value != newValue) {
                        oldField.value = newValue;
                        if (itemToUpdate.length != 0) {
                            var inputType = itemToUpdate.attr('type');
                            if (inputType != null && (inputType.toLowerCase() == 'checkbox' || inputType.toLowerCase() == 'radiobutton')) {
                                if (newValue.toLowerCase() == 'true') itemToUpdate.checked = true;
                                else itemToUpdate.checked = false;
                            }
                            else {
                                itemToUpdate.html(newFormattedValue);
                            }
                        }
                    }

                }
            }
            if (isExternalDelete) {
                $('input[id^="' + oldItemRoot + '"]').remove();
                if (deletedRecordCss != null) {
                    var newSavePostFixD = eval(itemRoot + SavePostFixD);
                    newSavePostFixD.addClass(deletedRecordCss);
                    $('#' + itemRoot + DisplayPostfix + ContainerPostFix).replaceWith(newSavePostFixD.clone(true));
                }
            }
            else if (!isInsert) {
                if (changedDisplayAvailable)
                    eval(itemRoot + SavePostFixD + " = " + itemRoot + SavePostFixC + ";");
                var newSavePostFixD = eval(itemRoot + SavePostFixD);
                $('#' + itemRoot + DisplayPostfix + ContainerPostFix).replaceWith(newSavePostFixD.clone(true));
            }

        }
    }

    if (detailType == 'Display') {
        if (displayExecute != null) eval(displayExecute + '();');
    }
    else {
        if (editExecute != null) eval(editExecute + '();');
    }
    MvcControlsToolkit_DataGridApplyStylesItem(itemRoot);
    eval(baseName + '_DetailBusy = false;');
}

////////////////////////////////ViewList and ViewsOnOff///////////////////////////////////////////////

function ViewList_Client(groupName, hiddenField, cssSelected, prefix) {
    this.CssSelected = cssSelected;
    this.GroupName = groupName;
    this.HiddenField = hiddenField;
    this.Prefix = prefix;
    var allViews = $('.' + groupName);
    allViews.each(function (i) {
        var name = this.id + "_placeholder";
        var thisId = this.id;
        $('#' + thisId).before("<span style='display:none;' id='" + name + "'></span>");

        $('.' + thisId + "_checkbox").click(function () {
            if ($("."+thisId + "_checkbox")[0].checked)
                eval(groupName + "_ViewList").Select(thisId);
            else
                eval(groupName + "_ViewList").Select('');
        });
        
    });
    this.SelectionSet = allViews.detach();
    this.SelectionSet.find('script').remove();
}

ViewList_Client.prototype = {
    HiddenField: null,
    GroupName: null,
    CssSelected: null,
    SelectionSet: null,
    Prefix: null,
    Select: function (target) {
        $('.' + this.GroupName + '_button').removeClass(this.CssSelected);
        $('.' + this.GroupName + '_checkbox').each(function(i){this.checked=false});
        $('.' + this.GroupName).detach();

        if (target == '') {
            document.getElementById(this.HiddenField).value = '';
            return;
        }
        document.getElementById(this.HiddenField).value = target;
        target = this.Prefix + target;
        this.SelectionSet.filter('#' + target).insertBefore('#' + target + '_placeholder');

        $('.' + target + '_button').addClass(this.CssSelected);

        $('.' + target + '_checkbox').each(function (i) { this.checked = true });
    }
}

function ViewsOnOff_Client_Switch(groupName, on, hidden) {
    if (on) {
        $('.' + groupName + '_checkbox').each(function (i) { this.checked = true });
        var toAttach = eval(groupName + "_ViewsOnOff");
        toAttach.each(function (i) {
            var currId = this.id;
            $(this).insertBefore('#' + currId + "_placeholder");
        });
        document.getElementById(hidden).value = "True";
    }
    else {
        $('.' + groupName + '_checkbox').each(function (i) { this.checked = false });
        $('.' + groupName).detach();
        document.getElementById(hidden).value = "False";
    }
}

function ViewsOnOff_Client_Initialize(groupName, initial_on, hidden) {
    var allViews = $('.' + groupName);
    allViews.each(function (i) {
        var prova = this.id;
        if (this.id == null || this.id == '') this.id = groupName + "_el" + i;
        var name = this.id + "_placeholder";
        var thisId = this.id;
        $('#' + this.id).before("<span style='display:none;' id='" + name + "'></span>");

        $('.' + groupName + "_checkbox").click(function (event) {
            ViewsOnOff_Client_Switch(groupName, event.target.checked, hidden);
        });
    });
    var selectionSet = allViews.detach();
    selectionSet.find('script').remove();
    eval(groupName + "_ViewsOnOff = selectionSet;");
    if (initial_on) {
        $('.' + groupName + '_checkbox').each(function (i) { this.checked = false });
        selectionSet.each(function (i) {
            var currId = this.id;
            $(this).insertBefore('#' + currId + "_placeholder");
        });
    }
    else {
        $('.' + groupName + '_checkbox').each(function (i) { this.checked = false });
    }

}
////////////Typed TextBox ////////////////
var MvcControlsToolkit_DataType_String = 0;
var MvcControlsToolkit_DataType_UInt = 1;
var MvcControlsToolkit_DataType_Int = 2;
var MvcControlsToolkit_DataType_Float = 3;
var MvcControlsToolkit_DataType_DateTime = 4;

function MvcControlsToolkit_Format(value, format, dataType, prefix, postfix) {
    if (dataType<0)
        return value;
    if (value == null) return '';
    return prefix + MvcControlsToolkit_ToString(value, format, dataType) + postfix;
}
function MvcControlsToolkit_FormatDisplay(value, format, dataType, prefix, postfix, nullString) {
    if (dataType<0) 
        return value;
    if (value == null || value === '') return nullString;
    return prefix + MvcControlsToolkit_ToString(value, format, dataType) + postfix;
}
function MvcControlsToolkit_ToString(value, format, dataType) {
    if (dataType == MvcControlsToolkit_DataType_String || dataType < 0) {
        if (value === true) return "True";
        if (value === false) return "False";
        return value;
    }
    if (value == null) return '';
    if (format == '') {
        if (dataType == MvcControlsToolkit_DataType_DateTime) {
            format = 'd';
        }
        else if (dataType == MvcControlsToolkit_DataType_Int ||
            dataType == MvcControlsToolkit_DataType_UInt) {
            format = 'd';
        }
        else if (dataType == MvcControlsToolkit_DataType_Float) {
            format = 'n';
        }
        else {
            return value;
        }
        
    }
    if ((typeof jQuery !== 'undefined') && (typeof jQuery.global !== 'undefined') && (typeof jQuery.global.parseInt === 'function')) {
        return jQuery.global.format(value, format);
    }
    else if ((typeof Number !== 'undefined') && (typeof Number.parseLocale === 'function')) {
        return value.localeFormat(format);
    }
    else {
        return value + '';
    }
}
function MvcControlsToolkit_Parse(value, dataType) {
    if (dataType == MvcControlsToolkit_DataType_String) return value;
    if (value == '') return null;
    if (dataType == MvcControlsToolkit_DataType_Float) {
        if ((typeof jQuery !== 'undefined') && (typeof jQuery.global !== 'undefined') && (typeof jQuery.global.parseFloat == 'function')) {
            return jQuery.global.parseFloat(value);
        }
        else if ((typeof Number !== 'undefined') && (typeof Number.parseLocale == 'function')) {
            return Number.parseLocale(value);
        }
        else {
            return parseFloat(value);
        }
    }
    else if (dataType == MvcControlsToolkit_DataType_DateTime) {
        if ((typeof jQuery !== 'undefined') && (typeof jQuery.global !== 'undefined') && (typeof jQuery.global.parseDate == 'function')) {
            return jQuery.global.parseDate(value);
        }
        else if ((typeof Date !== 'undefined') && (typeof Date.parseLocale == 'function')) {
            return Date.parseLocale(value);
        }
        else {
            return Date.parse(value);
        }
    }
    else {
        if ((typeof jQuery !== 'undefined') && (typeof jQuery.global !== 'undefined') && (typeof jQuery.parseInt == 'function')) {
            return jQuery.global.parseInt(value);
        }
        else if (typeof Number.parseLocale == 'function') {
            var tFloat = Number.parseLocale(value);
            if (isNaN(tFloat)) return tFloat;
            return parseInt(tFloat + '');
        }
        else {
            return parseInt(value, 10);
        }
    }
}

function MvcControlsToolkit_TypedTextBox_Input(charCode, fieldId, companionId, dataType, decimalSeparator, digitSeparator, plus, minus) {
    if (dataType == MvcControlsToolkit_DataType_String ||
    charCode == 0 || charCode == 13 || charCode == 8 || charCode == digitSeparator.charCodeAt(0)
    || (charCode >= 48 && charCode <= 57)) return true;
    if ((dataType == MvcControlsToolkit_DataType_Int || dataType == MvcControlsToolkit_DataType_Float)
    && (charCode == plus.charCodeAt(0) || charCode == minus.charCodeAt(0))) {
        var value = document.getElementById(fieldId).value;
        return value.indexOf(plus) < 0 && value.indexOf(minus) < 0;
    }
    if (dataType == MvcControlsToolkit_DataType_Float && charCode == decimalSeparator.charCodeAt(0)) {
        var value = document.getElementById(fieldId).value;

        return value.indexOf(decimalSeparator) < 0;
    }
    return false;
}
function MvcControlsToolkit_TypedTextBox_Focus(fieldId, companionId, watermarkCss) {
    document.getElementById(fieldId).value = document.getElementById(companionId).value;
    if (watermarkCss != '') $('#' + fieldId).removeClass(watermarkCss);
}
function MvcControlsToolkit_DisplayEdit_DbClick(fieldId, companionId) {
    $('#' + companionId).hide();
    $('#' + fieldId).show().focus();
}
function MvcControlsToolkit_TypedTextBox_Blur(
fieldId, companionId, dataType,
pre, post, format, plus, minus, decimalSeparator, digitSeparator,
watermark, watermarkCss, validationType) {
    var fieldElement = document.getElementById(fieldId);
    if (fieldElement == null) return;
    var value = fieldElement.value;
    
    var innerValue = value;
    if (dataType != MvcControlsToolkit_DataType_String) {
        value = MvcControlsToolkit_Trim(value);
        innerValue = value;
        var tValue = value;
        tValue = tValue.replace(digitSeparator, '');
        tvalue = tValue.replace(plus, '');
        negative = tValue.indexOf(minus);
        tValue = tValue.replace(minus, '');
        var toBuild = '';
        var charCode = '';
        for (var i = 0; i < tValue.length; i++) {
            charCode = tValue.charCodeAt(i);
            if ((charCode >= 48 && charCode <= 57) || charCode == decimalSeparator.charCodeAt(0)) {
                toBuild = toBuild + tValue.charAt(i);
            }
        }
        tValue = toBuild;
        if (value != '') {
            var nValue = 0;
            try {
                nValue = MvcControlsToolkit_Parse(tValue, dataType);
                if (negative >= 0) nValue = nValue * -1;
                if (!isNaN(nValue)) {
                    value = MvcControlsToolkit_ToString(nValue, format, dataType);
                    if (dataType == MvcControlsToolkit_DataType_Float) {
                        if (negative >= 0) innerValue = minus + tValue;
                        else innerValue = tValue;
                    }
                    else
                        innerValue = MvcControlsToolkit_ToString(nValue, 'n0', dataType);
                }
            }
            catch (e) {
            }

        }
    }
    document.getElementById(companionId).value = innerValue;

    MvcControlsToolkit_Validate(companionId, validationType);

    $('#' + fieldId).removeClass('input-validation-error');
    if ($('#' + companionId).hasClass('input-validation-error')) {
        $('#' + fieldId).addClass('input-validation-error');
    }
    
    if (value == '') {
        if (watermarkCss != '') $('#' + fieldId).addClass(watermarkCss);
        document.getElementById(fieldId).value = watermark;
    }
    else {
        document.getElementById(fieldId).value = pre + value + post;
    }
    
    $('#' + fieldId).trigger('TypedTextBox_Changed');

}
function MvcControlsToolkit_DisplayEdit_Blur(
fieldId, companionId, dataType,
pre, post, format, plus, minus, 
decimalSeparator, digitSeparator, validationType, nullDisplayText) {
    var fieldElement = document.getElementById(fieldId);
    if (fieldElement == null) return;
    var value = fieldElement.value;
    
    var innerValue = value;
    if (dataType != MvcControlsToolkit_DataType_String) {
        value = MvcControlsToolkit_Trim(value);
        innerValue = value;
        var tValue = value;
        tValue = tValue.replace(digitSeparator, '');
        tvalue = tValue.replace(plus, '');
        negative = tValue.indexOf(minus);
        tValue = tValue.replace(minus, '');
        var toBuild = '';
        var charCode = '';
        for (var i = 0; i < tValue.length; i++) {
            charCode = tValue.charCodeAt(i);
            if ((charCode >= 48 && charCode <= 57) || charCode == decimalSeparator.charCodeAt(0)) {
                toBuild = toBuild + tValue.charAt(i);
            }
        }
        tValue = toBuild;
        if (value != '') {
            var nValue = 0;
            try {
                nValue = MvcControlsToolkit_Parse(tValue, dataType);
                if (negative >= 0) nValue = nValue * -1;
                if (!isNaN(nValue)) {
                    value = MvcControlsToolkit_ToString(nValue, format, dataType);
                    if (dataType == MvcControlsToolkit_DataType_Float) {
                        if (negative >= 0) innerValue = minus + tValue;
                        else innerValue = tValue;
                    }
                    else
                        innerValue = MvcControlsToolkit_ToString(nValue, 'n0', dataType);
                }
            }
            catch (e) {
            }

        }
    }
    fieldElement.value = innerValue;

    MvcControlsToolkit_Validate(fieldId, validationType);
    $('#' + fieldId).hide();
    $('#' + companionId).show();

    if (value == '') {
        if (nullDisplayText == '')
            $('#' + companionId).html('&nbsp;&nbsp;&nbsp;');
        else
            $('#' + companionId).text(nullDisplayText); 
    }
    else {
        $('#'+companionId).text(pre + value + post);
    }
    $('#' + fieldId).trigger('TypedEditDisplay_Changed');
}

function MvcControlsToolkit_TypedTextBox_SetString(field, value)
{
    var field = $(field);
    field.trigger('focus');
    field.val(value);
    field.trigger('blur');
}
function MvcControlsToolkit_TypedTextBox_GetString(source) {
    var companionId = source.id.substring(0, source.id.lastIndexOf("_"));
    return $('#' + companionId).val();
}
function MvcControlsToolkit_TypedTextBox_Set(source, value, format, valueType) {
    var field = $(source);
    var companionId = source.id.substring(0, source.id.lastIndexOf("_"));
    var companion = $('#' + companionId);
    if (companion.length == 0) {
        var retry = function () { MvcControlsToolkit_TypedTextBox_Set(source, value, format, valueType); };
        setTimeout(retry, 0);
        return;
    }
    var value = MvcControlsToolkit_Format(value, format, valueType, '', '');
    companion.val(value);
    field.val(value);
    field.trigger('pfocus');
    field.trigger('pblur');
    
}
function MvcControlsToolkit_TypedTextBox_Get(source, valueType) {
    var companionId = source.id.substring(0, source.id.lastIndexOf("_"));
    return MvcControlsToolkit_Parse($('#' + companionId).val(), valueType);
}

function MvcControlsToolkit_TypedEditDisplay_SetString(field, value) {
    var field = $(field);
    field.val(value);
    field.trigger('blur');
}
function MvcControlsToolkit_TypedEditDisplay_GetString(source) {
    return $(source).val();
}
function MvcControlsToolkit_TypedEditDisplay_Set(source, value, format, valueType) {
    var field = $(source);
    $(source).val(MvcControlsToolkit_Format(value, format, valueType, '', ''));
    field.trigger('pblur');

}
function MvcControlsToolkit_TypedEditDisplay_Get(source, valueType) {
    return MvcControlsToolkit_Parse($(source).val(), valueType);
}

//////////////////////////Display Field ////////////
function MvcControlsToolkit_DisplayField_SetString(field, value) {
    return;   
}
function MvcControlsToolkit_DisplayField_GetString(field) {
    return eval(field.id + "_True");
}

//////////////////////////Timer/////////////////////
function MvcControlsToolkit_AjaxLink(url, targetId) {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (data) {
            $('#' + targetId).html(data);
        }
    });
}
function MvcControlsToolkit_AjaxSubmit(validationType, elementId) {
    if (!MvcControlsToolkit_FormIsValid(elementId, validationType)) return;

    $('#' + elementId).parents('form').submit();
}
/////////////////////////Hover menu/////////////////////////////
$(document).ready(function () {
    $('.MvcControlsToolkit-Hover').mouseover(function () {
        $(this).addClass('MvcControlsToolkit-Hover-On');
    });
    $('.MvcControlsToolkit-Hover').mouseout(function (e) {
        // Do not close if going over to a select element
        if (e.target.tagName.toLowerCase() == 'select') return;
        $(this).removeClass('MvcControlsToolkit-Hover-On');
    });

});
///////////////////////Template Names//////////////////////////////

function MvcControlsToolkit_NewTemplateName(item) {
    if (eval("typeof _MvcControlsToolkit_" + item.ModelId + " === 'undefined'")) {
        jQuery.globalEval("var _MvcControlsToolkit_" + item.ModelId + " = 0;");
    }
    else {
        eval("_MvcControlsToolkit_" + item.ModelId + "++ ;");
    }
    return "";
}
function MvcControlsToolkit_TemplateName(item) {
    return item.ModelPrefix + "[" + eval("_MvcControlsToolkit_" + item.ModelId)+"]";
}
function MvcControlsToolkit_TemplateId(item) {
    return item.ModelId + "_" + eval("_MvcControlsToolkit_" + item.ModelId) + "_";
}
///////////////////////////Templates/////////////////////////////
function MvcControlsToolkit_InitializeCreatedNodes(elements, unobtrusiveValidation, noInput, noJavaScript, applyValidation, delayedBatch) {
    if (elements == null || (noJavaScript && noInput)) return;
    
    toApply = function () {
        for (var i = 0; i < elements.length; i++) {
            if (unobtrusiveValidation && applyValidation) {
                jQuery.validator.unobtrusive.parseExt(elements[i]);
            }
            if (!noJavaScript) GlobalEvalScriptAndDestroy(elements[i]);
            if (!noInput) {
                var jel = $(elements[i]);

                var sons = jel.children();
                sons.trigger('blur');
                sons.trigger('change');
            }

        }
    };
    setTimeout(toApply, 0);
}

function MvcControlsToolkit_ServerErrors(elements) {
    var ToApply = function () {
        for (var i = 0; i < elements.length; i++) {
            var currElement = elements[i];
            var currDom = $('#' + currElement.id);
            if (currDom.length == 0) continue;
            var currForm = currDom.parents('form').first();
            if (currForm.length == 0) continue;

            if (!currDom.hasClass('input-validation-error'))
                currDom.addClass('input-validation-error');
            var attr = currDom.attr('companionpostfix');
            if (typeof attr !== 'undefined' && attr !== false) {
                var companion = $('#'+ currElement.id + attr);
                if (companion.length > 0 && !companion.hasClass('input-validation-error'))
                    companion.addClass('input-validation-error');
            }
            var currDisplay = $(currForm).find("[data-valmsg-for='" + currElement.name + "']");
            if (currDisplay.length > 0) {
                currDisplay.removeClass("field-validation-valid").addClass("field-validation-error");
                replace = $.parseJSON(currDisplay.attr("data-valmsg-replace")) !== false;
                if (replace) {
                    currDisplay.empty();
                    $(currElement.errors[0]).appendTo(currDisplay);
                }
            }
        }
    };
    setTimeout(ToApply, 0);
}
