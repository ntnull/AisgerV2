
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
    $('.rating').each(function () {
        var tableName = $(this).attr('tablename');
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
    $('.commentDialog').live("click", function (e) {
        var modelId = parseInt($("#currentDataViewId").val(), 10) || 0;
        var nameTable = $(this).closest('tbody').attr('id');
        var colIndex = $(this).closest('td').index();
        var rowIndex = $(this).closest('tr').index();
        var inputControl;
        var controlEdit = $(this).parent().parent().find('.form-edit');
        var fieldValue;
        var fieldName;
        var idAttr = controlEdit.attr('name');
       
        if (controlEdit.hasClass("select2-container")) {
            fieldValue = controlEdit[0].innerText;
            inputControl = $(this).parent().parent();
            fieldName = "Event";

        } else {
            fieldValue = $(controlEdit).val();
            inputControl = $(this).parent().prev();
            fieldName = idAttr.split('.')[1];
        }
       

      
      
        var rowId = $(this).closest('tr').attr('rowid');
        var thisControl = $(this).find("i");
        if (modelId == 0) {
            return;
        }
        var url = "/SubActionPlan/ShowComment?modelId=" + modelId;
        url += "&nameTable=" + nameTable;
        url += "&colIndex=" + colIndex;
        url += "&rowIndex=" + rowIndex;
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
                buttons:  [ {
                    text: $("#saveTitleRes").val(),
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
                            url: '/SubActionPlan/SaveComment',
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
            })
            .load(url);
    });
    $(".close").live("click", function (e) {
        e.preventDefault();
        $(this).closest(".dialog").dialog("close");
    });
});