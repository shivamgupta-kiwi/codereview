var viewModelForgotPassword;
$(document).ready(function () {
  
  ResetForm();
});

var ForgotPassword = {
  fpCall: function () {
    $("#errorMessage").html("");
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('forgotPasswordForm');

    $.ajax({
      url: BCMConfig.API_FORGOT_PASSWORD,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelForgotPassword),
      processdata: true,
      success: function (response) { //call successfull
        if (response.errorModel != null && response.errorModel != "") {
          //check validations for all controls
          ValidateForm('forgotPasswordForm', response.errorModel);
        }
        else {
          $("#errorMessage").html(response.errorMessage).show();
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
  ValidateForm('forgotPasswordForm', '');
  ResetvalidateForm('forgotPasswordForm');
  kendo.bind($("#forgotPasswordForm"), viewModelForgotPassword);
}

var initViewModel = function () {
  viewModelForgotPassword = kendo.observable({
    EmailAddress: ""
  });
};

////Sumit login form
$("#forgotPasswordForm").submit(function (event) {
  ForgotPassword.fpCall();
  event.preventDefault();
});