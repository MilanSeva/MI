let repoGridAPI;
var headerComponentParams = {
    template:
        '<div class="ag-cell-label-container" role="presentation">' +
        '  <span ref="eMenu" class="ag-header-icon ag-header-cell-menu-button"></span>' +
        '  <div ref="eLabel" class="ag-header-cell-label" role="presentation">' +
        '    <span ref="eSortOrder" class="ag-header-icon ag-sort-order"></span>' +
        '    <span ref="eSortAsc" class="ag-header-icon ag-sort-ascending-icon"></span>' +
        '    <span ref="eSortDesc" class="ag-header-icon ag-sort-descending-icon"></span>' +
        '    <span ref="eSortNone" class="ag-header-icon ag-sort-none-icon"></span>' +
        '    <span ref="eText" class="ag-header-cell-text" role="columnheader" style="white-space: pre-wrap !important;"></span>' +
        '    <span ref="eFilter" class="ag-header-icon ag-filter-icon"></span>' +
        "  </div>" +
        "</div>",
};
const columnDefs = [];

var repoGridOptions = {
    defaultColDef: {
        sortable: true,
        filter: true,
    },
    columnDefs: columnDefs,
    defaultColDef: {
        editable: false,
        sortable: true,
        resizable: true,
        flex: 1,
        minWidth: 50,
        //headerComponentParams: headerComponentParams,
    },
    onGridReady: function (params) {
        //repoGridOptions.api.sizeColumnsToFit();
        const allColumnIds = [];
        repoGridAPI.getAllGridColumns().forEach((column) => {
            allColumnIds.push(column.getColId());
        });
        repoGridAPI.autoSizeColumns(allColumnIds, false);
    },
    pagination: true,
    paginationAutoPageSize: true,
    // paginationPageSize: 10,
    animateRows: true,
};
class Report {
    static #calcDataTableHeight(height) {
        var decreaseHeight = ($(window).innerHeight() * height) / 657;
        return $(window).innerHeight() - decreaseHeight;
    }
    static async ApplyAGGrid() {


        $("#resultGrid").empty();


        let res = await this.#getQueryResult();
        let data = JSON.parse(res);

        var gridDiv = document.querySelector("#resultGrid");

        repoGridAPI = new agGrid.createGrid(gridDiv, repoGridOptions);
        repoGridAPI.setGridOption("theme", agGrid.themeQuartz
            .withParams(
                {
                    backgroundColor: "#1e2838",
                    foregroundColor: "#FFFFFFCC",
                    browserColorScheme: "dark",
                },
                "dark-red",
            ));
        document.body.dataset.agThemeMode = "dark-red";
        this.#dynamicallyConfigureColumnsFromObject(data[0], repoGridOptions);
        //repoGridOptions.api.setRowData(data);
        repoGridAPI.setGridOption("rowData", data);

        $("#resultGrid").height(this.#calcDataTableHeight(250));
    }
    static #dynamicallyConfigureColumnsFromObject(anObject, repoGridOptions) {
        const colDefs = repoGridAPI.getColumnDefs();
        colDefs.length = 0;
        const keys = Object.keys(anObject);
        keys.forEach((key) =>
            colDefs.push({ field: key, headerName: key })
        );
        console.log('def:', colDefs);
        repoGridAPI.setGridOption('columnDefs', colDefs);
        console.log("def completed");
    }
    static DownloadExcel() {
        var params = {
            sheetName: 'Result',
            fileName: 'Result.xlsx',
            //columnKeys: cols
        }
        repoGridAPI.exportDataAsExcel(params);
    }
    static async #getQueryResult() {
        var query = $("#query").val();
        return await fetch(
            baseUrl + "report/fa9f3be62c748689d5b041dadf4ccf9?query=", {
            method: "POST",
            body: JSON.stringify(query),
            headers: {
                Accept: "application/json",
                "Content-Type": "application/json",
            },
        }
        ).then((response) => {
            return response.json();
        });
    }
}
jQuery(document).ready(async function () {
    //$("#query").focusTextToEnd();
    $("#query").keydown(function (e) {
        if (e.ctrlKey && e.keyCode == 13) {
            $("#run").click();
        }
    });
});
