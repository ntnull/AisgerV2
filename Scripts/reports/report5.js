$(function () {

    console.log('report5.js');
    var qwe = {};
    window.qwe = qwe;

    initVars();
    createControllers();
    createGrid();
    fillYearGrid();
    bindEventHandlers();
  

    //----
    function initVars() {
        qwe.paramQuery = {};
        qwe.paramQuery.isFirst = false;
        qwe.paramQuery.oblastId = 1;
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

        //---- вкладка
        qwe.tabStrip = $('.tab-report5').kendoTabStrip({
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
        });
    }

    //----
    function createGrid() {

        //----год
        qwe.yearGrid = $('.tab-report5 .year-grid').kendoGrid({
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
                    GetReportConsByOked();
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
            GetReportConsByOked();
        });

    	//---- вернуться
        $('.btn-back').click(function () {
        	window.location.href = "/ReportAnalyse/Index";
        });
    }

    //----
    function fillYearGrid() {

        dic_getDicRstReport(function () {

            qwe.dicRstReport = dic.dicRstReport;
            console.log("dicRstReport", qwe.dicRstReport);
            qwe.yearGrid.dataSource.data(qwe.dicRstReport);

            var rows = qwe.yearGrid.dataSource.data();
            var row = qwe.yearGrid.table.find("[data-uid=" + rows[3].uid + "]");
            qwe.yearGrid.select(row);
            GetReportConsByOked();

        });

    }

    //----
    function GetReportConsByOked() {

        kendo.ui.progress($('.report1'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportConsByOked', { year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report1'), false);

            if (data.ErrorMessage) {
                alert(data.ErrorMessage);
                createChart([], []);
                return;
            }

            qwe.paramQuery.isFirst = true;

            var series = [];
            var data1 = [], data2 = [];
            var categories = [];
            $.map(data.ListItems, function (item) {

                var y1 = decimalAdjust('round',parseFloat((item.subject_count * 100) / item.count_all),-1);
                var y2 = decimalAdjust('round',parseFloat((item.consumption * 100) / item.consumption_all),-1);

                data1.push({ id: item.oked_root_id, y: y1 });
                data2.push({ id: item.oked_root_id, y: y2 });

                categories.push(item.oked_name);
            });

            series.push({ name: L.valumeConsGER + ', %', color: '#F2596B', data: data2 });
            series.push({ name: L.countConsGER + ', %', color: '#7CB5EC', data: data1 });
      
            createChart(categories, series);

        });

    }

    //----
    function createChart(categories, series) {

        qwe.chart1 = Highcharts.chart('report5-item1-chart1', {
            credits: {
                enabled: false
            },
            chart: {
                type: 'bar'
            },
            title: {
                text: L.report5
            },
            xAxis: {
                categories: categories //['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                }
            },
            legend: {
                reversed: true
            },
            plotOptions: {
                series: {
                    stacking: 'percent',
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        color: '#000000',
                        format: '{point.y:.1f}%'
                    }
                },
                bar: {
                    tooltip: {
                        pointFormat: '<b>{point.y}%</b>'
                    }
                }
              
            },
            series: series,
            exporting: {
                //sourceWidth: 400,
                fallbackToExportServer: true,
                sourceHeight: 750,
                scale: 1,
                chartOptions: {
                    chart: {
                        margin: [70, 70, 70, 70]
                    }

                }
            }
        });

    }

});