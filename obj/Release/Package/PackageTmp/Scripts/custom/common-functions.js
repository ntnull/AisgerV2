var CommonFunctions = {
    Home: {
        CommonFunctions: {}
    },

    DateTimeHelper: {
        setDefaults: function() {
            // For todays date;
            Date.prototype.today = function() {
                return ((this.getDate() < 10) ? "0" : "") + this.getDate() + "."
                    + (((this.getMonth() + 1) < 10) ? "0" : "") + (this.getMonth() + 1) + "."
                    + this.getFullYear();
            };

            // For the time now
            Date.prototype.timeNow = function() {
                return ((this.getHours() < 10) ? "0" : "") + this.getHours() + ":"
                    + ((this.getMinutes() < 10) ? "0" : "") + this.getMinutes() + ":"
                    + ((this.getSeconds() < 10) ? "0" : "") + this.getSeconds();
            };

            $.fn.datepicker.defaults = $.extend($.fn.datepicker.defaults, {
                language: 'ru',
                autoclose: true
                //format: 'dd.mm.yyyy'
                //, weekStart: 1
            });
        }, // end of setDefaults

        getCurrentDate: function() {
            var currentTime = new Date();
            return currentTime.today();
        }, // end of getCurrentDate

        getDatePicker: function(el) {
            $(el).datepicker({ dateFormat: 'dd.mm.yy' });
        } // end of getDatePicker
    },

    DropDownListHelper: {
        changeItemFn: function(controlId, url, childControlId) {
            if ($(controlId).val() != "") {
                var options = {};
                options.url = url;
                options.type = "POST";
                options.data = JSON.stringify({ code: $(controlId).val() });
                options.datatype = "json";
                options.contentType = "application/json";
                options.success = function(lineList) {
                    $(childControlId).empty();
                    for (var i = 0; i < lineList.length; i++) {
                        var option = "<option value=\"" + lineList[i].Value + "\"" + (lineList[i].Selected == true ? " selected=\"selected\"" : "") + ">" + lineList[i].Text + "</option>";
                        $(childControlId).append(option);
                    }
                };
                options.error = function() { alert("Error in Getting!"); };
                $.ajax(options);
            } else {
                $(childControlId).empty();
            }
        }, //end of changeItemFn
        initFn: function(controlId, url, param) {
            var options = {};
            options.url = url;
            options.type = "POST";
            options.data = JSON.stringify({ code: param });
            options.datatype = "json";
            options.contentType = "application/json";
            options.success = function(lineList) {
                $(controlId).empty();
                for (var i = 0; i < lineList.length; i++) {
                    var option = "<option value=\"" + lineList[i].Value + "\"" + (lineList[i].Selected == true ? " selected=\"selected\"" : "") + ">" + lineList[i].Text + "</option>";
                    $(controlId).append(option);
                }
            };
            options.error = function() { alert("Error in Getting!"); };
            $.ajax(options);
        }
    },

    HighChartHelper: {
        intervalId: null,
        initDarkThemeFn: function() {
            Highcharts.theme = {
                colors: [
                    "#2b908f", "#90ee7e", "#f45b5b", "#7798BF", "#aaeeee", "#ff0066", "#eeaaee",
                    "#55BF3B", "#DF5353", "#7798BF", "#aaeeee"
                ],
                chart: {
                    backgroundColor: {
                        linearGradient: { x1: 0, y1: 0, x2: 1, y2: 1 },
                        stops: [
                            [0, '#2a2a2b'],
                            [1, '#3e3e40']
                        ]
                    },
                    style: {
                        fontFamily: "'Unica One', sans-serif"
                    },
                    plotBorderColor: '#606063'
                },
                title: {
                    style: {
                        color: '#E0E0E3',
                        textTransform: 'uppercase',
                        fontSize: '20px'
                    }
                },
                subtitle: {
                    style: {
                        color: '#E0E0E3',
                        textTransform: 'uppercase'
                    }
                },
                xAxis: {
                    gridLineColor: '#707073',
                    labels: {
                        style: {
                            color: '#E0E0E3'
                        }
                    },
                    lineColor: '#707073',
                    minorGridLineColor: '#505053',
                    tickColor: '#707073',
                    title: {
                        style: {
                            color: '#A0A0A3'

                        }
                    }
                },
                yAxis: {
                    gridLineColor: '#707073',
                    labels: {
                        style: {
                            color: '#E0E0E3'
                        }
                    },
                    lineColor: '#707073',
                    minorGridLineColor: '#505053',
                    tickColor: '#707073',
                    tickWidth: 1,
                    title: {
                        style: {
                            color: '#A0A0A3'
                        }
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.85)',
                    style: {
                        color: '#F0F0F0'
                    }
                },
                plotOptions: {
                    series: {
                        dataLabels: {
                            color: '#B0B0B3'
                        },
                        marker: {
                            lineColor: '#333'
                        }
                    },
                    boxplot: {
                        fillColor: '#505053'
                    },
                    candlestick: {
                        lineColor: 'white'
                    },
                    errorbar: {
                        color: 'white'
                    }
                },
                legend: {
                    itemStyle: {
                        color: '#E0E0E3'
                    },
                    itemHoverStyle: {
                        color: '#FFF'
                    },
                    itemHiddenStyle: {
                        color: '#606063'
                    }
                },
                credits: {
                    style: {
                        color: '#666'
                    }
                },
                labels: {
                    style: {
                        color: '#707073'
                    }
                },

                drilldown: {
                    activeAxisLabelStyle: {
                        color: '#F0F0F3'
                    },
                    activeDataLabelStyle: {
                        color: '#F0F0F3'
                    }
                },

                navigation: {
                    buttonOptions: {
                        symbolStroke: '#DDDDDD',
                        theme: {
                            fill: '#505053'
                        }
                    }
                },

                // scroll charts
                rangeSelector: {
                    buttonTheme: {
                        fill: '#505053',
                        stroke: '#000000',
                        style: {
                            color: '#CCC'
                        },
                        states: {
                            hover: {
                                fill: '#707073',
                                stroke: '#000000',
                                style: {
                                    color: 'white'
                                }
                            },
                            select: {
                                fill: '#000003',
                                stroke: '#000000',
                                style: {
                                    color: 'white'
                                }
                            }
                        }
                    },
                    inputBoxBorderColor: '#505053',
                    inputStyle: {
                        backgroundColor: '#333',
                        color: 'silver'
                    },
                    labelStyle: {
                        color: 'silver'
                    }
                },

                navigator: {
                    handles: {
                        backgroundColor: '#666',
                        borderColor: '#AAA'
                    },
                    outlineColor: '#CCC',
                    maskFill: 'rgba(255,255,255,0.1)',
                    series: {
                        color: '#7798BF',
                        lineColor: '#A6C7ED'
                    },
                    xAxis: {
                        gridLineColor: '#505053'
                    }
                },

                scrollbar: {
                    barBackgroundColor: '#808083',
                    barBorderColor: '#808083',
                    buttonArrowColor: '#CCC',
                    buttonBackgroundColor: '#606063',
                    buttonBorderColor: '#606063',
                    rifleColor: '#FFF',
                    trackBackgroundColor: '#404043',
                    trackBorderColor: '#404043'
                },

                // special colors for some of the
                legendBackgroundColor: 'rgba(0, 0, 0, 0.5)',
                background2: '#505053',
                dataLabelsColor: '#B0B0B3',
                textColor: '#C0C0C0',
                contrastTextColor: '#F0F0F3',
                maskColor: 'rgba(255,255,255,0.3)'
            };

            // Apply the theme
            Highcharts.setOptions(Highcharts.theme);
        },
        initColumnChartFn: function (containerId, url, urlParam) {
            $.getJSON(url, urlParam, function (data) {
                var highChartSetting = data;
                console.log("column chart data=", data);

                var chart = {
                    type: 'column',
                    backgroundColor: '#2d353c',
                };

                var title = {
                    text: highChartSetting.chartTitle
                };

                var subtitle = {
                    text: highChartSetting.chartSubtitle
                };

                var xAxis = {
                    categories: highChartSetting.categories,
                    crosshair: true,
                };

                var yAxis = {
                    title: {
                        text: highChartSetting.yTitle
                    }
                };

                // ?
                var series = highChartSetting.series;

                var credits = {
                    enabled: false
                };

                var plotOptions = {
                    column: {
                        pointPadding: 0.1,
                        borderWidth: 0
                    }
                };

                var tooltip = {
                    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                        '<td style="padding:0"><b>{point.y:.2f} {series.unit}</b></td></tr>',
                    footerFormat: '</table>',
                    shared: true,
                    useHTML: true
                };

                var exporting = {
                    enabled: false
                };

                var jsonHighChartSettings = {};
                jsonHighChartSettings.chart = chart;
                jsonHighChartSettings.title = title;
                jsonHighChartSettings.subtitle = subtitle;
                jsonHighChartSettings.xAxis = xAxis;
                jsonHighChartSettings.yAxis = yAxis;
                jsonHighChartSettings.tooltip = tooltip;
                jsonHighChartSettings.series = series;
                jsonHighChartSettings.plotOptions = plotOptions;
                jsonHighChartSettings.credits = credits;
                jsonHighChartSettings.exporting = exporting;

                $('#' + containerId).highcharts(jsonHighChartSettings);
            });
        },
        initPieChartFn: function (containerId, url, urlParam, containerStatId) {
            $.getJSON(url, urlParam, function (data) {
                var pieGraphParameter = data;
                var highChartSetting = pieGraphParameter.HighchartSettings;

                var chart = {
                    type: 'pie',
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    backgroundColor: '#242a30',
                };

                var title = {
                    text: highChartSetting.chartTitle
                };

                var series = highChartSetting.series;

                var credits = {
                    enabled: false
                };

                var plotOptions = {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: false
                        },
                        showInLegend: true
                    }
                };

                var tooltip = {
                    pointFormat: '{series.name}: <b>{point.percentage:.2f}%</b>'
                };

                var exporting = {
                    enabled: false
                };

                var jsonHighChartSettings = {};
                jsonHighChartSettings.chart = chart;
                jsonHighChartSettings.title = title;
                jsonHighChartSettings.tooltip = tooltip;
                jsonHighChartSettings.series = series;
                jsonHighChartSettings.plotOptions = plotOptions;
                jsonHighChartSettings.credits = credits;
                jsonHighChartSettings.exporting = exporting;

                $('#'+ containerId).highcharts(jsonHighChartSettings);

                if (pieGraphParameter.StatData) {
                    var stat = '<ul class="stat-list">';
                    for (var i = 0; i < pieGraphParameter.StatData.length; i++) {
                        stat += '<li>';
                        stat += '<h4 class="no-margins">';
                        stat += pieGraphParameter.StatData[i].Value;
                        stat += '</h4>';
                        stat += '<div class="stat-percent">';
                        stat += pieGraphParameter.StatData[i].ValuePercent;
                        stat += '</div>';
                        stat += '<small>';
                        stat += pieGraphParameter.StatData[i].Title;
                        stat += '</small>';
                        stat += '</li>';
                    }
                    
                    stat += '</ul>';

                    $('#' + containerStatId).html(stat);
                }
                    

            });
        },
        initSplineChartFn: function (containerId, url, urlParam) {
            $.getJSON(url, urlParam, function (data) {
                var highChartSetting = data;

                var chart = {
                    type: 'spline',
                    animation: Highcharts.svg, // don't animate in old IE
                    marginRight: 10,
                    events: {
                        load: function () {
                            // set up the updating of the chart each second
                            var series = this.series[0];
                            setInterval(function () {

                                var x = (new Date()).getTime(), // current time
                                    y = Math.random();
                                series.addPoint([x, y], true, true);
                            }, 60*1000);

                        }
                    }
                };

                var title = {
                    text: highChartSetting.chartTitle
                };

                var subtitle = {
                    text: highChartSetting.chartSubtitle
                };

                var xAxis = {
                    type: 'datetime',
                    tickPixelInterval: 150
                };

                var yAxis = {
                    title: {
                        text: highChartSetting.yTitle
                    },
                    plotLines: [{
                        value: 0,
                        width: 1,
                        color: '#808080'
                    }]
                };

                var series = highChartSetting.series;

                var credits = {
                    enabled: false
                };

                var plotOptions = {
                    column: {
                        pointPadding: 0.1,
                        borderWidth: 0
                    }
                };

                var tooltip = {
                    formatter: function () {
                        return '<b>' + this.series.name + '</b><br/>' +
                            Highcharts.dateFormat('%d:%m:%Y %H:%M:%S', this.x) + '<br/>' +
                            Highcharts.numberFormat(this.y, 2);
                    }
                };

                var exporting = {
                    enabled: false
                };

                var legend = {
                    enabled: false
                };

                var jsonHighChartSettings = {};
                jsonHighChartSettings.chart = chart;
                jsonHighChartSettings.title = title;
                jsonHighChartSettings.subtitle = subtitle;
                jsonHighChartSettings.xAxis = xAxis;
                jsonHighChartSettings.yAxis = yAxis;
                jsonHighChartSettings.tooltip = tooltip;
                jsonHighChartSettings.series = series;
                jsonHighChartSettings.plotOptions = plotOptions;
                jsonHighChartSettings.credits = credits;
                jsonHighChartSettings.exporting = exporting;
                jsonHighChartSettings.legend = legend;

                $('#' + containerId).highcharts(jsonHighChartSettings);
            });
        },
        initAdditionalSplineChartFn: function (containerId, url, urlParam, collectDataUrl) {

            /**
             * In order to synchronize tooltips and crosshairs, override the
             * built-in events with handlers defined on the parent element.
             */
            $('#' + containerId).bind('mousemove touchmove touchstart', function (e) {
                var chart,
                    point,
                    i,
                    event;

                for (i = 0; i < Highcharts.charts.length; i = i + 1) {
                    chart = Highcharts.charts[i];

                    if (chart == null || typeof chart.container === "undefined" || chart.container == null || chart.container.offsetParent == null)
                        continue;
                    else if (chart.container.offsetParent.id != containerId)
                        continue;

                    event = chart.pointer.normalize(e.originalEvent); // Find coordinates within the chart
                    point = chart.series[0].searchPoint(event, true); // Get the hovered point

                    if (point) {
                        point.highlight(e);
                    }
                }
            });

            /**
            * Override the reset function, we don't need to hide the tooltips and crosshairs.
            */
            Highcharts.Pointer.prototype.reset = function () {
                return undefined;
            };

            /**
             * Highlight a point by showing tooltip, setting hover state and draw crosshair
             */
            Highcharts.Point.prototype.highlight = function (event) {
                this.onMouseOver(); // Show the hover marker
                this.series.chart.tooltip.refresh(this); // Show the tooltip
                this.series.chart.xAxis[0].drawCrosshair(event, this); // Show the crosshair
            };
            
            /**
             * Synchronize zooming through the setExtremes event handler.
             */
            function syncExtremes(e) {
                var thisChart = this.chart;

                if (e.trigger !== 'syncExtremes') { // Prevent feedback loop
                    Highcharts.each(Highcharts.charts, function (chart) {
                        if (chart !== thisChart) {
                            if (chart.xAxis[0].setExtremes) { // It is null while updating
                                chart.xAxis[0].setExtremes(e.min, e.max, undefined, false, { trigger: 'syncExtremes' });
                            }
                        }
                    });
                }
            }


            $.getJSON(url, urlParam, function (realtimeGraphParameter) {
                // window.updateSchemaData(realtimeGraphParameter.AccountingActualPerLine);

                $.each(realtimeGraphParameter.HighchartSettings.series, function (i, series) {
//                $.each(realtimeGraphParameter.Datasets, function (i, dataset) {
//                    debugger;
                    // Add X values
//                    dataset.data = Highcharts.map(dataset.data, function(val, j) {
//                        return [realtimeGraphParameter.XData[j], val];
//                    });

                    $('<div class="chart">')
                        .appendTo('#' + containerId)
                        .highcharts({
                            chart: {
                                type: 'spline',
                                animation: Highcharts.svg,
                                marginLeft: 60, // Keep all charts left aligned
                                spacingTop: 20,
                                spacingBottom: 20,
                                backgroundColor: '#2d353c',
                                events: {
                                    load: function() {
                                // function for update data
                                    }
                                }
                            },
                            title: {
                                text: series.name, //dataset.name
                                align: 'left',
                                margin: 0,
                                x: 30
                            },
                            credits: {
                                enabled: false
                            },
                            legend: {
                                enabled: false
                            },
                            
                            xAxis: {
                                type: 'datetime',
                                categories: realtimeGraphParameter.HighchartSettings.categories,
                                crosshair: true,
                                events: {
                                    setExtremes: syncExtremes
                                },
                                
                                //tickPixelInterval: 150,
//                                labels: {
//                                    format: '{value}'
//                                },
                            },
                            yAxis: {
                                title: {
                                    text: null
                                }
                            },
                            tooltip: {
                                positioner: function() {
                                    return {
                                        x: this.chart.chartWidth - this.label.width, // right aligned
                                        y: -1 // align to title
                                    };
                                },
                                borderWidth: 0,
                                backgroundColor: 'none',
                                pointFormat: '{point.y}',
                                headerFormat: '',
                                shadow: false,
                                style: {
                                    fontSize: '18px'
                                },
                                valueDecimals: 3
                            },
                            exporting: {
                                enabled: false
                            },
                            series: [
                                {
                                    data: series.data, // dataset.data
                                    name: series.name, // dataset.name
                                    type: 'spline',
                                    // animation: Highcharts.svg,
                                    color: Highcharts.getOptions().colors[0],
                                    fillOpacity: 0.3,
                                    tooltip: {
                                        valueSuffix: ' ' + series.unit // dataset.unit
                                    }
                                }
                            ]
                        });
                });
            });


            function startCollectData() {
                $.getJSON(collectDataUrl, urlParam, function (rlData) {
                    // empty
                    if (!rlData)
                        return;

                    // window.updateSchemaData(rlData.AccountingActualPerLine);
                    for (i = 0; i < Highcharts.charts.length; i = i + 1) {
                        var chart = Highcharts.charts[i];

                        if (chart.container.offsetParent == null || (chart.container.offsetParent.id != containerId
                            && $(chart.container.offsetParent).attr("class") != "chart"))
                            continue;

                        for (var j = 0; j < rlData.Datasets.length; j++) {
                            if (rlData.Datasets[j].name == chart.title.textStr) {
                                chart.series[0].addPoint([rlData.Datasets[j].xdata, rlData.Datasets[j].ydata], true, true);
                            }
                        }
                    }
                });
            }

            if (this.intervalId != null)
                clearInterval(this.intervalId);

            this.intervalId = setInterval(function () {
                startCollectData();
            }, 60 * 1000);
        },
        clearInterval: function() {
            if (this.intervalId != null)
                clearInterval(this.intervalId);
        }
    }
};