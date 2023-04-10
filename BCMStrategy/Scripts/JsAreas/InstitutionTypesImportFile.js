var lisfOfImportedRecords;
$(document).ready(function () {
  
  $("#btnValidateImportFile").hide();
  $("#divImportBody").hide();
  //jQuery.browser = {};
  //(function () {
  //  jQuery.browser.msie = false;
  //  jQuery.browser.version = 0;
  //  if (navigator.userAgent.match(/MSIE ([0-9]+)\./)) {
  //    jQuery.browser.msie = true;
  //    jQuery.browser.version = RegExp.$1;
  //  }
  //})();
});

$("#importFile").change(function () {
  if (this.files.length > 0) {
    //if (jQuery.browser.msie) {
    //  $('#btnValidateImportFile').css({ "display": "block" });
    //}
    //else {
    //  $("#btnValidateImportFile").show();
    //}
    $("#btnValidateImportFile").show();

  }
  else {
    //if (jQuery.browser.msie) {
    //  $('#btnValidateImportFile').css({ "display": "none" });
    //}
    //else {
    //  $("#btnValidateImportFile").hide();

    //}
    $("#btnValidateImportFile").hide();

  }



});

$("#btnImportFile").click(function () {
  $("#successMessage").html('');
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  var listOfHeadState;

  listOfHeadState = lisfOfImportedRecords.filter(function (x) { return x.isValidRecord; });
  

  $.ajax({
    type: "POST",
    url: BCMConfig.API_INSTTYPESIMPORT_URL,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(listOfHeadState),
    processdata: true,
    success: function (response) { //call successfull
      if (response.data) {
        lisfOfImportedRecords = [];
        $("#divImportBody").show();
        $("#grid").data('kendoGrid').dataSource.data(lisfOfImportedRecords);
        $("#importFile").val('');
        $("#btnValidateImportFile").hide();
        $("#btnImportFile").hide();
        $("#divImportBody").hide();
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        toastr.info(response.errorMessage);
      }
      else {
        toastr.error(response.errorMessage);
      }
    },
    error: function (response) {
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      //error occurred
    }
  });
});

$("#btnValidateImportFile").click(function () {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  $("#errorMessage").html("");
  var data = new FormData();
  var files = $("#importFile").get(0).files;

  // Add the uploaded image content to the form data collection
  if (files.length > 0) {
    data.append("UploadedImage", files[0]);
  }

  var ajaxRequest = $.ajax({
    type: "POST",
    url: BCMConfig.API_INSTTYPESIMPORTVALIDATE_URL,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    contentType: false,
    processData: false,
    data: data,
    success: function (response) { //call successfull
      if (response.errorMessage != "") {
        $("#errorMessage").html(response.errorMessage).show();
        lisfOfImportedRecords = [];
        if ($("#grid").data('kendoGrid') != undefined) {
          $("#grid").data('kendoGrid').dataSource.data(lisfOfImportedRecords);
        }
      }
      else {
        $("#divImportBody").show();
        lisfOfImportedRecords = response.data;
        loadKendoGrid(response.data);
        ShowImportButton(response.data);
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
  });
});

$("#btnInstitutionTypeImportBack").click(function () {
  window.location.href = backURL;
})

var loadKendoGrid = function (data) {
  if($("#grid").data("kendoTooltip")!=undefined)
  {
    $("#grid").data("kendoTooltip").destroy();
  }
  $("#grid").kendoGrid({
    dataSource: {
      type: "json",
      data: data,
      schema: {
        model: {
          fields: {
            InstitutionTypes: { type: "string" },
            errorModel: {}
          }
        }
      },
      //pageSize: 10,
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
    //pageable: {
    //  pageSize: 10,
    //  pageSizes: [5, 10, 15],
    //  refresh: true,
    //  buttonCount: 5,
    //  input: true
    //},
    columns: [
        {
          field: "errorModel",
          template: "#=GetImage(isValidRecord)#",
          title: " ",
          width: 30
        },
        {
          field: "institutionTypesName",
          title: institutionTypesName,
          width: 120

        },
            
    ]
  });

  $("#grid").kendoTooltip({
    filter: "td:nth-child(1) .validate", //this filter selects the second column's cells
    position: "bottom",
    //autoHide: false,
    showOn: "mouseenter",
    content: function (e) {
      var dataItem = $("#grid").data("kendoGrid").dataItem(e.target.closest("tr"));
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
    lisfOfRecords.filter(function(x) { return x.isValidRecord; }).length > 0

  )
  { $("#btnImportFile").show(); }
  else { $("#btnImportFile").hide();
  }
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