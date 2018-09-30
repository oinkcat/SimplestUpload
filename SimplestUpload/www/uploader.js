// Client interactivity
(function (w, d) {

    var FILELIST_URL = '/list';
    var UPLOAD_URL = '/upload';
    var DOWNLOAD_URL = '/download';

    var fileSource = null;

    // Prompt user to select file to upload
    function promptForFile() {
        fileSource.click();
    }

    // Upload selected file to server
    function uploadFile() {
        var selectedFile = this.files[0];

        var uploader = new XMLHttpRequest();
        uploader.upload.onprogress = uploadProgress;
        uploader.onreadystatechange = readyStateChanged;

        uploader.open('POST', UPLOAD_URL, true);
        var formData = new FormData();
        formData.append('file', selectedFile);
        uploader.send(formData);
    }

    function uploadProgress(e) {
        console.log('Progress: ' + e.loaded + '/' + e.total);
    }

    function readyStateChanged(e) {
        var xhr = e.srcElement;

        if (xhr.readyState == xhr.DONE) {
            fetchFileList();
        }
    }

    // Show list of uploaded files
    function showFileList(files) {
        var listFragment = d.createDocumentFragment();

        // Create HTML list elements for each file
        for (var i in files) {
            var fileInfo = files[i];
            var listItem = d.createElement('li');
            var fileLink = d.createElement('a');
            fileLink.href = DOWNLOAD_URL + '/' + fileInfo.id;
            fileLink.textContent = fileInfo.name;
            listItem.appendChild(fileLink);
            listFragment.appendChild(listItem);
        }

        // Replace contents of file list
        var container = d.getElementById('fileList');
        container.innerHTML = '';
        container.appendChild(listFragment);
    }

    // Fetch uploaded files list from server
    function fetchFileList() {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', FILELIST_URL, true);

        xhr.onreadystatechange = function () {
            // Show uploaded files
            if (xhr.readyState == xhr.DONE) {
                var fileList = JSON.parse(xhr.responseText);
                showFileList(fileList);
            }
        };

        xhr.send(null);
    }

    // Initialize client uploader features
    function initialize() {
        // Click to select a file
        var uploadLink = d.querySelector('.upload-link a');
        uploadLink.addEventListener('click', promptForFile);

        // Upload file immediately after select by user
        fileSource = d.getElementById('uploadSource');
        fileSource.addEventListener('change', uploadFile);

        // Request file list from server
        fetchFileList();
    }

    initialize();
})(window, document);