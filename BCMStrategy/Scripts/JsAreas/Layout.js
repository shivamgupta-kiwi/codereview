var LayoutJS = {
    logout: function () {
        CommonJS.RemoveStorageItem('access_token');
        CommonJS.RemoveStorageItem('refresh_token');
        CommonJS.RemoveStorageItem('token_type');
        CommonJS.RemoveStorageItem('userHashId');
        CommonJS.RemoveStorageItem('userName');
        CommonJS.RemoveStorageItem('expire_time');
        CommonJS.RemoveStorageItem('userFullName');
        $.ajax({
            url: LogoutURL,
            type: 'POST',
            contentType: 'application/json;',
            success: function (redirectUrl) {
                window.location.href = LogoutSuccessURL;
            }
        });
    },
    
    writeLogOutAuditLog: function() {

    $.ajax({
      url: BCMConfig.API_LOGOUT_AUDIT,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      processdata: true,
      success: function (response) {
        if(response === true)
        {
          LayoutJS.logout();
        }
      }
    });
  }
}

$(document).ready(function () {
  $("#btnLogout").click(function () { LayoutJS.writeLogOutAuditLog(); });
    $("#userName").html(CommonJS.GetStorageItem("userFullName"));
    //if (CommonJS.GetStorageItem('access_token') == null) {
    //    LayoutJS.logout();
    //}
});