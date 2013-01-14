/*Creating inheritance chains (the original intention)
Prototypal inheritance was conceived to allow inheritance chains that mimic the inheritance patterns found in traditional object oriented programming languages. In order for one object to inherit from another object in JavaScript, all you have to do is instantiate an instance of the object you want to inherit from and assign it to the prototype property of the object that is doing the inheriting.
In the code sample that follows, Chef objects (i.e. cody) inherit from Person(). This means that if a property is not found in a Chef object, it will then be looked for on the prototype of the function that created Person() objects. To wire up the inheritance, all you have to do is instantiate an instance of Person() as the value for Chef.prototype (i.e. Chef.prototype = new Person(); ).
*/

var Person = function () { this.bar = 'bar' };
Person.prototype.foo = 'foo';
var Chef = function () { this.goo = 'goo' }; Chef.prototype = new Person();
var cody = new Chef();
console.log(cody.foo);// Logs 'foo'.
console.log(cody.goo);// Logs 'goo'.
console.log(cody.bar);// Logs 'bar'.


var Person = function () { };
// All Person instances inherit the legs, properties.
Person.prototype.legs = 2;
Person.prototype.arms = 2;
Person.prototype.countLimbs = function () {
    return this.legs + this.arms;
};

var chuck = new Person(); console.log(chuck.countLimbs()); // Logs 4.