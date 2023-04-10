var viewModelLegislator;
var selectedPersonDict = {};

$(document).ready(function ()    {
    GetCountryDDList("countryMasterHashId");
    loadKendoGrid();
    GetLegislatorManagementDDL();
});


var GetLegislatorManagementDDL = function () {
    $("#divPageLoader").show();
    $.ajax({
        url: BCMConfig.API_GETLEGISLATOR_MANAGEMENT_DATA,
        beforeSend: CommonJS.BeforeSendAjaxCall,
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        method: 'GET',
        processdata: true,
        success: function (response) {
            if (response) {
                BindSector(response.sectorDDL);
                BindEntity(response.entityDDL);
                BindDesignation(response.designationDDL);
                BindCommitee(response.commiteeDDL);
                $("#divPageLoader").hide();
            }
        },
        error: function (e) {
            CommonJS.HandleErrorStatus(e.status);
        }
    });
}

function BindDesignation(DesignationDDL) {
    $.each(DesignationDDL, function (key, value) {
        if (value.data) {
            $("#designation").append($("<option  selected='selected'></option>").val(value.keyHash).html(value.value));
        }
        else {
            $("#designation").append($("<option></option>").val(value.keyHash).html(value.value));
        }

    });
    InitializedDesignationMultiSelect();
}

function InitializedDesignationMultiSelect() {
    $('#designation').fastselect({
        placeholder: 'Select Designation',
        onItemSelect: function () {
            CloseMultiSelectOptionPopUp();
        }
    });
}

function BindCommitee(commiteeDDL) {
    $.each(commiteeDDL, function (key, value) {
        if (value.data) {
            $("#commitee").append($("<option  selected='selected'></option>").val(value.keyHash).html(value.value));
        }
        else {
            $("#commitee").append($("<option></option>").val(value.keyHash).html(value.value));
        }

    });
    InitializedCommiteeMultiSelect();
}

function InitializedCommiteeMultiSelect() {
     $('#commitee').fastselect({
         placeholder: 'Select Committee',
         onItemSelect: function () {
             CloseMultiSelectOptionPopUp();
         }
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
}

function InitializedSectorMultiSelect() {
    $('#sector').fastselect({
        placeholder: 'Select Sector',
    });
}

function BindEntity(entityDDL) {
    $.each(entityDDL, function (key, value) {
        if (value.data) {
            $("#entity").append($("<option selected='selected'></option>").val(value.keyHash).html(value.value));
        }
        else {
            $("#entity").append($("<option></option>").val(value.keyHash).html(value.value));
        }
    });
}

function InitializedEntityMultiSelect() {
    $('#entity').fastselect({
        placeholder: 'Select Entity',
    });
}

$('#btnCancel').click(function () {
    $('#collapseExample').collapse('hide');
});

$('#btnLegislatorCancel').click(function (e) {
    ResetFormValues();
    $('#collapseExample').collapse('hide');
});

var loadKendoGrid = function () {
    $("#gridLegislator").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: BCMConfig.API_GETALLLIST_Legislators_URL,
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
                        LegislatorHashId: { type: "string" },
                        FirstName: { type: "string" },
                        LastName: { type: "string" },
                        Country: { type: "string" },
                        Entity: { type: "string" },
                        Sector: { type: "string" },
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
            },
            {
                field: "Entity",
                title: "Entity",
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
                field: "Sector",
                title: "Sector",
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
                field: "Country",
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
                title: "",
                template: "<a href='javascript:' data-toggle='tooltip' title='Edit' onclick='EditLegislator(\"#=LegislatorHashId #\")'><i class='fa fa-pencil-alt' aria-hidden='true'></i></a>",
                width: 50
            },
            {
                title: "",
                template: "<a href='javascript:' data-toggle='tooltip' title='Delete' onclick='DeleteLegislator(\"#=LegislatorHashId #\")'><i class='fa fa-trash-alt' aria-hidden='true'></i></a>",
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

function IsInDictionary(key, dictionary) {
    if (dictionary[key])
        return true;
}

$("#clearFilters").click(function () {
    $("#gridLegislator").data("kendoGrid").dataSource.filter([]);
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

function DeleteLegislator(LegislatorHashId) {
    bcmStrategyAlerts({
        type: "confirm",
        messageText: "You won't be able to undo this action. Are you sure to delete this Legislator?",
        headerText: deletePopupHeader,
        alertType: alertTypes.Info
    }).done(function (e) {
        if (e) {
            CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
            ////var row = $(itemObject).closest("tr");
            ////var uid = $(row).data("uid");
            ////var grid = $("#gridLegislator").data("kendoGrid");
            ////var currentDataRow = grid.dataItem(row);

            var legislatorHashId = LegislatorHashId; ////currentDataRow.LegislatorHashId

            $.ajax({
                url: BCMConfig.API_LEGISLATOR_DELETE + "?legislatorHashId=" + legislatorHashId,
                beforeSend: CommonJS.BeforeSendAjaxCall,
                method: 'GET',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                processdata: true,
                success: function (response) { //call successfull
                    if (response.data) {
                        toastr.info(response.errorMessage);
                        //ResetForm();
                        loadKendoGrid();
                    }
                    else {
                        toastr.info("legislator can not be deleted as it is associated with other entities");
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

function EditLegislator(LegislatorHashId) {
    CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
    ResetForm();
    ////var row = $(itemObject).closest("tr");
    ////var uid = $(row).data("uid");
    ////var grid = $("#gridLegislator").data("kendoGrid");
    ////var currentDataRow = grid.dataItem(row);

    $('#legislatorHashHiddenVal').val(LegislatorHashId);
    GetLegislatorBasedOnHash(LegislatorHashId);

    $('#collapseExample').collapse('show');
    $(".form-group").removeClass('has-error');
    moveDivTop("topPanel");
}

function GetLegislatorBasedOnHash(legislatorHashId) {
    $.ajax({
        url: BCMConfig.API_GETLEGISLATOR_BASED_ON_HASH + "?legislatorHashId=" + legislatorHashId,
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

function BindDataOnEdit(response) {
    ResetvalidateForm();
    AppendHTMLOnEdit();
    $('[data-role="remove"]').click();
    $('#entity').val(response.entityHashId);
    $('#sector').val(response.sectorHashId);
    $('#firstName').val(response.firstName);
    $('#lastName').val(response.lastName);
    $('#countryMasterHashId').val(response.countryHashId);
    BindDesignation(response.designationDDL);
    BindCommitee(response.commiteeDDL);
    CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
}

function HidePanelOnPageTypeChange() {
    $('.dropDownSearch').hide();
    $('.hyperLinkClicked').hide();
    $('.searchFunctionality').hide();
    $('.RSSFeeds').hide();
}

function BindData() {
    var viewModelLegislator = {
        'LegislatorHashId':$('#legislatorHashHiddenVal').val(),
        'FirstName': $('#firstName').val(),
        'LastName': $('#lastName').val(),
        'SectorHashId' : $('#sector').val(),
        'EntityHashId':$('#entity').val(),
        'DesignationHashIds': $('#designation').val(),
        'CommiteeHashIds': $('#commitee').val(),
        'CountryHashId': $('#countryMasterHashId').val()
    };

    return viewModelLegislator;
}

function ResetForm() {
    $('#legislatorForm').trigger("reset");
    //InitializedActivityTypeMultiSelect();
}

$("#btnLegislatorSave").click(function () {
    if ($("#legislatorForm").valid()) {
        CommonJS.SetPageLoader(PageLoaderActivity.SHOW);
        ResetvalidateForm();
        var LegislatorModel = BindData();
        $.ajax({
            url: BCMConfig.API_POST_LEGISLATOR_URL,
            beforeSend: CommonJS.BeforeSendAjaxCall,
            method: 'POST',
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(LegislatorModel),
            processdata: true,
            success: function (response) {
                if (response.errorModel != null && response.errorModel != "") {
                    AppyCustomValidation(response.errorModel);
                    ValidateForm('legislatorForm', response.errorModel);
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

function RemoveErrorClass() {
    $('label[id*="error"]').addClass('error').removeClass('red-text');
}

function AppyCustomValidation(errorModel)
{
    if (errorModel['designationHashIds']) {
        $('.DesignationDiv').append('<label id="designation-error" class="red-text" style="padding-top:  5px;" for="designation">Designation is required</label>');
        delete errorModel['designationHashIds'];
    }
    else
        $('.DesignationDiv').find('.red-text').remove();
}


function CloseMultiSelectOptionPopUp() {
    $('.fstActive').removeClass('fstActive');
}

function RemoveErrorClass() {
    $('.form-control').removeClass('error');
    $('label[id*="error"]').removeClass('error').addClass('red-text');
}

function ResetvalidateForm() {
    $('label[id*="error"]').remove();
    RemoveErrorClass();
}

function ResetFormValues() {
    $('#legislatorHashHiddenVal').val('');
    $('.fstChoiceRemove').click();
    $('.red-text').remove();
    $('#firstName,#lastName,#entity,#sector,#countryMasterHashId').val('');
}

function BindMetaDataProprietaryTagMultiSelect() {
    //var elementText = $($('#metaDataProprietaryTag').parent().find('.fstResults').children()[0]).text();
    //$('#metaDataProprietaryTag').children('option:selected').index();
    //$("#metaDataProprietaryTag > [value='DQJGnWkcHHfbxcgQhodVeg']").attr("selected", "true");
    //$($('.fstResults').children()[1]).click();
    //var bindedOptions = $('#metaDataProprietaryTag').children('option');
    //var valueToSelect = 'EjCn-S3aA9scphtowkLOfg';
    //var fstElement = $('.fstElement');
    //fstElement.click();
    //fstElement.addClass('fstActive fstResultsOpened');
    //fstElement.removeClass('fstActive fstResultsOpened');
    //setTimeout(function () {
    //  bindedOptions.each(function (index, value) {
    //    var optionVal = $(this).val();
    //    if (optionVal == valueToSelect) {
    //      $($('#metaDataProprietaryTag').parent().find('.fstResults').children()[index]).click();
    //    }
    //  });
    //}, 3000);
    //bindedOptions.each(function (index, value) {
    //  //var valueToSelect = 'NXST7EeFub1iPVUfbeNirQ';
    //  //$("#metaDataProprietaryTag > [value='"+valueToSelect+"']").attr("selected", "true");
    //  var optionVal = $(this).val();
    //  if (optionVal == valueToSelect)
    //  {
    //    $($('#metaDataProprietaryTag').parent().find('.fstResults').children()[index]).click();
    //  }
    //    //$($('#metaDataProprietaryTag').parent().find('.fstResults').children()[index]).click();
    //});

    //$('#metaDataProprietaryTag').children('option');
}

function AppendHTMLOnEdit() {
    $('.fstElement').remove();
    $('.DesignationDiv').append('<select class="form-control multipleSelect" multiple id="designation" name="designation"></select>');
    $('.CommiteeDiv').append('<select class="form-control multipleSelect" multiple id="commitee" name="commitee"></select>');
}