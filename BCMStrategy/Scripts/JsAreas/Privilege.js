var selectedCustomerDict = {};
var LexiconPrivilegeCheckedIds = {};
var customerElement = $('#customer');
var lexiconTypeWiseCount = {};
var filterDict = {};

$(document).ready(function () {
  var element;
  customerElement.on('itemRemoved', function (event) {
    if (IsInDictionary(event.item.value, selectedCustomerDict))
      $('#' + event.item.value).prop('checked', false);
    if (selectedCustomerDict[event.item.value]) {
      delete selectedCustomerDict[event.item.value]
    }

  });
  var params = getParams(window.location.href);
  if (params['CustomerHashId']) {
    $('#customerHashId').val(params['CustomerHashId']);
    $('.customerPanel').hide();
    GetLexiconSelectedIdsBasedOnHash(params['CustomerHashId']);
    loadKendoGrid();
  }
  else {
    loadKendoGrid();
    $('.customerPanel').show();
  }
  BindCustomerGrid();
  customerElement.tagsinput({
    freeInput: false,
    itemValue: 'value',
    itemText: 'text'
  });
  $('input[type=text]').prop("readonly", true);

});

var loadKendoGrid = function () {

  element = $("#gridLexicon").kendoGrid({
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
            LexiconTypeMasterHashId: {
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
    height: 700,
    detailInit: detailInit,
    dataBound: masterGridDataBoundForParentGrid,
    columns: [
    {
      field: "LexiconType",
      title: "Lexicon Type",
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


  function detailInit(e) {
    var detailGrid = $("<div/>").appendTo(e.detailCell).kendoGrid({
      dataSource: {
        type: "json",
        transport: {
          read: {
            url: BCMConfig.API_GETALLLIST_LEXICON_URL,
            beforeSend: CommonJS.BeforeSendAjaxCall,
          },
          parameterMap: function (options) {
            filterDict[e.data.LexiconTypeMasterHashId] = JSON.stringify(options);
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
              TotalRecordCount: { type: "number" }
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
      height: 450,
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
      dataBound: masterDetailGridDataBound,
      columns: [
          {
            title: "<input id='" + e.data.LexiconTypeMasterHashId + "' name='" + e.data.LexiconTypeMasterHashId + "' type='checkbox' class='check-box checkBoxSize SelectAll' align='center' onChange='SelectAllClick(this)' />",
            template: "<input id='#=LexiconeIssueMasterHashId#' name='" + e.data.LexiconTypeMasterHashId + "' type='checkbox' class='checkbox checkBoxSize' onChange='ChangeLexiconSelection(this);' />",
            width: "43px"
          },
          {
            field: "LexiconIssue",
            title: 'Lexicon Term', width: "538px",
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
             title: 'Combination Value',
             width: "538px",
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
              title: "<input id='2' type='checkbox' class='checkbox' />",
              template: "<input name='TotalRecord' value='#=TotalRecordCount#' id='#=TotalRecordCount#' type='hidden' class='checkbox' />",
              hidden: true,
              filterable: {
                extra: false,
                operators: {
                  string: {
                    contains: "Contains"
                  }
                }
              }

            },

      ]
    });

    //CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    detailGrid.on("click", ".checkbox", function (event) {
      var checked = this.checked;
      row = $(this).closest("tr");
      var totalCount = GetLexiconIssueRecordCount(this);
      var lexiconTypeHashId = $(this).attr('name');
      var id = $(row).find('input[type="checkbox"]').attr('id');
      ManageDictionary(checked, id, lexiconTypeHashId);
      ManageSelectAllSelection(lexiconTypeHashId, totalCount);
    })
  }

};

function ManageDictionary(checked, id, lexiconTypeHashId) {
  if (checked) {
    LexiconPrivilegeCheckedIds[id] = {
      LexiconIssueHashId: id,
      LexiconTypeHashId: lexiconTypeHashId
    };
  }
  else {
    delete LexiconPrivilegeCheckedIds[id];
  }
}

function ManageSelectAllSelection(lexiconTypeHashId, totalCount) {
  var selectedElementArray = Object.keys(LexiconPrivilegeCheckedIds).map(function (key) {
    if (LexiconPrivilegeCheckedIds[key].LexiconTypeHashId != lexiconTypeHashId)
      return null;
    return LexiconPrivilegeCheckedIds[key].LexiconTypeHashId == lexiconTypeHashId;
  });
  var selectedElementCount = selectedElementArray.filter(function (value) { return value != null }).length;
  var selectAllElement = $("[name='" + lexiconTypeHashId + "']").filter(function () {
    return $(this).attr('id') == lexiconTypeHashId;
  });
  if (selectedElementCount == totalCount) {
    selectAllElement.prop('checked', true);
  }
  else {
    selectAllElement.prop('checked', false);
  }
}

function GetLexiconIssueRecordCount(target) {
  var count = $(target).closest('tr').find('[name="TotalRecord"]').val();
  return count;
}

function SelectAllClick(object) {
  var lexiconTypeHashId = $(object).attr('name');
  var isSelectAllChecked = $(object).is(':checked');
  var url;

  if (filterDict[lexiconTypeHashId]) {
    url = BCMConfig.GET_LEXICON_TERM_HASH_IDS_BASED_ON_LEXICONTYPE + "?lexiconTypeHashId=" + lexiconTypeHashId + "&parameterMap=" + filterDict[lexiconTypeHashId];
  }
  else {
    url = BCMConfig.GET_LEXICON_TERM_HASH_IDS_BASED_ON_LEXICONTYPE + "?lexiconTypeHashId=" + lexiconTypeHashId;
  }

  $.ajax({
    url: url,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    method: 'GET',
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    processdata: true,
    success: function (response) {
      if (response.data) {
        if (isSelectAllChecked) {
          AddLexiconHashIdsOnSelectAllChecked(response.data, lexiconTypeHashId);
          SelectLexiconRowCheckBox(response.data);
        }
        else {
          UnSelectLexiconRowCheckBox(response.data);
          RemoveLexiconHashIdsOnSelectAllUnChecked(response.data);
        }
      }
      else {
        toastr.info("webLink can not be deleted as it is associated with other entities");
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

function AddLexiconHashIdsOnSelectAllChecked(lexiconList, lexiconTypeHashId) {
  //var lexiconTypeHashId = $(this).attr('name');
  $.each(lexiconList, function (index, value) {
    if (!LexiconPrivilegeCheckedIds[value.lexiconeIssueMasterHashId]) {
      LexiconPrivilegeCheckedIds[value.lexiconeIssueMasterHashId] = {
        LexiconIssueHashId: value.lexiconeIssueMasterHashId,
        LexiconTypeHashId: lexiconTypeHashId
      }
    }
  });
}

function SelectLexiconRowCheckBox() {
  var isEdit = $('#customerHashId').val() ? true : false;
  $('td input:checkbox').each(function (index, element) {
    if (LexiconPrivilegeCheckedIds != null && IsInDictionary($(element).attr('id'), LexiconPrivilegeCheckedIds))
      $(this).prop('checked', true);
    //if (isEdit) {
    var lexiconTypeHashId = $(this).attr('name');
    var selectAllElement = $("[name='" + lexiconTypeHashId + "']").filter(function () {
      return $(this).attr('id') == lexiconTypeHashId;
    });

    var recCount = GetLexiconTypeWiseCount(lexiconTypeHashId);
    var totalGridCount = GetLexiconIssueRecordCount($(element));
    if (totalGridCount && recCount && (totalGridCount == recCount)) {
      selectAllElement.prop('checked', true);
    }
    else {
      selectAllElement.prop('checked', false);
    }
    // }
  });
}

function RemoveLexiconHashIdsOnSelectAllUnChecked(lexiconList) {
  $.each(lexiconList, function (index, value) {
    if (LexiconPrivilegeCheckedIds[value.lexiconeIssueMasterHashId]) {
      delete LexiconPrivilegeCheckedIds[value.lexiconeIssueMasterHashId];
    }
  });
}

function UnSelectLexiconRowCheckBox(lexiconList) {
  $.each(lexiconList, function (index, value) {
    $('#' + value.lexiconeIssueMasterHashId).prop('checked', false);
  });
}

function ChangeLexiconSelection(object) {

}

function EditLexicon(e) {
  ResetForm();

  e.preventDefault();
  var element = $(e.target);
  var grid = element.closest("[data-role=grid]").data("kendoGrid");
  var dataItem = grid.dataItem(element.closest("tr"));

  viewModelLexicon.LexiconeIssueMasterHashId = dataItem.LexiconeIssueMasterHashId
  viewModelLexicon.LexiconeTypeMasterHashId = dataItem.LexiconeTypeMasterHashId

  if (dataItem.IsNested == true) {
    $("#yes").prop("checked", true);
    $("#DivCombination").show();
  }
  else {
    $("#no").prop("checked", true);
    $("#DivCombination").hide();
  }
  if (dataItem.CombinationValue != null) {
    var arrayCombination = dataItem.CombinationValue.split(",");
    $.each(arrayCombination, function (i) {
      $("#combinationValue").tagsinput('add', arrayCombination[i]);
    });
  }

  $.each(dataItem.Linker, function (i) {
    $("#lexiconLinkers").tagsinput('add', dataItem.Linker[i]);
  });
  viewModelLexicon.LexiconIssue = dataItem.LexiconIssue

  kendo.bind($("#lexiconForm"), viewModelLexicon);
  $('#collapseExample').collapse('show');

  $(".form-group").removeClass('has-error');
  ResetvalidateForm('lexiconForm');
  moveDivTop("topPanel");
}

function DeleteLexicon(d) {
  bcmStrategyAlerts({
    type: "confirm",
    messageText: deletePopupMessage,
    headerText: deletePopupHeader,
    alertType: alertTypes.Info
  }).done(function (e) {
    if (e) {
      CommonJS.SetPageLoader(PageLoaderActivity.SHOW);

      d.preventDefault();
      var element = $(d.target);
      var grid = element.closest("[data-role=grid]").data("kendoGrid");
      var dataItem = grid.dataItem(element.closest("tr"));

      var LexiconeIssueMasterHashId = dataItem.LexiconeIssueMasterHashId

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

function masterDetailGridDataBound(e) {
  var grid = e.sender;
  if (grid.dataSource.total() == 0) {
    var colCount = grid.columns.length;
    $(e.sender.wrapper)
        .find('tbody').first()
        .append('<tr class="kendo-data-row" style="text-align: center;"><td colspan="' + colCount + '" class="no-data">' + noDataFound + '</td></tr>');
  }
  SelectLexiconRowCheckBox();
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}

function masterGridDataBoundForParentGrid(e) {
  //element.expand(".k-item");
  this.expandRow(this.tbody.find("tr.k-master-row").first());
  var grid = e.sender;
  if (grid.dataSource.total() == 0) {
    var colCount = grid.columns.length;
    $(e.sender.wrapper)
        .find('tbody').first()
        .append('<tr class="kendo-data-row" style="text-align: center;"><td colspan="2" class="no-data">' + noDataFound + '</td></tr>');
  }
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}

function ToggelCustomerModel() {
  $('#customerModal').modal('toggle');
}

function BindCustomerGrid() {
  $("#customerGrid").kendoGrid({
    height: 300,
    dataSource: {
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_GET_ALL_CUSTOMER,
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
            CustomerMasterHashId: { type: "string" },
            CustomerFirstName: { type: "string" },
            CustomerMiddleName: { type: "string" },
            CustomerLastName: { type: "string" },
            Designation: { type: "string" },


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
    height: 380,
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
    dataBound: masterGridCustomerDataBound,
    columns: [
         {
           title: "",
           template: "<input id='#= CustomerMasterHashId #' onchange=OnCustomerSelectionCheckBoxClick(this) type='checkbox' name='Customer' value='customer'>",
           width: 50
         },
        {
          field: "Designation",
          title: title,
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
          field: "CustomerFirstName",
          title: "First Name",
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
          field: "CustomerMiddleName",
          title: "Middle Name",
          width: 140,
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
          field: "CustomerLastName",
          title: "Last Name",
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

    ]
  });
}

function OnCustomerSelectionCheckBoxClick(itemObject) {
  var row = $(itemObject).closest("tr");
  var uid = $(row).data("uid");
  var grid = $("#customerGrid").data("kendoGrid");
  var currentDataRow = grid.dataItem(row);
  var customerMasterHashId = currentDataRow.CustomerMasterHashId;
  var firstName = currentDataRow.CustomerFirstName;
  var middleName = currentDataRow.CustomerMiddleName;
  var lastName = currentDataRow.CustomerLastName;
  if ($(itemObject).is(':checked')) {
    if (!selectedCustomerDict[customerMasterHashId]) {
      selectedCustomerDict[customerMasterHashId] = {
        firstName: firstName,
        middleName: middleName,
        lastName: lastName
      };
    }
  }
  else {
    if (selectedCustomerDict[customerMasterHashId]) {
      delete selectedCustomerDict[customerMasterHashId]
    }
  }
}

function masterGridCustomerDataBound(e) {
  var grid = e.sender;
  var noDataFoundString = 'No Data Found';
  if (grid.dataSource.total() == 0) {
    var colCount = grid.columns.length;
    $(e.sender.wrapper)
        .find('tbody').first()
        .append('<tr class="kendo-data-row" style="text-align: center;"><td colspan="' + colCount + '" class="no-data">' + noDataFoundString + '</td></tr>');
  }
  $('#customerGrid .k-grid-content').height(250);
  $('td input:checkbox', $('#customerGrid')).each(function (index, element) {
    if (selectedCustomerDict != null && IsInDictionary($(element).attr('id'), selectedCustomerDict))
      $(this).prop('checked', true);
  });

  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}

function OnCustomerSelectClick(itemObject) {
  $("#customer").tagsinput('removeAll');
  $('#selectedCustomerHashIds').val('');
  var grid = $("#customerGrid").data("kendoGrid");

  $.each(selectedCustomerDict, function (key, object) {
    var firstName = object.firstName;
    var middleName = object.middleName;
    var lastName = object.lastName;
    $('#selectedCustomerHashIds').val(function (i, val) {
      return val != '' ? (val + ';' + key) : key;
    });
    customerElement.tagsinput('add', { "value": key, "text": firstName + " " + middleName + " " + lastName });
  });
  $('#customerModal').modal('toggle');
}

function IsInDictionary(key, dictionary) {
  if (dictionary[key])
    return true;
}

$('#btnLexiconAccessSave').click(function () {
  //if ($('#customer').val() == '' || $('#customer').val() == undefined) {
  //    toastr.info("Please select at least one customer to assign lexicon subscription privileges");
  //    return false;
  //}
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  var legislatorAccessModel = BindData();
  $.ajax({
    url: BCMConfig.API_POST_LEXICON_ACCESS,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    method: 'POST',
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(legislatorAccessModel),
    processdata: true,
    success: function (response) {
      if (response.errorModel != null && response.errorModel != "") {
        ApplyCustomValidation(response.errorModel);
      }
      if (response.data) {
        toastr.info(response.errorMessage);
        //ReloadPage();
        RedirectToUrl($('#lexiconCustomerListUrl')[0].href, true);
      }
      else {
        if (response.errorMessage != null && response.errorMessage != "")
          toastr.error(response.errorMessage);
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    complete: function (e) {
    },
    error: function (e) {
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      CommonJS.HandleErrorStatus(e.status);
    }
  });
});

function ReloadPage() {
  setTimeout(function () {
    location.reload(true);
  }, 3000);
}

function RedirectToUrl(url, isTimeOut) {
  if (isTimeOut) {
    setTimeout(function () {
      window.location.href = url;
    }, 3000);
  }
  else {
    window.location.href = url;
  }
}

$('#lexiconAccessManCancel').click(function (e) {
  RedirectToUrl($('#lexiconCustomerListUrl')[0].href, false);
});


function ApplyCustomValidation(errorModel) {
  if (errorModel['selectedCustomerHashIds']) {
    toastr.error(errorModel['selectedCustomerHashIds']);
  }
  if (errorModel['selectedLexiconHashIds']) {
    toastr.error(errorModel['selectedLexiconHashIds']);
  }
}

function BindData() {
  $('#selectedCustomerHashIds').val('');
  $('#selectedLexiconHashIds').val('');
  var selectedCustomerHashIds = Object.keys(selectedCustomerDict);
  var selectedLexiconHashIds = Object.keys(LexiconPrivilegeCheckedIds);
  var customerHashId = $('#customerHashId').val();
  var legislatorAccessViewModel;
  if (customerHashId) {
    legislatorAccessViewModel = {
      'SelectedCustomerHashIds': selectedCustomerHashIds,
      'SelectedLexiconHashIds': selectedLexiconHashIds,
      'CustomerMasterHashId': customerHashId
    };
  }
  else {
    legislatorAccessViewModel = {
      'SelectedCustomerHashIds': selectedCustomerHashIds,
      'SelectedLexiconHashIds': selectedLexiconHashIds
    };
  }
  return legislatorAccessViewModel;
}

function GetLexiconSelectedIdsBasedOnHash(customerHash) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  AjaxCall(customerHash);
}



function AjaxCall(customerHash) {
  $.ajax({
    url: BCMConfig.API_GET_LEXICON_SELECTED_IDS_BASED_ON_CUSTOMER + "?customerHashId=" + customerHash,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    async: false,
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      BindLexiconIdsOnEdit(response);
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
    }
  });
}

function BindLexiconIdsOnEdit(responce) {
  if (responce) {
    $.each(responce, function (index, value) {
      if (!LexiconPrivilegeCheckedIds[value.lexiconeIssueMasterHashId]) {
        LexiconPrivilegeCheckedIds[value.lexiconeIssueMasterHashId] = {
          LexiconIssueHashId: value.lexiconeIssueMasterHashId,
          LexiconTypeHashId: value.lexiconTypeHashId
        }
      }
    });
  }
}

function GetLexiconTypeWiseCount(lexiconTypeHashId) {
  var array = Object.keys(LexiconPrivilegeCheckedIds).map(function (key) {
    return LexiconPrivilegeCheckedIds[key];
  });
  var filteredArray = array.filter(function (element, index) {
    return element.LexiconTypeHashId == lexiconTypeHashId;

  });

  return filteredArray.length;
}

function UniqueBy(arr, prop) {
  return arr.reduce(function (a, d) {
    if (!a.includes(d[prop])) a.push(d[prop]);
    return a;
  }, []);
}

var getParams = function (url) {
  var params = {};
  var parser = document.createElement('a');
  parser.href = url;
  var query = parser.search.substring(1);
  var vars = query.split('&');
  for (var i = 0; i < vars.length; i++) {
    var pair = vars[i].split('=');
    params[pair[0]] = decodeURIComponent(pair[1]);
  }
  return params;
};
