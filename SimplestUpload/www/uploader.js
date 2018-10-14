// Client interactivity
(function (w, d) {

    var FILELIST_URL = '/list';
    var UPLOAD_URL = '/upload';
    var DOWNLOAD_URL = '/download';
    var DELETE_URL = '/delete/'

    // DOM elements
    var fileSource = null;
    var uploadLinkBlock = null;
    var uploadProgressBlock = null;
    var uploadBar = null;

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

        toggleUploadState(true);
    }

    // Toggle UI elements visibility
    function toggleUploadState(uploading) {
        if (uploading) {
            uploadLinkBlock.classList.add('hidden');
            uploadProgressBlock.classList.remove('hidden');
            uploadBar.style.width = '0%';
        } else {
            uploadLinkBlock.classList.remove('hidden');
            uploadProgressBlock.classList.add('hidden');
        }
    }

    function uploadProgress(e) {
        console.log('Progress: ' + e.loaded + '/' + e.total);
        var percent = Math.round(e.loaded / e.total * 100);
        uploadBar.style.width = percent + '%';
    }

    function readyStateChanged(e) {
        var xhr = e.srcElement;

        if (xhr.readyState == xhr.DONE) {
            fetchFileList();
            toggleUploadState(false);
        }
    }

    // Get human-readable size of file
    function getReadableFileSize(sizeInBytes) {
        var units = ['B', 'KB', 'MB', 'GB'];

        var size = sizeInBytes;
        var unitIdx = 0;

        while (size > 1024) {
            size = Math.floor(size / 1024);
            unitIdx++;
        }

        return size + ' ' + units[unitIdx];
    }

    // Show list of uploaded files
    function showFileList(files) {
        var listFragment = d.createDocumentFragment();

        // Create HTML list elements for each file
        for (var i in files) {
            var fileInfo = files[i];
            var listItem = d.createElement('li');
            // Download link
            var fileLink = d.createElement('a');
            fileLink.href = DOWNLOAD_URL + '/' + fileInfo.id;
            fileLink.textContent = fileInfo.name;
            listItem.appendChild(fileLink);
            // File size label
            var sizeElem = d.createElement('span');
            sizeElem.textContent = getReadableFileSize(fileInfo.size);
            var sizeContainer = d.createElement('div');
            sizeContainer.className = 'size-container right';
            sizeContainer.appendChild(sizeElem);
            // Delete link
            var delLink = d.createElement('a');
            delLink.href = 'javascript:void(0)';
            delLink.innerHTML = '&times;';
            var deleteFunc = deleteFile.bind(this, fileInfo.id);
            delLink.addEventListener('click', deleteFunc);
            var delContainer = d.createElement('div');
            delContainer.className = 'del-container right';
            delContainer.appendChild(delLink);
            // Append in reverse order
            listItem.appendChild(delContainer);
            listItem.appendChild(sizeContainer);
            listFragment.appendChild(listItem);
        }

        // Replace contents of file list
        var container = d.getElementById('fileList');
        container.innerHTML = '';
        container.appendChild(listFragment);
    }

    // Send delete request
    function deleteFile(id) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', DELETE_URL + id);

        xhr.onreadystatechange = function () {
            if (xhr.readyState == xhr.DONE) {
                fetchFileList();
            }
        };

        xhr.send();
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
        // Blocks and upload bar
        uploadLinkBlock = d.getElementById('uploadButton');
        uploadProgressBlock = d.getElementById('uploadIndicator');
        uploadBar = d.getElementById('bar');

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