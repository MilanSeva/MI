﻿let partyGridAPI;
function ActionCellRenderer() { }

ActionCellRenderer.prototype.init = function (params) {
    this.params = params;

    this.eGui = document.createElement('span');
    this.eGui.innerHTML = '<a href="#" class="link-primary" onclick="Common.OpenModal(this)" data-id="' + params.data.id + '" data-target="AddEditProduct" title="Edit"><i class="bi bi-pencil-square fs-6"></i></a>';
}

ActionCellRenderer.prototype.getGui = function () {
    return this.eGui;
}

var partyGridOptions = {

    // define grid columns
    columnDefs: [
        {
            headerName: 'Name', field: 'name', filter: 'agTextColumnFilter', headerTooltip: 'Name'
        },
        {
            headerName: 'Payer Type', field: 'type', filter: 'agTextColumnFilter', headerTooltip: 'Payer Type'
        },
        {
            headerName: 'Category', field: 'category', filter: 'agTextColumnFilter', headerTooltip: 'Category'
        },
        {
            headerName: 'City', field: 'city', filter: 'agTextColumnFilter', headerTooltip: 'City'
        },
        {
            headerName: 'Country', field: 'country', filter: 'agTextColumnFilter', headerTooltip: 'Country'
        },
        {
            headerName: '', field: 'id', headerTooltip: 'Action', pinned: 'right', width: 80, suppressSizeToFit: true,
            cellRenderer: 'actionCellRenderer',
        }
    ],
    //sideBar: { toolPanels: ['columns', 'filters'] },
    rowClassRules: {
        'sick-days-warning': function (params) {
            return params.data.currentStock < params.data.reorderLevel;
        },
    },
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
    pagination: true,
    paginationAutoPageSize: true,
    animateRows: true,
    defaultColGroupDef: {
        marryChildren: true
    },
    getRowId: params => {
        return params.data.id;
    },
    suppressContextMenu: true,
    components: {
        actionCellRenderer: ActionCellRenderer
    },
    onGridReady: function (params) {
        partyGridAPI.sizeColumnsToFit();
        //const allColumnIds = [];
        //partyGridOptions.columnApi.getAllColumns().forEach((column) => {
        //    if (column.colId != 'id')
        //        allColumnIds.push(column.colId);
        //});
        //partyGridOptions.columnApi.autoSizeColumns(allColumnIds, false);
    },
    overlayLoadingTemplate:
        '<span class="ag-overlay-loading-center">Please wait while your perties are loading</span>',
    overlayNoRowsTemplate:
        `<div class="text-center">
                <h5 class="text-center"><b>Parties will be appear here.</b></h5>
            </div>`
};


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
            $('#ModalTitle').html('Add Party');
            Common.BindValuesToPartyForm(new Party(0, null, "Both", null, null, null, null, null, null, null, null, null, null));
        }
        else {
            $('#ModalTitle').html('Edit Party');
            Common.GetPartyById(id);
        }
    }

    static ApplyAGGrid() {

        var gridDiv = document.querySelector('#partiesdata');
        partyGridAPI = new agGrid.createGrid(gridDiv, partyGridOptions);
        partyGridAPI.setGridOption("theme", agGrid.themeQuartz
            .withParams(
                {
                    backgroundColor: "#1e2838",
                    foregroundColor: "#FFFFFFCC",
                    browserColorScheme: "dark",
                },
                "dark-red",
            ));
        document.body.dataset.agThemeMode = "dark-red";
        fetch(baseUrl + 'api/parties')
            .then((response) => response.json())
            .then(data => {
                partyGridAPI.setGridOption("rowData", data);
                Common.InitSelect2();
            })
            .catch(error => {
                partyGridAPI.setGridOption("rowData", []);
                //toastr.error(error, '', {
                //    positionClass: 'toast-top-center'
                //});
            });

    }

    static BindValuesToPartyForm(model) {
        $('#PartyErrorSection').empty();
        $('#Id').val(model.Id);
        $('#Name').val(model.Name);
        $('#Type').val(model.Type);
        $('#CategoryId').val(model.CategoryId);
        $('#City').val(model.City);
        $('#Country').val(model.Country);
    }

    static init() {
        $('#partiesdata').height(Common.calcDataTableHeight(27));
    }

    static async SaveParty(mthis) {
        $('#PartyErrorSection').empty();
        let Id = $('#Id').val();
        let Name = $('#Name').val();
        let Type = $('#Type').val();
        let CategoryId = $('#CategoryId').val();
        let City = $('#City').val();
        let Country = $('#Country').val();
        let party = new Party(Id, Name, Type, CategoryId, City, Country);

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
            let target = $(mthis).data('target');
            $('#' + target).modal('hide');
            if (Id == 0) {
                partyGridAPI.applyTransaction({ add: [response.data] });//addIndex
            }
            else {
                partyGridAPI.applyTransaction({ update: [response.data] });
            }
            let rowNode = partyGridAPI.getRowNode(response.data.id);
            partyGridAPI.flashCells({ rowNodes: [rowNode] });
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
    static async GetPartyById(id) {
        await fetch(baseUrl + 'api/party/byid/' + id, {
            method: 'GET',
            headers: {
                //'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() })
            .then(data => {
                $('#AddEditParty').modal('show');
                Common.BindValuesToPartyForm(new Party(data.id, data.name, data.type, data.categoryId, data.city, data.country));
            })
            .catch(error => {
                console.log(error);
                toastr.success("Unexpected error", '', { positionClass: 'toast-top-center' });
            });
    }

    static BindSelectData() {
        var result = 'India,Afghanistan,Aland Islands,Albania,Algeria,American Samoa,Andorra,Angola,Anguilla,Antarctica,Antigua and Barbuda,Argentina,Armenia,Aruba,Australia,Austria,Azerbaijan,Bahamas,Bahrain,Bangladesh,Barbados,Belarus,Belgium,Belize,Benin,Bermuda,Bhutan,Bolivia,Bosnia and Herzegovina,Botswana,Bouvet Island,Brazil,British Indian Ocean Territory,British Virgin Islands,Brunei,Bulgaria,Burkina Faso,Burundi,Cambodia,Cameroon,Canada,Cape Verde,Caribbean Netherlands,Cayman Islands,Central African Republic,Chad,Chile,China,Christmas Island,Cocos (Keeling) Islands,Colombia,Comoros,Cook Islands,Costa Rica,Croatia,Cuba,Curaçao,Cyprus,Czechia,Denmark,Djibouti,Dominica,Dominican Republic,DR Congo,Ecuador,Egypt,El Salvador,Equatorial Guinea,Eritrea,Estonia,Eswatini,Ethiopia,Falkland Islands,Faroe Islands,Fiji,Finland,France,French Guiana,French Polynesia,French Southern and Antarctic Lands,Gabon,Gambia,Georgia,Germany,Ghana,Gibraltar,Greece,Greenland,Grenada,Guadeloupe,Guam,Guatemala,Guernsey,Guinea,Guinea-Bissau,Guyana,Haiti,Heard Island and McDonald Islands,Honduras,Hong Kong,Hungary,Iceland,Indonesia,Iran,Iraq,Ireland,Isle of Man,Israel,Italy,Ivory Coast,Jamaica,Japan,Jersey,Jordan,Kazakhstan,Kenya,Kiribati,Kosovo,Kuwait,Kyrgyzstan,Laos,Latvia,Lebanon,Lesotho,Liberia,Libya,Liechtenstein,Lithuania,Luxembourg,Macau,Madagascar,Malawi,Malaysia,Maldives,Mali,Malta,Marshall Islands,Martinique,Mauritania,Mauritius,Mayotte,Mexico,Micronesia,Moldova,Monaco,Mongolia,Montenegro,Montserrat,Morocco,Mozambique,Myanmar,Namibia,Nauru,Nepal,Netherlands,New Caledonia,New Zealand,Nicaragua,Niger,Nigeria,Niue,Norfolk Island,North Korea,North Macedonia,Northern Mariana Islands,Norway,Oman,Pakistan,Palau,Palestine,Panama,Papua New Guinea,Paraguay,Peru,Philippines,Pitcairn Islands,Poland,Portugal,Puerto Rico,Qatar,Republic of the Congo,Réunion,Romania,Russia,Rwanda,Saint Barthélemy,Saint Helena, Ascension and Tristan da Cunha,Saint Kitts and Nevis,Saint Lucia,Saint Martin,Saint Pierre and Miquelon,Saint Vincent and the Grenadines,Samoa,San Marino,São Tomé and Príncipe,Saudi Arabia,Senegal,Serbia,Seychelles,Sierra Leone,Singapore,Sint Maarten,Slovakia,Slovenia,Solomon Islands,Somalia,South Africa,South Georgia,South Korea,South Sudan,Spain,Sri Lanka,Sudan,Suriname,Svalbard and Jan Mayen,Sweden,Switzerland,Syria,Taiwan,Tajikistan,Tanzania,Thailand,Timor-Leste,Togo,Tokelau,Tonga,Trinidad and Tobago,Tunisia,Turkey,Turkmenistan,Turks and Caicos Islands,Tuvalu,Uganda,Ukraine,United Arab Emirates,United Kingdom,United Citys,United Citys Minor Outlying Islands,United Citys Virgin Islands,Uruguay,Uzbekistan,Vanuatu,Vatican City,Venezuela,Vietnam,Wallis and Futuna,Western Sahara,Yemen,Zambia,Zimbabwe';
        return result.split(',');
    }
    static async InitSelect2() {
        $('#Country').select2({
            placeholder: 'Search Country',
            data: Common.BindSelectData(),
            theme: "bootstrap4",
            dropdownParent: $("#AddEditParty")
        });
    }


}

jQuery(document).ready(function () {
    Common.init();
    Common.ApplyAGGrid();
});