$(document).ready(function () {
  loadKendoGrid();
});

var loadKendoGrid = function () {
  $("#gridPrivilege").kendoGrid({
    dataSource: {
      type: "json",
      transport: {
        read: {
          url: BCMConfig.GET_LEXICON_ACCESS_CUSTOMER_LIST,
          beforeSend: CommonJS.BeforeSendAjaxCall

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
            CustomerMasterHashId: { type: "string" },
            CustomerMasterId: { type: "string" },
            CustomerFirstName: { type: "string" },
            CustomerMiddleName: { type: "string" },
            CustomerLastName: { type: "string" },
            CompanyName: { type: "string" },
            Designation: { type: "string" }
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
    height: 500,
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
            field: "Designation",
            title: designation,
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
          field: "CustomerFirstName",
          title: firstName,
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
          field: "CustomerMiddleName",
          title: middleName,
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
          field: "CustomerLastName",
          title: lastName,
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
          field: "CompanyName",
          title: companyName,
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
          field: "AccessLexiconTypesString",
          title: "Lexicons",
          width: 125,
          filterable: false,
          sortable: false,
        },
        {
          title: "",
          template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditCustomer(\"#=CustomerMasterHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
          width: 50
        },
    ]
  });
}

function EditCustomer(CustomerHashId) {
  //var row = $(itemObject).closest("tr");
  //var uid = $(row).data("uid");
  //var grid = $("#gridPrivilege").data("kendoGrid");
  //var currentDataRow = grid.dataItem(row);
  var customerMasterId = CustomerHashId;  ////currentDataRow.CustomerMasterHashId;
  window.location.href = $('#lexiconAccessManagementUrl')[0].href + "?CustomerHashId=" + customerMasterId;
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




