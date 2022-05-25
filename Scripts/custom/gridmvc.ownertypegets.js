﻿/***
* This script demonstrates how you can build you own custom filter widgets:
* 1. Specify widget type for column:
*       columns.Add(o => o.Customers.CompanyName)
*           .SetFilterWidgetType("CustomCompanyNameFilterWidget")
* 2. Register script with custom widget on the page:
*       <script src="@Url.Content("~/Scripts/gridmvc.customwidgets.js")" type="text/javascript"> </script>
* 3. Register your own widget in Grid.Mvc:
*       GridMvc.addFilterWidget(new CustomersFilterWidget());
*
* For more documentation see: http://gridmvc.codeplex.com/documentation
*/

/***
* CustomersFilterWidget - Provides filter user interface for customer name column in this project
* This widget onRenders select list with avaliable customers.
*/

function OwnerTypeFilterWidget() {
    /***
    * This method must return type of registered widget type in 'SetFilterWidgetType' method
    */
    this.getAssociatedTypes = function () {
        return ["OwnerTypeNameFilterWidget"];
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
        var html = '<div class="grid-filter-type-label"><i>Тип получателя лицензии</i></div>\
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
        $.post("GetOwnerTypeNames", function (data) {
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