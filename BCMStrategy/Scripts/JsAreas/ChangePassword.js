var viewModelChangePassword;
$(document).ready(function () {
  ResetForm();
});

$("#btnChangePasswordSave").click(function () {
  if ($("#changePasswordForm").valid()) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetvalidateForm('changePasswordForm');

    $.ajax({
      url: BCMConfig.API_CHANGEPASSWORD_URL,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(viewModelChangePassword),
      processdata: true,
      success: function (response) { //call successfull
        if (response.errorModel != null && response.errorModel != "") {
          ValidateForm('changePasswordForm', response.errorModel);
        }
        if (response.data) {
          toastr.info(response.errorMessage);
          ResetForm();
        }
        else if (response.errorModel == null && response.errorModel == "") { toastr.error(response.errorMessage); }
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      },
      error: function (response) {
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      }
    });
  }
});

$("#btnChangePasswordCancel").click(function () {
  window.location.href = redirectHome;
});

var ResetForm = function () {
  initViewModel();
  ValidateForm('changePasswordForm', '');
  ResetvalidateForm('changePasswordForm');
  kendo.bind($("#changePasswordForm"), viewModelChangePassword);
}

var initViewModel = function () {
  viewModelChangePassword = kendo.observable({
    OldPassword: "",
    NewPassword: "",
    ConfirmPassword: ""
  });
};
