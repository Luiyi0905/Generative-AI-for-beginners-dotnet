window.pdfInterop = {
    pdfDoc: null,

    loadPdf: async function (bytes) {
        const uint8Array = new Uint8Array(bytes);
        const loadingTask = pdfjsLib.getDocument({ data: uint8Array });
        this.pdfDoc = await loadingTask.promise;
        console.log('PDF loaded');
    },

    renderPage: async function (pageNum) {
        if (!this.pdfDoc) return;

        const page = await this.pdfDoc.getPage(pageNum);
        const viewport = page.getViewport({ scale: 1.0 });
        const canvas = document.getElementById('pdf-canvas');
        const context = canvas.getContext('2d');

        // Adjust scale to fit the viewer width
        const viewer = document.getElementById('pdf-viewer');
        const scale = (viewer.clientWidth - 40) / viewport.width;
        const scaledViewport = page.getViewport({ scale: scale });

        canvas.height = scaledViewport.height;
        canvas.width = scaledViewport.width;

        const renderContext = {
            canvasContext: context,
            viewport: scaledViewport
        };
        await page.render(renderContext).promise;
    }
};

window.saveAsFile = function (fileName, byteBase64) {
    var link = document.createElement('a');
    link.download = fileName;
    link.href = "data:application/octet-stream;base64," + byteBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
