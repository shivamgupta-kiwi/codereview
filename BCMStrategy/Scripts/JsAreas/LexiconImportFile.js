var lisfOfLexiconImportedRecords;

$(document).ready(function () {
  GetLexiconTypeDDList("lexiconTypeHashId");
  $("#btnValidateInstanceImportFile").hide();
  $("#divImportBody").hide();
});

$("#importLexiconFile").change(function () {
  if (this.files.length > 0) {
    $("#btnValidateInstanceImportFile").show();
  }
  else { $("#btnValidateInstanceImportFile").hide(); }
});

$("#lexiconTypeHashId").change(function () {
  if ($("#lexiconTypeHashId").val() == "") {
    lisfOfLexiconImportedRecords = [];
    $("#btnValidateInstanceImportFile").hide();
    $("#btnInstanceImportFile").hide();
    $("#importLexiconFile").val('');
    $("#gridImportLexicon").data('kendoGrid').dataSource.data(lisfOfLexiconImportedRecords);
  }
})

$("#btnInstanceImportFile").click(function () {
  $("#successMessage").html('');
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  var listOfLexicon = lisfOfLexiconImportedRecords.filter(function (x) { return x.isValidRecord; });

  $.ajax({
    type: "POST",
    url: BCMConfig.API_LEXICON_IMPORT_URL,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(listOfLexicon),
    processdata: true,
    success: function (response) { //call successfull
      if (response.data) {
        lisfOfLexiconImportedRecords = [];
        $("#gridImportLexicon").data('kendoGrid').dataSource.data(lisfOfLexiconImportedRecords);
        $("#importLexiconFile").val('');
        $("#btnValidateInstanceImportFile").hide();
        $("#btnInstanceImportFile").hide();
        $("#divImportBody").hide();
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        toastr.info(response.errorMessage);
        $("#lexiconTypeHashId").val('')
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
  $("#errorMessageLexiconType").html("");
  var data = new FormData();
  var files = $("#importLexiconFile").get(0).files;
  data.append("LexiconTypeHashId", $("#lexiconTypeHashId").val());
  // Add the uploaded image content to the form data collection
  if (files.length > 0) {
    data.append("UploadedImage", files[0]);
  }
  var ajaxRequest = $.ajax({
    type: "POST",
    url: BCMConfig.API_LEXICON_VALIDATE_URL,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    contentType: false,
    processData: false,
    data: data,
    success: function (response) { //call successfull
      if (response.errorMessage != "") {
        if (response.errorMessage.toLowerCase().indexOf("lexicontype") != -1)
          $("#errorMessageLexiconType").html(response.errorMessage.split(':')[1]).show();
        else
          $("#errorMessage").html(response.errorMessage).show();

        lisfOfLexiconImportedRecords = [];
        if ($("#gridImportLexicon").data('kendoGrid') != undefined) {
          $("#gridImportLexicon").data('kendoGrid').dataSource.data(lisfOfLexiconImportedRecords);
        }
      }
      else {
        $("#divImportBody").show();
        var result = response.data;
        if (result.IsNested == true) {
          result.isValidRecord = true;
        }
        lisfOfLexiconImportedRecords = response.data;
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

$("#btnLexiconImportBack").click(function () {
  window.location.href = backURL;
})

var loadKendoGrid = function (data) {
  if ($("#gridImportLexicon").data("kendoTooltip") != undefined) {
    $("#gridImportLexicon").data("kendoTooltip").destroy();
  }
  $("#gridImportLexicon").kendoGrid({
    dataSource: {
      type: "json",
      data: data,
      schema: {
        model: {
          fields: {
            lexiconIssue: { type: "string" },
            combinationValue: { type: "string" },
            status: { type: "string" },
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
         field: "lexiconIssue",
         title: LexiconTerm,
         width: 120

       },
       {
         field: "combinationValue",
         title: CombinationValue,
         width: 120

       },
       {
         field: "status",
         title: Nested,
         width: 120
       },
       {
         field: "lexiconLinkers",
         title: LexiconLinker,
         width: 120

       }
    ]
  });

  $("#gridImportLexicon").kendoTooltip({
    filter: "td:nth-child(1) .validate", //this filter selects the second column's cells
    position: "bottom",
    //autoHide: false,
    showOn: "mouseenter",
    content: function (e) {
      var dataItem = $("#gridImportLexicon").data("kendoGrid").dataItem(e.target.closest("tr"));
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
