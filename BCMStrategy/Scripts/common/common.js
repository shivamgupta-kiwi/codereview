//// Base API URL of the Application
var APIServerURL = 'http://192.168.0.98:8454/';

toastr.options = {
	"closeButton": true, // true/false
	"debug": false, // true/false
	"newestOnTop": false, // true/false
	"progressBar": false, // true/false
	"positionClass": "toast-bottom-right", // toast-top-right / toast-top-left / toast-bottom-right / toast-bottom-left
	"preventDuplicates": false, //true/false
	"onclick": null,
	"showDuration": "300", // in milliseconds
	"hideDuration": "1000", // in milliseconds
	"timeOut": "5000", // in milliseconds
	"extendedTimeOut": "1000", // in milliseconds
	"showEasing": "swing",
	"hideEasing": "linear",
	"showMethod": "fadeIn",
	"hideMethod": "fadeOut"
}
var BCMConfig = {
	//Registration API's
	API_REGISTRATION: APIServerURL + "api/Registration/SaveRegistration",

	//Get My profile details
	API_GETPROFILEDETAIL_API: APIServerURL + "api/UserProfile/GetProfileDetail",

	//Save User profile details
	API_SAVEPROFILEDETAIL_API: APIServerURL + "api/UserProfile/SaveUserProfile",

	//Country API's
	API_COUNTRY_DROPDOWNLIST: APIServerURL + "api/Country/GetDropdownCountryList",

	//Forgot Password API's
	API_FORGOT_PASSWORD: APIServerURL + "api/ForgotPassword/Reset",

	//Activation Code Exist API's
	API_ACTIVATIONCODEEXIST: APIServerURL + "api/Registration/ActivationCodeExist",

	//Set Password Or Update Password API's
	API_SETPASSWORD: APIServerURL + "api/Registration/SetPassword",

	//Import File URL For State Head
	API_HEADSTATEIMPORTVALIDATE_URL: APIServerURL + "api/StateHead/ValidateCSVRecord",

	//Import File URL For State Head
	API_HEADSTATEIMPORT_URL: APIServerURL + "api/StateHead/ImportCSVRecord",

	//Get all State Head List
	API_STATEHEADGETALLDATA_URL: APIServerURL + "api/StateHead/GetAllStateHeadList",

	//StateHead Post API's
	API_STATEHEAD_UPDATE: APIServerURL + "api/StateHead/UpdateStateHead",

	//StateHead Delete API's
	API_STATEHEAD_DELETE: APIServerURL + "api/StateHead/DeleteStateHead",

	//Change Password API's
	API_CHANGEPASSWORD_URL: APIServerURL + "api/UserProfile/ChangePassword",

	//Add/Update Users API's
	API_USERMANAGEMENTUPDATE_URL: APIServerURL + "api/UserManagement/UpdateUser",

	//Get the List of Users API's
	API_GETALLLIST_USERMANAGEMENT_URL: APIServerURL + "api/UserManagement/GetAllUserList",

	//Delete User API's
	API_USERMANAGEMENT_DELETE: APIServerURL + "api/UserManagement/DeleteUser",

	//Import File URL For Institution Types
	API_INSTTYPESIMPORTVALIDATE_URL: APIServerURL + "api/InstitutionTypes/ValidateCSVRecord",

	//Import File URL For Institution Types
	API_INSTTYPESIMPORT_URL: APIServerURL + "api/InstitutionTypes/ImportCSVRecord",

	//Get all Institution Types List
	API_INSTTYPESGETALLDATA_URL: APIServerURL + "api/InstitutionTypes/GetAllInstitutionTypesList",

	//Institution Types Post API's
	API_INSTTYPES_UPDATE: APIServerURL + "api/InstitutionTypes/UpdateInstitutionTypes",

	//Institution Types Delete API's
	API_INSTTYPES_DELETE: APIServerURL + "api/InstitutionTypes/DeleteInstitutionTypes",

	//Institution Type DropDownlist API's
	API_INSTITUTIONTYPE_DROPDOWNLIST: APIServerURL + "api/InstitutionTypes/GetDropdownInstitutionTypeList",

	//Import File URL For Institution Types
	API_INSTTYPESIMPORTVALIDATE_URL: APIServerURL + "api/InstitutionTypes/ValidateCSVRecord",

	//Import File URL For Institution Types
	API_INSTTYPESIMPORT_URL: APIServerURL + "api/InstitutionTypes/ImportCSVRecord",

	//Institution Insert/Update API's
	API_INSTITUTION_UPDATE: APIServerURL + "api/Institution/UpdateInstitutions",

	//Get the List of Institutions API's
	API_GETALLLIST_INSTITUTIONS_URL: APIServerURL + "api/Institution/GetAllInstitutionsList",

	//Institutions Delete API's
	API_INSTITUTIONS_DELETE: APIServerURL + "api/Institution/DeleteInstitution",

	//Institutions Import API's
	API_INSTITUTIONS_IMPORT_URL: APIServerURL + "api/Institution/ImportInstitutionsCSVRecord",

	// Validate File URL For Institutions
	API_INSTITUTIONS_VALIDATE_URL: APIServerURL + "api/Institution/ValidateInstitutionCSVRecord",

	//Update Policy Makers API
	API_POLICYMAKERS_UPDATE: APIServerURL + "api/PolicyMakers/UpdatePolicyMaker",

	//Delete Policy Makers API
	API_POLICYMAKERS_DELETE: APIServerURL + "api/PolicyMakers/DeletePolicyMaker",

	//Get All Policy Makers API
	API_POLICYMAKERS_GETALL_List: APIServerURL + "api/PolicyMakers/GetAllPolicyMakerList",

	//Get Designation DropDownlist API
	API_GETDESIGNATION_LIST: APIServerURL + "api/PolicyMakers/GetDropdownDesignationList",

	// Validate File URL For Policy Makers
	API_POLICYMAKERS_VALIDATE_URL: APIServerURL + "api/PolicyMakers/ValidatePolicyMakersCSVRecord",

	//Policy Makers Import API's
	API_POLICYMAKERS_IMPORT_URL: APIServerURL + "api/PolicyMakers/ImportPolicyMakersCSVRecord",

	//Get the List of International Organization API's
	API_GETALLLIST_INTERNATION_ORGANIZATION_URL: APIServerURL + "api/InternationalOrganization/GetAllInternationalOrganizationList",

	//International Organization Insert/Update API's
	API_INTERNATIONALORGANIZATION_UPDATE: APIServerURL + "api/InternationalOrganization/UpdateInternationalOrganization",

	//International Organization Delete API's
	API_INTERNATIONAL_ORG_DELETE: APIServerURL + "api/InternationalOrganization/DeleteInternationalOrganization",

	//Import File URL For International Organization
	API_INTERNATIONALORGIMPORT_URL: APIServerURL + "api/InternationalOrganization/ImportInternationalOrgCSVRecord",

	//Validate File URL For International Organization
	API_INTERNATIONALORGVALIDATE_URL: APIServerURL + "api/InternationalOrganization/ValidateInternationalOrgCSVRecord",

	// Get scrapped URL data
	API_GETALLLIST_SCRAPPED_DATA: APIServerURL + "api/ScrappingProcess/GetAllScrappedList",

	// Get scrapped URL data
	API_GETALLLIST_SCRAPPED_SUMMARY_DATA: APIServerURL + "api/ScrappingProcess/GetAllScrappedSummaryList",

	// Get Web Link Management Data
	API_GETWEBLINK_MANAGEMENT_DATA: APIServerURL + "api/WebLink/GetAllWebLinkManagementDDL",

	//Individual Person API's
	API_INDIVIDUAL_PERSON_DROPDOWNLIST: APIServerURL + "api/WebLink/GetAllIndividualPersons",

	//Entity FullName API's
	API_ENTITYFULLNAME_DROPDOWNLIST: APIServerURL + "api/WebLink/GetAllEntityFullName",

	//Get the List of WebLinks
	API_GETALLLIST_WEBLINKS_URL: APIServerURL + "api/WebLink/GetAllWebLinks",

	//Get the List of WebLinks
	API_POST_WEBLINKS_URL: APIServerURL + "api/WebLink/UpdateWebLink",

	//Web Link Delete API's
	API_WEBLINKS_DELETE: APIServerURL + "api/WebLink/DeleteWebLink",

	//API GETWEBLINK BASED ON HASH
	API_GETWEBLINK_BASED_ON_HASH: APIServerURL + "api/WebLink/GetWebLinkByHashId",

	//Lexicon Type DropDownlist API's
	API_LEXICONTYPE_DROPDOWNLIST: APIServerURL + "api/LexiconType/GetDropdownLexiconTypeList",

	//Lexicon Insert/Update API's
	API_LEXICON_UPDATE: APIServerURL + "api/Lexicon/UpdateLexicon",

	//Get the List of Institutions API's
	API_GETALLLIST_LEXICON_URL: APIServerURL + "api/Lexicon/GetAllLexiconList",

	//Institutions Delete API's
	API_LEXICON_DELETE: APIServerURL + "api/Lexicon/DeleteLexicon",

	//Get the List of lexicon type API's
	API_GETALLLIST_LEXICONTYPE_URL: APIServerURL + "api/LexiconType/GetAllLexiconTypeList",

	////Lexicon Import API's
	//API_LEXICON_IMPORT_URL: APIServerURL + "api/Lexicon/ImportLexiconCSVRecord",

	// Validate File URL For lexicon
	API_LEXICON_VALIDATE_URL: APIServerURL + "api/Lexicon/ValidateLexiconCSVRecord",

	// Get Legislator Management Data
	API_GETLEGISLATOR_MANAGEMENT_DATA: APIServerURL + "api/Legislator/GetAllLegislatorManagementDDL",

	//Get the List of Legislator
	API_GETALLLIST_Legislators_URL: APIServerURL + "api/Legislator/GetAllLegislators",

	//Legislator Delete API's
	API_LEGISLATOR_DELETE: APIServerURL + "api/Legislator/DeleteLegislator",

	//Get the List of Legislator
	API_POST_LEGISLATOR_URL: APIServerURL + "api/Legislator/UpdateLegislator",

	//Edit Legislator
	API_GETLEGISLATOR_BASED_ON_HASH: APIServerURL + "api/Legislator/GetLegislatorByHashId",

	//website Type DropDownlist API's
	API_WEBSITETYPE_DROPDOWNLIST: APIServerURL + "api/WebsiteTypes/GetDropdownWebsiteTypesList",

	//Get the List of MetadataTypes API's
	API_GETALLLIST_METADATATYPES_URL: APIServerURL + "api/MetadataTypes/GetAllMetadataTypesList",

	//Metadata Types Insert/Update API's
	API_METADATATYPES_UPDATE: APIServerURL + "api/MetadataTypes/UpdateMetadataTypes",

	//Metadata Types Delete API's
	API_METADATATYPES_DELETE: APIServerURL + "api/MetadataTypes/DeleteMetadataTypes",

	//Metadata Import API's
	API_METADATATYPES_IMPORT_URL: APIServerURL + "api/MetadataTypes/ImportMetadataTypesCSVRecord",

	//Metadata Type DropDownlist API's
	API_METADATATYPES_DROPDOWNLIST: APIServerURL + "api/MetadataTypes/GetDropdownMetadataTypesList",

	//Get the List of activity type API's
	API_GETALLLIST_ACTIVITYTYPE_URL: APIServerURL + "api/ActivityType/GetAllActivityTypeList",

	//Activity Types Insert/Update API's
	API_ACTIVITYTYPE_UPDATE: APIServerURL + "api/ActivityType/UpdateActivityType",

	//Activity Types Delete API's
	API_ACTIVITYTYPE_DELETE: APIServerURL + "api/ActivityType/DeleteActivityType",

	//Get the List of metadata phrases API's
	API_GETALLLIST_METADATAPHRASES_URL: APIServerURL + "api/MetadataPhrases/GetAllMetadataPhrasesList",

	//Metadata Types Insert/Update API's
	API_METADATAPHRASES_UPDATE: APIServerURL + "api/MetadataPhrases/UpdateMetadataPhrases",

	//Metadata Types Delete API's
	API_METADATAPHRASES_DELETE: APIServerURL + "api/MetadataPhrases/DeleteMetadataPhrases",

	//Get the List of metadata noun plus verb API's
	API_GETALLLIST_METADATANOUNPLUSVERB_URL: APIServerURL + "api/MetadataNounplusVerb/GetAllMetadataNounplusVerbList",

	//Metadata noun plus verb Insert/Update API's
	API_METADATANOUNPLUSVERB_UPDATE: APIServerURL + "api/MetadataNounplusVerb/UpdateMetadataNounplusVerb",

	//Metadata noun plus verb Delete API's
	API_METADATANOUNPLUSVERB_DELETE: APIServerURL + "api/MetadataNounplusVerb/DeleteMetadataNounplusVerb",

	//dropdown list for dynamic table of noun plus verb API's
	API_METADATADYNAMICNOUNPLUSVERB_DROPDOWNLIST: APIServerURL + "api/MetadataNounplusVerb/GetDropdownMetadataDynamicNounplusVerbList",

	//dropdown list for dynamic table of noun plus verb API's
	API_GET_ALL_CUSTOMER: APIServerURL + "api/PrivilegeApi/GetAllCustomer",

	//Get Ids of LexiconIssues
	GET_LEXICON_TERM_HASH_IDS_BASED_ON_LEXICONTYPE: APIServerURL + "api/PrivilegeApi/GetLexiconTermHashIdsBasedOnLexiconType",

	//Post Lexicon Access Privilege
	API_POST_LEXICON_ACCESS: APIServerURL + "api/PrivilegeApi/UpdateLexiconAccessPrivilege",

	//Get Lexicon Access Customer List
	GET_LEXICON_ACCESS_CUSTOMER_LIST: APIServerURL + "api/PrivilegeApi/GetAllLexiconAccessCustomer",

	//Get the List of Audit Log API's
	API_GETALLLIST_AUDITLOG_URL: APIServerURL + "api/AuditLog/GetAllAuditLogList",

	//dropdown list for user API's
	API_USER_DROPDOWNLIST: APIServerURL + "api/UserAccessRights/GetDDUserList",

	//Get the List of Menu API's
	API_GETALLLIST_MENU_URL: APIServerURL + "api/UserAccessRights/GetAllMenuList",

	//dropdown list for user API's
	API_SCHEDULERFREQUENCY_DROPDOWNLIST: APIServerURL + "api/Scheduler/GetDDSchedulerFrequencyList",

	//Metadata Types Insert/Update API's
	API_SCHEDULER_UPDATE: APIServerURL + "api/Scheduler/UpdateScheduler",

	//Metadata Types Delete API's
	API_SCHEDULER_DELETE: APIServerURL + "api/Scheduler/DeleteScheduler",

	//Get the List of metadata noun plus verb API's
	API_GETALLLIST_SCHEDULER_URL: APIServerURL + "api/Scheduler/GetAllSchedulerList",

	//Get LexiconIds Based on Hash
	API_GET_LEXICON_SELECTED_IDS_BASED_ON_CUSTOMER: APIServerURL + "api/PrivilegeApi/GetLexiconIdsBasedOnCustomer",

	//dropdown list for audit table API's
	API_AUDITTABLE_DROPDOWNLIST: APIServerURL + "api/AuditLog/GetDDAuditTableList",

	//API_DASHBOARD_GET_LEXICONTERMS
	API_DASHBOARD_GET_LEXICONTERMS: APIServerURL + "api/Dashboard/GetAllDashboardLexiconTerms",

	//API_GET_LEXICON_WISE_ACTIONTYPE_VALUES
	API_GET_CHART_LEXICON_VALUES: APIServerURL + "api/Dashboard/GetChartLexiconValues",

	//API_GET_LEXICON_WISE_ACTIONTYPE_VALUES
	API_POST_CHART_LEXICON_VALUES: APIServerURL + "api/Dashboard/PostChartLexiconValues",

  API_POST_UPDATE_LEXICON_DEFAULTFILTER: APIServerURL + "api/Dashboard/UpdateLexiconDefaultFilter",

	//API_GET_VALUES_BASED_ON_LEXICON_TYPE: APIServerURL + "api/Dashboard/GetChartLexiconValues"
	API_GET_ACTIVITY_TYPE_LIST_URL: APIServerURL + "api/Dashboard/GetActivityTypeValues",

	//dropdown list for audit table API's
	API_AUDITTABLE_DROPDOWNLIST: APIServerURL + "api/AuditLog/GetDDAuditTableList",

	//dropdown list for audit table API's
	API_ACTIVITYTYPE_DROPDOWNLIST: APIServerURL + "api/ActivityType/GetDDActivityTypeList",

	//GET ACTIVITYTYPE BASED ON ACTION TYPE
	API_POST_ACTIVITYTYPE_BASED_ON_ACTION_TYPE: APIServerURL + "api/WebLink/GetActivityTypeBasedOnActionType",

	//GetActionTypeBasedOnWebsiteType
	API_POST_ACTION_TYPE_BASED_ON_WEBSITETYPE: APIServerURL + "api/WebLink/GetActionTypeBasedOnWebsiteType",

	//Get LexiconIds Based on Hash
	API_GET_LEXICON_SELECTED_IDS_BASED_ON_CUSTOMER: APIServerURL + "api/PrivilegeApi/GetLexiconIdsBasedOnCustomer",

	//Get LexiconIds Based on Hash
	API_GET_PROCESSDETAIL_BASED_ON_SCHEDULER: APIServerURL + "api/Scheduler/GetProcessDetailBasedOnScheduler",

	//API GET POLICY MAKER BASED ON HASH
	API_GETPOLICYMAKER_BASED_ON_HASH: APIServerURL + "api/PolicyMakers/GetPolicyMakerByHashId",

	//API GET LEXICON BASED ON HASH
	API_GETLEXICON_BASED_ON_HASH: APIServerURL + "api/Lexicon/GetLexiconByHashId",

	//API GET METADATA TYPE BASED ON HASH
	API_GETMETADATA_TYPE_BASED_ON_HASH: APIServerURL + "api/MetadataTypes/GetMetadataTypeByHashId",

	//API GET ACTIVITY TYPE BASED ON HASH
	API_GETACTIVITY_TYPE_BASED_ON_HASH: APIServerURL + "api/ActivityType/GetActivityTypeByHashId",

	//API GET METADATA PHRASES BASED ON HASH
	API_GETMETADATA_PHRASES_BASED_ON_HASH: APIServerURL + "api/MetadataPhrases/GetMetadataPhrasesByHashId",

	//API GET METADATA NOUN PLUS VERB BASED ON HASH
	API_GETMETADATA_NOUN_PLUS_VERB_BASED_ON_HASH: APIServerURL + "api/MetadataNounplusVerb/GetMetadataNounPlusVerbByHashId",

	//API GET INSTITUTION TYPES BASED ON HASH
	API_GETINSTITUTION_TYPE_BASED_ON_HASH: APIServerURL + "api/InstitutionTypes/GetInstitutionTypeByHashId",

	//API GET INSTITUTION BASED ON HASH
	API_GETINSTITUTION_BASED_ON_HASH: APIServerURL + "api/Institution/GetInstitutionByHashId",

	//API GETSTATE HEAD BASED ON HASH
	API_GETSTATE_HEAD_BASED_ON_HASH: APIServerURL + "api/StateHead/GetStateHeadByHashId",

	//API GET INTERNATIONAL ORGANIZATION BASED ON HASH
	API_GETINTERNATIONAL_ORG_BASED_ON_HASH: APIServerURL + "api/InternationalOrganization/GetInternationalOrgByHashId",

	//API GETUSER BASED ON HASH
	API_GETUSER_BASED_ON_HASH: APIServerURL + "api/UserManagement/GetUserByHashId",

	//API GET SCHEDULER BASED ON HASH
	API_GETSCHEDULER_BASED_ON_HASH: APIServerURL + "api/Scheduler/GetSchedulerByHashId",

	//API GET SCHEDULER BASED ON HASH
	API_AUTHENTICATE_USER_FOR_VIRTUAL_DASHBOARD: APIServerURL + "api/Dashboard/AuthenticateUserForVirtualDashboard",

	//API GET SEARCHABLE PDF DATA
	API_GETSEARCHABLE_PDF_DATA: APIServerURL + "api/SearchablePDF/GetSearchablePDfDatabasedOnLexicon",

	//API GET SEARCHABLE PDF DATA FOR LINE CHART
	API_GETSEARCHABLE_PDF_DATA_FOR_LINECHART: APIServerURL + "api/SearchablePDF/GetSearchablePDfChartDatabasedOnLexicon",

	//API GET SEARCHABLE PDF DATA FOR LINE MONTH WISE CHART
	API_GETSEARCHABLE_PDF_DATA_FOR_LINEMONTHWISECHART: APIServerURL + "api/SearchablePDF/GetSearchablePDfMonthWiseChartDatabasedOnLexicon",
	
	//Lexicon API's
	API_LEXICON_DROPDOWNLIST: APIServerURL + "api/SearchablePDF/GetAllLexicons",

  // Customer Audit API's
  API_CUSTOMERAUDIT_DROPDOWNLIST: APIServerURL + "api/AuditLog/GetDDCustomerTableList",

  //Get the List of Audit Log API's
  API_GETALL_CUSTOMER_AUDITLOGLIST: APIServerURL + "api/AuditLog/GetAllCustomerAuditLogList",

  API_LOGOUT_AUDIT: APIServerURL + "api/AuditLog/WriteAudit",

	//Get Global Settings Data
  API_GETGLOBALSETTINGS_API: APIServerURL + "api/GlobalSettings/GetGlobalSettingDetails",

	//Save User profile details
  API_SAVEGLOBALSETTING_API: APIServerURL + "api/GlobalSettings/SaveGlobalSettings",

  // Default Lexicon details
  API_GET_DEFAULT_ALLLIST_LEXICON_URL: APIServerURL + "api/UserManagement/GetDefaultLexiconList",
}

var CommonJS = {
	//// Authorization Header for Ajax call
	BeforeSendAjaxCall: function (xhr) {
		CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
		xhr.setRequestHeader("Authorization", "Bearer " + CommonJS.GetStorageItem("access_token"));
		xhr.setRequestHeader("RefreshToken", CommonJS.GetStorageItem("refresh_token"));
	},
	SetStorageItem: function (key, value) {
		localStorage.setItem(key, value);
	},
	GetStorageItem: function (key) {
		return localStorage.getItem(key);
	},
	RemoveStorageItem: function (key) {
		localStorage.removeItem(key);
	},
	Getcookie: function (cookiename) {
		var nameEQ = cookiename + "=";
		var ca = document.cookie.split(';');
		for (var i = 0; i < ca.length; i++) {
			var c = ca[i];
			while (c.charAt(0) == ' ') c = c.substring(1, c.length);
			if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
		}
		return null;
	},
	HandleErrorStatus: function (statusCode) {
		switch (statusCode) {
			case 500:
				window.location.href = UnauthorizedURL;
				break;
			case 404:
				window.location.href = UnauthorizedURL;
				break
			case 403:
				window.location.href = UnauthorizedURL;
				break;
			default:
				break
		}
	},
	SetPageLoader: function (toggleIndicator) {
		if (toggleIndicator) {
			if (toggleIndicator === "show") {
				$("#divPageLoaderBG").show();
				$("#divPageLoader").show();
			}
			else if (toggleIndicator === "hide") {
				$("#divPageLoaderBG").hide();
				$("#divPageLoader").hide();
			}
		}
	}
}

var ChartStyle = {
	SetChartFonts: function (chart) {
		chart.options.legend.labels.fontFamily = "Roboto";
		chart.options.axisDefaults.labels.font = "12px Roboto";
		chart.options.categoryAxis.labels.font = "12px Roboto";
		chart.options.seriesDefaults.labels.font = "12px Roboto";
		chart.options.title.font = "bold  15px Roboto";
		chart.options.valueAxis.labels.font = "15px Roboto";
		chart.options.tooltip.font = "12px Roboto";
	}
}

var showPageAlert = function (alertMsg, alertTypes, callback) {
	$(".modal .modal-title").html(alertTypes.toString());
	$(".modal .modal-body").html(alertMsg);
	$('.modal').modal({ backdrop: 'static', keyboard: false })
	$(".modal").modal("show");

	$('.modal .model-ok').on('click', function () {
		window.location.href = callback;
	});
};
function bcmStrategyAlerts(options) {
	var deferredObject = $.Deferred();
	var defaults = {
		type: "alert", //alert, prompt,confirm
		modalSize: 'modal-sm', //modal-sm, modal-lg
		okButtonText: 'Ok',
		cancelButtonText: 'Cancel',
		yesButtonText: 'Yes',
		noButtonText: 'No',
		headerText: 'Attention',
		messageText: 'Message',
		alertType: 'default', //default, primary, success, info, warning, danger
		inputFieldType: 'text', //could ask for number,email,etc
	}
	$.extend(defaults, options);

	var _show = function () {
		var headClass = "navbar-default";
		switch (defaults.alertType) {
			case "primary":
				headClass = "alert-primary";
				break;
			case "Success":
				headClass = "alert-success";
				break;
			case "Info":
				headClass = "alert-info";
				break;
			case "Warning":
				headClass = "alert-warning";
				break;
			case "Danger":
				headClass = "alert-danger";
				break;
		}
		$('BODY').append(
            '<div id="bcmStrategyAlerts" class="modal fade bcm-modal">' +
            '<div class="modal-dialog" class="' + defaults.modalSize + '">' +
            '<div class="modal-content">' +
            '<div id="bcmStrategyAlerts-header" class="modal-header">' +
      '<h4 id="bcmStrategyAlerts-title" class="modal-title">Modal title</h4>' +
            '<button id="close-button" type="button" class="close" data-dismiss="modal"><span aria-hidden="true">×</span><span class="sr-only">Close</span></button>' +
            '</div>' +
            '<div id="bcmStrategyAlerts-body" class="modal-body">' +
            '<div id="bcmStrategyAlerts-message" ></div>' +
            '</div>' +
            '<div id="bcmStrategyAlerts-footer" class="modal-footer">' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>'
        );

		$('.modal-header').css({
			'padding': '15px 15px',
			'-webkit-border-top-left-radius': '5px',
			'-webkit-border-top-right-radius': '5px',
			'-moz-border-radius-topleft': '5px',
			'-moz-border-radius-topright': '5px',
			'border-top-left-radius': '5px',
			'border-top-right-radius': '5px'
		});

		$('#bcmStrategyAlerts-title').text(defaults.headerText);
		$('#bcmStrategyAlerts-message').html(defaults.messageText);

		var keyb = "false", backd = "static";
		var calbackParam = "";
		switch (defaults.type) {
			case 'alert':
				keyb = "true";
				backd = "true";
				$('#bcmStrategyAlerts-footer').html('<button class="btn btn-' + defaults.alertType + '">' + defaults.okButtonText + '</button>').on('click', ".btn", function () {
					calbackParam = true;
					$('#bcmStrategyAlerts').modal('hide');
				});
				break;
			case 'confirm':
				var btnhtml = '<button id="bcmok-btn" class="btn btn-dark-green waves-effect waves-light">' + defaults.yesButtonText + '</button>';
				if (defaults.noButtonText && defaults.noButtonText.length > 0) {
					btnhtml += '<button id="bcmclose-btn" class="btn btn-blue-grey waves-effect waves-light">' + defaults.noButtonText + '</button>';
				}
				$('#bcmStrategyAlerts-footer').html(btnhtml).on('click', 'button', function (e) {
					if (e.target.id === 'bcmok-btn') {
						calbackParam = true;
						$('#bcmStrategyAlerts').modal('hide');
					} else if (e.target.id === 'bcmclose-btn') {
						calbackParam = false;
						$('#bcmStrategyAlerts').modal('hide');
					}
				});
				break;
			case 'prompt':
				$('#bcmStrategyAlerts-message').html(defaults.messageText + '<br /><br /><div class="form-group"><input type="' + defaults.inputFieldType + '" class="form-control" id="prompt" /></div>');
				$('#bcmStrategyAlerts-footer').html('<button class="btn btn-primary">' + defaults.okButtonText + '</button>').on('click', ".btn", function () {
					calbackParam = $('#prompt').val();
					$('#bcmStrategyAlerts').modal('hide');
				});
				break;
		}
		$('.bcm-modal').modal({ backdrop: 'static', keyboard: false })
		$(".bcm-modal").modal("show");

		$('#bcmStrategyAlerts').modal({
			show: 'hide',
			backdrop: backd,
			keyboard: keyb
		}).on('hidden.bs.modal', function (e) {
			$('#bcmStrategyAlerts').remove();
			deferredObject.resolve(calbackParam);
		}).on('shown.bs.modal', function (e) {
			if ($('#prompt').length > 0) {
				$('#prompt').focus();
			}
		}).modal('show');
	}

	_show();
	return deferredObject.promise();
}

//Function to handle API Error
function HandleError(error, redirectUrl) {
	//var userId = localStorage.getItem("UserId");
	var userId = localStorage.getItem("userHashId");

	if (userId == null) {
		//LayoutJS.logout();
		window.location.href = UnauthorizedURL;
	}
	else if (error && error.xhr.status == "403") {
		window.location.href = UnauthorizedURL;
	}
	else if (error && error.xhr.status == "500") {
		window.location.href = UnauthorizedURL;
	}
		//else if (error && error.status == 400) {
		//  showPageAlert(error.responseText, alertTypes.Warning);
		//}
	else {
		//showPageAlert('A technical error has occurred. Please contact administrator.', alertTypes.Danger);
	}
	CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
};

//$(document).ready(function () {
//  $("#btnAlert").on("click", function () {
//    var prom = bcmStrategyAlerts({
//      messageText: "hello world",
//      alertType: "danger"
//    }).done(function (e) {
//      $("body").append('<div>Callback from alert</div>');
//    });
//  });

//  $("#btnConfirm").on("click", function () {
//    bcmStrategyAlerts({
//      type: "confirm",
//      messageText: "hello world",
//      alertType: "info"
//    }).done(function (e) {
//      $("body").append('<div>Callback from confirm ' + e + '</div>');
//    });
//  });

//  $("#btnPrompt").on("click", function () {
//    bcmStrategyAlerts({
//      type: "prompt",
//      messageText: "Enter Something",
//      alertType: "primary"
//    }).done(function (e) {
//      bcmStrategyAlerts({
//        messageText: "You entered: " + e,
//        alertType: "success"
//      });
//    });
//  });

//});

var PageLoaderActivity = {
	SHOW: "show",
	HIDE: "hide",
};
var alertTypes = {
	Success: "Success",
	Info: "Info",
	Warning: "Warning",
	Danger: "Danger",
};

//Function to handle JQuery Errors
function ValidateForm(validateForm, response) {
	if (validateForm != null) {
		var validator = $("#" + validateForm + "").validate({
			focusInvalid: true,
			onfocusout: function (element) {
				// $(element).valid();
			},
			highlight: function (element) {
				$(element).closest('.form-group').addClass('has-error');
			},

			unhighlight: function (element) {
				$(element).closest('.form-group').removeClass('has-error');
			},

			errorElement: 'span',
			errorClass: 'red-text',
			errorPlacement: function (error, element) {
				if (error[0] && element[0]) {
					error[0].id = element[0].id + '-error';
				}

				if (element.parent('.input-group').length) {
					error.insertAfter(element.parent());
				}
				else if (element.parent().parent().hasClass('k-widget')) {
					error.insertAfter(element.parent().parent());
				}
				else {
					error.insertAfter(element);
				}
			},
			invalidHandler: function (form, vl) {
				var errors = vl.numberOfInvalids();
				if (errors) {
					vl.errorList[0].element.focus();
				}
			},
			ignore: ".ignore"
		});
		validator.showErrors(response);
	}
	else {
		showPageAlert('A technical error has occurred. Please contact administrator', alertTypes.Danger);
	}
};

//Function to reset JQuery Validator
function ResetvalidateForm(validateForm) {
	var resetValidator = $("#" + validateForm + "").validate();
	resetValidator.resetForm();
	$(".form-group").removeClass('has-error');
}

function moveDivTop(e) {
	$('html, body').animate({
		'scrollTop': $('#' + e).position().top
	});
};