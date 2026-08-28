async function downloadFileFromStream(fileName, contentStreamReference) {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);

    const url = URL.createObjectURL(blob);

    triggerFileDownload(fileName, url);

    URL.revokeObjectURL(url);
}

function triggerFileDownload(fileName, url) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;

    if (fileName) {
        anchorElement.download = fileName;
    }

    anchorElement.click();
    anchorElement.remove();
}

function mySnackBar() {
    var x = document.getElementById("snackbar");
    x.className = "show";
    setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
}

function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    ev.dataTransfer.setData("text", ev.target.id);
    
}

function drop(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    ev.target.appendChild(document.getElementById(data));
}

async function showSuccessMessage() {
    alert("Transaction submission was successful!");
}

window.BlazorDownloadFile = (filename, contentBase64) => {
    const link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + contentBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

window.downloadFile = (bytes, filename) => {
    const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
};

window.checkingPriceDraftGet = (key) => sessionStorage.getItem(key);
window.checkingPriceDraftSet = (key, value) => sessionStorage.setItem(key, value);
window.checkingPriceDraftRemove = (key) => sessionStorage.removeItem(key);
