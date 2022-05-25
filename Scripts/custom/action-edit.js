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
       
        var row = $(this).parent().closest('tr');
        var entityId = row.attr('rowid');
        UpdateModel(typeForm, entityId, $(this).attr('id'), fieldName, $(this).val(), "long", 0);
    });
}

