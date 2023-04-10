var viewModelScheduler;

$(document).ready(function () {
  GetSchedulerFrequencyDDList("frequencyTypeMasterHashId");
  GetSchedulerWebsiteTypeDDList("websiteType");
  ResetForm();
  BindDateTimeTextbox();
  loadKendoGrid();

});

var loadKendoGrid = function () {
  $("#gridScheduler").kendoGrid({
    dataSource: {
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_GETALLLIST_SCHEDULER_URL,
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
            SchedulerMasterHashId: { type: "string" },
            FrequencyTypeMasterHashId: { type: "string" },
            Status: { type: "string" },
            Name: { type: "string" },
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
        field: "Name",
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
        field: "FrequencyType",
        title: frequencyType,
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
         field: "Details",
         title: "Details",
         width: 140,
         filterable: false,
         sortable: false

       }
       ,
       {
         field: "Description",
         title: "Description",
         width: 140,
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
           title: enabled,
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
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditScheduler(\"#=SchedulerMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
          width: 50
        }
        ,
            {
              title: "",
              template: "<a href='javascript:' data-toggle='tooltip' title='View Log' onclick='GoToProcessDetail(\"#=Name #\")'><i class='fa fa-eye' aria-hidden='true'></i></a>",
              width: 50
            }
        ////,
        ////{
        ////  title: "",
        ////  template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteScheduler(this)'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
        ////  width: 50
        ////}

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

function GoToProcessDetail(webSiteType) {
  //var row = $(itemObject).closest("tr");
  //var uid = $(row).data("uid");
  //var grid = $("#gridScheduler").data("kendoGrid");
  //var currentDataRow = grid.dataItem(row);
  //var schedulerMasterId = webSiteType;
  window.location.href = $('#ProcessDetailUrl')[0].href + "?webSiteType=" + webSiteType;
}

$("#btnSchedulerSave").click(function () {
  if ($("#schedulerForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('schedulerForm');

    viewModelScheduler.StartTime = $("#startTimeFinal").val();
    viewModelScheduler.StartTimeFinal = $("#startTimeFinal").val();
    
    viewModelScheduler.StartDateFinal = $("#startDateFinal").val();
    viewModelScheduler.StartDate = $("#startDateFinal").val();
    
    viewModelScheduler.EndDateFinal = $("#endDateFinal").val();
    viewModelScheduler.EndDate = $("#endDateFinal").val();
    
    if ($("#yes")[0].checked) {
      viewModelScheduler.IsEnabled = "True";
    }
    if ($("#no")[0].checked) {
      viewModelScheduler.IsEnabled = "False";
    }
    viewModelScheduler.WeekdaysCheckbox = "";
    if ($("#sunday").is(':checked')) {
      viewModelScheduler.Sunday = "True";
      viewModelScheduler.WeekdaysCheckbox = "checked";
    }
    else {
      viewModelScheduler.Sunday = "False";
      viewModelScheduler.WeekdaysCheckbox = viewModelScheduler.WeekdaysCheckbox == "" ? "" : "checked";
    }
    if ($("#monday").is(':checked')) {
      viewModelScheduler.Monday = "True";
      viewModelScheduler.WeekdaysCheckbox = "checked";
    }
    else {
      viewModelScheduler.Monday = "False";
      viewModelScheduler.WeekdaysCheckbox = viewModelScheduler.WeekdaysCheckbox == "" ? "" : "checked";
    }
    if ($("#tuesday").is(':checked')) {
      viewModelScheduler.Tuesday = "True";
      viewModelScheduler.WeekdaysCheckbox = "checked";
    }
    else {
      viewModelScheduler.Tuesday = "False";
      viewModelScheduler.WeekdaysCheckbox = viewModelScheduler.WeekdaysCheckbox == "" ? "" : "checked";
    }
    if ($("#wednesday").is(':checked')) {
      viewModelScheduler.Wednesday = "True";
      viewModelScheduler.WeekdaysCheckbox = "checked";
    }
    else {
      viewModelScheduler.Wednesday = "False";
      viewModelScheduler.WeekdaysCheckbox = viewModelScheduler.WeekdaysCheckbox == "" ? "" : "checked";
    }
    if ($("#thursday").is(':checked')) {
      viewModelScheduler.Thursday = "True";
      viewModelScheduler.WeekdaysCheckbox = "checked";
    }
    else {
      viewModelScheduler.Thursday = "False";
      viewModelScheduler.WeekdaysCheckbox = viewModelScheduler.WeekdaysCheckbox == "" ? "" : "checked";
    }
    if ($("#friday").is(':checked')) {
      viewModelScheduler.Friday = "True";
      viewModelScheduler.WeekdaysCheckbox = "checked";
    }
    else {
      viewModelScheduler.Friday = "False";
      viewModelScheduler.WeekdaysCheckbox = viewModelScheduler.WeekdaysCheckbox == "" ? "" : "checked";
    }
    if ($("#saturday").is(':checked')) {
      viewModelScheduler.Saturday = "True";
      viewModelScheduler.WeekdaysCheckbox = "checked";
    }
    else {
      viewModelScheduler.Saturday = "False";
      viewModelScheduler.WeekdaysCheckbox = viewModelScheduler.WeekdaysCheckbox == "" ? "" : "checked";
    }

    $.ajax({
      url: BCMConfig.API_SCHEDULER_UPDATE,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelScheduler),
      processdata: true,
      success: function (response) {
        if (response.errorModel != null && response.errorModel != "") {
          ValidateForm('schedulerForm', response.errorModel);
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

$("#btnSchedulerCancel").click(function () {
  ResetForm();
  $('#collapseExample').collapse('hide');
});
$("#clearFilters").click(function () {
  $("#gridScheduler").data("kendoGrid").dataSource.filter([]);
});

function EditScheduler(SchedulerHashId) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  ResetForm();

  //var row = $(itemObject).closest("tr");
  //var uid = $(row).data("uid");
  //var grid = $("#gridScheduler").data("kendoGrid");
  //var currentDataRow = grid.dataItem(row);



  GetSchedulerBasedOnHash(SchedulerHashId);
  $('#collapseExample').collapse('show');
  $(".form-group").removeClass('has-error');
  ResetvalidateForm('schedulerForm');
  moveDivTop("topPanel");
}

var ResetForm = function () {
  initViewModel();
  ValidateForm('schedulerForm', '');
  ResetvalidateForm('schedulerForm');
  kendo.bind($("#schedulerForm"), viewModelScheduler);
  DivHide();
  ClearCheckbox();
}
function DivHide() {
   $("#DivWeekDays").hide();
}
function BindDateTimeTextbox() {
  $("#startTimeFinal").kendoTimePicker({
    dateInput: true
  });

  $("#startDateFinal").kendoDatePicker({
    //// display month and year in the input
    format: "MM/dd/yyyy",
    //// specifies that DateInput is used for masking the input element
    dateInput: true
  });
  $("#endDateFinal").kendoDatePicker({
    //// display month and year in the input
    format: "MM/dd/yyyy",
    //// specifies that DateInput is used for masking the input element
    dateInput: true
  });
}
var initViewModel = function () {
  viewModelScheduler = kendo.observable({
    SchedulerMasterHashId: "",
    FrequencyTypeMasterHashId: "",
    WebsiteType: "",
    Description: "",
    StartDateFinal: "",
    StartDate:"",
    EndDate: "",
    EndDateFinal:"",
    StartTime: "",
    StartTimeFinal:"",
    RepeatEveryHour : "",
    IsEnabled: "",
    Sunday: "",
    Monday: "",
    Tuesday: "",
    Wednesday: "",
    Thursday: "",
    Friday: "",
    Saturday: "",
    WeekdaysCheckbox: ""
  });
};

function DeleteScheduler(itemObject) {
  bcmStrategyAlerts({
    type: "confirm",
    messageText: deletePopupMessage,
    headerText: deletePopupHeader,
    alertType: alertTypes.Info
  }).done(function (e) {
    if (e) {
      CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
      var row = $(itemObject).closest("tr");
      var uid = $(row).data("uid");
      var grid = $("#gridScheduler").data("kendoGrid");
      var currentDataRow = grid.dataItem(row);

      var SchedulerMasterHashId = currentDataRow.SchedulerMasterHashId

      $.ajax({
        url: BCMConfig.API_SCHEDULER_DELETE + "?schedulerMasterHashId=" + SchedulerMasterHashId,
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



$('#frequencyTypeMasterHashId').on('change', function () {
  var frequency = this.options[this.selectedIndex].text;
  if (frequency == "Daily") {
    DivHideShowForDaily();
    ClearDailyWeeklySection();
  } else if (frequency == "Weekly") {
    DivHideShowForWeekly();
    ClearDailyWeeklySection();
  }
  else {
    DivHide();
  }
});
function ClearDailyWeeklySection() {
  ClearCheckbox();
}
function DivHideShowForDaily() {
  $("#DivstartTime").show();
   $("#DivWeekDays").hide();
}
function DivHideShowForWeekly() {
  $("#DivstartTime").show();
   $("#DivWeekDays").show();
}
function ClearCheckbox() {
  $("#sunday").prop('checked', false);
  $("#monday").prop('checked', false);
  $("#tuesday").prop('checked', false);
  $("#wednesday").prop('checked', false);
  $("#thursday").prop('checked', false);
  $("#friday").prop('checked', false);
  $("#saturday").prop('checked', false);
}


function GetSchedulerBasedOnHash(SchedulerHashId) {

  $.ajax({
    url: BCMConfig.API_GETSCHEDULER_BASED_ON_HASH + "?schedulerHashId=" + SchedulerHashId,
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

  viewModelScheduler.SchedulerMasterHashId = response.schedulerMasterHashId;
  viewModelScheduler.FrequencyTypeMasterHashId = response.frequencyTypeMasterHashId
  viewModelScheduler.WebsiteType = response.websiteType
  viewModelScheduler.StartDateFinal = response.startDate;
  viewModelScheduler.EndDateFinal = response.endDate;
  viewModelScheduler.StartTimeFinal = response.startTime;
  viewModelScheduler.Description = response.description;

  if (response.isEnabled) {
    $("#yes").prop("checked", true);
  }
  else {
    $("#no").prop("checked", true);
  }
  viewModelScheduler.RepeatEveryHour = response.recurEvery;
  if (response.frequencyTypeMasterId == 2) {
    DivHideShowForDaily();
  } else if (response.frequencyTypeMasterId == 3) {
    DivHideShowForWeekly();
  }

  if (response.sunday) {
    $("#sunday").prop('checked', true);
  }
  else {
    $("#sunday").prop('checked', false);
  }
  if (response.monday) {
    $("#monday").prop('checked', true);
  }
  else {
    $("#monday").prop('checked', false);
  }
  if (response.tuesday) {
    $("#tuesday").prop('checked', true);
  }
  else {
    $("#tuesday").prop('checked', false);
  }
  if (response.wednesday) {
    $("#wednesday").prop('checked', true);
  }
  else {
    $("#wednesday").prop('checked', false);
  }
  if (response.thursday) {
    $("#thursday").prop('checked', true);
  }
  else {
    $("#thursday").prop('checked', false);
  }
  if (response.friday) {
    $("#friday").prop('checked', true);
  }
  else {
    $("#friday").prop('checked', false);
  }
  if (response.saturday) {
    $("#saturday").prop('checked', true);
  }
  else {
    $("#saturday").prop('checked', false);
  }
  kendo.bind($("#schedulerForm"), viewModelScheduler);

  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}
