var viewModelPolicyMaker;
$(document).ready(function () {
  GetInstituionTypeDDList("institutionTypeHashId");
  GetCountryDDList("countryMasterHashId");
  GetDesignationDDList("designationHashId");
  ResetForm();
  loadKendoGrid();
});

var loadKendoGrid = function () {
  $("#gridPolicyMakers").kendoGrid({
    dataSource: {
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_POLICYMAKERS_GETALL_List,
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
            PolicyMakerHashId: { type: "string" },
            CountryMasterHashId: { type: "string" },
            InstitutionTypeHashId: { type: "string" },
            CountryName: { type: "string" },
            InstitutionType: { type: "string" },
            PolicyMakerName: { type: "string" },
            DesignationName: { type: "string" },
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
          field: "InstitutionType",
          title: institutionType,
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
          field: "PolicyMakerName",
          title: policyMakerName,
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
          template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditPolicyMakers(\"#=PolicyMakerHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
          width: 50
        },
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeletePolicyMakers(\"#=PolicyMakerHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
          width: 50
        }

    ]
  });
}

function EditPolicyMakers(itemObject) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  ResetForm();
  ////var row = $(itemObject).closest("tr");
  ////var uid = $(row).data("uid");
  ////var grid = $("#gridPolicyMakers").data("kendoGrid");
  ////var currentDataRow = grid.dataItem(row);


  GetPolicyMakerBasedOnHash(itemObject);
  $('#collapseExample').collapse('show');
  $(".form-group").removeClass('has-error');
  ResetvalidateForm('policyMakerForm');
  moveDivTop("topPanel");
}

function DeletePolicyMakers(itemObject) {
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
      ////var grid = $("#gridPolicyMakers").data("kendoGrid");
      ////var currentDataRow = grid.dataItem(row);

      var PolicyMakerHashId = itemObject; ////currentDataRow.PolicyMakerHashId

      $.ajax({
        url: BCMConfig.API_POLICYMAKERS_DELETE + "?policyMakerHashId=" + PolicyMakerHashId,
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
}

var ResetForm = function () {
  initViewModel();
  ValidateForm('policyMakerForm', '');
  ResetvalidateForm('policyMakerForm');
  kendo.bind($("#policyMakerForm"), viewModelPolicyMaker);
}

var initViewModel = function () {
  viewModelPolicyMaker = kendo.observable({
    InstitutionMasterHashId: "",
    InstitutionTypeHashId: "",
    CountryMasterHashId: "",
    DesignationHashId: "",
    PolicyFirstName: "",
    PolicyLastName:"",
    PolicyMakerHashId: ""
  });
};

$("#btnPolicyMakersCancel").click(function () {
  ResetForm();
  $('#collapseExample').collapse('hide');
});

$("#clearFilters").click(function () {
  $("#gridPolicyMakers").data("kendoGrid").dataSource.filter([]);
});

$("#btnPolicyMakersSave").click(function () {
  if ($("#policyMakerForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('policyMakerForm');
     
    $.ajax({
      url: BCMConfig.API_POLICYMAKERS_UPDATE,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelPolicyMaker),
      processdata: true,
      success: function (response) { //call successfull
        if (response.errorModel != null && response.errorModel != "") {
          ValidateForm('policyMakerForm', response.errorModel);
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

function GetPolicyMakerBasedOnHash(policyMakerHashId) {
  $.ajax({
    url: BCMConfig.API_GETPOLICYMAKER_BASED_ON_HASH + "?policyMakerHashId=" + policyMakerHashId,
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
  
  viewModelPolicyMaker.CountryMasterHashId = response.countryMasterHashId;
  viewModelPolicyMaker.InstitutionTypeHashId = response.institutionTypeHashId;
  viewModelPolicyMaker.InstitutionsName = response.institutionsName;

  viewModelPolicyMaker.PolicyMakerHashId = response.policyMakerHashId;
  viewModelPolicyMaker.DesignationHashId = response.designationHashId;
  viewModelPolicyMaker.PolicyFirstName = response.policyFirstName;
  viewModelPolicyMaker.PolicyLastName = response.policyLastName;

  kendo.bind($("#policyMakerForm"), viewModelPolicyMaker);

  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}
