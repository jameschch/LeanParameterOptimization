window.JSInterop = {


    ReadUploadedFileAsText: (inputFile) => {
        const temporaryFileReader = new FileReader();

        return new Promise((resolve, reject) => {
            temporaryFileReader.onerror = () => {
                temporaryFileReader.abort();
                reject(new DOMException("Problem parsing input file."));
            };

            temporaryFileReader.onload = () => {
                resolve(temporaryFileReader.result);
            };
            temporaryFileReader.readAsText(inputFile);
        });
    },

    GetFileData: async () => {
        const file = document.getElementById("fileUpload").files[0];

        try {
            const fileContents = await JSInterop.ReadUploadedFileAsText(file);
            return fileContents;
        }
        catch (e) {
            console.warn(e.message);
        }
    },

    DownloadConfig: (json) => {
        blob = new Blob([json], { type: "octet/stream" }), url = window.URL.createObjectURL(blob);

        var save = document.getElementById("save");
        save.href = url;

        // target filename
        save.download = 'optimization.json';
    },

};

