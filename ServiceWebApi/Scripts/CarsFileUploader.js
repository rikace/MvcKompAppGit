/*
CarlsFileUploader.js
by Carl Franklin

Uploads multiple files to Web API service using Jquery and vanilla javascript.

*/

var progressCallbackFunction;
var doneCallbackFunction;
var errorCallbackFunction;
var uploadStartCallbackFunction;
var username;
var fileQueue;
var file;
var fileCount;
var fileIndex;

/*
CFUpload Arguments:
    username            name used to create subfolders on the server.
    files               files object from the file input tag.
    uploadStartFunction function that receives a file object just before uploading.
    progressFunction    function that accepts a percent-complete integer value.
    doneFunction        function called when file is uploaded.
    errorFunction       function called when an error occurs.
    chunkSize           size in bytes of each chunk uploaded.
    autoResume          bool to control auto resuming.
*/
function CFUpload(name, files, uploadStartFunction, progressFunction, doneFunction, errorFunction, chunkSize, autoResume) {
    // change these if you need to.
    var webapiUrl = "/api/FileUpload";
    var webapiGetFileSizeUrl = "/api/FileUpload/GetFileSize";

    // function/var refrences
    progressCallbackFunction = progressFunction;
    doneCallbackFunction = doneFunction;
    uploadStartCallbackFunction = uploadStartFunction;
    errorCallbackFunction = errorFunction;
    username = name;
    fileQueue = files;
    fileCount = files.length;
    fileIndex = 0;

    // get the first file
    file = files[fileIndex];

    // calculate number of chunks
    var numchunks = toInt(file.size / chunkSize);
    // how much is left over, if any?
    var leftover = file.size % chunkSize;

    var request;
    var position = 0;
    var chunk = 0;

    // this gets called after reading a chunk from the file
    function processChunk() {
        
        var buffer = this.result;
        //var blobstring = btoa(bytes);
        var blobstring = arrayBufferToBase64(buffer);
        //var blobstring = bin2hex2(buffer);
        //var blobstring = bin2hex2(ar);

        // create an object to pass
        var chunkObject = {
            "username": username,
            "filename": file.name,
            "numchunks": numchunks,
            "chunk": chunk,
            "position": position,
            "chunksize": chunkSize,
            "data": blobstring
        }

        // here's the async post to the webapi 
        // method to process this one chunk
        request = $.ajax({
            type: "POST",
            url: webapiUrl,
            data: JSON.stringify(chunkObject),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        // handle success and error results
        request.success(sent);
        request.error(error);
    }

    function arrayBufferToBase64(buffer) {
        var binary = ''
        var bytes = new Uint8Array(buffer)
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i])
        }
        return window.btoa(binary);
    }

    // callback from webapi post to process a chunk
    function sent(response, textStatus, jqXHR) {

        // call the progress function so the client can 
        // show progress
        updateprogress();

        // do we still have chunks (chunks) to send?
        if (chunk < numchunks) {
            chunk += 1  // increment chunk and file position
            position = chunk * chunkSize;
            // read the next chunk from the file
            var blob = file.slice(position, position + chunkSize);
            var reader = new FileReader();
            reader.onload = processChunk;   // processChunk called after reading
            reader.readAsArrayBuffer(blob);
        }
        else {
            // that was the last chunk. Is there more data?
            if (leftover != 0) {
                // read the next chunk just like above
                chunk += 1
                position = chunk * chunkSize;
                var blob = file.slice(position, position + leftover);
                leftover = 0;
                var reader = new FileReader();
                reader.onloadend = processChunk;
                reader.readAsArrayBuffer(blob);
            }
            else {

                // are there more files?
                if (fileIndex + 1 == fileCount) {
                    // that's it!
                    doneCallbackFunction();
                }
                else {

                    // get the next file
                    fileIndex += 1;
                    file = files[fileIndex];

                    // calculate number of chunks
                    numchunks = toInt(file.size / chunkSize);
                    // how much is left over, if any?
                    leftover = file.size % chunkSize;
                    // initialize variables
                    position = 0;
                    chunk = 0;

                    // do the whole thing again
                    preUploadCheck()
                }
            }
        }
    };

    // called when we get an error with ajax call to webapi
    function error(jqXHR, textStatus, errorThrown) {
        errorFunction(textStatus, errorThrown);
        // wait for 30 seconds, then try again;
        setTimeout(startupload(), 30000);
    }

    // called after retrieving the size of this file on the server
    function gotfilesize(response, textStatus, jqXHR) {
        var filesize = response;
        if (filesize != "0")
            chunk = toInt(filesize / chunkSize);
        startupload();
    }

    // called if there was an error retrieving the size of this file on the server
    function filesizeerror(jqXHR, textStatus, errorThrown) {
        errorFunction(textStatus, errorThrown);
    }

    // makeshift serializer that works on client and server with pure javascript.
    function bin2hex(data) {
        var output = "";
        for (i = 0; i < data.length; i++) {
            var b = data.charCodeAt(i).toString(16);
            if (b.length == 1)
                b = "0" + b;
            output += b;
        }
        return output;
    }

    function bin2hex2(data) {
        var output = "";
        for (i = 0; i < data.length; i++) {
            //var b = data.charCodeAt(i).toString(16);
            var b = data[i].toString(16);
            
            if (b.length == 1)
                b = "0" + b;
            output += b;
        }
        return output;
    }

    // calculates the perecent done and calls the progress function 
    function updateprogress() {
        var totalchunks = numchunks;

        if (leftover != 0)
            totalchunks += 1;

        var percent = toInt(chunk * 100 / totalchunks);

        if (percent > 100)
            percent = 100;

        progressCallbackFunction(percent);
    }

    // remove those pesky decimal points.
    function toInt(value) {
        return parseInt(value, 10);
    }

    // initializes the upload process
    function startupload() {
        // report 0 percent progress
        //progressCallbackFunction(0);
        uploadStartCallbackFunction(file);

        // read the first chunk
        var reader = new FileReader();
        reader.onloadend = processChunk; // this is where we actually start sending
        var blob = file.slice(0, chunkSize);
        reader.readAsArrayBuffer(blob);
    }

    // THIS IS ACTUALLY WHERE WE START!
    function preUploadCheck() {
        if (autoResume == true) {
            // get the file size at the server
            filesizerequest = $.ajax({
                type: "GET",
                url: webapiGetFileSizeUrl,
                data: { username: username, filename: file.name },
                dataType: "json"
            });
            // call gotfile size to process the response
            filesizerequest.success(gotfilesize);
            filesizerequest.error(filesizeerror);
        } else {
            // autoresume is false. Upload from the beginning.
            startupload();
        }
    }

    preUploadCheck();

}