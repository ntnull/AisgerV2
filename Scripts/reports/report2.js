$(function () {

    console.log('app.js');
    var qwe = {};
    window.qwe = qwe;
    createControllers();
    initVars();
    createGrid();
    fillYearGrid();
    getDicKato();
    bindEventHandlers();
    loadHtmlTemplates();
	   

    //----
    function initVars() {
        qwe.paramQuery = {};
        qwe.paramQuery.isFirst = false;
        qwe.paramQuery.oblast_id = -1;
        qwe.paramQuery.year = 2015;
    }

    //----
    function createControllers() {

        //---- создать вкладку  
        qwe.tabStrip = $('#tabstrip').kendoTabStrip({
            animation: {
                open: { effects: false }
            },
            activate: function (e) {

                //var indx = qwe.tabStrip.select().index();
                //if (indx != 0 && indx != 2 && indx != 9) {
                //    wrapper.find(".btnOpenObjectAttributes").addClass('hide');
                //} else {
                //    wrapper.find(".btnOpenObjectAttributes").removeClass('hide');
                //}

            }
        }).getKendoTabStrip();

        qwe.tabStrip.tabGroup.on("click", "[data-type='remove']", function (e) {
            e.preventDefault();
            e.stopPropagation();

            var tabItem = $(e.target).closest(".k-item");
            var item = $(e.target).closest(".k-item").addClass('hide');

            if (tabItem.index() == 1) {
                var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item1");
                qwe.tabStrip.activateTab(tabToActivate);
            } else {
                var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item2");
                qwe.tabStrip.activateTab(tabToActivate);
            }

            //            qwe.tabStrip.enable(qwe.tabStrip.tabGroup.children("li:eq("+item.index()+")"), false);

            // qwe.tabStrip.remove(item.index());
        });

        //---- меню
        $(".menu-report").kendoMenu({
            //select: onSelect22
        });

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

                Highcharts.exportCharts([qwe.pieChart, qwe.clchart1, qwe.clchart2], options);

            } else if (type === 'png') {

                Highcharts.exportCharts([qwe.pieChart, qwe.clchart1, qwe.clchart2]);
            } else if (type === 'jpg') {

                var options = {
                    type: "image/jpeg"
                }

                Highcharts.exportCharts([qwe.pieChart, qwe.clchart1, qwe.clchart2], options);

            } else if (type === "svg") {

                var options = {
                    type: "image/svg+xml"
                }

                Highcharts.exportCharts([qwe.pieChart, qwe.clchart1, qwe.clchart2], options);

            } else if (type === 'excell') {

                qwe.pieChart.exportChartLocal({ type: 'application/vnd.ms-excel' });

                var options = {
                    type: "application/vnd.ms-excel"
                }
                Highcharts.exportCharts([qwe.pieChart, qwe.clchart1, qwe.clchart2], options);
            }

        });

        $('.menu-item-refresh').click(function () {
            GetReportConsByRes7ForPie();
            GetReportConsByRes8();
        });

    	//---- вернуться
        $('.btn-back').click(function () {
        	window.location.href = "/ReportAnalyse/Index";
        })
    }

    //----
    function createGrid() {

        //----
        qwe.yearGrid = $('.tab-report2 .year-grid').kendoGrid({
            scrollable: true,
            sortable: true,
            selectable: 'row',
            columns: [
			{
			    field: "ReportYear",
			    title:L.year,
			    filterable: false,
			    sortable: false
			}],
            resizable: true,
            change: function (e) {

                if (qwe.paramQuery.isFirst) {
                    var dataItem = qwe.yearGrid.dataItem(this.select());
                    qwe.paramQuery.year = dataItem.ReportYear;

                    GetReportConsByRes7ForPie();
                    GetReportConsByRes8();
                }

            }
        }).getKendoGrid();

        //----
        qwe.regionGrid = $('.tab-report2 .region-grid').kendoGrid({
            scrollable: true,
            sortable: true,
            selectable: 'row',
            columns: [
			{
			    field: "kato_name",
			    title: L.oblast,
			    filterable: false,
			    sortable: false
			}],
            resizable: true,
            change: function (e) {

                if (qwe.paramQuery.isFirst) {

                    var dataItem = qwe.regionGrid.dataItem(this.select());
                    qwe.paramQuery.oblast_id = dataItem.kato_id;
                    qwe.paramQuery.currRegion = dataItem;

                    GetReportConsByRes7ForPie();
                    GetReportConsByRes8();
                }

            }
        }).getKendoGrid();

        resizeGrids();
    }

    //----
    function fillYearGrid() {

        dic_getDicRstReport(function () {

            qwe.dicRstReport = dic.dicRstReport;
            var isactive = 0, activeYear = 2015;
            $.map(qwe.dicRstReport, function (item, indx) {
            	if (item.isactive) {
            		isactive = indx;
            		activeYear = item.ReportYear;
            	}
            });

            qwe.paramQuery.year = activeYear;

            qwe.yearGrid.dataSource.data(qwe.dicRstReport);

            var rows = qwe.yearGrid.dataSource.data();
            var row = qwe.yearGrid.table.find("[data-uid=" + rows[isactive].uid + "]");
            qwe.yearGrid.select(row);

			//----
            GetReportConsByRes7ForPie();
            GetReportConsByRes8();

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
    function createHighCharts(data) {

        var colors = Highcharts.getOptions().colors;

        qwe.pieChart = Highcharts.chart('pie-chart', { //$('#pie-chart').highcharts({
            credits: {
                enabled: false
            },
            chart: {
                type: 'pie'
            },
            title: {
                text: L.report2CTitle
            },
            subtitle: {
                text: ''
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
                    tooltip: {
                        pointFormat: '<b>{point.percentage:.1f}%</b>'
                    },
                    events: {
                        click: function (event) {

                            var point = this;

                            qwe.tabStrip.wrapper.find('.tab-item2').removeClass('hide');
                            var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item2");
                            qwe.tabStrip.activateTab(tabToActivate);

                            console.log("event", event.point);
                            var dataItem = qwe.yearGrid.dataItem(qwe.yearGrid.select());;
                            qwe.paramQuery.year = dataItem.ReportYear;

                            qwe.paramQuery.currObject = { resource_id: event.point.id, NameRu: event.point.name };
                            initialReport2();

                        }
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
                pointFormat: '<b>{point.percentage:.1f}%</b>'
            },
            series: [{
                innerSize: '30%',
                data: data
            }],
            exporting: {
                sourceWidth: 600,
                sourceHeight: 400,
                scale: 1,
                chartOptions: {
                    chart: {
                        margin: [70, 70, 70, 70],
                        spacingTop: 0,
                        spacingBottom: 0,
                        spacingLeft: 70,
                        spacingRight: 0
                        //  backgroundColor: '#ff0000'
                    }
                }
            }
        });
    }

    //----
    function createChart2(categories, chdata) {

        //----chart 2
        qwe.clchart2 = Highcharts.chart('chart-2', {
            chart: {
                type: 'column'
            },
            credits: {
                enabled: false
            },
            title: {
                text: ''
            },
            subtitle: {
                text: ''
            },
            legend: {
                align: 'center',
                verticalAlign: 'bottom',
                layout: 'horizontal'
            },
            xAxis: {
                categories: categories,//['Apples', 'Oranges', 'Bananas'],
                labels: {
                    x: -10
                }
            },
            yAxis: {
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                },
                maxPadding: 0.002,
                minPadding: 0.0005,
                allowDecimals: true,
                title: {
                    text:L.y1000
                }
            },
            plotOptions: {
                column: {
                    events: {
                        click: function (event) {

                            qwe.tabStrip.wrapper.find('.tab-item2').removeClass('hide');
                            var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item2");
                            qwe.tabStrip.activateTab(tabToActivate);

                            var series = event.point.series;
                            qwe.paramQuery.year = series.name;

                            qwe.paramQuery.currObject = { resource_id: event.point.id, NameRu: event.point.category };
                            initialReport2();
                        }
                    }
                }
            },
            series: chdata,
            exporting: {
                sourceWidth: 400,
                sourceHeight: 350,
                scale: 1,
                chartOptions: {
                    chart: {
                        margin: [70, 70, 70, 70]
                        //  backgroundColor: '#ff0000'
                    }, //this works
                    //plotOptions: {
                    //    series: {
                    //        dataLabels: {
                    //            enabled: true
                    //        } //this one doesn't work
                    //    }
                    //}
                }
            }

        });

    }

    function createChart1(categories, chdata) {

        //---- chart 1
        qwe.clchart1 = Highcharts.chart('chart-1', {
            credits: {
                enabled: false
            },
            chart: {
                type: 'column'
            },
            title: {
                useHTML: false,
                text: ''
            },
            subtitle: {
                text: ''
            },
            legend: {
                align: 'center',
                verticalAlign: 'bottom',
                layout: 'horizontal'
            },
            plotOptions: {
                column: {
                    events: {
                        click: function (event) {

                            qwe.tabStrip.wrapper.find('.tab-item2').removeClass('hide');
                            var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item2");
                            qwe.tabStrip.activateTab(tabToActivate);

                            var series = event.point.series;
                            qwe.paramQuery.year = series.name;

                            qwe.paramQuery.currObject = { resource_id: event.point.id, NameRu: event.point.category };
                            initialReport2();
                        }
                    }
                }
            },
            xAxis: {
                categories: categories,// ['Apples', 'Oranges', 'Bananas'],
                labels: {
                    x: -10
                }
            },
            yAxis: {
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                },
                allowDecimals: true,
                title: {
                    text: L.y1000
                }
            },
            series: chdata,
            exporting: {
                sourceWidth: 200,
                sourceHeight: 350,
                scale: 1,
                chartOptions: {
                    subtitle: null
                }
            }
        });

    }

    //----pie chart backend
    function GetReportConsByRes7ForPie() {

        $.post(rootUrl + 'ReportAnalyse/GetReportConsByRes7ForPie', {
            year: qwe.paramQuery.year,
            oblast_id: qwe.paramQuery.oblast_id
        }, function (data) {

            qwe.paramQuery.isFirst = true;
            if (data.ErrorMessage)
                alert(data.ErrorMessage);

            var piedata = [];
            $.map(data.ListItems, function (item) {
                piedata.push({ id: item.resource_id, name: item.resource_name, y: item.consumption });
            });

            createHighCharts(piedata);

        });
    }

    //-----
    function GetReportConsByRes8() {

        kendo.ui.progress($('.report1'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportConsByRes1', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report1'), false);
            qwe.paramQuery.isFirst = true;
            if (data.ErrorMessage)
                alert(data.ErrorMessage);

            //----
            var resource_name = "";
            var categories = [], series = [];

			 var orderByData = new jinqJs()
                                         .from(data.ListItems)
                                         .orderBy([{ field: 'report_year'}])
                                         .select();
										 
            $.map(orderByData, function (item, index) {

                var y = decimalAdjust("round", item.consumption, '-3');

                series.push({ name: item.report_year, data: [{ id: item.resource_id, y: y }] });

                if (index == 0)
                    resource_name = item.resource_name;
            });

            var y16Count = $.grep(orderByData, function (row) { return row.report_year == 2016 }).length;
            if (y16Count == 0) {
                series.push({ name: 2016, data: [] });
            }

            categories.push(resource_name);
            createChart1(categories, series);

        });

        //----chart 22 
        kendo.ui.progress($('.report1'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportConsByRes7', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report1'), false);

            //----
            var distinctRes = new jinqJs()
                                      .from(data.ListItems)
                                      .distinct('resource_id', 'resource_name')
                                      .select();

            var categories = $.map(distinctRes, function (item) {
                return item.resource_name
            });

            //----
            var distinctYear = new jinqJs()
                                     .from(data.ListItems)
                                     .distinct('report_year')
                                     .orderBy([{ field: 'report_year' }])
                                     .select();
            var series = [];
            $.map(distinctYear, function (item) {
				
                var buffer = $.grep(data.ListItems, function (row) { return row.report_year == item.report_year; });
                var indata = [];

				
                $.map(buffer, function (row) {
                    indata.push({ id: row.resource_id, y: decimalAdjust("round", row.consumption, '-3') });
                });

                series.push({ name: item.report_year, data: indata });
                
            });

            var y16Count = $.grep(distinctYear, function (row) { return row.report_year == 2016 }).length;
            if (y16Count == 0) {
                series.push({ name: 2016, data: [] });
            }

            createChart2(categories, series);

        });
    }
    
    //============== report 2 =======
    function initialReport2() {

        GetReportTop10ByRes();
        GetReportResByRegion();

    }

    //---- chart 2 1  backend
    function GetReportTop10ByRes() {

        kendo.ui.progress($('.tab-report2 .report2'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportTop10ByRes', { resource_id: qwe.paramQuery.currObject.resource_id, year: qwe.paramQuery.year, oblastId: qwe.paramQuery.oblast_id }, function (data) {

            kendo.ui.progress($('.tab-report2 .report2'), false);

            if (data.ErrorMessage) {
                qwe.paramQuery.rt21DataSource.ListItems = [];
                rt2CreateChart1([], []);
                openAlertWindow(data.ErrorMessage);
                return;
            }

            qwe.paramQuery.rt21DataSource = data;
            rt2CalcChart1();
        });

    }

    //---- chart 2 2 backend
    function GetReportResByRegion() {

        kendo.ui.progress($('.tab-report2 .report2'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportResByRegion', { resource_id: qwe.paramQuery.currObject.resource_id, year: qwe.paramQuery.year, oblastId: qwe.paramQuery.oblast_id }, function (data) {

            kendo.ui.progress($('.tab-report2 .report2'), false);

            if (data.ErrorMessage) {
                qwe.paramQuery.rt22DataSource.ListItems = [];
                rt2CreateChart2([], []);
                openAlertWindow(data.ErrorMessage);
                return;
            }

            qwe.paramQuery.rt22DataSource = data;
            rt2CalcChart2();
        });

    }

    //----
    function rt2CreateChart1(categories, series) {

        $('#report2-bar-chart').highcharts({
            credits: {
                enabled: false
            },
            chart: {
                type: 'bar'
            },
            title: {
                text: L.top10+' ' + qwe.paramQuery.currObject.NameRu
            },
            subtitle: {
                text: L.region+':' + qwe.paramQuery.currRegion.kato_name
            },
            xAxis: {
                categories: categories
            },
            yAxis: {
                min: 0,
                title: {
                    text:L.y1000
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

                            var secUserId = event.point.id;
                            console.log("secuserId", secUserId);

                            qwe.tabStrip.wrapper.find('.tab-item3').removeClass('hide');
                            var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item3");
                            qwe.tabStrip.activateTab(tabToActivate);

                            var p = { year: qwe.paramQuery.year, secUserId: secUserId, rObject: qwe.paramQuery.reportStore };
                            qwe.FormaReport1.openWindow(p);

                            //var series = event.point.series;
                            //qwe.paramQuery.currObject = { TypeResourceId: event.point.id, NameRu: event.point.name };
                            //var index = series.index;
                            //qwe.paramQuery.year = series.name;
                            //initialReport2();
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

    //----
    function rt2CreateChart2(seriesData) {

        var colors = Highcharts.getOptions().colors;

        // Create the chart
        $('#report2-pie-chart').highcharts({
            credits: {
                enabled: false
            },
            chart: {
                type: 'pie'
            },
            title: {
                text: L.consByRegion
            },
            subtitle: {
                text: ''
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
                    tooltip: {
                        pointFormat: '<b>{point.percentage:.1f}%</b>'
                    },
                    events: {
                        click: function (event) {

                        }
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
    function rt2CalcChart1() {

        kendo.ui.progress($('.tab-report2 .report2'), true);

        setTimeout(function () {

            if (qwe.paramQuery.rt21DataSource.ListItems.length > 0) {

                //----fill chart series and categories
                var categories = [];
                var seriesData = [];
                $.map(qwe.paramQuery.rt21DataSource.ListItems, function (item) {

                    categories.push(item.subject_name);

                    var y = parseFloat(item.consumption) / 1000;
                    seriesData.push({ id: item.subject_id, y: Math.floor(y) });
                });

                var series = [{ name: '', data: seriesData }];
                rt2CreateChart1(categories, series);

                kendo.ui.progress($('.tab-report2 .report2'), false);
            } else {
                rt2CreateChart1([], []);
                kendo.ui.progress($('.tab-report2 .report2'), false);
            }
        }, 100);

    }

    //---- 
    function rt2CalcChart2() {

        kendo.ui.progress($('.tab-report2 .report2'), true);

        setTimeout(function () {

            if (qwe.paramQuery.rt22DataSource.ListItems.length > 0) {

                var seriesData = [];
                $.map(qwe.paramQuery.rt22DataSource.ListItems, function (item) {

                    var y = parseFloat(item.consumption);
                    seriesData.push({ id: item.Id, name: item.oblast_name, y: Math.round(y) });
                });

                rt2CreateChart2(seriesData);
                kendo.ui.progress($('.tab-report2 .report2'), false);

            } else {
                rt2CreateChart2([]);
                kendo.ui.progress($('.tab-report2 .report2'), false);
            }

        }, 100);

    }

    //----
    function loadHtmlTemplates() {

        //----bi report 
        templ_universal('FormaReport1', function () {
            qwe.FormaReport1 = bi.FormaReport1({
                content: templs.FormaReport1,
                wrapper: $('.bi-report-container')
            });
        });

    }

});
