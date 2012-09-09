var GLOBALS = GLOBALS || {};

GLOBALS.namespace = function (ns) {
    var objects = ns.split("."),
            parent = GLOBALS,
            startIndex = 0,
            i;

    // You have one GLOBALS (or whatever name) per app. It exists if you
    // call this function; so you can safely ignore the root of the namespace 
    // if it matches the parent string.
    if (objects[0] === "GLOBALS")
        startIndex = 1;

    for (i = startIndex; i < objects.length; i++) {
        // Create the object as child of MYAPP if it doesn't exist
        var property = objects[startIndex];
        if (typeof parent[property] === "undefined")
            parent[property] = {};
        parent = parent[property];
    }
    
    return parent;
};