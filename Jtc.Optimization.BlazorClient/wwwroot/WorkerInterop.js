window.WorkerInterop = {   

    worker: null,

    runWorker: (code) => {
        if (typeof (Worker) !== "undefined") {

            if (typeof (worker) === "undefined") {
                worker = new Worker("worker.js");
                console.log("Worker was initialized.");
            }

            worker.onmessage = function (event) {
                console.log("Worker sent:" + event.data);
                workerCallback.invokeMethodAsync(workerMethodname, { error: event.data });
            };

        }
        else {
            //todo: fall back to ui thread
            document.getElementsByTagName("body").innerHTML += "Sorry, your browser is not supported.";
        }

        worker.postMessage([code, workerCallback]);
    },

    workerCallback: null,

    workerMethodname: null,

    setWorkerCallback: (reference, methodName) => {
        workerCallback = reference;
        workerMethodname = methodName;
    }

};

