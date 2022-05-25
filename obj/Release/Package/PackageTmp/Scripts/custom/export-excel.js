function replaceAll(find, replace, str) {
    while (str.indexOf(find) > -1) {
        str = str.replace(find, replace);
    }
    return str;
}
function removeTags(find, str) {
    while (str.indexOf(find) > -1) {
        var index = str.indexOf(find);
        var end = str.indexOf('>', index);
        var output = str.substring(index, end + 1);
        str = str.replace(output, "");
    }
    return str;
}
function exportExcelBtn (divtable) {

    var fedHtml = replaceAll("<tr style=\"text-align: center;\">", "<tr style=\"text-align: center; font-weight: bold;\">", document.getElementById(divtable).innerHTML);
    fedHtml = replaceAll("<td>", "<td style=\"border:1px solid black;\">", fedHtml);
    fedHtml = replaceAll("<td rowspan", "<td style=\"border:1px solid black;\" rowspan", fedHtml);
    fedHtml = replaceAll("<td colspan", "<td style=\"border:1px solid black;\" colspan", fedHtml);
    fedHtml = replaceAll("<td style=\"width: 40px\">", "<td style=\"border:1px solid black; width: 40px\">", fedHtml);
    fedHtml = replaceAll("<td style=\"padding-left", "<td style=\"border:1px solid black; padding-left ", fedHtml);
    fedHtml = replaceAll("<tr style=\"overflow: hidden;\">", "<tr>", fedHtml);
    fedHtml = replaceAll("<tr style=\"display: none;\">", "<tr>", fedHtml);
    fedHtml = replaceAll("<span>[-]</span>", "", fedHtml);
    fedHtml = replaceAll("<span>[+]</span>", "", fedHtml);

    fedHtml = removeTags("<img", fedHtml);
    fedHtml = removeTags("<a", fedHtml);

    var blob = new Blob([fedHtml], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=windows-1251"
    });

    saveAs(blob, "Report.xls");
};