window.WorkerInterop = {

    instance: null,

    runWorker: (code) => {
        if (typeof (Worker) !== "undefined") {

            if (typeof (instance) === "undefined") {
                instance = new Worker("worker.js");
                //console.log("Worker was initialized.");
            }

            instance.onmessage = function (event) {
                //console.log("Worker sent:" + event.data);
                workerCallback.invokeMethodAsync(workerMethodname, event.data);
            };

        }
        else {
            //todo: fall back to ui thread
            document.getElementsByTagName("body").innerHTML += "Sorry, your browser is not supported.";
        }

        instance.postMessage(code);
    },

    workerCallback: null,

    workerMethodname: null,

    setWorkerCallback: (reference, methodName) => {
        workerCallback = reference;
        workerMethodname = methodName;
    }

};

