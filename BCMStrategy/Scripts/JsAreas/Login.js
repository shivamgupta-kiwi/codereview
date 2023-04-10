
var Login = {
  logincall: function () {
    $("#errorMessage").html("");

    $.ajax({
      url: LoginUrl,
      dataType: "json",
      contentType: 'application/x-www-form-urlencoded',
      method: "POST",
      data: {
        Username: $("#username").val().toLowerCase(),
        Password: $("#password").val()
      },
      success: function (response) {
        if (response.Success) {
          CommonJS.SetStorageItem('token_type', response.Result.TokenType);
          CommonJS.SetStorageItem('access_token', response.Result.AccessToken);
          CommonJS.SetStorageItem('refresh_token', response.Result.RefreshToken);
          CommonJS.SetStorageItem('userHashId', response.Result.UserMasterHashId);
          CommonJS.SetStorageItem('userName', response.Result.UserName);
          CommonJS.SetStorageItem('expire_time', response.Result.Expires)
          CommonJS.SetStorageItem('userFullName', response.Result.UserFullName)
          if (returnURL == "") {
            window.location = DashBoardUrl;
          } else {
            window.location = returnURL;
          }

        } else {
            $("#password").val('');
          CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
          $("#btnLogin").attr("disabled", false);
          $("#errorMessage").html(response.ResponseMessage).show();
        }
      },
      error: function (res) {
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        CommonJS.HandleErrorStatus(res.status); ////Get API status 404,405...
        $("#btnLogin").attr("disabled", false);
        //$("#errorMessage").html(res.responseText).show();
      }
    });
  }
};
////Submit login form
$("#loginForm").submit(function (event) {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  Login.logincall();
  event.preventDefault();
});