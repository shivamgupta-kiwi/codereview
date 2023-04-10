var viewModelGlobalSetting;
$(document).ready(function () {
		ResetForm();
		GetGlobalSettingData();
});

var ResetForm = function () {
	initViewModel();
	ValidateForm('globalSettingForm', '');
	ResetvalidateForm('globalSettingForm');
	kendo.bind($("#globalSettingForm"), viewModelGlobalSetting);
}

var initViewModel = function () {
	viewModelGlobalSetting = kendo.observable({
		SMTPDetails: ""
	});
};

var GetGlobalSettingData = function () {
	$.ajax({
		url: BCMConfig.API_GETGLOBALSETTINGS_API,
		beforeSend: CommonJS.BeforeSendAjaxCall,
		dataType: "json",
		contentType: 'application/json; charset=utf-8',
		method: 'GET',
		processdata: true,
		success: function (response) {
			if (response) {
				var globalSettingsDetail = response.data;
				viewModelGlobalSetting.SMTPDetails = globalSettingsDetail.smtpDetails;
				kendo.bind($("#globalSettingForm"), viewModelGlobalSetting);
			}
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
		},
		error: function (data) {
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
		}
	});
}

$("#btnGlobalSettingSave").click(function () {
	if ($("#globalSettingForm").valid()) {
		CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
		ResetvalidateForm('globalSettingForm');

		$.ajax({
			url: BCMConfig.API_SAVEGLOBALSETTING_API,
			beforeSend: CommonJS.BeforeSendAjaxCall,
			method: 'POST',
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			data: JSON.stringify(viewModelGlobalSetting),
			processdata: true,
			success: function (response) { //call successfull
				if (response.errorModel != null && response.errorModel != "") {
					ValidateForm('globalSettingForm', response.errorModel);
				}
				if (response.data) {
					toastr.info(response.errorMessage);
					window.setTimeout(function () {
						window.location.href = redirectHome;
					}, 2000);

					GetGlobalSettingData();
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


$("#btnGlobalSettingCancel").click(function () {
	window.location.href = redirectHome;
});
