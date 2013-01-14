/*Complex objects are made up of dynamic properties. This allows user-defined objects, and most of the native objects, to be mutated. This means that the majority of objects in JavaScript can be updated or changed at any time. Because of this, we can change the native pre-configured nature of JavaScript itself by augmenting its native objects. However, I am not telling you to do this; in fact I do not think you should. But let's not cloud what is possible with opinions.*/

// Augment the built-in String constructor Function() with the augmentedProperties property.
String.augmentedProperties = [];
if (!String.prototype.trimIT) { // If the prototype does not have trimIT() add it.
    String.prototype.trimIT = function () {
        return this.replace(/^\s+|\s+$/g, '');
    }
    // Now add trimIT string to the augmentedProperties array.
    String.augmentedProperties.push('trimIT');
}
var myString = '  trim me  ';
console.log(myString.trimIT()); // Invoke our custom trimIT string method, logs 'trim me'.
console.log(String.augmentedProperties.join()); // Logs 'trimIT'.

// It can also work easy=iy like this:
String.prototype.trimIT = function () {
    return this.replace(/^\s+|\s+$/g, '');
}
