$(function () {

	var qwe = {};
	window.removeDuplicate = qwe;

	createGrid();
	createWindow();
	bindEventHandler();


	function createWindow() {

		qwe.modalWindow = $('.duplicate-window').kendoWindow({
			width: "1200px",
			height: "800px",
			title: "Редактировать дубликаты",
			visible: false,
			modal: false,
			actions: ["Maximize", "Close"],
			resizable: true,
			draggable: true,
			open: function (e) { },
			close: function () { }
		}).getKendoWindow().center();

	}

	function bindEventHandler() {

		//----создать
		$('.btn-search').click(function () {
			if ($('.inp-search').val() != "") {
				GetDublicateBin($('.inp-search').val());
			}
		});

		//----
		$('.btn-refresh').click(function () {

			if ($('.inp-search').val() != "") {
				GetDublicateBin($('.inp-search').val());
			} else GetDublicateBin("");

		})

		//----очистить
		$('.btn-dclear').click(function () {
			$('.inp-search').val("");
			GetDublicateBin("");
		})

		$('.btn-clear').click(function () {
			qwe.searchgrid.dataSource.data([]);
		})

		//----поиск
		$('.btn-search').click(function () {
			search();
		})

		$('.inp-search-by-bin').keyup(function (e) {
			if (e.which == 13) {
				search();
			}
		});

		$('.inp-search-by-juridicalname').keyup(function (e) {
			if (e.which == 13) {
				search();
			}
		});

		$('.inp-search-by-idk').keyup(function (e) {
			if (e.which == 13) {
				search();
			}
		});

		//----выбрать
		$('.window-btn-execute').click(function () {

			var biniin = "";
			var chbxs = $('.search-grid td[role="gridcell"] input[u-id]:checked');

			if (!chbxs || chbxs.length < 1) {
				$('.error-validate').text("Объект не выбран");
				return;
			}
						
			$('.error-validate').text('');

			var checked_ids = [];			
			$.map(chbxs, function (item) {
				var id = $(item).attr('u-id');
				checked_ids.push(id);
				var dataItem = $.grep(qwe.searchgrid.dataSource.data(), function (e) { return e.user_id == id; })[0];
				biniin = dataItem.login;
			});
			var StrCheckedIds = checked_ids.join(',');

			var unchecked_ids = [];
			var unchecked = $('.search-grid td[role="gridcell"] input[u-id]:not(:checked)');//
			$.map(unchecked, function (item) {
				var id = $(item).attr('u-id');
				unchecked_ids.push(id);
			});

			var StrUncheckedIds = unchecked_ids.join(',');

			console.log("StrCheckedIds:", StrCheckedIds);
			console.log("StrUncheckedIds:", StrUncheckedIds);

			kendo.ui.progress($('.search-grid'), true);
			$.post(rootUrl + 'RemoveDuplicate/ExecuteDuplicate', { userId: StrCheckedIds, removedIds: StrUncheckedIds }, function (data) {
				console.log(data);
				kendo.ui.progress($('.search-grid'), false);

				if (data.ErrorMessage != "") {
					$('.error-validate').text(data.ErrorMessage);
					return;
				}

				SearchBin(biniin);
			});

			//qwe.modalWindow.close();
		});

		//----
		$('.window-btn-close').click(function () {
			qwe.modalWindow.close();
		});

		//----
		$('.object-grid').on('click', 'td button.biniin-edit', function () {
			var biniin = $(this).attr('biniin');
			SearchBin(biniin);
			qwe.modalWindow.open();
		});

		//----выбрат одно
		$('.search-grid').on('click', 'td .checkbox', function (e) {

			if (this.checked) {

				var chbxs = $('.search-grid td[role="gridcell"] input[u-id]:checked');
				$.map(chbxs, function (item) {
					$(item).attr("checked", false);
				});

				this.checked = true;
			}

		});

		//showConfirmation("@ResourceSetting.Delete", "Выполнить удаление дубликатов?", success, cancel);
	}

	GetDublicateBin("");

	function GetDublicateBin(biniin) {

		$.post(rootUrl + 'RemoveDuplicate/GetDublicateBin', { biniin: biniin }, function (data) {
			console.log(data);
			if (data.ErrorMessage != "") {
				showWarning(data.ErrorMessage);
				return;
			}
			qwe.grid.dataSource.data(data.ListItems);
		});
	}

	function SearchBin(biniin) {

		$.post(rootUrl + 'RemoveDuplicate/GetByBiniin', { biniin: biniin }, function (data) {
			console.log(data);
			qwe.searchgrid.dataSource.data(data.ListItems);
		});

	}

	function createGrid() {

		qwe.grid = $('.object-grid').kendoGrid({
			scrollable: true,
			sortable: true,
			selectable: 'row',
			columns: [
				{
					field: "biniin",
					title: "№ п\\п",
					template: '#= ++record #',
					width: 30,
					filterable: false
				}, {
					field: 'biniin',
					title: '@ResourceSetting.sBIN',
					width: 200
				}, {
					field: 'cnt',
					title: 'Количество',
					width: 200
				}, {
					field: "biniin",
					title: '...',
					type: 'number',
					template: '<button class="k-button biniin-edit" biniin="#=biniin#">Выбрать</button>',
					width: 60
				}
			],
			dataBinding: function () {
				record = 0; //(this.dataSource.page() - 1) * this.dataSource.pageSize();
			},
			resizable: true
		}).getKendoGrid();

		qwe.searchgrid = $('.search-grid').kendoGrid({
			scrollable: true,
			sortable: true,
			selectable: 'row',
			columns: [
				{
					field: "ID",
					title: " ",
					template: '<input type="checkbox" class="checkbox" style="margin-left: 15px;" u-id="#: data.user_id #" />',
					width: 40,
					filterable: false
				},
				{
					field: 'login',
					title: '@ResourceSetting.biniinSubject',
					width: 100
				}, {
					field: 'user_idk',
					title: '@ResourceSetting.IDK',
					width: 100
				}, {
					field: 'juridicalname',
					title: '@ResourceSetting.SubPerson',
					width: 200
				}, {
					field: 'oblast_name',
					title: '@ResourceSetting.Address',
					width: 150
				}, {
					field: 'kind_name',
					width: 200,
					title: '@ResourceSetting.KindUser',
					template: '# var arr_kind=(data.kind_name)?data.kind_name.split(","):[];  var tbl=""; '
							  + 'for(var i=0;i<arr_kind.length;i++) {'
							  + '  tbl+="<div>"+arr_kind[i]+"</div>" '
							  + ' }'
							  + '# #=tbl #'
				}, {
					field: 'sub_status',
					width: 200,
					title: '@ResourceSetting.RegisterForm',
					template: '# var arr_status=(data.sub_status)?data.sub_status.split(","):[]; var arr_year=(data.sub_year)?data.sub_year.split(","):[]; var tbl=""; '
							  + 'for(var i=0;i<arr_status.length;i++) {'
							  + '  tbl+="<div>"+arr_year[i]+" | "+arr_status[i]+"</div>" '
							  + ' }'
							  + '# #=tbl #'
				}, {
					field: 'rst_',
					title: 'В реестре',
					//width: 200,
					template: '# var arr_owner=(data.rst_ownername)?data.rst_ownername.split(","):[]; var rst_year=(data.rst_year)?data.rst_year.split(","):[]; var rst_idk=(data.rst_idk)?data.rst_idk.split(","):[]; var tbl=""; '
							  + 'for(var i=0;i<arr_status.length;i++) {'
							  + '  tbl+="<div style=\'padding:5px;border-bottom-color:black;\'>"+arr_year[i]+" | "+rst_idk[i]+" | "+arr_owner[i]+"</div>" '
							  + ' }'
							  + '# #=tbl #'
				}
			],
			resizable: true
		}).getKendoGrid();
	}


	function search() {

		var bin = $('.inp-search-by-bin').val();
		var juridicalname = $('.inp-search-by-juridicalname').val();
		var idk = $('.inp-search-by-idk').val();

		if (bin == "" && juridicalname == "" && idk == "")
			return;

		kendo.ui.progress($('.search-grid'), true);
		$.post(rootUrl + 'RemoveDuplicate/SearchUsers', {
			bin: bin,
			juridicalname: juridicalname,
			idk: idk
		}, function (data) {

			kendo.ui.progress($('.search-grid'), false);

			console.log(data);
			qwe.searchgrid.dataSource.data(data);
		});
	}

});



  //qwe.grid = $('.object-grid').kendoGrid({
  //	scrollable: true,
  //	sortable: true,
  //	selectable: 'row',
  //	columns: [
  //  	{
  //  		field: "Id",
  //  		title: " ",
  //  		template: '<input type="checkbox" class="checkbox" style="margin-left: 15px;" obj-id="#: data.Id #" />',
  //  		width: 30,
  //  		filterable: false
  //  	},
  //  	{
  //  		field: 'ApplicationName',
  //  		title: '@ResourceSetting.Name',
  //  		width: 200
  //  	}, {
  //  		field: 'BINIIN',
  //  		title: '@ResourceSetting.sBIN',
  //  		width: 200
  //  	}, {
  //  		field: 'FullName',
  //  		title: '@ResourceSetting.boss',
  //  		width: 200
  //  	}, {
  //  		field: 'TypeNames',
  //  		title: '@ResourceSetting.KindUser',
  //  		width: 200
  //  	}, {
  //  		field: 'IDK',
  //  		title: '@ResourceSetting.IDK',
  //  		width: 200
  //  	}, {
  //  		field: "Id",
  //  		title: 'х',
  //  		type: 'number',
  //  		template: '<button class="del k-button" del-id="#=Id#">Исключить</button>',
  //  		width: 60
  //  	}
  //	],
  //	resizable: true
  //}).getKendoGrid();
