$(function () {

    console.log('report3.js');
    var qwe = {};
    window.qwe = qwe;

    initVars();
    createControllers();
    createGrid();
    getDicKato();

    bindEventHandlers();

    createChart1();
    GetReportByJurType();

    loadHtmlTemplates();

    //----
    function initVars() {
        qwe.paramQuery = {};
        qwe.paramQuery.isFirst = false;
        qwe.paramQuery.oblast_id = -1;
        qwe.paramQuery.year = 2015;

        //----
        qwe.paramQuery.resYear = [{ name: 2012 }, { name: 2013 }, { name: 2014 }, { name: 2015 }];

        //---- 
        qwe.paramQuery.resTypeApply = [{ Id: 1, NameRu: 'Юридические лица', code: 'юр' }, { Id: 2, NameRu: 'Квазигосударственный сектор', code: 'кв' }, { Id: 3, NameRu: 'Государственные учреждения', code: 'гу' }];
        qwe.paramQuery.jurTypeApply = [{ Id: 1, NameRu: 'Юридические лица', code: 'юр' }, { Id: 2, NameRu: 'Квазигосударственный сектор', code: 'кв' }, { Id: 3, NameRu: 'Государственные учреждения', code: 'гу' }];

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

        qwe.tabStrip = $('.tab-report3').kendoTabStrip({
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
    function createGrid() {

        //----область
        qwe.regionGrid = $('.tab-report3 .region-grid').kendoGrid({
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
    function createChart1(categories, series) {

        qwe.chart1 = Highcharts.chart('report3-item1-chart1', {
            chart: {
                type: 'bar'
            },
            title: {
                text: L.report3
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: categories,//['Africa', 'America', 'Asia', 'Europe', 'Oceania'],
                title: {
                    text: null
                }
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
                },
                allowDecimals: true
            },
            tooltip: {
                valueSuffix: ''
            },
            plotOptions: {
                bar: {
                    events: {
                        click: function (event) {


                            qwe.tabStrip.wrapper.find('.tab-item2').removeClass('hide');
                            var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item2");
                            qwe.tabStrip.activateTab(tabToActivate);

                            var points = event.point;
                            qwe.paramQuery.year = points.category;

                            qwe.paramQuery.currObject = { id: points.id, name:points.series.name };
                            console.log(qwe.paramQuery.currObject);
                            initialReport2();
                        }
                    }
                }
            },
            legend: {
                align: 'center',
                verticalAlign: 'bottom',
                layout: 'horizontal'
            },
            credits: {
                enabled: false
            },
            series: series
        });

    }

    //----
    function createChart2(categories, series) {

        qwe.chart2 = Highcharts.chart('report3-item1-chart2', {
            chart: {
                type: 'bar'
            },
            title: {
                text:L.report2
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: categories,//['Africa', 'America', 'Asia', 'Europe', 'Oceania'],
                title: {
                    text: null
                }
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
                },
                allowDecimals: true
            },
            tooltip: {
                valueSuffix: ''
            },
            plotOptions: {
                bar: {
                    events: {
                        click: function (event) {

                            qwe.tabStrip.wrapper.find('.tab-item2').removeClass('hide');
                            var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item2");
                            qwe.tabStrip.activateTab(tabToActivate);

                            var points = event.point;

                            qwe.paramQuery.currObject = { id: points.id, name: series.name };
                            console.log(qwe.paramQuery.currObject);

                            initialReport2();
                        }
                    }
                }
            },
            legend: {
                align: 'center',
                verticalAlign: 'bottom',
                layout: 'horizontal'
            },
            credits: {
                enabled: false
            },
            series: series
        });

    }

    //----
    function GetReportByJurType() {

        kendo.ui.progress($('.report1'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportByJurType', { oblast_id: qwe.paramQuery.oblast_id }, function (data) {

            kendo.ui.progress($('.report1'), false);
            if (data.ErrorMessage) {
                openAlertWindow(data.ErrorMessage);
            }

            qwe.paramQuery.isFirst = true;

            dic_getDicRstReport(function () {

                qwe.dicRstReport = dic.dicRstReport;
                var categories = [];
                var series1 = [];
                var series2 = [];
                $.map(qwe.paramQuery.jurTypeApply, function (item, index) {

                    var buffer = $.grep(data.ListItems, function (row) { return row.jur_type_id == item.Id; });
                    var sdata1 = [], sdata2 = [];
                    //---- order by 

                    var orderByData = new jinqJs()
                          .from(buffer)
                          .orderBy([{ field: 'report_year', sort: 'desc' }])
                          .select();

                    var jur_type_id = 0;
                    var jur_type_name_full = "";
                    $.map(orderByData, function (inItem) {

                        sdata1.push({ id: inItem.jur_type_id, y: inItem.subject_count });
                        sdata2.push({ id: inItem.jur_type_id, y: Math.round(inItem.consumption) });

                        jur_type_name_full = inItem.jur_type_name_full;

                        if (index == 0)
                            categories.push(inItem.report_year);

                    });

                    series1.push({ name: jur_type_name_full, data: sdata1 });
                    series2.push({ name: jur_type_name_full, data: sdata2 });
                });

                createChart1(categories, series1);
                createChart2(categories, series2);
            });

        });

    }
    
    //==========================================================================report3 - item2
    //----
    function initialReport2() {
        GetReportTop10ByJurType();
    }

    //----
    function GetReportTop10ByJurType() {

        //----       
        kendo.ui.progress($('.report2'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportTop10ByJurType', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year, jur_type_id: qwe.paramQuery.currObject.id }, function (data) {

            kendo.ui.progress($('.report2'), false);

            if (data.ErrorMessage) {
                openAlertWindow(data.ErrorMessage);
            }

            var categories = [], seriesData = [];

            $.map(data.ListItems, function (item) {

                categories.push(item.subject_name);
                var y = Math.round(parseFloat(item.consumption / 1000));
                seriesData.push({ id: item.subject_id, y: y });

            });

            var series = [{ name: '', data: seriesData }];

            rt2CreateChart1(categories, series);

        });

    }

    //----
    function rt2CreateChart1(categories, series) {

        Highcharts.chart('report3-item2-chart1', {
            credits: {
                enabled: false
            },
            chart: {
                type: 'bar',
                height: 500
            },
            title: {
                text:L.top10
            },
            subtitle: {
                text: L.formofownership + ':' + qwe.paramQuery.currObject.name + ' <br> '+L.region+':' + qwe.paramQuery.currRegion.kato_name
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

                            console.log("eventes", event.point);

                            qwe.tabStrip.wrapper.find('.tab-item3').removeClass('hide');
                            var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item3");
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
            /*
            series: [{
                name: 'John',
                data:[5, 3, 4, 7, 2]
            }]*/
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

});