var viewModelStateHead;
$(document).ready(function () {
  GetCountryDDList();
  GetDesignationDDList("designationHashId");
  ResetForm();
  loadKendoGrid();
});

var loadKendoGrid = function () {
  $("#gridStateHead").kendoGrid({
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
          url: BCMConfig.API_STATEHEADGETALLDATA_URL,
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
            StateHeadMasterHashId: { type: "string" },
            CountryName: { type: "string" },
            DesignationName: { type: "string" },
            StateHeadName: { type: "string" },
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
    dataBound: masterGridDataBound,
    columns: [
        {
          field: "CountryName",
          title: countryName,
          width: 120,
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
          field: "DesignationName",
          title: designationType,
          width: 120,
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
          field: "StateHeadName",
          title: stateHeadName,
          width: 120,
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
          template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditStateHead(\"#=StateHeadMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
          width: 50
        },
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteStateHead(\"#=StateHeadMasterHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
          width: 50
        }

    ]
  });
}


$("#btnSave").click(function () {
  if ($("#stateHeadForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('stateHeadForm');

    $.ajax({
      url: BCMConfig.API_STATEHEAD_UPDATE,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelStateHead),
      processdata: true,
      success: function (response) { //call successfull
        if (response.errorModel != null && response.errorModel != "") {
          ValidateForm('stateHeadForm', response.errorModel);
        }
        if (response.data) {
          toastr.info(response.errorMessage);
          ResetForm();
          loadKendoGrid();
          $('#collapseExample').collapse('hide');
        }
        else {
          if (response.errorMessage != null && response.errorMessage != "")
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

$("#btnCancel").click(function () {
  ResetForm();
  $('#collapseExample').collapse('hide');
});

$("#clearFilters").click(function () {
  $("#gridStateHead").data("kendoGrid").dataSource.filter([]);
});

function EditStateHead(StateHeadHashId) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  ResetForm();

  //var row = $(itemObject).closest("tr");
  //var uid = $(row).data("uid");
  //var grid = $("#gridStateHead").data("kendoGrid");
  //var currentDataRow = grid.dataItem(row);

  
  GetStateHeadBasedOnHash(StateHeadHashId);
  $('#countryMasterHashId').attr('disabled', true);
  $('#collapseExample').collapse('show');

  $(".form-group").removeClass('has-error');
  ResetvalidateForm('stateHeadForm');
  moveDivTop("topPanel");
}

function DeleteStateHead(StateHeadHashId) {
  bcmStrategyAlerts({
    type: "confirm",
    messageText: deletePopupMessage,
    headerText: deletePopupHeader,
    alertType: alertTypes.Info
  }).done(function (e) {
    if (e) {
      CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
      //var row = $(itemObject).closest("tr");
      //var uid = $(row).data("uid");
      //var grid = $("#gridStateHead").data("kendoGrid");
      //var currentDataRow = grid.dataItem(row);

      var StateHeadMasterHashId = StateHeadHashId; ////currentDataRow.StateHeadMasterHashId

      $.ajax({
        url: BCMConfig.API_STATEHEAD_DELETE + "?stateHeadMasterHashId=" + StateHeadMasterHashId,
        beforeSend: CommonJS.BeforeSendAjaxCall,
        method: 'GET',
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        processdata: true,
        success: function (response) { //call successfull
          if (response.data) {
            toastr.info(response.errorMessage);
            ResetForm();
            loadKendoGrid();
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

function masterGridDataBound(e) {
  var grid = e.sender;
  if (grid.dataSource.total() == 0) {
    var colCount = grid.columns.length;
    $(e.sender.wrapper)
        .find('tbody').first()
        .append('<tr class="kendo-data-row" style="text-align: center;"><td colspan="' + colCount + '" class="no-data">' + noDataFound + '</td></tr>');
  }
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
};

var ResetForm = function () {
  initViewModel();
  $('#countryMasterHashId').attr('disabled', false);
  ValidateForm('stateHeadForm', '');
  ResetvalidateForm('stateHeadForm');
  kendo.bind($("#stateHeadForm"), viewModelStateHead);

}

var initViewModel = function () {
  viewModelStateHead = kendo.observable({
    StateHeadMasterHashId: "",
    CountryMasterHashId: "",
    DesignationHashId: "",
    FirstName: "",
    LastName: ""
  });
};

function GetStateHeadBasedOnHash(StateHeadHashId) {

  $.ajax({
    url: BCMConfig.API_GETSTATE_HEAD_BASED_ON_HASH + "?stateHeadHashId=" + StateHeadHashId,
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

  viewModelStateHead.StateHeadMasterHashId = response.stateHeadMasterHashId
  viewModelStateHead.CountryMasterHashId = response.countryMasterHashId
  viewModelStateHead.DesignationHashId = response.designationHashId;
  viewModelStateHead.FirstName = response.firstName;
  viewModelStateHead.LastName = response.lastName;
  kendo.bind($("#stateHeadForm"), viewModelStateHead);
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}