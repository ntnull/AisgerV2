$(function () {
    var qwe = {};
    window.formsReestr = qwe;
    var reportName = "Aisger.Reports.ReportReestr, Aisger";
    var currentLang = getCookie('lang');

    createControls();
    createGrids();
    getDicTypes();
    bindEventHandlers();

    function createControls() {
        $(".menu1").kendoMenu({
            //select: onSelect22
        });

        qwe.cmbxYears = $('.cmbx2').kendoComboBox({
        	dataTextField: "ReportYear",
        	dataValueField: "ReportYear"
        }).getKendoComboBox();

        /**/
        qwe.cmbxReportNames = $('.cmbx1').kendoComboBox({
            dataTextField: (currentLang === 'kk') ? "NAMEKZ" : "NAMERU",
            dataValueField: "ID",
            change: function (e) {
                var id = this.value();
                hideShowElements(id);
            }
        }).getKendoComboBox();
        /**/
        qwe.cmbxRegion = $('.cmbx3').kendoComboBox({
            dataTextField: "NameRu",
            dataValueField: "Id"
        }).getKendoComboBox();

        qwe.beginDp = $('.dp1').kendoDatePicker().data("kendoDatePicker");
        qwe.finishDp = $('.dp2').kendoDatePicker().data("kendoDatePicker");

        //----
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

    function getDicTypes() {
    	//var data = [{ ID: -1, NAMERU: '-' }, { ID: 2012, NAMERU: 2012 }, { ID: 2013, NAMERU: 2013 }, { ID: 2014, NAMERU: 2014 }, { ID: 2015, NAMERU: 2015 }, { ID: 2016, NAMERU: 2016 }, { ID: 2017, NAMERU: 2017 }];
    	//----
    	dic_getDicRstReport(function () {
    		var ds = dic.dicRstReport;
    		qwe.cmbxYears.dataSource.data(ds);
    		qwe.cmbxYears.select(0);
    	});

        /**/

    	var data2 = [{ ID: -1, NAMERU: '-', NAMEKZ: '-' },
                    { ID: 1, NAMERU: 'Отчет по реестру Субъектов ГЭР по республике', NAMEKZ: 'Республика бойынша МЭТ субъектілерінің тізілімі бойынша есеп' },
			        { ID: 2, NAMERU: 'Отчет по реестру Субъектов ГЭР в разрезе областей', NAMEKZ: 'Облыстар бөлінісінде МЭТ субъектілерінің тізілімі бойынша есеп' },
					{ ID: 3, NAMERU: 'Основные причины исключения из реестра по республике', NAMEKZ: 'Республика бойынша Тізілімнен шығарудың негізгі себептері' },
					{ ID: 4, NAMERU: 'Основные причины исключения из реестра в разрезе областей', NAMEKZ: 'Облыстар бөлінісінде Тізілімнен шығарудың негізгі себептері' },
					{ ID: 40, NAMERU: 'Отчет по Карта энергоэффективности', NAMEKZ: 'Энергия тиімділігі картасы бойынша есеп' }];

    	qwe.cmbxReportNames.dataSource.data(data2);

    	var lArr = location.href.split('/');
    	if (lArr == null || lArr.length == 0)
    	    window.location.href = "/Home/Index";

    	var reportId = parseInt(lArr[lArr.length - 1]);

    	var row = data2.filter(x=>x.ID == parseInt(reportId));
    	if (row == null || row.length == 0) {
    	    window.location.href = "/Home/Index";
    	}
    	qwe.cmbxReportNames.value(reportId);
    	hideShowElements(parseInt(reportId));

    	var name = "";
    	if (row != null && row.length > 0)
    	    name = (currentLang === 'kk') ? row[0].NAMEKZ : row[0].NAMERU;
    	$('#selectedReportText').text(name);


        /**/
        dic_getDicCato(function () {
            var ds = dic.dicCato;
            ds.shift();
            ds.unshift({ Id: 0, NameRu: '-' })
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
        var year = qwe.cmbxYears.value();
        var oblastId = qwe.cmbxRegion.value();
        var beginDate = getDateStrToCSharp(qwe.beginDp.value());
        var endDate = getDateStrToCSharp(qwe.finishDp.value());

        var reportData = {
            year: year,
            reportId: id,
            oblastId: oblastId,
            beginDate: beginDate,
            endDate: endDate
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

    function getCookie(cname) {
        var name = cname + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }
})