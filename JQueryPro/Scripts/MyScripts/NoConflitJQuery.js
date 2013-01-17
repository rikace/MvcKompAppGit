/*Creating an alias by renaming the jQuery object itself
jQuery provides the noConflict() method, which has several uses—namely, the ability to replace $ with another alias. This can be helpful in three ways: It can relinquish the use of the $ sign to another library, help avoid potential conflicts, and provide the ability to customize the namespace/alias for the jQuery object.
For example, let's say that you are building a Web application for company XYZ. It might be nice to customize jQuery so that instead of having to use jQuery('div').show() or $('div').show() you could use XYZ('div').show() instead. */

var Syncfusion = jQuery.noConflict();
// Do something with jQuery methods.
alert(Syncfusion("div").text());

