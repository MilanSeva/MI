
let down = false;
let notificationIds = [];
let notificationGridAPI;

var notificationGridAPIOptions = {

    // define grid columns
    columnDefs: [
        {
            headerName: 'title', field: 'title', filter: 'agTextColumnFilter', headerTooltip: 'Title'
        },
        {
            headerName: 'Message', field: 'message', filter: 'agTextColumnFilter', headerTooltip: 'Message'
        },
        {
            headerName: 'Quantity', field: 'quantity', filter: 'agNumberColumnFilter', headerTooltip: 'Quantity'
        },
        {
            headerName: 'Status', field: 'status', filter: 'agTextColumnFilter', headerTooltip: 'Status'
        },
        //{
        //    headerName: '', field: 'id', headerTooltip: 'Action', pinned: 'right', suppressSizeToFit: true,
        //    cellRenderer: 'actionCellRenderer',
        //}
    ],
    defaultColDef: {
        editable: false,
        sortable: true,
        resizable: true,
        flex: 1,
        minWidth: 50,
        wrapText: true,
        autoHeight: true,
        floatingFilter: false,
    },
    animateRows: true,
    pagination: true,
    paginationPageSize: 200,
    getRowId: params => {
        return params.data.id;
    },
    onGridReady: function (params) {
        notificationGridAPI.sizeColumnsToFit();
    },
    overlayLoadingTemplate:
        '<span class="ag-overlay-loading-center">Please wait while your notifications are loading</span>',
    overlayNoRowsTemplate:
        `<div class="text-center">
                <h5 class="text-center"><b>Notification(s) will appear here.</b></h5>
            </div>`
};

class MyNotification {

    static NotificationMessageTemplate() {
        return ' <div class="notifications-item"> <div class="notification-text"> <h4>{title}</h4> <p>{message}</p> </div> <div class="notification-action"> <button class="btn btn-light btn-sm" onclick="MyNotification.MarkAsRead(this)" data-id="{id}">√</button> </div> </div> ';
    }

    static async GetPendingORNotifiedNotifications() {

        fetch(baseUrl + 'api/notification/pendingornotified')
            .then((response) => response.json())
            .then(data => {
                notificationGridAPI.setGridOption("rowData", data.myNotifications);
                if (data.myNotifications.length != 0) {
                    $.each(data.myNotifications, function (i, v) {
                        if (v.status === 'Pending') {
                            notificationIds.push(v.id);
                        }
                    });
                }
                //$('#NotificationMessages').empty();
                if (data.pendingNotificationCount > 0) {
                    $('#NotificationPendingCount').show();
                    $('#NotificationPendingCount').html(data.pendingNotificationCount);
                    MyNotification.MarkAsNotified();
                }

                //if (data.myNotifications.length == 0) {
                //    $('#NotificationMessages').html('<h2>Notifications</h2><h6>Notifications will be appear here</h6>');
                //}
                //else {
                //    $('#NotificationMessages').html('<h2>Notifications</h2>');
                //    let template = MyNotification.NotificationMessageTemplate();

            })
            .catch(error => {
                console.log(error);
            });
    }
    static async MarkAsNotified() {
        fetch(baseUrl + 'api/notification/notified', {
            method: 'POST',
            body: JSON.stringify(notificationIds),
            headers: {
                //'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        })
            .then((response) => response.json())
            .then(data => {

            })
            .catch(error => {
                console.log(error);
            });

    }
    static async MarkAsRead(mthis) {
        let id = $(mthis).data('id');
        fetch(baseUrl + 'api/notification/read', {
            method: 'POST',
            body: JSON.stringify(id),
            headers: {
                //'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        })
            .then((response) => response.json())
            .then(data => {
                $(mthis).closets('.notifications-item').remove();
            })
            .catch(error => {
                console.log(error);
            });
    }

    static async ViewNotificationModal(mthis) {
        $('#NotificationModal').modal('show');
        //if (down) {
        //    $('#NotificationMessages').css('height', '0px');
        //    $('#NotificationMessages').css('opacity', '0');
        //    down = false;
        //} else {
        //    $('#NotificationMessages').css('height', 'auto');
        //    $('#NotificationMessages').css('opacity', '1');
        //    down = true;
        //}
    }

    static init() {
        MyNotification.ApplyAGGrid();
        MyNotification.GetPendingORNotifiedNotifications();
    }

    static ApplyAGGrid() {
        var gridDiv = document.querySelector('#notificationgrid');
        //var state = localStorage.getItem("r447774316684a1d9074esdscddc7a02s23");
        //if (state) {
        //    notificationGridAPIOptions.initialState = JSON.parse(state);
        //}
        notificationGridAPI = new agGrid.createGrid(gridDiv, notificationGridAPIOptions);
        notificationGridAPI.setGridOption("theme", agGrid.themeQuartz
            .withParams(
                {
                    backgroundColor: "#1e2838",
                    foregroundColor: "#FFFFFFCC",
                    browserColorScheme: "dark",
                },
                "dark-red",
            ));
        document.body.dataset.agThemeMode = "dark-red";
        notificationGridAPI.setGridOption("rowData", []);
    }
}

jQuery(document).ready(function () {
    MyNotification.init();
});