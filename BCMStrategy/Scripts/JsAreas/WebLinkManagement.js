var viewModelInstitutions;
var selectedPersonDict = {};
var isActionTypeRemove = 0;
var RSSFeeds = "RSSFeeds";
var isShowRssDiv = "false";
$(document).ready(function () {
	GetCountryDDList("countryMasterHashId");
	loadKendoGrid();
	GetWebLinkManagementDDL();
	BindIndividualPersonGrid();
	BindEntityFullNameAutoComplete();
});

function InitializedProprietaryTagMultiSelect() {
	$('#metaDataProprietaryTag').fastselect({
		placeholder: 'Select Action Type',
		onItemSelect: function (i, k) {
			CloseMultiSelectOptionPopUp();
			GetActivityTypeBasedOnActionType();
		}
	});
}

function CloseMultiSelectOptionPopUp() {
	$('.fstActive').removeClass('fstActive');
}

function InitializedActivityTypeMultiSelect() {
	$('#activityType').fastselect({
		placeholder: 'Select Activity Type',
		onItemSelect: function (i, k) {
			CloseMultiSelectOptionPopUp();
		}
	});
}

function InitializedSectorMultiSelect() {
	$('#sector').fastselect({
		placeholder: 'Select Sector',
		onItemSelect: function (i, k) {
			CloseMultiSelectOptionPopUp();
		}
	});
}

var GetWebLinkManagementDDL = function () {
	$("#divPageLoader").show();
	$.ajax({
		url: BCMConfig.API_GETWEBLINK_MANAGEMENT_DATA,
		beforeSend: CommonJS.BeforeSendAjaxCall,
		dataType: "json",
		contentType: 'application/json; charset=utf-8',
		method: 'GET',
		processdata: true,
		success: function (response) {
			if (response) {
				BindPageType(response.pageTypeDDL);
				BindWebSiteType(response.webSiteTypeDDL);
				BindEntityType(response.entityTypeDDL);
				BindSector(response.sectorDDL);
				BindActivityType(response.activityTypeDDL);
				BindProprierityType(response.metaDataProprietaryDDL);
				$("#divPageLoader").hide();
			}
		},
		error: function (e) {
			CommonJS.HandleErrorStatus(e.status);
		}
	});
}

function BindSearchKeyword(response) {
	if (response.searchKeywords != null && response.searchKeywords.length > 0) {
		$.each(response.searchKeywords, function (index, val) {
			$('#searchKeyWord').tagsinput('add', val);
		});
	}
}

function BindRSSFeedUrls(response) {
	//$("#rssFeedUrlList").html('');
	$("#RSSFeedURL").tagsinput('removeAll');
	var webSiteType = $('#pageType option:selected').text();
	if (webSiteType == RSSFeeds) {
		HideShowRssUrlDiv(true);
		if (response.rssFeedURLs != null && response.rssFeedURLs.length > 0) {
			var u = response.rssFeedURLs;
			$.each(u, function (index, val) {

				$('#RSSFeedURL').tagsinput('add', val);

			});


			//$.each(u, function (index, val) {

			//  //$('.wp_pres_slug').append('<a href="#" onClick="openInAppBrowserBlank(' + link + ');"><img src=' + link + ' alt="*"/></a>');

			//  $("#rssFeedUrlList").append('<a href=' + val + ' class="list-group-item">' + val + '</a>');

			//});

		}
	}
	else {
		HideShowRssUrlDiv(false);
	}
}

function BindPageType(pageTypeDDL) {
	$.each(pageTypeDDL, function (key, value) {
		$("#pageType").append($("<option></option>").val(value.keyHash).html(value.value));
	});
}

function BindWebSiteType(websiteDDL) {
	$.each(websiteDDL, function (key, value) {
		$("#webSiteType").append($("<option></option>").val(value.keyHash).html(value.value));
	});
}

function BindActivityType(activityTypeDDL) {
	$.each(activityTypeDDL, function (key, value) {
		if (value.data) {
			$("#activityType").append($("<option selected='selected'></option>").val(value.keyHash).html(value.value));
		}
		else {
			$("#activityType").append($("<option></option>").val(value.keyHash).html(value.value));
		}
	});
	InitializedActivityTypeMultiSelect();
}

function BindProprierityType(proprierityTypeDDL) {
	$.each(proprierityTypeDDL, function (key, value) {
		if (value.data) {
			$("#metaDataProprietaryTag").append($("<option  selected='selected'></option>").val(value.keyHash).html(value.value));
		}
		else {
			$("#metaDataProprietaryTag").append($("<option></option>").val(value.keyHash).html(value.value));
		}

	});
	InitializedProprietaryTagMultiSelect();
}

function BindEntityName(entityNameDDL) {
	$.each(entityNameDDL, function (key, value) {
		$("#entityName").append($("<option></option>").val(value.keyHash).html(value.value));
	});
}

function BindEntityType(entityType) {
	$.each(entityType, function (key, value) {
		$("#entityType").append($("<option></option>").val(value.keyHash).html(value.value));
	});
}

function BindSector(sectorDDL) {
	$.each(sectorDDL, function (key, value) {
		if (value.data) {
			$("#sector").append($("<option selected='selected'></option>").val(value.keyHash).html(value.value));
		}
		else {
			$("#sector").append($("<option></option>").val(value.keyHash).html(value.value));
		}
	});
	InitializedSectorMultiSelect();
}

function BindIndividualPersonAutoComplete() {
	$("#individualPerson").autocomplete({
		minLength: 1,
		multiselect: true,
		source: function (request, response) {
			$.ajax({
				url: BCMConfig.API_INDIVIDUAL_PERSON_DROPDOWNLIST,
				beforeSend: CommonJS.BeforeSendAjaxCall,
				method: "GET",
				data: { searchTerm: request.term },
				dataType: "json",
				success: function (data) {
					console.log(data);
					if (!data.length) {
						var result = [
						 {
						 	label: 'No matches found',
						 	value: ''
						 }
						];
						response(result);
						$('#individualPersonNoResultFlag').val(0);
					}
					else {
						response($.map(data, function (el) {
							return {
								label: el.value,
								value: el.value
							};
						}));
						$('#individualPersonNoResultFlag').val(1);
					}
				},
				error: function (err) {
					alert(err);
				}
			});
		},
		change: function () {
		},
		open: function (event, ui) {
			$(this).autocomplete("widget").css({
				"width": ($(this).width() + 25 + "px")
			});
		},
		select: function (event, ui) {
			if ($('#individualPersonNoResultFlag').val() == 0) {
				event.preventDefault();
				$(this).val('');
			}
		},
		focus: function (event, ui) {
			if ($('#individualPersonNoResultFlag').val() == 0) {
				event.preventDefault();
			}
		}
	})
}

function BindIndividualPersonGrid() {
	$("#individualPersonGrid").kendoGrid({
		height: 300,
		dataSource: {
			type: "json",
			transport: {
				read: {
					url: BCMConfig.API_INDIVIDUAL_PERSON_DROPDOWNLIST,
					beforeSend: CommonJS.BeforeSendAjaxCall,
				},
				parameterMap: function (options) {
					return "parametersJson=" + JSON.stringify(options);
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
						PolicyMakerMasterHashId: { type: "string" },
						FirstName: { type: "string" },
						LastName: { type: "string" },
						Designation: { type: "string" },
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
		height: 380,
		pageable: {
			messages: {
				itemsPerPage: "Items",
				display: "{0} - {1} of {2}",
			},
			pageSize: 10,
			pageSizes: [10, 20, 50],
			refresh: true,
			buttonCount: 4,
			input: true
		},
		dataBound: masterGridIndividualPersonDataBound,
		columns: [
				 {
				 	title: "",
				 	template: "<input id='#= PolicyMakerMasterHashId #' onchange=OnPersonSelectionCheckBoxClick(this) type='checkbox' name='vehicle1' value='Bike'>",
				 	width: 50
				 },
				{
					field: "Designation",
					title: "Designation",
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
					field: "FirstName",
					title: "First Name",
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
					field: "LastName",
					title: "Last Name",
					width: 120,
					filterable: {
						extra: false,
						operators: {
							string: {
								contains: "Contains"
							}
						}
					}
				}
		]
	});
}

function OnPersonSelectionCheckBoxClick(itemObject) {
	var row = $(itemObject).closest("tr");
	var uid = $(row).data("uid");
	var grid = $("#individualPersonGrid").data("kendoGrid");
	var currentDataRow = grid.dataItem(row);
	var policyMakerMasterHashId = currentDataRow.PolicyMakerMasterHashId;
	var firstName = currentDataRow.FirstName;
	var lastName = currentDataRow.LastName;
	var designation = currentDataRow.Designation;
	if ($(itemObject).is(':checked')) {
		if (!selectedPersonDict[policyMakerMasterHashId]) {
			selectedPersonDict[policyMakerMasterHashId] = {
				firstName: firstName,
				lastName: lastName
			};
		}
	}
	else {
		if (selectedPersonDict[policyMakerMasterHashId]) {
			delete selectedPersonDict[policyMakerMasterHashId]
		}
	}
}

function OnIndividualPersonModelSelectClick(itemObject) {
	$('#selectedIndividualPersonHashIds').val('');
	$('#individualPerson').val('');
	var grid = $("#individualPersonGrid").data("kendoGrid");

	$.each(selectedPersonDict, function (key, object) {
		var firstName = object.firstName;
		var lastName = object.lastName;
		var designation = object.Design;
		$('#selectedIndividualPersonHashIds').val(function (i, val) {
			return val != '' ? (val + ';' + key) : key;
		});
		$('#individualPerson').val(function (i, val) {
			return val != '' ? (val + ';' + (firstName ? firstName : '') + " " + (lastName ? lastName : '')) : ((firstName ? firstName : '') + " " + (lastName ? lastName : ''));
		});
	});
	$('#myModal').modal('toggle');
}

function ToggelPersonModel() {
	$('#myModal').modal('toggle');
}

$('#btnCancel').click(function () {
	$('#collapseExample').collapse('hide');
});

function BindEntityFullNameAutoComplete() {
	var noResultFlag = $('#entityFullNameNoResultFlag');
	$("#entityFullName").autocomplete({
		minLength: 3,
		source: function (request, response) {
			$.ajax({
				url: BCMConfig.API_ENTITYFULLNAME_DROPDOWNLIST,
				beforeSend: CommonJS.BeforeSendAjaxCall,
				method: "GET",
				data: { searchTerm: request.term },
				dataType: "json",
				success: function (data) {
					console.log(data);
					if (!data.length) {
						var result = [
						 {
						 	label: 'No matches found',
						 	value: '',
						 	data: ''
						 }
						];
						response(result);
						noResultFlag.val(0);
					}
					else {
						response($.map(data, function (el) {
							return {
								label: el.value,
								value: el.value,
								data: el.keyHash
							};
						}));
						noResultFlag.val(1);
					}
				},
				error: function (err) {
					alert(err);
				}
			});
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
		},
		change: function () {
			if ($('#entityFullNameHiddenVal').val() == '')
				$(this).val('')
		},
		open: function (event, ui) {
			$(this).autocomplete("widget").css({
				"width": ($(this).width() + 25 + "px")
			});
		},
		select: function (event, ui) {
			if (noResultFlag.val() == '0') {
				event.preventDefault();
				$(this).val('')
			}
			$('#entityFullNameHiddenVal').val(ui.item.data);
		},
		focus: function (event, ui) {
			if (noResultFlag.val() == '0') {
				event.preventDefault();
			}
		}
	})
}

var loadKendoGrid = function () {
	$("#gridWebLink").kendoGrid({
		dataSource: {
			type: "json",
			transport: {
				read: {
					url: BCMConfig.API_GETALLLIST_WEBLINKS_URL,
					beforeSend: CommonJS.BeforeSendAjaxCall

				},
				parameterMap: function (options) {
					if (options.filter && options.filter.filters) {
						options.filter.filters.filter(function (index, element) {
							var obj = options.filter.filters[element];

							if (obj.field == 'WebLinkUrl') {
								obj.value = encodeURIComponent(obj.value);

							}
						});
					}

					return "parametersJson=" + JSON.stringify(options);
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
						WebLinkMasterHashId: { type: "string" },
						WebSiteType: { type: "string" },
						EntityType: { type: "string" },
						IsHardCodedString: { type: "string" },
						WebLinkUrl: { type: "string" },
						PageType: { type: "string" },
						CountryName: { type: "string" },
						IsActiveString: { type: "string" }
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
			refresh: true,
			buttonCount: 5,
			input: true
		},
		dataBound: masterGridDataBound,
		columns: [
				{
					field: "WebSiteType",
					title: "Website Type",
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
					field: "WebLinkUrl",
					title: "WebLink URL",
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
					field: "IsHardCodedString",
					title: "HardCoded?",
					width: 125,
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
					field: "EntityFullName",
					title: "Entity Name",
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
					field: "EntityType",
					title: "Entity Type",
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
							field: "CountryName",
							title: "Country",
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
						 	field: "PageType",
						 	title: "Page Type",
						 	//hidden:true,
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
								 	field: "IsActiveString",
								 	title: "Active ?",
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
					title: "",
					template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditWebLink(\"#=WebLinkMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
					width: 50
				},
				{
					title: "",
					template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteWebLink(\"#=WebLinkMasterHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
					width: 50
				}

		]
	});
}

function masterGridDataBound(e) {
	var grid = e.sender;
	if (grid.dataSource.total() == 0) {
		var colCount = grid.columns.length;
		$(e.sender.wrapper)
				.find('tbody').first()
				.append('<tr class="kendo-data-row" style="text-align: center;"><td colspan="' + colCount + '" class="no-data">' + noDataFound + '</td></tr>');
	}

	CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}

function masterGridIndividualPersonDataBound(e) {
	var grid = e.sender;
	if (grid.dataSource.total() == 0) {
		var colCount = grid.columns.length;
		$(e.sender.wrapper)
				.find('tbody').first()
				.append('<tr class="kendo-data-row" style="text-align: center;"><td colspan="' + colCount + '" class="no-data">' + noDataFound + '</td></tr>');
	}
	$('#individualPersonGrid .k-grid-content').height(250);
	$('td input:checkbox', $('#individualPersonGrid')).each(function (index, element) {
		if (selectedPersonDict != null && IsInDictionary($(element).attr('id'), selectedPersonDict))
			$(this).prop('checked', true);
	});

	CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}

function IsInDictionary(key, dictionary) {
	if (dictionary[key])
		return true;
}

$("#clearFilters").click(function () {
	$("#gridWebLink").data("kendoGrid").dataSource.filter([]);
});

var initViewModel = function () {
	viewModelInstitutions = kendo.observable({
		InstitutionMasterHashId: "",
		CountryMasterHashId: "",
		InstitutionTypeHashId: "",
		InstitutionsName: ""
	});
	GetInstituionTypeDDList("institutionTypeHashId");

};

function DeleteWebLink(itemObject) {
	bcmStrategyAlerts({
		type: "confirm",
		messageText: "You won't be able to undo this action. Are you sure to delete this WebLink?",
		headerText: deletePopupHeader,
		alertType: alertTypes.Info
	}).done(function (e) {
		if (e) {
			CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
			////var row = $(itemObject).closest("tr");
			////var uid = $(row).data("uid");
			////var grid = $("#gridWebLink").data("kendoGrid");
			////var currentDataRow = grid.dataItem(row);

			var webLinkHashId = itemObject; ////currentDataRow.WebLinkMasterHashId

			$.ajax({
				url: BCMConfig.API_WEBLINKS_DELETE + "?webLinkHashId=" + webLinkHashId,
				beforeSend: CommonJS.BeforeSendAjaxCall,
				method: 'GET',
				dataType: "json",
				contentType: "application/json; charset=utf-8",
				processdata: true,
				success: function (response) { //call successfull
					if (response.data) {
						toastr.info(response.errorMessage);
						ResetForm();
						loadKendoGrid();
					}
					else {
						toastr.info("webLink can not be deleted as it is associated with other entities");
					}
					CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
				},
				error: function (e) {
					CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
					CommonJS.HandleErrorStatus(e.status);
					//error occurred
				}
			});
		}
	});
}

function EditWebLink(itemObject) {
	CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
	ResetForm();
	////var row = $(itemObject).closest("tr");
	////var uid = $(row).data("uid");
	////var grid = $("#gridWebLink").data("kendoGrid");
	////var currentDataRow = grid.dataItem(row);

	$('#collapseExample').collapse('show');
	////if (currentDataRow.PageType == RSSFeeds) {
	////}
	////if (currentDataRow.PageType == "SearchFunctionality") {
	////}
	GetWebLinkBasedOnHash(itemObject);
	$(".form-group").removeClass('has-error');
	moveDivTop("topPanel");
	$('#webLinkHashHiddenVal').val(itemObject);
	
	isShowRssDiv = "true";
}

function GetWebLinkBasedOnHash(webLinkHashId) {
	var guiId = generateGUID();

	$.ajax({
		url: BCMConfig.API_GETWEBLINK_BASED_ON_HASH + "?webLinkHashId=" + webLinkHashId + '&guiId=' + guiId,
		dataType: "json",
		beforeSend: CommonJS.BeforeSendAjaxCall,
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

function BindIndividualPersonCheckBoxAndDict(response) {
	selectedPersonDict = {};
	var str = '';
	$.each(response.policyMakerModelList, function (index, object) {
		selectedPersonDict[object.policyMakerHashId] = {
			firstName: object.policyFirstName,
			lastName: object.policyLastName
		}
		str = str != '' ? str + "," + object.policyFirstName + " " + object.policyLastName : object.policyFirstName + " " + object.policyLastName;
	});
	$('#individualPerson').val(str);
	$('td input:checkbox', $('#individualPersonGrid')).each(function (index, element) {
		if (selectedPersonDict != null && IsInDictionary($(element).attr('id'), selectedPersonDict))
			$(this).prop('checked', true);
	});
}

function isInArray(value, array) {
	return array.indexOf(value) > -1;
}

function HidePanelOnPageTypeChange() {
	$('.dropDownSearch').hide();
	$('.hyperLinkClicked').hide();
}

function ClearTextBoxOnPageTypeChange() {
	$('[data-role="remove"]').click();
}

function BindData() {
	var viewModelWebLink = {
		'WebLinkMasterHashId': $('#webLinkHashHiddenVal').val(),
		'WebSiteTypeHashId': $('#webSiteType').val(),
		'WebLinkUrl': $('#webLinkUrl').val(),
		'IsBlocked': $('#blockedTrue').is(':checked') ? true : false, //$("input[name='isBlocked']:checked").val(),
		'ProprietaryHashIds': $('#metaDataProprietaryTag').val(),
		'ActivityTypeHashIds': $('#activityType').val(),
		'EntityTypeHashId': $('#entityType').val(),
		'EntityFullNameHashId': $('#entityFullNameHiddenVal').val(),
		'IndividualPersonHashIds': $('#selectedIndividualPersonHashIds').val(),
		'SectorHashIds': $('#sector').val(),
		'CountryHashId': $('#countryMasterHashId').val(),
		'PageTypeHashId': $('#pageType').val(),
		'IsActive': $('#isActiveTrue').is(':checked') ? true : false
	};

	return viewModelWebLink;
}

function ResetForm() {
	$('#webLinkForm').trigger("reset");
	InitializedActivityTypeMultiSelect();
	//$("#rssFeedUrlList").html('');
	$("#RSSFeedURL").tagsinput('removeAll');
}

function GetPageTypeRegexInputFlag(selector) {
	var selectorVal = selector.val();


}

$("#btnWebLinkSave").click(function () {
	if ($("#webLinkForm").valid()) {
		CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
		ResetvalidateForm();
		var webLinkModel = BindData();
		$.ajax({
			url: BCMConfig.API_POST_WEBLINKS_URL,
			beforeSend: CommonJS.BeforeSendAjaxCall,
			method: 'POST',
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			data: JSON.stringify(webLinkModel),
			processdata: true,
			success: function (response) {
				if (response.errorModel != null && response.errorModel != "") {
					AppyCustomValidation(response.errorModel);
					ValidateForm('webLinkForm', response.errorModel);
				}
				if (response.data) {
					toastr.info(response.errorMessage);
					ResetFormValues();
					loadKendoGrid();
					$('#collapseExample').collapse('hide');
				}
				else {
					if (response.errorMessage != null && response.errorMessage != "")
						toastr.error(response.errorMessage);
				}
				CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
				RemoveErrorClass();
			},
			complete: function (e) {
				$('.error').removeAttr('for');
			},
			error: function (e) {
				CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
				CommonJS.HandleErrorStatus(e.status);
			}
		});
	}
});

function AppyCustomValidation(errorModel) {
	if (errorModel['proprietaryHashIds']) {
		$('.proprierityTagDiv').append('<label id="proprierity-error" class="red-text" style="padding-top:  5px;" for="proprierity">Action Type is required</label>');
		delete errorModel['proprietaryHashIds'];
	}
	else
		$('.proprierityTagDiv').find('.red-text').remove();
}

function RemoveErrorClass() {
	$('.form-control').removeClass('error');
	$('label[id*="error"]').removeClass('error').addClass('red-text');
}

function ResetvalidateForm() {
	$('label[id*="error"]').remove();
	$('label[id*="error"]').addClass('error').removeClass('red-text');
}

function GetPageTypeRegexFlagObj() {
	var obj = {
		IsSearchFunctionality: $('#pageTypeRegexPanelFlag').val() == 1 ? true : false,
		IsRSSFeedAvailable: $('#pageTypeRegexPanelFlag').val() == 2 ? true : false,
	};
	return obj;
}

$('#btnWebLinkCancel').click(function (e) {
	ResetFormValues();
	$('#collapseExample').collapse('hide');
	ClearPopIndividualPersonSelection();
});

function ClearPopIndividualPersonSelection() {
	selectedPersonDict = {};
	$('td input:checkbox', $('#individualPersonGrid')).each(function (index, element) {
		$(this).prop('checked', false);
	});
}

function ResetFormValues() {
	$('#webLinkHashHiddenVal').val('');
	$('.fstChoiceRemove').click();
	$('.red-text').remove();
	$('#webSiteType,#webLinkUrl,#entityFullName,#entityFullName,#entityType,#countryMasterHashId,#pageType,#RSSFeedURL,#searchKeyWord').val('');
	$("#searchKeyWord,#RSSFeedURL").tagsinput('removeAll');
	$('input:radio[name="isBlocked"][value="false"]').prop('checked', true);
	$('#individualPerson').val('');
	$('#selectedIndividualPersonHashIds').val('');
	$('#blockedTrue').prop('disabled', false);
	$('#blockedFalse').prop('disabled', false);
	ClearPopIndividualPersonSelection();
}

function AppendHTMLOnEdit() {
	$('.fstElement').remove();
	$('.proprierityTagDiv').append('<select class="form-control multipleSelect" multiple id="metaDataProprietaryTag" name="metaDataProprietaryTag"></select>');
	$('.activityTypeDiv').append('<select class="form-control multipleSelect" multiple id="activityType" name="activityType"></select>');
	$('.sectorDiv').append('<select class="form-control multipleSelect" multiple id="sector" name="sector"></select>');
}

function AppendSectorTagHtml() {
	$('.sectorDiv').find('.fstElement').remove();
	$('.sectorDiv').append('<select class="form-control multipleSelect" multiple id="sector" name="sector"></select>');
}

function AppendProprietoryTagHtml() {
	$('.proprierityTagDiv').find('.fstElement').remove();
	$('.proprierityTagDiv').append('<select class="form-control multipleSelect" multiple id="metaDataProprietaryTag" name="metaDataProprietaryTag"></select>');
}

function AppendActivityTypeHtml() {
	$('.activityTypeDiv').find('.fstElement').remove();
	$('.activityTypeDiv').append('<select class="form-control multipleSelect" multiple id="activityType" name="activityType"></select>');
}

function BindDataOnEdit(response) {
	ResetvalidateForm();
	AppendSectorTagHtml();
	$('[data-role="remove"]').click();
	$('#webSiteType').val(response.webSiteTypeHashId);
	$('#webLinkUrl').val(response.webLinkUrl);
	$("input[name='isBlocked']").val(response.isBlocked);
	$('#selectedIndividualPersonHashIds').val(response.individualPersonHashIds);
	if (response.isBlocked)
		$('#blockedTrue').prop('checked', true);
	else
		$('#blockedFalse').prop('checked', true);
	if (response.isActive)
		$('#isActiveTrue').prop('checked', true);
	else
		$('#isActiveFalse').prop('checked', true);

	$('#entityFullName').val(response.entityFullName);
	$('#entityFullNameHiddenVal').val(response.entityFullNameHashId);
	$('#entityType').val(response.entityTypeHashId);
	$('#countryMasterHashId').val(response.countryHashId);
	$('#pageType').val(response.pageTypeHashId);
	BindSector(response.sectorDDL);
	BindIndividualPersonCheckBoxAndDict(response);
	webSiteTypeChange(true);
	BindRSSFeedUrls(response);
	CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}

function webSiteTypeChange(isEdit) {
	var webSiteType = $('#webSiteType option:selected').text();
	$('.proprierityTagDiv').find('[id*="error"]').remove();
	if (webSiteType == 'Media Pages') {
		ClearValsOnMediaSectorSelection();
		$('.activityTypeDisabledDiv').show();
		$('.activityTypeDiv').hide();
		$('.entitysIndividualNameDisabledDiv').show();
		$('.entityTypeDiv').hide();
		$('.entityTypeDivDisabled').show();
		$('.countryDivDisabled').show();
		$('.countryDiv').hide();
		$('.entityFullNameDisabledDiv').show();
		$('.entityFullNameDiv').hide();
		$('.entitysIndividualNameDiv').hide();
		$('#entityType').prop('disabled', true);
		$('#countryMasterHashId').prop('disabled', true);
		$('#blockedTrue').prop('disabled', true);
		$('#blockedFalse').prop('disabled', true);
		$('#entityFullName').prop('disabled', true);
	}
	else {
		$('.activityTypeDisabledDiv').hide();
		$('.activityTypeDiv').show();
		$('.entitysIndividualNameDisabledDiv').hide();
		$('.entitysIndividualNameDiv').show();
		$('.entityTypeDivDisabled').hide();
		$('.entityTypeDiv').show();
		$('.countryDivDisabled').hide();
		$('.countryDiv').show();
		$('.entityFullNameDisabledDiv').hide();
		$('.entityFullNameDiv').show();
		$('#entityType').prop('disabled', false);
		$('#countryMasterHashId').prop('disabled', false);
		$('#blockedTrue').prop('disabled', false);
		$('#blockedFalse').prop('disabled', false);
		$('#entityFullName').prop('disabled', false);
	}

	GetActionTypeBasedOnWebsiteType(isEdit);
}

function GetActionTypeBasedOnWebsiteType(isEdit) {
	CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
	var viewModelWebLink = {
		'WebLinkMasterHashId': $('#webLinkHashHiddenVal').val(),
		'WebSiteTypeHashId': $('#webSiteType').val(),
		'IsEdit': isEdit
	};
	$.ajax({
		url: BCMConfig.API_POST_ACTION_TYPE_BASED_ON_WEBSITETYPE,
		beforeSend: CommonJS.BeforeSendAjaxCall,
		method: 'POST',
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify(viewModelWebLink),
		processdata: true,
		async: false,
		success: function (response) {
			if (response) {
				AppendProprietoryTagHtml();
				BindProprierityType(response.metaDataProprietaryDDL);
				AppendActivityTypeHtml();
				BindActivityType(response.activityTypeDDL);
				CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			}
		},
		complete: function (e) {
		},
		error: function (e) {
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			CommonJS.HandleErrorStatus(e.status);
		}
	});

}

function GetActivityTypeBasedOnActionType() {
	var webLinkModel = {
		'ProprietaryHashIds': $('#metaDataProprietaryTag').val()
	};
	CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
	$.ajax({
		url: BCMConfig.API_POST_ACTIVITYTYPE_BASED_ON_ACTION_TYPE,
		beforeSend: CommonJS.BeforeSendAjaxCall,
		method: 'POST',
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify(webLinkModel),
		processdata: true,
		success: function (response) {
			if (response) {
				AppendActivityTypeHtml();
				BindActivityType(response.activityTypeDDL);
				CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			}
		},
		complete: function (e) {
		},
		error: function (e) {
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			CommonJS.HandleErrorStatus(e.status);
		}
	});

}

function ClearValsOnMediaSectorSelection() {
	$('#activityType').parent().find('.fstChoiceRemove').click();
	$('#individualPerson').val('');
	$('#selectedIndividualPersonHashIds').val('');
	ClearPopIndividualPersonSelection();
	$('#entityType,#countryMasterHashId,#entityFullName,#entityFullNameHiddenVal').val('');
	$('#blockedFalse').prop('checked', true);
}


function generateGUID() { // Public Domain/MIT
	var d = new Date().getTime();
	if (typeof performance !== 'undefined' && typeof performance.now === 'function') {
		d += performance.now(); //use high-precision timer if available
	}
	return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
		var r = (d + Math.random() * 16) % 16 | 0;
		d = Math.floor(d / 16);
		return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
	});
}

function onChnagePageType() {
	var webSiteType = $('#pageType option:selected').text();
	if (webSiteType == RSSFeeds && isShowRssDiv == "true") {
		HideShowRssUrlDiv(true);
	}
	else {
		HideShowRssUrlDiv(false);
	}
}

function HideShowRssUrlDiv(input) {
	$("#divRssUrl").attr('disabled', 'disabled');
	$("#RSSFeedURL").attr('disabled', 'disabled');
	if (input) {
		$("#divRssUrl").show();
	}
	else {
		$("#divRssUrl").hide();
	}
}

$('#BtnAddNew').click(function () {
	isShowRssDiv = "false";
	HideShowRssUrlDiv(false);
});
