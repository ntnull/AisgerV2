$(function () {

    console.log('report6.js');
    var qwe = {};
    window.qwe = qwe;

    initVars();
    createControllers();
    bindEventHandlers();
    createGrid();
    fillYearGrid();
    getDicKato();
    GetReportEffecSecial();

    //----
    function initVars() {
        qwe.paramQuery = {};
        qwe.paramQuery.isFirst = false;
        qwe.paramQuery.oblast_id = -1;
        qwe.paramQuery.year = 2015;
        qwe.paramQuery.year2 = 2015;

        qwe.paramQuery.dicSocial = ["ЗДРАВООХРАНЕНИЕ И СОЦИАЛЬНЫЕ УСЛУГИ", "ИСКУССТВО, РАЗВЛЕЧЕНИЯ И ОТДЫХ", "ОБРАЗОВАНИЕ"];
    }

    //----
    function createControllers() {

        //---- создать вкладку  
        qwe.tabStrip = $('#tabstrip').kendoTabStrip({
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
            GetReportEffecSecial();
        });

    	//---- вернуться
        $('.btn-back').click(function () {
        	window.location.href = "/ReportAnalyse/Index";
        });
    }

    //----
    function createGrid() {

        //----
        qwe.yearGrid = $('.tab-report6 .year-grid').kendoGrid({
            scrollable: true,
            sortable: true,
            selectable: 'row',
            columns: [
			{
			    field: "ReportYear",
			    title: L.year,
			    filterable: false,
			    sortable: false
			}],
            resizable: true,
            change: function (e) {

                if (qwe.paramQuery.isFirst) {
                    var dataItem = qwe.yearGrid.dataItem(this.select());
                    qwe.paramQuery.year = dataItem.ReportYear;

                    GetReportEffecSecial();
                }

            }
        }).getKendoGrid();

        //----
        qwe.regionGrid = $('.tab-report6 .region-grid').kendoGrid({
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

                    GetReportEffecSecial();
                }

            }
        }).getKendoGrid();

        //----
        qwe.objectGrid = $('.tab-report6 .report-grid').kendoGrid({
            scrollable: false,
            sortable: true,
            selectable: 'row',
            //height:772,
            columns: [
			{
			    field: "subject_name",
			    title:"Деятельность",
			    filterable: false,
			    sortable: false
			}, {
			    field: "idk",
			    title: "ИДК",
			    filterable: false,
			    sortable: false
			}, {
			    field: "oked_name",
			    title: "Название субъекта",
			    filterable: false,
			    sortable: false
			}, {
			    field: "energy_value",
			    title: "Показатель",
			    filterable: false,
			    sortable: false
			}],
            resizable: true,
            change: function (e) {


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
        });

    }

    //----
    function createChart(categories, chdata) {

        //----chart 2
        qwe.clchart = Highcharts.chart('object-chart', {
            chart: {
                type: 'column'
            },
            credits: {
                enabled: false
            },
            title: {
                text: 'Показатели энергоэффективности социальных объектов'
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
                    text: L.y1000
                }
            },
            plotOptions: {
                column: {
                    events: {
                        click: function (event) {

                            qwe.tabStrip.wrapper.find('.tab-item2').removeClass('hide');
                            var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item2");
                            qwe.tabStrip.activateTab(tabToActivate);

                            GetReportFlatEffecSecial();
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
                    }
                }
            }

        });

    }

    function GetReportEffecSecial() {

        kendo.ui.progress($('.report1'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportEffecSecial', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report1'), false);

            qwe.paramQuery.isFirst = true;
            if (data.ErrorMessage)
                alert(data.ErrorMessage);

            //----
            var resource_name = "";
            var categories = [];

            var minData = [], maxData = [], avgData = [];

            var min_sum = 0, avg_sum = 0, max_sum = 0;
            $.map(qwe.paramQuery.dicSocial, function (item) {

                var bufferStore = $.grep(data.ListItems, function (row) { return row.oked_name == item; });
                min_sum = 0, avg_sum = 0, max_sum = 0;
                $.map(bufferStore, function (row) {
                    min_sum += row.min_value;
                    max_sum += row.max_value;
                    avg_sum += row.avg_value;
                });

                minData.push(decimalAdjust("round",min_sum,-3));
                avgData.push(decimalAdjust("round", avg_sum, -3));
                maxData.push(decimalAdjust("round", max_sum, -3));

            });
            
            var series = [{ name: 'Минимальное', data: minData }, { name: 'Среднее', data: avgData }, { name: 'Максимальное', data: maxData }];
            console.log("seri", series);
            createChart(qwe.paramQuery.dicSocial, series);
        });
    }


    function GetReportFlatEffecSecial() {

        kendo.ui.progress($('.report1'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportFlatEffecSecial', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report1'), false);
            console.log("data=", data);
            qwe.objectGrid.dataSource.data(data.ListItems);
        });

    }
})