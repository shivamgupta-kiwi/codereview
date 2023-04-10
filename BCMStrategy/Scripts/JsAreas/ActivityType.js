var viewModelActivityType;



$(document).ready(function () {
  GetMetadataTypeDDList("metadataTypeMasterHashId");
  ResetForm();
  loadKendoGrid();
});

var loadKendoGrid = function () {
  $("#gridActivityType").kendoGrid({
    dataSource: {
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_GETALLLIST_ACTIVITYTYPE_URL,
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
            ActivityTypeMasterHashId: { type: "string" },
            MetadataTypeMasterHashId: { type: "string" },
            WebsiteType: { type: "string" },
            ActivityName: { type: "string" },
            ActivityValue: { type: "string" },
            RelatedActvityTypeDisplay: { type: "string" }
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
          width: 90,
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
        title: metadataType,
        width: 90,
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
        field: "ActivityName",
        title: activityType,
        width: 90,
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
           field: "ActivityValue",
           title: activityValue,
           width: 90,
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
           field: "RelatedActvityTypeDisplay",
           title: relatedActvityType,
           width: 100,
           filterable: false,
           sortable: false
				 },
				 {
				 	field: "ColorCode",
				 	title: colorCode,
				 	width: 60,
				 	template: "<div style='width:50px;height:25px;background:\#=ColorCode #\'></div>",
				 	filterable: false,
				 	sortable: false
				 },
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditActivityType(\"#=ActivityTypeMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
          width: 40
        },
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteActivityType(\"#=ActivityTypeMasterHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
          width: 40
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

$("#btnActivityTypeSave").click(function () {
  if ($("#activityTypeForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('activityTypeForm');

    viewModelActivityType.RelatedActvityType = $("#relatedActvityType").val();

    $.ajax({
      url: BCMConfig.API_ACTIVITYTYPE_UPDATE,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelActivityType),
      processdata: true,
      success: function (response) {
        if (response.errorModel != null && response.errorModel != "") {
          ValidateForm('activityTypeForm', response.errorModel);
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

$("#btnActivityTypeCancel").click(function () {
  ResetForm();
  $('#collapseExample').collapse('hide');
});
$("#clearFilters").click(function () {
  $("#gridActivityType").data("kendoGrid").dataSource.filter([]);
});

function EditActivityType(ActivityTypeHashId) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  ResetForm();
  //var row = $(itemObject).closest("tr");
  //var uid = $(row).data("uid");
  //var grid = $("#gridActivityType").data("kendoGrid");
  //var currentDataRow = grid.dataItem(row);


  GetActivityTypeBasedOnHash(ActivityTypeHashId);
  $('#collapseExample').collapse('show');
  $(".form-group").removeClass('has-error');
  ResetvalidateForm('activityTypeForm');
  moveDivTop("topPanel");
}

var ResetForm = function () {
  initViewModel();
  ValidateForm('activityTypeForm', '');
  ResetvalidateForm('activityTypeForm');
  kendo.bind($("#activityTypeForm"), viewModelActivityType);
  $("#relatedActvityType").tagsinput('removeAll');
}

var initViewModel = function () {
  viewModelActivityType = kendo.observable({
    ActivityTypeMasterHashId: "",
    MetadataTypeMasterHashId: "",
    ActivityName: "",
    ActivityValue: "",
    RelatedActvityType: "",
		ColorCode: "#ffffff"
  });
};

function DeleteActivityType(ActivityTypeHashId) {
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
      ////var grid = $("#gridActivityType").data("kendoGrid");
      ////var currentDataRow = grid.dataItem(row);

      var ActivityTypeMasterHashId = ActivityTypeHashId; ////currentDataRow.ActivityTypeMasterHashId

      $.ajax({
        url: BCMConfig.API_ACTIVITYTYPE_DELETE + "?ActivityTypeMasterHashId=" + ActivityTypeMasterHashId,
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

function GetActivityTypeBasedOnHash(ActivityTypeHashId) {

  $.ajax({
    url: BCMConfig.API_GETACTIVITY_TYPE_BASED_ON_HASH + "?activityTypeHashId=" + ActivityTypeHashId,
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
  viewModelActivityType.ActivityTypeMasterHashId = response.activityTypeMasterHashId;
  viewModelActivityType.MetadataTypeMasterHashId = response.metadataTypeMasterHashId
  viewModelActivityType.ActivityName = response.activityName
  viewModelActivityType.ActivityValue = response.activityValue;
  viewModelActivityType.ColorCode = response.colorCode;
  $.each(response.relatedActvityTypeList, function (i) {
    $("#relatedActvityType").tagsinput('add', response.relatedActvityTypeList[i]);
  });


  kendo.bind($("#activityTypeForm"), viewModelActivityType);
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}