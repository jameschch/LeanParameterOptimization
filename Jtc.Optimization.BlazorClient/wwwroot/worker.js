onmessage = function (e) {

    console.log('Message received:' + e.data);

    var result = eval(e.data);

    this.postMessage(result);
};