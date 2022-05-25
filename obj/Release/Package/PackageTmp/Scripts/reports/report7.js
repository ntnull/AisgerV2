$(function () {

    console.log('report7.js');
    var qwe = {};
    window.qwe = qwe;

    createControllers();
    initVars();
    createGrid();
    fillYearGrid();
    getDicKato();
    GetReportMoreThan100();
    bindEventHandlers();
    loadHtmlTemplates();

    //----
    function initVars() {
        qwe.paramQuery = {};
        qwe.paramQuery.isFirst = false;
        qwe.paramQuery.oblast_id = -1;
        qwe.paramQuery.year = 2015;


    }

    //----
    function createControllers() {

        //----
        qwe.tabStrip = $('.tab-report7').kendoTabStrip({
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

        //---- меню
        $(".menu-report").kendoMenu({
            //select: onSelect22
        });

    }

    //----
    function bindEventHandlers() {

        //---- меню экспорт отчета
        $('.item-export').click(function () {

            var type = $(this).attr('type');
            console.log(type);
            if (type === 'pdf') {

                qwe.reportGrid.pdfExport();

            } else {
                qwe.reportGrid.saveAsExcel();
            }

        });

        //---- обновить
        $('.menu-item-refresh').click(function () {
            GetReportMoreThan100();
        });

        //----
        $('.tab-report7 .report-grid').on('click', 'td a.a-list', function () {

            qwe.tabStrip.wrapper.find('.tab-item2').removeClass('hide');
            var tabToActivate = qwe.tabStrip.wrapper.find("li.tab-item2");
            qwe.tabStrip.activateTab(tabToActivate);

            var objId = $(this).attr('obj-id');  

            var p = { year: qwe.paramQuery.year, secUserId: objId };
            qwe.FormaReport1.openWindow(p);

        });

    	//---- вернуться
        $('.btn-back').click(function () {
        	window.location.href = "/ReportAnalyse/Index";
        });
    }

    //----
    function createGrid() {

        //----
        qwe.yearGrid = $('.tab-report7 .year-grid').kendoGrid({
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
                    GetReportMoreThan100();
                }

            }
        }).getKendoGrid();

        //----
        qwe.regionGrid = $('.tab-report7 .region-grid').kendoGrid({
        	scrollable: true,
            sortable: true,
            selectable: 'row',
            columns: [
			{
			    field: "kato_name",
			    title:L.oblast,
			    filterable: false,
			    sortable: false
			}],
            resizable: true,
            change: function (e) {

                if (qwe.paramQuery.isFirst) {

                    var dataItem = qwe.regionGrid.dataItem(this.select());
                    qwe.paramQuery.oblast_id = dataItem.kato_id;
                    qwe.paramQuery.currRegion = dataItem;

                    GetReportMoreThan100();
                }

            }
        }).getKendoGrid();

        //---- report grid         
        qwe.reportGrid = $('.tab-report7 .report-grid').kendoGrid({
            scrollable: true,
            sortable: true,
            selectable: 'row',
            //toolbar: ["pdf"],
            excel: {
                allPages: true,
                fileName: "report.xlsx",
                filterable: true
            },
            pdf: {
                allPages: true,
                avoidLinks: true,
                paperSize: "A4",
                margin: { top: "2cm", left: "1cm", right: "1cm", bottom: "1cm" },
                landscape: true,
                repeatHeaders: true,
                //template: $("#page-template").html(),
                scale: 0.8
            },
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
			        title: L.gridCol15,
			        template: '<a class="a-list" obj-id="#=data.subject_id#"> #=data.subject_name #</a>',
			        filterable: false,
			        sortable: false
			    }, {
			        field: "consumption",
			        title: L.gridCol16,
                    //template:'',
			        filterable: false,
			        sortable: false
			    }, {
			        field: "cons_prev",
			        title: L.gridCol17,
			        filterable: false,
			        template: '',
			        sortable: false
			    }, {
			        field: "dynamic",
			        title: L.gridCol18,
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
        resizeGrids();
    }

    //----
    function fillYearGrid() {

        dic_getDicRstReport(function () {

            qwe.dicRstReport = dic.dicRstReport;

            qwe.yearGrid.dataSource.data(qwe.dicRstReport);

            var rows = qwe.yearGrid.dataSource.data();
            var row = qwe.yearGrid.table.find("[data-uid=" + rows[3].uid + "]");
            qwe.yearGrid.select(row);
            GetReportMoreThan100();

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

        });

    }

    //----
    function GetReportMoreThan100() {

        kendo.ui.progress($('.report1'), true);
        $.post(rootUrl + 'ReportAnalyse/GetReportMoreThan100', { oblast_id:qwe.paramQuery.oblast_id, year: qwe.paramQuery.year }, function (data) {

            kendo.ui.progress($('.report1'), false);

            if (data.ErrorMessage) {
                alert(data.ErrorMessage);
                qwe.reportGrid.dataSource.data([]);
                return;
            }
           
            qwe.paramQuery.isFirst = true;
            $.map(data.ListItems, function (item) {
                item.consumption = Math.round(item.consumption);
                item.cons_prev = Math.round(item.cons_prev);
                item.dynamic = Math.round(item.dynamic);
            });
            qwe.reportGrid.dataSource.data(data.ListItems);

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
})