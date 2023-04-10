var viewModelInternationalOrganization;
$(document).ready(function () {
  GetDesignationDDList("designationOrganizationHashId");
  ResetForm();
  loadKendoGrid();
});

var loadKendoGrid = function () {
  $("#gridInternationalOrganization").kendoGrid({
    dataSource: {
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_GETALLLIST_INTERNATION_ORGANIZATION_URL,
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
            InternaltionalOrganizationMasterHashId: { type: "string" },
            DesignationMasterHashId: { type: "string" },
            DesignationName: { type: "string" },
            OrganizationName: { type: "string" },
            Leader: { type: "string" },
            EntityName: { type: "string" }
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
          field: "OrganizationName",
          title: organizationName,
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
          title: designationName,
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
          field: "LeaderName",
          title: leader,
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
          field: "EntityName",
          title: entityName,
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
              field: "Status",
              title: MultiLateral,
              width: 100,
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
          template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditInternationOrganization(\"#=InternaltionalOrganizationMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
          width: 50
        },
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteInternationalOrganzation(\"#=InternaltionalOrganizationMasterHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
          width: 50
        }

    ]
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

$("#btnInternationalOrgSave").click(function () {
  if ($("#internationalOrganizationForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('internationalOrganizationForm');

    if ($("#yes")[0].checked) {
      viewModelInternationalOrganization.IsMultiLateral = "True";
    }
    if ($("#no")[0].checked) {
      viewModelInternationalOrganization.IsMultiLateral = "False";
    }

    $.ajax({
      url: BCMConfig.API_INTERNATIONALORGANIZATION_UPDATE,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelInternationalOrganization),
      processdata: true,
      success: function (response) { //call successfull
        if (response.errorModel != null && response.errorModel != "") {
          ValidateForm('internationalOrganizationForm', response.errorModel);
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
      error: function (e) {
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        CommonJS.HandleErrorStatus(e.status);
      }
    });
  }
});

$("#btnInternationalOrgCancel").click(function () {
  ResetForm();
  $('#collapseExample').collapse('hide');
});

$("#clearInternationalOrgFilters").click(function () {
  $("#gridInternationalOrganization").data("kendoGrid").dataSource.filter([]);
});

function EditInternationOrganization(InternationalOrgHashId) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  ResetForm();
  //var row = $(itemObject).closest("tr");
  //var uid = $(row).data("uid");
  //var grid = $("#gridInternationalOrganization").data("kendoGrid");
  //var currentDataRow = grid.dataItem(row);
  
  GetInternationalOrgBasedOnHash(InternationalOrgHashId);
  $('#collapseExample').collapse('show');
  $(".form-group").removeClass('has-error');
  ResetvalidateForm('internationalOrganizationForm');
  moveDivTop("topPanel");
}

var ResetForm = function () {
  initViewModel();
  ValidateForm('internationalOrganizationForm', '');
  ResetvalidateForm('internationalOrganizationForm');
  kendo.bind($("#internationalOrganizationForm"), viewModelInternationalOrganization);
}

var initViewModel = function () {
  viewModelInternationalOrganization = kendo.observable({
    InternaltionalOrganizationMasterHashId: "",
    OrganizationName: "",
    DesignationMasterHashId: "",
    DesignationName: "",
    Leader: "",
    EntityName: "",
    IsMultiLateral: ""
  });
};

function DeleteInternationalOrganzation(InternationalOrgHashId) {
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
      ////var grid = $("#gridInternationalOrganization").data("kendoGrid");
      ////var currentDataRow = grid.dataItem(row);

      var InternaltionalOrganizationMasterHashId = InternationalOrgHashId; ////currentDataRow.InternaltionalOrganizationMasterHashId

      $.ajax({
        url: BCMConfig.API_INTERNATIONAL_ORG_DELETE + "?internationalOrgMasterHashId=" + InternaltionalOrganizationMasterHashId,
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
        error: function (e) {
          CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
          CommonJS.HandleErrorStatus(e.status);
        }
      });
    }
  });
}

function GetInternationalOrgBasedOnHash(InternationalOrgHashId) {

  $.ajax({
    url: BCMConfig.API_GETINTERNATIONAL_ORG_BASED_ON_HASH + "?internationalOrgHashId=" + InternationalOrgHashId,
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

  viewModelInternationalOrganization.InternaltionalOrganizationMasterHashId = response.internaltionalOrganizationMasterHashId
  viewModelInternationalOrganization.OrganizationName = response.organizationName
  viewModelInternationalOrganization.DesignationMasterHashId = response.designationMasterHashId;
  viewModelInternationalOrganization.DesignationName = response.designationName;
  viewModelInternationalOrganization.LeaderFirstName = response.leaderFirstName;
  viewModelInternationalOrganization.LeaderLastName = response.leaderLastName;
  viewModelInternationalOrganization.EntityName = response.entityName;

  if (response.isMultiLateral) {
    $("#yes").prop("checked", true);
  }
  else {
    $("#no").prop("checked", true);
  }
  kendo.bind($("#internationalOrganizationForm"), viewModelInternationalOrganization);
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}