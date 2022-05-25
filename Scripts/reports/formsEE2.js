$(function () {

    var qwe = {};
    window.formsReestr = qwe;
    var reportName = "Aisger.Reports.ReportReestr, Aisger";

    createControls();
    createGrids();
    getDatas();
    bindEventHandlers();

    function createControls() {

        //----меню
        $(".menu1").kendoMenu({
            //select: onSelect22
        });

        //----наименование отчета
        qwe.cmbxReportNames = $('.cmbx1').kendoComboBox({
            dataTextField: "NameRu",
            dataValueField: "Id",
            change: function (e) {

                var id = this.value();
            }
        }).getKendoComboBox();

        //----область
        qwe.cmbxRegion = $('.cmbx3').kendoComboBox({
            dataTextField: "kato_name",
            dataValueField: "kato_id"
        }).getKendoComboBox();

        //----отчет
        qwe.reportViewer = $(".divReportViewer").telerik_ReportViewer({
            serviceUrl: rootUrl + "api/reports/",
            templateUrl: rootUrl + 'ReportViewer/templates/telerikReportViewerTemplate-9.0.15.225.html',
            scaleMode: 'FIT_PAGE_WIDTH',
            reportSource: {
                report: reportName,
                parameters: {
                    year: '1200'
                }
            }
        });
        qwe.viewer = $(".divReportViewer").data("telerik_ReportViewer");
    }

    function getDatas() {

        //----наименование отчета 
        var data2 = [{ Id: -1, NameRu: '-' }, { Id: 41, NameRu: '1. Отчет по объектам ЕЕ 2.0' }];
        qwe.cmbxReportNames.dataSource.data(data2);
        qwe.cmbxReportNames.select(0);

        //----области
        dic_getDicKato(function () {
            var ds = dic.dicKato;
            console.log("dic:", ds);
            //ds.shift();
            //ds.unshift({ kato_id: -1, kato_name: '' })
            qwe.cmbxRegion.dataSource.data(ds);
            qwe.cmbxRegion.select(0);
        });
    }

    function createGrids() {

        qwe.grid = $('.divGrid').kendoGrid({
            scrollable: true,
            selectable: 'row',
            columns: [],
            //filterable: asd.kendo.grid.filterable,
            resizable: true,
            change: function () {
                qwe.currItem = qwe.grid.dataItem(this.select());
            }
        }).getKendoGrid();
    }

    function bindEventHandlers() {
        $('.btnRun').click(updateReportViewer);
    }

    //----
    function updateReportViewer() {

        var id = qwe.cmbxReportNames.value();
        var oblastId = qwe.cmbxRegion.value();

        var reportData = {
            year: 2015,
            reportId: id,
            oblastId: oblastId,
            beginDate: 12,
            endDate: 12
        };
        qwe.parameterJson = JSON.stringify(reportData);

        qwe.viewer.reportSource({
            report: reportName,
            parameters: {
                json: qwe.parameterJson
            }
        });
        qwe.viewer.refreshReport();

    	//----
        setTimeout(function () {
        	qwe.viewer.commands.zoomOut.exec();
        }, 1000);
    }

    function hideShowElements(id) {
        if (id == 1 || id == 3 || id == 38) {
            $('.liCmbx2').show();
            $('.liCmbx3, .liDp1, .liDp2').hide();
        }
        else if (id == 2 || id == 4) {
            $('.liCmbx2, .liCmbx3').show();
            $('.liDp1, .liDp2').hide();
        } else if (id == 5 || id == 6 || id == 7 || id == 39 || id == 40) {
            $('.liDp1, .liDp2').show();
            $('.liCmbx2, .liCmbx3').hide();
        }

    }
})