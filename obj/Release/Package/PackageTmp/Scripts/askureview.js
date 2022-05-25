$(function () {

	console.log('askureview.js');
	var qwe = {};
	window.qwe = qwe;
	createControllers();
	initVars();
	bindEventHandlers();
	createGrid();
	GetGridDataSources();
	GetViewAskuer();

	//----
	function initVars() {
		qwe.paramQuery = {};
		qwe.paramQuery.isFirst = false;
		qwe.paramQuery.oblast_id = -1;
		qwe.paramQuery.year = 2015;
		qwe.paramQuery.device_id = 1;
		qwe.paramQuery.isdaymonth = 1;

	}

	//----
	function createControllers() {

		//---- создать вкладку  
		qwe.tabStrip = $('.tab-view2').kendoTabStrip({
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

	}

	//----
	function bindEventHandlers() {

	}

	//----
	function createGrid() {

		//----
		qwe.objectGrid1 = $('.tab-view2 .object-grid1').kendoGrid({
			scrollable: false,
			sortable: true,
			height:200,
			selectable: 'row',
			columns: [
			{
				field: "name",
				title: 'Детализация:',
				filterable: false,
				sortable: false
			}],
			resizable: true,
			change: function (e) {

				if (qwe.paramQuery.isFirst) {

					var dataItem = qwe.objectGrid1.dataItem(this.select());
					qwe.paramQuery.isdaymonth = dataItem.id;
					GetViewAskuer();
				}

			}
		}).getKendoGrid();

		//----
		qwe.objectGrid2 = $('.tab-view2 .object-grid2').kendoGrid({
			scrollable: false,
			height: 200,
			sortable: true,
			selectable: 'row',
			columns: [
			{
				field: "device_name",
				title:'Счетчик:',
				filterable: false,
				sortable: false
			}],
			resizable: true,
			change: function (e) {

				if (qwe.paramQuery.isFirst) {

					var dataItem = qwe.objectGrid2.dataItem(this.select());
					qwe.paramQuery.device_id = dataItem.id;

					qwe.paramQuery.currRegion = dataItem;
					GetViewAskuer();
				}

			}
		}).getKendoGrid();

		resizeGrids();
	}
	
	//----
	function createChart(categories, series) {
		
		var $container = $('.tab-view2 .view-chart');

		qwe.chart1 = Highcharts.chart($container[0], {
			credits: {
				enabled: false
			},
			title: {
				text: ''
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

							//qwe.tabStrip.wrapper.find('.tab-item-report2').removeClass('hide');
							//var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item-report2");
							//qwe.tabStrip.activateTab(tabToActivate);

							//qwe.paramQuery.currObject = { name: e.point.category };
							//qwe.paramQuery.year = e.point.category;

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
	function GetGridDataSources() {

		//---- Детализация:
		$.post(rootUrl + 'ReportAskuerCase/GetDetailing', function (data) {

			console.log("data", data);
			qwe.objectGrid1.dataSource.data(data.ListItems);
			var rows = qwe.objectGrid1.dataSource.data();
			var row = qwe.objectGrid1.table.find("[data-uid=" + rows[0].uid + "]");
			qwe.objectGrid1.select(row);
		});

		//---- Счетчик:
		$.post(rootUrl + 'ReportAskuerCase/COLLECTOR_Cmdevice', function (data) {
			
			console.log("data", data);
			qwe.objectGrid2.dataSource.data(data.ListItems);

			var rows = qwe.objectGrid2.dataSource.data();
			var row = qwe.objectGrid2.table.find("[data-uid=" + rows[0].uid + "]");
			qwe.objectGrid2.select(row);

		});

	}

	//----
	function GetViewAskuer() {

		kendo.ui.progress($('.report1'), true);

		if (qwe.paramQuery.isdaymonth == 1) {

			//----day
			$.post(rootUrl + 'ReportAskuerCase/GetViewAskuerDay', { device_id: qwe.paramQuery.device_id }, function (data) {

				kendo.ui.progress($('.report1'), false);
				qwe.paramQuery.isFirst = true;

				if (data.ErrorMessage) {
					openAlertWindow(data.ErrorMessage);
					createChart([], []);
					return
				}
				
			    //----
				var orderByData = data.ListItems;
				//var orderByData = new jinqJs()
				//  .from(data.ListItems)
				//  .orderBy([{ field: 'day' }])
				//  .select();


				var categories = [];
				var seriesData = [];
				$.map(orderByData, function (item) {
				    console.log(item);
					categories.push(item.day);
					var y = decimalAdjust('round', parseFloat(item.val / 1000), -3);
					seriesData.push({ y: y });
				});

				console.log("series data=", seriesData);
				var series = [{ name: '', data: seriesData }];
				createChart(categories, series);

			});

		} else {

			//---- month
			$.post(rootUrl + 'ReportAskuerCase/GetViewAskuerMonth', { device_id: qwe.paramQuery.device_id }, function (data) {

				kendo.ui.progress($('.report1'), false);
				qwe.paramQuery.isFirst = true;

				if (data.ErrorMessage) {
					openAlertWindow(data.ErrorMessage);
					createChart([], []);
					return
				}
				
				//----
				var orderByData = new jinqJs()
				  .from(data.ListItems)
				  .orderBy([{ field: 'month_name' }])
				  .select();

				var categories = [];
				var seriesData = [];
				$.map(orderByData, function (item) {

					categories.push(item.month_name);
					var y = decimalAdjust('round', parseFloat(item.val / 1000), -3);
					seriesData.push({ y: y });
				});

				var series = [{ name: '', data: seriesData }];
				createChart(categories, series);

			});

		}

	}


});