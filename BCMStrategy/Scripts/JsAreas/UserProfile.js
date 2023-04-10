var viewModelUserProfile;
$(document).ready(function () {
  $.when(GetCountryDDList()).then(function (x) {
    ResetForm();
    GetUserProfile();
  });
});

var GetUserProfile = function () {
  $.ajax({
    url: BCMConfig.API_GETPROFILEDETAIL_API,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response) {
        var userDetail = response.data;
        viewModelUserProfile.UserMasterHashId = userDetail.userMasterHashId;
        viewModelUserProfile.FirstName = userDetail.firstName;
        viewModelUserProfile.MiddleName = userDetail.middleName;
        viewModelUserProfile.LastName = userDetail.lastName;
        viewModelUserProfile.EmailAddress = userDetail.emailAddress;
        viewModelUserProfile.Address = userDetail.address;
        viewModelUserProfile.State = userDetail.state;
        viewModelUserProfile.City = userDetail.city;
        viewModelUserProfile.ZipCode = userDetail.zipCode;
        viewModelUserProfile.CountryMasterHashId = userDetail.countryMasterHashId;
        viewModelUserProfile.Status = userDetail.status;
        kendo.bind($("#userProfileForm"), viewModelUserProfile);
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (data) {
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    }
  });
}

$("#btnUserProfileCancel").click(function () {
  window.location.href = redirectHome;
});

$("#btnUserProfileSave").click(function () {
  if ($("#userProfileForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('userProfileForm');

    $.ajax({
      url: BCMConfig.API_SAVEPROFILEDETAIL_API,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelUserProfile),
      processdata: true,
      success: function (response) { //call successfull
        if (response.errorModel != null && response.errorModel != "") {
          ValidateForm('userProfileForm', response.errorModel);
        }
        if (response.data) {
          toastr.info(response.errorMessage);
          window.setTimeout(function () {
            window.location.href = dashBoardURL;
          }, 2000);
          
          GetUserProfile();
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

var ResetForm = function () {
  initViewModel();
  ValidateForm('userProfileForm', '');
  ResetvalidateForm('userProfileForm');
  kendo.bind($("#userProfileForm"), viewModelUserProfile);
}

var initViewModel = function () {
  viewModelUserProfile = kendo.observable({
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
    CountryMasterHashId: "",
    Status: ""
  });
};