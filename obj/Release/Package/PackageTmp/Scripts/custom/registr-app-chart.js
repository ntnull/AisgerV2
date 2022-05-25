

function buildChart(param, contenername, urlPoints, title, code, kind) {
    $.ajax({
        url: urlPoints,
        type: 'POST',
        data: param,
        dataType: 'json',
        cache: 'false',
        success: function (data) {
            console.log(data.drilldown);

            drawChart(contenername, data.mains, data.drilldown, title, code, kind);
        },
        error: function (data) {
            alert("1Error" + data);
        }
    });;
}

function drawChart(contenername, data, drilldown, title, code, kind) {
    var format = "";
    if (kind == 'column') {
        format = '{point.y}';
    }
    if (kind == 'pie') {
        format = '{point.name}: {point.y}';
        //        format = '{point.name}: {point.y}';
    }
    $('#' + contenername).highcharts({
        chart: {
            type: kind
        },

        title: {
            text: title
        },

        xAxis: {
            type: 'category'
        },
        yAxis: {
            title: {
                text: 'Общее кол-во'
            }

        },
        legend: {
            enabled: false
        },
        plotOptions: {
            series: {
                borderWidth: 0,
                dataLabels: {
                    enabled: true,
                    format: format
                }
            }

        },

        tooltip: {
            headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
            pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y}</b> <br/>'
        },

        series: [{
            name: code,
            colorByPoint: true,
            data: data
        }],
        drilldown: {
            series: drilldown
        }
    });
}