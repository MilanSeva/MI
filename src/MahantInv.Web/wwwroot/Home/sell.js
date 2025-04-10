﻿let productUsageAPI;
function ActionCellRenderer() { }

ActionCellRenderer.prototype.init = function (params) {
    this.params = params;

    this.eGui = document.createElement('span');
    let btn = '';
    btn += ' <a href="#" class="link-danger" onclick="Common.DeleteSell(this)" data-id="' + params.data.id + '" title="Delete"><i class="bi bi-trash3 fs-6"></i></a>';
    this.eGui.innerHTML = btn;
}

ActionCellRenderer.prototype.getGui = function () {
    return this.eGui;
}
//function onCellClickedEvent(params) {
//    $('#ProductUsageSelect').val(params.data.id);
//    $('#ProductUsageSelect').trigger('change');
//    //$('#UsageQuantity').focus();
//}
const stockClassRules = {
    'sick-days-warning': (params) => params.data.currentStock < params.data.reorderLevel
};
var productUsageGridOptions = {

    // define grid columns
    columnDefs: [
        {
            headerName: 'Product', field: 'productName', wrapText: false, filter: 'agTextColumnFilter', headerTooltip: 'Product'
        },
        {
            headerName: 'Quantity', field: 'quantity', filter: 'agNumberColumnFilter', headerTooltip: 'Quantity',
            editable: true
        },
        {
            headerName: 'Buyer', field: 'buyer', filter: 'agTextColumnFilter', headerTooltip: 'Buyer'
        },
        {
            headerName: 'Usage Date', field: 'usageDateFormat', filter: 'agDateColumnFilter', headerTooltip: 'Usage Date'
        },
        {
            headerName: '', field: 'id', headerTooltip: 'Action', pinned: 'right', width: 80, suppressSizeToFit: true,
            cellRenderer: 'actionCellRenderer',
        }
    ],
    //sideBar: { toolPanels: ['columns', 'filters'] },
    //rowClassRules: {
    //    'sick-days-warning': function (params) {
    //        return params.data.currentStock < params.data.reorderLevel;
    //    },
    //},
    defaultColDef: {
        editable: false,
        sortable: true,
        resizable: true,
        //flex: 1,
        minWidth: 50,
        wrapText: true,
        autoHeight: true,
        floatingFilter: true,
    },
    animateRows: true,
    //rowSelection: 'single',
    pagination: true,
    paginationAutoPageSize: true,
    animateRows: true,
    defaultColGroupDef: {
        marryChildren: true
    },
    //onCellClicked: onCellClickedEvent,
    //onSelectionChanged: onSelectionChanged,
    getRowId: params => {
        return params.data.id;
    },
    suppressContextMenu: true,
    components: {
        actionCellRenderer: ActionCellRenderer
    },
    onCellValueChanged: onCellValueChanged,
    onStateUpdated: onStateUpdated,
    autoSizeStrategy: {
        type: "fitGridWidth",
        defaultMinWidth: 100,
        columnLimits: [
            {
                colId: "productName",
                minWidth: 500,
            },
        ],
    },
    onGridReady: function (params) {
        //productUsageAPI.autoSizeAllColumns();
        productUsageAPI.sizeColumnsToFit(
            gridApi.sizeColumnsToFit({
                defaultMinWidth: 100,
                columnLimits: [{ key: "productName" }],
            })
        );
    },
    overlayLoadingTemplate:
        '<span class="ag-overlay-loading-center">Please wait while your data are loading</span>',
    overlayNoRowsTemplate:
        `<div class="text-center">
                <h5 class="text-center"><b>Data will be appear here.</b></h5>
            </div>`
};
function onCellValueChanged(event) {
    //get the row id
    let id = event.data.id;
    let quantity = event.newValue;

    //Call the API to update the quantity
    fetch(baseUrl + 'api/product/usage/' + id, {
        method: 'PUT',
        body: quantity,
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
    }).then(response => { return response.json() })
        .then(data => {
            if (data.success) {
                toastr.success("Updated", '', { positionClass: 'toast-top-center' });
                //Get the row node by id
                let rowNode = productUsageAPI.getRowNode(id);
                //Update the row node in the grid
                rowNode.setDataValue('quantity', quantity);
                productUsageAPI.flashCells({ rowNodes: [rowNode] });
            }
            if (data.success == false) {
                toastr.error(data.errors, '', { positionClass: 'toast-top-center' });
            }
        }).catch(error => {
            console.log('err:', error);
            toastr.error(error, '', { positionClass: 'toast-top-center' });
        });
}
function onStateUpdated(event) {
    var state = productUsageAPI.getState();
    localStorage.setItem("612bd1bf907e4d08a2a86799e193ecfb", JSON.stringify(state));
}
class ProductUsageModel {
    constructor(ProductId, Quantity, Buyer, UsageDate) {
        this.ProductId = ProductId;
        this.Quantity = Quantity
        this.Buyer = Buyer;
        this.UsageDate = UsageDate;
    }
}
class Common {

    static ParseValue(val) {
        if (val == null) return null;
        if (val == '') return null;
        return val.trim();
    }
    static calcDataTableHeight(decreaseTableHeight) {
        return ($(window).innerHeight() - 150) - decreaseTableHeight;
    };

    static OpenModal(mthis) {
        let id = $(mthis).data('id');
        let target = $(mthis).data('target');
        $('#' + target).modal('show');
        if (id == 0) {
            $('#ModalTitle').html('Add Product');
            Common.BindValuesToProductForm(new Product(0, null, null, null, null, null, null, null, null));
        }
        else {
            $('#ModalTitle').html('Edit Product');
            Common.GetProductById(id);
        }
    }
    static ResetGrid(mthis) {
        localStorage.removeItem('612bd1bf907e4d08a2a86799e193ecfb');
        window.location.reload();
    }
    static ApplyAGGrid() {

        var gridDiv = document.querySelector('#usagedata');
        var state = localStorage.getItem("612bd1bf907e4d08a2a86799e193ecfb");
        if (state) {
            productUsageGridOptions.initialState = JSON.parse(state);
        }
        productUsageAPI = new agGrid.createGrid(gridDiv, productUsageGridOptions);
        productUsageAPI.setGridOption("theme", agGrid.themeQuartz
            .withParams(
                {
                    backgroundColor: "#1e2838",
                    foregroundColor: "#FFFFFFCC",
                    browserColorScheme: "dark",
                },
                "dark-red",
            ));
        document.body.dataset.agThemeMode = "dark-red";
        fetch(baseUrl + 'api/usages')
            .then((response) => response.json())
            .then(data => {
                productUsageAPI.setGridOption("rowData", data);
                Common.InitSelect2();
            })
            .catch(error => {
                console.log('err:', error);
                productUsageAPI.setGridOption("rowData", []);
                //toastr.error(error, '', {
                //    positionClass: 'toast-top-center'
                //});
            });

    }

    //static BindValuesToProductForm(model) {
    //    $('#ProductErrorSection').empty();
    //    $('#Id').val(model.Id);
    //    $('#Name').val(model.Name);
    //    $('#Description').val(model.Description);
    //    $('#Size').val(model.Size);
    //    $('#UnitTypeCode').val(model.UnitTypeCode);
    //    $('#ReorderLevel').val(model.ReorderLevel);
    //    $('#IsDisposable').prop("checked", model.IsDisposable);
    //    $('#Company').val(model.Company);
    //    $('#StorageId').val(model.StorageId);
    //}

    static init() {
        $('#usagedata').height(Common.calcDataTableHeight(27));
        Common.ProductSearchSelect2();
    }

    static async DeleteSell(mthis) {
        //get confirmation using confirm box
        let isDelete = confirm('Are you sure to delete this record?');

        if (!isDelete) return;

        let id = $(mthis).data('id');
        let response = await fetch(baseUrl + 'api/product/usage/' + id, {
            method: 'DELETE',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() });
        if (response.success) {
            toastr.success("Deleted", '', { positionClass: 'toast-top-center' });
            //Get the row node by id
            let rowNode = productUsageAPI.getRowNode(id);
            //Remove the row node from the grid
            productUsageAPI.applyTransaction({ remove: [rowNode] });
        }
        if (response.success == false) {
            toastr.error(response.errors, '', { positionClass: 'toast-top-center' });
        }
    }

    //static BindSelectData() {
    //    var result = [];
    //    result.push({ id: '', text: '' });
    //    productUsageGridOptions.api.forEachNode((rowNode, index) => {
    //        result.push({ id: rowNode.data.id, text: rowNode.data.name });
    //    });
    //    return result;
    //}
    static async ProductSearchSelect2() {
        let response = await fetch(baseUrl + 'api/product/search', {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() });

        $('#ProductUsageSelect').prepend('<option selected></option>').select2({
            placeholder: 'Search Product',
            closeOnSelect: true,
            allowClear: true,
            minimumResultsForSearch: 10,
            data: response,
            templateResult: function (repo) {
                if (repo.loading) {
                    return repo.name;
                }
                var $container = $(
                    "<div class='select2-result-repository clearfix'>" +
                    "<div class='select2-result-repository__avatar'><img src='" + repo.picturePath + "'></div>" +
                    "<div class='select2-result-repository__meta'>" +
                    "<div class='select2-result-repository__title'>" + repo.gujaratiName + "</div>" +
                    "<div class='select2-result-repository__description'>" + repo.description + "</div>" +
                    "<div class='select2-result-repository__statistics'>" +
                    "<div class='select2-result-repository__forks'>" + repo.size + "" + repo.unitTypeCode + "</div>" +
                    "<div class='select2-result-repository__stargazers'>" + repo.company + "</div>" +
                    "<div class='select2-result-repository__watchers'>" + repo.storage + "</div>" +
                    "</div>" +
                    "</div>" +
                    "</div>"
                );
                return $container;
            },
            templateSelection: function (repo) {
                return repo.name
            }
        });
    }
    static async InitSelect2() {
        //$('#ProductUsageSelect').select2({
        //    placeholder: 'Search Product',
        //    theme: "bootstrap4",
        //    data: Common.BindSelectData(),
        //    closeOnSelect: true,
        //});
        $('#BuyersSelect').select2({
            placeholder: 'Search Buyer',
            //theme: "bootstrap4",
            closeOnSelect: true,
            tags: true
        });
        $(document).on('select2:open', () => {
            document.querySelector('.select2-search__field').focus();
        });
    }

    static async UseProduct(mthis) {
        let productId = $('#ProductUsageSelect').val();
        let quantity = $('#UsageQuantity').val();
        let Buyer = $('#BuyersSelect').val();
        let UsageDate = $('#UsageDate').val();
        var productUsageModel = new ProductUsageModel(productId, quantity, Buyer, UsageDate);
        var response = await fetch(baseUrl + 'api/product/usage', {
            method: 'POST',
            body: JSON.stringify(productUsageModel),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() });
        if (response.status > 399 && response.status < 500) {
            if (response != null) {
                var errorHtml = "";
                $.each(response.errors, function (index, element) {
                    errorHtml += element[0] + '.';
                });
                toastr.error(errorHtml, '', { positionClass: 'toast-top-center' });
            }
        }
        if (response.success) {
            toastr.success("Saved", '', { positionClass: 'toast-top-center' });
            productUsageAPI.applyTransaction({ add: [response.data], addIndex: 0 });
            let rowNode = productUsageAPI.getRowNode(response.data.id);
            productUsageAPI.flashCells({ rowNodes: [rowNode] });

            $('#ProductUsageSelect').val('').trigger('change');
            $('#UsageQuantity').val('');
            $('#BuyersSelect').val('').trigger('change');
            //$('#UsageDate').val('');
            //$('#ProductUsageSelect').select2('open');
            $('.select2-search__field').focus();
            MyNotification.GetPendingORNotifiedNotifications();
        }
        if (response.success == false) {
            toastr.error(response.errors, '', { positionClass: 'toast-top-center' });
        }
    }

    static async ExportToExcel() {
        productUsageGridOptions.api.exportDataAsExcel({ fileName: 'Usages.xlsx' });
    }
}

jQuery(document).ready(function () {
    Common.init();
    Common.ApplyAGGrid();
});