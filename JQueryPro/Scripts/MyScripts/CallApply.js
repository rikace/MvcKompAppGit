// Function pattern.
var myFunction = function () { return 'foo' }; console.log(myFunction()); // Logs 'foo'.
// Method pattern.
var myObject = { myFunction: function () { return 'bar'; } }
console.log(myObject.myFunction()); // Logs 'bar'.
// Constructor pattern.
var Cody = function () {
    this.living = true;
    this.age = 33;
    this.gender = 'male';
    this.getGender = function () { return this.gender; };
}
var cody = new Cody(); // Invoke via the Cody constructor. console.log(cody); // Logs the cody object and properties.
// apply() and call() pattern.
var greet = {
    runGreet: function () {
        console.log(this.name, arguments[0], arguments[1]); }
}
var cody = { name: 'cody' };
var lisa = { name: 'lisa' };
// Invoke the runGreet function as if it were inside of the cody object. 
greet.runGreet.call(cody, 'foo', 'bar'); // Logs 'cody foo bar'.


// Invoke the runGreet function as if it were inside of the lisa object. 
greet.runGreet.apply(lisa, ['foo', 'bar']); // Logs 'lisa foo bar'.
/* Notice the difference between call() and apply() in how parameters are sent to the function being invoked. */