$(document).ready(function() {
    $("#Note").change(function () {
        UpdateModel("main", 0, $(this).attr('id'), "Note", $(this).val(), "string", 0);
    });
});

function SetComboBoxEvent(idcontrol) {
    $(idcontrol).change(function () {
        var idControl = $(this).attr('id');
        var typeForm = "";
        var fieldName = "";
        if (idControl.indexOf('TypeCounterId') > -1) {
            fieldName = "TypeCounterId";
        } else if (idControl.indexOf('TypeResourceId') > -1) {
            fieldName = "TypeResourceId";
        } else if (idControl.indexOf('EventId') > -1) {
            fieldName = "EventId";
        }

        if (idControl.indexOf('SubForm4Records') > -1) {
            typeForm = "form4";
        }

        if (idControl.indexOf('SubForm4RecordGuList') > -1) {
            typeForm = "form4";
        }

        if (idControl.indexOf('SubForm6Records') > -1) {
            typeForm = "form6";
        }
        var row = $(this).parent().closest('tr');
        var entityId = row.attr('rowid');
        UpdateModel(typeForm, entityId, $(this).attr('id'), fieldName, $(this).val(), "long", 0);
    });
}

$("select[id*='TypeCounterId']").change(function () {
    var idControl = $(this).attr('id');
    var typeForm = "";
    var row = $(this).parent().closest('tr');
    var entityId = row.attr('rowid');

    if (idControl.indexOf('SubForm4Records') > -1)
    {
        typeForm = "form4";
        var noteDiv = "#"+idControl.replace('TypeCounterId', 'NoteDiv');
        if ($(this).val() == 5) {
            $(noteDiv).show();
        } else {
            $(noteDiv).hide();
        }
    }

    if (idControl.indexOf('SubForm4RecordGuList') > -1) {
        typeForm = "form4";
        var noteDiv = "#" + idControl.replace('TypeCounterId', 'NoteDiv');
        if ($(this).val() == 5) {
            $(noteDiv).show();
        } else {
            $(noteDiv).hide();
        }
    }

    if (idControl.indexOf('SubForm6Records') > -1) {
        typeForm = "form6";
    }

    UpdateModel(typeForm, entityId, $(this).attr('id'), "TypeCounterId", $(this).val(), "long", 0);
});

$("select[id*='EventId']").change(function () {
    var idControl = $(this).attr('id');
    var typeForm = "";
    var row = $(this).parent().closest('tr');
    var entityId = row.attr('rowid');
        typeForm = "form4";
   

    UpdateModel(typeForm, entityId, $(this).attr('id'), "EventId", $(this).val(), "long", 0);
});
