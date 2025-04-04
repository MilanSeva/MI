﻿let usersAPI;
function ActionCellRenderer() { }

ActionCellRenderer.prototype.init = function (params) {

    this.params = params;

    // Create the parent container for the buttons
    this.eGui = document.createElement('span');

    //// Create the "Edit" button
    //let editBtn = document.createElement('button');
    //editBtn.className = 'btn btn-sm btn-link';
    //editBtn.type = 'button';
    //editBtn.textContent = 'Edit';
    //editBtn.dataset.id = params.data.id; // Add data-id attribute
    //editBtn.dataset.target = 'AddEditProduct';
    //editBtn.onclick = function () {
    //    Common.OpenModal(editBtn); // Call the modal opening function
    //};

    // Create the "Reset MFA" button
    let resetMfaBtn = document.createElement('button');
    resetMfaBtn.className = 'btn btn-sm btn-link resetmfa';
    resetMfaBtn.type = 'button';
    resetMfaBtn.textContent = 'Reset MFA';

    // Bind the "Reset MFA" button click event
    resetMfaBtn.addEventListener('click', () => {
        let isConfirm = confirm("Are  you sure for reset MFA?");
        if (isConfirm) {
            this.btnClickedHandler(params.data);
        }
    });

    // Append the buttons to the parent container
    //this.eGui.appendChild(editBtn);
    //this.eGui.appendChild(document.createTextNode(' ')); // Add space between buttons
    this.eGui.appendChild(resetMfaBtn);
}
// Define the handler for Reset MFA
ActionCellRenderer.prototype.btnClickedHandler = function (data) {
    fetch(baseUrl + 'api/user/resetmfa/' + data.id, {
        method: 'POST',
    })
        .then((response) => response.json())
        .then(data => {
            toastr.success("MFA reset successfully", '', {
                positionClass: 'toast-top-center'
            });
            Common.LoadDataInGrid();
        })
        .catch(error => {
            console.log('err:', error);
            toastr.error("Unexpected error", '', {
                positionClass: 'toast-top-center'
            });
        });
};
ActionCellRenderer.prototype.getGui = function () {
    return this.eGui;
}
//function onCellClickedEvent(params) {
//    $('#ProductUsageSelect').val(params.data.id);
//    $('#ProductUsageSelect').trigger('change');
//    //$('#UsageQuantity').focus();
//}
var productUsageGridOptions = {

    // define grid columns
    columnDefs: [
        {
            headerName: 'User Name', field: 'userName', filter: 'agTextColumnFilter', headerTooltip: 'User Name'
        },
        {
            headerName: 'Email', field: 'email', filter: 'agTextColumnFilter', headerTooltip: 'Email'
        },
        {
            headerName: 'Status', field: 'status', filter: 'agTextColumnFilter', headerTooltip: 'Status'
        },
        {
            headerName: 'Role', field: 'role', filter: 'agTextColumnFilter', headerTooltip: 'Role'
        },
        {
            headerName: 'Is MFA Enabled?', field: 'isMfaEnabled', filter: 'agTextColumnFilter', headerTooltip: 'Is MFA Enabled?'
        },
        {
            headerName: '', headerTooltip: 'Action', pinned: 'right', suppressSizeToFit: true,
            cellRenderer: 'actionCellRenderer',
        }
    ],
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
    onGridReady: function (params) {
        usersAPI.sizeColumnsToFit();
        //const allColumnIds = [];
        //productUsageGridOptions.columnApi.getAllColumns().forEach((column) => {
        //    if (column.colId != 'id')
        //        allColumnIds.push(column.colId);
        //});
        //productUsageGridOptions.columnApi.autoSizeColumns(allColumnIds, false);
    },
    overlayLoadingTemplate:
        '<span class="ag-overlay-loading-center">Please wait while your data are loading</span>',
    overlayNoRowsTemplate:
        `<div class="text-center">
                <h5 class="text-center"><b>Data will be appear here.</b></h5>
            </div>`
};
class AddUserDto {
    constructor(UserName, Email, Role, Password, ConfirmPassword) {
        this.UserName = UserName;
        this.Email = Email;
        this.Role = Role;
        this.Password = Password;
        this.ConfirmPassword = ConfirmPassword;
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

    static BindValuesToUserForm(user) {
        $('#UserName').val(user.UserName);
        $('#Email').val(user.Email);
        $('#Role').val(user.Role);
        $('#Password').val(user.Password);
        $('#ConfirmPassword').val(user.ConfirmPassword);
    }
    static OpenModal(mthis) {
        let target = $(mthis).data('target');
        $('#' + target).modal('show');
        Common.BindValuesToUserForm(new AddUserDto(null, null, null, null));
    }

    static ApplyAGGrid() {

        var gridDiv = document.querySelector('#usergrid');
        usersAPI = new agGrid.createGrid(gridDiv, productUsageGridOptions);
        usersAPI.setGridOption("theme", agGrid.themeQuartz
            .withParams(
                {
                    backgroundColor: "#1e2838",
                    foregroundColor: "#FFFFFFCC",
                    browserColorScheme: "dark",
                },
                "dark-red",
            ));
        document.body.dataset.agThemeMode = "dark-red";

        Common.LoadDataInGrid();
    }
    static LoadDataInGrid() {
        fetch(baseUrl + 'api/user/all')
            .then((response) => response.json())
            .then(data => {
                usersAPI.setGridOption("rowData", data);
            })
            .catch(error => {
                console.log('err:', error);
                usersAPI.setGridOption("rowData", []);
                //toastr.error(error, '', {
                //    positionClass: 'toast-top-center'
                //});
            });
    }

    static async SaveUser(mthis) {
        $('#AddUserErrorSection').empty();
        let UserName = $('#UserName').val();
        let Email = $('#Email').val();
        let Role = $('#Role option:selected').val();
        let Password = $('#Password').val();
        let ConfirmPassword = $('#ConfirmPassword').val();
        let user = new AddUserDto(UserName, Email, Role, Password, ConfirmPassword);

        var response = await fetch(baseUrl + 'api/user/save', {
            method: 'POST',
            body: JSON.stringify(user),
            headers: {
                //'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() });

        console.log(response);
        if (response.status > 399 && response.status < 500) {
            if (response != null) {
                var errorHtml = "";
                $.each(response.errors, function (index, element) {
                    errorHtml += element[0] + '<br/>';
                });
                $('#AddUserErrorSection').html(errorHtml);
                return;
            }
        }
        if (response.success) {
            toastr.success("User Saved", '', { positionClass: 'toast-top-center' });
            let target = $(mthis).data('target');
            $('#' + target).modal('hide');
            Common.LoadDataInGrid();
            return;
        }
        if (response.success == false) {
            var errorHtml = "";
            $.each(response.errors, function (index, element) {
                errorHtml += element + '<br/>';
            });
            $('#AddUserErrorSection').html(errorHtml);
        }
    }

    static init() {
        $('#usergrid').height(Common.calcDataTableHeight(27));
    }

}

jQuery(document).ready(function () {
    Common.init();
    Common.ApplyAGGrid();
});