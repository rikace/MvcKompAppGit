/// <reference path="jquery-1.7.1.intellisense.js" />
/// <reference path="jquery-1.7.1.js" />


// create an object literal
var myObj = {
    Name:"Riccardo",
    Age:30
};

// Create the cody object
var cody = new Object();
// then fill the cody object with properties (using dot notation).
var cody = new Object();
cody.living = true;
cody.age = 33;
cody.gender = 'male';
cody.getGender = function () { return cody.gender; };

console.log(cody.getGender()); // Logs 'male'.
console.log(cody); // Logs Object {living = true, age = 33, gender = 'male'}


var myObj2 = {
    "First": "Bugghina",
    "Last": "Terrell"
}

if ("Name" in myObj) {
    // do something
}

var star = {};
star["Polaris"] = new Object();
star["Vega"] = new Object();

var starArray = new Array();
starArray[0] = "Polaris";
starArray[1] = "Vega";


var someOtherStar = new Object();
someOtherStar.name = "Polaros";
someOtherStar.type = "Star";
someOtherStar.constellation = "Ursa Minor";

for (var i = 0; i < length; i++) {
    console.log(i);
}

for (var m in star) {
    console.log(star[m]);
}

var cookieName = "testcookie";
var cookieVal = "value";
var myCookie = cookieName + '=' + cookieVal;
document.cookie = myCookie;


var myObject = new Object(); // Produces an Object() object. myObject['0'] = 'f';
myObject['1'] = 'o';
myObject['2'] = 'o';
console.log(myObject); // Logs Object { 0="f", 1="o", 2="o"}


var myString = new String('foo'); // Produces a String() object.
console.log(myString); // Logs foo { 0="f", 1="o", 2="o"}


// Instantiate an instance for each native constructor using the new keyword.
var myNumber = new Number(23);
var myString = new String('male');
var myBoolean = new Boolean(false);
var myObject = new Object();
var myArray = new Array('foo', 'bar');
var myFunction = new Function("x", "y", "return x*y"); var myDate = new Date();
var myRegExp = new RegExp('\bt[a-z]+\b');
var myError = new Error('Darn!');
// Log/verify which constructor created the object. console.log(myNumber.constructor); // Logs Number() console.log(myString.constructor); // Logs String() console.log(myBoolean.constructor); // Logs Boolean() console.log(myObject.constructor); // Logs Object() console.log(myArray.constructor); // Logs Array() in modern browsers. console.log(myFunction.constructor); // Logs Function() console.log(myDate.constructor); // Logs Date() console.log(myRegExp.constructor); // Logs RegExp() console.log(myError.constructor); // Logs Error()

var myNumber = new Number(23); // An object.
var myNumberLiteral = 23; // Primitive number value, not an object. var myString = new String('male'); // An object.
var myStringLiteral = 'male'; // Primitive string value, not an object. var myBoolean = new Boolean(false); // An object.
var myBooleanLiteral = false; // Primitive boolean value, not an object. var myObject = new Object();
var myObjectLiteral = {};
var myArray = new Array('foo', 'bar');
var myArrayLiteral = ['foo', 'bar'];
var myFunction = new Function("x", "y", "return x*y");
var myFunctionLiteral = function (x, y) { return x * y };
var myRegExp = new RegExp('\bt[a-z]+\b');
var myRegExpLiteral = /\bt[a-z]+\b/;