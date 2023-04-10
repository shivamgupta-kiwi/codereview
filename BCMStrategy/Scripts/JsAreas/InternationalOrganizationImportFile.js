var lisfOfInternationalOrganizationImportedRecords;
$(document).ready(function () {
  $("#btnValidateInternationalOrgImportFile").hide();
  $("#divImportBody").hide();
});


$("#importInternationOrgFile").change(function () {
  if (this.files.length > 0) {
    $("#btnValidateInternationalOrgImportFile").show();
  }
  else { $("#btnValidateInternationalOrgImportFile").hide(); }
});

$("#btnInternationalOrgImportFile").click(function () {
  $("#successMessage").html('');
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  var listOfInternationalOrganization = lisfOfInternationalOrganizationImportedRecords.filter(function (x) { return x.isValidRecord; });
  $.ajax({
    type: "POST",
    url: BCMConfig.API_INTERNATIONALORGIMPORT_URL,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(listOfInternationalOrganization),
    processdata: true,
    success: function (response) { //call successfull
      if (response.data) {
        lisfOfInternationalOrganizationImportedRecords = [];
        $("#gridInternationalOrganizationImport").data('kendoGrid').dataSource.data(lisfOfInternationalOrganizationImportedRecords);
        $("#importInternationOrgFile").val('');
        $("#btnValidateInternationalOrgImportFile").hide();
        $("#btnInternationalOrgImportFile").hide();
        $("#divImportBody").hide();
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        toastr.info(response.errorMessage);
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

$("#btnValidateInternationalOrgImportFile").click(function () {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  $("#errorMessage").html("");
  var data = new FormData();
  var files = $("#importInternationOrgFile").get(0).files;

  // Add the uploaded image content to the form data collection
  if (files.length > 0) {
    data.append("UploadedImage", files[0]);
  }

  var ajaxRequest = $.ajax({
    type: "POST",
    url: BCMConfig.API_INTERNATIONALORGVALIDATE_URL,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    contentType: false,
    processData: false,
    data: data,
    success: function (response) { //call successfull
      if (response.errorMessage != "") {
        $("#errorMessage").html(response.errorMessage).show();
        lisfOfInternationalOrganizationImportedRecords = [];
        if ($("#gridInternationalOrganizationImport").data('kendoGrid') != undefined) {
          $("#gridInternationalOrganizationImport").data('kendoGrid').dataSource.data(lisfOfInternationalOrganizationImportedRecords);
        }
      }
      else {
        $("#divImportBody").show();
        lisfOfInternationalOrganizationImportedRecords = response.data;
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

$("#btnInternationalOrgImportBack").click(function () {
  window.location.href = backURL;
})

var loadKendoGrid = function (data) {
  if($("#gridInternationalOrganizationImport").data("kendoTooltip")!=undefined)
  {
    $("#gridInternationalOrganizationImport").data("kendoTooltip").destroy();
  }
  $("#gridInternationalOrganizationImport").kendoGrid({
    dataSource: {
      type: "json",
      data: data,
      schema: {
        model: {
          fields: {
            organizationName: { type: "string" },
            designationName: { type: "string" },
            leaderFirstName: { type: "string" },
            leaderLastName: { type: "string" },
            entityName: { type: "string" },
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
          field: "organizationName",
          title: organizationName,
          width: 120

        },
        {
          field: "designationName",
          title: designation,
          width: 120

        },
        {
          field: "leaderFirstName",
          title: leaderFirstName,
          width: 120

        },
        {
          field: "leaderLastName",
          title: leaderLastName,
          width: 120

        },
        {
          field: "entityName",
          title: entityName,
          width: 120

        }
        ,
        {
          field: "isMultiLateralStr",
          title: MultiLateral,
          width: 100
        }
    ]
  });

  $("#gridInternationalOrganizationImport").kendoTooltip({
    filter: "td:nth-child(1) .validate", //this filter selects the second column's cells
    position: "bottom",
    //autoHide: false,
    showOn: "mouseenter",
    content: function (e) {
      
      var dataItem = $("#gridInternationalOrganizationImport").data("kendoGrid").dataItem(e.target.closest("tr"));
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
    lisfOfRecords.filter(function (x) { return x.isValidRecord ; }).length > 0
  )
  { $("#btnInternationalOrgImportFile").show(); }
  else { $("#btnInternationalOrgImportFile").hide(); }
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