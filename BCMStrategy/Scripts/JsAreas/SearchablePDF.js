//function BindDateTimeTextbox() {
//	var date = new Date();
//	var toDate = new Date(date.getFullYear(), date.getMonth() + 1, 0);
//	$("#txtToDate").kendoDatePicker({
//		format: "MMM/yyyy",
//		//change: OnChangeDate,
//		start: "year",
//		depth: "year",
//		value: toDate,
//		dateInput: true,
//		disableDates: function (date1) {
//			return date1 > new Date(date.getFullYear(), date.getMonth() + 1, 0);
//		},
//		change: function (e) {
//			var val = this.value();
//			if (val != null)
//				this.value(new Date(val.setMonth(val.getMonth() + 1, 0)));
//			viewModelSearchablePDFTypes.toDate = val
//		}
//		//min: new Date()
//		//max: new Date().getMonth + 1,
//		//month: {
//		//  empty: '<span class="k-state-disabled">#= data.value #</span>',
//		//}
//	});

//	var fromDate = new Date(toDate.getFullYear(), toDate.getMonth() - 1, 1);
//	$("#txtFromDate").kendoDatePicker({
//		format: "MMM/yyyy",
//		//change: OnChangeDate,
//		start: "year",
//		depth: "year",
//		value: fromDate,
//		dateInput: true,
//		disableDates: function (date1) {
//			return date1 > new Date(date.getFullYear(), date.getMonth() + 1, 1);
//		},
//		change: function (e) {
//			var val = this.value();
//			if (val != null)
//				this.value(new Date(val.setMonth(val.getMonth(), 1)));
//			viewModelSearchablePDFTypes.fromDate = val
//		}
//		//min: new Date()
//		//max: new Date().getMonth + 1,
//		//month: {
//		//  empty: '<span class="k-state-disabled">#= data.value #</span>',
//		//}
//	});


//	//$("#txtToDate").attr("readonly", true);
//	//$("#txtFromDate").attr("readonly", true);
//}

function BindDateTimeTextbox() {
	var date = new Date();
	var toDate = date;
	$("#txtToDate").kendoDatePicker({
		format: "MM/dd/yyyy",
		//change: OnChangeDate,
		value: toDate,
		dateInput: true,
		disableDates: function (date) {
			return date > new Date();
		}
	});

	//var fromDate = new Date(date.getFullYear(), date.getMonth() - 1, date.getDate(), 0, 0, 0);
	var fromDate = new Date(date.getFullYear(), date.getMonth() - 1, date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds());
	$("#txtFromDate").kendoDatePicker({
		format: "MM/dd/yyyy",
		//change: OnChangeDate,
		value: fromDate,
		dateInput: true,
		disableDates: function (date) {
			return date > new Date();
		}

	});
}


$("#btnSearch").click(function () {
	ResetvalidateForm('searchablPDFForm');
	$("#parentGridSearchablePDF").hide();
	$("#parentDateChartSearchablePDF").hide();
	$.when(CommonJS.SetPageLoader(PageLoaderActivity.SHOW))
	.then(function () {
		//var SearchableModel = {
		//	FromDate: $('#txtFromDate').val(),
		//	ToDate: $('#txtToDate').val(),
		//	Lexicon: $('#txtLexicon').val()
		//};
		viewModelSearchablePDFTypes.Lexicon = $('#txtLexicon').val();
		$.ajax({
			url: BCMConfig.API_GETSEARCHABLE_PDF_DATA_FOR_LINEMONTHWISECHART,
			beforeSend: CommonJS.BeforeSendAjaxCall,
			method: 'POST',
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			data: JSON.stringify(viewModelSearchablePDFTypes),
			processdata: true,
			async: true,
			success: function (response) {
				if (response.errorModel != null && response.errorModel != "") {
					ValidateForm('searchablPDFForm', response.errorModel);
				}
				else {
					var lineChartArray = []
					if (response.length > 0) {
						lineChartArray = response;
					}
					RenderLineMonthWiseChart(response, $('#txtLexicon').val());
				}
				CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			},
			error: function (e) {
				CommonJS.HandleErrorStatus(e.status);
				CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			}
		});
	});
});

var ResetSearchablePDFForm = function () {
	initViewModel();

	ValidateForm('searchablPDFForm', '');
	ResetvalidateForm('searchablPDFForm');
	kendo.bind($("#searchablPDFForm"), viewModelSearchablePDFTypes);
	BindDateTimeTextbox();
}

var initViewModel = function () {
	var date = new Date();
	var toDate = date;
	var fromDate = new Date(date.getFullYear(), date.getMonth() - 1, date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds());

	viewModelSearchablePDFTypes = kendo.observable({
		FromDate: fromDate,
		ToDate: toDate,
		Lexicon: ""
	});
};

var loadKendoGrid = function (fromDate, toDate, actionType, Lexicon, isMonthlyClicked) {
	if ($("#gridSearchablePDF").data("kendoGrid") != undefined) {
		//$("#gridSearchablePDF").data("kendoGrid").dataSource.data([]);
		//$('#gridSearchablePDF').data('kendoGrid').refresh();
		$("#gridSearchablePDF").data("kendoGrid").destroy()
		$("#gridSearchablePDF").empty();
	}
	$("#parentGridSearchablePDF").show();
	$("#gridSearchablePDF").kendoGrid({
		dataSource: {
			type: "json",
			transport: {
				read: {
					url: BCMConfig.API_GETSEARCHABLE_PDF_DATA,
					beforeSend: CommonJS.BeforeSendAjaxCall,
					async: false
				},
				parameterMap: function (options) {
					return "parametersJson=" + JSON.stringify(options) + "&FromDate=" + fromDate + "&ToDate=" + toDate + "&MetaDataType=" + actionType + "&Lexicon=" + Lexicon + "&IsMonthlyClicked=" + isMonthlyClicked;
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
						WebSiteURL: {
							type: "string"
						},
						PDFURL: {
							type: "string"
						},
						CreatedString: {
							type: "string"
						},
						ProprietaryTags: {
							type: "string"
						}
					}
				}
			},
			pageSize: 10,
			serverPaging: true,
			serverFiltering: false,
			serverSorting: true
		},
		filterable: false,
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
	field: "Created",
	title: CreateDate,
	width: 120
},
	{
		field: "ProprietaryTags",
		title: ActionType,
		width: 120
	},
	{
		field: "WebSiteURL",
		title: WebSiteURL,
		template: "<a href=\"#=WebSiteURL #\" data-toggle='tooltip' target='_blank'>#=WebSiteURL #</a>",
		width: 600,
	},
{
	field: "PDFURL",
	title: PDFURL,
	template: "#if (PDFURL!='') {# <a href=\"#=PDFURL #\" data-toggle='tooltip' target='_blank'><i class='fa fa-file-pdf' aria-hidden='true'></i></a> #} else {#  #}#",
	width: 100,
	sortable: false
}

		]
	});
	$("#gridSearchablePDF").data("kendoGrid").refresh();

	if (isCustomer == "True") {
		var grid = $("#gridSearchablePDF").data("kendoGrid")
		grid.hideColumn("PDFURL");
		$("#gridSearchablePDF .k-grid-content") //content
        .find("colgroup col")
        .eq(2)
        .css({ width: 765 });
	}
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
};

function RenderLineMonthWiseChart(actionTypeChartResponseData, Lexicon) {
	var axisMin = 0;
	var axisMax = 15;
	var width = $('#lexiconTypechart').width();
	if (width < 768) {
		var mobileViewToShow = numberOfColumns_Mobile;
		var axisMin = actionTypeChartResponseData.length > mobileViewToShow ? actionTypeChartResponseData.length - mobileViewToShow : 0;
		var axisMax = actionTypeChartResponseData.length;
	}
	else {
		var axisMin = actionTypeChartResponseData.length > numberOfColumns_Web ? actionTypeChartResponseData.length - numberOfColumns_Web : 0;
		var axisMax = actionTypeChartResponseData.length;
	}

	function updateRange(e) {
		var axis = e.sender.getAxis('axis')
		var range = axis.range()
		axisMin = range.min;
		axisMax = range.max;
	}

	function restoreRange(e) {
		e.sender.options.categoryAxis.min = axisMin;
		e.sender.options.categoryAxis.max = axisMax;
	}

	////working
	$("#lexiconTypechart").kendoChart({
		renderAs: "canvas",
		title: {
			text: "Policy Risk Time Series: " + Lexicon,
			font: "bold  15px sans-serif"
		},
		dataSource: actionTypeChartResponseData,
		legend: {
			visible: true,
			position: "bottom",
			itemclick: function (e) {
			}
		},
		seriesClick: function (e) {
			OnMonthSeriesClick(e, Lexicon)
		},
		seriesDefaults: {
			type: "line",
			style: "smooth",
			labels: {
				background: "transparent"
			}
		},
		series: GetSeries(GetActionTypeArray(actionTypeChartResponseData), actionTypeChartResponseData),
		valueAxis: {
			line: {
				visible: false
			},
			minorGridLines: {
				visible: true
			}
		},
		categoryAxis: {
			name: "axis",
			min: axisMin,
			max: axisMax,
			categories: GetCategoryAxisVals(actionTypeChartResponseData),
			majorGridLines: {
				visible: false
			},
			labels: {
				rotation: -55
			}
		},
		tooltip: {
			visible: false,
			template: "#= series.name #: #= value #"
		},
		render: function (e) {
			var chart = e.sender;
			var titleText = chart.options.title.text;
			////var width = chart._plotArea.box.width();
			var width = $('#lexiconTypechart').width();
			var wordsPerLine = Math.round(width / 100) * 2;
			var arr = titleText.split(" ");
			var newTitle = "";
			for (var i = 0; i < arr.length; i++) {
				if ((i + 1) % wordsPerLine == 0) {
					newTitle += arr[i].trim() + "\n";
				} else {
					newTitle += arr[i].trim() + " ";
				}
			}

			ChartStyle.SetChartFonts(chart);
			if (width < 768) {
				//chart.options.series[0].padding = 100
				//chart.options.series[0].padding = 100
				chart.options.tooltip.template = "#= series.name #: \n #= value#";
				chart.options.tooltip.visible = false;
				chart.options.seriesDefaults.labels.template = "#if(value>0){# #: value # #}#"
				chart.options.seriesDefaults.labels.visible = true;
				chart.options.seriesDefaults.labels.rotation = 270;
				chart.options.seriesDefaults.labels.format = "{0}";
				chart.options.seriesDefaults.labels.position = "center";
			}
			else {
				for (var i = 0; i < chart.options.series.length; i++) {
					chart.options.series[i].tooltip.visible = true;
				}
				chart.options.seriesDefaults.labels.template = "#= series.name #: \n #= value#";
			}

			chart.options.title.text = newTitle;
			chart._events.render = null;
			chart.refresh();
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE)
		},
		pannable: {
			lock: "y"
		},
		zoomable: false
	});
}

function OnMonthSeriesClick(e, Lexicon) {
	var data = e.sender.dataSource.data();
	$.when(CommonJS.SetPageLoader(PageLoaderActivity.SHOW))
	.then(function () {
		var fromDate = data[e.point.categoryIx].fromDate;
		var toDate = data[e.point.categoryIx].toDate;
		var SearchableModel = {
			MonthDate: e.category, Lexicon: Lexicon, FromDate: fromDate, ToDate: toDate
		};
		$.ajax({
			url: BCMConfig.API_GETSEARCHABLE_PDF_DATA_FOR_LINECHART,
			beforeSend: CommonJS.BeforeSendAjaxCall,
			method: 'POST',
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			data: JSON.stringify(SearchableModel),
			processdata: true,
			async: true,
			success: function (response) {
				var lineChartArray = []
				if (response.length > 0) {
					lineChartArray = response;
				}
				RenderLineChart(response, e.category, Lexicon);
				loadKendoGrid(fromDate, toDate, "", Lexicon, true);


				CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			},
			error: function (e) {
				CommonJS.HandleErrorStatus(e.status);
				CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			}
		});
	});
}

function OnLineSeriesClick(e, Lexicon) {
	$.when(CommonJS.SetPageLoader(PageLoaderActivity.SHOW))
	.then(function () {
		loadKendoGrid(e.category, e.category, e.series.name, Lexicon, false);
		$('html, body').animate({
			scrollTop: $("#parentGridSearchablePDF").offset().top
		}, 1000);
	});
}

function RenderLineChart(actionTypeChartResponseData, MonthDate, Lexicon) {
	var axisMin = 0;
	var axisMax = 15;
	var width = $('#lexiconTypechart').width();
	if (width < 768) {
		var mobileViewToShow = numberOfColumns_Mobile;
		var axisMin = actionTypeChartResponseData.length > mobileViewToShow ? actionTypeChartResponseData.length - mobileViewToShow : 0;
		var axisMax = actionTypeChartResponseData.length;
	}
	else {
		var axisMin = actionTypeChartResponseData.length > numberOfColumns_Web ? actionTypeChartResponseData.length - numberOfColumns_Web : 0;
		var axisMax = actionTypeChartResponseData.length;
	}

	function updateRange(e) {
		var axis = e.sender.getAxis('axis')
		var range = axis.range()
		axisMin = range.min;
		axisMax = range.max;
	}

	function restoreRange(e) {
		e.sender.options.categoryAxis.min = axisMin;
		e.sender.options.categoryAxis.max = axisMax;
	}

	////working
	$("#lexiconTypeDatechart").kendoChart({
		renderAs: "canvas",
		title: {
			text: "Policy Risk Time Series for the month of " + MonthDate + ": " + Lexicon,
			font: "bold  15px sans-serif"
		},
		legend: {
			visible: true,
			position: "bottom",
			itemclick: function (e) {
			}
		},
		seriesClick: function (e) {
			OnLineSeriesClick(e, Lexicon)
		},
		seriesDefaults: {
			type: "line",
			style: "smooth",
			labels: {
				background: "transparent"
			}
		},
		series: GetSeries(GetActionTypeArray(actionTypeChartResponseData), actionTypeChartResponseData, true),
		valueAxis: {
			line: {
				visible: false
			},
			minorGridLines: {
				visible: true
			}
		},
		categoryAxis: {
			name: "axis",
			min: axisMin,
			max: axisMax,
			categories: GetCategoryAxisVals(actionTypeChartResponseData),
			majorGridLines: {
				visible: false
			},
			labels: {
				rotation: -55
			}
		},
		tooltip: {
			visible: false,
			template: "#= series.name #: #= value #"
		},
		render: function (e) {
			$('html, body').animate({
				scrollTop: $("#parentDateChartSearchablePDF").offset().top
			}, 1000);
			var chart = e.sender;
			var titleText = chart.options.title.text;
			////var width = chart._plotArea.box.width();
			var width = $('#lexiconTypechart').width();
			var wordsPerLine = Math.round(width / 100) * 2;
			var arr = titleText.split(" ");
			var newTitle = "";
			for (var i = 0; i < arr.length; i++) {
				if ((i + 1) % wordsPerLine == 0) {
					newTitle += arr[i].trim() + "\n";
				} else {
					newTitle += arr[i].trim() + " ";
				}
			}

			ChartStyle.SetChartFonts(chart);
			if (width < 768) {
				//chart.options.series[0].padding = 100
				//chart.options.series[0].padding = 100
				chart.options.tooltip.template = "#= series.name #: \n #= value#";
				chart.options.tooltip.visible = false;
				chart.options.seriesDefaults.labels.template = "#if(value>0){# #: value # #}#"
				chart.options.seriesDefaults.labels.visible = true;
				chart.options.seriesDefaults.labels.rotation = 270;
				chart.options.seriesDefaults.labels.format = "{0}";
				chart.options.seriesDefaults.labels.position = "center";
			}
			else {
				for (var i = 0; i < chart.options.series.length; i++) {
					chart.options.series[i].tooltip.visible = true;
				}
				chart.options.seriesDefaults.labels.template = "#= series.name #: \n #= value#";
			}

			chart.options.title.text = newTitle;
			chart._events.render = null;
			chart.refresh();
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE)
		},
		pannable: {
			lock: "y"
		},
		zoomable: false
		//zoomable: {
		//	mousewheel: {
		//		lock: "y"
		//	},
		//	selection: {
		//		lock: "y"
		//	}
		//},
		//zoom: updateRange,
		//drag: updateRange,
		//dataBound: restoreRange
	});
}

function GetActionTypeArray(response) {
	var actionTypeArray = [];
	if (response.length > 0) {
		var firstObj = response[0];
		var actionTypeArray = firstObj.proprietaryModel;

		actionTypeArray = actionTypeArray.map(function (element, index) {
			return element.proprietaryTagType;
		});
		return actionTypeArray;
	}
}

var colorDict = {
	Rhetoric: "#0070C0", //Blue
	Action: "#00B050",//Action
	Judicial: "#7030A0", //purple
	Data: "#FFC000", // Data
	Leaks: "#C00000" //Red
};

function GetSeriesColor(actionTypeVal) {
	return colorDict[actionTypeVal];
}

function GetSeries(inputArray, response, isMonthlyChart) {
	if (isMonthlyChart == true) {
		$("#parentDateChartSearchablePDF").show();
		$("#lexiconTypeDatechart").show();
		////$("#note").show();
		$("#noDataDateNotification").hide();
		if (response.length == 0) {
			$("#lexiconTypeDatechart").hide();
			////$("#note").hide();
			$("#noDataDateNotification").show();
			$("#parentGridSearchablePDF").hide();
			$("#parentDateChartSearchablePDF").hide();
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			return;
		}
	} else {
		$("#lexiconTypechart").show();
		////$("#note").show();
		$("#noDataNotification").hide();
		if (response.length == 0) {
			$("#lexiconTypechart").hide();
			////$("#note").hide();
			$("#noDataNotification").show();
			$("#parentGridSearchablePDF").hide();
			$("#parentDateChartSearchablePDF").hide();
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			return;
		}
	}
	var seriesArray = [];
	if (inputArray == undefined) {
		CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
		return;
	}
	inputArray.forEach(function (actionTypeVal, actionTypeIndex) {
		tempseriesArray = response.map(function (element, index) {
			if (element.proprietaryModel.length > 0) {
				var tempActionTypeObj = element.proprietaryModel.filter(function (tempActionType, index) {
					return tempActionType.proprietaryTagType == actionTypeVal;
				});
				return tempActionTypeObj[0].proprietaryTagValue;
			}
			else {
				return null;
			}
		});
		var seriesToAdd = {
			name: actionTypeVal,
			data: tempseriesArray,
			color: GetSeriesColor(actionTypeVal),
			gap: parseFloat(5, 20),
			markers: {
				visible: true,
				background: GetSeriesColor(actionTypeVal),
				size: 3
			}
		};
		seriesArray.push(seriesToAdd);
	});
	return seriesArray;
}

function GetCategoryAxisVals(response) {
	var categoryArray = response.map(function (element, index) {
		return element.createdString;  //element.combinationValue ? (element.lexicon + " +" + element.combinationValue) : element.lexicon;
	});
	return categoryArray;
}

function BindLexiconAutoComplete() {
	var noResultFlag = $('#lexiconNoResultFlag');
	$("#txtLexicon").autocomplete({
		source: function (request, response) {
			$.ajax({
				url: BCMConfig.API_LEXICON_DROPDOWNLIST,
				beforeSend: CommonJS.BeforeSendAjaxCall,
				method: "GET",
				data: {
					searchTerm: request.term
				},
				dataType: "json",
				success: function (data) {
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
								data: el.value
							};
						}));
						noResultFlag.val(1);
					}
				},
				error: function (err) {
				}
			});
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
		},
		minLength: 3,
		change: function (event) {
			//if ($('#lexiconHiddenVal').val() == '')
			//	$(this).val('')
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
			$('#lexiconHiddenVal').val(ui.item.value);
		},
		focus: function (event, ui) {
			if (noResultFlag.val() == '0') {
				event.preventDefault();
			}
		}
	})
	CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}