window.MainInterop = {


    readUploadedFileAsText: (inputFile) => {
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

    getFileData: async () => {
        const file = document.getElementById("fileUpload").files[0];

        try {
            const fileContents = await MainInterop.readUploadedFileAsText(file);
            return fileContents;
        }
        catch (e) {
            console.warn(e.message);
        }
    },

    downloadConfig: (json) => {
        blob = new Blob([json], { type: "octet/stream" }), url = window.URL.createObjectURL(blob);

        var save = document.getElementById("save");
        save.href = url;

        // target filename
        save.download = 'optimization.json';
    },

    activityLogChanged: (e) => {
        e.target.scrollTop(Number.MAX_SAFE_INTEGER);
    },

    storeConfig: (data) => {
        sessionStorage.setItem("config", data);
    },

    fetchConfig: (data) => {
        return sessionStorage.getItem("config");
    }

};

