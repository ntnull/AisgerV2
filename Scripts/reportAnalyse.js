$(function () {

    console.log('index.js');
    var qwe = {};
    window.qwe = qwe;

    initVars();
    bindEventHandlers();
    GetReportByJurType1();

    GetReportConsByRes1();

    GetReportByJurType();
    GetReportTop10ByCons();
    GetReportConsByOked();
    GetReportEffecSecial(); // report 6

    grid7();
    GetReportMoreThan100();
    GetReportEE2();
    report8Map();
    report9Map();

    function initVars() {

        qwe.paramQuery = {};
        qwe.paramQuery.oblast_id = -1;
        qwe.paramQuery.year = 2015;
        qwe.paramQuery.jurTypeApply = [{ Id: 1, NameRu: 'Юридические лица', code: 'юр' }, { Id: 2, NameRu: 'Квазигосударственный сектор', code: 'кв' }, { Id: 3, NameRu: 'Государственные учреждения', code: 'гу' }];
        //---for report 6
        qwe.paramQuery.dicSocial = ["ЗДРАВООХРАНЕНИЕ И СОЦИАЛЬНЫЕ УСЛУГИ", "ИСКУССТВО, РАЗВЛЕЧЕНИЯ И ОТДЫХ", "ОБРАЗОВАНИЕ"];
    }

    function bindEventHandlers() {

        //---- report 1
        $('.r1').click(function () {
            window.location.href = rootUrl + "ReportAnalyse/Report1";
        });

        //---- report 2
        $('.r2').click(function () {
            window.location.href = rootUrl + "ReportAnalyse/Report2";
        })

        //---- report 3
        $('.r3').click(function () {
            window.location.href = rootUrl + "ReportAnalyse/Report3";
        });

        //---- report 4
        $('.r4').click(function () {
            window.location.href = rootUrl + "ReportAnalyse/Report4";
        })

        //---- report 5
        $('.r5').click(function () {
            window.location.href = rootUrl + "ReportAnalyse/Report5";
        })

        //---- report 6
        $('.r6').click(function () {
            window.location.href = rootUrl + "ReportAnalyse/Report6";
        })

        //---- report 7
        $('.r7').click(function () {
            window.location.href = rootUrl + "ReportAnalyse/Report7";
        })

        //---- report 8
        $('.r8').click(function () {
            window.location.href = rootUrl + "ReportAnalyse/Report8";
        })

        //---- report 9
        $('.r9').click(function () {
            window.location.href = rootUrl + "ReportAnalyse/Report9";
        })
        
        //---- report 9
        $('.r10').click(function () {
            window.location.href = rootUrl + "ReportAnalyse/Report10";
        })
    }

    //---- report 1
    function GetReportByJurType1() {

        kendo.ui.progress($('.report1'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportByJurType', { oblast_id: -1 }, function (data) {
            kendo.ui.progress($('.report1'), false);
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
                var y = decimalAdjust('round', parseFloat(item.consumption / 1000), -3);
                seriesData.push({ y: y });
            });

            var series = [{ name: '', data: seriesData }];
            chart1(categories, series);

        });

    }

    function chart1(categories, seriesdata) {

        Highcharts.chart('container1', {
            chart: { type: 'line', backgroundColor: '#FFF' },
            navigation: {
                buttonOptions: {
                    enabled: false
                }
            },
            colors: ['blue'],
            title: { text: '' },
            xAxis: { categories: categories },
            yAxis: {
                title: { text: 'тыс. т.у.т.' },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                }
            },
            tooltip: { enabled: false },
            credits: { enabled: false },
            plotOptions: {
                series: {
                    cursor: 'pointer',
                    lineWidth: 3,
                    marker: { radius: 0 }
                },
                line: { showInLegend: false }
            },
            series: seriesdata
            //series: [{
            //    name: 'Installation',
            //    data: [43934, 52503, 57177, 69658, 97031, 119931, 137133, 154175]
            //}]
        });

    }

    //---- report2
    function GetReportConsByRes1() {

    	kendo.ui.progress($('.report2'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportConsByRes1', { oblast_id: -1, year: 2015 }, function (data) {

        	kendo.ui.progress($('.report2'), false);

            //----
            var resource_name = "";
            var categories = [], series = [];

            $.map(data.ListItems, function (item, index) {

                var y = decimalAdjust("round", item.consumption, '-3');

                series.push({ name: item.report_year, data: [{ y: y }] });

                if (index == 0)
                    resource_name = item.resource_name;
            });

            categories.push(resource_name);

            chart21(categories, series);

        });

        //----chart 22 
        $.post(rootUrl + 'ReportAnalyse/GetReportConsByRes7', { oblast_id: -1, year: 2015 }, function (data) {

            if (data.ErrorMessage)
                alert(data.ErrorMessage);

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
                                     .select();
            var series = [];

            $.map(distinctYear, function (item) {

                var buffer = $.grep(data.ListItems, function (row) { return row.report_year == item.report_year; });
                var indata = $.map(buffer, function (row) { return row.consumption; });
                series.push({ name: item.report_year, data: indata });
            });

            chart22(categories, series);
            // chart10(categories,series);
        });
    }

    function chart21(categories1, series1) {

        //----
        var $container1 = $('.chart21');

        Highcharts.chart($container1[0], {
            chart: { type: 'column', backgroundColor: '#FFF' },
            navigation: {
                buttonOptions: {
                    enabled: false
                }
            },
            title: { text: '' },
            xAxis: { categories: categories1 },
            yAxis: {
                title: { text: 'тыс. т.у.т.' },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                }
            },
            tooltip: { enabled: false },
            credits: { enabled: false },
            plotOptions: {
                series: {
                    cursor: 'pointer',
                    lineWidth: 3,
                    marker: { radius: 0 }
                },
                column: { showInLegend: false }
            },
            series: series1
        });

    }
    function chart22(categories, series) {
        //----
        var $container2 = $('.chart22');
        Highcharts.chart($container2[0], {
            chart: { type: 'column', backgroundColor: '#FFF' },
            navigation: {
                buttonOptions: {
                    enabled: false
                }
            },
            title: { text: '' },
            xAxis: {
                categories: categories,
                labels: {
                    style: { "fontSize": "8px" },
                    reserveSpace: true,
                    autoRotation: [1],
                    staggerLines: 1
                },
                tickInterval: 1,
                showFirstLabel: true
            },
            yAxis: {
                title: { text: 'тыс. т.у.т.' },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                },
                step: 2000
            },
            tooltip: { enabled: false },
            credits: { enabled: false },
            plotOptions: {
                series: {
                    cursor: 'pointer',
                    dataLabels: {
                        style: { "color": "contrast", "fontSize": "5px", "fontWeight": "bold", "textOutline": "1px 1px contrast" }
                    }
                },
                column: { showInLegend: false }
            },
            series: series
        });
    }

    //---- report3
    function GetReportByJurType() {

        kendo.ui.progress($('.report3'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportByJurType', { oblast_id: -1 }, function (data) {

            kendo.ui.progress($('.report3'), false);
            if (data.ErrorMessage) {
                openAlertWindow(data.ErrorMessage);
            }

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

            chart31(categories, series1);
            chart32(categories, series2);
        });

    }

    function chart31(categories, series) {

        //----
        var $container = $('.chart31');

        Highcharts.chart($container[0], {
            chart: { type: 'bar', backgroundColor: '#FFF' },
            navigation: {
                buttonOptions: {
                    enabled: false
                }
            },
            title: { text: '' },
            xAxis: { categories: categories },
            yAxis: {
                title: { text: '' },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                }
            },
            tooltip: { enabled: false },
            credits: { enabled: false },
            plotOptions: {
                series: {
                    cursor: 'pointer',
                    lineWidth: 3,
                    marker: { radius: 0 }
                },
                bar: { showInLegend: false }
            },
            series: series
        });

    }
    function chart32(categories, series) {
        //----
        var $container2 = $('.chart32');
        Highcharts.chart($container2[0], {
            chart: { type: 'bar', backgroundColor: '#FFF' },
            navigation: {
                buttonOptions: {
                    enabled: false
                }
            },
            title: { text: '' },
            xAxis: {
                categories: categories,
                labels: {
                    style: { "fontSize": "8px" },
                    reserveSpace: true,
                    autoRotation: [1],
                    staggerLines: 1
                },
                tickInterval: 1,
                showFirstLabel: true
            },
            yAxis: {
                title: { text: '' },
                labels: {
                    formatter: function () {
                        return this.value;
                    },
                    style: { "fontSize": "8px" },
                    reserveSpace: true,
                    autoRotation: [1],
                    staggerLines: 1
                }

            },
            tooltip: { enabled: false },
            credits: { enabled: false },
            plotOptions: {
                series: {
                    cursor: 'pointer',
                    lineWidth: 3,
                    marker: { radius: 0 }
                },
                bar: { showInLegend: false }
            },
            series: series
        });
    }

    //---- report4
    function GetReportTop10ByCons() {

        kendo.ui.progress($('.report4'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportTop10ByCons', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report4'), false);

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
            chart4(categories, series);

        });

    }

    function chart4(categories, series) {

        var $container = $('.chart4');
        Highcharts.chart($container[0], {
            chart: { type: 'bar', backgroundColor: '#FFF' },
            navigation: {
                buttonOptions: {
                    enabled: false
                }
            },
            title: { text: '' },
            xAxis: { categories: categories },
            yAxis: {
                title: { text: '' },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                }
            },
            tooltip: { enabled: false },
            credits: { enabled: false },
            plotOptions: {
                series: {
                    cursor: 'pointer',
                    lineWidth: 3,
                    marker: { radius: 0 }
                },
                bar: { showInLegend: false }
            },
            series: series
        });
    }

    //---- report5
    function GetReportConsByOked() {

        kendo.ui.progress($('.report5'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportConsByOked', { year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report5'), false);

            if (data.ErrorMessage) {
                alert(data.ErrorMessage);
                chart5([], []);
                return;
            }

            qwe.paramQuery.isFirst = true;

            var series = [];
            var data1 = [], data2 = [];
            var categories = [];
            $.map(data.ListItems, function (item) {

                var y1 = parseFloat((item.subject_count * 100) / item.count_all);
                var y2 = parseFloat((item.consumption * 100) / item.consumption_all);

                data1.push({ id: item.oked_root_id, y: y1 });
                data2.push({ id: item.oked_root_id, y: y2 });

                categories.push(item.oked_name);
            });

            series.push({ name: 'Объем потребления ГЭР, %', color: '#F2596B', data: data2 });
            series.push({ name: 'Количество субъектов ГЭР, %', color: '#7CB5EC', data: data1 });

            chart5(categories, series);

        });

    }

    function chart5(categories, series) {

        var $container = $('.chart5');
        Highcharts.chart($container[0], {
            chart: { type: 'bar', backgroundColor: '#FFF' },
            navigation: {
                buttonOptions: {
                    enabled: false
                }
            },
            title: { text: '' },
            xAxis: { categories: categories },
            yAxis: {
                title: { text: '' },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                }
            },
            tooltip: { enabled: false },
            credits: { enabled: false },
            plotOptions: {
                series: {
                    stacking: 'percent',
                    cursor: 'pointer'
                },
                bar: { showInLegend: false }
            },
            series: series
        });

    }

    //---- report6
    function GetReportEffecSecial() {

        kendo.ui.progress($('.report6'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportEffecSecial', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report6'), false);

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

                minData.push(min_sum);
                avgData.push(avg_sum);
                maxData.push(max_sum);

            });

            var series = [{ name: 'Минимальное', data: minData }, { name: 'Среднее', data: avgData }, { name: 'Максимальное', data: maxData }];
            chart6(qwe.paramQuery.dicSocial, series);
        });
    }

    function chart6(categories,series) {

        var $container = $('.chart6');
        Highcharts.chart($container[0], {
            chart: { type: 'column', backgroundColor: '#FFF' },
            navigation: {
                buttonOptions: {
                    enabled: false
                }
            },
            title: { text: '' },
            xAxis: {
                categories: categories,
                labels: {
                    style: { "fontSize": "8px" },
                    reserveSpace: true,
                    autoRotation: [1],
                    staggerLines: 1
                },
                tickInterval: 1,
                showFirstLabel: true
            },
            yAxis: {
                title: { text: 'тыс. т.у.т.' },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                },
                step: 2000
            },
            tooltip: { enabled: false },
            credits: { enabled: false },
            plotOptions: {
                series: {
                    cursor: 'pointer',
                    dataLabels: {
                        style: { "color": "contrast", "fontSize": "5px", "fontWeight": "bold", "textOutline": "1px 1px contrast" }
                    }
                },
                column: { showInLegend: false }
            },
            series: series
        });
    }

    //---- report7
    function GetReportMoreThan100() {

        kendo.ui.progress($('.report7'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportMoreThan100', { oblast_id: qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report7'), false);

            if (data.ErrorMessage) {
                alert(data.ErrorMessage);
                qwe.reportGrid.dataSource.data([]);
                return;
            }

            $.map(data.ListItems, function (item) {
                item.consumption = Math.round(item.consumption);
                item.cons_prev = Math.round(item.cons_prev);
                item.dynamic = Math.round(item.dynamic);
            });
            qwe.reportGrid7.dataSource.data(data.ListItems);

        });

    }

    function grid7() {

        //---- report grid         
        qwe.reportGrid7 = $('.grid7').kendoGrid({
            scrollable: true,
            sortable: false,
            selectable: false,
            height: 250,
            columns: [
                {
                    //  field: "RNUMBER",
                    title: "№",
                    width: 65,
                    filterable: false,
                    template: '#= ++record #',
                    sortable: false
                },
			    {
			        field: "subject_name",
			        title: "Название субъекта",
			        template: '<a class="a-list" obj-id="#=data.subject_id#"> #=data.subject_name #</a>',
			        filterable: false,
			        sortable: false
			    }, {
			        field: "consumption",
			        title: "Потребление (тыс. т.у.т.)",
			        //template:'',
			        filterable: false,
			        sortable: false
			    }, {
			        field: "cons_prev",
			        title: "Потребление в пред. году (тыс.тут)",
			        filterable: false,
			        template: '',
			        sortable: false
			    }, {
			        field: "dynamic",
			        title: "Динамика",
			        template: '# if(data.dynamic!=0) {# #=data.dynamic# % #} #',
			        filterable: false,
			        //template: '',
			        sortable: false
			    }],
            dataBinding: function () {
                record = 0; //(this.dataSource.page() - 1) * this.dataSource.pageSize();
            },
            resizable: true,
            change: function (e) {

            }
        }).getKendoGrid();

    }

    //---- report10
    function GetReportEE2() {

        kendo.ui.progress($('.report10'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportEE2', { oblast_id: qwe.paramQuery.oblast_id }, function (data) {

            kendo.ui.progress($('.report10'), false);

            if (data.ErrorMessage) {
                alert(data.ErrorMessage);
                chart10([], []);
                return;
            }
            console.log(data);
            
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

            chart10(categories, series);

            //$.map(data.ListItems, function (item) {
            //    item.consumption = Math.round(item.consumption);
            //    item.cons_prev = Math.round(item.cons_prev);
            //    item.dynamic = Math.round(item.dynamic);
            //});
            //qwe.reportGrid7.dataSource.data(data.ListItems);

        });

    }
    function chart10(categories, series) {
        //----
        var $container = $('.chart10');
        Highcharts.chart($container[0], {
            chart: { type: 'column', backgroundColor: '#FFF' },
            navigation: {
                buttonOptions: {
                    enabled: false
                }
            },
            title: { text: '' },
            xAxis: {
                categories: categories,
                labels: {
                    style: { "fontSize": "8px" },
                    reserveSpace: true,
                    autoRotation: [1],
                    staggerLines: 1
                },
                tickInterval: 1,
                showFirstLabel: true
            },
            yAxis: {
                title: { text: 'тыс. т.у.т.' },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                },
                step: 2000
            },
            tooltip: { enabled: false },
            credits: { enabled: false },
            plotOptions: {
                series: {
                    cursor: 'pointer',
                    dataLabels: {
                        style: { "color": "contrast", "fontSize": "5px", "fontWeight": "bold", "textOutline": "1px 1px contrast" }
                    }
                },
                column: { showInLegend: false }
            },
            series: series
        });
    }

    //---- 
    function ShowSpinner1() {

        $("#loading").fadeIn();
        var opts = {
            lines: 15, // The number of lines to draw
            length: 34, // The length of each line
            width: 14, // The line thickness
            radius: 41, // The radius of the inner circle
            corners: 1, // Corner roundness (0..1)
            rotate: 64, // The rotation offset
            direction: 1, // 1: clockwise, -1: counterclockwise
            color: '#000', // #rgb or #rrggbb
            speed: 1, // Rounds per second
            trail: 60, // Afterglow percentage
            shadow: false, // Whether to render a shadow
            hwaccel: false, // Whether to use hardware acceleration
            className: 'spinner', // The CSS class to assign to the spinner
            zIndex: 2e9, // The z-index (defaults to 2000000000)
            top: 'auto', // Top position relative to parent in px
            left: 'auto' // Left position relative to parent in px
        };
        var target = document.getElementById('loading');
        var spinner = new Spinner(opts).spin(target);
    }

    function ShowSpinner(tagName) {

        $(tagName).fadeIn();
        var opts = {
            lines: 15, // The number of lines to draw
            length: 34, // The length of each line
            width: 14, // The line thickness
            radius: 41, // The radius of the inner circle
            corners: 1, // Corner roundness (0..1)
            rotate: 64, // The rotation offset
            direction: 1, // 1: clockwise, -1: counterclockwise
            color: '#000', // #rgb or #rrggbb
            speed: 1, // Rounds per second
            trail: 60, // Afterglow percentage
            shadow: false, // Whether to render a shadow
            hwaccel: false, // Whether to use hardware acceleration
            className: 'spinner', // The CSS class to assign to the spinner
            zIndex: 2e9, // The z-index (defaults to 2000000000)
            top: 'auto', // Top position relative to parent in px
            left: 'auto' // Left position relative to parent in px
        };

        var target = $(tagName); // document.getElementsByClassName(tagName);
        var spinner = new Spinner(opts).spin(target);
    }

    //----
    function hideSpinner() {

        var target = document.getElementById('loading');
        $(target).hide();

    }

    function d() {
        var target = document.getElementById('loading');
        $(target).show();
    }

    function report8Map() {

        //---- карта
        var source = new ol.source.Vector({
            //features: features
        });

        var clusterSource = new ol.source.Cluster({
            distance: 40,
            source: source
        });

        var styleCache = {};
        var clusters = new ol.layer.Vector({
            source: clusterSource,
            style: function (feature, resolution) {
                var size = feature.get('features').length;
                var style = styleCache[size];
                if (!style) {
                    style = [new ol.style.Style({
                        image: new ol.style.Circle({
                            radius: 10,
                            stroke: new ol.style.Stroke({
                                color: '#fff'
                            }),
                            fill: new ol.style.Fill({
                                color: '#3399CC'
                            })
                        }),
                        text: new ol.style.Text({
                            text: size.toString(),
                            fill: new ol.style.Fill({
                                color: '#fff'
                            })
                        })
                    })];
                    styleCache[size] = style;
                }
                return style;
            }
        });

        var raster = new ol.layer.Tile({
            source: new ol.source.OSM()
        });

        var raw = new ol.layer.Vector({
            source: source
        });

        var map8 = new ol.Map({
            layers: [raster, clusters],
            renderer: 'canvas',
            target: 'map8',
            view: new ol.View({
                center: [-11000000, 4600000],
                zoom: 6,
                minZoom: 3
            })
        });
        map8.getView().setCenter(ol.proj.transform([71.44598, 51.1801], 'EPSG:4326', 'EPSG:3857'));

        var features = [];
        var vectorSource = new ol.source.Vector({
            features: features
        });

        var vectorLayer = new ol.layer.Vector({
            source: vectorSource
        });
        map8.addLayer(vectorLayer);

        kendo.ui.progress($('.report8'), true);

        $.post(rootUrl + 'ReportAnalyse/GetReportOblConsOnMap', {
            year: 2015
        }, function (data) {

            kendo.ui.progress($('.report8'), false);
            vectorSource.clear();

            if (data.ErrorMessage)
                alert(data.ErrorMessage);

            var features = new Array(data.ListItems.length);

            var max = Math.max.apply(Math, data.ListItems.map(function (o) { return o.val; }));
            var numDive = Math.pow(10, max.toString().length);

            $.map(data.ListItems, function (item, i) {

                var coordinates = [parseFloat(item.Lng), parseFloat(item.Lat)];
                rnd = Math.random();
                                
                var rnd2 = (parseFloat(item.val) / numDive) * 250;                
                if (rnd2 < 4.6)
                    rnd2 = 4;

                if (rnd2 > 41.19)
                    rnd2 = 43;

                features[i] = new ol.Feature({
                    geometry: new ol.geom.Point(ol.proj.transform(coordinates, 'EPSG:4326', 'EPSG:3857')),
                    radius: rnd2,//
                    name: '',
                    tname: item.name + ":" + item.val
                });
                style = computeFeatureStyle(features[i]);
                features[i].setStyle(style);
            });

            vectorSource.addFeatures(features);
        });
    }

    function report9Map() {
        
        //---- карта
        var source = new ol.source.Vector({
            //features: features
        });

        var clusterSource = new ol.source.Cluster({
            distance: 40,
            source: source
        });

        var styleCache = {};
        var clusters = new ol.layer.Vector({
            source: clusterSource,
            style: function (feature, resolution) {
                var size = feature.get('features').length;
                var style = styleCache[size];
                if (!style) {
                    style = [new ol.style.Style({
                        image: new ol.style.Circle({
                            radius: 10,
                            stroke: new ol.style.Stroke({
                                color: '#fff'
                            }),
                            fill: new ol.style.Fill({
                                color: '#3399CC'
                            })
                        }),
                        text: new ol.style.Text({
                            text: size.toString(),
                            fill: new ol.style.Fill({
                                color: '#fff'
                            })
                        })
                    })];
                    styleCache[size] = style;
                }
                return style;
            }
        });

        var raster = new ol.layer.Tile({
            source: new ol.source.OSM()
        });

        var raw = new ol.layer.Vector({
            source: source
        });

        var map9 = new ol.Map({
            layers: [raster, clusters],
            renderer: 'canvas',
            target: 'map9',
            view: new ol.View({
                center: [-11000000, 4600000],
                zoom: 6,
                minZoom: 3
            })
        });
        map9.getView().setCenter(ol.proj.transform([71.44598, 51.1801], 'EPSG:4326', 'EPSG:3857'));

        var features = [];
        var vectorSource = new ol.source.Vector({
            features: features
        });

        var vectorLayer = new ol.layer.Vector({
            source: vectorSource
        });
        map9.addLayer(vectorLayer);

        source.clear();
        kendo.ui.progress($('.report9'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportSubjectsOnMap', {
            year: qwe.paramQuery.year
        }, function (data) {

            kendo.ui.progress($('.report9'), false);
            qwe.paramQuery.isFirst = true;
            
            if (data.ErrorMessage)
                alert(data.ErrorMessage);

            var features = new Array(data.ListItems.length);
            $.map(data.ListItems, function (item, i) {

                var coordinates = [parseFloat(item.Lng), parseFloat(item.Lat)];
                features[i] = new ol.Feature({
                    geometry: new ol.geom.Point(ol.proj.transform(coordinates, 'EPSG:4326', 'EPSG:3857')),
                    name: item.JuridicalName
                });

            });
            source.addFeatures(features);
        });

    }
    function computeFeatureStyle(feature) {
        return new ol.style.Style({
            image: new ol.style.Circle({
                radius: feature.get('radius'),
                fill: new ol.style.Fill({
                    color: 'rgba(100,50,200,0.5)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(120,30,100,0.8)',
                    width: 3
                })
            }),
            text: new ol.style.Text({
                font: '12px helvetica,sans-serif',
                text: feature.get('name'),
                rotation: 360 * rnd * Math.PI / 180,
                fill: new ol.style.Fill({
                    color: '#000'
                }),
                stroke: new ol.style.Stroke({
                    color: '#fff',
                    width: 2
                })
            })
        });
    }
});
