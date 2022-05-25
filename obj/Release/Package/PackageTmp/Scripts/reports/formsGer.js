$(function () {
    var qwe = {};
    window.formsGer = qwe;
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
                if (id == 53)
                    qwe.cmbxRegion.select(1);

                hideShowElements(parseInt(id));
            }
        }).getKendoComboBox();
        /**/
        qwe.cmbxRegion = $('.cmbx3').kendoComboBox({
            dataTextField: "NameRu",
            dataValueField: "Id",
            change: function () {
                
             

            }
        }).getKendoComboBox();

        //----
        qwe.cmbxLimit = $('.cmbx4').kendoComboBox({
            dataTextField: "limit",
            dataValueField: "limit"
        }).getKendoComboBox();

        //----
        qwe.cmbxFsCode = $('.cmbx5').kendoComboBox({
            dataTextField: (currentLang === 'kk') ? "NameKz" : "NameRu",
            dataValueField: "Id"
        }).getKendoComboBox();

        qwe.beginDp = $('.dp1').kendoDatePicker().data("kendoDatePicker");
        qwe.finishDp = $('.dp2').kendoDatePicker().data("kendoDatePicker");

        //----oked
        qwe.cmbxOKED = $('.cmbx6').kendoComboBox({
            dataTextField:"Name",
            dataValueField: "Id"
        }).getKendoComboBox();

        //----
        qwe.reportViewer = $(".divReportViewer").telerik_ReportViewer({
            serviceUrl: rootUrl + "api/reports/",
            scaleMode: 'FIT_PAGE_WIDTH',
            templateUrl: rootUrl + 'ReportViewer/templates/telerikReportViewerTemplate-9.0.15.225.html',
            reportSource: {
                report: reportName,
                parameters: {
                    year: '1200'
                }
            }
        });

        qwe.viewer = $(".divReportViewer").data("telerik_ReportViewer");

        // var viewer = wrapper.find(".divReportViewer").data("telerik_ReportViewer");
        
    }

    function getDicTypes() {

        //----limit
        var limitData = [{ limit: 10 }, { limit: 50 }];
        //----fsCode
        var fsCodeData = [{ Id: 0, NameRu: 'Все', NameKz: 'Барлығы' },
                          { Id: 1, NameRu: 'ЮР', NameKz: 'ЗТ' },
                          { Id: 2, NameRu: 'КВ', NameKz: 'КВ' },
                          { Id: 3, NameRu: 'ГУ', NameKz: 'ММ' },
        { Id: 4, NameRu: 'ИП', NameKz: 'ЖК' }];
        qwe.cmbxFsCode.dataSource.data(fsCodeData);
        qwe.cmbxFsCode.value(0);

        //----
        dic_getDicRstReport(function () {
            var ds = dic.dicRstReport;
            qwe.cmbxYears.dataSource.data(ds);
            qwe.cmbxYears.select(0);
            qwe.cmbxLimit.dataSource.data(limitData);
            qwe.cmbxLimit.select(0);
        });

        /**/

        var data2 = [
                { ID: -1, NAMERU: '-', NAMEKZ: '' },
                { ID: 8, NAMERU: 'Отчет по количеству, с разбивкой по формам собственности', NAMEKZ: 'Меншік нысандары бойынша бөле отырып, тұтыну көлемімен саны бойынша есеп' },
                { ID: 9, NAMERU: 'Отчет по областям, с объемами потребления', NAMEKZ: 'Облыстар бойынша бөле отырып, тұтыну көлемімен есеп' },
                { ID: 10, NAMERU: 'Отчет по видам экономической деятельности, с количеством, с объемами потребления', NAMEKZ: 'Саны, тұтыну көлемі бар Экономикалық қызмет түрлері бойынша есеп' },
                { ID: 11, NAMERU: 'Отчет по группам потребления', NAMEKZ: 'Тұтыну топтары бойынша есеп' }, //Основные причины исключения из реестра в разрезе областей
                { ID: 12, NAMERU: 'Отчет по категориям объемов  потребления', NAMEKZ: 'Тұтыну көлемдерінің санаттары бойынша есеп' },  //Отчет по наименованиям, с сортировкой по объемам потребления, выводить первые 100
                { ID: 13, NAMERU: 'Отчет по наименованиям, с сортировкой по объемам потребления, выводить первые 100', NAMEKZ: 'Тұтыну көлемі бойынша сұрыптаумен атаулар бойынша есеп, алғашқы 100 шығару' },
                { ID: 14, NAMERU: 'Отчет по видам энергоресурсов, с объемами потребления', NAMEKZ: 'Тұтыну көлемімен энергия ресурстарының түрлері бойынша есеп' },
                { ID: 15, NAMERU: 'Отчет о проверке предоставленных неполных сведений по республике', NAMEKZ: 'Республика бойынша берілген толық емес мәліметтерді тексеру туралы есеп' },
                { ID: 16, NAMERU: 'Отчет о проверке предоставленных неполных сведений по областям', NAMEKZ: 'Облыстар бойынша берілген толық емес мәліметтерді тексеру туралы есеп' },
                { ID: 17, NAMERU: 'Отчет по уклонившимся и/или предоставивших недостоверные сведения по республике', NAMEKZ: 'Республика бойынша жалтарған және/немесе дәйексіз мәліметтер берген есеп' },
                { ID: 25, NAMERU: 'Объемы потребления основных энергоресурсов, в т.у.т. и % от предыдущего периода', NAMEKZ: 'Шартты отын тоннасында негізгі энергия ресурстарын тұтыну көлемі және өткен кезеңнен % ' },
                { ID: 26, NAMERU: 'Объемы потребления основных энергоресурсов, в т.у.т. и % от предыдущего периода в разрезе областей', NAMEKZ: 'Негізгі энергия ресурстарын тұтыну көлемі, шартты отын тоннасында және облыстар бөлінісінде өткен кезеңнен % - бен' },
                { ID: 27, NAMERU: 'Доли потребления энергоресурсов субъектами ГЭР по областям', NAMEKZ: 'МЭТ субъектілерінің облыстар бойынша энергия ресурстарын тұтыну үлестері' },
                { ID: 28, NAMERU: 'Субъекты ГЭР с наибольшим потреблением электроэнергии, топ 100', NAMEKZ: 'Электр энергиясын ең көп тұтынатын МЭТ субъектілері, топ 100' },
                { ID: 29, NAMERU: 'Субъекты ГЭР с наибольшим потреблением теплоэнергии, топ 100', NAMEKZ: 'Жылу энергиясын ең көп тұтынатын МЭТ субъектілері, топ 100' },
                { ID: 30, NAMERU: 'Субъекты ГЭР с наибольшим потреблением природного газа, топ 100', NAMEKZ: 'Табиғи газды ең көп тұтынатын МЭТ субъектілері, топ 100' },
                { ID: 31, NAMERU: 'Субъекты ГЭР с наибольшим потреблением каменного угля, топ 100', NAMEKZ: 'Тас көмірді ең көп тұтынатын МЭТ субъектілері, топ 100' },
                { ID: 32, NAMERU: 'Сравнительные данные субъектов ГЭР, потребляющих более 100 тысяч т.у.т. в год', NAMEKZ: 'Жылына 100 мың тоннадан астам шартты отынды тұтынатын МЭТ субъектілерінің салыстырмалы деректері' },
                { ID: 33, NAMERU: 'Средние показатели удельного теплопотребления по областям', NAMEKZ: 'Облыстар бойынша үлестік жылу тұтынудың орташа көрсеткіштері' },
                { ID: 34, NAMERU: 'Информация об оснащенности приборами учета потребления энергоресурсов', NAMEKZ: 'Энергия ресурстарын тұтынуды есепке алу аспаптарымен жарақтандыру туралы ақпарат' },
                { ID: 37, NAMERU: 'Объемы потребления воды субъектами ГЭР (в млн. м3) в разрере областей', NAMEKZ: 'МЭТ субъектілерінің облыстардың тілігінде суды тұтыну көлемі (млн. м3) ' },
	            { ID: 42, NAMERU: 'Свод по областям, ФС и исключенным', NAMEKZ: 'Облыстар бойынша жиынтық, МС және шығарылған' },
                { ID: 43, NAMERU: 'Оснащенность приборами учета', NAMEKZ: 'Есептеу аспаптарымен жарақтандырылуы' },
                { ID: 44, NAMERU: 'Отчет по расходам на энергопотребление в разрезе областей', NAMEKZ: 'Облыстар бөлінісінде энергия тұтынуға арналған шығыстар бойынша есеп' },
                { ID: 45, NAMERU: 'Отчет по количественным показателям ГУ', NAMEKZ: 'ММ сандық көрсеткіштері бойынша есеп' },
                { ID: 46, NAMERU: 'Отчет по общему потреблению по РК (в т.у.т.)', NAMEKZ: 'ҚР бойынша жалпы тұтыну бойынша есеп (шартты отын тоннасында)' },
                { ID: 47, NAMERU: 'Выборка крупных субъектов по потреблению (т.у.т.)', NAMEKZ: 'Ірі субъектілерді тұтыну (шартты отын тоннасы) бойынша іріктеу)' },
                { ID: 48, NAMERU: '100 промышленных субъектов', NAMEKZ: '100 өнеркәсіптік субъект' },
                { ID: 49, NAMERU: 'Социальные объекты', NAMEKZ: 'Әлеуметтік нысандар' },
                { ID: 50, NAMERU: 'Список мало затратных мероприятий исполненных субъектами ГЭР', NAMEKZ: 'МЭТ субъектілері орындаған шығыны аз іс-шаралар тізімі' },
                { ID: 51, NAMERU: 'Список эффективных мероприятий исполненных субъектами ГЭР', NAMEKZ: 'МЭТ субъектілері орындаған тиімді іс-шаралар тізімі' },
                { ID: 52, NAMERU: 'Список субъектов ГЭР (список), не обеспечивших ежегодное снижение объема потребления энергоресурсов на единицу продукции', NAMEKZ: 'Өнім бірлігіне энергия ресурстарын тұтыну көлемінің жыл сайынғы төмендеуін қамтамасыз етпеген МЭТ субъектілерінің тізімі (тізім)' },
                { ID: 53, NAMERU: 'Сравнение данных по областям', NAMEKZ: 'Облыс бойынша деректерді салыстыру' },
                { ID: 54, NAMERU: 'Свод по удельному потреблению энергоресурсов субъектами ГЭР', NAMEKZ: 'МЭТ субъектілерінің энергия ресурстарын үлестік тұтынуы бойынша жиынтық' },
                { ID: 55, NAMERU: 'Свод  мероприятий исполненных субъектами ГЭР', NAMEKZ: 'Свод  мероприятий исполненных субъектами ГЭР' }
        ];

        qwe.cmbxReportNames.dataSource.data(data2);
       // var reportId = getUrlParameter("reportId");
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
    

        var name="";
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

        
        $.post(rootUrl + 'GraReports/GetOKED', { lang: currentLang }, function (data) {
            console.log('oked=', data);
            if (currentLang == 'kk')
                data.unshift({ Id: 0, Name: 'Барлығы' });
            else
                data.unshift({ Id: 0, Name: 'Все' });

            qwe.cmbxOKED.dataSource.data(data);
            qwe.cmbxOKED.value(0);
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

        //----
        $('.btnRun').click(updateReportViewer);

    }

    //----
    function updateReportViewer() {

        var id = qwe.cmbxReportNames.value();
        if (id == -1){
            alert("Выберите отчет");
            return;
        }

        var year = qwe.cmbxYears.value();
        var oblastId = qwe.cmbxRegion.value();
        var beginDate = getDateStrToCSharp(qwe.beginDp.value());
        var endDate = getDateStrToCSharp(qwe.finishDp.value());
        var limit = qwe.cmbxLimit.value();
        var oblastName = qwe.cmbxRegion.text();
        var fsCode = qwe.cmbxFsCode.value();
        var okedId = qwe.cmbxOKED.value();
        if (oblastId == 0)
            oblastName = (currentLang == 'kk') ? "Қазақстан Республикасы" : "Республика Казахстан";
        
        if (id == 53 && oblastId == 0) {
            qwe.cmbxRegion.select(1);
            oblastId = qwe.cmbxRegion.value();
            oblastName = qwe.cmbxRegion.text();
        }

        var reportData = {
            year: year,
            reportId: id,
            oblastId: oblastId,
            beginDate: beginDate,
            endDate: endDate,
            limit: limit,
            oblastName: oblastName,
            fscode: fsCode,
            okedId: okedId
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
      
        //qwe.viewer.commands.zoomOut();
    }

    function hideShowElements(id) {
        // жыл ашык
        $('.liCmbx2').show();
        if (id === 45) {
            $('.liDp1, .liDp2 , .liCmbx4, .liCmbx5, .liCmbx6').hide();
        } else {
            $('.liCmbx3 , .liCmbx4, .liCmbx5, .liCmbx6, .liDp1, .liDp2').hide();
        }

        if (id == 47) {
            $('.liCmbx4 , .liCmbx3').show();
        }

        if (id == 48 || id == 49 || id == 50 || id == 51 || id == 52 || id == 54) {
            $('.liCmbx3').show();
        }

        if (id == 54) {
            $('.liCmbx5').show();
        }

        if (id == 55) {
            $('.liCmbx3').show();
            $('.liCmbx5').show();
            $('.liCmbx6').show();
        }
    }

    function getUrlParameter(sParam) {
        var sPageURL = window.location.search.substring(1),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;

        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
            }
        }
    };

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