function LicensiatFilterWidget() {
    this.getAssociatedTypes = function() {
        return ["LicensiatNameFilterWidget"];
    };
 
    this.onShow = function () {
    };

    this.showClearFilterButton = function () {
        return true;
    };
  
    this.onRender = function (container, lang, typeName, filterType, filterValue, cb) {
        //store parameters:
        this.cb = cb;
        this.container = container;
        this.lang = lang;
        this.filterValue = filterValue;
        this.filterType = filterType;

        this.renderWidget(); //onRender filter widget
        this.loadCustomers(); //load customer's list from the server
        this.registerEvents(); //handle events
    };
    this.renderWidget = function () {
        var html = '<div class="grid-filter-type-label"><i>Признак заявителя</i></div>\
                    <div class="grid-filter-type-label">Выберите из списка:</div>\
                    <select style="width:250px;" class="grid-filter-type customerslist">\
                    </select>';
        this.container.append(html);
    };
    /***
    * Method loads all customers from the server via Ajax:
    */
    this.loadCustomers = function () {
        var $this = this;
        $.post("Licence/GetLicesiatNames", function (data) {
            $this.fillCustomers(data.Items);
        });
    };
    /***
    * Method fill customers select list by data
    */
    this.fillCustomers = function (items) {
        var customerList = this.container.find(".customerslist");
        for (var i = 0; i < items.length; i++) {
            customerList.append('<option ' + (items[i] == this.filterValue ? 'selected="selected"' : '') + ' value="' + items[i] + '">' + items[i] + '</option>');
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
            $context.cb("1"/* Equals */, $(this).val());
        });
    };

}