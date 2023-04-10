var lisfOfPolicyMakersImportedRecords;
$(document).ready(function () {
  GetInstituionTypeDDList("institutionTypeHashId");
  $("#btnValidatePolicyMakersImportFile").hide();
  $("#divImportBody").hide();
});

$("#importPolicyMakersFile").change(function () {
  if (this.files.length > 0) {
    $("#btnValidatePolicyMakersImportFile").show();
  }
  else { $("#btnValidatePolicyMakersImportFile").hide(); }
});

$("#institutionTypeHashId").change(function () {
  if ($("#institutionTypeHashId").val() == "") {
    $("#btnValidatePolicyMakersImportFile").hide();
    $("#btnPolicyMakersImportFile").hide();
    $("#importPolicyMakersFile").val('');
    lisfOfPolicyMakersImportedRecords = [];
    $("#gridImportPolicyMakers").data('kendoGrid').dataSource.data(lisfOfPolicyMakersImportedRecords);
  }
});

$("#btnPolicyMakersImportFile").click(function () {
  $("#successMessage").html('');
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  var listOfPolicyMakers = lisfOfPolicyMakersImportedRecords.filter(function (x) { return x.isValidRecord; });
  $.ajax({
    type: "POST",
    url: BCMConfig.API_POLICYMAKERS_IMPORT_URL,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(listOfPolicyMakers),
    processdata: true,
    success: function (response) { //call successfull
      if (response.data) {
        lisfOfPolicyMakersImportedRecords = [];
        $("#gridImportPolicyMakers").data('kendoGrid').dataSource.data(lisfOfPolicyMakersImportedRecords);
        $("#importPolicyMakersFile").val('');
        $("#btnValidatePolicyMakersImportFile").hide();
        $("#btnPolicyMakersImportFile").hide();
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        toastr.info(response.errorMessage);
        $("#institutionTypeHashId").val('');
        $("#divImportBody").hide();
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

$("#btnValidatePolicyMakersImportFile").click(function () {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  $("#errorMessage").html("");
  $("#errorMessageInstType").html("");
  var data = new FormData();
  var files = $("#importPolicyMakersFile").get(0).files;
  data.append("InstitutionTypeHashId", $("#institutionTypeHashId").val());
  // Add the uploaded image content to the form data collection
  if (files.length > 0) {
    data.append("UploadedImage", files[0]);
  }

  var ajaxRequest = $.ajax({
    type: "POST",
    url: BCMConfig.API_POLICYMAKERS_VALIDATE_URL,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    contentType: false,
    processData: false,
    data: data,
    success: function (response) { //call successfull
      if (response.errorMessage != "") {
        //$("#errorMessage").html(response.errorMessage).show();
        if (response.errorMessage.toLowerCase().indexOf("insttype") != -1)
          $("#errorMessageInstType").html(response.errorMessage.split(':')[1]).show();
        else
          $("#errorMessage").html(response.errorMessage).show();

        lisfOfPolicyMakersImportedRecords = [];
        if ($("#gridImportPolicyMakers").data('kendoGrid') != undefined) {
          $("#gridImportPolicyMakers").data('kendoGrid').dataSource.data(lisfOfPolicyMakersImportedRecords);
        }
      }
      else {
        $("#divImportBody").show();
        lisfOfPolicyMakersImportedRecords = response.data;
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

$("#btnPolicyMakersImportBack").click(function () {
  window.location.href = backURL;
})

var loadKendoGrid = function (data) {
  if ($("#gridImportPolicyMakers").data("kendoTooltip") != undefined) {
    $("#gridImportPolicyMakers").data("kendoTooltip").destroy();
  }
  $("#gridImportPolicyMakers").kendoGrid({
    dataSource: {
      type: "json",
      data: data,
      schema: {
        model: {
          fields: {
            countryName: { type: "string" },
            designationName: { type: "string" },
            policyFirstName: { type: "string" },
            policyLastName: { type: "string" },
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
         field: "countryName",
         title: countryName,
         width: 120

       },
       {
         field: "designationName",
         title: designationType,
         width: 120

       },
       {
         field: "policyFirstName",
         title: policyFirstName,
         width: 120

       },
    {
      field: "policyLastName",
      title: policyLastName,
      width: 120

    }
    ]
  });

  $("#gridImportPolicyMakers").kendoTooltip({
    filter: "td:nth-child(1) .validate", //this filter selects the second column's cells
    position: "bottom",
    //autoHide: false,
    showOn: "mouseenter",
    content: function (e) {
      var dataItem = $("#gridImportPolicyMakers").data("kendoGrid").dataItem(e.target.closest("tr"));
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
  { $("#btnPolicyMakersImportFile").show(); }
  else { $("#btnPolicyMakersImportFile").hide(); }
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