
$(document).ready(function () {
  var params = getParams(window.location.href);
  if (params['webSiteType']) {
    //var value = $('#schedulerMasterHashId').val(params['SchedulerMasterHashId']);
    //alert(params['SchedulerMasterHashId']);
    loadKendoGrid(params['webSiteType']);
  }

});

var loadKendoGrid = function (SchedulerMasterHashId) {
  $("#gridProcessDetail").kendoGrid({
    //toolbar: ["excel"],
    //excel: {
    //  fileName: "data.xlsx",
    //  filterable: true
    //},
    dataSource: {
      //requestStart: CommonJS.SetPageLoader(PageLoaderActivity.SHOW),
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_GET_PROCESSDETAIL_BASED_ON_SCHEDULER + "?schedulerMasterHashId=" + SchedulerMasterHashId,
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
            ProcessEventId: { type: "string" },
            StartDateTime: { type: "string" },
            EndDateTime: { type: "string" },
          }
        }
      }
      ,
      //pageSize: 15,
      serverPaging: true,
      serverFiltering: true,
      serverSorting: true
    },
    filterable: true,
    sortable: true,
    height: 450,
    //pageable: {
    //  messages: {
    //    itemsPerPage: "Items",
    //    display: "{0} - {1} of {2}",
    //  }
    //  ,
    //  pageSize: 20,
    //  pageSizes: [10, 20, 50],
    //  refresh: true,
    //  buttonCount: 5,
    //  input: true
    //},
    dataBound: masterGridDataBound,
    columns: [
        {
          field: "ProcessEventId",
          title: "Process Id",
          width: 50,
          filterable: false,
          sortable: false
        },
        {
          field: "StartDateTimeString",
          title: "Start DateTime",
          width: 80,
          filterable: false,
          sortable: false
        },
        {
          field: "EndDateTimeString",
          title: "End DateTime",
          width: 80,
          filterable: false,
          sortable: false
        }
        ,
        {
          field: "second",
          title: "Time Taken(minute)",
          width: 70,
          filterable: false,
          sortable: false
        }
        ,
        {
          field: "status",
          title: "Status",
          template: "#=SetColour(status)#",
          width: 50,
          filterable: false,
          sortable: false
        },
            {
              title: "View Detail",
              template: "#=CreateButton(status)#",
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

function GoToProcessDetail(itemObject) {
  var row = $(itemObject).closest("tr");
  var uid = $(row).data("uid");
  var grid = $("#gridProcessDetail").data("kendoGrid");
  var currentDataRow = grid.dataItem(row);
  var processId = currentDataRow.ProcessHashId;
  if (currentDataRow.ScraperName == "OfficialSector") {
    window.open($('#OfficialSectorUrl')[0].href + "?ProcessId=" + processId, '_blank');
  }
  if (currentDataRow.ScraperName == "MediaSector") {
    window.open($('#MediaSectorUrl')[0].href + "?ProcessId=" + processId, '_blank');
  }
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

function SetColour(status) {
  var fontColour = status == "Completed" ? "green" : status == "Running" ? "orange" : "red";
  return '<span style="color:' + fontColour + ';font-weight:bold">' + status + '</span>';
}
function CreateButton(status) {
  //if (status == "Completed") {
    return "<a href='javascript:' data-toggle='tooltip' title='View Detail' onclick='GoToProcessDetail(this)'><i class='fa fa-eye' aria-hidden='true'></i></a>";
  //}
  //else {
  //  return "<i class='fa fa-eye' aria-hidden='true'</i>";
  //}
}

$("#btnSchedulerBack").click(function () {
  window.location.href = backURL;
})