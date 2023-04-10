var viewModelLexicon;

var loadKendoGrid = function (userHashId) {
  $(document).ready(function () {
    var element = $("#gridDefaultLexicon").kendoGrid({
      dataSource: {
        type: "json",
        transport: {
          read: {
            url: BCMConfig.API_GETALLLIST_LEXICONTYPE_URL,
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
              LexiconTypeMasterId: {
                type: "string"
              },
              LexiconType: {
                type: "string"
              },
            }
          }
        },
        pageSize: 10,
        serverPaging: false,
        serverFiltering: false,
        serverSorting: false
      },
      filterable: true,
      sortable: true,
      height: 450,
      detailInit: detailInit,
      dataBound: masterGridDataBoundForParentGrid,
      
      columns: [
      {
        field: "LexiconType",
        title: LexiconType,
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
  });

  function detailInit(e) {
    $("<div/>").appendTo(e.detailCell).kendoGrid({
      dataSource: {
        type: "json",
        transport: {
          read: {
            url: BCMConfig.API_GET_DEFAULT_ALLLIST_LEXICON_URL + "?userMasterHashId=" + userHashId,
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
              LexiconeIssueMasterHashId: { type: "string" },
              LexiconIssue: { type: "string" },
              CombinationValue: { type: "string" },
              IsNestedStr: { type: "string" },
              Linkers: { type: "string" },
            }
          }
        },
        serverPaging: true,
        serverSorting: true,
        serverFiltering: true,
        pageSize: 10,
        filter: {
          field: "LexiconeTypeMasterId", operator: "eq", value: e.data.LexiconTypeMasterId
        }
      },
      filterable: true,
      height: 350,
      sortable: true,
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
      dataBound: masterGridDataBound,
      columns: [
          {
            field: "LexiconIssue",
            title: LexiconTerm, width: "150px",
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
             field: "CombinationValue",
             title: CombinationValue,
             width: "240px",
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
              field: "Linkers",
              title: LexiconLinker,
              width: "240px",
              sortable: false,
              filterable: false
            }
      ]
    });
    CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
  }

};

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

function masterGridDataBoundForParentGrid(e) {
  var grid = e.sender;
  if (grid.dataSource.total() == 0) {
    var colCount = grid.columns.length;
    $(e.sender.wrapper)
        .find('tbody').first()
        .append('<tr class="kendo-data-row" style="text-align: center;"><td colspan="2" class="no-data">' + noDataFound + '</td></tr>');
  }
  CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}