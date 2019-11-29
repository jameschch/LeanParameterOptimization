window.PlotlyInterop = {

    layout: function (isDark) {
        return {
            "margin": {
                "b": 0,
                "l": 30,
                "t": 20,
                "r": 30
            },
            "plot_bgcolor": isDark ? "#000" : "#fff",
            "paper_bgcolor": isDark ? "#000" : "#fff",
            // "family": "Roboto, Helvetica, Arial, sans-serif",
            "font": { "color": isDark ? "#fff" : "#000" },
            "xaxis": {
                "showspikes": true,
                //"domain": [0, 1],
                "automargin": true,
                // "title": []
            },
            "yaxis": {
                "showspikes": true,
                // "domain": [0, 1],
                "automargin": true,
                // "title": []
            },
            "hovermode": "compare",
            "showlegend": true
        };

    },

    selectedLayout: function () {
        return this.layout(true);
    },

    toggleTheme: function (foreground, background) {
        jQuery("body").css("background-color", background);
        jQuery("h2, h3, body, p").css("color", foreground);
        jQuery("h1").css("color", "#000");
        Plotly.relayout("graph", PlotlyInterop.selectedLayout());
    },

    newPlot: function (data) {
        console.log(data);
        Plotly.newPlot(document.getElementById("graph"), data, PlotlyInterop.selectedLayout(), { displayModeBar: true, showSendToCloud: false, displaylogo: false });

        return true;
    }
};