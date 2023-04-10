var viewModelLexicon;

$(document).ready(function () {
  GetLexiconTypeDDList("lexiconeTypeMasterHashId");
  ResetForm();
  loadKendoGrid();
});

var ResetForm = function () {
  initViewModel();
  ValidateForm('lexiconForm', '');
  ResetvalidateForm('lexiconForm');
  kendo.bind($("#lexiconForm"), viewModelLexicon);
  $("#combinationValue").tagsinput('removeAll');
  $("#lexiconLinkers").tagsinput('removeAll');
}

var hide = function () {
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
  requestStart: CommonJS.SetPageLoader(PageLoaderActivity.HIDE);

};

$("#btnLexiconCancel").click(function () {
  ResetForm();
  $('#collapseExample').collapse('hide');
});
$("#clearFilters").click(function () {
  $("#gridLexicon").data("kendoGrid").dataSource.filter([]);
});

var initViewModel = function () {
  viewModelLexicon = kendo.observable({
    LexiconeIssueMasterHashId: "",
    LexiconeTypeMasterHashId: "",
    IsNested: "",
    CombinationValue: "",
    LexiconIssue: "",
    LexiconLinkers: ""
  });

};

$("#btnLexiconSave").click(function () {

  if ($("#lexiconForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('lexiconForm');

    if ($("#yes")[0].checked) {
      viewModelLexicon.IsNested = "True";
      viewModelLexicon.CombinationValue = $("#combinationValue").val();
    }
    if ($("#no")[0].checked) {
      viewModelLexicon.IsNested = "False";
      viewModelLexicon.CombinationValue = "";
    }

    viewModelLexicon.Linker = $("#linker").val();

    $.ajax({
      url: BCMConfig.API_LEXICON_UPDATE,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelLexicon),
      processdata: true,
      success: function (response) { //call successfull
        if (response.errorModel != null && response.errorModel != "") {
          ValidateForm('lexiconForm', response.errorModel);
        }
        if (response.data) {
          toastr.info(response.errorMessage);
          ResetForm();
          loadKendoGrid();
          $('#collapseExample').collapse('hide');
          loadKendoGrid();
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

$('input[name=isNested]').change(function () {

  if ($("#yes")[0].checked) {
    $("#DivCombination").show();
  }
  if ($("#no")[0].checked) {
    $("#DivCombination").hide();
  }

});

var loadKendoGrid = function () {
  $(document).ready(function () {
    var element = $("#gridLexicon").kendoGrid({
      dataSource: {
        type: "json",
        transport: {
          read: {
            url: BCMConfig.API_GETALLLIST_LEXICONTYPE_URL,
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
              LexiconTypeMasterId: {
                type: "string"
              },
              LexiconType: {
                type: "string"
              },
            }
          }
        },
        pageSize: 10,
        serverPaging: false,
        serverFiltering: false,
        serverSorting: false
      },
      filterable: true,
      sortable: true,
      height: 450,
      //pageable: {
      //  messages: {
      //    itemsPerPage: "Items",
      //    display: "{0} - {1} of {2}",
      //  },
      //  pageSize: 10,
      //  pageSizes: [10, 20, 50],
      //  refresh: true,
      //  buttonCount: 5,
      //  input: true
      //},
      detailInit: detailInit,
      dataBound: masterGridDataBoundForParentGrid,
      //dataBound: function () {
      //  this.expandRow(this.tbody.find("tr.k-master-row").first());
      //},
      columns: [
      {
        field: "LexiconType",
        title: LexiconType,
        filterable: {
          extra: false,
          operators: {
            string: {
              contains: "Contains"
            }
          }
        }
      }
      ]
    });
  });

  function detailInit(e) {
    $("<div/>").appendTo(e.detailCell).kendoGrid({
      dataSource: {
        type: "json",
        transport: {
          read: {
            url: BCMConfig.API_GETALLLIST_LEXICON_URL,
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
              LexiconeIssueMasterHashId: { type: "string" },
              LexiconIssue: { type: "string" },
              CombinationValue: { type: "string" },
              IsNestedStr: { type: "string" },
              Linkers: { type: "string" },

            }
          }
        },
        serverPaging: true,
        serverSorting: true,
        serverFiltering: true,
        pageSize: 10,
        filter: {
          field: "LexiconeTypeMasterId", operator: "eq", value: e.data.LexiconTypeMasterId
        }
      },
      filterable: true,
      height: 350,
      sortable: true,
      pageable: {
        messages: {
          itemsPerPage: "Items",
          display: "{0} - {1} of {2}",
        },
        pageSize: 10,
        pageSizes: [10, 20, 50],
        refresh: true,
        buttonCount: 4,
        input: true
      },
      dataBound: masterGridDataBound,
      columns: [
          {
            field: "LexiconIssue",
            title: LexiconTerm, width: "150px",
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
             field: "CombinationValue",
             title: CombinationValue,
             width: "240px",
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
              field: "Status",
              title: Nested,
              width: "120px",
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
              field: "Linkers",
              title: LexiconLinker,
              width: "240px",
              sortable: false,
              filterable: false
              //filterable: {
              //  extra: false,
              //  operators: {
              //    string: {
              //      contains: "Contains"
              //    }
              //  }
              //}
            },
             {
               title: "",
               template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditLexicon(\"#=LexiconeIssueMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
               width: 50
             },
             {
               title: "",
               template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteLexicon(\"#=LexiconeIssueMasterHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
               width: 50
             }
            //{
            //  title: "", width: 50, command: [
            //            {
            //              id: "edit",
            //              name: "edit",
            //              click: EditLexicon,
            //              template: "<a class='k-button k-grid-edit' data-toggle='tooltip' title='Edit'  href='' style='min-width:16px;align:'center''><span class='fa fa-pencil-alt'></span></a> "
            //            }
            //  ]
            //},

//{
//  title: "", width: 50, command: [
//            {
//              id: "delete",
//              name: "delete",
//              click: DeleteLexicon,
//              template: "<a class='k-button k-grid-delete' data-toggle='tooltip' title='Delete' href='javascript:' style='min-width:16px;'><span class='fa fa-trash-alt'></span></a>"
//            }
//  ]
//}

      ]

    });

    CommonJS.SetPageLoader(PageLoaderActivity.HIDE);

  }

};

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
function masterGridDataBoundForParentGrid(e) {
  var grid = e.sender;
  if (grid.dataSource.total() == 0) {
    var colCount = grid.columns.length;
    $(e.sender.wrapper)
        .find('tbody').first()
        .append('<tr class="kendo-data-row" style="text-align: center;"><td colspan="2" class="no-data">' + noDataFound + '</td></tr>');
  }
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}

function EditLexicon(LexiconHashId) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  ResetForm();
  ////e.preventDefault();
  ////var element = $(e.target);
  ////var grid = element.closest("[data-role=grid]").data("kendoGrid");
  ////var dataItem = grid.dataItem(element.closest("tr"));


  GetLexiconBasedOnHash(LexiconHashId);
  $('#collapseExample').collapse('show');
  $(".form-group").removeClass('has-error');
  ResetvalidateForm('lexiconForm');
  moveDivTop("topPanel");
}

function DeleteLexicon(LexiconHashId) {
  bcmStrategyAlerts({
    type: "confirm",
    messageText: deletePopupMessage,
    headerText: deletePopupHeader,
    alertType: alertTypes.Info
  }).done(function (e) {
    if (e) {
      CommonJS.SetPageLoader(PageLoaderActivity.SHOW);

      ////d.preventDefault();
      ////var element = $(d.target);
      ////var grid = element.closest("[data-role=grid]").data("kendoGrid");
      ////var dataItem = grid.dataItem(element.closest("tr"));

      var LexiconeIssueMasterHashId = LexiconHashId; ////dataItem.LexiconeIssueMasterHashId

      $.ajax({
        url: BCMConfig.API_LEXICON_DELETE + "?lexiconIssueMasterHashId=" + LexiconeIssueMasterHashId,
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

function GetLexiconBasedOnHash(LexiconHashId) {

  $.ajax({
    url: BCMConfig.API_GETLEXICON_BASED_ON_HASH + "?lexiconHashId=" + LexiconHashId,
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

  viewModelLexicon.LexiconeIssueMasterHashId = response.lexiconeIssueMasterHashId;
  viewModelLexicon.LexiconeTypeMasterHashId = response.lexiconeTypeMasterHashId;
  if (response.isNested) {
    $("#yes").prop("checked", true);
    $("#DivCombination").show();
  }
  else {
    $("#no").prop("checked", true);
    $("#DivCombination").hide();
  }
  if (response.combinationValue != null) {
    var arrayCombination = response.combinationValue.split(",");
    $.each(arrayCombination, function (i) {
      $("#combinationValue").tagsinput('add', arrayCombination[i]);
    });
  }
  $.each(response.linker, function (i) {
    $("#lexiconLinkers").tagsinput('add', response.linker[i]);
  });
  viewModelLexicon.LexiconIssue = response.lexiconIssue
  kendo.bind($("#lexiconForm"), viewModelLexicon);
   
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}
