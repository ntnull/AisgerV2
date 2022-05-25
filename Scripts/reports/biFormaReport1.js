$(function () {

    //----
    bi.FormaReport1 = function (param) {

        var qwe = {
            param: param,
            openWindow: openWindow
        };

        param.wrapper.append(param.content);
        var wrapper = qwe.param.wrapper;

        changeInterfaceLang();
        createGrids();
        createObjChart3([], []);
        createObjChart4([], []);

        function openWindow(p) {

            qwe.currObject = p;
            console.log("open window", p);
            GetSubjectForm();
            GetSubjectFormPieChart();
            GetSubForm4Record();
            GetSubFormResValumeByYears();
            GetSubFormResConsumption();
            GetSubFormEnergyIndicators();
            GetSubFormEAudit();
            GetSubFormDynamicsResByYears1();
            GetSubFormDynamicsResByYears7();

        }

        function createGrids() {

            //----Объем потребления энергоресурсов
            qwe.objGrid1 = wrapper.find('.object-grid1').kendoGrid({
                height: 230,
                scrollable: false,
                sortable: true,
                selectable: 'row',
                columns: [
                {
                    field: "report_year",
                    title: "Год",
                    filterable: false,
                    sortable: false
                },
                {
                    field: "consumption",
                    title: L.gridCol1,
                    filterable: false,
                    sortable: false
                }],
                resizable: true,
                change: function (e) {


                }
            }).getKendoGrid();

            //----Структура потребления энергоресурсов
            qwe.objGrid2 = wrapper.find('.object-grid2').kendoGrid({
                scrollable: true,
                sortable: true,
                height: 350,
                selectable: 'row',
                columns: [
                {
                    field: "resource_name",
                    title: L.gridCol2,
                    filterable: false,
                    sortable: false
                },
                {
                    field: "unit_meas",
                    title: L.gridCol3,
                    filterable: false,
                    sortable: false
                },
                {
                    field: "consumption_own",
					template:'<span class="number-right">#: consumption_own #</span>',
                    title: L.gridCol4,
                    filterable: false,
                    sortable: false
                },
                {
                    field: "consumption_notown",
					template:'<span class="number-right">#: consumption_notown #</span>',
                    title: L.gridCol5,
                    filterable: false,
                    sortable: false
                },
                {
                    field: "consumption",
					template:'<span class="number-right">#: consumption #</span>',
                    title: L.gridCol6,
                    filterable: false,
                    sortable: false
                },
                {
                    field: "transfer",
					template:'<span class="number-right">#: transfer #</span>',
                    title: L.gridCol7,
                    filterable: false,
                    sortable: false
                },
                {
                    field: "expense",
					template:'<span class="number-right">#: expense #</span>',
                    title: L.gridCol8,
                    filterable: false,
                    sortable: false
                }],
                resizable: true,
                change: function (e) {

                }
            }).getKendoGrid();


            //----Показатели энергоэффективности
            qwe.objGrid3 = wrapper.find('.object-grid3').kendoGrid({
                scrollable: true,
                sortable: true,
				height:300,
                selectable: 'row',
                columns: [
                {
                    field: "IndicatorName1",
                    title: L.gridCol9,
                    filterable: false,
                    sortable: false
                },
                {
                    field: "UnitMeasure",
                    title: L.gridCol3,
                    filterable: false,
                    sortable: false
                },
                {
                    field: "EnergyValue",
                    title: L.gridCol10,
                    filterable: false,
                    sortable: false
                }],
                resizable: true,
                change: function (e) {
                }
            }).getKendoGrid();

            //----Сведения об энергоаудите
            qwe.objGrid4 = wrapper.find('.object-grid4').kendoGrid({
                scrollable: true,
                sortable: true,
                selectable: 'row',
                columns: [
                {
                    field: "auditor_name",
                    title: L.gridCol11,
                    filterable: false,
                    sortable: false
                },
                {
                    field: "report_year",
                    title: L.year,
                    filterable: false,
                    sortable: false
                }],
                resizable: true,
                change: function (e) {
                }
            }).getKendoGrid();

            //----Мероприятия
            qwe.objGrid5 = wrapper.find('.object-grid5').kendoGrid({
                scrollable: true,
                sortable: true,
                height: 350,
                selectable: 'row',
                columns: [
                {
                    field: "EventName",
                    title: L.gridCol12,
                    filterable: false,
                    sortable: false
                },
                {
                    field: "EmplPeriod",
                    title: L.gridCol13,
                    filterable: false,
                    sortable: false
                },
                {
                    field: "ActualInvest",
                    title: L.gridCol14,
                    filterable: false,
                    sortable: false
                }],
                resizable: true,
                change: function (e) {
                }
            }).getKendoGrid();
        }

        function createObjChart1(categories, chdata) {

            //---- chart 1
            var $container = wrapper.find('.object-chart1');
            qwe.clchart1 = Highcharts.chart($container[0], {
                credits: {
                    enabled: false
                },
                chart: {
                    type: 'column',
                    height: 240,
                    width: 400
                },
                navigation: {
                    buttonOptions: {
                        enabled: false
                    }
                },
                title: {
                    useHTML: false,
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                legend: {
                    enabled: false,
                    align: 'center',
                    verticalAlign: 'bottom',
                    layout: 'horizontal'
                },
                plotOptions: {
                    series: {
                        cursor: 'pointer'
                    },
                    column: {
                        events: {
                            click: function (event) {
                            }
                        },
                        tooltip: {
                            pointFormat: '<b>{point.y}</b>'
                        }
                    }
                },
                xAxis: {
                    categories: categories,
                    labels: {
                        x: -10
                    }
                },
                yAxis: {
                    min: 0,
                    endOnTick: false,
                    startOnTick: true,
                    tickInterval: 200,
                    labels: {
                        formatter: function () {
                            return this.value;
                        }
                    },
                    allowDecimals: true,
                    title: {
                        text: ''
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

        function createObjChart2(data) {

            var $container = wrapper.find('.object-chart2');
            qwe.pieChart = Highcharts.chart($container[0], {
                credits: {
                    enabled: false
                },
                navigation: {
                    buttonOptions: {
                        enabled: false
                    }
                },
                chart: {
                    type: 'pie',
                    height: 240,
                },
                title: {
                    text: ''
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
                        allowPointSelect: true,
                        cursor: 'pointer',
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
                    enabled: true,
                    margin: 0,
                    padding: 1,
                    align: 'center',
                    verticalAlign: 'bottom',
                    layout: 'horizontal',
                    itemStyle: {
                        fontSize: '9px',
                        font: '9pt Trebuchet MS, Verdana, sans-serif'
                        //color: '#A0A0A0'
                    }
                },
                tooltip: {
                    valueSuffix: '%'
                },
                series: [{
                    innerSize: '30%',
                    data: data
                }]
            });

        }

        //----
        function createObjChart3(categories, chdata) {

        	var $container = wrapper.find('.object-chart3');

        	console.log("height", $($container).height());
        	console.log("width", $($container).width());

            qwe.clchart1 = Highcharts.chart($container[0], {
                credits: {
                    enabled: false
                },
                navigation: {
                    buttonOptions: {
                        enabled: false
                    }
                },               
                chart: {
                	type: 'column',
                	width: 250,
                	height: 300
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
                    series: {
                        cursor: 'pointer'
                    },
                    column: {
                        events: {
                        	click: function (event) {

                                var series = event.point.series;
                                qwe.paramQuery.currObject = { TypeResourceId: event.point.id, NameRu: event.point.name };
                                var index = series.index;
                                qwe.paramQuery.year = series.name;
                                initialReport2();
                            }
                        },
                        tooltip: {
                            pointFormat: '<b>{point.y}</b>'
                        }
                    }
                },
                xAxis: {
                    categories: categories,// ['Apples', 'Oranges', 'Bananas'],
                    labels: {
                        x: -10,
                        style: { "fontSize": "8px" },
                        reserveSpace: true,
                        autoRotation: [1],
                        staggerLines: 1
                    },
                    tickInterval: 1,
                    showFirstLabel: true
                },
                yAxis: {
                    labels: {
                        formatter: function () {
                            return this.value;
                        }
                    },
                    allowDecimals: false,
                    title: {
                        text:L.y1000
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

        //----
        function createObjChart4(categories, chdata) {

            var $container = wrapper.find('.object-chart4');
            qwe.clchart2 = Highcharts.chart($container[0], {
                chart: {
                	type: 'column',
					height:300,
                	width: 700
                },
                navigation: {
                    buttonOptions: {
                        enabled: false
                    }
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
                    min: 0,
                    //max: 100,
                    endOnTick: false,
                    startOnTick: true,
                    tickInterval: 0.5,
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
                    series: {
                        cursor: 'pointer'
                    },
                    column: {
                        events: {
                            click: function (event) {

                            }
                        },
                        tooltip: {
                            pointFormat: '<b>{point.y}</b>'
                        }
                    }
                },
                series: chdata,

            });

        }

        //----
        function GetSubjectForm() {

            $.post(rootUrl + 'ReportAnalyse/GetSubjectForm', { subject_id: qwe.currObject.secUserId ,year:qwe.currObject.year }, function (data) {

                if (data.ErrorMessage) {
                    openAlertWindow(data.ErrorMessage);
                }

                if (data.ListItems.length > 0) {

                    var dataItem = data.ListItems[0];
                    wrapper.find('.span-title').text(dataItem.JuridicalName);
                    wrapper.find('.span-region').text(dataItem.oblast_name);
                    wrapper.find('.span-oked').text(dataItem.oked_name);
                    wrapper.find('.span-formof-ownership').text(dataItem.jur_name);
                    wrapper.find('.span-idk').text(dataItem.IDK);

                } else {

                    wrapper.find('.span-title').text('');
                    wrapper.find('.span-region').text('');
                    wrapper.find('.span-oked').text('');
                    wrapper.find('.span-formof-ownership').text('');
                    wrapper.find('.span-idk').text('');

                }
            });

        }

        //----
        function GetSubjectFormPieChart() {

            $.post(rootUrl + 'ReportAnalyse/GetSubjectFormPieChart', { subject_id: qwe.currObject.secUserId, year: qwe.currObject.year }, function (data) {

                if (data.ErrorMessage) {
                    openAlertWindow(data.ErrorMessage);
                }

                var pieData = [];
                var otherSum = 0, otherName = "";
                $.map(data.ListItems, function (item, index) {

                    var y = parseFloat(item.consumption);
                    if (index == 0) {
                        pieData.push({ name: item.JuridicalName, y: y });
                    } else {

                        otherSum += y;
                        if (index == 1)
                            otherName = item.JuridicalName;
                    }

                });

                pieData.push({ name: otherName, y: otherSum });
                createObjChart2(pieData);

                console.log("pie", data);
            });

        }

        //----Объем потребления энергоресурсов
        function GetSubFormResValumeByYears() {

            $.post(rootUrl + 'ReportAnalyse/GetSubFormResValumeByYears', { subject_id: qwe.currObject.secUserId }, function (data) {

                if (data.ErrorMessage) {
                    openAlertWindow(data.ErrorMessage);
                }

                var chdata = [];
                var categories = [];
                var buffer = [];
                $.map(data.ListItems, function (item, index) {

                    item.consumption = decimalAdjust('round', parseFloat(item.consumption), 0);

                    categories.push(item.report_year);
                    var y = (item.consumption / 1000);
                    buffer.push(y);

                });

                console.log("ch111", buffer);
                qwe.objGrid1.dataSource.data(data.ListItems);

                chdata.push({ name: '', data: buffer });
                createObjChart1(categories, chdata);

            });

        }

        //----Структура потребления энергоресурсов 
        function GetSubFormResConsumption() {

            $.post(rootUrl + 'ReportAnalyse/GetSubFormResConsumption', { subject_id: qwe.currObject.secUserId, year: qwe.currObject.year }, function (data) {

                if (data.ErrorMessage) {
                    openAlertWindow(data.ErrorMessage);
                }

                $.map(data.ListItems, function (item) {
					
                    item.consumption = Math.round(parseFloat(item.consumption));
					item.consumption_notown=decimalAdjust("round", item.consumption_notown, '-3');
                });

                qwe.objGrid2.dataSource.data(data.ListItems);
            });

        }

        //----Показатели энергоэффективности
        function GetSubFormEnergyIndicators() {

            $.post(rootUrl + 'ReportAnalyse/GetSubFormEnergyIndicators', { subject_id: qwe.currObject.secUserId, year: qwe.currObject.year }, function (data) {

                console.log("en=", data);
                if (data.ErrorMessage) {
                    openAlertWindow(data.ErrorMessage);
                }

                qwe.objGrid3.dataSource.data(data.ListItems);
            });

        }

        //----Сведения об энергоаудите
        function GetSubFormEAudit() {

            $.post(rootUrl + 'ReportAnalyse/GetSubFormEAudit', { subject_id: qwe.currObject.secUserId }, function (data) {

                if (data.ErrorMessage) {
                    openAlertWindow(data.ErrorMessage);
                }

                qwe.objGrid4.dataSource.data(data.ListItems);
            });

        }

        //----(тыс. т.у.т.) Динамика потребления
        function GetSubFormDynamicsResByYears1() {

            $.post(rootUrl + 'ReportAnalyse/GetSubFormDynamicsResByYears1', { subject_id: qwe.currObject.secUserId, year: qwe.currObject.year }, function (data) {

                if (data.ErrorMessage) {
                    openAlertWindow(data.ErrorMessage);
                }

                console.log("dynamic of consumption", data);

                if (data.ListItems.length > 0) {

                    var categories = [];
                    var series = [];
                    var buffer = [];
                    var resource_name = "";

					 var orderByData = new jinqJs()
                                         .from(data.ListItems)
                                         .orderBy([{ field: 'report_year'}])
                                         .select();
										 
                    $.map(orderByData, function (item, index) {

                        var y = decimalAdjust("round", item.consumption, '-3');
                        series.push({ name: item.report_year, data: [{ y: y }] });

                        if (index == 0)
                            resource_name = item.resource_name;
                    });

                    categories.push(resource_name);
                    createObjChart3(categories, series);

                } else createObjChart3([], []);

            });

        }

        //---- Динамика потребления энергоресурсов
        function GetSubFormDynamicsResByYears7() {

            $.post(rootUrl + 'ReportAnalyse/GetSubFormDynamicsResByYears7', { subject_id: qwe.currObject.secUserId, year: qwe.currObject.year }, function (data) {

                if (data.ErrorMessage) {
                    openAlertWindow(data.ErrorMessage);
                }

                console.log("dynamic chart7", data);

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
                                         .orderBy([{ field: 'report_year'}])
                                         .select();
                var series = [];
                $.map(distinctYear, function (item) {

                    var buffer = $.grep(data.ListItems, function (row) { return row.report_year == item.report_year; });

                    var indata = [];
                    $.map(buffer, function (row) {
                        var y = decimalAdjust('round', row.consumption, -2);
                        indata.push({ id: row.resource_id, y: y });
                    });

                    series.push({ name: item.report_year, data: indata });
                });

                createObjChart4(categories, series);
            });

        }

        //----Мероприятия
        function GetSubForm4Record() {

            $.post(rootUrl + 'ReportAnalyse/GetSubForm4Record', { subject_id: qwe.currObject.secUserId, year: qwe.currObject.year }, function (data) {

                if (data.ErrorMessage) {
                    openAlertWindow(data.ErrorMessage);
                }

                $.map(data.ListItems, function (item) {
                    item.EmplPeriod = getDateStrFromEFDateFormat(item.EmplPeriod, 'DD.MM.YYYY');
                });

                qwe.objGrid5.dataSource.data(data.ListItems);
                console.log("grid", data);
            });

        }

        //----interface change languages
        function changeInterfaceLang() {
            //----регион
            wrapper.find('.span-region-title').text(L.region);

            //----Деятельность
            wrapper.find('.span-oked-title').text(L.oked);

            //----Форма собственности 
            wrapper.find('.span-formof-ownership-title').text(L.formofownership);

            //----Объем потребления энергоресурсов
            wrapper.find('.ogrid1Title').text(L.ogrid1Title);

            //----Динамика потребления (тыс. т.у.т.)
            wrapper.find('.object-chart1-title').text(L.ochart1Title);

            //----Соотношение к остальным субъектам
            wrapper.find('.object-chart2-title').text(L.ochart2Title);

            //----Динамика потребления энергоресурсов
            wrapper.find('.dynamic-cons-title').text(L.dynamicConsTitle);

            //----Структура потребления энергоресурсов
            wrapper.find('.object-grid2-title').text(L.ogrid2Title);

            //----Показатели энергоэффективности
            wrapper.find('.object-grid3-title').text(L.ogrid3Title);

            //----Сведения об энергоаудите
            wrapper.find('.object-grid4-title').text(L.ogrid4Title);

            //----Мероприятия
            wrapper.find('.object-grid5-title').text(L.ogrid5Title);
        }

        return qwe;
    }



});
