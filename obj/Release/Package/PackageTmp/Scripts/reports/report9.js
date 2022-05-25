$(function () {

    console.log('report9.js');
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
        qwe.paramQuery.oblast_id = -1;
        qwe.paramQuery.year = 2015;

    }

    //----
    function createControllers() {

        qwe.tabStrip = $('.tab-report9').kendoTabStrip({
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


    }

    //----
    function createGrid() {

        //----
        qwe.yearGrid = $('.tab-report9 .year-grid').kendoGrid({
            scrollable: false,
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
                    cpMap();
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
    function fillYearGrid() {

        dic_getDicRstReport(function () {

            qwe.dicRstReport = dic.dicRstReport;

            qwe.yearGrid.dataSource.data(qwe.dicRstReport);

            var rows = qwe.yearGrid.dataSource.data();
            var row = qwe.yearGrid.table.find("[data-uid=" + rows[3].uid + "]");
            qwe.yearGrid.select(row);

        });

    }
	  

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

    var map = new ol.Map({
        layers: [raster, clusters],
        renderer: 'canvas',
        target: 'map',
        view: new ol.View({
            center: [-11000000, 4600000],
            zoom: 5,
            minZoom: 3
        })
    });
    map.getView().setCenter(ol.proj.transform([76.889709, 43.238949], 'EPSG:4326', 'EPSG:3857'));

    //----
    var element = document.getElementById('popup');
    var popup = new ol.Overlay({
        element: element,
        positioning: 'bottom-center',
        stopEvent: false
    });
    map.addOverlay(popup);

    //---- click істегенде шығатын сообщения (display popup on click)
    map.on('click', function (evt) {

        var feature = map.forEachFeatureAtPixel(evt.pixel,
            function (feature, layer) {
                return feature;
            });
        if (feature) {
            qwe.feature = feature;
            console.log(feature);
            console.log("name=" + feature.get('name'));
            $('#map').find('.popover-content').empty();

            popup.setPosition(evt.coordinate);
            $(element).popover({
                'placement': 'top',
                'html': true,
                'content':feature.j.features[0].j.name
            });
            $(element).popover('show');
        } else {
            $(element).popover('destroy');
        }
    });

    cpMap();
    function cpMap() {

        //----
        source.clear();

        $.post(rootUrl + 'ReportAnalyse/GetReportSubjectsOnMap', {
            year: qwe.paramQuery.year
        }, function (data) {

            qwe.paramQuery.isFirst = true;
            console.log(data);


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
});