var viewModelMetadataPhrases;

$(document).ready(function () {
	GetMetadataTypeDDList("metadataTypeMasterHashId");
	ResetForm();
	loadKendoGrid();
});

var loadKendoGrid = function () {
	$("#gridMetadataPhrases").kendoGrid({
		dataSource: {
			type: "json",
			transport: {
				read: {
					url: BCMConfig.API_GETALLLIST_METADATAPHRASES_URL,
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
						MetadataPhrasesMasterHashId: { type: "string" },
						MetadataTypeMasterHashId: { type: "string" },
						MetaData: { type: "string" },
						MetadataPhrases: { type: "string" },
						WebsiteType: { type: "string" },
						ActivityTypeMasterHashId: { type: "string" },
						ActivityType: { type: "string" },
						ActivityValue: { type: "string" }
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
       	field: "WebsiteType",
       	title: websiteType,
       	width: 180,
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
       	field: "MetaData",
       	title: metadataType,
       	width: 180,
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
       	field: "ActivityType",
       	title: activityType,
       	width: 170,
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
       	field: "MetadataPhrases",
       	title: metadataPhrases,
       	width: 240,
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
       	field: "ActivityValue",
       	title: activityValue,
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
       	title: "",
       	template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditMetadataPhrases(\"#=MetadataPhrasesMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
       	width: 50
       },
       {
       	title: "",
       	template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteMetadataPhrases(\"#=MetadataPhrasesMasterHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
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

$("#btnMetadataPhrasesSave").click(function () {
	if ($("#metadataPhrasesForm").valid()) {
		CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
		ResetvalidateForm('metadataPhrasesForm');

		$.ajax({
			url: BCMConfig.API_METADATAPHRASES_UPDATE,
			beforeSend: CommonJS.BeforeSendAjaxCall,
			method: 'POST',
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			data: JSON.stringify(viewModelMetadataPhrases),
			processdata: true,
			success: function (response) {
				if (response.errorModel != null && response.errorModel != "") {
					ValidateForm('metadataPhrasesForm', response.errorModel);
				}
				if (response.data) {
					toastr.info(response.errorMessage);
					ResetForm();
					loadKendoGrid();
					$('#collapseExample').collapse('hide');
				}
				else {
					if (response.errorMessage != null && response.errorMessage != "")
						toastr.error(response.errorMessage);
				}
				CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			},
			error: function (e) {
				CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
				CommonJS.HandleErrorStatus(e.status);
			}
		});
	}
});

$("#btnMetadataPhrasesCancel").click(function () {
	ResetForm();
	$('#collapseExample').collapse('hide');
});
$("#clearFilters").click(function () {
	$("#gridMetadataPhrases").data("kendoGrid").dataSource.filter([]);
});

function EditMetadataPhrases(PhrasesHashId) {
	CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
	ResetForm();
	//var row = $(itemObject).closest("tr");
	//var uid = $(row).data("uid");
	//var grid = $("#gridMetadataPhrases").data("kendoGrid");
	//var currentDataRow = grid.dataItem(row);

	GetMetadataPhrasesBasedOnHash(PhrasesHashId);
	$('#collapseExample').collapse('show');
	$(".form-group").removeClass('has-error');
	ResetvalidateForm('metadataPhrasesForm');
	moveDivTop("topPanel");
}

$("#metadataTypeMasterHashId").change(function () {
	resetDDL();
	var metadataTypeMasterHashId = $("#metadataTypeMasterHashId option:selected").val();
	GetActivityTypeDDList("activityTypeMasterHashId", metadataTypeMasterHashId);
	viewModelMetadataPhrases.ActivityTypeMasterHashId = "";
});

var ResetForm = function () {
	initViewModel();
	ValidateForm('metadataPhrasesForm', '');
	ResetvalidateForm('metadataPhrasesForm');
	kendo.bind($("#metadataPhrasesForm"), viewModelMetadataPhrases);
	resetDDL();
}

var initViewModel = function () {
	viewModelMetadataPhrases = kendo.observable({
		MetadataPhrasesMasterHashId: "",
		MetadataTypeMasterHashId: "",
		MetadataPhrases: "",
		ActivityTypeMasterHashId: ""
	});
};

function DeleteMetadataPhrases(PhrasesHashId) {
	bcmStrategyAlerts({
		type: "confirm",
		messageText: deletePopupMessage,
		headerText: deletePopupHeader,
		alertType: alertTypes.Info
	}).done(function (e) {
		if (e) {
			CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
			////var row = $(itemObject).closest("tr");
			////var uid = $(row).data("uid");
			////var grid = $("#gridMetadataPhrases").data("kendoGrid");
			////var currentDataRow = grid.dataItem(row);

			var MetadataPhrasesMasterHashId = PhrasesHashId; ////currentDataRow.MetadataPhrasesMasterHashId

			$.ajax({
				url: BCMConfig.API_METADATAPHRASES_DELETE + "?metadataPhrasesMasterHashId=" + MetadataPhrasesMasterHashId,
				beforeSend: CommonJS.BeforeSendAjaxCall,
				method: 'GET',
				dataType: "json",
				contentType: "application/json; charset=utf-8",
				processdata: true,
				success: function (response) {
					if (response.data) {
						toastr.info(response.errorMessage);
						ResetForm();
						loadKendoGrid();
					}
					CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
				},
				error: function (e) {
					CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
					CommonJS.HandleErrorStatus(e.status);
				}
			});
		}
	});
}

function GetMetadataPhrasesBasedOnHash(PhrasesHashId) {
	$.ajax({
		url: BCMConfig.API_GETMETADATA_PHRASES_BASED_ON_HASH + "?phrasesHashId=" + PhrasesHashId,
		beforeSend: CommonJS.BeforeSendAjaxCall,
		dataType: "json",
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

function resetDDL() {
	$("#activityTypeMasterHashId").find("option:not(:first)").remove();
}

function BindDataOnEdit(response) {
	viewModelMetadataPhrases.MetadataPhrasesMasterHashId = response.metadataPhrasesMasterHashId;
	viewModelMetadataPhrases.MetadataTypeMasterHashId = response.metadataTypeMasterHashId;
	viewModelMetadataPhrases.MetadataPhrases = response.metadataPhrases;
	resetDDL();
	GetActivityTypeDDList("activityTypeMasterHashId", viewModelMetadataPhrases.MetadataTypeMasterHashId);
	viewModelMetadataPhrases.ActivityTypeMasterHashId = response.activityTypeMasterHashId;

	kendo.bind($("#metadataPhrasesForm"), viewModelMetadataPhrases);
	CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}