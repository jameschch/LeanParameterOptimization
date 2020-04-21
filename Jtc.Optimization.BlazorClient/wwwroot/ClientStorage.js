window.ClientStorage = {

    storeConfig: (data) => {
        localStorage.setItem("config", data);
    },

    fetchConfig: () => {
        return localStorage.getItem("config");
    },

    storeChartData: (data) => {
        localStorage.setItem("chartData", data);
    },

    fetchChartData: () => {
        return localStorage.getItem("chartData");
    },

    hasChartData: () => {
        return localStorage.getItem("chartData") && localStorage.getItem("chartData").length > 0;
    }

};

