// Define Person constructor function in order to create custom Person() objects later.
var Person = function (living, age, gender) {
    this.living = living;
    this.age = age;
    this.gender = gender;
    this.getGender = function () { return this.gender; };
};

var Person2 = function (living, age, gender) {
    this.living = living;
    this.age = age;
    this.gender = gender;
    this.getGender = function () { return this.gender; };
};

/*
If you said, "A constructor is nothing more than a function," then I would reply, "You are correct—unless that function is invoked using the new keyword." (For example, new String('foo')). When this happens, a function takes on a special role, and JavaScript treats the function as special by setting the value of this for the function to the new object that is being constructed. In addition to this special behavior, the function will return the newly created object (i.e. this) by default instead of the value false. The  new object that is returned from the function is considered to be an instance of the constructor function that constructs it. 
As you can see, by passing unique parameters and invoking the Person() constructor function, you could easily create a vast number of unique people objects. 
if you create a constructor function and call it without the use of the new keyword, the this value will refer to the "parent" object that contains the function.*/

// Instantiate a Person object and store it in the cody variable.
var cody = new Person(true, 33, 'male');
var cody2 = new Person2(true, 38, 'female');

var Person = function () { };
// All Person instances inherit the legs, properties.
Person.prototype.legs = 2; 
Person.prototype.arms = 2; 
Person.prototype.countLimbs = function () {
    return this.legs + this.arms;
};

var chuck = new Person(); console.log(chuck.countLimbs()); // Logs 4.