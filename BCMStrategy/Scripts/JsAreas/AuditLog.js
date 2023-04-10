var viewModelAuditLog;

$(document).ready(function () {
  GetAuditTableList();
});

function GetAuditTableList() {
  CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
  $.ajax({
    url: BCMConfig.API_AUDITTABLE_DROPDOWNLIST,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    //contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response && response.data) {
        BindAuditTable(response.data)
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      }
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
    }
  });

}

function BindAuditTable(auditTableDDL) {
  $.each(auditTableDDL, function (key, value) {
    $("#auditLog").append($("<option></option>").val(value.keyHash).html(value.value));
  });
}

function LoadKendoGrid() {
  var grid = $("#grid").kendoGrid({
    dataSource: {
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_GETALLLIST_AUDITLOG_URL,
          beforeSend: CommonJS.BeforeSendAjaxCall,
        },
        parameterMap: function (options) {
          return "parametersJson=" + JSON.stringify(options) + "&auditPageHash=" + $('#auditLog').val()
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
            AuditTableMasterHashId: { type: "string" },
            AuditTable: { type: "string" },
            AuditType: { type: "string" },
            BeforeValue: { type: "string" },
            AfterValue: { type: "string" },
            AuditDescription: { type: "string" },
            IpAddress: { type: "string" },
            Created: { type: "string" },
            UserName: { type: "string" }
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
    height: 530,
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
         field: "AuditTable",
         title: auditTable,
         width: 130,
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
       field: "AuditType",
       title: auditType,
       width: 130,
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
       field: "BeforeValue",
       title: beforeValue,
       width: 140,
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
       field: "AfterValue",
       title: afterValue,
       width: 130,
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
       field: "AuditDescription",
       title: discription,
       width: 130,
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
       field: "IpAddress",
       title: ipAddress,
       width: 130,
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
       field: "Created",
       title: dateField,
       width: 90,
       filterable: {
         extra: false,
         operators: {
           string: {
             contains: "Contains"
           }
         },
       }
     },
     {
         field: "UserName",
         title: userName,
         width: 130,
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

$('#auditLog').change(function () {
  LoadKendoGrid();
  $('#grid').data('kendoGrid').dataSource.read();
  $('#grid').data('kendoGrid').refresh();
});
