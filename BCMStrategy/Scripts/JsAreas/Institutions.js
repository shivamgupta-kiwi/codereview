var viewModelInstitutions;

$(document).ready(function () {
  GetInstituionTypeDDList("institutionTypeHashId");
  GetCountryDDList("countryMasterHashId");
  ResetForm();
  $('#countryMasterHashId').prop('disabled', true);
  loadKendoGrid();
});

var loadKendoGrid = function () {
  $("#gridInstitutions").kendoGrid({
    //toolbar: ["excel"],
    //excel: {
    //  fileName: "data.xlsx",
    //  filterable: true
    //},
    dataSource: {
      //requestStart: CommonJS.SetPageLoader(PageLoaderActivity.SHOW),
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_GETALLLIST_INSTITUTIONS_URL,
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
            InstitutionMasterHashId: { type: "string" },
            CountryMasterHashId: { type: "string" },
            InstitutionTypeHashId: { type: "string" },
            CountryName: { type: "string" },
            InstitutionType: { type: "string" },
            InstitutionsName: { type: "string" },
            //IsEuropeanUnion: {type : "bool"}
            IsEuropeanUnionString: { type: "string" }
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
          field: "CountryName",
          title: countryName,
          width: 100,
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
          field: "InstitutionType",
          title: institutionType,
          width: 100,
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
          field: "InstitutionsName",
          title: institution,
          width: 100,
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
           field: "IsEuropeanUnionString",
           title: EuropeanUnionString,
           width: 100,
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
           field: "EntityName",
           title: Acronym,
           width: 100,
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
          template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditInstitutions(\"#=InstitutionMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
          width: 40
        },
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteInstitutions(\"#=InstitutionMasterHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
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

$("#btnInstitutionSave").click(function () {
  if ($("#institutionForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('institutionForm');

    if ($("#yes")[0].checked) {
      viewModelInstitutions.IsEuropeanUnion = "True";
      viewModelInstitutions.CountryMasterHashId = "";
      //$('#countryAsteriskSign').hide();
    }
    if ($("#no")[0].checked) {
      viewModelInstitutions.IsEuropeanUnion = "False";

      var selectedValue = $("#institutionTypeHashId").find("option:selected").text();

      if (selectedValue == "Multilateral") {
          viewModelInstitutions.CountryMasterHashId = "";
      }
      //viewModelInstitutions.EntityName = "";
      //$('#countryAsteriskSign').show();
    }

    $.ajax({
      url: BCMConfig.API_INSTITUTION_UPDATE,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelInstitutions),
      processdata: true,
      success: function (response) { //call successfull
        if (response.errorModel != null && response.errorModel != "") {
          ValidateForm('institutionForm', response.errorModel);
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

$("#btnInstitutionCancel").click(function () {
  ResetForm();
  $('#collapseExample').collapse('hide');
});
$("#btnAddNewInstitution").click(function () {
  GetInstituionTypeDDList("institutionTypeHashId");
})
$("#clearFilters").click(function () {
  $("#gridInstitutions").data("kendoGrid").dataSource.filter([]);
});

function EditInstitutions(InstitutionHashId) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  ResetForm();

  //var row = $(itemObject).closest("tr");
  //var uid = $(row).data("uid");
  //var grid = $("#gridInstitutions").data("kendoGrid");
  //var currentDataRow = grid.dataItem(row);


  GetInstitutionBasedOnHash(InstitutionHashId);
  $('#collapseExample').collapse('show');
  $(".form-group").removeClass('has-error');
  ResetvalidateForm('institutionForm');
  moveDivTop("topPanel");
}

var ResetForm = function () {
  initViewModel();
  ValidateForm('institutionForm', '');
  ResetvalidateForm('institutionForm');
  kendo.bind($("#institutionForm"), viewModelInstitutions);
}

var initViewModel = function () {
  viewModelInstitutions = kendo.observable({
    InstitutionMasterHashId: "",
    CountryMasterHashId: "",
    InstitutionTypeHashId: "",
    InstitutionsName: "",
    EntityName: ""
  });
  //GetInstituionTypeDDList("institutionTypeHashId");

};

function DeleteInstitutions(InstitutionHashId) {
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
      ////var grid = $("#gridInstitutions").data("kendoGrid");
      ////var currentDataRow = grid.dataItem(row);

      var InstitutionMasterHashId = InstitutionHashId; ////currentDataRow.InstitutionMasterHashId

      $.ajax({
        url: BCMConfig.API_INSTITUTIONS_DELETE + "?institutionMasterHashId=" + InstitutionMasterHashId,
        beforeSend: CommonJS.BeforeSendAjaxCall,
        method: 'GET',
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        processdata: true,
        success: function (response) { //call successfull
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
          //error occurred
        }
      });
    }
  });
}

$("input[name='IsEuropeanUnion']").change(function () {
  if ($(this).attr('id') == "yes") {
    //$('.entityNameDiv').show();
    $('#countryAsteriskSign').hide();
    $('#countryMasterHashId').val('');
    $('#countryMasterHashId').prop('disabled', true);
  }
  else {
    var selectedValue = $("#institutionTypeHashId").find("option:selected").text();

    if (selectedValue == "Multilateral") {
      $('#countryAsteriskSign').hide();
      $('#countryMasterHashId').val('');
      $('#countryMasterHashId').prop('disabled', true);
    }
    else {
      $('#countryAsteriskSign').show();
      $('#countryMasterHashId').prop('disabled', false);
    }
  }
});

$("#institutionTypeHashId").change(function () {
  var selectedValue = $(this).find("option:selected").text();

  if (selectedValue == "Multilateral") {
    $('#countryAsteriskSign').hide();
    $('#countryMasterHashId').val('');
    $('#countryMasterHashId').prop('disabled', true);
  }
  else {
    if ($("#yes")[0].checked) {
      $('#countryAsteriskSign').hide();
      $('#countryMasterHashId').val('');
      $('#countryMasterHashId').prop('disabled', true);
    }
    if ($("#no")[0].checked) {
      $('#countryAsteriskSign').show();
      $('#countryMasterHashId').prop('disabled', false);
    }
  }
});

function GetInstitutionBasedOnHash(InstitutionHashId) {

  $.ajax({
    url: BCMConfig.API_GETINSTITUTION_BASED_ON_HASH + "?institutionHashId=" + InstitutionHashId,
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

  viewModelInstitutions.CountryMasterHashId = response.countryMasterHashId
  viewModelInstitutions.InstitutionTypeHashId = response.institutionTypeHashId
  viewModelInstitutions.InstitutionsName = response.institutionsName;
  viewModelInstitutions.InstitutionMasterHashId = response.institutionMasterHashId;
  viewModelInstitutions.EntityName = response.entityName;
  viewModelInstitutions.InstitutionType = response.institutionType;

  if (response.isEuropeanUnionString == "Yes") {
    $("#yes").prop("checked", true);
    //$('.entityNameDiv').show();
    $('#countryAsteriskSign').hide();
    $('#countryMasterHashId').val('');
    $('#countryMasterHashId').prop('disabled', true);
  }
  else {

    if (viewModelInstitutions.InstitutionType == "Multilateral")
    {
      $('#countryAsteriskSign').hide();
      $('#countryMasterHashId').val('');
      $('#countryMasterHashId').prop('disabled', true);
    }
  else
    {
      $("#no").prop("checked", true);
      $('#countryAsteriskSign').show();
      $('#countryMasterHashId').prop('disabled', false);
    }
  }
  kendo.bind($("#institutionForm"), viewModelInstitutions);

  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}