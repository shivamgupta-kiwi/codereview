var viewModelMetadataTypes;

$(document).ready(function () {
  GetWebsiteTypeDDList("websiteTypeHashId");
  ResetForm();
  loadKendoGrid();
  $("#DivValue").hide();
});

var loadKendoGrid = function () {
  $("#gridMetadataTypes").kendoGrid({
    dataSource: {
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_GETALLLIST_METADATATYPES_URL,
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
            MetadataTypesMasterHashId: { type: "string" },
            WebsiteTypeHashId: { type: "string" },
            WebsiteType: { type: "string" },
            MetaData: { type: "string" },
            MetaDataValue: { type: "string" },
            IsActivityTypeExist: { type: "string" },
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
          field: "WebsiteType",
          title: websiteType,
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
          field: "MetaData",
          title: metadata,
          width: 120,
          filterable: {
            extra: false,
            operators: {
              string: {
                contains: "Contains"
              }
            }
          }
        }
        ,
        {
          field: "MetaDataValue",
          title: metadataValue,
          width: 80,
          filterable: {
            extra: false,
            operators: {
              string: {
                contains: "Contains"
              }
            }
          }
        }
        ,
        {
          field: "Status",
          title: ActivityTypeExist,
          width: 160,
          filterable: {
            extra: false,
            operators: {
              string: {
                contains: "Contains"
              }
            }
          }
        }
        ,
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditMetadataTypes(\"#=MetadataTypesMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
          width: 50
        },
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteMetadataTypes(\"#=MetadataTypesMasterHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
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

$("#btnMetadataTypesSave").click(function () {
  if ($("#metadataTypesForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('metadataTypesForm');


    if ($("#yes")[0].checked) {
      viewModelMetadataTypes.IsActivityTypeExist = "True";
      viewModelMetadataTypes.MetaDataValue = "";
    }
    if ($("#no")[0].checked) {
      viewModelMetadataTypes.IsActivityTypeExist = "False";
    }
    $.ajax({
      url: BCMConfig.API_METADATATYPES_UPDATE,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelMetadataTypes),
      processdata: true,
      success: function (response) {  
        if (response.errorModel != null && response.errorModel != "") {
          ValidateForm('metadataTypesForm', response.errorModel);
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

$("#btnMetadataTypesCancel").click(function () {
  ResetForm();
  $('#collapseExample').collapse('hide');
});
$("#clearFilters").click(function () {
  $("#gridMetadataTypes").data("kendoGrid").dataSource.filter([]);
});

function EditMetadataTypes(MetadataTypeHashId) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  ResetForm();

  //var row = $(itemObject).closest("tr");
  //var uid = $(row).data("uid");
  //var grid = $("#gridMetadataTypes").data("kendoGrid");
  //var currentDataRow = grid.dataItem(row);

  
  GetMetadataTypeBasedOnHash(MetadataTypeHashId);
  $('#collapseExample').collapse('show');
  $(".form-group").removeClass('has-error');
  ResetvalidateForm('metadataTypesForm');
  moveDivTop("topPanel");
}

var ResetForm = function () {
  initViewModel();
  ValidateForm('metadataTypesForm', '');
  ResetvalidateForm('metadataTypesForm');
  kendo.bind($("#metadataTypesForm"), viewModelMetadataTypes);
}

var initViewModel = function () {
  viewModelMetadataTypes = kendo.observable({
    MetadataTypesMasterHashId: "",
    WebsiteTypeHashId: "",
    MetaData: "",
    MetaDataValue: "",
    IsActivityTypeExist: ""
  });

};

function DeleteMetadataTypes(MetadataTypeHashId) {
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
      ////var grid = $("#gridMetadataTypes").data("kendoGrid");
      ////var currentDataRow = grid.dataItem(row);

      var MetadataTypesMasterHashId = MetadataTypeHashId; ////currentDataRow.MetadataTypesMasterHashId

      $.ajax({
        url: BCMConfig.API_METADATATYPES_DELETE + "?metadataTypesMasterHashId=" + MetadataTypesMasterHashId,
        beforeSend: CommonJS.BeforeSendAjaxCall,
        method: 'GET',
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        processdata: true,
        success: function (response) {  
          if (response.data) {
            toastr.info(response.errorMessage);
            ResetForm();
            loadKendoGrid();
          }
          else {
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
}

$('input[name=isExist]').change(function () {

  if ($("#yes")[0].checked) {
    $("#DivValue").hide();
    $("#metaDataValue").val("");
  }
  if ($("#no")[0].checked) {
    $("#DivValue").show();
  }

});

function  GetMetadataTypeBasedOnHash(MetadataTypeHashId) {

  $.ajax({
    url: BCMConfig.API_GETMETADATA_TYPE_BASED_ON_HASH + "?metadataTypeHashId=" + MetadataTypeHashId,
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

  viewModelMetadataTypes.MetadataTypesMasterHashId = response.metadataTypesMasterHashId;
  viewModelMetadataTypes.MetadataTypesMasterHashId = response.metadataTypesMasterHashId
  viewModelMetadataTypes.WebsiteTypeHashId = response.websiteTypeHashId
  viewModelMetadataTypes.MetaData = response.metaData;
  viewModelMetadataTypes.MetaDataValue = response.metaDataValue;
  viewModelMetadataTypes.IsActivityTypeExist = response.isActivityTypeExist;

  if (response.isActivityTypeExist) {
    $("#yes").prop("checked", true);
    $("#DivValue").hide();
    viewModelMetadataTypes.MetaDataValue = "";
  }
  else {
    $("#no").prop("checked", true);
    $("#DivValue").show();
  }
  kendo.bind($("#metadataTypesForm"), viewModelMetadataTypes);
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}
