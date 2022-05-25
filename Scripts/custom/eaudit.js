var EAuditGeneral = {
    NumberDecimalSeparatorConst:',',
    NumberGroupSeparatorConst: ' ',
    init: function (tableId) {
//        $('.commentDialog').live("click", function (e) {
//            EAuditGeneral.comment(e);
        //        });

        $(".raiting").each(function () {
            var tbodyId = $(this).attr('formCode');
            var rowId = $(this).attr('rowId');
            var fieldName = $(this).attr('fieldName');
            var isError = $(this).attr('isError');
            var tbody = $('#' + tbodyId);
            if (!tbody)
                return;

            var controlEdit = tbody.find("#" + rowId).find("input[id*='_" + fieldName + "']");
            var thisControl = $(controlEdit).closest(".input-group").find('i');

            if (!thisControl.hasClass('mark-icon')) {
                thisControl.addClass('mark-icon');

            }
            if (isError =='True') {
                if (!controlEdit.hasClass('control-error')) {
                    controlEdit.addClass('control-error');
                }
                if (controlEdit.hasClass('control-good')) {
                    controlEdit.removeClass('control-good');
                }
            } else {
                if (!controlEdit.hasClass('control-good')) {
                    controlEdit.addClass('control-good');
                }
                if (controlEdit.hasClass('control-error')) {
                    controlEdit.removeClass('control-error');
                }
            }
        });
        
        $('#' + tableId).on("click", ".commentDialog", function(s, e) {
            EAuditGeneral.comment(s);
            return false;
        });
    },
    comment: function (e) {
        var s = e.currentTarget;
        var formCode = $(s).closest('tbody').attr('id');
        var rowid = $(s).closest('tr').attr('id');
        var controlEdit = $(s).closest(".input-group").find('.form-edit');
        var value = controlEdit.val();


        var fieldName = controlEdit.attr("name");
        if (fieldName.indexOf('.') > -1) {
            var nameArr = fieldName.split('.');
            fieldName = nameArr[nameArr.length - 1];
        }
        
        var url = "/EnergyAudit/GetFieldComments?formCode=" + formCode;
        url += "&rowId=" + rowid;
        url += "&fieldName=" + fieldName;
        e.preventDefault();


        var thisControl = $(s).find("i");
        
        var div = '<div style="text-align: center;">' +
            +'<img src="/Content/images/spinner.gif" style="display:block; margin: 0 auto;" />'
            +'<br/>...'
            +'</div>';
        $(div).addClass("dialog")
            .appendTo("body")
            .dialog({
                title:  EAuditGeneral.tTitle,
                closeText: "",
                close: function() { $(this).remove(); },
                width: 800,
                modal: true,
                position: 'top',
                buttons:[
                    {
                        text: EAuditGeneral.sSave,
                        click: function() {
                            var comment = $("#comment").val();
                            var isError;
                            if ($("#isError").is(":checked")) {
                                isError = true;
                            } else {
                                isError = false;
                            }
                            var fieldCommentObj = JSON.stringify({
                                'FieldName': fieldName,
                                'RowId': rowid,
                                'FormCode': formCode,
                                'Comment': comment,
                                'IsError': isError,
                                'FieldValue': value
                            });
                            $.ajax({
                                type: "POST",
                                url: '/EnergyAudit/SaveFieldComment',
                                data: fieldCommentObj,
                                dataType: 'json',
                                cache: false,
                                contentType: "application/json; charset=utf-8",
                                success: function(data) {

                                    if (!thisControl.hasClass('mark-icon')) {
                                        thisControl.addClass('mark-icon');

                                    }
                                    if (isError) {
                                        if (!controlEdit.hasClass('control-error')) {
                                            controlEdit.addClass('control-error');
                                        }
                                        if (controlEdit.hasClass('control-good')) {
                                            controlEdit.removeClass('control-good');
                                        }
                                    } else {
                                        if (!controlEdit.hasClass('control-good')) {
                                            controlEdit.addClass('control-good');
                                        }
                                        if (controlEdit.hasClass('control-error')) {
                                            controlEdit.removeClass('control-error');
                                        }
                                    }

                                },
                                error: function() {
                                    alert("Connection Failed. Please Try Again");
                                }
                            });
                            $(this).dialog("close");
                        }
                    }
                ]
            })
            .load(url);
    },
    fixDecimalSeparatorFn : function(value) {
        return value.replace('.', EAuditGeneral.NumberDecimalSeparatorConst);
    },
    _alerWrongInput: function () {
        $.notify({
            icon: "glyphicon glyphicon-remove-circle",
            message: EAuditGeneral.msgFieldMustBeDigit,
        }, {
            type: 'danger'
        });
    },
    msgFieldMustBeDigit: 'Поле должно содержат числовое значение',
    tTitle: 'Комментарии',
    sSave: 'Сохранить',
}




