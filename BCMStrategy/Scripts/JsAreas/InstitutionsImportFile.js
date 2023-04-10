var lisfOfInstitutionsImportedRecords;
$(document).ready(function () {
  GetInstituionTypeDDList("institutionTypeHashId");
  $("#btnValidateInstanceImportFile").hide();
  $("#divImportBody").hide();
});

$("#importInstitutionsFile").change(function () {
  if (this.files.length > 0) {
    $("#btnValidateInstanceImportFile").show();
  }
  else { $("#btnValidateInstanceImportFile").hide(); }
});
$("#institutionTypeHashId").change(function(){
  if ($("#institutionTypeHashId").val() == "") {
    lisfOfInstitutionsImportedRecords = [];
    $("#btnValidateInstanceImportFile").hide();
    $("#btnInstanceImportFile").hide();
    $("#importInstitutionsFile").val('');
    $("#gridImportInstitutions").data('kendoGrid').dataSource.data(lisfOfInstitutionsImportedRecords);
  }
})
$("#btnInstanceImportFile").click(function () {
  $("#successMessage").html('');
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  var listOfInstitutions = lisfOfInstitutionsImportedRecords.filter(function (x) { return x.isValidRecord; });
  
  $.ajax({
    type: "POST",
    url: BCMConfig.API_INSTITUTIONS_IMPORT_URL,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(listOfInstitutions),
    processdata: true,
    success: function (response) { //call successfull
      if (response.data) {
        lisfOfInstitutionsImportedRecords = [];
        $("#gridImportInstitutions").data('kendoGrid').dataSource.data(lisfOfInstitutionsImportedRecords);
        $("#importInstitutionsFile").val('');
        $("#btnValidateInstanceImportFile").hide();
        $("#btnInstanceImportFile").hide();
        $("#divImportBody").hide();
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        toastr.info(response.errorMessage);
        $("#institutionTypeHashId").val('')
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
    var files = $("#importInstitutionsFile").get(0).files;
    data.append("InstitutionTypeHashId",$("#institutionTypeHashId").val());
    // Add the uploaded image content to the form data collection
    if (files.length > 0) {
      data.append("UploadedImage", files[0]);
    }
    var ajaxRequest = $.ajax({
      type: "POST",
      url: BCMConfig.API_INSTITUTIONS_VALIDATE_URL,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      contentType: false,
      processData: false,
      data: data,
      success: function (response) { //call successfull
        if (response.errorMessage != "") {
          if (response.errorMessage.toLowerCase().indexOf("insttype")!=-1)
            $("#errorMessageInstType").html(response.errorMessage.split(':')[1]).show();
          else
            $("#errorMessage").html(response.errorMessage).show();

          lisfOfInstitutionsImportedRecords = [];
          if ($("#gridImportInstitutions").data('kendoGrid') != undefined) {
            $("#gridImportInstitutions").data('kendoGrid').dataSource.data(lisfOfInstitutionsImportedRecords);
          }
        }
        else {
          $("#divImportBody").show();
          lisfOfInstitutionsImportedRecords = response.data;
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

$("#btnInstitutionImportBack").click(function () {
  window.location.href = backURL;
})

var loadKendoGrid = function (data) {
  if($("#gridImportInstitutions").data("kendoTooltip")!=undefined)
  {
    $("#gridImportInstitutions").data("kendoTooltip").destroy();
  }
  $("#gridImportInstitutions").kendoGrid({
    dataSource: {
      type: "json",
      data: data,
      schema: {
        model: {
          fields: {
            countryName: { type: "string" },
            institutionName: { type: "string" },
            IsEuropeanUnionStr: { type: "string" },
            entityName:{type:"string"},
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
         width: 70

       },
       {
         field: "institutionName",
         title: institutionName,
         width: 70

       },
        {
          field: "isEuropeanUnionStr",
          title: EuropeanUnionString,
          width: 60
        },
        {
          field: "entityName",
          title: EntityName,
          width: 60
         }
    ]
  });

  $("#gridImportInstitutions").kendoTooltip({
    filter: "td:nth-child(1) .validate", //this filter selects the second column's cells
    position: "bottom",
    //autoHide: false,
    showOn: "mouseenter",
    content: function (e) {
      var dataItem = $("#gridImportInstitutions").data("kendoGrid").dataItem(e.target.closest("tr"));
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
    lisfOfRecords.filter(function (x) { return x.isValidRecord;}).length > 0
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
