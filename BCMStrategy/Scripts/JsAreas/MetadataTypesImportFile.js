var lisfOfMetadataTypesImportedRecords;
$(document).ready(function () {
  GetWebsiteTypeDDList("websiteTypeHashId");
  $("#btnValidateInstanceImportFile").hide();
  $("#divImportBody").hide();
});

$("#importMetadataTypesFile").change(function () {
  if (this.files.length > 0) {
    $("#btnValidateInstanceImportFile").show();
  }
  else { $("#btnValidateInstanceImportFile").hide(); }
});
$("#websiteTypeHashId").change(function () {
  if ($("#websiteTypeHashId").val() == "") {
    lisfOfMetadataTypesImportedRecords = [];
    $("#btnValidateInstanceImportFile").hide();
    $("#btnInstanceImportFile").hide();
    $("#importMetadataTypesFile").val('');
    $("#gridImportMetadataTypes").data('kendoGrid').dataSource.data(lisfOfMetadataTypesImportedRecords);
  }
})
$("#btnInstanceImportFile").click(function () {
  $("#successMessage").html('');
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  var listOfMetadataTypes = lisfOfMetadataTypesImportedRecords.filter(function (x) { return x.isValidRecord; });

  $.ajax({
    type: "POST",
    url: BCMConfig.API_METADATATYPES_IMPORT_URL,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(listOfMetadataTypes),
    processdata: true,
    success: function (response) { //call successfull
      if (response.data) {
        lisfOfMetadataTypesImportedRecords = [];
        $("#gridImportMetadataTypes").data('kendoGrid').dataSource.data(lisfOfMetadataTypesImportedRecords);
        $("#importMetadataTypesFile").val('');
        $("#btnValidateInstanceImportFile").hide();
        $("#btnInstanceImportFile").hide();
        $("#divImportBody").hide();
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        toastr.info(response.errorMessage);
        $("#websiteTypeHashId").val('')
      }
      else {
        toastr.error(response.errorMessage);
      }
    },
    error: function (e) {
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      CommonJS.HandleErrorStatus(e.status);
      //error occurred
    }
  });
});

$("#btnValidateInstanceImportFile").click(function () {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);

  $("#errorMessage").html("");
  $("#errorMessageInstType").html("");
  var data = new FormData();
  var files = $("#importMetadataTypesFile").get(0).files;
  data.append("websiteTypeHashId", $("#websiteTypeHashId").val());
  // Add the uploaded image content to the form data collection
  if (files.length > 0) {
    data.append("UploadedImage", files[0]);
  }
  var ajaxRequest = $.ajax({
    type: "POST",
    url: BCMConfig.API_METADATATYPES_VALIDATE_URL,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    contentType: false,
    processData: false,
    data: data,
    success: function (response) { //call successfull
      if (response.errorMessage != "") {
        if (response.errorMessage.toLowerCase().indexOf("metadatatype") != -1)
          $("#errorMessageMetadataType").html(response.errorMessage.split(':')[1]).show();
        else
          $("#errorMessage").html(response.errorMessage).show();

        lisfOfMetadataTypesImportedRecords = [];
        if ($("#gridImportMetadataTypes").data('kendoGrid') != undefined) {
          $("#gridImportMetadataTypes").data('kendoGrid').dataSource.data(lisfOfMetadataTypesImportedRecords);
        }
      }
      else {
        $("#divImportBody").show();
        lisfOfMetadataTypesImportedRecords = response.data;
        loadKendoGrid(response.data);
        ShowImportButton(response.data);
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      CommonJS.HandleErrorStatus(e.status);
      //error occurred
    }
  });
});

$("#btnMetadataTypesImportBack").click(function () {
  window.location.href = backURL;
})

var loadKendoGrid = function (data) {
  if ($("#gridImportMetadataTypes").data("kendoTooltip") != undefined) {
    $("#gridImportMetadataTypes").data("kendoTooltip").destroy();
  }
  $("#gridImportMetadataTypes").kendoGrid({
    dataSource: {
      type: "json",
      data: data,
      schema: {
        model: {
          fields: {
            countryName: { type: "string" },
            institutionName: { type: "string" },
            errorModel: {}
          }
        }
      },
      serverPaging: false,
      serverFiltering: false,
      serverSorting: false
    },
    error: function (e) {
      HandleError(e);
    },
    filterable: false,
    sortable: false,
    height: 450,
    pageable: false,
    columns: [
       {
         field: "errorModel",
         template: "#=GetImage(isValidRecord)#",
         title: " ",
         width: 30
       },
       {
         field: "WebsiteType",
         title: websiteType,
         width: 120

       },
       {
         field: "MetaData",
         title: metadata,
         width: 120

       }
       ,
       {
         field: "MetaDataValueStr",
         title: metadataValue,
         width: 120

       }
       ,
       {
         field: "Status",
         title: ActivityTypeExist,
         width: 120

       }
    ]
  });

  $("#gridImportMetadataTypes").kendoTooltip({
    filter: "td:nth-child(1) .validate", //this filter selects the second column's cells
    position: "bottom",
    //autoHide: false,
    showOn: "mouseenter",
    content: function (e) {
      var dataItem = $("#gridImportMetadataTypes").data("kendoGrid").dataItem(e.target.closest("tr"));
      $(".k-animation-container").css("width", "300px");
      var errorModel = dataItem.errorModel;
      var message = "<ul>";
      if (errorModel.length > 0) {

        for (i = 0; i < errorModel.length; i++) {
          message += "<li>" + errorModel[i].value + "</li>";
        }

      }
      return message;
    }
  });
};

/*Check for valid record exist or not*/
function ShowImportButton(lisfOfRecords) {
  if (
    lisfOfRecords &&
    lisfOfRecords.length > 0 &&
    lisfOfRecords.filter(function (x) { return x.isValidRecord; }).length > 0
  )
  { $("#btnInstanceImportFile").show(); }
  else { $("#btnInstanceImportFile").hide(); }
}

/*Get Image for display valid or invalid icons(Thumbs-Up and Thumbs-Down)*/
function GetImage(status) {
  if (status) {
    return '<i style="color: green;" class="fa fa-thumbs-up"></i>';

  }
  else {
    return "<i style='color: red;cursor: pointer'  class='fa fa-thumbs-down validate'></i>";
  }
}
