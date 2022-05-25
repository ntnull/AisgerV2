/**
 * Select2 Russian translation.
 *
 * @author  Uriy Efremochkin <efremochkin@uriy.me>
 */
(function ($) {
    "use strict";

    $.fn.select2.locales['kk'] = {
        formatNoMatches: function () { return "Сәйкестіктер табылған жоқ"; },
        formatInputTooShort: function (input, min) { return "Кем дегенде тағы  " + character(min - input.length); },
        formatInputTooLong: function (input, max) { return "Пожалуйста, введите на" + character(input.length - max) + " меньше"; },
        formatSelectionTooBig: function (limit) { return "Вы можете выбрать не более " + limit + " элемент" + (limit%10 == 1 && limit%100 != 11 ? "а" : "ов"); },
        formatLoadMore: function (pageNumber) { return "Деректер енгізуде…"; },
        formatSearching: function () { return "Издеу…"; }
    };

    $.extend($.fn.select2.defaults, $.fn.select2.locales['kk']);

    function character (n) {
        return " " + n + " символ енгізіңіз";
    }
})(jQuery);
