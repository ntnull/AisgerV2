function formatNumber(num) {
    return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1 ");
}
function replaceAll1(find, replace, str) {
    while (str.indexOf(find) > -1) {
        str = str.replace(find, replace);
    }
    return str;
}

   
function showTutOrValue(isTut) {
    var ExtractVolume_All = 0;
    var NotOwnSource_All = 0;
    var LosEnergy_All = 0;
    var OwnSource_All = 0;
    var TransferOtherLegal_All = 0;
    var ExpenceEnergy_All = 0;
    $(".form2Field").each(function () {
        var valueField = $(this).val();
        var idField = $(this).attr('id');
        if (valueField.indexOf(',') > 0) {
            valueField = valueField.replace(',', '.');
        }
        if (valueField.length > 0) {
            valueField = replaceAll1(' ','',valueField);
            var row = $(this).closest('tr');
            var idkoef = "#tut_" + row.attr('typeid');
            var newValue = "";
            var isCalc = true;
            if (idField.indexOf('ExpenceEnergy') > -1) {
                ExpenceEnergy_All += valueField*1;
                return;
            }
            if (isTut) {
                $("#alltutlabel").show();
                $(this).attr('currentValue', valueField);
                newValue = valueField * $(idkoef).val().replace(',', '.');
                newValue = newValue.toFixed(2)*1;
                $("#from2SumRow").show();
                if (idField.indexOf('ExtractVolume') > -1) {
                    ExtractVolume_All += newValue;
                }
                if (idField.indexOf('NotOwnSource') > -1) {
                    NotOwnSource_All += newValue;
                }
                if (idField.indexOf('LosEnergy') > -1) {
                    LosEnergy_All += newValue;
                }
                if (idField.indexOf('OwnSource') > -1 && idField.indexOf('NotOwnSource')<0) {
                    OwnSource_All += newValue;
                }
                if (idField.indexOf('TransferOtherLegal') > -1) {
                    TransferOtherLegal_All += newValue;
                }
                if (idField.indexOf('ExpenceEnergy') > -1) {
                    ExpenceEnergy_All += newValue;
               }

            }
            if (!isTut) {
                $("#alltutlabel").hide();
                $("#from2SumRow").hide();
//                newValue = valueField / $(idkoef).val().replace(',', '.');
                //                newValue = newValue.toFixed(2);
                newValue = $(this).attr('currentValue');
            }
            if (newValue != null && newValue.length > 0) {
                newValue = formatNumber(newValue);
            }
            if (newValue != null && newValue.length > 0) {
                newValue = formatNumber(newValue);
            }
            $(this).val(newValue);
        }
    });
    $("#ExtractVolume_All").val(formatNumber(ExtractVolume_All.toFixed(2)));
    $("#NotOwnSource_All").val('NotOwnSource_All');
    //$("#NotOwnSource_All").val(formatNumber(NotOwnSource_All.toFixed(2)));
    $("#LosEnergy_All").val(formatNumber(LosEnergy_All.toFixed(2)));
    $("#OwnSource_All").val('OwnSource_All');
    //$("#OwnSource_All").val(formatNumber(OwnSource_All.toFixed(2)));
    $("#TransferOtherLegal_All").val('TransferOtherLegal_All');
    //$("#TransferOtherLegal_All").val(formatNumber(TransferOtherLegal_All.toFixed(2)));
    $("#ExpenceEnergy_All").val(formatNumber(ExpenceEnergy_All.toFixed(2)));
    var sumtut = NotOwnSource_All + OwnSource_All - TransferOtherLegal_All;  
    $("#alltutCalc").text('FFF');
    //$("#alltutCalc").text(formatNumber(sumtut.toFixed(2)));
    console.log('NotOwnSource_All', NotOwnSource_All);
    console.log('OwnSource_All', OwnSource_All);
    console.log('TransferOtherLegal_All', TransferOtherLegal_All);
}

function showInformIcon(isShow) {
    if (isShow) {
        $('.input-group-addon').show();
    } else {
        $('.input-group-addon').hide();
    }
}

$(document).ready(function () {

   /* $('.glyphicon-info-sign').each(function () {
        $(this).addClass("def-icon");
    });*/

	$('.form1-tr-class td').on('hover', function () {
		$('.commentDialog-form1-span').css('display', 'block');
	});

	//----
	$('.rating').each(function () {
		
		var tableName = $(this).attr('tablename');

		console.log("rating:", tableName);

        if (document.getElementById(tableName)==null) {
            return;
        }
        var columnindex = parseInt($(this).attr('columnindex'));
        var rowindex = parseInt($(this).attr('rowindex'));
        var iserror = $(this).attr('iserror');
//        console.log(tableName + '; col:' + columnindex + "; row:" + rowindex);
        if (document.getElementById(tableName)==null || document.getElementById(tableName).rows==null || document.getElementById(tableName).rows.length < rowindex + 1) {
            return;
        }
        var cell = document.getElementById(tableName).rows[rowindex].cells[columnindex];

        if (cell != null && cell.children[0] != null && cell.children[0].children[0] != null) {

            if (cell.children[0].children[1].name == "SEC_User1.IsCvazy") {

                if (iserror) {
                    cell.children[0].className += " control-error";
                } else {
                    cell.children[0].className += " control-good";
                }
            } else {


                cell.children[0].children[1].children[0].children[0].className = "glyphicon glyphicon-info-sign mark-icon";
                if (iserror) {
                    cell.children[0].children[0].className += " control-error";
                } else {
                    cell.children[0].children[0].className += " control-good";
                }
            }
        }

    });

	//----
    $('.commentDialog').live("click", function (e) {
        var modelId = parseInt($("#currentDataViewId").val(), 10) || 0;
        var nameTable = $(this).closest('tbody').attr('id');
        var colIndex = $(this).closest('td').index();
        var rowIndex = $(this).closest('tr').index();
        var inputControl = $(this).parent().prev();
        var controlEdit = $(this).parent().parent().find('.form-edit');
        var fieldValue = "";
        var idAttr = controlEdit.attr('name');
        if (controlEdit.is("select")) {
            fieldValue = controlEdit.find('option:selected').text();
        } else {
            if (inputControl.attr('id') == 'wastesDiv') {
                idAttr= 'SEC_User1.Waste';
                $('.search-choice').each(function () {
                    fieldValue += $(this).text() + ';';
                });
//                fieldValue = controlEdit.find('option:selected').text();
            } else {
                fieldValue = $(controlEdit).val();
            }
        }
        
        var fieldName = idAttr.split('.')[1];
        if (fieldName == "IsCvazy") {
            if (fieldValue == "true") {
                fieldValue = "Да";
            }
            if (fieldValue == "false") {
                fieldValue = "Нет";
            }
        }

        var rowId = $(this).closest('tr').attr('rowid');
        if (location.href.indexOf("Gu") != -1) {
            if (rowId == undefined && nameTable == "form2")
                rowId = $(this).closest('tr').attr('rowid2');;
            if (rowId == undefined)
                rowId = $(this).closest('tr').attr('rowid6');
        }

        var thisControl = $(this).find("i");
        if (modelId == 0) {
            return;
        }
        var url = "/RegisterForm/ShowComment?modelId=" + modelId;
        url += "&nameTable=" + nameTable;
        url += "&colIndex=" + colIndex;
        url += "&rowIndex=" + rowIndex;
        url += "&rowId=" + rowId;
        url += "&fieldName=" + fieldName;
        e.preventDefault();
        $("<div style=" + '"' + "text-align: center;" + '"' + "><img src=" + '"' + "../content/images/spinner.gif" + '"' + " style=" + '"' + "display: block; margin: 0 auto;" + '"' + " /></br>" + "....</div>")
            .addClass("dialog")
            .attr("id", $(this)
                .attr("data-dialog-id"))
            .appendTo("body")
            .dialog({
                title: $("#commentTitleRes").val(),
                closeText: "",
                close: function () { $(this).remove(); },
                width: 800,
                modal: true,
                //   open: function(event, ui) { $(".ui-dialog-titlebar-close").text(''); },
                buttons:[ {
                    text: 'Сохранить',//$("#saveTitleRes").val(),
                    click: function () {
                        var comment = $("#NoteComment").val();
                        var isError;
                        if ($("#IsError").is(":checked")) {
                            isError = true;
                        } else {
                            isError = false;
                        }
                        var params = JSON.stringify({ 'modelId': modelId, 'nameTable': nameTable, 'colIndex': colIndex, 'rowIndex': rowIndex, 'isError': isError, 'comment': comment, 'rowId':rowId, 'fieldName':fieldName, 'fieldValue': fieldValue });
                        $.ajax({
                            type: "POST",
                            url: '/RegisterForm/SaveComment',
                            data: params,
                            dataType: 'json',
                            cache: false,
                            contentType: "application/json; charset=utf-8",
                            success: function (data) {
                                if (!thisControl.hasClass('mark-icon')) {
                                    thisControl.addClass('mark-icon');

                                }
                                if (isError) {
                                    if (!inputControl.hasClass('control-error')) {
                                        inputControl.addClass('control-error');
                                    }
                                    if (inputControl.hasClass('control-good')) {
                                        inputControl.removeClass('control-good');
                                    }
                                } else {
                                    if (!inputControl.hasClass('control-good')) {
                                        inputControl.addClass('control-good');
                                    }
                                    if (inputControl.hasClass('control-error')) {
                                        inputControl.removeClass('control-error');
                                    }
                                }

                            },
                            error: function () {
                                alert("Connection Failed. Please Try Again");
                            }
                        });
                        $(this).dialog("close");
                    }
                }]
            }).load(url);
    });

	//----
    $('.commentDialog-form1-span').live("click", function (e) {
    	var modelId = parseInt($("#currentDataViewId").val(), 10) || 0;
    	var nameTable = $(this).closest('tbody').attr('id');
    	var colIndex = $(this).closest('td').index();
    	var rowIndex = $(this).closest('tr').index();
    	var inputControl = $(this).parent().parent();
    	console.log("inputControl:", inputControl);
    	var controlEdit = $(this).parent().parent().find('.form-edit');
    	var fieldValue = "";
    	var idAttr = controlEdit.attr('name');
    	if (controlEdit.is("select")) {
    		fieldValue = controlEdit.find('option:selected').text();
    	} else {
    		if (inputControl.attr('id') == 'wastesDiv') {
    			idAttr = 'SEC_User1.Waste';
    			$('.search-choice').each(function () {
    				fieldValue += $(this).text() + ';';
    			});
    			//                fieldValue = controlEdit.find('option:selected').text();
    		} else {
    			fieldValue = $(controlEdit).val();
    		}
    	}

    	var fieldName = idAttr.split('.')[1];
    	if (fieldName == "IsCvazy") {
    		if (fieldValue == "true") {
    			fieldValue = "Да";
    		}
    		if (fieldValue == "false") {
    			fieldValue = "Нет";
    		}
    	}
    	var rowId = $(this).closest('tr').attr('rowid');
    	var thisControl = $(this).find("i");
    	if (modelId == 0) {
    		return;
    	}
    	var url = "/RegisterForm/ShowComment?modelId=" + modelId;
    	url += "&nameTable=" + nameTable;
    	url += "&colIndex=" + colIndex;
    	url += "&rowIndex=" + rowIndex;
    	url += "&rowId=" + rowId;
    	url += "&fieldName=" + fieldName;

    	e.preventDefault();
    	$("<div style=" + '"' + "text-align: center;" + '"' + "><img src=" + '"' + "../content/images/spinner.gif" + '"' + " style=" + '"' + "display: block; margin: 0 auto;" + '"' + " /></br>" + "....</div>")
            .addClass("dialog").attr("id", $(this).attr("data-dialog-id"))
            .appendTo("body")
            .dialog({
            	title: $("#commentTitleRes").val(),
            	closeText: "",
            	close: function () { $(this).remove(); },
            	width: 800,
            	modal: true,
            	//   open: function(event, ui) { $(".ui-dialog-titlebar-close").text(''); },
            	buttons: [{
            		text: $("#saveTitleRes").val(),
            		click: function () {
            			var comment = $("#NoteComment").val();
            			var isError;
            			if ($("#IsError").is(":checked")) {
            				isError = true;
            			} else {
            				isError = false;
            			}
            			var params = JSON.stringify({ 'modelId': modelId, 'nameTable': nameTable, 'colIndex': colIndex, 'rowIndex': rowIndex, 'isError': isError, 'comment': comment, 'rowId': rowId, 'fieldName': fieldName, 'fieldValue': fieldValue });
            			$.ajax({
            				type: "POST",
            				url: '/RegisterForm/SaveComment',
            				data: params,
            				dataType: 'json',
            				cache: false,
            				contentType: "application/json; charset=utf-8",
            				success: function (data) {
            					if (!thisControl.hasClass('mark-icon')) {
            						thisControl.addClass('mark-icon');

            					}
            					if (isError) {
            						if (!inputControl.hasClass('control-error')) {
            							inputControl.addClass('control-error');
            						}
            						if (inputControl.hasClass('control-good')) {
            							inputControl.removeClass('control-good');
            						}
            					} else {
            						if (!inputControl.hasClass('control-good')) {
            							inputControl.addClass('control-good');
            						}
            						if (inputControl.hasClass('control-error')) {
            							inputControl.removeClass('control-error');
            						}
            					}

            				},
            				error: function () {
            					alert("Connection Failed. Please Try Again");
            				}
            			});
            			$(this).dialog("close");
            		}
            	}]
            }).load(url);
    });

	//----
    $(".close").live("click", function (e) {
        e.preventDefault();
        $(this).closest(".dialog").dialog("close");
    });
});