(function($) { 

    "use strict";
//    console.log($('img[data-lazyload]'));
    $('img[data-lazyload]').map(function(item) {
        var url = $(this).attr('data-lazyload');
        if (url.indexOf('~/') === 0) {
            $(this).attr('data-lazyload', url.replace('~/', rootUrl));
        }
    });

    $('.cmbx-forms').change(function() {
        var formNumber = $(this).val();
        if (formNumber == 1)
            location.href = rootUrl + 'Home/Form1';
        else if (formNumber == 2)
            location.href = rootUrl + 'Home/Form2';
    });

    $('.cmbx-years').change(function () {
        var year = $(this).val();
        $('.curr-year').text(year);
    });

    $(window).load(function () {

    });

})(jQuery);