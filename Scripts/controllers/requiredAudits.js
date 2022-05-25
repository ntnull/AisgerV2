$(function () {

	var qwe = {};
	window.qwe = qwe;
	initVars();
	createControllers();
	createGrids();
	getYears();	
	bindEventHandlers();

	getDics();
	createWindows();

	function initVars() {
		qwe.tempSource = {};
		qwe.paramQuery = {};
		qwe.paramQuery.pageNum = 1;
		qwe.paramQuery.scrollPageNum = 1;
		qwe.paramQuery.pageCount = 1;
		qwe.paramQuery.allCount = 0;
		qwe.paramQuery.year = 2016;
		qwe.paramQuery.calcDeep = 1;
		qwe.paramQuery.oblast_ids = "-1";
		qwe.paramQuery.activeYear=2016;
	}

	function createControllers() {

		//----отчетный год
		qwe.dropdownYear = $('.select-year').kendoDropDownList({
			dataTextField: "ReportYear",
			dataValueField: "ReportYear",
			change: function () {
			
				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;
				qwe.paramQuery.scrollPageNum = 1;
				qwe.paramQuery.year = qwe.dropdownYear.value();
				getData();
			}
		}).getKendoDropDownList();

		//----глубина расчета
		qwe.dropdownCalcDeep = $('.select-calc-deep').kendoDropDownList({
			dataTextField: "Value",
			dataValueField: "Value",
			dataSource: {
				data: [{ Value: 1 }, { Value: 2 }, { Value: 3 }, { Value: 4 }, { Value: 5 }]
			},
			change: function () {
				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;
				qwe.paramQuery.scrollPageNum = 1;
				qwe.paramQuery.calcDeep = qwe.dropdownCalcDeep.value();
				getData();
			}
		}).getKendoDropDownList();

	}

	//----
	function createGrids() {

		//----
		qwe.grid = $('.object-grid').kendoGrid({
			scrollable: true,
			sortable: false,
			width: '98%',
			//height: '80%',
			selectable: 'row',
			columns: [{
				field: "ROWNUMBER",
				title: "№ п\\п",
				width: 45,
				filterable: false,
				template: '#= ++record #'
			}, {
				field: "IDK",
				title: 'ИДК',
				filterable: true,
				sortable: true,
				width: 130
			}, {
				field: "BINIIN",
				title: 'ИИН/БИН',
				filterable: true,
				sortable: true,
				width: 130
			}, {
				field: "OwnerName",
				title: 'Наименование',
				filterable: false,
				sortable: true,
				width: 130
			}, {
				field: "FullName",
				//template: '#:usrfirstname+" "+usrlastname+" " # </div> ',
				title: 'Первый руководитель',
				filterable: true,
				sortable: true,
				width: 130
			}, {
				field: "usrresponcefio",
				title: 'ФИО ответственного лица',
				filterable: false,
				sortable: true,
				width: 130
			}, {
				field: "OblastName",
				title: 'Область',
				filterable: false,
				sortable: true,
				width: 130
			}, {
				field: "Address",
				title: 'Адрес',
				filterable: true,
				sortable: true,
				width: 130
			}],
			resizable: true,
			filterable: false,
			change: function (e) { },
			dataBinding: function () {
				record = 0; //(this.dataSource.page() - 1) * this.dataSource.pageSize();
			}
		}).getKendoGrid();

		//----
		resizeGridsAll();

		//----
		$('.object-grid .k-grid-content').scroll(function () {
			var curElem = $(this);
			if (curElem.scrollTop() + curElem.height() >= this.scrollHeight) {

				qwe.paramQuery.scrollPageNum = qwe.paramQuery.scrollPageNum + 1;
				if (qwe.paramQuery.scrollPageNum <= qwe.paramQuery.pageCount) {
					qwe.paramQuery.pageNum = qwe.paramQuery.scrollPageNum;
					getData();
					console.log("refresh...");
				}
			}
		});

	}

	//----
	function createWindows() {

		//----область
		qwe.modalWindowOblast = $(".oblast-window").kendoWindow({
			width: "600px",
			height: "560px",
			title: "Область",
			visible: false,
			modal: false,
			resizable: true,
			draggable: true,
			actions: ["Close"],
			open: function () {
			}
		}).getKendoWindow().center();
	}

	//----
	function bindEventHandlers() {

		//----toggle
		$(".divpanel").toggle();
		$("a.lotLihk").click(function () {
			$(".divpanel").toggle();
		});

		//----refresh
		$('.btn-refresh').click(function () {
			getData();
		});

		//---- btn export excell
		$('.btn-export-excell').click(function () {
			exportExcel();
		});
		
		//---- Наименование keyup
		$(".search-name-idk-bin").on('keyup', function (e) {
			if (e.keyCode == 13) {
				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;

				qwe.paramQuery.name_idk_bin = $(this).val();
				getData();
			}
		});

		//----oblast
		$('.btn-oblast').click(function () {

			var ids = $('.input-oblast').attr('obj-id');
			var arr = ids.split(',');
			var chbxs = $('.oblast-ul input[obj-id]');
			//----
			$.map(chbxs, function (item) {
				$(item).attr("checked", false);
			});

			//----
			if (ids != -1) {
				for (var i = 0; i < arr.length; i++) {

					$.map(chbxs, function (item) {
						var id = $(item).attr('obj-id');
						if (arr[i] == id) {
							$(item).attr("checked", 'true');
						}
					});

				}
			} else {
				$.map(chbxs, function (item) {
					$(item).attr("checked", 'true');
				});
			}

			$('.label-oblast-error').text('');
			qwe.modalWindowOblast.open();

		})

		//----
		$('.oblast-ul').on('click', 'input.oblast-checkbox', function () {
			var id = $(this).attr('obj-id');
			var flag = false;
			if (id == -1) {
				if (this.checked)
					flag = true;

				//----
				var chbxs = $('.oblast-ul input[obj-id]');
				$.map(chbxs, function (item) {
					$(item).attr("checked", flag);
				});
			} else {

				var chbxs_checked = $('.oblast-ul input[obj-id]:checked');
				var chbxs = $('.oblast-ul input[obj-id]');

				if (chbxs.length == (chbxs_checked.length + 1) && this.checked == true) {
					$.map(chbxs, function (item) {
						$(item).attr("checked", true);
					});
				} else {

					if (chbxs.length == chbxs_checked.length)
						flag = true;

					$.map(chbxs, function (item) {
						var id = $(item).attr('obj-id');
						if (id == -1) {
							$(item).attr("checked", flag);
						}
					});

				}


			}
		})

		//----ok
		$('.btn-oblast-ok').click(function () {

			var chbxs = $('.oblast-ul input[obj-id]:checked');
			if (chbxs.length > 0) {

				var ids = "", texts = "", first_text = "";
				$.map(chbxs, function (item, indx) {
					ids += $(item).attr('obj-id');
					texts += $(item).parent().find('label').text();

					if (ids == -1)
						first_text = texts;

					if (indx != chbxs.length - 1) {
						ids += ",";
						texts += ",";
					}
				});

				if (ids.indexOf("-1") != -1) {
					ids = "-1";
					texts = first_text;
				}

				$('.input-oblast').val(texts);
				$('.input-oblast').attr("obj-id", ids);
				$('.input-oblast').attr('title', texts);

				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;
				while (ids.indexOf(",") != -1) {
					ids = ids.replace(",", "*");
				}

				qwe.modalWindowOblast.close();
				qwe.paramQuery.oblast_ids = ids;

				getData();

			} else $('.label-oblast-error').text("Не выбрано ");

		});

		//----close
		$('.btn-oblast-close').click(function () {
			qwe.modalWindowOblast.close();
		});

		//----reset
		$('.btn-reset').click(function () {

			qwe.paramQuery.pageNum = 1;
			qwe.paramQuery.scrollPageNum = 1;
			qwe.paramQuery.year = qwe.paramQuery.activeYear;
			qwe.paramQuery.pageCount = 1;
			qwe.paramQuery.allCount = 0;
			qwe.paramQuery.name_idk_bin = "";

			$('.search-name-idk-bin').val('');

			//----oblast
			qwe.paramQuery.oblast_ids = "-1";
			var chbxs = $('.oblast-ul input[obj-id]');
			$.map(chbxs, function (item, indx) {
				if (indx == 0) {
					var text = $(item).parent().find('label').text();
					$('.input-oblast').val(text).attr('obj-id', '-1');
				}
			});

			getData();

		})
	}

	//---- 
	function getYears() {

		$.post(rootUrl + 'SourceController/GetYears', function (data) {
			qwe.dropdownYear.dataSource.data(data);
			var isactive = 0;
			$.map(data, function (item, indx) {
				if (item.isactive)
					isactive = indx;
			});

			qwe.dropdownYear.select(isactive);
			qwe.paramQuery.year = qwe.dropdownYear.value();
			qwe.paramQuery.activeYear = qwe.paramQuery.year;
			getData();
		});
	}

	//----
	function getDics() {

		//---- fill
		$.post(rootUrl + 'SourceController/GetSomeDictionary', function (data) {

			if (data.ErrorMessage) {
				showWarning(data.ErrorMessage);
				return;
			}
			
			//----
			var $ul_oblast = $('.oblast-ul');
			$.map(data.dickatos, function (item, indx) {
				var style = "";
				if (indx == 0) {
					$('.input-oblast').val(item.Text);
					$('.input-oblast').attr('obj-id', item.Value);
					style = "font-weight: bold;";
				}

				var $li = "<li><input type='checkbox' class='oblast-checkbox' obj-id='" + item.Value + "'/><label style='" + style + "'>" + item.Text + "</label></li>";
				$ul_oblast.append($li);
			});
			
			
		});

	}
	
	//---- 
	function getData() {

		kendo.ui.progress($('.object-grid'), true);

		$.post(rootUrl + "AuditReestr/GetRstReportReestr", {
			pageNum: qwe.paramQuery.pageNum,
			year: qwe.paramQuery.year,
			calcDeep: qwe.paramQuery.calcDeep,
			oblast_ids: qwe.paramQuery.oblast_ids,
			name_idk_bin:qwe.paramQuery.name_idk_bin
		}, function (data) {

			kendo.ui.progress($('.object-grid'), false);

			if (data.ErrorMessage) {
				showWarning(data.ErrorMessage);
				return
			}

			//----
			$.map(data.ListItems, function (item) {
				item.FullName = checkEmptyOrWhiteSpace(item.usrlastname)+" "+ checkEmptyOrWhiteSpace(item.usrfirstname)+" "+ checkEmptyOrWhiteSpace(item.usrsecondname);
			});

			qwe.paramQuery.allCount = parseInt(data.AllCount);

			if (qwe.paramQuery.allCount % 50 != 0)
				qwe.paramQuery.pageCount = parseInt(qwe.paramQuery.allCount / 50) + 1;
			else qwe.paramQuery.pageCount = parseInt(qwe.paramQuery.allCount / 50);


			if (qwe.paramQuery.pageNum == 1) {
				qwe.tempSource.data = data.ListItems;
				qwe.grid.dataSource.data(qwe.tempSource.data);
			}
			else {
				qwe.tempSource.data = qwe.tempSource.data.concat(data.ListItems);
				qwe.grid.dataSource.data(qwe.tempSource.data);
			}

			var itemCount = data.ListItems.length;

			//----
			var div = ('.countGridRecord');
			var html = qwe.tempSource.data.length + " из " + data.AllCount;

			if (data.Count == data.AllCount)
				html = data.Count;

			$(div).html(html);
		});
	}

	//----helper 
	function checkEmptyOrWhiteSpace(term){	  
		if(term==="" || term==null)
			term="";
		return term;
	}

	//----
	function exportExcel() {
		var url = rootUrl + "AuditReestr/ExportExcel";
		url+="?year=" + qwe.paramQuery.year + "&calcDeep=" + qwe.paramQuery.calcDeep + "&oblast_ids=" + qwe.paramQuery.oblast_ids;
		//name_idk_bin:qwe.paramQuery.name_idk_bin

		window.location.href = url;
	}
});