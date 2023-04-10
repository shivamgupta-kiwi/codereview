var viewModelMetadataNounlplusVerb;

$(document).ready(function () {
  GetMetadataTypeDDList("metadataTypeMasterHashId");
  GetMetadataDynamicNounplusVerbDDList("metadataDynamicNounplusVerb");
  ResetForm();
  loadKendoGrid();
  $("#DivDynamicNoun").hide();
});

var loadKendoGrid = function () {
  $("#gridMetadataNounplusVerb").kendoGrid({
    dataSource: {
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_GETALLLIST_METADATANOUNPLUSVERB_URL,
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
            MetadataNounplusVerbMasterHashId: { type: "string" },
            MetadataTypeMasterHashId: { type: "string" },
            MetaData: { type: "string" },
            Noun: { type: "string" },
            Verb: { type: "string" },
            Status: { type: "string" },
            ActivityTypeMasterHashId: { type: "string" },
            ActivityType: { type: "string" },
            ActivityValue: { type: "string" }
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
        width: 170,
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
        width: 170,
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
          field: "ActivityType",
          title: activityType,
          width: 170,
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
          field: "Noun",
          title: metadataNoun,
          width: 130,
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
          field: "Verb",
          title: metadataVerb,
          width: 130,
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
          title: metadataIsHardCoded,
          width: 170,
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
          field: "ActivityValue",
          title: activityValue,
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
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditMetadataNounplusVerb(\"#=MetadataNounplusVerbMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
          width: 50
        },
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteMetadataNounplusVerb(\"#=MetadataNounplusVerbMasterHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
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

$("#btnMetadataNounplusVerbSave").click(function () {
  if ($("#metadataNounplusVerbForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('metadataNounplusVerbForm');

    if ($("#yes")[0].checked) {
      viewModelMetadataNounlplusVerb.IsHardCoded = "True";
    }
    if ($("#no")[0].checked) {
      viewModelMetadataNounlplusVerb.IsHardCoded = "False";
    }

    $.ajax({
      url: BCMConfig.API_METADATANOUNPLUSVERB_UPDATE,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelMetadataNounlplusVerb),
      processdata: true,
      success: function (response) {
        if (response.errorModel != null && response.errorModel != "") {
          ValidateForm('metadataNounplusVerbForm', response.errorModel);
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

$("#btnMetadataNounplusVerbCancel").click(function () {
  ResetForm();
  $('#collapseExample').collapse('hide');
});
$("#clearFilters").click(function () {
  $("#gridMetadataNounplusVerb").data("kendoGrid").dataSource.filter([]);
});

function EditMetadataNounplusVerb(NounVerbHashId) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  ResetForm();

  ////var row = $(itemObject).closest("tr");
  ////var uid = $(row).data("uid");
  ////var grid = $("#gridMetadataNounplusVerb").data("kendoGrid");
  ////var currentDataRow = grid.dataItem(row);

  GetMetadataNounPlusVerbBasedOnHash(NounVerbHashId);
  $('#collapseExample').collapse('show');
  $(".form-group").removeClass('has-error');
  ResetvalidateForm('metadataNounplusVerbForm');
  moveDivTop("topPanel");
}

var ResetForm = function () {
  initViewModel();
  ValidateForm('metadataNounplusVerbForm', '');
  ResetvalidateForm('metadataNounplusVerbForm');
  kendo.bind($("#metadataNounplusVerbForm"), viewModelMetadataNounlplusVerb);
  resetDDL();
}

var initViewModel = function () {
  viewModelMetadataNounlplusVerb = kendo.observable({
    MetadataNounplusVerbMasterHashId: "",
    MetadataTypeMasterHashId: "",
    Noun: "",
    Verb: "",
    IsHardCoded: "",
    MetadataDynamicNounplusVerb: "",
    ActivityTypeMasterHashId: ""
  });

};

function DeleteMetadataNounplusVerb(NounVerbHashId) {
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
      ////var grid = $("#gridMetadataNounplusVerb").data("kendoGrid");
      ////var currentDataRow = grid.dataItem(row);

      var MetadataNounplusVerbMasterHashId = NounVerbHashId; ////currentDataRow.MetadataNounplusVerbMasterHashId

      $.ajax({
        url: BCMConfig.API_METADATANOUNPLUSVERB_DELETE + "?metadataNounplusVerbMasterHashId=" + MetadataNounplusVerbMasterHashId,
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

$('input[name=isHardCoded]').change(function () {

  if ($("#yes")[0].checked) {
    $("#DivDynamicNoun").hide();
    $("#DivNoun").show();
  }
  if ($("#no")[0].checked) {
    $("#DivDynamicNoun").show();
    $("#DivNoun").hide();
  }

});

$("#metadataTypeMasterHashId").change(function () {
  resetDDL();
  var metadataTypeMasterHashId = $("#metadataTypeMasterHashId option:selected").val();
  GetActivityTypeDDList("activityTypeMasterHashId", metadataTypeMasterHashId);
  viewModelMetadataNounlplusVerb.ActivityTypeMasterHashId = "";
});

function resetDDL() {
  $("#activityTypeMasterHashId").find("option:not(:first)").remove();
}

function GetMetadataNounPlusVerbBasedOnHash(NounVerbHashId) {

  $.ajax({
    url: BCMConfig.API_GETMETADATA_NOUN_PLUS_VERB_BASED_ON_HASH + "?nounVerbHashId=" + NounVerbHashId,
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

  viewModelMetadataNounlplusVerb.MetadataNounplusVerbMasterHashId = response.metadataNounplusVerbMasterHashId;
  viewModelMetadataNounlplusVerb.MetadataTypeMasterHashId = response.metadataTypeMasterHashId;
  viewModelMetadataNounlplusVerb.Verb = response.verb;
  viewModelMetadataNounlplusVerb.IsHardCoded = response.isHardCoded;

  if (response.isHardCoded) {
    $("#yes").prop("checked", true);
    $("#DivDynamicNoun").hide();
    $("#DivNoun").show();
    viewModelMetadataNounlplusVerb.Noun = response.noun;
  }
  else {
    $("#no").prop("checked", true);
    $("#DivDynamicNoun").show();
    $("#DivNoun").hide();
    viewModelMetadataNounlplusVerb.MetadataDynamicNounplusVerb = response.noun;
  }
  resetDDL();
  GetActivityTypeDDList("activityTypeMasterHashId", viewModelMetadataNounlplusVerb.MetadataTypeMasterHashId);
  viewModelMetadataNounlplusVerb.ActivityTypeMasterHashId = response.activityTypeMasterHashId;

  kendo.bind($("#metadataNounplusVerbForm"), viewModelMetadataNounlplusVerb);
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}