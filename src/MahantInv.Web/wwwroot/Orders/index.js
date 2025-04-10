﻿let orderGridAPI;
var orderTransaction = [];
let editModeIdx = -1;
let bulkOrderName = '';
let bulkOrderQuantity = 1;
function ActionCellRenderer() { }

ActionCellRenderer.prototype.init = function (params) {
    var cellBlank = !params.value;
    if (cellBlank) {
        return null;
    }
    this.params = params;

    this.eGui = document.createElement('div');
    let btn = '';
    if (params.data.status != 'Ordered') {
        btn += '<a href="#" class="link-primary" onclick="Common.OpenModal(this)" data-id="' + params.data.id + '" data-target="PlaceOrder" title="View"><i class="bi bi-eye"></i></a>';
    }
    else {
        btn += '<a href="#" class="link-info" onclick="Common.OpenModal(this)" data-id="' + params.data.id + '" data-target="PlaceOrder" title="Edit"><i class="bi bi-pencil-square"></i></a>';
    }
    btn += ' <a href="#" class="link-danger" onclick="Common.DeleteAction(this)" data-id="' + params.data.id + '" title="Delete"><i class="bi bi-trash"></i></a>';
    this.eGui.innerHTML = btn;
}

ActionCellRenderer.prototype.getGui = function () {
    return this.eGui;
}
const stockClassRules = {
    'sick-days-warning': (params) => params.data.currentStock < params.data.reorderLevel
};

const spanCellClassRules = {
    'cell-span': (params) => params.data.orderTransactionsCount > 1
};

var orderGridOptions = {

    // define grid columns
    columnDefs: [
        {
            headerName: 'Product', field: 'product', filter: 'agTextColumnFilter', headerTooltip: 'Name'
            //, cellRenderer: "agGroupCellRenderer"
        },
        {
            headerName: 'Order Date', field: 'orderDate', filter: 'agDateColumnFilter', headerTooltip: 'Order Date'
        },
        {
            headerName: 'Ordered Quantity', field: 'quantity', filter: 'agNumberColumnFilter', headerTooltip: 'Ordered Quantity'
        },
        {
            headerName: 'Received Quantity', field: 'receivedQuantity', filter: 'agNumberColumnFilter', headerTooltip: 'Received Quantity'
        },
        {
            headerName: 'Net Amount', field: 'netAmount', filter: 'agNumberColumnFilter', headerTooltip: 'Net Amount'
        },
        {
            headerName: 'Payment Status', field: 'paymentStatus', filter: 'agTextColumnFilter', headerTooltip: 'Payment Status',
            cellClassRules: {
                "text-danger": (params) => params.value == "Unpaid",
                "text-warning": (params) => params.value == "Partially Paid",
                "text-success": (params) => params.value == "Paid"
            },
            //cellRenderer: function (params) {
            //    if (params.value == 'Partially Paid') {
            //        return '<span>' + params.value + '</span> <span class="badge badge-danger">' + params.data.pendingAmount + '</span>';
            //    }
            //    else {
            //        return params.value;
            //    }
            //},
        },
        {
            headerName: 'Seller', field: 'seller', filter: 'agTextColumnFilter', headerTooltip: 'Seller',
        },
        {
            headerName: 'Order Status', field: 'status', filter: 'agTextColumnFilter', headerTooltip: 'Status',
            cellRenderer: function (params) {
                if (params.value == 'Ordered') {
                    return '<button type="button" class="btn btn-link btn-sm" onclick="Common.OpenModal(this)" data-id="' + params.data.id + '" data-target="PlaceOrder">' + params.value + '</button>'
                }

                let cls = params.value == 'Received' ? 'success' : 'danger';
                return '<span class="text text-' + cls + '">' + params.value + '</span>';
            }
        },
        {
            headerName: 'Received date', field: 'receivedDate', filter: 'agdatecolumnfilter', headerTooltip: 'received date',
        },
        {
            headerName: 'Remark', field: 'remark', filter: 'agtextcolumnfilter', headerTooltip: 'Remark', minWidth: 100
        },
        {
            headerName: '', field: 'id', headerTooltip: 'Action'
            , pinned: 'right',
            width: 80, suppressSizeToFit: true,
            cellRenderer: 'actionCellRenderer',
        }
    ],
    //sideBar: { toolPanels: ['columns', 'filters'] },
    defaultColDef: {
        editable: false,
        sortable: true,
        resizable: true,
        flex: 1,
        minWidth: 50,
        wrapText: false,
        //autoHeight: true,
        floatingFilter: true,
    },
    //masterDetail: true,
    //detailRowAutoHeight: true,
    //detailCellRendererParams: {
    //    suppressDetailGrid: true // This ensures detail rows are collapsed by default
    //},
    //detailCellRendererParams: {
    //    detailGridOptions: {
    //        columnDefs: [
    //            { field: "party" },
    //            { field: "paymentType" },
    //            { field: "amount" },
    //            { field: "paymentDate" },
    //        ],
    //        defaultColDef: {
    //            flex: 1,
    //        },
    //    },
    //    getDetailRowData: (params) => {
    //        params.successCallback(params.data.orderTransactions);
    //    },
    //},
    suppressRowTransform: true,
    pagination: true,
    paginationPageSize: 200,
    //paginationAutoPageSize: true,
    animateRows: true,

    getRowId: params => {
        return params.data.id;
    },
    suppressContextMenu: true,
    components: {
        actionCellRenderer: ActionCellRenderer
    },
    columnTypes: {
        numberColumn: {
            minWidth: 50,
        },
        dateColumn: {
            minWidth: 130,
        }
    },
    onStateUpdated: onStateUpdated,
    autoSizeStrategy: {
        type: "fitGridWidth",
        defaultMinWidth: 100,
        columnLimits: [
            {
                colId: "product",
                minWidth: 500,
            },
        ],
    },
    onGridReady: function (params) {
        orderGridAPI.sizeColumnsToFit(
            gridApi.sizeColumnsToFit({
                defaultMinWidth: 100,
                columnLimits: [{ key: "productName" }],
            })
        );
    },
    overlayLoadingTemplate:
        '<span class="ag-overlay-loading-center">Loading your orders, please wait…</span>',
    overlayNoRowsTemplate:
        `<div class="text-center">
                <h5 class="text-center"><b>Oops! We couldn’t find any records matching your search.</b></h5>
            </div>`
};

function onStateUpdated(event) {
    var state = orderGridAPI.getState();
    localStorage.setItem("9427363582ba4ccda0a9aa2fcd422bc77", JSON.stringify(state));
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
        this.StorageNames = StorageNames;
    }
}
class Order {
    constructor(Id, ProductId, Quantity, SellerId, OrderDate, Remark, PricePerItem, Discount, Tax, DiscountAmount, NetAmount, ReceivedQuantity, ReceivedDate) {
        this.Id = parseInt(Id);
        this.ProductId = ProductId;
        this.Quantity = Quantity;
        this.SellerId = SellerId;
        this.OrderDate = OrderDate;
        this.Remark = Common.ParseValue(Remark);
        this.ReceivedQuantity = ReceivedQuantity;
        this.ReceivedDate = ReceivedDate;
        this.OrderTransactions = [];
        this.PricePerItem = PricePerItem;
        this.Discount = Discount;
        this.Tax = Tax;
        this.DiscountAmount = DiscountAmount;
        this.NetAmount = NetAmount;
    }
}
class OrderTransaction {
    constructor(Id, PartyId, Party, PaymentTypeId, PaymentType, Amount, PaymentDate) {
        this.Id = Id;
        this.PartyId = PartyId;
        this.Party = Party;
        this.PaymentTypeId = PaymentTypeId;
        this.PaymentType = PaymentType;
        this.Amount = Amount;
        this.PaymentDate = PaymentDate;
    }
}
class Party {
    constructor(Id, Name, Type, CategoryId, City, Country) {
        this.Id = parseInt(Id);
        this.Name = Common.ParseValue(Name);
        this.Type = Common.ParseValue(Type);
        this.CategoryId = CategoryId;
        this.City = Common.ParseValue(City);
        this.Country = Common.ParseValue(Country);
    }
}
class DiscountAndNetPay {
    constructor(DiscountAmount, NetAmount) {
        this.DiscountAmount = DiscountAmount;
        this.NetAmount = NetAmount;
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
        editModeIdx = -1;
        orderTransaction = [];
        $('#BulkNameQuantity').val('');
        $('#BulkNameQuantityLabel').html('.');
        if (id == 0) {
            $('#actionsection').find('.cancelbtn').hide();
            Common.BindValuesToOrderForm(new Order(0, null, null, null, moment().format("YYYY-MM-DD"), null, null, null, null, null, null, null, null, null, null));
        }
        else {
            $('#actionsection').find('.cancelbtn').show();
            Common.GetOrderById(id);
        }
    }

    static OpenProductModal(mthis) {

        $('#ProductErrorSection').empty();
        //$('#Id').val(model.Id);
        $('#Name').val('');
        $('#GujaratiName').val('');
        $('#Description').val('');
        $('#Size').val('');
        $('#UnitTypeCode').val('');
        $('#OrderBulkName').val('');
        $('#OrderBulkQuantity').val('');
        $('#ReorderLevel').val('');
        $('#IsDisposable').prop("checked", false);
        $('#Company').val('');
        $('#StorageNames').val('').trigger('change');

        let target = $(mthis).data('target');
        $('#' + target).modal('show');

    }

    static OpenPartyModal(mthis) {

        //$('#PartyErrorSection').empty();
        //$('#Id').val(model.Id);
        $('#ProductName').val('');
        $('#Type').val('Both');
        $('#CategoryId').val('');
        $('#City').val('');
        $('#Country').val('');

        $('#AddParty').modal('show');
    }
    static OpenActionModal(mthis) {
        let id = $(mthis).data('id');
        $('#ReceivedOrCancelledOrder').modal('show');
        let rowData = orderGridAPI.getRowNode(id).data;
        $('#OrderId').val(rowData.id);
        $('#ProductId').val(rowData.productId);
        $('#ProductName').html(rowData.productName);
        $('#Quantity').val(rowData.quantity);
        $('#PaymentTypeId').val(rowData.paymentTypeId).trigger('change');
        $('#PayerId').val(rowData.payerId).trigger('change');
        $('#PaidAmount').val(rowData.paidAmount);
        $('#OrderDate').val(moment(rowData.orderDate).format("YYYY-MM-DD"));
        $('#ReceivedQuantity').val(rowData.receivedQuantity);
        $('#ReceivedDate').val(moment().format("YYYY-MM-DD"));
        $('#Remark').val(rowData.remark);
    }
    static ResetGrid(mthis) {
        localStorage.removeItem('9427363582ba4ccda0a9aa2fcd422bc77');
        window.location.reload();
    }
    static ApplyAGGrid() {
        var gridDiv = document.querySelector('#ordersdata');
        var state = localStorage.getItem("9427363582ba4ccda0a9aa2fcd422bc77");
        if (state) {
            orderGridOptions.initialState = JSON.parse(state);
        }
        orderGridAPI = new agGrid.createGrid(gridDiv, orderGridOptions);
        orderGridAPI.setGridOption("theme", agGrid.themeQuartz
            .withParams(
                {
                    backgroundColor: "#1e2838",
                    foregroundColor: "#FFFFFFCC",
                    browserColorScheme: "dark",
                },
                "dark-red",
            ));
        document.body.dataset.agThemeMode = "dark-red";
    }

    static LoadDataInGrid(startDate, endDate) {
        fetch(baseUrl + 'api/orders', {
            method: 'POST',
            body: JSON.stringify({ startDate: startDate.format('YYYY-MM-DD'), endDate: endDate.format('YYYY-MM-DD') }),
            headers: {
                //'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        })
            .then((response) => response.json())
            .then(data => {

                orderGridAPI.setGridOption("rowData", data);
            })
            .catch(error => {
                console.log('error:', error);
                orderGridAPI.setGridOption("rowData", []);
                //toastr.error(error, '', {
                //    positionClass: 'toast-top-center'
                //});
            });
    }


    static BindValuesToOrderForm(model) {
        $('#OrderErrorSection').empty();
        $('#Id').val(model.Id);
        $('#ProductId').val(model.ProductId).trigger('change');
        $('#Quantity').val(model.Quantity);
        $('#SellerId').val(model.ProductId).trigger('change');
        $('#OrderDate').val(moment(model.OrderDate).format("YYYY-MM-DD"));
        $('#Remark').val(model.Remark);
        $('#ReceivedQuantity').val(model.ReceivedQuantity);
        $('#ReceivedDate').val(model.ReceivedDate == null ? null : moment(model.ReceivedDate).format("YYYY-MM-DD"));
        $('#PricePerItem').val(model.PricePerItem);
        $('#Discount').val(model.Discount == null ? 0 : model.Discount == model.DiscountAmount ? model.Discount : model.Discount.toString().concat('%'));
        $('#Tax').val(model.Tax);
        $('#DiscountAmount').val(model.DiscountAmount);
        $('#NetAmount').val(model.NetAmount);
        $('#OrderTransactionSummarySectionPaidAmount').html(0);
        $('#OrderTransactionSummarySectionPendingAmount').html(model.NetAmount ?? 0);

        if (model.OrderTransactions.length == 0) {
            $('#OrderTransactionBody').html("<tr><td colspan='5' class='text-center alert alert-info'>Transaction(s) will be appear here.</td></tr>");
        }
        else {
            orderTransaction = [];
            for (var i = 0; i < model.OrderTransactions.length; i++) {
                orderTransaction.push(new OrderTransaction(model.OrderTransactions[i].Id, model.OrderTransactions[i].PartyId, model.OrderTransactions[i].Party,
                    model.OrderTransactions[i].PaymentTypeId, model.OrderTransactions[i].PaymentType, model.OrderTransactions[i].Amount, moment(model.OrderTransactions[i].PaymentDate, 'YYYY-MM-DD').format('DD/MM/YYYY')))
            }
            //orderTransaction = model.orderTransactions;
            Common.UpdateOrderTransactionGrid();
        }
    }
    static BindSelectData() {
        var result = 'India,Afghanistan,Aland Islands,Albania,Algeria,American Samoa,Andorra,Angola,Anguilla,Antarctica,Antigua and Barbuda,Argentina,Armenia,Aruba,Australia,Austria,Azerbaijan,Bahamas,Bahrain,Bangladesh,Barbados,Belarus,Belgium,Belize,Benin,Bermuda,Bhutan,Bolivia,Bosnia and Herzegovina,Botswana,Bouvet Island,Brazil,British Indian Ocean Territory,British Virgin Islands,Brunei,Bulgaria,Burkina Faso,Burundi,Cambodia,Cameroon,Canada,Cape Verde,Caribbean Netherlands,Cayman Islands,Central African Republic,Chad,Chile,China,Christmas Island,Cocos (Keeling) Islands,Colombia,Comoros,Cook Islands,Costa Rica,Croatia,Cuba,Curaçao,Cyprus,Czechia,Denmark,Djibouti,Dominica,Dominican Republic,DR Congo,Ecuador,Egypt,El Salvador,Equatorial Guinea,Eritrea,Estonia,Eswatini,Ethiopia,Falkland Islands,Faroe Islands,Fiji,Finland,France,French Guiana,French Polynesia,French Southern and Antarctic Lands,Gabon,Gambia,Georgia,Germany,Ghana,Gibraltar,Greece,Greenland,Grenada,Guadeloupe,Guam,Guatemala,Guernsey,Guinea,Guinea-Bissau,Guyana,Haiti,Heard Island and McDonald Islands,Honduras,Hong Kong,Hungary,Iceland,Indonesia,Iran,Iraq,Ireland,Isle of Man,Israel,Italy,Ivory Coast,Jamaica,Japan,Jersey,Jordan,Kazakhstan,Kenya,Kiribati,Kosovo,Kuwait,Kyrgyzstan,Laos,Latvia,Lebanon,Lesotho,Liberia,Libya,Liechtenstein,Lithuania,Luxembourg,Macau,Madagascar,Malawi,Malaysia,Maldives,Mali,Malta,Marshall Islands,Martinique,Mauritania,Mauritius,Mayotte,Mexico,Micronesia,Moldova,Monaco,Mongolia,Montenegro,Montserrat,Morocco,Mozambique,Myanmar,Namibia,Nauru,Nepal,Netherlands,New Caledonia,New Zealand,Nicaragua,Niger,Nigeria,Niue,Norfolk Island,North Korea,North Macedonia,Northern Mariana Islands,Norway,Oman,Pakistan,Palau,Palestine,Panama,Papua New Guinea,Paraguay,Peru,Philippines,Pitcairn Islands,Poland,Portugal,Puerto Rico,Qatar,Republic of the Congo,Réunion,Romania,Russia,Rwanda,Saint Barthélemy,Saint Helena, Ascension and Tristan da Cunha,Saint Kitts and Nevis,Saint Lucia,Saint Martin,Saint Pierre and Miquelon,Saint Vincent and the Grenadines,Samoa,San Marino,São Tomé and Príncipe,Saudi Arabia,Senegal,Serbia,Seychelles,Sierra Leone,Singapore,Sint Maarten,Slovakia,Slovenia,Solomon Islands,Somalia,South Africa,South Georgia,South Korea,South Sudan,Spain,Sri Lanka,Sudan,Suriname,Svalbard and Jan Mayen,Sweden,Switzerland,Syria,Taiwan,Tajikistan,Tanzania,Thailand,Timor-Leste,Togo,Tokelau,Tonga,Trinidad and Tobago,Tunisia,Turkey,Turkmenistan,Turks and Caicos Islands,Tuvalu,Uganda,Ukraine,United Arab Emirates,United Kingdom,United States,United States Minor Outlying Islands,United States Virgin Islands,Uruguay,Uzbekistan,Vanuatu,Vatican City,Venezuela,Vietnam,Wallis and Futuna,Western Sahara,Yemen,Zambia,Zimbabwe';
        return result.split(',');
    }

    static init() {
        $('#ordersdata').height(Common.calcDataTableHeight(27));
        Common.ApplyAGGrid();
        var start = moment().subtract(90, 'days');
        var end = moment();
        Common.LoadDataInGrid(start, end);
        $('#ordersdaterange').daterangepicker({
            opens: 'left',
            startDate: start,
            endDate: end,
            ranges: {
                'Today': [moment(), moment()],
                'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                'This Month': [moment().startOf('month'), moment().endOf('month')],
                'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
            }
        }, function (start, end) {
            $('#ordersdaterange').val(start.format('D/MM/YYYY') + ' - ' + end.format('D/MM/YYYY'));
            Common.LoadDataInGrid(start, end);
        });

        $('.select2').select2({
            dropdownParent: $('#PlaceOrder'),
            placeholder: 'Search option',
            theme: "bootstrap4",
            allowClear: true
        });
        $('#Country').select2({
            placeholder: 'Search Country',
            data: Common.BindSelectData(),
            theme: "bootstrap4",
            dropdownParent: $("#AddParty")
        });
        $('#StorageNames').select2({
            dropdownParent: $('#AddProduct'),
            placeholder: 'Search Storage',
            //theme: "bootstrap4",
            closeOnSelect: true,
            tags: true
        });
        Common.GetAllProducts();
        Common.InitCountable();

        $('#PlaceOrder').find('.modal-dialog').css('max-width', '{v}px'.replace('{v}', ($(window).width() - 100)));
    }
    static async GetAllProducts() {
        let response = await fetch(baseUrl + 'api/product/search', {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() });

        $('#ProductId').select2({
            dropdownParent: $('#PlaceOrder'),
            placeholder: 'Search Product',
            closeOnSelect: true,
            allowClear: true,
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
                bulkOrderName = repo.orderBulkName;
                bulkOrderQuantity = repo.orderBulkQuantity;
                Common.SetBulkNameAndQuantity();
                return repo.name
            }
        });
    }
    static SetBulkNameAndQuantity() {
        bulkOrderName = bulkOrderName == null || bulkOrderName == "" ? "-" : bulkOrderName;
        bulkOrderQuantity = bulkOrderQuantity == null || bulkOrderQuantity == "" ? "-" : bulkOrderQuantity;
        let bulkDispLabel = bulkOrderName + "(" + bulkOrderQuantity + ")";
        $('#BulkNameQuantityLabel').html(bulkDispLabel);
    }
    static async SaveOrder(mthis) {
        $('#OrderErrorSection').empty();
        let order = Common.BuildOrderValues();
        var response = await fetch(baseUrl + 'api/order/save', {
            method: 'POST',
            body: JSON.stringify(order),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() });
        if (response.status > 399 && response.status < 500) {
            if (response != null) {
                var errorHtml = "";
                $.each(response.errors, function (index, element) {
                    errorHtml += element[0] + '<br/>';
                });
                $('#OrderErrorSection').html(errorHtml);
            }
        }
        if (response.success) {
            toastr.success("Order Saved", '', { positionClass: 'toast-top-center' });
            let target = $(mthis).data('target');
            $('#' + target).modal('hide');
            if (order.Id == 0) {
                orderGridAPI.applyTransaction({ add: response.data, addIndex: 0 });
            }
            else {
                orderGridAPI.applyTransaction({ update: response.data });
            }
            let rowNode = orderGridAPI.getRowNode(response.data[0].id);
            orderGridAPI.flashCells({ rowNodes: [rowNode] });
            $('#NewOrderBtn').click();
        }
        if (response.success == false) {
            var errorHtml = "";
            $.each(response.errors, function (index, element) {
                errorHtml += element[0].errorMessage + '<br/>';
            });
            $('#OrderErrorSection').html(errorHtml);
        }
    }
    static async GetOrderById(id) {
        await fetch(baseUrl + 'api/order/byid/' + id, {
            method: 'GET',
            headers: {
                //'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() })
            .then(data => {
                var order = new Order(data.id, data.productId, data.quantity, data.sellerId, data.orderDate, data.remark, data.pricePerItem, data.discount, data.tax, data.discountAmount, data.netAmount, data.receivedQuantity, data.receivedDate);
                order.OrderTransactions = [];
                if (data.orderTransactions != null && data.orderTransactions.length > 0) {
                    $.each(data.orderTransactions, function (i, v) {
                        order.OrderTransactions.push(new OrderTransaction(v.id, v.partyId, v.party, v.paymentTypeId, v.paymentType, v.amount, v.paymentDate));
                    });
                }
                Common.BindValuesToOrderForm(order);

                if (data.status == 'Received') {
                    $('#ReceivedQuantity').attr('readonly', true);
                }
                else {
                    $('#ReceivedQuantity').attr('readonly', false);
                }
                if (data.status == 'Ordered') {
                    $('.cancelbtn').show();
                    $('.saveorderbtn').show();
                    $('.receiveorderbtn').show();
                }
                else {
                    $('.cancelbtn').hide();
                    $('.receiveorderbtn').hide();
                }
            })
            .catch(error => {
                console.log(error);
                toastr.error("Unexpected error", '', { positionClass: 'toast-top-center' });
            });
    }
    static BuildOrderValues() {
        let Id = $('#Id').val();
        let ProductId = $('#ProductId').val();
        let Quantity = $('#Quantity').val();
        let SellerId = $('#SellerId').val();
        let OrderDate = $('#OrderDate').val();
        let Remark = $('#Remark').val();
        let ReceivedQuantity = $('#ReceivedQuantity').val();
        let ReceivedDate = $('#ReceivedDate').val();
        let PricePerItem = $('#PricePerItem').val();
        let Discount = parseFloat($('#Discount').val());
        let Tax = $('#Tax').val();
        let DiscountAmount = $('#DiscountAmount').val();
        let NetAmount = $('#NetAmount').val();
        let order = new Order(Id, ProductId, Quantity, SellerId, OrderDate, Remark, PricePerItem, Discount, Tax, DiscountAmount, NetAmount, ReceivedQuantity, ReceivedDate);
        //order.OrderTransactions = orderTransaction;
        for (var i = 0; i < orderTransaction.length; i++) {
            order.OrderTransactions.push(new OrderTransaction(orderTransaction[i].Id, orderTransaction[i].PartyId, orderTransaction[i].Party,
                orderTransaction[i].PaymentTypeId, orderTransaction[i].PaymentType, orderTransaction[i].Amount, moment(orderTransaction[i].PaymentDate, 'DD/MM/YYYY').format('YYYY-MM-DD')))
        }
        return order;
    }

    static async ReceiveOrder(mthis) {
        $('#OrderErrorSection').empty();
        let order = Common.BuildOrderValues();
        var response = await fetch(baseUrl + 'api/order/receive', {
            method: 'POST',
            body: JSON.stringify(order),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() });
        if (response.status > 399 && response.status < 500) {
            if (response != null) {
                var errorHtml = "";
                $.each(response.errors, function (index, element) {
                    errorHtml += element[0] + '<br/>';
                });
                $('#OrderErrorSection').html(errorHtml);
            }
        }
        if (response.success) {
            toastr.success("Order has been received", '', { positionClass: 'toast-top-center' });
            let target = $(mthis).data('target');
            $('#' + target).modal('hide');
            if (order.Id == 0) {
                orderGridAPI.applyTransaction({ add: response.data });
            }
            else {
                orderGridAPI.applyTransaction({ update: response.data });
            }
            //orderGridAPI.applyTransaction({ update: [response.data] });
            let rowNode = orderGridAPI.getRowNode(response.data[0].id);
            orderGridAPI.flashCells({ rowNodes: [rowNode] });
            $('#NewOrderBtn').click();
        }
        if (response.success == false) {
            var errorHtml = "";
            $.each(response.errors, function (index, element) {
                errorHtml += element[0].errorMessage + '<br/>';
            });
            $('#OrderErrorSection').html(errorHtml);
        }
    }
    static async CancelOrder(mthis) {
        $('#OrderErrorSection').empty();
        let orderId = $('#Id').val();
        if (!(orderId > 0)) {
            let target = $(mthis).data('target');
            $('#' + target).modal('hide');
            return true;
        }
        let isConfirm = confirm("Are you sure to cancel this Order?");
        if (!isConfirm) {
            let target = $(mthis).data('target');
            $('#' + target).modal('hide');
            return true;
        }
        var response = await fetch(baseUrl + 'api/order/cancel', {
            method: 'POST',
            body: JSON.stringify(orderId),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() });
        if (response.status > 399 && response.status < 500) {
            if (response != null) {
                var errorHtml = "";
                $.each(response.errors, function (index, element) {
                    errorHtml += element[0] + '<br/>';
                });
                $('#OrderErrorSection').html(errorHtml);
            }
        }
        if (response.success) {
            toastr.success("Order has been cancelled", '', { positionClass: 'toast-top-center' });
            let target = $(mthis).data('target');
            $('#' + target).modal('hide');

            orderGridAPI.applyTransaction({ update: [response.data] });
            let rowNode = orderGridAPI.getRowNode(response.data.id);
            orderGridAPI.flashCells({ rowNodes: [rowNode] });
        }
        if (response.success == false) {
            var errorHtml = "";
            $.each(response.errors, function (index, element) {
                errorHtml += element[0].errorMessage + '<br/>';
            });
            $('#OrderErrorSection').html(errorHtml);
        }
    }

    static async AddOrderTransaction(mthis) {
        let PartyId = $('#PartyId').val();
        let PaymentTypeId = $('#PaymentTypeId').val();
        let Amount = $('#Amount').val();
        let PaymentDate = $('#PaymentDate').val();

        if (PartyId == null || PartyId == "") {
            toastr.error('Payer field is required', '', {
                positionClass: 'toast-top-center'
            });
            return false;
        }
        if (PaymentTypeId == null || PaymentTypeId == "") {
            toastr.error('Payment Type field is required', '', {
                positionClass: 'toast-top-center'
            });
            return false;
        }
        if (Amount == null || Amount == "") {
            toastr.error('Amount field is required', '', {
                positionClass: 'toast-top-center'
            });
            return false;
        }
        if (Amount <= 0) {
            toastr.error('Amount must be larger than 0', '', {
                positionClass: 'toast-top-center'
            });
            return false;
        }

        let Party = $('#PartyId option:selected').text();
        let PaymentType = $('#PaymentTypeId option:selected').text();
        if (editModeIdx === -1) {
            orderTransaction.push(new OrderTransaction(0, PartyId, Party, PaymentTypeId, PaymentType, Amount, moment(PaymentDate, 'YYYY-MM-DD').format('DD/MM/YYYY')));
        } else {
            orderTransaction[editModeIdx].PartyId = PartyId;
            orderTransaction[editModeIdx].Party = Party;
            orderTransaction[editModeIdx].PaymentTypeId = PaymentTypeId;
            orderTransaction[editModeIdx].PaymentType = PaymentType;
            orderTransaction[editModeIdx].Amount = Amount;
            orderTransaction[editModeIdx].PaymentDate = moment(PaymentDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
            if (orderTransaction[editModeIdx].PaymentDate == "Invalid date") {
                orderTransaction[editModeIdx].PaymentDate = moment(PaymentDate, 'YYYY-MM-DDTHH-mm-ss').format('DD/MM/YYYY');
            }
            editModeIdx = -1;
        }
        Common.UpdateOrderTransactionGrid();
        Common.ClearSelectionOrderTransaction();
    }
    static CancelOrderTransaction(mthis) {
        editModeIdx = -1;
        Common.ClearSelectionOrderTransaction();
    }
    static async ClearSelectionOrderTransaction() {
        $('#PartyId').val(null).trigger('change');
        $('#PaymentTypeId').val(null).trigger('change');
        $('#Amount').val(null);
        $('#Amount').val(moment().format("YYYY-MM-DD"));
    }
    static async UpdateOrderTransactionGrid() {
        $('#OrderTransactionBody').empty();
        if (orderTransaction.length == 0) {
            $('#OrderTransactionBody').html("<tr><td colspan='4' class='text-center alert alert-info'>Transaction(s) will be apprear here.</td></tr>");
        }
        else {
            let NetAmount = parseFloat($('#NetAmount').val());
            let PaidAmount = parseFloat(0);
            $.each(orderTransaction, function (i, v) {
                let template = $('#OrderTransactionBodyTemplate').find('tbody').html();
                v.idx = i;
                $('#OrderTransactionBody').prepend(template.supplant(v));
                PaidAmount += parseFloat(v.Amount);
            });
            $('#OrderTransactionSummarySectionPaidAmount').html(PaidAmount);
            $('#OrderTransactionSummarySectionPendingAmount').html(NetAmount - PaidAmount);
        }
    }
    static async EditOrderTransaction(mthis) {
        let idx = $(mthis).parent().parent().attr('id');
        $('#PartyId').val(orderTransaction[idx].PartyId).trigger('change');
        $('#PaymentTypeId').val(orderTransaction[idx].PaymentTypeId).trigger('change');
        $('#Amount').val(orderTransaction[idx].Amount);
        $('#PaymentDate').val(moment(orderTransaction[idx].PaymentDate, 'DD/MM/YYYY').format('YYYY-MM-DD'));
        editModeIdx = idx;
    }
    static async DeleteOrderTransaction(mthis) {
        let idx = $(mthis).parent().parent().attr('id');
        orderTransaction.splice(idx, 1);
        Common.UpdateOrderTransactionGrid();
    }
    static CalculateDiscountAndNetPay() {
        let Quantity = $('#ReceivedQuantity').val() || $('#Quantity').val() || 0;
        let PricePerItem = $('#PricePerItem').val() || 0;
        let Discount = $('#Discount').val() || '0';
        let Tax = $('#Tax').val() || 0;
        let TotalAmount = (Quantity * PricePerItem);
        let DiscountAmount = Discount.toString().indexOf('%') == -1 ? Discount : (TotalAmount * parseFloat(Discount)) / 100;
        let NetTax = ((TotalAmount - DiscountAmount) * Tax) / 100;
        let NetAmount = (TotalAmount - DiscountAmount) + NetTax;
        $('#DiscountAmount').val(parseFloat(DiscountAmount).toFixed(2));
        $('#NetAmount').val(parseFloat(NetAmount).toFixed(2));
    }
    static CalculatePricePerItem(mthis) {
        let Quantity = $('#ReceivedQuantity').val() || $('#Quantity').val() || 0;
        let NetAmount = $('#NetAmount').val();
        NetAmount = parseFloat(NetAmount);
        let Tax = $('#Tax').val() || 0;
        let Discount = $('#Discount').val() || '0';
        let DiscountAmount = Discount.toString().indexOf('%') == -1 ? Discount : (NetAmount * parseFloat(Discount)) / 100;
        Quantity = Quantity == null ? 1 : Quantity;
        DiscountAmount = DiscountAmount == null ? 0 : DiscountAmount;
        Quantity = parseFloat(Quantity);
        DiscountAmount = parseFloat(DiscountAmount);
        let actualAmount = NetAmount / (1 + (Tax / 100));
        let PricePerItem = (actualAmount + DiscountAmount) / Quantity;
        $('#PricePerItem').val(parseFloat(PricePerItem).toFixed(2));
        //return PricePerItem;
    }
    static async InitCountable() {
        $(".countable").on("input", function () {
            if ($(this).attr('id') === 'BulkNameQuantity') {
                var bulkQuantity = $(this).val(); // Get the value of BulkNameQuantity
                if (bulkQuantity >= 0) {
                    bulkOrderQuantity = bulkOrderQuantity == null || bulkOrderQuantity == "" || bulkOrderQuantity == "-" ? 1 : bulkOrderQuantity;
                    $('#Quantity').val(bulkQuantity * bulkOrderQuantity);
                }
            }
            $('#ReceivedQuantity').val($('#Quantity').val());
            Common.CalculateDiscountAndNetPay();

            Common.UpdateOrderTransactionGrid();
        });
    }
    static async InitPricePerItemInput(mthis) {
        Common.CalculateDiscountAndNetPay();
    }
    static async InitNetAmountInput(mthis) {

        Common.CalculatePricePerItem();
    }
    static async SaveParty(mthis) {
        $('#PartyErrorSection').empty();
        let Name = $('#Name').val();
        let Type = $('#Type').val();
        let CategoryId = $('#CategoryId').val();
        let City = $('#City').val();
        let Country = $('#Country').val();
        let party = new Party(0, Name, Type, CategoryId, City, Country);

        var response = await fetch(baseUrl + 'api/party/save', {
            method: 'POST',
            body: JSON.stringify(party),
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
                $('#PartyErrorSection').html(errorHtml);
                return;
            }
        }
        if (response.success) {
            toastr.success("Party Saved", '', { positionClass: 'toast-top-center' });
            if (Type === 'Payer' || Type === 'Both') {
                $('#PartyId').prepend('<option value=' + response.data.id + '>' + response.data.name + '</option>');
            }
            if (Type === 'Seller' || Type === 'Both') {
                $('#SellerId').prepend('<option value=' + response.data.id + '>' + response.data.name + '</option>');
            }
            $('#AddParty').modal('hide');
            return;
        }
        if (response.success == false) {
            var errorHtml = "";
            $.each(response.errors, function (index, element) {
                errorHtml += element + '<br/>';
            });
            $('#PartyErrorSection').html(errorHtml);
        }
    }

    static async ExportToExcel() {
        orderGridAPI.exportDataAsExcel({ fileName: 'Orders_' + $('#ordersdaterange').val() + '.xlsx' });
    }

    static async SaveProduct(mthis) {
        $('#ProductErrorSection').empty();
        let Name = $('#ProductName').val();
        let GujaratiName = $('#GujaratiName').val();
        let Description = $('#Description').val();
        let Size = $('#Size').val();
        let UnitTypeCode = $('#UnitTypeCode').val();
        let OrderBulkName = $('#OrderBulkName');
        let OrderBulkQuantity = $('#OrderBulkQuantity');
        let ReorderLevel = $('#ReorderLevel').val();
        let IsDisposable = $('#IsDisposable').is(':checked');
        let Company = $('#Company').val();
        let StorageNames = $('#StorageNames option:selected').toArray().map(item => item.text).join();
        let product = new Product(0, Name, GujaratiName, Description, Size, UnitTypeCode, OrderBulkName, OrderBulkQuantity, ReorderLevel, IsDisposable, Company, StorageNames);

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
            $("#ProductId").select2('destroy').empty();
            Common.GetAllProducts();
            //if (Id == 0) {
            //    productGridOptions.api.applyTransaction({ add: [response.data] });//addIndex
            //}
            //else {
            //    productGridOptions.api.applyTransaction({ update: [response.data] });
            //}
            let rowNode = productGridOptions.api.getRowNode(response.data.id);
            productGridOptions.api.flashCells({ rowNodes: [rowNode] });
            $("#ProductUsageSelect").select2('destroy').empty();
            setTimeout(function () {
                Common.InitSelect2();
            }, 1000);
        }
    }

    static async DeleteAction(mthis) {
        // get current row id
        let id = $(mthis).data('id');
        //get confirmation
        let isConfirm = confirm("Are you sure to delete this Order?");
        if (!isConfirm) {
            return true;
        }
        //call api
        var response = await fetch(baseUrl + 'api/order/delete/' + id, {
            method: 'POST',
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
                toastr.warning(errorHtml, '', { positionClass: 'toast-top-center' });
            }
        }
        if (response.success) {
            toastr.success("Order Deleted", '', { positionClass: 'toast-top-center' });
            //Get Row node using id
            let rowNode = orderGridAPI.getRowNode(id);
            //remove row from grid
            orderGridAPI.applyTransaction({ remove: [rowNode.data] });
        }
        if (response.success == false) {
            var errorHtml = "";
            $.each(response.errors, function (index, element) {
                errorHtml += element[0].errorMessage + '<br/>';
            });
            toastr.error(errorHtml, '', { positionClass: 'toast-top-center' });
        }

    }
}

jQuery(document).ready(function () {
    Common.init();
});