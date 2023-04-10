var viewModelUserManagement;
$(document).ready(function () {
  GetCountryDDList();
  ResetForm();
  loadKendoGrid();
});

$("#btnUserManagementSave").click(function () {
  if ($("#userManagementForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('userManagementForm');

    if (viewModelUserManagement.UserMasterHashId != '') {
      if ($("#active")[0].checked) {
        viewModelUserManagement.Status = UserMgmt.active
      }
      if ($("#inactive")[0].checked) {
        viewModelUserManagement.Status = UserMgmt.inactive;
      }
    }

    $.ajax({
      url: BCMConfig.API_USERMANAGEMENTUPDATE_URL,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelUserManagement),
      processdata: true,
      success: function (response) { //call successfull
        if (response.errorModel != null && response.errorModel != "") {
          //check validations for all controls
          ValidateForm('userManagementForm', response.errorModel);
        }
        if (response.data) {
          toastr.info(response.errorMessage);
          ResetForm();
          loadKendoGrid();
          $('#collapseExample').collapse('hide');
        }
        else if (response.errorModel == null && response.errorModel == "") {
          toastr.error(response.errorMessage);
        }
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      },
      error: function (response) {
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        //error occurred
      }
    });
  }
});

$("#btnUserManagementCancel").click(function () {
  ResetForm();
  loadKendoGrid();
  $('#collapseExample').collapse('hide');
});

$("#clearFilters").click(function () {
  $("#gridUserManagement").data("kendoGrid").dataSource.filter([]);
});

var loadKendoGrid = function () {
  $("#gridUserManagement").kendoGrid({
    dataSource: {
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_GETALLLIST_USERMANAGEMENT_URL,
          beforeSend: CommonJS.BeforeSendAjaxCall,
        },
        parameterMap: function (options) {
          return "parametersJson=" + JSON.stringify(options) + "&userType=" + UserMgmt.userType;
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
            UserMasterHashId: { type: "string" },
            FirstName: { type: "string" },
            LastName: { type: "string" },
            EmailAddress: { type: "string" },
            Address: { type: "string" },
            ZipCode: { type: "string" },
            Status: { type: "string" },
            DefaultLexicon: { type: "string" },
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
      refresh: false,
      buttonCount: 5,
      input: true
    },
    dataBound: masterGridDataBound,
    columns: [
          {
            field: "Designation",
            title: UserMgmt.title,
            width: 200,
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
          field: "FirstName",
          title: UserMgmt.firstName,
          width: 130,
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
          field: "LastName",
          title: UserMgmt.lastName,
          width: 130,
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
          field: "EmailAddress",
          title: UserMgmt.emalId,
          width: 150,
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
          field: "Address",
          title: UserMgmt.address,
          width: 200,
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
          field: "ZipCode",
          title: UserMgmt.zipCode,
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
          field: "Status",
          title: UserMgmt.status,
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
           field: "CompanyName",
           title: UserMgmt.comapnyName,
           width: 200,
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
           template: "<div align=center># if (DefaultLexicon > 0) {# <a href='javascript:' class='count-lexiconfollow' data-toggle='tooltip' title='Lexicon To Follow' onclick='FetchLexiconToFollow(\"#=UserMasterHashId #\")'>#=DefaultLexicon#</a> #} else { # <span class='count-lexiconfollow' data-toggle='tooltip' title='Lexicon To Follow'>#=DefaultLexicon#</span> # }#</div>",
           title: UserMgmt.defaultLexicon,
           width: 150,
           filterable: false,
           sortable: false
         },
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditUser(\"#=UserMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
          width: 50
        },
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteUser(\"#=UserMasterHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
          width: 50
        }

    ]
  });
}

function FetchLexiconToFollow(UserMasterHashId) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  window.location.href = $('#DefaultLexiconURL')[0].href + "?userMasterHashId=" + UserMasterHashId + "&userType=Customer";
}

function EditUser(UserMasterHashId) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  ResetForm();

  GetUserBasedOnHash(UserMasterHashId);
  $('#collapseExample').collapse('show');
  $(".form-group").removeClass('has-error');
  ResetvalidateForm('userManagementForm');
  moveDivTop("topPanel");
}

function DeleteUser(UserHashId) {
  bcmStrategyAlerts({
    type: "confirm",
    messageText: UserMgmt.userType.toLowerCase() == "customer" ? UserMgmt.deleteCustomerMessage : UserMgmt.deleteAdminMessage,
    headerText: UserMgmt.deletePopupHeader,
    alertType: alertTypes.Info
  }).done(function (e) {
    if (e) {
      CommonJS.SetPageLoader(PageLoaderActivity.SHOW);

      var UserMasterHashId = UserHashId;

      $.ajax({
        url: BCMConfig.API_USERMANAGEMENT_DELETE + "?userHashId=" + UserMasterHashId + "&userType=" + UserMgmt.userType,
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
            $('#collapseExample').collapse('hide');
          }
          CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        },
        error: function (response) {
          CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
          //error occurred
        }
      });
    }
  });
}

function selectActiveStatus(status) {
  if (status == "Active") {
    $('#active').prop("checked", true);
  }
  else {
    $('#inactive').prop("checked", true);
  }
}

function masterGridDataBound(e) {
  var grid = e.sender;
  if (grid.dataSource.total() == 0) {
    var colCount = grid.columns.length;
    $(e.sender.wrapper)
        .find('tbody').first()
        .append('<tr class="kendo-data-row" style="text-align: center;"><td colspan="' + colCount + '" class="no-data">' + UserMgmt.noDataFound + '</td></tr>');
  }
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
};

var ResetForm = function () {
  initViewModel();
  ValidateForm('userManagementForm', '');
  ResetvalidateForm('userManagementForm');
  kendo.bind($("#userManagementForm"), viewModelUserManagement);
  $("#activeStatus").hide();
  //
}

var initViewModel = function () {
  viewModelUserManagement = kendo.observable({
    UserMasterHashId: "",
    FirstName: "",
    MiddleName: "",
    LastName: "",
    EmailAddress: "",
    Address: "",
    CountryId: "",
    State: "",
    City: "",
    ZipCode: "",
    Active: false,
    CountryMasterHashId: "",
    UserType: UserMgmt.userType,
    Status: "",
    CompanyName: "",
    Designation: "",
    DefaultLexicon: ""
  });
};

function GetUserBasedOnHash(UserMasterHashId) {

  $.ajax({
    url: BCMConfig.API_GETUSER_BASED_ON_HASH + "?userHashId=" + UserMasterHashId,
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

  viewModelUserManagement.UserMasterHashId = response.userMasterHashId
  viewModelUserManagement.FirstName = response.firstName
  viewModelUserManagement.MiddleName = response.middleName;
  viewModelUserManagement.LastName = response.lastName;
  viewModelUserManagement.EmailAddress = response.emailAddress;
  viewModelUserManagement.CountryMasterHashId = response.countryMasterHashId;
  viewModelUserManagement.Address = response.address;
  viewModelUserManagement.State = response.state;
  viewModelUserManagement.City = response.city;
  viewModelUserManagement.ZipCode = response.zipCode;
  viewModelUserManagement.Status = response.status;
  viewModelUserManagement.OldStatus = response.status;
  viewModelUserManagement.CompanyName = response.companyName;
  viewModelUserManagement.Designation = response.designation;
  viewModelUserManagement.DefaultLexicon = response.defaultLexicon;
  selectActiveStatus(response.status);
  $("#activeStatus").show();
  kendo.bind($("#userManagementForm"), viewModelUserManagement);

  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}
