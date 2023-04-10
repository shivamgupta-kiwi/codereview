var lexiconIssueArray = [];
var processId = "";
var isDateChange = false;
//var lexiconChartArray = [];
var actionTypeChartResponseData = [];
var lexiconTermsChartResponceData = [];
//var actionTypeArray = [];
//var seriesArray = [];
var chartDataArray = [];
var selectedLexiconArray = [];
var lexiconTypeArray = [];
var lexiconChartResponseData = [];
var checkBoxSelectionArray = [];
var colorArray = ['#0070C0', '#C00000', '#FFC000', '#00B050', '#7030A0'];
var title = "";
var refHashId = "";
var date = "";
var key = "";
var isDirect = "";
var lexiconTypeHashId = "";

var colorDict = {
  Rhetoric: "#0070C0", //Blue
  Action: "#00B050",//Action
  Judicial: "#7030A0", //purple
  Data: "#FFC000", // Data
  Leaks: "#C00000" //Red
};

function ClearGlobalVar() {
  refHashId = "";
  date = "";
  key = "";
  isDirect = "";
}
function BindDashboard(RefHashId, Date, Key, IsDirect) {
  ClearGlobalVar();

  refHashId = RefHashId;
  date = Date;
  key = Key;
  isDirect = IsDirect;

  $.when(CommonJS.SetPageLoader(PageLoaderActivity.SHOW))
    .then(BindDateTimeTextbox())
    .then(RenderLexiconTypeChart());
}

function SetCheckBoxSelectionDict() {
  checkBoxSelectionArray = [];
  $('input[type="checkbox"]:checked').each(function () {
    checkBoxSelectionArray.push(this.value)
  });
}

function OnCheckBoxChange(target) {
  var element = $(target);
  if (element.is(":checked")) {
    if (checkBoxSelectionArray.length < numberOfColumns) {
      checkBoxSelectionArray.push(element.val());
    }
    else if (checkBoxSelectionArray.length == numberOfColumns) {
      toastr.info("You can not select more than " + numberOfColumns + " lexicons at a time");
      // element.checked = !element.checked;
      $(element).is(":checked")
      {
        $(element).prop("checked", false);
      }
    }
  }
  else {
    var index = checkBoxSelectionArray.indexOf(element.val());
    if (index > -1) {
      checkBoxSelectionArray.splice(index, 1);
    }
  }
}

$("#btnSubmit").click(function () {
  $("#InvalidUrlNotification").hide();
  //var lexiconsModel = BindData();
  $.when(CommonJS.SetPageLoader(PageLoaderActivity.SHOW))
    .then(RenderLexiconTypeChart());
});

function BindData() {
  
  var viewModel = {
    SelectedLexicons: checkBoxSelectionArray,
    SelectedDate: $('#txtDateTime').val(),
    IsAggregateDisplay: false
  };

  return viewModel;
}

function GetModel() {
  var DashboardModel = {
    LexiconTypeHashId: "",
    IsAggregateDisplay: true,
    SelectedDate: $('#txtDateTime').val(),
    ScanDate: date,
    RefHashId: refHashId,
    Key: key,
    IsDirect: isDirect
  };
  return DashboardModel;
}


function DisableLexiconSelection() {
  $('[type="checkbox"]').prop('checked', false);
  $('[type="checkbox"]').prop('disabled', true);
}

function GetLexiconTypeData() {

  actionTypeChartResponseData = [];
  var DashboardModel = GetModel();

  $.ajax({
    url: BCMConfig.API_GET_CHART_LEXICON_VALUES,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    method: 'POST',
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(DashboardModel),
    processdata: true,
    async: false,
    success: function (response) {
      if (response.length > 0) {
        actionTypeChartResponseData = response;
      }
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
    }
  });
}

function RenderLexiconTypeChart() {
  GetLexiconTypeData();

  ////working
  var selectedDate = $('#txtDateTime').val();
  $("#lexiconTypechart").kendoChart({
    title: {
      text: "Policy Risk Indicators: [Date: " + selectedDate + "]",
      font: "bold  15px sans-serif"
    },
    legend: {
      visible: true,
      position: "bottom",
      itemclick: function (e) {
      }
    },
    seriesClick: OnLexiconTypeSeriesClick,
    render: onRender,
    seriesDefaults: {
      type: "column",
      stack: true,
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
      categories: GetCategoryAxisVals(actionTypeChartResponseData, true),
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
    }
  });
}

function OnLexiconTypeSeriesClick(e) {
  DisableLexiconSelection();
  title = "";
  console.log(e.series.name + '+' + e.category);
  var clickedObj = actionTypeChartResponseData.filter(function (element, index) {
    return element.lexiconType == e.category;
  });

  lexiconTypeHashId = clickedObj[0].lexiconTypeHashId;
  $('#selectedLexiconTypeHiddenVal').val(lexiconTypeHashId);
  $('#' + lexiconTypeHashId).parent().parent().parent().find('.checkBoxPanel').find('.checkboxDiv').find('[type="checkbox"]').prop('disabled', false);
  GetPanelLexiconTermsBasedOnLexiconType(lexiconTypeHashId); ////get LexiconTerm list based on lexicon type
  GetLexiconWiseActionTypeValues(lexiconTypeHashId);
  SetDeafultCheckBoxSelection();
  title = "Policy Risk Indicators For " + e.category + " [Date: " + $('#txtDateTime').val() + "]"
  RenderLexiconTermsChart(title);
  $('#btnFilter').show();
  $('#btnDefaultFilter').show();
  var pageHight = 500;
  window.scrollBy(0, pageHight);

}

function GetLexiconTypeCategotyArray(lexiconTypeArray) {
  var lexiconTypeCategotyArray = lexiconTypeArray.map(function (element, index) {
    return element.lexiconType;
  });
  return lexiconTypeCategotyArray;
}

function GetLexiconTypeSeriesArray() {
  var series = [];
  var lexiconTypeDataArray = lexiconTypeArray.map(function (element, index) {
    return element.value;
  });
  var seriesObj = {
    name: "Lexicon Types",
    data: lexiconTypeDataArray,
    gap: parseFloat(3, 10)
  };
  series.push(seriesObj);
  return series;
}

function GetPanelLexiconTermsBasedOnLexiconType(lexiconTypeHashId) {
  $.ajax({
    url: BCMConfig.API_DASHBOARD_GET_LEXICONTERMS + "?selectedDate=" + $('#txtDateTime').val() + "&lexiconTypeHashId=" + lexiconTypeHashId,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    async: false,
    processdata: true,
    success: function (response) {
      console.log(response);
      if (response.length > 0) {
        lexiconIssueArray = [];
        lexiconIssueArray = response;
        RenderLexiconPanel(lexiconIssueArray);

        $('#accordion1 .collapse').collapse('show');
      }

    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
    }
  });
}
function RenderLexiconPanel(lexiconIssueArray) {
  $('#accordion1').html('');
  lexiconIssueArray.forEach(function (lexiconTypeObj, lexiconTypeObjIndex) {
    var tempPanelHtml = GetPanelHeadingHTML().replace("{{panelHref}}", "collapse_" + lexiconTypeObjIndex).replace("{{PanelHeading}}", lexiconTypeObj.lexiconType).replace("{{LexiconTypeHashId}}", lexiconTypeObj.lexiconTypeMasterHashId);
    var panelElement = $(document.createElement('div')).addClass("panel panel-default filter-lexicons");
    panelElement.html(tempPanelHtml);
    var panelBodyInnerDivElement = $('<div class="checkboxDiv" style="max-height:180px;"></div>');
    var panelBodyMainDivElement = $('<div class="checkBoxPanel panel-collapse collapse in filter-lexicons-content" style="overflow-y:scroll"></div>');
    var checkboxElement = '';
    lexiconTypeObj.dashBoardLexiconTermsList.forEach(function (lexiconTermObj, lexiconTermObjIndex) {
      if (lexiconTermObj.value > 0) {
        checkboxElement = $('<div class="checkbox-container"><label class="checkbox-nan"><input onchange="OnCheckBoxChange(this)" type="checkbox" name="' + lexiconTermObj.lexiconTerm + ' ' + (lexiconTermObj.combinationValue != null && lexiconTermObj.combinationValue != '' && lexiconTermObj.combinationValue != "" ? ('+ ' + lexiconTermObj.combinationValue) : '') + '" id="' + lexiconTermObj.lexiconTermssHashId + '" value="' + lexiconTermObj.lexiconTermssHashId + '">' + "<span>" + lexiconTermObj.lexiconTerm + ' ' + (lexiconTermObj.combinationValue != null && lexiconTermObj.combinationValue != '' && lexiconTermObj.combinationValue != "" ? ('+ ' + lexiconTermObj.combinationValue) : '') + '</span></input> <span class="checkmark"></span></label></div>');
      }
      else {
        checkboxElement = $('<div class="checkbox-container"><label><input onchange="OnCheckBoxChange(this)" type="checkbox" name="' + lexiconTermObj.lexiconTerm + ' ' + (lexiconTermObj.combinationValue != null && lexiconTermObj.combinationValue != '' && lexiconTermObj.combinationValue != "" ? ('+ ' + lexiconTermObj.combinationValue) : '') + '" id="' + lexiconTermObj.lexiconTermssHashId + '" value="' + lexiconTermObj.lexiconTermssHashId + '">' + lexiconTermObj.lexiconTerm + ' ' + (lexiconTermObj.combinationValue != null && lexiconTermObj.combinationValue != '' && lexiconTermObj.combinationValue != "" ? ('+ ' + lexiconTermObj.combinationValue) : '') + ' </input><span class="checkmark"></span></label></div>');
      }
      panelBodyInnerDivElement.append(checkboxElement);
    });
    if (lexiconTypeObj.dashBoardLexiconTermsList.length > 0) {
      panelBodyMainDivElement.append(panelBodyInnerDivElement);
      panelElement.append(panelBodyMainDivElement);
    }
    $('#accordion1').append(panelElement);
  });
}

function GetPanelHeadingHTML() {
  return "<a  data-toggle=\"collapse\" data-parent=\"#accordion1\" href=\"#{{panelHref}}\"><div class=\"panel-heading\"><h5 id=\"{{LexiconTypeHashId}}\" class=\"panel-title\">{{PanelHeading}}</h5></div></a>"
}

function GetLexiconWiseActionTypeValues(lexiconTypeHashId) {

  var DashboardModel = GetModel();
  DashboardModel.IsAggregateDisplay = false;
  DashboardModel.LexiconTypeHashId = lexiconTypeHashId;

  $.ajax({
    url: BCMConfig.API_GET_CHART_LEXICON_VALUES,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    method: 'POST',
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(DashboardModel),
    processdata: true,
    async: false,
    success: function (response) {
      console.log(response);
      lexiconChartResponseData = [];
      lexiconChartResponseData = response;
    },
    complete: function () {
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
    }
  });
}

function GetCategoryAxisVals(response, isLexiconTypeChart) {
  if (isLexiconTypeChart) {
    var categoryArray = response.map(function (element, index) {
      return element.lexiconType;
    });
    return categoryArray;
  }
  else {
    var categoryArray = response.map(function (element, index) {
      return element.lexicon;  //element.combinationValue ? (element.lexicon + " +" + element.combinationValue) : element.lexicon;
    });
    return categoryArray;
  }
}

function GetActionTypeArray(response) {
  var actionTypeArray = [];
  if (response.length > 0) {
    var firstObj = response[0];
    var actionTypeArray = firstObj.actionType;

    actionTypeArray = actionTypeArray.map(function (element, index) {
      return element.name;
    });
    return actionTypeArray;
  }
}
function GetSeries(inputArray, response, isLexiconTypeChart) {
  $("#lexiconTypechart").show();
  $("#noDataNotification").hide();
  if (response.length == 0) {
    $("#lexiconTypechart").hide();
    $("#noDataNotification").show();
    CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    return;
  }
  var seriesArray = [];
  if (inputArray == undefined) {
    CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    return;
  }
  inputArray.forEach(function (actionTypeVal, actionTypeIndex) {
    tempseriesArray = response.map(function (element, index) {
      if (element.actionType.length > 0) {
        var tempActionTypeObj = element.actionType.filter(function (tempActionType, index) {
          return tempActionType.name == actionTypeVal;
        });
        return tempActionTypeObj[0].value;
      }
      else {
        return null;
      }
    });
    var seriesToAdd = {
      name: actionTypeVal,
      data: tempseriesArray,
      color: GetSeriesColor(actionTypeVal),
      gap: parseFloat(3, 10)
    };
    seriesArray.push(seriesToAdd);
  });
  return seriesArray;
}


function GetSeriesColor(actionTypeVal) {
  return colorDict[actionTypeVal];
}


function GetChartDataOnSeriesClick(actionName, country) {
  var tempArrayBasedOnActionType;
  var tempArrayBasedOnCountry;
  if (actionTypeChartResponseData.length > 0) {
    tempArrayBasedOnCountry = actionTypeChartResponseData.filter(function (element, index) {
      return element.Country == country;
    });
  }
  if (tempArrayBasedOnCountry[0].ActionType.length > 0) {
    tempArrayBasedOnActionType = tempArrayBasedOnCountry[0].ActionType.filter(function (element, index) {
      return element.Name == actionName;
    });
  }
  var tempJson = {
    IsEuropeanUnion: tempArrayBasedOnCountry[0].IsEuropeanUnion,
    CountryHashId: tempArrayBasedOnCountry[0].CountryHashId,
    Country: tempArrayBasedOnCountry[0].Country,
    ActionType: tempArrayBasedOnActionType[0].Name,
    ActionTypeHashId: tempArrayBasedOnActionType[0].ActionTypeHashId
  };
  return tempJson;
}

function RedirectToUrl(url, actionHashId, lexiconHashId) {

  url = url + "?selectedDate=" + $('#txtDateTime').val() + "&actionHashId=" + actionHashId + "&lexiconHashId=" + lexiconHashId;
  window.open(url);
}

function RenderLexiconTermsChart(title) {
  $("#DivlexiconTermsChart").show();
  $("#lexiconTermsChart").kendoChart({
    title: {
      text: title,
      font: "bold  15px sans-serif"
    },
    legend: {
      visible: true,
      position: "bottom",
      itemclick: function (e) {
      }
    },
    seriesClick: onSeriesClick,
    seriesHover: onSeriesHover,
    dataBound: onDataBound,
    axisLabelClick: onAxisLabelClick,
    legendItemClick: onLegendItemClick,
    legendItemHover: onLegendItemHover,
    plotAreaClick: onPlotAreaClick,
    plotAreaHover: onPlotAreaHover,
    render: onRender,
    dragStart: onDragStart,
    drag: onDrag,
    dragEnd: onDragEnd,
    zoomStart: onZoomStart,
    zoom: onZoom,
    zoomEnd: onZoomEnd,
    seriesDefaults: {
      type: "column",
      stack: true,
      labels: {
        background: "transparent"
      }
    },
    series: GetSeries(GetActionTypeArray(lexiconChartResponseData), lexiconChartResponseData, false),
    valueAxis: {
      line: {
        visible: false
      },
      minorGridLines: {
        visible: true
      }
    },
    categoryAxis: {
      categories: GetCategoryAxisVals(lexiconChartResponseData, false),
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
        chart.options.tooltip.template = "#= series.name #: \n #= value#";
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
        chart.options.seriesDefaults.labels.template = "#= series.name #: \n #= value#"
      }

      chart.options.title.text = newTitle;
      chart._events.render = null;
      chart.refresh();
    }
  });


  $("#DivlexiconTermsFilter").show();

}

function onSeriesClick(e) {
  //var obj = GetChartDataOnSeriesClick(e.series.name, e.category);
  console.log(e.series.name + ":" + e.category)
  var clickedObj = lexiconChartResponseData.filter(function (element, index) {
    return element.lexicon == e.category;
  });
  var lexiconHashId = clickedObj[0].lexiconHashId;
  var actionTypeObj = clickedObj[0].actionType.filter(function (element, index) {
    return element.name == e.series.name;
  });
  var actionTypeHashId = actionTypeObj[0].actionTypeHashId;
  RedirectToUrl($('#chartDetailUrl')[0].href, actionTypeHashId, lexiconHashId);
}

function onSeriesHover(e) {
  console.log(kendo.format("Series hover :: {0} ({1}): {2}",
					e.series.name, e.category, e.value));
}

function onDataBound(e) {
  console.log("Data bound");
}

function onAxisLabelClick(e) {
  console.log(kendo.format("Axis label click :: {0} axis : {1}",
					e.axis.type, e.text));
}

function onLegendItemClick(e) {
  console.log("Legend item click :: {0}", e.text);
}

function onLegendItemHover(e) {
  console.log(kendo.format("Legend item hover :: {0}", e.text));
}

function onPlotAreaClick(e) {
  console.log(kendo.format("Plot area click :: {0} : {1:N0}",
					e.category, e.value
	));
  console.log("onPlotAreaClick:" + e);
}

function onPlotAreaHover(e) {
}

function onRender(e) {
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
  console.log("Render");
}

function onDragStart(e) {
  console.log("Drag start");
}

function onDrag(e) {
  console.log("Drag");
}

function onDragEnd(e) {
  console.log("Drag end");
}

function onZoomStart(e) {
  console.log("Zoom start");
}

function onZoom(e) {
  console.log(kendo.format("Zoom :: {0}", e.delta));
  e.originalEvent.preventDefault();
}

function onZoomEnd(e) {
  console.log("Zoom end");
}

function SetDeafultCheckBoxSelection() {
  var selectedCheckBoxArray = [];

  $.each(lexiconChartResponseData, function (index, lexiconObj) {
    var checkBoxDiv = $('[value=' + lexiconObj.lexiconHashId + ']').closest('div').closest('.checkboxDiv');
    selectedCheckBoxArray.push(lexiconObj.lexiconHashId);
    var elementDiv = $('[value=' + lexiconObj.lexiconHashId + ']').closest('div');
    var element = $('[value=' + lexiconObj.lexiconHashId + ']');
    var chechBoxName = element.attr('name');
    var chechBoxid = element.attr('id');
    var chechBoxval = element.attr('value');
    elementDiv.remove();
    if (lexiconObj.lexiconValuesSum > 0){
      checkBoxDiv.prepend($('<div class="checkbox-container"><label class="checkbox-nan"><input type="checkbox" onchange="OnCheckBoxChange(this)" name="' + chechBoxName + '" id="' + chechBoxid + '" value="' + chechBoxval + '">' + chechBoxName + '</input> <span class="checkmark"></span></label></div>'))
    }
    else {
      checkBoxDiv.prepend($('<div class="checkbox-container"><label><input type="checkbox" onchange="OnCheckBoxChange(this)" name="' + chechBoxName + '" id="' + chechBoxid + '" value="' + chechBoxval + '">' + chechBoxName + '</input> <span class="checkmark"></span> </label></div>'))
    }
    
    $('[value=' + lexiconObj.lexiconHashId + ']').prop('checked', true);
  });
  SetCheckBoxSelectionDict();
}

function BindDateTimeTextbox() {
  var Today = isDirect == "False" || isDirect == "" ? new Date() : date;
  
  $("#txtDateTime").kendoDatePicker({
    format: "MM/dd/yyyy",
    change: OnChangeDate,
    value: Today,
    //// specifies that DateInput is used for masking the input element
    dateInput: true,
    disableDates: function (date) {
      return date > new Date();
    }
  });
}

function OnChangeDate() {
  isDateChange = true;
  DisableLexiconSelection();
  $('#lexiconTypechart').html('');
  $('#lexiconTermsChart').html('');
  $('#accordion1').html('');
  checkBoxSelectionArray = [];
  $('#btnFilter').hide();
  $('#btnDefaultFilter').hide();
  $("#DivlexiconTermsChart").hide();
  $("#DivlexiconTermsFilter").hide();
}

$("#btnFilter").click(function () {
  $("#InvalidUrlNotification").hide();
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  var lexiconsModel = BindData();
  lexiconsModel.LexiconTypeHashId = lexiconTypeHashId;
  if (lexiconsModel.SelectedLexicons.length > 0) {
    $.ajax({
      url: BCMConfig.API_POST_CHART_LEXICON_VALUES,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(lexiconsModel),
      processdata: true,
      success: function (response) {
        if (response.length > 0) {
          lexiconChartResponseData = [];
          lexiconChartResponseData = response;
          RenderLexiconTermsChart(title);
        }
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      },
      complete: function (e) {
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        isDateChange = false;
      },
      error: function (e) {
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        CommonJS.HandleErrorStatus(e.status);
      }
    });
    var pageHight = 500;//document.body.scrollHeight + 300;
    window.scrollTo(0, pageHight);
  }
  else {
    CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    toastr.info(validationLexiconTerm);
  }

});

$("#btnDefaultFilter").click(function () {

  $("#InvalidUrlNotification").hide();
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  var lexiconsModel = BindData();
  lexiconsModel.LexiconTypeHashId = lexiconTypeHashId;
  
  if (lexiconsModel.SelectedLexicons.length > 0) {
    $.ajax({
      url: BCMConfig.API_POST_UPDATE_LEXICON_DEFAULTFILTER,
      beforeSend: CommonJS.BeforeSendAjaxCall,
      method: 'POST',
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(lexiconsModel),
      processdata: true,
      success: function (response) {
        toastr.info(response.errorMessage);
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      },
      complete: function (e) {
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        isDateChange = false;
      },
      error: function (e) {
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        CommonJS.HandleErrorStatus(e.status);
      }
    });
  }
  else {
    CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    toastr.info(validationLexiconTerm);
  }
});

function AuthenticateUserForDashboard(refUserHashId, scanDate, hashKey, isDirectDashboard) {
  ClearGlobalVar();

  var vitualDashboardModel = {
    ScanDate: scanDate,
    RefHashId: refUserHashId,
    Key: hashKey
  };

  $.ajax({
    url: BCMConfig.API_AUTHENTICATE_USER_FOR_VIRTUAL_DASHBOARD,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    method: 'POST',
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(vitualDashboardModel),
    processdata: true,
    success: function (response) {
      if (response) {
        ClearGlobalVar();
        refHashId = refUserHashId;
        date = scanDate;
        key = hashKey;
        isDirect = isDirectDashboard;

        $.when(CommonJS.SetPageLoader(PageLoaderActivity.SHOW))
		 .then(BindDateTimeTextbox())
		 .then(RenderLexiconTypeChart());
      }
      else {
        BindDateTimeTextbox();
        $("#noDataNotification").hide();
        $("#InvalidUrlNotification").show();
        ////window.location.href = VirtualdashboardURL;
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    complete: function (e) {
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      CommonJS.HandleErrorStatus(e.status);
    }
  });
}



