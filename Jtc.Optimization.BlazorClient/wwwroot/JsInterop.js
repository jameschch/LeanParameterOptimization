window.JSInterop = {


    Eval: function (script) {
        eval(script);
    },

    //GetFileData: function () {
    //var reader = new FileReader();
    //var data;
    //reader.onload = function(e) {          
    //    data = e.target.result;
    //};

    //reader.readAsText();

    
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
    }


};

