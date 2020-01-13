window.pool = workerpool.pool();

window.WorkerPoolInterop = {

    methodName: null,

    //todo: needed?
    callBackRegistry: [],

    runWorker: (dotNetreference, methodName, code, key) => {
        if (typeof (Worker) !== "undefined") {

            WorkerPoolInterop.methodName = methodName;

            //console.log("Worker thread commenced.");

            WorkerPoolInterop.callBackRegistry[dotNetreference._id] = dotNetreference;

            pool.exec(WorkerPoolInterop.executeEval, [dotNetreference._id, code, key])
                .then(function (result) {
                    //console.log("Worker thread completed.");
                    WorkerPoolInterop.callBackRegistry[result[0]].invokeMethodAsync(methodName, result[2], result[1]);
                })
                .catch(function (err) {
                    console.error(err);
                    dotNetreference.invokeMethodAsync(methodName, -10, key);
                });
        }
        else {
            //todo: fall back to ui thread
            document.getElementsByTagName("body").innerHTML += "Sorry, your browser is not supported.";
        }
    },

    executeEval: function (callbackId, code, key) {
        return [callbackId, eval(code), key];
    }
};



