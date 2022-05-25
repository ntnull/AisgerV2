$(function () {

    console.log('report1.js');
    var qwe = {};
    window.qwe = qwe;

    initVars();
    createControllers();
    createGrid();
    getDicKato();
    bindEventHandlers();
    GetReportEE2();
    createChart([], []);

    //----
    function initVars() {

        qwe.paramQuery = {};
        qwe.paramQuery.isFirst = false;
        qwe.paramQuery.oblast_id = -1;
        qwe.paramQuery.year = 2015;

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
			    title: L.oblast,
			    filterable: false,
			    sortable: false
			}],
            resizable: true,
            change: function (e) {

                if (qwe.paramQuery.isFirst) {

                    var dataItem = qwe.regionGrid.dataItem(this.select());
                    qwe.paramQuery.oblast_id = dataItem.kato_id;

                    GetReportEE2();
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
            GetReportEE2();
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
            qwe.paramQuery.isFirst = true;
        });

    }

    //----
    function GetReportEE2() {

        kendo.ui.progress($('.report1'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportEE2', { oblast_id: qwe.paramQuery.oblast_id }, function (data) {

            kendo.ui.progress($('.report1'), false);

            if (data.ErrorMessage) {
                alert(data.ErrorMessage);
                createChart([], []);
                return;
            }

            var orderByData = new jinqJs()
                   .from(data.ListItems)
                   .orderBy([{ field: 'summa', sort: 'desc' }])
                   .select();

            var categories = [];
            var series = [];
            $.map(data.Categories, function (item, indx) {

                var filter;
                if (indx == 0) {
                    filter = $.map(orderByData, function (row) { return row.oked1; });
                    categories = $.map(orderByData, function (row) { return row.oblast_name; });
                }

                if (indx == 1)
                    filter = $.map(orderByData, function (row) { return row.oked2; });

                if (indx == 3)
                    filter = $.map(orderByData, function (row) { return row.oked3; });

                if (indx == 4)
                    filter = $.map(orderByData, function (row) { return row.oked4; });


                series.push({ name: item.oked_name, data: filter });
            });

            console.log("categories", categories);
            createChart(categories, series);

            //$.map(data.ListItems, function (item) {
            //    item.consumption = Math.round(item.consumption);
            //    item.cons_prev = Math.round(item.cons_prev);
            //    item.dynamic = Math.round(item.dynamic);
            //});
            //qwe.reportGrid7.dataSource.data(data.ListItems);

        });

    }

    //----
    function createChart(categories, chdata) {

        //----chart 2
        qwe.chart1 = Highcharts.chart('chart-1', {
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
                    text: L.y1000
                }
            },
            plotOptions: {
                column: {
                    events: {
                        click: function (event) {

                           
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
});