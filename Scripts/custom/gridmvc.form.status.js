
function StatusFilterWidget(codeName) {
    /***
    * This method must return type of registered widget type in 'SetFilterWidgetType' method
    */
    this.getAssociatedTypes = function () {
        return ["StatusNameFilterWidget"];
    };
    /***
    * This method invokes when filter widget was shown on the page
    */
    this.onShow = function () {
        /* Place your on show logic here */
    };

    this.showClearFilterButton = function () {
        return true;
    };
    /***
    * This method will invoke when user was clicked on filter button.
    * container - html element, which must contain widget layout;
    * lang - current language settings;
    * typeName - current column type (if widget assign to multipile types, see: getAssociatedTypes);
    * filterType - current filter type (like equals, starts with etc);
    * filterValue - current filter value;
    * cb - callback function that must invoked when user want to filter this column. Widget must pass filter type and filter value.
    */
    this.onRender = function (container, lang, typeName, values, cb, data) {
        //store parameters:
        this.cb = cb;
        this.container = container;
        this.lang = lang;

        //this filterwidget demo supports only 1 filter value for column column
        this.value = values.length > 0 ? values[0] : { filterType: 1, filterValue: "" };

        this.renderWidget(); //onRender filter widget
        this.loadCustomers(); //load customer's list from the server
        this.registerEvents(); //handle events
    };
    this.renderWidget = function () {
        var choose = "Выберите из списка";
        if (this.lang.code == "kk") {
            choose = "Таңдап алыңыз";
        }
        var html = '<p><i>Статус</i></p>\
                    <p>' + choose + ':</p>\
                    <select style="width:250px;" class="grid-filter-type customerslist form-control">\
                    </select>';
        this.container.append(html);
    };
    /***
    * Method loads all customers from the server via Ajax:
    */
    this.loadCustomers = function () {
        var $this = this;
        $.post("/Home/GetStatus?code=" + codeName, function (data) {
            $this.fillCustomers(data.Items);
        });
    };
    /***
    * Method fill customers select list by data
    */
    this.fillCustomers = function (items) {
        var customerList = this.container.find(".customerslist");
        for (var i = 0; i < items.length; i++) {
            customerList.append('<option ' + (items[i] == this.value.filterValue ? 'selected="selected"' : '') + ' value="' + items[i] + '">' + items[i] + '</option>');
        }
    };
    /***
    * Internal method that register event handlers for 'apply' button.
    */
    this.registerEvents = function () {
        //get list with customers
        var customerList = this.container.find(".customerslist");
        //save current context:
        var $context = this;
        //register onclick event handler
        customerList.change(function () {
            //invoke callback with selected filter values:
            var values = [{ filterValue: $(this).val(), filterType: 1 /* Equals */ }];
            $context.cb(values);
        });
    };

}