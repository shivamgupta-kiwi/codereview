var viewModelRegistration;
$(document).ready(function () {
    GetCountryDDList();
    ResetForm();
});

$("#btnSave").click(function () {
    if ($("#registrationForm").valid()) {
        CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
        ResetvalidateForm('registrationForm');

        $.ajax({
            url: BCMConfig.API_REGISTRATION,
            method: 'POST',
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(viewModelRegistration),
            processdata: true,
            success: function (response) { //call successfull
                if (response.errorModel != null && response.errorModel != "") {
                    //check validations for all controls
                    ValidateForm('registrationForm', response.errorModel);
                }
                if (response.data) {
                  showPageAlert(response.errorMessage, alertTypes.Success, redirect);
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
});

$("#btnCancel").click(function () {
  window.location.href = redirectHome;
});

var ResetForm = function () {
    initViewModel();
    ValidateForm('registrationForm', '');
    ResetvalidateForm('registrationForm');
    kendo.bind($("#registrationForm"), viewModelRegistration);
}

var initViewModel = function () {
    viewModelRegistration = kendo.observable({
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
        UserType: userType,
        CompanyName:"",
        Designation : ""
    });
};

