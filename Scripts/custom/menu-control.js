
$(document).ready(function () {
    if (document.getElementById("selectSubMenuTitle") == null) {
        return;
    }
    var selectMenuTitle = document.getElementById("selectSubMenuTitle").value;
    if (selectMenuTitle != "") {
        var obj = document.getElementById(selectMenuTitle);
        if (obj != null) {
            obj.className = "current_submenu";
            $(".panel-collapse").removeClass("in");
            var idcollapse = '#' + obj.parentElement.parentElement.parentElement.id;
            $(idcollapse).addClass("in");
        } else {
            var selectaActionTitle = document.getElementById("selectaActionTitle").value;
            if (selectaActionTitle == "AccountSettingEdit") {
                selectaActionTitle = "AccountSetting";
            }
            if (selectaActionTitle != "") {
                var obj = document.getElementById(selectaActionTitle);
                if (obj != null) {
                    obj.className = "current_submenu";
                    $(".panel-collapse").removeClass("in");
                    var idcollapse1 = '#' + obj.parentElement.parentElement.parentElement.id;
                    $(idcollapse1).addClass("in");
                }
            }
        }
    } 


    /*$('#B').css('margin-left', 250);
//    $('#B').animate({ left: 250 });
    $("#menuBtnId").click(function () {
        $('#B').css('margin-left', 0);
        if ($(this).hasClass("menuitem_active")) {
            $(this).removeClass("menuitem_active");
            $(this).addClass("menuitem_deactive");
            $('#B').animate({ left: 0 });
//            $('.sidebar-nav').animate({ width: 0 });

        } else {
            $(this).removeClass("menuitem_deactive");
            $(this).addClass("menuitem_active");
            $('#B').animate({ left: 250 });
//            $('.sidebar-nav').animate({ width: 250 });
            
        }
    });*/
//    $('#B').animate({ left: 250 });
    /*  $(".grid-div").hide();
    var whatToDo = document.location.hash;
    if (whatToDo.length>0) {
        $(whatToDo).show();
    }*/

    /* $("#sidenav01 > li > a").on("click", function (e) {
        var divContent = $(this).attr("data-target");
//        $(divContent).show();
//        $(divContent).addClass("collapsed");
//        return;
//        alert($(divContent).attr('class'));
//        $(divContent).hide();
//        return;
        if ($(divContent).height()==0) {
            $(divContent).show();
            return;
        } else {
            $(divContent).hide();
        }
    });*/
    /*  $(".nav > li > a").on("click", function (e) {
        if (!$(this).hasAttr('id')) {
            return;
        }
        if ($(this).attr('id').indexOf("root")>=0) {
            return;
        }
        
        $("#selectLabel").html($(this).html());
        var divContent = $(this).parent().parent().attr("parent-label");
        $("#parentLabel").html($(divContent).text().trim());
//        ajaxifyGridMvc("#mylists");
        var idval = "#" + $(this).attr('id') + "Div";
        

        document.location.hash = "1";
        $(".grid-div").hide();
        $(idval).show();
        return;
        if (idval == 'inboxMenu') {
        }
        
        if (idval == 'outboxMenu') {
            $('#divid').load('/DicKindDocument/Index');
        }*/
    //       var divContent = $(this).attr("data-target");
//        $(divContent).show();
//        $(divContent).addClass("collapsed");
//        return;
//        alert($(divContent).attr('class'));
//        $(divContent).hide();
//        return;


});
