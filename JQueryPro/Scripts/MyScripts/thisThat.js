var myObject = {
    myProperty: 'I can see the light', myMethod: function () {
        var that = this; // Store a reference to this (i.e. myObject) in myMethod scope.

        var helperFunction = function () { // Child function.
            // Logs 'I can see the light' via scope chain because that
            console.log(that.myProperty); // Logs 'I can see the
            console.log(this); // Logs window object, if we don't use }();
        }();
    }
}
myObject.myMethod(); // Invoke myMethod.
