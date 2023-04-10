var viewModelUserAccessRights;

$(document).ready(function () {
  GetUserDDList("userMasterHashId");
  //ResetForm();
  //loadKendoGrid();
  BindTreeView();
});

var BindTreeView = function () {

  var dataSource = new kendo.data.HierarchicalDataSource({
    dataSource: {
      type: "json",
      transport: {
        read: {
          url: BCMConfig.API_GETALLLIST_MENU_URL,
          beforeSend: CommonJS.BeforeSendAjaxCall
        }
      }
    }
  });




  $("#treeview").kendoTreeView({
    checkboxes: {
      checkChildren: true
    },
    check: onCheck,
    dataSource: [{

      id: 6, text: "parent 1", expanded: true, items: [
                { id: 7, text: "Child 1" },
                { id: 8, text: "Child 2" },
      ]
    },
    {
      id: 6, text: "parent 2", expanded: true, items: [
                { id: 7, text: "Child 1" },
                { id: 8, text: "Child 2" },
      ]
    }
    ]

  });

}

// function that gathers IDs of checked nodes
function checkedNodeIds(nodes, checkedNodes) {
  for (var i = 0; i < nodes.length; i++) {
    if (nodes[i].checked) {
      checkedNodes.push(nodes[i].id);
    }

    if (nodes[i].hasChildren) {
      checkedNodeIds(nodes[i].children.view(), checkedNodes);
    }
  }
}

// show checked node IDs on datasource change
function onCheck() {
  var checkedNodes = [],
      treeView = $("#treeview").data("kendoTreeView"),
      message;

  checkedNodeIds(treeView.dataSource.view(), checkedNodes);

  if (checkedNodes.length > 0) {
    message = "IDs of checked nodes: " + checkedNodes.join(",");
  } else {
    message = "No nodes checked.";
  }

  $("#result").html(message);
}

