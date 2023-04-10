var viewModelSetPassword;
$(document).ready(function () {
  ResetForm();
});
var Activation = function (activationCode) {
  IsActivationCode(activationCode);
}



var IsActivationCode = function (activationCode) {

  $.ajax({
    url: BCMConfig.API_ACTIVATIONCODEEXIST,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    data: { activationCode: activationCode },
    processdata: true,
    success: function (response) {
      if (!response.data) {
        window.location.href = ExpiredLink;
      }
      else {
        window.location.href = SetPasswordLink + "?activationCode=" + activationCode;
      }
    },
    error: function (data) {
    }
  });
}


var UpdatePassword = function () {
  if ($("#setPasswordForm").valid()) {
    $("#errorMessage").html("");
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('setPasswordForm');
    $.ajax({
      url: BCMConfig.API_SETPASSWORD,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelSetPassword),
      processdata: true,
      success: function (response) { //call successfull
        if (response.errorModel != null && response.errorModel != "") {
          //check validations for all controls
          ValidateForm('setPasswordForm', response.errorModel);
        }
        else {
          if (response.data) {
            showPageAlert(response.errorMessage, SuccessTitle, SuccessURL);
          }
          else {
            $("#errorMessage").html(response.errorMessage).show();
          }
          ResetForm();
        }
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      },
      error: function (response) {
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        //error occurred
      }
    });
  }
};

var ResetForm = function () {
  initViewModel();
  ValidateForm('setPasswordForm', '');
  ResetvalidateForm('setPasswordForm');
  kendo.bind($("#setPasswordForm"), viewModelSetPassword);
}

var initViewModel = function () {
  viewModelSetPassword = kendo.observable({
    Password: "",
    ReEnterPassword: "",
    HashKey: activationCode
  });
};

$("#btnReset").click(function () {
  UpdatePassword();
  return false;
});

