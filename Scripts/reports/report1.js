$(function () {

    console.log('report1.js');
    var qwe = {};
    window.qwe = qwe;

    initVars();
    createControllers();
    createGrid();
    getDicKato();
    bindEventHandlers();
    GetReportByJurType(); 

    //----
    function initVars() {

        qwe.paramQuery = {};
        qwe.paramQuery.isFirst = false;
        qwe.paramQuery.oblast_id = -1;
        qwe.paramQuery.year = 2015;
        qwe.paramQuery.kato_name = "";
    }

    //----
    function createControllers() {

        qwe.tabStrip = $('.tab-report1').kendoTabStrip({
            animation: {
                open: { effects: false }
            },
            activate: function (e) {

            }
        }).getKendoTabStrip();

        qwe.tabStrip.tabGroup.on("click", "[data-type='remove']", function (e) {
            e.preventDefault();
            e.stopPropagation();

            var tabItem = $(e.target).closest(".k-item");

            var item = $(e.target).closest(".k-item").addClass('hide');

            if (tabItem.index() == 1) {
                var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item-report1");
                qwe.tabStrip.activateTab(tabToActivate);
            } else {
                var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item-report2");
                qwe.tabStrip.activateTab(tabToActivate);
            }

        });

        //---- меню
        $(".menu-report").kendoMenu({
            //select: onSelect22
        });

    }

    //----
    function createGrid() {

        //----
        qwe.regionGrid = $('.tab-report1 .region-grid').kendoGrid({
            scrollable: true,
            sortable: true,
            selectable: 'row',
            columns: [
			{
			    field: "kato_name",
			    title:L.oblast,
			    filterable: false,
			    sortable: false
			}],
            resizable: true,
            change: function (e) {

                if (qwe.paramQuery.isFirst) {

                    var dataItem = qwe.regionGrid.dataItem(this.select());
                    qwe.paramQuery.oblast_id = dataItem.kato_id;
					
                    qwe.paramQuery.currRegion = dataItem;
                    GetReportByJurType();
                }

            }
        }).getKendoGrid();

        resizeGrids();
    }

    //----
    function bindEventHandlers() {

        //---- меню экспорт отчета
        $('.item-export').click(function () {

            var type = $(this).attr('type');
            console.log(type);
            if (type === 'pdf') {

                var options = {
                    type: "application/pdf"
                }

                Highcharts.exportCharts([qwe.chart1], options);

            } else if (type === 'png') {

                Highcharts.exportCharts([qwe.chart1], options);
            } else if (type === 'jpg') {

                var options = {
                    type: "image/jpeg"
                }

                Highcharts.exportCharts([qwe.chart1], options);

            } else if (type === "svg") {

                var options = {
                    type: "image/svg+xml"
                }

                Highcharts.exportCharts([qwe.chart1], options);

            } else if (type === 'excell') {

                qwe.pieChart.exportChartLocal({ type: 'application/vnd.ms-excel' });

                var options = {
                    type: "application/vnd.ms-excel"
                }
                Highcharts.exportCharts([qwe.chart1], options);
            }

        });

        //---- обновить
        $('.menu-item-refresh').click(function () {
            GetReportByJurType();
        });

    	//---- вернуться
        $('.btn-back').click(function () {
        	window.location.href = "/ReportAnalyse/Index";
        });

    }

    //----
    function getDicKato() {

        dic_getDicKato(function () {

            qwe.dicKato = dic.dicKato;

            qwe.regionGrid.dataSource.data(qwe.dicKato);

            var rows = qwe.regionGrid.dataSource.data();
            var row = qwe.regionGrid.table.find("[data-uid=" + rows[0].uid + "]");
            qwe.regionGrid.select(row);
            qwe.paramQuery.currRegion = rows[0];

        });              

    }

    //----
    function GetReportByJurType() {

        kendo.ui.progress($('.report1'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportByJurType', { oblast_id: qwe.paramQuery.oblast_id}, function (data) {

            kendo.ui.progress($('.report1'), false);
            qwe.paramQuery.isFirst = true;

            //----
            var groupByData = new jinqJs()
              .from(data.ListItems)
              .groupBy("report_year")
              .sum('consumption')
              .select([{ field: 'report_year' }, { field: 'consumption' }]);
              

            var categories = [];
            var seriesData = [];
            $.map(groupByData, function (item) {

                categories.push(item.report_year);
                var y=decimalAdjust('round', parseFloat(item.consumption/1000), -3);
                seriesData.push({ y:y });
            });

            var series = [{ name: '', data: seriesData }];
            createChart(categories, series);

        });

    }

    //----
    function createChart(categories, series) {

        qwe.chart1 = Highcharts.chart('line-chart', {
            credits: {
                enabled: false
            },
            title: {
            	text: L.report1CTitle + qwe.paramQuery.currRegion.kato_name + L.report1CTitle2
            },

            subtitle: {
                text: ''
            },
            xAxis: {
                categories: categories
            },
            yAxis: {
                title: {
                    text: 'тыс. т.у.т.'
                },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                }
            },
            legend: {
                enabled: false,
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle'
            },
            plotOptions: {
                series: {
                    cursor: 'pointer',
                    point: {
                        events: {
                            click: function (e) {

                            }
                        }
                    }

                },
                line: {
                    events: {
                        click: function (e) {

                            qwe.tabStrip.wrapper.find('.tab-item-report2').removeClass('hide');
                            var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item-report2");
                            qwe.tabStrip.activateTab(tabToActivate);

                            qwe.paramQuery.currObject = { name: e.point.category };
                            qwe.paramQuery.year = e.point.category;

                            initialReport2();
                        }
                    },                   
                    tooltip: {
                        pointFormat: '<b>{point.y}</b>'
                    }
                }
            },
            series: series
        });

    }

    //===================report 2
    function initialReport2() {
        //----       
        GetReportTop10AndOthersByCons();
        GetReportTop10ByCons();
    }

    //----
    function rt2CreateChart1(seriesData) {

        var colors = Highcharts.getOptions().colors;

        // Create the chart
        Highcharts.chart('r1-report2-pie-chart', {
            credits: {
                enabled: false
            },
            chart: {
                type: 'pie'
            },
            title: {
                text: L.region+':' + qwe.paramQuery.currRegion.kato_name + " <br> "+L.year+":" + qwe.paramQuery.currObject.name
            },
            subtitle: {
                text: L.top10Other
            },
            yAxis: {
                title: {
                    text: ''
                }
            },
            plotOptions: {
                pie: {
                    shadow: false,
                    center: ['50%', '50%'],
                    showInLegend: true,
                    dataLabels: {
                        enabled: true,
                        format: '{point.percentage:.1f} %',
                        style: {
                            color: 'black'
                        }
                    },
                    events: {
                        click: function (event) {


                        }
                    },
                    tooltip: {
                        pointFormat: '<b>{point.percentage:.1f}%</b>'
                    }
                }
            },
            legend: {
                align: 'right',
                verticalAlign: 'top',
                y: 100,
                layout: 'vertical'
            },
            tooltip: {
                valueSuffix: '%'
            },
            series: [{
                innerSize: '30%',
                data: seriesData
            }]
        });
    }

    //----
    function rt2CreateChart2(categories, series) {

        Highcharts.chart('r1-report2-bar-chart', {
            credits: {
                enabled: false
            },
            chart: {
                type: 'bar'
            },
            title: {
                text: L.top10
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: categories //['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'тыс. т.у.т'
                },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                }
            },
            legend: {
                reversed: true,
                enabled: false
            },
            plotOptions: {
                bar: {
                    events: {
                        click: function (event) {

                        }
                    },
                    tooltip: {
                        pointFormat: '<b>{point.y}</b>'
                    },
                }
            },
            series: series
        });

    }
    
    //----
    function GetReportTop10ByCons() {

        kendo.ui.progress($('.report2'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportTop10ByCons', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report2'), false);

            if (data.ErrorMessage)
                openAlertWindow(data.ErrorMessage);
            
            var categories = [];
            var seriesData = [];

            $.map(data.ListItems, function (item) {

                categories.push(item.subject_name);
                var y = Math.round(parseFloat(item.consumption));
                seriesData.push({ id: item.subject_id, y: y });
            });

            var series = [{ name: '', data: seriesData }];
            rt2CreateChart2(categories, series);

        });

    }

    //----
    function GetReportTop10AndOthersByCons() {

        kendo.ui.progress($('.report2'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportTop10AndOthersByCons', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report2'), false);

            if (data.ErrorMessage)
                openAlertWindow(data.ErrorMessage);

            console.log("pea1", data);
            var piedata = [];
            $.map(data.ListItems, function (item) {

                var y = Math.round(parseFloat(item.top10_value));
                piedata.push({ name: item.top10_name, y: y });
            });

            rt2CreateChart1(piedata);


        });

    }
});