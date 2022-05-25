
$(function () {

    console.log('report4.js');
    var qwe = {};
    window.qwe = qwe;

    initVars();
    createControllers();
    createGrid();
    bindEventHandlers();
    fillYearGrid();
    getDicKato();


    loadHtmlTemplates();

    function initVars() {

        qwe.paramQuery = {};
        qwe.paramQuery.isFirst = false;
        qwe.paramQuery.oblast_id = -1;
        qwe.paramQuery.year = 2015;

        //----
        qwe.paramQuery.resYear = [{ name: 2012 }, { name: 2013 }, { name: 2014 }, { name: 2015 }];

        //---- 
        qwe.paramQuery.resTypeApply = [{ id: 1, NameRu: 'Юридические лица', code: 'юр' }, { id: 2, NameRu: 'Квазигосударственный сектор', code: 'кв' }, { id: 3, NameRu: 'Государственные учреждения', code: 'гу' }];

        //---- 
        qwe.paramQuery.resDict = [{ NameRu: "Газ нефтяной попутный", id: 17 },
                               { NameRu: "Уголь каменный", id: 10 },
                               { NameRu: "Газ природный", id: 9 },
                               { NameRu: "Дизельное топливо", id: 6 },
                               { NameRu: "Теплоэнергия", id: 2 },
                               { NameRu: "Электроэнергия", id: 1 },
                               { NameRu: "Прочие", id: 0 }];

    }

    //----
    function createControllers() {

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

            if (type === 'pdf') {

                var options = {
                    type: "application/pdf"
                }

                Highcharts.exportCharts([qwe.chart1, qwe.chart2], options);

            } else if (type === 'png') {

                Highcharts.exportCharts([qwe.chart1, qwe.chart2], options);
            } else if (type === 'jpg') {

                var options = {
                    type: "image/jpeg"
                }

                Highcharts.exportCharts([qwe.chart1, qwe.chart2], options);

            } else if (type === "svg") {

                var options = {
                    type: "image/svg+xml"
                }

                Highcharts.exportCharts([qwe.chart1, qwe.chart2], options);

            } else if (type === 'excell') {

                qwe.pieChart.exportChartLocal({ type: 'application/vnd.ms-excel' });

                var options = {
                    type: "application/vnd.ms-excel"
                }
                Highcharts.exportCharts([qwe.chart1, qwe.chart2], options);
            }

        });

        //---- обновить
        $('.menu-item-refresh').click(function () {
            GetReportTop10ByCons();
            GetReportTop10AndOthersByCons();
        });

    	//---- вернуться
        $('.btn-back').click(function () {
        	window.location.href = "/ReportAnalyse/Index";
        });
    }

    //----
    function createGrid() {

        //----
        qwe.yearGrid = $('.tab-report4 .year-grid').kendoGrid({
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
                    GetReportTop10AndOthersByCons();
                    GetReportTop10ByCons();
                }

            }
        }).getKendoGrid();

        //----
        qwe.regionGrid = $('.tab-report4 .region-grid').kendoGrid({
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

                    GetReportTop10AndOthersByCons();
                    GetReportTop10ByCons();
                }
            }
        }).getKendoGrid();

        resizeGrids();
    }

    //----
    function fillYearGrid() {

        dic_getDicRstReport(function () {

            qwe.dicRstReport = dic.dicRstReport;

            qwe.yearGrid.dataSource.data(qwe.dicRstReport);

            var rows = qwe.yearGrid.dataSource.data();
            var row = qwe.yearGrid.table.find("[data-uid=" + rows[3].uid + "]");
            qwe.yearGrid.select(row);

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

            GetReportTop10ByCons();
            GetReportTop10AndOthersByCons();

            qwe.paramQuery.isFirst = true;
        });

    }

    //----
    function GetReportTop10ByCons() {

        kendo.ui.progress($('.report1'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportTop10ByCons', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report1'), false);

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
            CreateChart2(categories, series);

        });

    }

    //----
    function GetReportTop10AndOthersByCons() {

        kendo.ui.progress($('.report1'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportTop10AndOthersByCons', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report1'), false);

            if (data.ErrorMessage)
                openAlertWindow(data.ErrorMessage);

            var piedata = [];
            $.map(data.ListItems, function (item) {

                var y = Math.round(parseFloat(item.top10_value));
                piedata.push({ name: item.top10_name, y: y });
            });

            CreateChart1(piedata);

        });

    }

    //----
    function CreateChart1(seriesData) {

        var colors = Highcharts.getOptions().colors;

        // Create the chart
        qwe.chart1 = Highcharts.chart('report4-item1-chart1', {
            credits: {
                enabled: false
            },
            chart: {
                type: 'pie'
            },
            title: {
                text: L.region+' :' + qwe.paramQuery.currRegion.kato_name + '<br>' +
                      L.year+' :' + qwe.paramQuery.year
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
    function CreateChart2(categories, series) {

        qwe.chart2 = Highcharts.chart('report4-item1-chart2', {
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

                            qwe.tabStrip.wrapper.find('.tab-item2').removeClass('hide');
                            var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item2");
                            qwe.tabStrip.activateTab(tabToActivate);

                            var p = { year: qwe.paramQuery.year, secUserId: event.point.id };
                            qwe.FormaReport1.openWindow(p);

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
    function loadHtmlTemplates() {

        //----bi report 
        templ_universal('FormaReport1', function () {
            qwe.FormaReport1 = bi.FormaReport1({
                content: templs.FormaReport1,
                wrapper: $('.bi-report-container')
            });
        });


    }
})