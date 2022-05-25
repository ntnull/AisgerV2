$(function () {

	var qwe = {};
	window.qwe = qwe;
	initVars();
	createControllers();
	getYears();
	getDics();
	bindEventHandlers();
	setLabelText();
	createWindows();
	createSumGrid();
	function initVars() {
		//----
		qwe.tempSource = {};
		qwe.paramQuery = {};
		qwe.paramQuery.columnsVal = "NotOwnSource";
		qwe.paramQuery.pageNum = 1;
		qwe.paramQuery.scrollPageNum = 1;
		qwe.paramQuery.year = 2015;
		qwe.paramQuery.pageCount = 1;
		qwe.paramQuery.allCount = 0;
		qwe.paramQuery.oblast_ids ="-1";
		qwe.paramQuery.excluded_id = 0;
		qwe.paramQuery.isshowtut = false;
		qwe.paramQuery.restype_id = -1;		
		qwe.paramQuery.min = 0;
		qwe.paramQuery.max = 0;
		qwe.paramQuery.orderBy = -1;
		qwe.paramQuery.name_oked_idk = "";

		//----
		qwe.paramQuery.fscode = "";
		qwe.paramQuery.oked_ids = "";
		qwe.paramQuery.reason_ids = "";
		qwe.paramQuery.expectant_ids = "";

		//----
		qwe.paramQuery.user_id = -1;
		qwe.paramQuery.sub_form_id = -1;

	    //----
		qwe.paramQuery.isplan = -1;
		qwe.paramQuery.isem_system = -1;

		qwe.paramQuery.sumData = [];

	}

	//----
	function createGrids(columns) {
		//----
		qwe.grid = $('.object-source-controller-grid').kendoGrid({
			scrollable: true,
			sortable: false,
			width: '98%',
			//height: '80%',
			selectable: 'row',
			columns: columns,
			resizable: true,
			filterable: false,
			//filterable: zxc.kendo.grid.filterable,
			change: function (e) { },
			dataBinding: function () {
				record = 0; //(this.dataSource.page() - 1) * this.dataSource.pageSize();
			}
		}).getKendoGrid();

		//----
		resizeGridsAll();

		//----
		$('.object-source-controller-grid .k-grid-content').scroll(function () {
			var curElem = $(this);
			if (curElem.scrollTop() + curElem.height() >= this.scrollHeight) {

				qwe.paramQuery.scrollPageNum = qwe.paramQuery.scrollPageNum + 1;
				if (qwe.paramQuery.scrollPageNum <= qwe.paramQuery.pageCount) {
					qwe.paramQuery.pageNum = qwe.paramQuery.scrollPageNum;
					getNotOwnResource(false, true);
					console.log("refresh");
				}
			}
		});

	}

	function createSumGrid() {

	    //----
	    qwe.sumGrid = $('.sum-grid').kendoGrid({
	        scrollable: true,
	        sortable: false,
	        width: '98%',
	        //height: '80%',
	        selectable: 'row',
	        columns: [
                {
                    field: "ROWNUMBER",
                    title: "№ п\\п",
                    width: 45,
                    filterable: false,
                    template: '#= ++record2 #'
                }, {
                    field: "title",
                    title: 'Наименование колонки',
                    filterable: true,
                    sortable: true,
                    width: 130
                }, {
                    field: "val",
                    title: 'Итого',
                    filterable: true,
                    sortable: true,
                    width: 130
                }
	        ],
	        resizable: true,
	        filterable: false,
	        change: function (e) { },
	        dataBinding: function () {
	            record2 = 0; //(this.dataSource.page() - 1) * this.dataSource.pageSize();
	        }
	    }).getKendoGrid();
	}

	//----
	function createControllers() {
		
		//----toggle
		$(".divpanel").toggle();
		$("a.lotLihk").click(function () {
			$(".divpanel").toggle();
		});

		//----menu
		$(".column-menu").kendoMenu({
			select: function () {

			}
		});

		//----year
		qwe.dropdownYear = $('.select-year').kendoDropDownList({
			dataTextField: "ReportYear",
			dataValueField: "ReportYear",
			change: function () {
				var year = qwe.dropdownYear.value();
				qwe.paramQuery.year = year;
				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;

				getNotOwnResource();
			}
		}).getKendoDropDownList();

		//---- resources value
		qwe.dropdownColumnsVal = $('.select-columns-value').kendoDropDownList({
			dataTextField: "Text",
			dataValueField: "Value",
			change: function () {
				qwe.paramQuery.columnsVal = qwe.dropdownColumnsVal.value();
				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;

				getNotOwnResource();
			}
		}).getKendoDropDownList();
						
		//---- excluded
		qwe.dropdownExcluded = $('.select-excluded').kendoDropDownList({
			dataTextField: "NAME_RU",
			dataValueField: "ID",
			change: function () {
				qwe.paramQuery.excluded_id = qwe.dropdownExcluded.value();
				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;
				getNotOwnResource();
			}
		}).getKendoDropDownList();

		//---- type res 
		qwe.dropdownTypeRes = $('.select-typeres').kendoDropDownList({
			dataTextField: "resource_name",
			dataValueField: "resource_id",
			change: function () {

				qwe.paramQuery.restype_id = qwe.dropdownTypeRes.value();
				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;
			}
		}).getKendoDropDownList();
		
		//---- order by
		qwe.dropdownOrderBy = $('.select-orderby').kendoDropDownList({
			dataTextField: "resource_name",
			dataValueField: "resource_id",
			change: function () {

				qwe.paramQuery.orderBy = qwe.dropdownOrderBy.value();
				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;
				getNotOwnResource();
			}
		}).getKendoDropDownList();
		
		//----fscode
		$('.multiple-fscode-selected').multiselect({
			includeSelectAllOption: false,
			maxHeight: 400
		});

		//----reason
		$('.multiple-reason-selected').multiselect({
			includeSelectAllOption: false,
			maxHeight: 400
		});

		//----expectant
		$('.multiple-expectant-selected').multiselect({
			includeSelectAllOption: false,
			maxHeight: 400
		}); 

	    //---- isplan 
		qwe.dropdownIsPlan = $('.select-isplan').kendoDropDownList({
		    dataTextField: "Name",
		    dataValueField: "Id",
		    change: function () {

		        qwe.paramQuery.isplan = qwe.dropdownIsPlan.value();
		        qwe.paramQuery.pageCount = 0;
		        qwe.paramQuery.pageNum = 1;
		    }
		}).getKendoDropDownList();

	    //---- isem-system 
		qwe.dropdownIsEmSystem = $('.select-isem-system').kendoDropDownList({
		    dataTextField: "Name",
		    dataValueField: "Id",
		    change: function () {

		        qwe.paramQuery.isem_system = qwe.dropdownIsEmSystem.value();
		        qwe.paramQuery.pageCount = 0;
		        qwe.paramQuery.pageNum = 1;
		    }
		}).getKendoDropDownList();

		//---------------------------windows controll

		//---- select-expectant
		qwe.dropdownExpectantWindow = $('.select-expectant-window').kendoDropDownList({
			dataTextField: "Name",
			dataValueField: "Id",
			change: function () {

			}
		}).getKendoDropDownList();
	}

	//----
	function createWindows() {
		//----область
		qwe.modalWindowOblast = $(".oblast-window").kendoWindow({
			width: "600px",
			height: "535px",
			title: "Область",
			visible: false,
			modal: false,
			resizable: true,
			draggable: true,
			actions: ["Close"],
			open: function () {

			}
		}).getKendoWindow().center();

		//----завершение согл. отказ
		qwe.modalWindowEditControlParam = $(".edit-controlls-params-window").kendoWindow({
			width: "600px",
			height: "300px",
			title: "Редактировать контрольные параметры",
			visible: false,
			modal: false,
			resizable: true,
			draggable: true,
			actions: ["Close"],
			open: function () {

			}
		}).getKendoWindow().center();


		//----окэд
		qwe.modalWindowOked = $(".oked-window").kendoWindow({
			width: "600px",
			height: "450px",
			title: "ОКЭД",
			visible: false,
			modal: false,
			resizable: true,
			draggable: true,
			actions: ["Close"],
			open: function () {

			}
		}).getKendoWindow().center();

	    //----Итоговые значения
		qwe.modalWindowSum = $(".sum-window").kendoWindow({
		    width: "600px",
		    height: "500px",
		    title: "Итоговые значения",
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

		//----window click events
		$(window).click(function (e) {

			//----
			var target_classname = e.target.className;
			if (target_classname.indexOf('btn-columns-editor') == -1 && target_classname.indexOf('column-editor-1') == -1 && target_classname.indexOf('column-editor-2') == -1 && target_classname.indexOf('editor-li') == -1) {

				var val = $('.columns-editor-menu').attr('isopen');
				var isopen = (val == 'true');
				if (isopen)
					columnsEditor(isopen);

			}
		});
		//----
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

		//----
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

				getNotOwnResource();				

			} else $('.label-oblast-error').text("Не выбрано ");

		});

		//----
		$('.btn-oblast-close').click(function () {
			qwe.modalWindowOblast.close();
		})

		//----refresh
		$('.btn-refresh').click(function () {
			getNotOwnResource();
		});

		//---- refresh
		$('.btn-reset').click(function () {

			qwe.paramQuery.columnsVal = "NotOwnSource";
			qwe.paramQuery.pageNum = 1;
			qwe.paramQuery.scrollPageNum = 1;
			qwe.paramQuery.year = 2015;
			qwe.paramQuery.pageCount = 1;
			qwe.paramQuery.allCount = 0;
			qwe.paramQuery.oblast_id = -1;
			qwe.paramQuery.excluded_id = 0;
			
			qwe.paramQuery.isshowtut = false;
			qwe.paramQuery.restype_id = -1;
			qwe.paramQuery.min = 0;
			qwe.paramQuery.max = 0;
			qwe.paramQuery.orderBy = -1;
			qwe.paramQuery.name_oked_idk = "";
			
			//----
			qwe.paramQuery.user_id = -1;
			qwe.paramQuery.sub_form_id = -1;

			$('.restype-min').val('');
			$('.restype-max').val('');
			$('.search-name-idk-oked').val('');

			qwe.dropdownColumnsVal.select(0);
			qwe.dropdownExcluded.select(0);
			qwe.dropdownOrderBy.select(0);
			qwe.dropdownTypeRes.select(0);
			//qwe.dropdownYear.select(3);
				
			//----
			qwe.paramQuery.oked_ids = "";
			$('.input-oked').val('').attr('obj-id', '');

			//----oblast
			qwe.paramQuery.oblast_ids = "-1";
			var chbxs = $('.oblast-ul input[obj-id]');			
			$.map(chbxs, function (item, indx) {
				if (indx == 0) {
					var text=$(item).parent().find('label').text();
					$('.input-oblast').val(text).attr('obj-id', '-1');
				}
			});

		

			//----expectant
			$('.multiple-expectant-selected option:selected').each(function () {
				$(this).prop('selected', false);
			})
			$('.multiple-expectant-selected').multiselect('refresh');
			qwe.paramQuery.expectant_ids = "";

			//----reason
			$('.multiple-reason-selected option:selected').each(function () {
				$(this).prop('selected', false);
			})
			$('.multiple-reason-selected').multiselect('refresh');
			qwe.paramQuery.reason_ids = "";

			//----fscode
			$('.multiple-fscode-selected option:selected').each(function () {
				$(this).prop('selected', false);
			})
			$('.multiple-fscode-selected').multiselect('refresh');
			qwe.paramQuery.fscode = "";
						
			getNotOwnResource();
		});

		//---- column editor
		$('.btn-columns-editor').click(function (event) {

			var val = $('.columns-editor-menu').attr('isopen');
			var isopen = (val == 'true');

			if (isopen == false)
				$('.columns-editor-menu').removeClass('hide');
			else
				$('.columns-editor-menu').addClass('hide');

			$('.columns-editor-menu').attr('isopen', (!isopen).toString());

		});

		//----
		$('.columns-editor-menu li').click(function () {
			var classname = $(this).attr('input-class');
			$('.' + classname).trigger("click");
		})

		//----
		$('.column-editor-1').click(function (e) {
			e.stopPropagation();

			var flag = $(this).is(':checked');
			kendo.ui.progress($('.object-source-controller-grid'), true);
			setTimeout(function () {

				if (flag) {
					for (var i = 0; i < 4; i++)
						qwe.grid.showColumn(4 + i);
				} else {
					for (var i = 0; i < 4; i++)
						qwe.grid.hideColumn(4 + i);
				}
				kendo.ui.progress($('.object-source-controller-grid'), false);

			}, 100);
		});

		//----
		$('.column-editor-2').click(function (e) {
			e.stopPropagation();
			var flag = $(this).is(':checked');
			kendo.ui.progress($('.object-source-controller-grid'), true);
			setTimeout(function () {

				if (flag) {
					for (var i = 0; i < 28; i++)
						qwe.grid.showColumn(10 + i);
				} else {
					for (var i = 0; i < 28; i++)
						qwe.grid.hideColumn(10+ i);
				}
				kendo.ui.progress($('.object-source-controller-grid'), false);

			}, 100);
		});

		//---- btn export excell
		$('.btn-export-excell').click(function () {
			exportExcel();
		});

		//---- т.у.т 
		$('.btn-showtut').click(function () {

			if (this.checked) {
				qwe.paramQuery.isshowtut = true;
				$('.btn-showtut').prop('checked', true);
			} else {
				qwe.paramQuery.isshowtut = false;
				$('.btn-showtut').prop('checked', false);
			}

			qwe.paramQuery.pageCount = 0;
			qwe.paramQuery.pageNum = 1;
			getNotOwnResource();
		});

		//---- Наименование keyup
		$(".search-name-idk-oked").on('keyup', function (e) {
			if (e.keyCode == 13) {
				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;

				qwe.paramQuery.name_oked_idk = $(this).val();
				getNotOwnResource();
			}
		});

		//----
		$(".restype-min").on('keyup', function (e) {
			if (e.keyCode == 13) {
				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;
				getNotOwnResource();
			}
		});

		//----
		$(".restype-max").on('keyup', function (e) {
			if (e.keyCode == 13) {
				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;
				getNotOwnResource();
			}
		});

		//----
		$('.object-source-controller-grid').on('hover', 'td', function () {

			if ($(this).find('.obj-grid-context')) {

				var chbxs = $('.object-source-controller-grid td[role="gridcell"] span.obj-grid-context-menu');
				$.map(chbxs, function (item) {
					$(item).removeClass('show');
					$(item).addClass('hide');
				});

				$(this).find('span.obj-grid-context-menu').addClass('show');
			}
		});

		//----
		$('.object-source-controller-grid').on('click', 'td .obj-grid-context-menu', function (event) {

			qwe.paramQuery.user_id = $(this).attr('user-id');;
			qwe.paramQuery.sub_form_id = $(this).attr('sub-form-id');;

			$('.context-menu-list').removeClass('hide');
			menu(event, qwe.paramQuery.user_id);

		});

		$('.context-menu-list').on('mouseleave', function () {
			$('.context-menu-list').addClass('hide');
		});

		//=====================context menu
		//----
		$('.link-subject-info').click(function () {

		    var host = window.location.protocol + "//" + window.location.host + rootUrl;
		    //todoselect-year
		    var url = host + "SubjectInfo/Index?id=" + qwe.paramQuery.user_id + "&year=" + qwe.dropdownYear.value(); ''
			console.log("url", url);

			window.open(url, '_blank');
		})

		//----	https://aisger.kz:81/AppForm/ApplicationEdit/85175
		$('.link-edit-details').click(function () {
			if (qwe.paramQuery.sub_form_id != null) {

				var host = window.location.protocol + "//" + window.location.host + rootUrl;
				var url = host + "AppForm/ApplicationEdit/" + qwe.paramQuery.user_id;
				window.open(url, '_blank');

			}
		});

		//----
		$('.link-app-form-showdetails').click(function () {
			if (qwe.paramQuery.sub_form_id != null) {

				var host = window.location.protocol + "//" + window.location.host + rootUrl;
				var url = host + "AppForm/ShowDetails/" + qwe.paramQuery.sub_form_id;
				window.open(url, '_blank');

			}
		});

		//----
		$('.link-rst-report-subjectcard').click(function () {
			var user_id = $(this).parent().parent().attr('user-id');

			var host = window.location.protocol + "//" + window.location.host + rootUrl;
			var url = host + "RstReport/SubjectCard?secId=" + user_id + "&reportYear=" + qwe.dropdownYear.value();
			window.open(url, '_blank');
		});

		//----
		$('.edit-controll-parameters').click(function () {

			var uid = -1;
			$.map(qwe.grid.dataSource.data(), function (item) {
				if (item.user_id == qwe.paramQuery.user_id) {
					uid = item.uid;
				}
			});

			qwe.paramQuery.dataItem = qwe.grid.dataSource.getByUid(uid);
			console.log(qwe.paramQuery.dataItem);

			//----fill
			$('.lbl-juridical-name').text(qwe.paramQuery.dataItem.juridical_name);

			//----
			var buttons = $('.btn-exclusion');
			$.map(buttons, function (item) {
				if ($(item).hasClass('btn-exclusion-selected'))
					$(item).removeClass('btn-exclusion-selected');

			});

			if (qwe.paramQuery.dataItem.isexcluded)
				$('.btn-exclusion[obj-id=1]').addClass('btn-exclusion-selected');
			else $('.btn-exclusion[obj-id=2]').addClass('btn-exclusion-selected');


			//----
			var selected_indx = 0;
			if (qwe.paramQuery.dataItem.expectant_id != null) {
				$.map(qwe.dropdownExpectantWindow.dataSource.data(), function (item, indx) {
					if (qwe.paramQuery.dataItem.expectant_id == item.Id)
						selected_indx = indx;
				});
			} else selected_indx = qwe.dropdownExpectantWindow.dataSource.data().length - 1;

			qwe.dropdownExpectantWindow.select(selected_indx);

			//----
			var buttons = $('.btn-fscode');
			$.map(buttons, function (item) {
				if ($(item).hasClass('btn-fscode-selected'))
					$(item).removeClass('btn-fscode-selected');

			});

			if (qwe.paramQuery.dataItem.fscode != null) {
				$('.btn-fscode[obj-id=' + qwe.paramQuery.dataItem.fscode + ']').addClass('btn-fscode-selected');
			} else $('.btn-fscode[obj-id=0]').addClass('btn-fscode-selected');

			$('.context-menu-list').addClass('hide');
			qwe.modalWindowEditControlParam.open();
		});

		//----
		$('.exclusion-window-div').on('click', '.btn-exclusion', function () {
			var buttons = $('.btn-exclusion');
			$.map(buttons, function (item) {
				if ($(item).hasClass('btn-exclusion-selected'))
					$(item).removeClass('btn-exclusion-selected');

			});

			$(this).addClass('btn-exclusion-selected');
		});

		//----
		$('.typeapplication-window-div').on('click', '.btn-fscode', function () {
			var buttons = $('.btn-fscode');
			$.map(buttons, function (item) {
				if ($(item).hasClass('btn-fscode-selected'))
					$(item).removeClass('btn-fscode-selected');

			});
			$(this).addClass('btn-fscode-selected');
		});

		//----save
		$('.btn-ecpw-save').click(function () {

			//----fscode
			var typeapplication_val = $('.btn-fscode-selected').attr('obj-id');

			//----
			var exclusion_val = $('.btn-exclusion-selected').attr('obj-id');

			console.log("typeapplication_val", typeapplication_val);
			console.log("exclusion_val", exclusion_val);
			console.log("expactant_val=", qwe.dropdownExpectantWindow.value());

			$.post(rootUrl + 'SourceController/EditControlParam', {
				rst_id: qwe.paramQuery.dataItem.rst_id,
				user_id: qwe.paramQuery.dataItem.user_id,
				exclusion: (exclusion_val == 1) ? true : false,
				fscode: typeapplication_val,
				expectant_id: qwe.dropdownExpectantWindow.value()
			}, function (data) {

				if (data.ErrorMessage) {
					showWarning(data.ErrorMessage);
					return;
				}
				qwe.modalWindowEditControlParam.close();
				getNotOwnResource();
			});

		})

		//----close
		$('.btn-ecpw-close').click(function () {
			qwe.modalWindowEditControlParam.close();
		});

		//---- oked ...
		$('.btn-oked').click(function () {

			var ids = $('.input-oked').attr('obj-id');
			if (ids) {
				var arr = ids.split(',');
				var chbxs = $('.oked-ul input[obj-id]');
				//----
				$.map(chbxs, function (item) {
					$(item).attr("checked", false);
				});

				//----
				for (var i = 0; i < arr.length; i++) {
					$.map(chbxs, function (item) {
						var id = $(item).attr('obj-id');
						if (arr[i] == id) {
							$(item).attr("checked", 'true');
						}
					});
				}
			}
			$('.label-oked-error').text('');
			qwe.modalWindowOked.open();
		});

		//----oked ok
		$('.btn-oked-ok').click(function () {

			var chbxs = $('.oked-ul input[obj-id]:checked');
			
				var ids = "", texts = "";
				$.map(chbxs, function (item, indx) {
					ids += $(item).attr('obj-id');
					texts += $(item).parent().find('label').text();
					if (indx != chbxs.length - 1) {
						ids += ",";
						texts += ",";
					}
				});

				$('.input-oked').val(texts);
				$('.input-oked').attr("obj-id", ids);
				$('.input-oked').attr('title', texts);

				qwe.paramQuery.pageCount = 0;
				qwe.paramQuery.pageNum = 1;
				while (ids.indexOf(",") != -1) {
					ids = ids.replace(",", "*");
				}

				qwe.modalWindowOked.close();
				qwe.paramQuery.oked_ids = ids;

				getNotOwnResource();
		});

		//----oked close
		$('.btn-oked-close').click(function () {
			qwe.modalWindowOked.close();
		});

		//----checked all
		$('.oked-checkbox-all').click(function () {

			var flag = false;
			if (this.checked)
				flag = true;

			//----
			var chbxs = $('.oked-ul input[obj-id]');
			$.map(chbxs, function (item) {
				$(item).attr("checked", flag);
			});

		});

		//----search input
		$('.input-oked-search').on('keyup', function (e) {
			var filter = $(this).val().toUpperCase();
			console.log("filter:", filter);

			var li = $(".oked-ul li");
			$.map(li, function (item) {

				var term = $(item).find('label').text();

				if (term.toUpperCase().indexOf(filter) > -1) {
					$(item).removeClass('hide');
				} else {
					//li[i].style.display = "none";
					$(item).addClass('hide');
				}

			});

		});

		//----clear all
		$('.span-oked-clear').click(function () {
		    $('.input-oked-search').val('');

		    //----
		    var chbxs = $('.oked-ul input[obj-id]');
		    $.map(chbxs, function (item) {
		        $(item).attr("checked", false);
		    });

		});


	    //----
		$('.btn-sum').click(function () {
		    getNotOwnSourceSum();
		    qwe.modalWindowSum.open();
		});

		$('.btn-sum-close').click(function () {
		    qwe.modalWindowSum.close();
		});

		$('.btn-sum-excell').click(function () {


		    var url = rootUrl + "SourceController/ExportSumExcel?year=" + qwe.paramQuery.year + "&oblast_ids=" + qwe.paramQuery.oblast_ids + "&reason_ids=" + qwe.paramQuery.reason_ids + "&excluded_id=" + qwe.paramQuery.excluded_id;
		    url = url + "&expectant_ids=" + qwe.paramQuery.expectant_ids + "&fscode=" + qwe.paramQuery.fscode + "&oked_ids=" + qwe.paramQuery.oked_ids + "&isshowtut=" + qwe.paramQuery.isshowtut + "&restype_id=" + qwe.paramQuery.restype_id + "&min=" + qwe.paramQuery.min + "&max=" + qwe.paramQuery.max + "&orderBy=" + qwe.paramQuery.orderBy + "&isplan=" + qwe.paramQuery.isplan + "&isem_system=" + qwe.paramQuery.isem_system + "&name_oked_idk=" + qwe.paramQuery.name_oked_idk;;
		    window.location.href = url;
		});
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
			getResourceType();
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

			console.log("some dic", data);
			//data.excludeds.unshift({ ID: 0, NAME_RU: '-' });

			qwe.dropdownExcluded.dataSource.data(data.excludeds);
			qwe.dropdownColumnsVal.dataSource.data(data.diccolumns);	
						
			//----
			var $ul_oblast = $('.oblast-ul');
			$.map(data.dickatos, function (item, indx) {
				var style = "";
				if (indx == 0) {
					$('.input-oblast').val(item.Text);
					$('.input-oblast').attr('obj-id', item.Value);
					style = "font-weight: bold;";
				}

				var $li = "<li><input type='checkbox' class='oblast-checkbox' obj-id='" + item.Value + "'/><label style='"+style+"'>" + item.Text + "</label></li>";
				$ul_oblast.append($li);
			});

			//---- windows
			var exp_buffer = $.grep(data.expectants, function (item) { return item.Id != 0; });
			qwe.dropdownExpectantWindow.dataSource.data(exp_buffer);

			//----
			var $div_e = $('.exclusion-window-div');
			$.map(data.excludeds2, function (item, indx) {
			    var $html = '<button class="btn-exclusion" obj-id="' + item.ID + '"  >' + item.NAME_RU + '</button>';
			    $div_e.append($html);
			});

		    //----
			console.log("fscodes=",data.fscodes);
			var $div_t = $('.typeapplication-window-div');
			$.map(data.fscodes, function (item, indx) {
				//if (indx > 0) {
					var val = item.Value;
					if (val == "")
						val = 0;

					//var $html = '<input type="radio" name="typeapplication"  value="' + val + '" data-type="date" style="vertical-align:middle;margin:0px; margin-top:-2px;" /><span style="display:inline-block;padding-left:5px;">' + item.Text + '</span>';
					var $html = '<button class="btn-fscode" obj-id="' + val + '"  >' + item.Text + '</button>';
					//if (indx != data.fscodes.length - 1)
					//	$html += ' &nbsp|&nbsp';

					$div_t.append($html);
				//}
			});

			//---- oked
			var $ul = $('.oked-ul');
			$.map(data.dicokeds, function (item) {
				var val = (item.Value == "") ? "null" : item.Value;
				var $li = "<li><input type='checkbox' class='oked-checkbox' obj-id='" + val + "'/><label>" + item.Text + "</label></li>";
				$ul.append($li);
			});

			//----fscode
			var $options_fscode = [];
			$.map(data.fscodes, function (item) {
				var val = (item.Value == "") ? "null" : item.Value;
				$options_fscode.push({ label: item.Text, title: item.Text, value: val });
			});
			$('.multiple-fscode-selected').multiselect('dataprovider', $options_fscode);

			//----reason
			var $options_reason = [];
			$.map(data.reasons, function (item) {
				var val = (item.Value == "") ? "null" : item.Value;
				$options_reason.push({ label: item.Text, title: item.Text, value: val });
			});
			$('.multiple-reason-selected').multiselect('dataprovider', $options_reason);

			//----
			var $options_expectant = [];
			$.map(data.expectants, function (item) {
				var val = (item.Id == "" || item.Id == null) ? "null" : item.Id;
				$options_expectant.push({ label: item.Name, title: item.Name, value: val });
			});
			$('.multiple-expectant-selected').multiselect('dataprovider', $options_expectant);

		    //----isplan and isEmSystem
			data.isplan.unshift({ Id: -1, Name: '-' });
			qwe.dropdownIsPlan.dataSource.data(data.isplan);
			qwe.dropdownIsEmSystem.dataSource.data(data.isplan);

		});

	}
	
	//----
	function getResourceType() {

		var columns = [{
			field: "ROWNUMBER",
			title: "№ п\\п",
			width: 45,
			filterable: false,
			template: '#= ++record #'
		}, {
			field: "idk",
			title: 'ИДК',
			filterable: true,
			sortable: true,
			width: 130
		}, {
		    field: "bin",
		    title: 'БИН',
		    filterable: true,
		    sortable: true,
		    width: 130
		},
        {
			field: "oked_name",
			title: 'ОКЭД',
			filterable: false,
			sortable: true,
			width: 130
		}, {
			field: "juridical_name",
			template: '<div class="obj-grid-context"><span class="obj-grid-context-menu hide" user-id="#:user_id#"  sub-form-id="#:sub_form_id#">⋮</span><br/> #:juridical_name # </div> ',
			title: 'Наименование',
			filterable: true,
			sortable: true,
			width: 130
		}, {
			field: "oblast_name",
			title: 'Область ',
			filterable: true,
			sortable: true,
			width: 130
		}, {
			field: "fscode_name",
			title: 'Вид собственности',
			filterable: true,
			sortable: true,
			width: 130
		}, {
			field: "excluded_name",
			title: 'Исключенность ',
			filterable: true,
			sortable: true,
			width: 130
		}, {
			field: "expectant_name",
			title: 'Причина исключения',
			filterable: true,
			sortable: true,
			width: 130
		},
        {
            field: "isplan",
            title: 'Энергоаудит проводился',
            filterable: true,
            sortable: true,
            width: 130
        },
		{
		    field: "isem_system",
		    title: 'Система энергоменеджмента внедрена',
		    filterable: true,
		    sortable: true,
		    width: 150
		},
		{
		    field: "consumption",
			title: 'Общее потребление т.у.т',
			filterable: true,
			sortable: true,
			template: '# if(data.consumption) {#<span style="float:right">#=consumption #</span> #}#',
			width: 130
		},
        {
            field: "sum_expenceenergy",
            title: 'Расходы в тенге (с учетом НДС)',
            filterable: true,
            sortable: true,
            template: '# if(data.sum_expenceenergy2) {#<span style="float:right">#=sum_expenceenergy2 #</span> #}#',
            width: 130
        }/*,
		{
		    field: "tut",
			title: 'Суммарно',
			filterable: true,
			sortable: true,
			template: '# if(data.tut) {#<span style="float:right">#=tut #</span> #}#',
			width: 150
		}*/];

		$.post(rootUrl + "SourceController/getResourceType", function (data) {

		    console.log("types=", data);

		    qwe.paramQuery.sumData.push({
		        title: "Суммарно",
		        field: "tut",
		        val: 0
		    });

			$.map(data, function (item, indx) {

			    var _columnName = "noS" + item.resource_id;
			    if (item.dic_type == 1) {
			        _columnName = "coV" + item.resource_id;
			    }

			    if (item.dic_type == 2) {

                    if(item.resource_id==1)
                        _columnName = "countOfEmployees";
                    if (item.resource_id == 2)
                        _columnName = "countOfStudents";
                    if (item.resource_id == 3)
                        _columnName = "countOfBeds";

			    }


				columns.push({
					field:_columnName,
					title: item.resource_name,
					headerTemplate: '<span title="' + item.resource_name + '(' + item.unit_name + ')">' + item.resource_name.substring(0, 20) + '</span>',
					template: '# if(data.' +_columnName+ ') {#<span style="float:right">#= ' + _columnName + ' #</span> #}#',
					type: "number",
					width: 130
				});


				

				qwe.paramQuery.sumData.push({
				    title: item.resource_name,
				    field: _columnName,
				    val: 0
				});

			});
            
			createGrids(columns);
			getNotOwnResource();

			var resData = data;

			resData.unshift({ resource_name: "Общее потребление т.у.т", resource_id: 0 });
			resData.unshift({ resource_name: "-", resource_id: -1 });

			qwe.dropdownTypeRes.dataSource.data(resData);
			
		});

	}

	//---- 
	function getNotOwnResource() {

		//----
		qwe.paramQuery.min = $('.restype-min').val();
		qwe.paramQuery.max = $('.restype-max').val();
		qwe.paramQuery.name_oked_idk = $('.search-name-idk-oked').val();

		if (qwe.paramQuery.columnsVal == 'ExpenceEnergy')
			qwe.paramQuery.isshowtut = false;
		
		//----
		getMultiSelectedValues();

		kendo.ui.progress($('.object-source-controller-grid'), true);
		$.post(rootUrl + "SourceController/GetNotOwnResources", {
			columnsVal: qwe.paramQuery.columnsVal,
			year: qwe.paramQuery.year,
			oblast_ids: qwe.paramQuery.oblast_ids,
			reason_ids: qwe.paramQuery.reason_ids,
			excluded_id: qwe.paramQuery.excluded_id,
			expectant_ids: qwe.paramQuery.expectant_ids,
			fscode: qwe.paramQuery.fscode,
			oked_ids: qwe.paramQuery.oked_ids,
			pageNum: qwe.paramQuery.pageNum,
			isshowtut: qwe.paramQuery.isshowtut,
			restype_id: qwe.paramQuery.restype_id,
			min: qwe.paramQuery.min,
			max: qwe.paramQuery.max,
			orderBy: qwe.paramQuery.orderBy,
			name_oked_idk: qwe.paramQuery.name_oked_idk,
			isplan: qwe.paramQuery.isplan,
            isem_system:qwe.paramQuery.isem_system
		}, function (data) {

		    console.log("d=", data);

			kendo.ui.progress($('.object-source-controller-grid'), false);

			if (data.ErrorMessage) {
				showWarning(data.ErrorMessage);
				return
			}

			if (qwe.paramQuery.columnsVal == 'ExpenceEnergy')
				qwe.grid.hideColumn(8);
			else qwe.grid.showColumn(8);

			$.map(data.ListItems, function (item) {

				for (var i = 1; i <= 28; i++) {
					item["noS" + i] = decimalAdjust("round", item["noS" + i], '-3');
				}

				item.tut = decimalAdjust("round", item.tut, '-3');
				item.consumption = decimalAdjust("round", item.consumption, '-3');
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
			var html = data.Count + " из " + data.AllCount;

			if (data.Count == data.AllCount)
				html = data.Count;

			$(div).html(html);

		});
	}

	//----
	function getMultiSelectedValues() {
		
		//---- reason		
		var reason_selected = [];
		$('.multiple-reason-selected option:selected').each(function (item, indx) {
			reason_selected.push($(this).val());
		});
				
		qwe.paramQuery.reason_ids = reason_selected.join('*');;
		console.log("qwe.paramQuery.reason_id=", reason_selected);
		//$('#example-select-onChange-array').multiselect('select', ['1', '3'], true);

		//---- fscode
		var fscode_selected = [];
		$('.multiple-fscode-selected option:selected').each(function (item, indx) {
			fscode_selected.push($(this).val());
		});

		qwe.paramQuery.fscode = fscode_selected.join('*');;
		console.log("qwe.paramQuery.reason_id=", fscode_selected);

		//---- expectant
		var expectant_selected = [];
		$('.multiple-expectant-selected option:selected').each(function (item, indx) {
			expectant_selected.push($(this).val());
		});

		qwe.paramQuery.expectant_ids = expectant_selected.join('*');;
		console.log("qwe.paramQuery.expectant_ids=", expectant_selected);

	}

	//----
    function exportExcel() {
        console.log("val=" + qwe.paramQuery.columnsVal);
	    var url = rootUrl + "SourceController/ExportExcel?year=" + qwe.paramQuery.year + "&oblast_ids=" + qwe.paramQuery.oblast_ids + "&reason_ids=" + qwe.paramQuery.reason_ids + "&excluded_id=" + qwe.paramQuery.excluded_id;
        url = url + "&expectant_ids=" + qwe.paramQuery.expectant_ids + "&fscode=" + qwe.paramQuery.fscode + "&oked_ids=" + qwe.paramQuery.oked_ids + "&columnsVal=" + qwe.paramQuery.columnsVal+"&isshowtut=" + qwe.paramQuery.isshowtut + "&restype_id=" + qwe.paramQuery.restype_id + "&min=" + qwe.paramQuery.min + "&max=" + qwe.paramQuery.max + "&orderBy=" + qwe.paramQuery.orderBy + "&isplan=" + qwe.paramQuery.isplan + "&isem_system=" + qwe.paramQuery.isem_system + "&name_oked_idk=" + qwe.paramQuery.name_oked_idk;;
		window.location.href = url;
	}

	//----калонки
	function columnsEditor(isopen) {

		if (isopen == false)
			$('.columns-editor-menu').removeClass('hide');
		else
			$('.columns-editor-menu').addClass('hide');

		$('.columns-editor-menu').attr('isopen', (!isopen).toString());

	}

	//----
	function menu(evt, user_id) {
		// Блокируем всплывание события contextmenu
		evt = evt || window.event;
		evt.cancelBubble = true;
		// Показываем собственное контекстное меню
		var menu = document.getElementById("context-menu-list");
		menu.setAttribute("user-id", user_id);
		menu.style.top = defPosition(evt).y + "px";
		menu.style.left = defPosition(evt).x + "px";
		menu.style.display = "block";

		// Блокируем всплывание стандартного браузерного меню
		return false;
	}

	//--- Функция для определения координат указателя мыши
	function defPosition(event) {
		var x = y = 0;
		if (document.attachEvent != null) { // Internet Explorer & Opera
			x = window.event.clientX + (document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft);
			y = window.event.clientY + (document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop);
		} else if (!document.attachEvent && document.addEventListener) { // Gecko
			x = event.clientX + window.scrollX;
			y = event.clientY + window.scrollY;
		} else {
			// Do nothing
		}
		return { x: x, y: y };
	}

	//----
	function setLabelText() {
		var exp_text = $('.lbl-expectant').text();
		var sub_text = exp_text.substring(0, 10);
		$('.lbl-expectant').text(sub_text);
	}


	function getNotOwnSourceSum() {
	    //----
	    qwe.paramQuery.min = $('.restype-min').val();
	    qwe.paramQuery.max = $('.restype-max').val();
	    qwe.paramQuery.name_oked_idk = $('.search-name-idk-oked').val();

	    if (qwe.paramQuery.columnsVal == 'ExpenceEnergy')
	        qwe.paramQuery.isshowtut = false;

	    //----
	    getMultiSelectedValues();

	    kendo.ui.progress($('.sum-grid'), true);

	    $.post(rootUrl + "SourceController/getNotOwnSourceSum", {
	        year: qwe.paramQuery.year,
	        oblast_ids: qwe.paramQuery.oblast_ids,
	        reason_ids: qwe.paramQuery.reason_ids,
	        excluded_id: qwe.paramQuery.excluded_id,
	        expectant_ids: qwe.paramQuery.expectant_ids,
	        fscode: qwe.paramQuery.fscode,
	        oked_ids: qwe.paramQuery.oked_ids,
	        pageNum: qwe.paramQuery.pageNum,
	        isshowtut: qwe.paramQuery.isshowtut,
	        restype_id: qwe.paramQuery.restype_id,
	        min: qwe.paramQuery.min,
	        max: qwe.paramQuery.max,
	        orderBy: qwe.paramQuery.orderBy,
	        name_oked_idk: qwe.paramQuery.name_oked_idk,
	        isplan: qwe.paramQuery.isplan,
	        isem_system: qwe.paramQuery.isem_system
	    }, function (data) {
	        console.log("data=", data);
	        kendo.ui.progress($('.sum-grid'), false);
	        $.map(qwe.paramQuery.sumData, function (item) {
	            var num = 0;
	            if (data.item[item.field] != null && data.item[item.field] != 0) {
	                num = parseFloat(data.item[item.field]);
	                num = num.toFixed(2);
	            }
	            item.val = num;
	        });

	        console.log("after fill=", qwe.paramQuery.sumData);
	        qwe.sumGrid.dataSource.data(qwe.paramQuery.sumData);
	    });
	}

});