var viewModelInstitutionTypes;
$(document).ready(function () {
  ResetInstitutionTypeForm();
  loadInstitutionTypeKendoGrid();
});
$("#btnInstTypesSave").click(function () {

    if ($("#institutionTypesForm").valid()) {
        CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
        ResetvalidateForm('institutionTypesForm');

        $.ajax({
            url: BCMConfig.API_INSTTYPES_UPDATE,
            beforeSend: CommonJS.BeforeSendAjaxCall,
            method: 'POST',
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(viewModelInstitutionTypes),
            processdata: true,
            success: function (response) { //call successfull
                if (response.errorModel != null && response.errorModel != "") {
                    ValidateForm('institutionTypesForm', response.errorModel);
                }
                if (response.data) {
                    toastr.info(response.errorMessage);
                    ResetInstitutionTypeForm();
                    loadInstitutionTypeKendoGrid();
                    $('#collapseExample').collapse('hide');
                }
                else {
                    if (response.errorMessage != null && response.errorMessage != "")
                        toastr.error(response.errorMessage);
                }

                CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
            },
            error: function (e) {
              CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
              CommonJS.HandleErrorStatus(e.status);
                //error occurred
            }
        });
    }
});
$("#btnInstTypesCancel").click(function () {
  ResetInstitutionTypeForm();
    $('#collapseExample').collapse('hide');
});
$("#clearFilters").click(function () {
  $("#gridInstTypes").data("kendoGrid").dataSource.filter([]);
});
var loadInstitutionTypeKendoGrid = function () {
    
    $("#gridInstTypes").kendoGrid({
        //toolbar: ["excel"],
        //excel: {
        //  fileName: "data.xlsx",
        //  filterable: true
        //},
        dataSource: {
            //requestStart: CommonJS.SetPageLoader(PageLoaderActivity.SHOW),
            type: "json",
            transport: {
                read: {
                    url: BCMConfig.API_INSTTYPESGETALLDATA_URL,
                    beforeSend: CommonJS.BeforeSendAjaxCall,
                },
                parameterMap: function (options) {
                    return "parametersJson=" + JSON.stringify(options);
                }
            },
            error: function (e) {
                HandleError(e);
            },
            schema: {
                data: 'Data',
                total: 'Total',
                model: {
                    fields: {
                        InstitutionTypesHashId: { type: "string" },
                        InstitutionTypesName: { type: "string" },
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        },
        filterable: true,
        sortable: true,
        height: 450,
        pageable: {
          messages: {
            itemsPerPage: "Items",
            display: "{0} - {1} of {2}",
          },
            pageSize: 10,
            pageSizes: [10, 20, 50],
            refresh: true,
            buttonCount: 5,
            input: true
        },
        dataBound: masterInstitutionTypeGridDataBound,
        columns: [
            {
                field: "InstitutionTypesName",
                title: institutionTypesName,
                width: 160,
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            contains: "Contains"
                        }
                    }
                }
            },
            {
                title: "",
                template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditInstitutionTypes(\"#=InstitutionTypesHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
                width: 30
            },
            {
                title: "",
                template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteInstitutionTypes(\"#=InstitutionTypesHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
                width: 30
            }

        ]
    });

}
function EditInstitutionTypes(InstitutionTypeHashId) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  ResetInstitutionTypeForm();
    ////var row = $(itemObject).closest("tr");
    ////var uid = $(row).data("uid");
    ////var grid = $("#gridInstTypes").data("kendoGrid");
    ////var currentDataRow = grid.dataItem(row);
    
    GetInstitutionTypeBasedOnHash(InstitutionTypeHashId);
    $('#collapseExample').collapse('show');
    $(".form-group").removeClass('has-error');
    ResetvalidateForm('institutionTypesForm');
    moveDivTop("topPanel");
}
function DeleteInstitutionTypes(InstitutionTypeHashId) {
    bcmStrategyAlerts({
        type: "confirm",
        messageText: deletePopupMessage,
        headerText: deletePopupHeader,
        alertType: alertTypes.Info
    }).done(function (e) {
        if (e) {
            CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
            ////var row = $(itemObject).closest("tr");
            ////var uid = $(row).data("uid");
            ////var grid = $("#gridInstTypes").data("kendoGrid");
            ////var currentDataRow = grid.dataItem(row);

            var InstitutionTypesHashId = InstitutionTypeHashId; ////currentDataRow.InstitutionTypesHashId

            $.ajax({
                url: BCMConfig.API_INSTTYPES_DELETE + "?institutionTypesHashId=" + InstitutionTypesHashId,
                beforeSend: CommonJS.BeforeSendAjaxCall,
                method: 'GET',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                processdata: true,
                success: function (response) { //call successfull
                  if (response.data) {
                    toastr.info(response.errorMessage);
                    ResetInstitutionTypeForm();
                    loadInstitutionTypeKendoGrid();
                  }
                  else {
                    toastr.error(response.errorMessage);
                  }
                    CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
                },
                error: function (response) {
                    CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
                    //error occurred
                }
            });
        }
    });
}
function masterInstitutionTypeGridDataBound(e) {
    var grid = e.sender;
    if (grid.dataSource.total() == 0) {
        var colCount = grid.columns.length;
        $(e.sender.wrapper)
            .find('tbody').first()
            .append('<tr class="kendo-data-row" style="text-align: center;"><td colspan="' + colCount + '" class="no-data">' + noDataFound + '</td></tr>');
    }
    CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
};
var ResetInstitutionTypeForm = function () {
    initViewModel();
    ValidateForm('institutionTypesForm', '');
    ResetvalidateForm('institutionTypesForm');
    kendo.bind($("#institutionTypesForm"), viewModelInstitutionTypes);

}
var initViewModel = function () {
    viewModelInstitutionTypes = kendo.observable({
        InstitutionTypesHashId: "",
        InstitutionTypesName: ""
    });
};

function GetInstitutionTypeBasedOnHash(InstitutionTypeHashId) {

  $.ajax({
    url: BCMConfig.API_GETINSTITUTION_TYPE_BASED_ON_HASH + "?institutionTypeHashId=" + InstitutionTypeHashId,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      BindDataOnEdit(response);
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
    }
  });
}

function BindDataOnEdit(response) {
  viewModelInstitutionTypes.InstitutionTypesHashId = response.institutionTypesHashId
  viewModelInstitutionTypes.InstitutionTypesName = response.institutionTypesName;
  kendo.bind($("#institutionTypesForm"), viewModelInstitutionTypes);

  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}