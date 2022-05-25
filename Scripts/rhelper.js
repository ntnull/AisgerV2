
initVars();
$(function () {

    kendo.culture("ru-RU");

    window.onWindowResize = function () {
    	resizeGrids();
    	resizeGridsAll();
    };

    var resizeTimer;
    $(window).resize(function () {

    	console.log("resize");

        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
        	resizeGridsAll();
        }, 100);
        
    });


    //----
   
});

//---- for report
function resizeGrids() {

    var h = $('#main-container').height() - 8;

    //---- report 1
    $('.tab-report1 .region-grid').css('height', (h - 200) + 'px');
    $('.tab-report1 .region-grid .k-grid').css('height', (h - 200) + 'px');
    $('.tab-report1 .region-grid .k-grid .k-grid-content').css('height', (h - 200) + 'px');

    //---- report 2
    $('.tab-report2 .region-grid').css('height', (h - 300) + 'px');
    $('.tab-report2 .region-grid .k-grid').css('height', (h - 300) + 'px');
    $('.tab-report2 .region-grid .k-grid .k-grid-content').css('height', (h - 300) + 'px');

	//---- report 3
    $('.tab-report3 .region-grid').css('height', (h - 200) + 'px');
    $('.tab-report3 .region-grid .k-grid').css('height', (h - 200) + 'px');
    $('.tab-report3 .region-grid .k-grid .k-grid-content').css('height', (h - 200) + 'px');

	//---- report 4
    $('.tab-report4 .region-grid').css('height', (h - 300) + 'px');
    $('.tab-report4 .region-grid .k-grid').css('height', (h - 300) + 'px');
    $('.tab-report4 .region-grid .k-grid .k-grid-content').css('height', (h - 300) + 'px');
	
    //---- report 5
    $('.tab-report5 .year-grid').css('height', (h - 300) + 'px');
    $('.tab-report5 .year-grid .k-grid').css('height', (h - 300) + 'px');
    $('.tab-report5 .year-grid .k-grid .k-grid-content').css('height', (h - 300) + 'px');

    //---- report 6
    $('.tab-report6 .region-grid').css('height', (h - 300) + 'px');
    $('.tab-report6 .region-grid .k-grid').css('height', (h - 300) + 'px');
    $('.tab-report6 .region-grid .k-grid .k-grid-content').css('height', (h - 300) + 'px');

	//---- report 7
    $('.tab-report7 .region-grid').css('height', (h - 300) + 'px');
    $('.tab-report7 .region-grid .k-grid').css('height', (h - 300) + 'px');
    $('.tab-report7 .region-grid .k-grid .k-grid-content').css('height', (h - 300) + 'px');

    //---- report 8
    $('.tab-report8 .year-grid').css('height', (h - 300) + 'px');
    $('.tab-report8 .year-grid .k-grid').css('height', (h - 300) + 'px');
    $('.tab-report8 .year-grid .k-grid .k-grid-content').css('height', (h - 300) + 'px');

    //---- report 9
    $('.tab-report9 .year-grid').css('height', (h - 300) + 'px');
    $('.tab-report9 .year-grid .k-grid').css('height', (h - 300) + 'px');
    $('.tab-report9 .year-grid .k-grid .k-grid-content').css('height', (h - 300) + 'px');

	//---- report 2
    $('.tab-report10 .region-grid').css('height', (h - 300) + 'px');
    $('.tab-report10 .region-grid .k-grid').css('height', (h - 300) + 'px');
    $('.tab-report10 .region-grid .k-grid .k-grid-content').css('height', (h - 300) + 'px');



}

//---- all forms
function resizeGridsAll() {

	var h = $('#B').height();
	console.log("height", h);

	//----object-source-controller
	$('.object-source-controller-grid').css('height', (h - 120) + 'px');
	$('.object-source-controller-grid .k-grid').css('height', (h - 120) + 'px');
	$('.object-source-controller-grid .k-grid .k-grid-content').css('height', (h - 120) + 'px');
	 
	//---- url=/AuditReestr/RequiredAudits   object-grid
	$('.object-grid').css('height', (h - 110) + 'px');
	$('.object-grid .k-grid').css('height', (h - 110) + 'px');
	$('.object-grid .k-grid .k-grid-content').css('height', (h - 110) + 'px');
	
}

function setCookie(name, value) {
    var d = new Date();
    d.setTime(d.getTime() + (60 * 60 * 1000)); //(exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = name + "=" + value + "; " + expires;
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) != -1) return c.substring(name.length, c.length);
    }
    return "";
}

function createWindows() {

    bi.modalWindowAlert = $(".divAlertWindow").kendoWindow({
        width: "400px",
        height: "182px",
        title: "Подтвердить",
        visible: false,
        modal: true,
        draggable: true,
        activate: function () {
            bi.modalWindowAlert.toFront();
        }
    }).getKendoWindow().center();

};

function openAlertWindow(message, options) {
    createWindows();
    var wnw = bi.modalWindowAlert;

    if (!options)
        options = { ok: function () { } };

    $('.sp-question').html(message);
    if (options.ok) {
        $(".divAlertWindow .btn-yes").hide();
        $(".divAlertWindow .btn-cancel").hide();
        $(".divAlertWindow .btn-ok").show();
    } else {
        $(".divAlertWindow .btn-yes").show();
        $(".divAlertWindow .btn-cancel").show();
        $(".divAlertWindow .btn-ok").hide();
    }
    if (message.length > 50)
        wnw.wrapper.css('width', '400px').css('height', '182px');
    else
        wnw.wrapper.css('width', '350px').css('height', '106px');

    if (options.size) {
        wnw.wrapper
            .css('width', options.size.width + 'px')
            .css('height', options.size.height + 'px');
    }

    wnw.open();

    var btnYes = $(".divAlertWindow .btn-yes");
    var yesFunc = function () {
        if (options.yes) {
            options.yes();
            wnw.close();
        }
        btnYes.off('click', yesFunc);
    };
    btnYes.on('click', yesFunc);

    var btnCancel = $(".divAlertWindow .btn-cancel");
    var cancelFunc = function () {

        if (options.cancel) {
            options.cancel();
            btnCancel.off('click', cancelFunc);
            btnYes.off('click', yesFunc);
            wnw.close();

        } else {
            wnw.close();
            btnCancel.off('click', cancelFunc);
            btnYes.off('click', yesFunc);
        }

    };
    btnCancel.on('click', cancelFunc);

    var btnOk = $(".divAlertWindow .btn-ok");
    var okFunc = function () {
        setCookie("entred", "0");
        wnw.close();
        btnOk.off('click', okFunc);
    };
    btnOk.on('click', okFunc);
}

function templ_universal(name, callback) {
    templs.loaded = templs.loaded || {};
    if (templs[name]) {
        if (callback) callback();
    } else if (zxc[name]) {
        if (callback) zxc[name].add(callback);
    } else {
        zxc[name] = $.Callbacks();
        templs.loaded[name] = $.Deferred();
        $('<div>').load(rootUrl + 'Template/' + name + '.html',
          function (response) {
              if (!templs[name]) {
                  templs[name] = response;
                  zxc[name].fire().empty();
              }
              templs.loaded[name].resolve();
              if (callback) callback();
          });
    }
}

function decimalAdjust(type, value, exp) {
    // Если степень не определена, либо равна нулю...
    if (typeof exp === 'undefined' || +exp === 0) {
        return Math[type](value);
    }
    value = +value;
    exp = +exp;
    // Если значение не является числом, либо степень не является целым числом...
    if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0)) {
        return NaN;
    }
    // Сдвиг разрядов
    value = value.toString().split('e');
    value = Math[type](+(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp)));
    // Обратный сдвиг
    value = value.toString().split('e');
    return +(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp));
}

function dic_fsCode(fscode) {
    var dic = [{ Id: 1, NameRu: 'Юридические лица', code: 'юр' }, { Id: 2, NameRu: 'Квазигосударственный сектор', code: 'кв' }, { Id: 3, NameRu: 'Государственные учреждения', code: 'гу' }];
    var result = $.grep(dic, function (row) { return row.Id == fscode; })[0].NameRu;
    return result;
}

function dic_Year() {
    var dic = [2012, 2013, 2014, 2015];
    return dic;
}

function getDateStrToCSharp(date) {
    try {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var hour = date.getHours();
        var minute = date.getMinutes();
        var second = date.getSeconds();

        if (day < 10) day = '0' + day;
        if (month < 10) month = '0' + month;
        if (hour < 10) hour = '0' + hour;
        if (minute < 10) minute = '0' + minute;
        if (second < 10) second = '0' + second;
        var time = day + "." + month + "." + year + " " + hour + ':' + minute + ':' + second;
        return time;
    } catch (e) {
        return null;
    }
}


function getDateFromEFDateFormat(date) {
    try {
        var intDate = parseInt(date.split("(")[1].split(")")[0]);
        return new Date(intDate);
    } catch (e) {
        return null;
    }
}

function getDateStr(date) {
    try {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();

        if (day < 10) day = '0' + day;
        if (month < 10) month = '0' + month;
        var time = day + "." + month + "." + year;
        return time;
    } catch (e) {
        return null;
    }
}

function getDateStrFromEFDateFormat(date, format) {
    try {
        date = getDateFromEFDateFormat(date);
        var dateStr = getDateStrFromDate(date, format);
        return dateStr;
    } catch (e) {
        return '';
    }
}

function getDateStrFromDate(date, format) {
    if (!date)
        return '';
    var dateStr = null;
    if (!format) format = 'DD.MM.YYYY hh:mm:ss';
    switch (format) {
        case 'YYYY.MM.DD':
            dateStr = padStr(date.getFullYear()) + "." +
			    padStr(1 + date.getMonth()) + "." +
			    padStr(date.getDate());
            break;
        case 'YYYY.MM.DD hh:mm:ss':
            dateStr = padStr(date.getFullYear()) + "." +
                          padStr(1 + date.getMonth()) + "." +
                          padStr(date.getDate()) + " " +
                          padStr(date.getHours()) + ":" +
                          padStr(date.getMinutes()) + ":" +
                          padStr(date.getSeconds());
            break;
        case 'DD.MM.YYYY hh:mm:ss':
            dateStr = padStr(date.getDate()) + "." +
                          padStr(1 + date.getMonth()) + "." +
                          padStr(date.getFullYear()) + " " +
                          padStr(date.getHours()) + ":" +
                          padStr(date.getMinutes()) + ":" +
                          padStr(date.getSeconds());
            break;
        case 'DD.MM.YYYY hh:mm':
            dateStr = padStr(date.getDate()) + "." +
                          padStr(1 + date.getMonth()) + "." +
                          padStr(date.getFullYear()) + " " +
                          padStr(date.getHours()) + ":" +
                          padStr(date.getMinutes());
            break;
        case 'DD.MM.YY':
            dateStr = padStr(date.getDate()) + "." +
                          padStr(1 + date.getMonth()) + "." +
                          (padStr(date.getFullYear()) - 2000);
            break;
        case 'MM.DD.YY':
            dateStr = padStr(padStr(1 + date.getMonth()) + "." +
                          date.getDate()) + "." +
                          (padStr(date.getFullYear()) - 2000);
            break;
        case 'DD.MM.YYYY':
            dateStr = padStr(date.getDate()) + "." +
                          padStr(1 + date.getMonth()) + "." +
                          padStr(date.getFullYear());
            break;
        case 'yyyy-MM-dd':
            dateStr = padStr(date.getFullYear()) + "-" +
                          padStr(1 + date.getMonth()) + "-" +
                          padStr(date.getDate());
            break;
        case 'CURRYEAR/MM/DD':
            dateStr = (new Date().getFullYear()) + "/" +
                          (date.getMonth()) + "/" +
                          (date.getDate());
            break;
        case 'DD MMMM':
            dateStr = date.getDate() + ' ' + getMonthName(1 + date.getMonth());
            break;
        case 'hh:mm':
            dateStr = padStr(date.getHours()) + ":" + padStr(date.getMinutes());
            break;
        default: break;
    }
    return dateStr;
}

function padStr(i) {
    return (i < 10) ? "0" + i : "" + i;
}

function initVars() {

	

	zxc.kendo = {
		grid: {
			filterable: {
				extra: false,
				messages: {
					noRows: "Нет данных",
					info: "Заголовок:",
					filter: "Фильтр",
					clear: "Очистить",

					isTrue: "custom is true",
					isFalse: "custom is false",

					and: "И",
					or: "Или"
				},
				operators: {
					string: {
						contains: "Содержит",
						eq: "Равно",
						neq: "Не равно",
						startswith: "Начинается с",
						endswith: "Заканчивается с"
					},
					number: {
						eq: "Равно",
						neq: "Не равно",
						gte: "Больше или равно",
						gt: "Больше",
						lte: "Меньше или равно",
						lt: "Меньше"
					},
					date: {
						eq: "Равно",
						neq: "Не равно",
						gte: "Больше или равно",
						gt: "Больше",
						lte: "Меньше или равно",
						lt: "Меньше"
					},
					enums: {
						eq: "custom Is Equal to",
						neq: "custom Is Not equal to"
					}
				}
			},
			groupable: {
				messages: {
					empty: "Перетащите сюда столбец для группирования"
				}
			},
			pageable: {
				input: true,
				numeric: true,
				messages: {
					display: "{0} - {1} из {2} элементов",
					NoDetailRecordsText: "2222",
					noRows: "Нет данных",
					empty: "Нет данных",
					page: "Страница",
					of: "из {0}",
					itemsPerPage: "items per page",
					first: "Перейти на первую страницу",
					previous: "Перейти на предыдущую страницу",
					next: "Перейти на следующую страницу",
					last: "Перейти на последнюю страницу",
					refresh: "Обновить"
				}
			},
		},
		upload: {
			localization: {
				cancel: "Отмена",
				dropFilesHere: "чтобы загрузить, перетащите файлы сюда",
				headerStatusUploaded: "Готово",
				headerStatusUploading: "Загружается...",
				remove: "Удалить",
				retry: "Повторить",
				select: "Выбрать файлы...",
				statusFailed: "не удалось",
				statusUploaded: "загружено",
				statusUploading: "загружается",
				statusWarning: "предупреждение",
				uploadSelectedFiles: "Загрузка файлов"
			}
		}
	};


}