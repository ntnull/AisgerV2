$(function () {

	console.log('app.js');
	var qwe = {};
	window.qwe = qwe;
	createControllers();
	initVars();
	bindEventHandlers();

	GetViewByJurType();
	GetViewConsByRes8();
	GetViewByJurTop10();

	loadHtmlTemplates();
	//----
	function initVars() {
		qwe.paramQuery = {};
		qwe.paramQuery.isFirst = false;
		qwe.paramQuery.oblast_id = -1;
		qwe.paramQuery.year = 2015;
		qwe.paramQuery.jurTypeApply = [{ Id: 1, NameRu: 'ЮР', code: 'юр' }, { Id: 2, NameRu: 'КВ', code: 'кв' }, { Id: 3, NameRu: 'ГУ', code: 'гу' }];

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

		});

	}

	//----
	function bindEventHandlers() {
	
	}
	 
	//---- chart 1
	function createChart1(categories, series) {

		var $container = $('.view-chart1');

		qwe.chart1 = Highcharts.chart($container[0], {
			chart: {
				type: 'bar'
			},
			title: {
				text: L.view1Chart1
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
					text: L.y1000
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

							qwe.paramQuery.currObject = { id: points.id, name: points.series.name };
							GetReportTop10ByJurType();
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

	//---- chart 2
	function createChart2(categories, series) {

		var $container = $('.view-chart2');
		qwe.chart2 = Highcharts.chart($container[0], {
			chart: {
				type: 'bar'
			},
			title: {
				text: L.view1Chart2
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
					text: L.y1000
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

							qwe.paramQuery.currObject = { id: points.id, name: points.series.name };
							console.log(qwe.paramQuery.currObject);
							GetReportTop10ByJurType();
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

	//----pie chart 3
	function createChart3(data) {

		var $container = $('.view-chart3');

		qwe.chart3 = Highcharts.chart($container[0], {
			credits: {
				enabled: false
			},
			chart: {
				type: 'pie'
			},
			title: {
				text: L.view1Chart3
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

					        qwe.paramQuery.currObject = { resource_id: event.point.id, NameRu: event.point.name };
							GetReportTop10ByRes();

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

	//----chart 4
	function createChart4(categories, series) {

		var $container = $('.view-chart4');

		qwe.chart4 = Highcharts.chart($container[0], {
			credits: {
				enabled: false
			},
			chart: {
				type: 'bar'
			},
			title: {
				text: L.view1Chart4
			},
			subtitle: {
				text:''
			},
			xAxis: {
				categories: categories
			},
			yAxis: {
				min: 0,
				title: {
					text: L.y1000
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
	function GetViewByJurType() {

		kendo.ui.progress($('.report1'), true);
		$.post(rootUrl + 'ReportAnalyse/GetViewByJurType', function (data) {

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
					var jur_type_name_short = "";
					$.map(orderByData, function (inItem) {

						sdata1.push({ id: inItem.jur_type_id, y: inItem.subject_count });
						sdata2.push({ id: inItem.jur_type_id, y: Math.round(inItem.consumption) });

						jur_type_name_short = inItem.jur_type_name_short;

						if (index == 0)
							categories.push(inItem.report_year);

					});

					series1.push({ name: jur_type_name_short, data: sdata1 });
					series2.push({ name: jur_type_name_short, data: sdata2 });
				});

				createChart1(categories, series1);
				createChart2(categories, series2);
			});

		});

	}

	//----
	function GetViewConsByRes8() {

		$.post(rootUrl + 'ReportAnalyse/GetViewConsByRes8', {
			year: qwe.paramQuery.year
		}, function (data) {

			qwe.paramQuery.isFirst = true;
			if (data.ErrorMessage)
				alert(data.ErrorMessage);

			var orderByData = new jinqJs()
								.from(data.ListItems)
								.orderBy([{ field: 'consumption', sort: 'desc' }])
								.select();

			var piedata = [], othersum = 0;
			$.map(orderByData, function (item, indx) {
				if (indx < 7) {
					piedata.push({ id: item.resource_id, name: item.resource_name, y: item.consumption });
				} else {
					othersum += item.consumption;
				}
			});

			piedata.push({ id: -1, name: L.others, y: othersum });
			createChart3(piedata);
		});
	}

	//----
	function GetViewByJurTop10() {

		kendo.ui.progress($('.report1'), true);
		$.post(rootUrl + 'ReportAnalyse/GetViewByJurTop10', {
			year: qwe.paramQuery.year
		}, function (data) {

			kendo.ui.progress($('.report1'), false);

			if (data.ErrorMessage) {
				createChart4([], []);
				openAlertWindow(data.ErrorMessage);
				return;
			}
			
			var categories = [];
			var seriesData = [];
			$.map(data.ListItems, function (item) {

				categories.push(item.subject_name);

				var y = parseFloat(item.consumption) / 1000;
				seriesData.push({ id: item.subject_id, y: Math.floor(y) });
			});

			var series = [{ name: '', data: seriesData }];
			createChart4(categories, series);
		});

	}

	//----------------------------------------------part 2
	function createChart21(categories, series) {

		var $container = $('.view-chart21');

		qwe.chart21 = Highcharts.chart($container[0], {
			credits: {
				enabled: false
			},
			chart: {
				type: 'bar'
			},
			title: {
				text: qwe.paramQuery.title
			},
			subtitle: {
				text: qwe.paramQuery.subtitle
			},
			xAxis: {
				categories: categories //['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
			},
			yAxis: {
				min: 0,
				title: {
					text: L.y1000
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
		});

	}

	//----
	function createChart22(seriesData) {

		var $container = $('.view-chart22');

		qwe.chart22 = Highcharts.chart($container[0], {
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
				//dataLabels: {
				//	formatter: function () {
				//		// display only if larger than 1
				//		return this.y > 1000 ? '<b>' + this.point.name + ':</b> ' +
				//			this.y + '%' : null;
				//	}
				//}
			}]
		});

	}

	//----
	function GetReportTop10ByJurType() {

		//----check class
		if (!$('.view-chart22').hasClass('hide')) {
			$('.view-chart22').addClass('hide');
			createChart21([], []);
		}

		qwe.paramQuery.title = L.top10;
		qwe.paramQuery.subtitle = L.formofownership + ':' + qwe.paramQuery.currObject.name + ' <br> ' + L.region + ':' + L.byRepublic;

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

			createChart21(categories, series);

		});

	}

	//---- chart 21  22  backend
	function GetReportTop10ByRes() {

		//----check class
		if ($('.view-chart22').hasClass('hide')) {
			$('.view-chart22').removeClass('hide');
			createChart21([], []);
		}
		
		qwe.paramQuery.title = L.top10 + ' ' + qwe.paramQuery.currObject.NameRu;
		qwe.paramQuery.subtitle = L.region + ':' + L.byRepublic;

		kendo.ui.progress($('.tab-report2 .report2'), true);
		$.post(rootUrl + 'ReportAnalyse/GetReportTop10ByRes', { resource_id: qwe.paramQuery.currObject.resource_id, year: qwe.paramQuery.year, oblastId: qwe.paramQuery.oblast_id }, function (data) {

			kendo.ui.progress($('.tab-report2 .report2'), false);

			if (data.ErrorMessage)
				openAlertWindow(data.ErrorMessage);
			

			//----fill chart series and categories
			var categories = [];
			var seriesData = [];
			$.map(data.ListItems, function (item) {

				categories.push(item.subject_name);

				var y = parseFloat(item.consumption) / 1000;
				seriesData.push({ id: item.subject_id, y: Math.floor(y) });
			});

			var series = [{ name: '', data: seriesData }];
			createChart21(categories, series);
		});
		
		//---- chart 2 2 backend
		kendo.ui.progress($('.tab-report2 .report2'), true);
		$.post(rootUrl + 'ReportAnalyse/GetReportResByRegion', { resource_id: qwe.paramQuery.currObject.resource_id, year: qwe.paramQuery.year, oblastId: qwe.paramQuery.oblast_id }, function (data) {

			kendo.ui.progress($('.tab-report2 .report2'), false);

			if (data.ErrorMessage)
				openAlertWindow(data.ErrorMessage);
			
			var orderByData = new jinqJs()
								.from(data.ListItems)
								.orderBy([{ field: 'consumption', sort: 'desc' }])
								.select();

			var seriesData = [],othersum=0;
			$.map(orderByData, function (item, indx) {

				var y = Math.round(parseFloat(item.consumption));
				if (indx < 6 && y > 0) {
					seriesData.push({ id: item.Id, name: item.oblast_name, y: Math.round(y) });
				} else {
					othersum += y;
				}
			});

			seriesData.push({ id: -1, name:L.others, y: othersum });
			createChart22(seriesData);
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