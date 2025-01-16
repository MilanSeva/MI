let storageGridAPI;
function ActionCellRenderer() { }

ActionCellRenderer.prototype.init = function (params) {
    this.params = params;

    this.eGui = document.createElement('span');
    this.eGui.innerHTML = '<button class="btn btn-sm btn-link" type="button" onclick="Common.OpenModal(this)" data-id="' + params.data.id + '" data-target="AddEditStorage">Edit</button>';
}

ActionCellRenderer.prototype.getGui = function () {
    return this.eGui;
}

var storageGridOptions = {

    // define grid columns
    columnDefs: [
        {
            headerName: 'Name', field: 'name', filter: 'agTextColumnFilter', headerTooltip: 'Name'
        },
        {
            headerName: 'Status', field: 'status', filter: 'agTextColumnFilter', headerTooltip: 'Status'
        },
        {
            headerName: '', field: 'id', headerTooltip: 'Action', width: 80, suppressSizeToFit: true,
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
        storageGridAPI.sizeColumnsToFit();
    },
    overlayLoadingTemplate:
        '<span class="ag-overlay-loading-center">Please wait while storage(s) are loading</span>',
    overlayNoRowsTemplate:
        `<div class="text-center">
                <h5 class="text-center"><b>Storage(s) will appear here.</b></h5>
            </div>`
};


class Storage {
    constructor(Id, Name) {
        this.Id = parseInt(Id);
        this.Name = Common.ParseValue(Name);
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
            Common.BindValuesToStorageForm(new Storage(0, null));
        }
        else {
            Common.GetStorageById(id);
        }
    }

    static ApplyAGGrid() {

        var gridDiv = document.querySelector('#storagedata');
        storageGridAPI = new agGrid.createGrid(gridDiv, storageGridOptions);
        storageGridAPI.setGridOption("theme", agGrid.themeQuartz
            .withParams(
                {
                    backgroundColor: "#1e2838",
                    foregroundColor: "#FFFFFFCC",
                    browserColorScheme: "dark",
                },
                "dark-red",
            ));
        document.body.dataset.agThemeMode = "dark-red";
        fetch(baseUrl + 'api/storages')
            .then((response) => response.json())
            .then(data => {                
                storageGridAPI.setGridOption("rowData", data);
                //Common.InitSelect2();
            })
            .catch(error => {
                storageGridAPI.setGridOption("rowData", []);
                //toastr.error(error, '', {
                //    positionClass: 'toast-top-center'
                //});
            });

    }

    static BindValuesToStorageForm(model) {
        $('#StorageErrorSection').empty();
        $('#Id').val(model.Id);
        $('#Name').val(model.Name);
    }

    static init() {
        $('#storagedata').height(Common.calcDataTableHeight(27));
    }

    static async SaveStorage(mthis) {
        $('#StorageErrorSection').empty();
        let Id = $('#Id').val();
        let Name = $('#Name').val();
        let storage = new Storage(Id, Name);

        var response = await fetch(baseUrl + 'api/storage/save', {
            method: 'POST',
            body: JSON.stringify(storage),
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
                $('#StorageErrorSection').html(errorHtml);
                return;
            }
        }
        if (response.success) {
            toastr.success("Storage Saved", '', { positionClass: 'toast-top-center' });
            let target = $(mthis).data('target');
            $('#' + target).modal('hide');
            if (Id == 0) {
                storageGridAPI.applyTransaction({ add: [response.data] });//addIndex
            }
            else {
                storageGridAPI.applyTransaction({ update: [response.data] });
            }
            let rowNode = storageGridAPI.getRowNode(response.data.id);
            storageGridAPI.flashCells({ rowNodes: [rowNode] });
            return;
        }
        if (response.success == false) {
            var errorHtml = "";
            $.each(response.errors, function (index, element) {
                errorHtml += element + '<br/>';
            });
            $('#StorageErrorSection').html(errorHtml);
        }
    }

    static async GetStorageById(id) {
        await fetch(baseUrl + 'api/storage/byid/' + id, {
            method: 'GET',
            headers: {
                //'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() })
            .then(data => {
                $('#AddEditStorage').modal('show');
                Common.BindValuesToStorageForm(new Storage(data.id, data.name));
            })
            .catch(error => {
                console.log(error);
                toastr.success("Unexpected error", '', { positionClass: 'toast-top-center' });
            });
    }

}

jQuery(document).ready(function () {
    Common.init();
    Common.ApplyAGGrid();
});