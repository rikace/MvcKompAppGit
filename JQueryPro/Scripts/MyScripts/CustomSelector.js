/*Creating custom filters for selecting elements
The capabilities of the jQuery selector engine can be extended by creating your own custom filters. In theory, all you are doing here is building upon the custom selectors that are already part of jQuery. For example, say we would like to select all elements on a Web page that are absolutely positioned. Since jQuery does not already have a custom :positionAbsolute filter, we can create our own.*/

// Define custom filter by extending $.expr[':']
$.expr[':'].positionAbsolute = function (element)
{ return $(element).css('position') === 'absolute'; };
// How many elements in the page are absolutely positioned?
alert($(':positionAbsolute').length); // Alerts "4"
// How many div elements are absolutely positioned?
alert($('div:positionAbsolute').length); // Alerts "2".

/*The most important thing to grasp here is that you are not limited to the default selectors provided by jQuery. You can create your own. However, before you spend the time creating your own version of a selector, you might just simply try the filter() method with a specified filtering function. For example, I could have avoided writing the :positionAbsolute selector by simply filtering the <div> elements in my prior example with a function I pass to the filter() method.*/
// Remove <div> elements from the wrapper
// set that are not absolutely positioned 

$('div').filter(function () { return $(this).css('position') === 'absolute'; });
// or
// Remove all elements from the wrapper
// set that are not absolutely positioned
$('*').filter(function () { return $(this).css('position') === 'absolute'; });

// selector with multi attribute
alert($('a[title="jQuery"][href^="http://"]').length);
 $('p').filter(':not(:first):not(:last)').


