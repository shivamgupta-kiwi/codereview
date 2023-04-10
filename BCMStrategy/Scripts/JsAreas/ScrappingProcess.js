var viewModelScrapping;
var tableRow = "";
var tableBody = "";
var tableRowCount = 1;
var innerTable_tableRow = "";
var innerTable_tableBody = "";
var innerTable_tableRowCount = 1;



$(document).ready(function () {

	var params = getParams(window.location.href);
	if (params['ProcessId']) {
		var ProcessId = params['ProcessId']
		LoadData(ProcessId);
	}

	$('.collapsible').collapsible();
	$('[data-toggle="tooltip"]').tooltip();
	$("#accordion1").html("");

})

$(function () {
	$(document).tooltip({
		selector: '.hoverTip'
	});
});

$("#btnGetData").click(function () {
	LoadData(0);
});

function LoadData(ProcessId) {

	tableBody = "";
	tableRow = "";
	$.ajax({
		url: BCMConfig.API_GETALLLIST_SCRAPPED_SUMMARY_DATA,
		beforeSend: CommonJS.BeforeSendAjaxCall,
		data: { 'webSiteType': webSiteType, 'processId': ProcessId },
		method: 'GET',
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		processdata: true,
		success: function (response) {
			tableBody = GenerateSummaryHeader();
			$.each(response.Data, function (index, data) {
				tableRow += GenerateTableRow(data, ProcessId);
			})
			tableBody += tableRow + "</tbody>";
			$("#tblSummary").html("");
			$("#tblSummary").append(tableBody);
			if (response.Data.length > 0) {
				$("#lastScannedDate").text(response.Data[0].LastScanDate);
				$("#previousScannedDate").text(response.Data[0].PreviousScanDate);
			}
			$(".contentHeader").css({ "display": "block" });
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);

		},
		error: function (e) {
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			CommonJS.HandleErrorStatus(e.status);
		}
	});

}

function GetDetails(webSiteId, processEventId) {
	innerTable_tableRowCount = 1;
	CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
	$.ajax({
		url: BCMConfig.API_GETALLLIST_SCRAPPED_DATA,
		beforeSend: CommonJS.BeforeSendAjaxCall,
		data: { 'webSiteHashId': webSiteId, 'webSiteType': webSiteType, 'processEventId': processEventId },
		method: 'GET',
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		processdata: true,
		success: function (response) {
			console.log(response);
			//call successfull
			$("#accordion1").html("");
			var displayScrappingDetail = "";
			$.each(response.Data.scrappingModelList, function (index, value) {
				displayScrappingDetail += formatAccordion(value);
			})
			$("#accordion1").append(displayScrappingDetail);
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);

		},
		error: function (e) {
			CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
			CommonJS.HandleErrorStatus(e.status);
			//error occurred
		}
	});
}

function toggleIcon(e) {
	$(e.target)
	  .prev('.panel-heading')
	  .find(".more-less")
	  .toggleClass('fa-plus fa-minus');
}

$('.panel-group').on('hidden.bs.collapse', toggleIcon);
$('.panel-group').on('shown.bs.collapse', toggleIcon);

function formatAccordion(data) {
	var parentURL = ""
	var parentWebsiteId = ""
	var html = '';
	html = '<div class="panel panel-default">';
	if (data.ChildURLList.length != 0) {
		html += '<div class="panel-heading ">';
		html += '  <h6 class="panel-title"> ';
		html += '<a data-toggle="collapse" data-parent="#accordion1" href="#outerAccordion' + data.ScrappingId + '">';
		html += '  <span class="fa fa-plus"></span> &nbsp;';

		html += formatLength(data.WebsiteURL) + '<a href= ' + data.WebsiteURL + ' target="_blank" style="padding-right: 20px;" class="clear-content"> Go to webpage </a></h6>';
		parentURL = data.WebsiteURL;
		parentWebsiteId = data.WebSiteHashId;
	}
	else {
		html += '<a data-parent="#accordion1">';
		html += '  <div class="panel-heading ">';
		html += '  <h6 class="panel-title"><span class="hoverTip" data-toggle="tooltip" data-original-title="No newer link(s) has been identified!" title="No newer link(s) has been identified!"><span class="fa fa-ban" style="color:red;"></span>&nbsp;' + data.WebsiteURL + '</span><a href= ' + data.WebsiteURL + ' target="_blank" style="padding-right: 20px;" class="clear-content"> Go to webpage </a></h6>';
		html += '</div>';
		html += '</a>';
		html += '</div>';
		return html;
	}

	html += '</div>';
	html += '</a>';
	html += '<div id="outerAccordion' + data.ScrappingId + '" class="panel-collapse collapse in">'
	html += '  <div class="panel-body">'
	html += '<div class="panel-group" id="accordion2">';

	var count = 1;
	if (data.ChildURLList.length != 0) {
		var ScrappedHtml = "";
		var scan1 = "";
		var scan2 = "";
		var proprietoryTagFound = 0;
		var standardTagFound = 0;
		var lexiconFound = 0;
		var firstScan = 0;
		var standardFound = 0;
		var proprietory = 0;

		$.each(data.ChildURLList, function (index, value) {
			var tableRowContent = "";

			if (value.Content == "" || value.Content == null) {
				ScrappedHtml += '<div class="panel panel-default">';
				ScrappedHtml += '<a  data-parent="#accordion2" >'
				ScrappedHtml += '<div class="panel-heading">';
				ScrappedHtml += '<h6 class="panel-title"><span class="badge badge-pill badge-info">' + count + '</span>&nbsp;&nbsp;<span class="hoverTip" data-toggle="tooltip" data-original-title="No lexicon(s) term(s) found."><i class="fa fa-thumbs-down" style="color:red"></i>  &nbsp;' + formatLength(value.ScrappedWebsiteURL) + ' </span><a href= ' + value.ScrappedWebsiteURL + ' target="_blank" style="padding-right: 20px;" class="clear-content"> Go to webpage </a></h6>';
				ScrappedHtml += '</div>';
				ScrappedHtml += '</a>';
				ScrappedHtml += '</div>';
			}
			else {
				ScrappedHtml += '<div class="panel panel-default">';
				ScrappedHtml += '<a data-toggle="collapse" data-parent="#accordion2" href="#inneraccordion' + value.ScrappedId + '">'
				ScrappedHtml += '<div class="panel-heading">';
				ScrappedHtml += '<h6 class="panel-title"><span class="badge badge-pill badge-info">' + count + '</span>&nbsp;&nbsp; ';
				if (value.ProprietoryTagList.length > 0 && value.StandardTagData.CountryName != null && value.StandardTagData.SearchType != null) {
					ScrappedHtml += ' <i class="fa fa-thumbs-up" style="color:green"></i>';
				}
				else {
					ScrappedHtml += ' <i class="fa fa-thumbs-up" style="color:orange"></i>';
				}
				ScrappedHtml += ' &nbsp;' + formatLength(value.ScrappedWebsiteURL) + ' <a href= ' + value.ScrappedWebsiteURL + ' target="_blank" style="padding-right: 20px;" class="clear-content"> Go to webpage </a></h6>'
				ScrappedHtml += '</div>';
				ScrappedHtml += '</a>';
				ScrappedHtml += '<div id="inneraccordion' + value.ScrappedId + '" class="panel-collapse collapse in">';
				ScrappedHtml += '<div class="panel-body">'
				//ScrappedHtml += "<p><b><u>Lexicon Terms :</u></b></p><p>" + value.ContentString + "</p>";
				ScrappedHtml += "<p><b><u>Lexicon Terms :</u></b></p><p>" + GenerateLexiconTable(value) + "</p>";

				ScrappedHtml += SetProprietoryTags(parentURL, value);
				ScrappedHtml += '</div></div></div>';
			}
			count++;
		})
		html += ScrappedHtml;
		html += ' </div></div></div></div>';
	}
	return html;
}

function formatAccordioUnprocesedSite(data) {
	var UnprocessedSiteHtml = "";
	UnprocessedSiteHtml += '<div class="panel panel-default">';
	UnprocessedSiteHtml += '<a  data-parent="#accordion2" >'
	UnprocessedSiteHtml += '<div class="panel-heading">';
	UnprocessedSiteHtml += '  <h6 class="panel-title"><span class="hoverTip" data-toggle="tooltip" data-original-title="No newer link(s) has been identified!" title="No newer link(s) has been identified!"><span class="fa fa-ban" style="color:red;"></span>&nbsp;' + data.WebsiteURL + '</span><a href= ' + data.WebsiteURL + ' target="_blank" style="padding-right: 20px;" class="clear-content"> Go to webpage </a></h6>';

	UnprocessedSiteHtml += '</div>';
	UnprocessedSiteHtml += '</a>';
	UnprocessedSiteHtml += '</div>';
	return UnprocessedSiteHtml;
}

function formatLength(str) {
	if (str != null && str.length > 70) {
		return '<span href"#" class="hoverTip" data-toggle="tooltip" data-placement="right" data-original-title="' + str + '">' + str.substring(0, 70) + "..." + "</span>";
	} else {
		return str;
	}
}

function GenerateTableRow(data, ProcessId) {
	var str = "";
	str = "<tr>";
	str += "<td style='text-align:center;font-weight: bold;'>" + data.SrNo + "</td>";
	str += "<td style='word-break:break-all;'>" + data.WebSiteURL + "</td>";
	str += "<td style='text-align:center;'>" + data.Scan1 + "</td>";
	str += "<td style='text-align:center;'>" + data.Scan2 + "</td>";
	str += "<td style='text-align:center;'>" + data.ProprietoryTag + "</td>";
	str += "<td style='text-align:center;'>" + data.StandardTag + "</td>";
	if (data.Scan1 != "No") {
		str += "<td style='text-align:center;'><a style='cursor: pointer;color: #0275d8;' target='_blank' href=" + RedirectToDetailURL + "?webSiteHashId=" + data.WebSiteHashId + "&webSiteType=" + webSiteType + "&processEventId=" + ProcessId + ">Detail</a></td>";
	}
	else {
		str += "<td style='text-align:center;'><a style='cursor: pointer;color: #0275d8;' target='_blank' href=" + data.WebSiteURL + " >Go to webpage</a></td>";

	}
	str += "</tr>";

	return str;
}

function CreateBlockForStandardTags(StandardTags) {
	if (StandardTags.DateOfIssue == "") {
		StandardTags.DateOfIssue = "Not available in content";
	}
	if (StandardTags.CountryName == "") {
		StandardTags.CountryName = "Not available in content";
	}
	if (StandardTags.EntityName == "") {
		StandardTags.EntityName = "Not available in content";
	}
	if (StandardTags.EntityTypeName == "") {
		StandardTags.EntityTypeName = "Not available in content";
	}
	if (StandardTags.Individual == "") {
		StandardTags.Individual = "Not available in content";
	}
	if (StandardTags.Sectors == "") {
		StandardTags.Sectors = "Not available in content";
	}
	//if (StandardTags.SearchType == "") {
	//  StandardTags.SearchType = "Not available in content";
	//}
	var str = "";
	str = "<table class='table table-bordered table-hover'>";
	str += "<tbody>";
	str += "<tr><td style='font-weight: bold;'>Date</td><td>" + StandardTags.DateOfIssue + "</td></tr>";
	str += "<tr><td style='font-weight: bold;'>Country</td><td>" + StandardTags.CountryName + "</td></tr>";
	str += "<tr><td style='font-weight: bold;'>Entity Name</td><td>" + StandardTags.EntityName + "</td></tr>";
	str += "<tr><td style='font-weight: bold;'>Entity Type</td><td>" + StandardTags.EntityTypeName + "</td></tr>";
	str += "<tr><td style='font-weight: bold;'>Individual Name</td><td>" + StandardTags.Individual + "</td></tr>";
	str += "<tr><td style='font-weight: bold;'>Sector</td><td>" + StandardTags.Sectors + "</td></tr>";
	////str += "<tr><td style='font-weight: bold;'>Search Type</td><td>" + StandardTags.SearchType + "</td></tr>";
	str += "</tbody>";
	str += "</table>";
	return str;
}

function GenerateSummaryHeader() {
	var str = "<thead><tr><th colspan='8' style='text-align:center;font-weight: bold;border-left-width:0px;border-right-width:0px;'>Summary of execution</th><tr/></thead>"
	str += " <tbody>";
	str += "<tr class='card-header'>";
	str += '<td style="text-align:center;font-weight: bold;width:50px">Sr. No.</td>';
	str += '<td style="text-align:center;font-weight: bold;">Link(s)</td>';
	str += '<td style="text-align:center;font-weight: bold;width:120px">Content updated? (Scan 1)</td>';
	str += '<td style="text-align:center;font-weight: bold;width:120px">Lexicon terms found? (Scan 2)</td>';
	str += '<td style="text-align:center;font-weight: bold;width:120px">Proprietary Tags Identified</td>';
	str += '<td style="text-align:center;font-weight: bold;width:120px">Standard Tags Identified</td>';
	str += '<td style="text-align:center;font-weight: bold;width:50px">Details</td>';
	str += "</tr>";
	return str;

}

function GenerateInnerTable(proprietoryList) {

	innerTable_tableRow = '';
	innerTable_tableBody = "";
	innerTable_tableBody = "<div class='table-responsive'><table class='table table-bordered table-hover'>";
	innerTable_tableBody += "<thead>";
	innerTable_tableBody += "<tr class='card-header'>";
	innerTable_tableBody += '<th style="text-align:center;font-weight: bold;">Sr. No.</th>';
	innerTable_tableBody += '<th style="text-align:center;font-weight: bold;">Proprietary Tag</th>';
	innerTable_tableBody += '<th style="text-align:center;font-weight: bold;">ActivityType / Phrases / Noun + Verb search</th>';
	innerTable_tableBody += '<th style="text-align:center;font-weight: bold;">Value</th>';
	//innerTable_tableBody += '<th style="text-align:center;font-weight: bold;">Count</th>';
	innerTable_tableBody += '<th style="text-align:center;font-weight: bold;">Search Method</th>';
	innerTable_tableBody += "</tr>";
	innerTable_tableBody += "</thead>";
	innerTable_tableBody += "<tbody>";
	innerTable_tableRowCount = 1;
	$.each(proprietoryList, function (index, value) {
		innerTable_tableRow += GenerateInnerTableRow(value)
		innerTable_tableRowCount++
	})
	innerTable_tableBody += innerTable_tableRow;
	innerTable_tableBody += "</tbody>";
	innerTable_tableBody += "</table></div>";
	return innerTable_tableBody;
}

function GenerateInnerTableRow(data) {

	var str = "";
	str = "<tr>";
	str += "<td style='text-align:center;font-weight: bold;'>" + innerTable_tableRowCount + "</td>";
	str += "<td style='text-align:center;'>" + data.MetadataTypeName + "</td>";
	str += "<td style='text-align:center;'>" + data.SearchType + "</td>";
	str += "<td style='text-align:center;'>" + data.SearchValue + "</td>";
	//str += "<td style='text-align:center;'>" + data.SearchCount + "</td>";

	str += "<td style='text-align:center;'><span class='badge badge-pill badge-primary'>" + data.SearchByType + "</span></td>";

	str += "</tr>";
	return str;
}
function SetLexiconTags(value) {
	var propContents = '';
	if (value.LexiconList.length > 0) {
		propContents += "<p><b><u>Proprietary Tags :</u></b></p><div>" + GenerateInnerTable(value.ProprietoryTagList) + "</div>";
	}
	return propContents;
}
function GenerateLexiconTable(lexiconList) {

	var lexicon_tableRow = '';
	var lexicon_tableBody = "";
	lexicon_tableBody = "<div class='table-responsive'><table class='table table-bordered table-hover'>";
	lexicon_tableBody += "<thead>";
	lexicon_tableBody += "<tr class='card-header'>";
	lexicon_tableBody += '<th style="text-align:center;font-weight: bold;">Sr. No.</th>';
	lexicon_tableBody += '<th style="text-align:center;font-weight: bold;">Lexicon Terms</th>';
	lexicon_tableBody += '<th style="text-align:center;font-weight: bold;">Count</th>';
	lexicon_tableBody += "</tr>";
	lexicon_tableBody += "</thead>";
	lexicon_tableBody += "<tbody>";
	var rowId = 1;

	$.each(lexiconList.LexiconTagList, function (index, value) {
		lexicon_tableRow += GenerateLexiconInnerTableRow(value, rowId)
		rowId++;
	})
	lexicon_tableBody += lexicon_tableRow;
	lexicon_tableBody += "</tbody>";
	lexicon_tableBody += "</table></div>";
	return lexicon_tableBody;
}

function GenerateLexiconInnerTableRow(data, rowId) {

	var str = "";
	str = "<tr>";
	str += "<td style='text-align:center;font-weight: bold;'>" + rowId + "</td>";
	str += "<td style='text-align:center;'>" + data.LexiconTerm + "</td>";
	str += "<td style='text-align:center;'>" + data.SearchCount + "</td>";
	str += "</tr>";
	return str;
}

function SetProprietoryTags(parentURL, value) {
	var propContents = '';
	if (value.ProprietoryTagList.length > 0) {
		propContents += "<p><b><u>Proprietary Tags :</u></b></p><div>" + GenerateInnerTable(value.ProprietoryTagList) + "</div>";
		if (value.StandardTagData.CountryName != null && value.StandardTagData.SearchType != null) {
			propContents += "<p><b><u>Standard Tags :</u></b></p><div>" + CreateBlockForStandardTags(value.StandardTagData) + "</div>";
			if (value.DocumentURL != null) {
				propContents += "<p><b><u>Document :</u></b></p>" + CreateDocumentPanel(value.DocumentURL) + "";
			}
		}
	}
	return propContents;
}
function CreateDocumentPanel(documentURL) {
	var panel = "";
	panel += '<div >';
	panel += ' <a href= ' + documentURL + ' target="_blank" style="padding-right: 20px;"> <span class="fa fa-file-pdf" style="font-size:30px; color:#f56161;"></span> </a>';

	panel += '</div>';
	return panel;
}

var getParams = function (url) {
	var params = {};
	var parser = document.createElement('a');
	parser.href = url;
	var query = parser.search.substring(1);
	var vars = query.split('&');
	for (var i = 0; i < vars.length; i++) {
		var pair = vars[i].split('=');
		params[pair[0]] = decodeURIComponent(pair[1]);
	}
	return params;
};