let productGridAPI;
function ActionCellRenderer() { }

ActionCellRenderer.prototype.init = function (params) {
    this.params = params;
    let hasAccess = $('#iamproductview').val() == 1 ? false : true;
    this.eGui = document.createElement('span');
    var btn = '';
    if (params.data.enabled && hasAccess) {
        btn += '<a href="#" class="link-primary" onclick="Common.OpenModal(this)" data-id="' + params.data.id + '" data-target="AddEditProduct" title="Edit"><i class="bi bi-pencil-square fs-6"></i></a>';
        btn += ' <a href="#" class="link-danger" onclick="Common.DeleteProduct(this)" data-id="' + params.data.id + '" title="Delete"><i class="bi bi-trash3 fs-6"></i></a>';
    }
    else {
        btn += '<span class="badge badge-dark">Deleted</span>';
    }
    this.eGui.innerHTML = btn;
}

ActionCellRenderer.prototype.getGui = function () {
    return this.eGui;
}
// Function to handle the file change event
function handleFileChange(event, rowId) {
    if (event.target.files.length === 0) {
        toastr.error("Please select a file.", '', { positionClass: 'toast-top-center' });
        return;
    }
    let file = event.target.files[0]; // Get the selected file
    // Validate file size (2 MB = 2 * 1024 * 1024 bytes)
    var maxSize = 2 * 1024 * 1024; // 2 MB
    if (file.size > maxSize) {
        toastr.error("File size must not exceed 2 MB.", '', { positionClass: 'toast-top-center' });
        return;
    }
    // Validate file type
    var allowedTypes = ['image/jpeg', 'image/png', 'image/gif'];
    if (!allowedTypes.includes(file.type)) {
        $('#message').html('<p style="color: red;">Invalid file type. Only JPG, PNG, and GIF are allowed.</p>');
        return;
    }


    // Create FormData for uploading the file
    let formData = new FormData();
    formData.append('file', file);
    formData.append('id', rowId); // Include row ID or other relevant data

    // Send file to the server using Fetch API
    fetch(baseUrl + 'api/product/image', {
        method: 'POST',
        body: formData,
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                toastr.success("File uploaded successfully!", '', { positionClass: 'toast-top-center' });
                productGridAPI.applyTransaction({ update: [data.data] });
            }
            else {
                toastr.error(data.message, '', { positionClass: 'toast-top-center' });
            }

        })
        .catch(error => {
            toastr.error("Unexpected error", '', { positionClass: 'toast-top-center' });
        });

}
function ImageCellRenderer() { }

ImageCellRenderer.prototype.init = function (params) {
    this.params = params;
    let img = document.createElement('img');
    img.src = params.value == null ? "/img/default.jpg" : params.value;
    img.setAttribute('class', 'agimg');
    // Apply cursor pointer for hover effect
    img.style.cursor = 'pointer';

    // Create a hidden file input element
    let fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.style.display = 'none';
    fileInput.accept = 'image/*'; // Accept only image files
    if (params.data.enabled) {
        // Attach the change event to the separate function
        fileInput.addEventListener('change', (event) => handleFileChange(event, params.data.id));

        // Trigger file input click on image click
        img.addEventListener('click', function () {
            fileInput.click();
        });
    }
    img.title = "Click to Change";

    this.eGui = document.createElement('span');
    this.eGui.setAttribute('class', 'agimgSpanLogo');
    this.eGui.appendChild(img);

}
ImageCellRenderer.prototype.getGui = function () {
    return this.eGui;
}
function onCellClickedEvent(params) {
    $('#ProductUsageSelect').val(params.data.id);
    $('#ProductUsageSelect').trigger('change');
    //$('#UsageQuantity').focus();
}
const stockClassRules = {
    'sick-days-warning': (params) => params.data.currentStock < params.data.reorderLevel
};
var productGridAPIOptions = {

    // define grid columns
    columnDefs: [
        {
            headerName: 'Img', field: 'picturePath', headerTooltip: 'Image', cellRenderer: "imageCellRenderer"
        },
        {
            headerName: 'Name', field: 'name', wrapText: false, filter: 'agTextColumnFilter', headerTooltip: 'Name', editable: true
        },
        {
            headerName: 'ગુજરાતી નામ', field: 'gujaratiName', filter: 'agTextColumnFilter', headerTooltip: 'Gujarati Name', editable: true
        },
        {
            headerName: 'Company', field: 'company', filter: 'agTextColumnFilter', headerTooltip: 'Company', editable: true
        },
        {
            headerName: 'Description', field: 'description', filter: 'agTextColumnFilter', headerTooltip: 'Description', editable: true
        },
        {
            headerName: 'Size & Unit', field: 'sizeUnitTypeCode', filter: 'agTextColumnFilter', headerTooltip: 'Size & Unit', editable: false
        },
        {
            headerName: 'Order Bulk Name', field: 'orderBulkName', filter: 'agTextColumnFilter', headerTooltip: 'Order Bulk Name', editable: true
        },
        {
            headerName: 'Order Bulk Quantity', field: 'orderBulkQuantity', filter: 'agTextColumnFilter', headerTooltip: 'Order Bulk Quantity', editable: true
        },
        {
            headerName: 'Current Stock', field: 'currentStock', filter: 'agNumberColumnFilter', headerTooltip: 'Stock', editable: true
            , cellClassRules: stockClassRules
        },
        {
            headerName: 'Reorder Level',
            field: 'reorderLevel', filter: 'agNumberColumnFilter', headerTooltip: 'Reorder Level', editable: true
        },
        {
            headerName: 'Is Disposable?', field: 'disposable', filter: 'agTextColumnFilter', headerTooltip: 'Is Disposable', editable: true,
            cellEditor: 'agSelectCellEditor',
            cellEditorParams: {
                values: ['Yes', 'No'],
            },
            // Ensure display always stays "Yes"/"No"
            valueFormatter: params => {
                if (params.value === true || params.value === 'true' || params.value === 1 || params.value === 'Yes')
                    return 'Yes';
                return 'No';
            },
            // Normalize user input to always store "Yes"/"No"
            valueParser: params => {
                const val = params.newValue?.toString().trim().toLowerCase();
                return val === 'yes' || val === 'true' || val === '1' ? 'Yes' : 'No';
            }
        },
        {
            headerName: 'Storage', field: 'storage', filter: 'agTextColumnFilter', headerTooltip: 'Storage', editable: true
        },
        {
            headerName: '', field: 'id', headerTooltip: 'Action', pinned: 'right', suppressSizeToFit: true,
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
        flex: 1,
        minWidth: 50,
        wrapText: true,
        autoHeight: true,
        floatingFilter: true,
    },
    getRowStyle: params => {
        if (params.node.data.enabled == false) {
            return { background: '#2C2C2C' };
        }
    },
    onCellValueChanged: onCellValueChanged,
    animateRows: true,
    //rowSelection: {
    //    mode: 'singleRow',
    //},
    pagination: true,
    //paginationAutoPageSize: true,
    paginationPageSize: 200,
    onCellClicked: onCellClickedEvent,
    getRowId: params => {
        return params.data.id;
    },
    //suppressContextMenu: true,
    components: {
        actionCellRenderer: ActionCellRenderer,
        imageCellRenderer: ImageCellRenderer
    },
    autoSizeStrategy: {
        type: 'fitCellContents'
    },
    onGridReady: function (params) {
        //productGridAPI.sizeColumnsToFit();
        productGridAPI.autoSizeAllColumns();
        //const allColumnIds = [];
        //productGridAPIOptions.columnApi.getAllColumns().forEach((column) => {
        //    if (column.colId != 'id')
        //        allColumnIds.push(column.colId);
        //});
        //productGridAPIOptions.columnApi.autoSizeColumns(allColumnIds, false);
    },
    onStateUpdated: onStateUpdated,
    overlayLoadingTemplate:
        '<span class="ag-overlay-loading-center">Please wait while your products are loading</span>',
    overlayNoRowsTemplate:
        `<div class="text-center">
                <h5 class="text-center"><b>Product(s) will appear here.</b></h5>
            </div>`
};

function onCellValueChanged(event) {
    const id = event.data.id;
    let newValue = event.newValue;
    const oldValue = event.oldValue;
    const field = event.colDef.field;
    // Convert Yes/No to true/false for backend
    if (field === 'disposable') {
        newValue = (newValue === 'Yes' || newValue === true);
    }
    const request = { id, field, newValue };

    fetch(baseUrl + 'api/product/inlineedit', {
        method: 'POST',
        body: JSON.stringify(request),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
    })
        .then(response => response.json())
        .then(data => {
            const rowNode = productGridAPI.getRowNode(id);

            if (data.success) {
                toastr.success("Updated", '', { positionClass: 'toast-top-center' });
                productGridAPI.flashCells({ rowNodes: [rowNode] });
            } else {
                toastr.error(data.message || "Update failed", '', { positionClass: 'toast-top-center' });
                // ✅ Restore old value WITHOUT triggering onCellValueChanged
                restoreOldValue(rowNode, field, oldValue);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            toastr.error("Network or server error", '', { positionClass: 'toast-top-center' });
            const rowNode = productGridAPI.getRowNode(id);
            restoreOldValue(rowNode, field, oldValue);
        });
}

// ✅ Helper function that restores value without retriggering AG Grid event
function restoreOldValue(rowNode, field, oldValue) {
    // Option 1 (recommended): directly mutate data and refresh
    rowNode.data[field] = oldValue;
    productGridAPI.refreshCells({ rowNodes: [rowNode], columns: [field] });

    // Option 2 (if transaction is preferred)
    // productGridAPI.applyTransactionAsync({ update: [rowNode.data] });
}


function onStateUpdated(event) {
    var state = productGridAPI.getState();
    localStorage.setItem("f840074316684a1d9074edcd72023fb3", JSON.stringify(state));
}
class Product {
    constructor(Id, Name, GujaratiName, Description, Size, UnitTypeCode, OrderBulkName, OrderBulkQuantity, ReorderLevel, IsDisposable, Company, StorageNames) {
        this.Id = parseInt(Id);
        this.Name = Common.ParseValue(Name);
        this.GujaratiName = Common.ParseValue(GujaratiName);
        this.Description = Common.ParseValue(Description);
        this.Size = Size;
        this.UnitTypeCode = Common.ParseValue(UnitTypeCode);
        this.OrderBulkName = OrderBulkName;
        this.OrderBulkQuantity = OrderBulkQuantity;
        this.ReorderLevel = ReorderLevel;
        this.IsDisposable = IsDisposable;
        this.Company = Common.ParseValue(Company);
        //this.StorageId = StorageId;
        this.StorageNames = StorageNames;
    }
}
class ProductUsageModel {
    constructor(ProductId, Quantity, Buyer) {
        this.ProductId = ProductId;
        this.Quantity = Quantity
        this.Buyer = Buyer;
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
            Common.BindValuesToProductForm(new Product(0, null, null, null, null, null, null, null, null, null));
        }
        else {
            $('#ModalTitle').html('Edit Product');
            Common.GetProductById(id);
        }
    }

    static DeleteProduct(mthis) {
        let id = $(mthis).data('id');
        if (confirm('Are you sure you want to delete this record?')) {
            fetch(baseUrl + 'api/product/delete/' + id, {
                method: 'DELETE',
                headers: {
                    //'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
            }).then(response => { return response.json() })
                .then(data => {
                    if (data.success) {
                        toastr.success("Deleted", '', { positionClass: 'toast-top-center' });
                        productGridAPI.applyTransaction({ remove: [data.data] });
                    }
                    else {
                        toastr.error(data.errors, '', { positionClass: 'toast-top-center' });
                    }
                })
                .catch(error => {
                    console.log(error);
                    toastr.success("Unexpected error", '', { positionClass: 'toast-top-center' });
                });
        }
    }
    static ResetGrid(mthis) {
        localStorage.removeItem('f840074316684a1d9074edcd72023fb3');
        window.location.reload();
    }
    static ApplyAGGrid() {
        var gridDiv = document.querySelector('#productsdata');
        var state = localStorage.getItem("f840074316684a1d9074edcd72023fb3");
        if (state) {
            productGridAPIOptions.initialState = JSON.parse(state);
        }
        productGridAPI = new agGrid.createGrid(gridDiv, productGridAPIOptions);
        productGridAPI.setGridOption("theme", agGrid.themeQuartz
            .withParams(
                {
                    backgroundColor: "#1e2838",
                    foregroundColor: "#FFFFFFCC",
                    browserColorScheme: "dark",
                },
                "dark-red",
            ));
        document.body.dataset.agThemeMode = "dark-red";

        fetch(baseUrl + 'api/products')
            .then((response) => response.json())
            .then(data => {
                //productGridAPI.setRowData(data);
                productGridAPI.setGridOption("rowData", data);
                Common.InitSelect2();
            })
            .catch(error => {
                productGridAPI.setGridOption("rowData", []);
                //productGridAPI.setRowData([])
                //toastr.error(error, '', {
                //    positionClass: 'toast-top-center'
                //});
            });

    }

    static BindValuesToProductForm(model) {
        $('#ProductErrorSection').empty();
        $('#Id').val(model.Id);
        $('#Name').val(model.Name);
        $('#GujaratiName').val(model.GujaratiName);
        $('#Description').val(model.Description);
        $('#Size').val(model.Size);
        //$('#OrderBulkName').val(model.OrderBulkName);
        $('#OrderBulkQuantity').val(model.OrderBulkQuantity);
        $('#UnitTypeCode').val(model.UnitTypeCode);
        $('#ReorderLevel').val(model.ReorderLevel);
        $('#IsDisposable').prop("checked", model.IsDisposable);
        $('#Company').val(model.Company);
        if (model.OrderBulkName != null) {
            $('#OrderBulkName').val(model.OrderBulkName).trigger('change');
        }
        else {
            $('#OrderBulkName').val('').trigger('change');
        }
        if (model.StorageNames != null) {
            $('#StorageNames').val(model.StorageNames.split(',')).trigger('change');
        }
        else {
            $('#StorageNames').val('').trigger('change');
        }
    }

    static init() {
        $('#productsdata').height(Common.calcDataTableHeight(27));
    }

    static async SaveProduct(mthis) {
        $('#ProductErrorSection').empty();
        let Id = $('#Id').val();
        let Name = $('#Name').val();
        let GujaratiName = $('#GujaratiName').val();
        let Description = $('#Description').val();
        let Size = $('#Size').val();
        let UnitTypeCode = $('#UnitTypeCode').val();
        let OrderBulkName = $('#OrderBulkName option:selected').val();
        let OrderBulkQuantity = $('#OrderBulkQuantity').val();
        let ReorderLevel = $('#ReorderLevel').val();
        let IsDisposable = $('#IsDisposable').is(':checked');
        let Company = $('#Company').val();
        let StorageNames = $('#StorageNames option:selected').toArray().map(item => item.text).join();
        let product = new Product(Id, Name, GujaratiName, Description, Size, UnitTypeCode, OrderBulkName, OrderBulkQuantity, ReorderLevel, IsDisposable, Company, StorageNames);

        var response = await fetch(baseUrl + 'api/product/save', {
            method: 'POST',
            body: JSON.stringify(product),
            headers: {
                //'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() });

        if (response.status > 399 && response.status < 500) {
            if (response != null) {
                var errorHtml = "";
                $.each(response.errors, function (index, element) {
                    errorHtml += element[0] + '<br/>';
                });
                $('#ProductErrorSection').html(errorHtml);
            }
        }
        if (response.success) {
            toastr.success("Product Saved", '', { positionClass: 'toast-top-center' });
            let target = $(mthis).data('target');
            $('#' + target).modal('hide');
            if (Id == 0) {
                productGridAPI.applyTransaction({ add: [response.data] });//addIndex
            }
            else {
                productGridAPI.applyTransaction({ update: [response.data] });
            }
            let rowNode = productGridAPI.getRowNode(response.data.id);
            productGridAPI.flashCells({ rowNodes: [rowNode] });
            $("#ProductUsageSelect").select2('destroy').empty();
            setTimeout(function () {
                Common.InitSelect2();
            }, 1000);
        }
    }
    static async GetProductById(id) {
        await fetch(baseUrl + 'api/product/byid/' + id, {
            method: 'GET',
            headers: {
                //'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() })
            .then(data => {
                Common.BindValuesToProductForm(new Product(data.id, data.name, data.gujaratiName, data.description, data.size, data.unitTypeCode, data.orderBulkName, data.orderBulkQuantity, data.reorderLevel, data.isDisposable, data.company, data.storageIds));
            })
            .catch(error => {
                console.log(error);
                toastr.success("Unexpected error", '', { positionClass: 'toast-top-center' });
            });
    }

    static BindSelectData() {
        var result = [];
        result.push({ id: '', text: '' });
        //productGridAPI.setGridOption("rows", data);
        productGridAPI.forEachNode((rowNode, index) => {
            result.push({ id: rowNode.data.id, text: rowNode.data.name });
        });
        return result;
    }
    static async InitSelect2() {
        $('#ProductUsageSelect').select2({
            placeholder: 'Search Product',
            //minimumInputLength: 1,
            //maximumSelectionLength: 1,
            //minimumResultsForSearch: 10,
            theme: "bootstrap4",
            data: Common.BindSelectData(),
            closeOnSelect: true,
            //allowClear: true
        });
        $('#BuyersSelect').select2({
            placeholder: 'Search Buyer',
            theme: "bootstrap4",
            closeOnSelect: true,
            tags: true
        });
        $('#StorageNames').select2({
            dropdownParent: $('#AddEditProduct'),
            placeholder: 'Search Storage',
            //theme: "bootstrap4",
            closeOnSelect: true,
            tags: true
        });
        $('#OrderBulkName').select2({
            dropdownParent: $('#AddEditProduct'),
            placeholder: 'Search OrderBulkName',
            //theme: "bootstrap4",
            closeOnSelect: true,
            tags: true,
            maximumSelectionLength: 1
        });

        //$(document).on('select2:open', () => {
        //    document.querySelector('.select2-search__field').focus();
        //});

    }

    static async UseProduct(mthis) {
        let productId = $('#ProductUsageSelect').val();
        let quantity = $('#UsageQuantity').val();
        let Buyer = $('#BuyersSelect').val();
        var productUsageModel = new ProductUsageModel(productId[0], quantity, Buyer);
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
            productGridAPI.applyTransaction({ update: [response.data] });
            let rowNode = productGridAPI.getRowNode(response.data.id);
            productGridAPI.flashCells({ rowNodes: [rowNode] });

            $('#ProductUsageSelect').val('').trigger('change');
            $('#UsageQuantity').val('');
            $('#BuyersSelect').val('').trigger('change');
            $('#ProductUsageSelect').select2('open');
            $('.select2-search__field').focus();
        }
        if (response.success == false) {

            toastr.error(response.errors, '', { positionClass: 'toast-top-center' });
        }
    }

    static async ExportToExcel() {
        productGridAPI.exportDataAsExcel({ fileName: 'Products.xlsx' });
    }

    static Search(mthis) {
        var search = $(mthis).val();
        productGridAPI.setGridOption(
            "quickFilterText",
            search,
        );

    }
}

jQuery(document).ready(function () {
    Common.init();
    Common.ApplyAGGrid();
});