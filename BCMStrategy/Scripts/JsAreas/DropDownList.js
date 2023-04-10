var GetInstituionTypeDDList = function (controlId) {
  $.ajax({
    url: BCMConfig.API_INSTITUTIONTYPE_DROPDOWNLIST,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response) {
        $.each(response.data, function (key, value) {
          if (controlId != "" && controlId != null && controlId != undefined) {
            $("#" + controlId).append($("<option></option>").val(value.keyHash).html(value.value));
          }
          else {
            $("#institutionTypeHashId").append($("<option></option>").val(value.keyHash).html(value.value));
          }
        });
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    }
  });
}

var GetCountryDDList = function (controlId) {
  $.ajax({
    url: BCMConfig.API_COUNTRY_DROPDOWNLIST,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response) {
        $.each(response.data, function (key, value) {
          if (controlId != "" && controlId != null && controlId != undefined) {
            $("#" + controlId).append($("<option></option>").val(value.keyHash).html(value.value));
          }
          else {
            $("#countryMasterHashId").append($("<option></option>").val(value.keyHash).html(value.value));
          }
        });
      }
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
    }
  });
}

var GetDesignationDDList = function (controlId) {
  $.ajax({
    url: BCMConfig.API_GETDESIGNATION_LIST,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response) {
        $.each(response.data, function (key, value) {
          if (controlId != "" && controlId != null && controlId != undefined) {
            $("#" + controlId).append($("<option></option>").val(value.keyHash).html(value.value));
          }
          else {
            $("#designationHashId").append($("<option></option>").val(value.keyHash).html(value.value));
          }
        });
        CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
      }
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    }
  });
}

var GetLexiconTypeDDList = function (controlId) {
    $.ajax({
        url: BCMConfig.API_LEXICONTYPE_DROPDOWNLIST,
        beforeSend: CommonJS.BeforeSendAjaxCall,
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        method: 'GET',
        processdata: true,
        success: function (response) {
            if (response) {
                $.each(response.data, function (key, value) {
                    if (controlId != "" && controlId != null && controlId != undefined) {
                        $("#" + controlId).append($("<option></option>").val(value.keyHash).html(value.value));
                    }
                    else {
                        $("#lexiconeTypeMasterHashId").append($("<option></option>").val(value.keyHash).html(value.value));
                    }
                });
            }
            CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        },
        error: function (e) {
            CommonJS.HandleErrorStatus(e.status);
            CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
        }
    });
}

var GetWebsiteTypeDDList = function (controlId) {
  $.ajax({
    url: BCMConfig.API_WEBSITETYPE_DROPDOWNLIST,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response) {
        $.each(response.data, function (key, value) {
          if (controlId != "" && controlId != null && controlId != undefined) {
            $("#" + controlId).append($("<option></option>").val(value.keyHash).html(value.value));
          }
          else {
            $("#websiteTypeHashId").append($("<option></option>").val(value.keyHash).html(value.value));
          }
        });
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    }
  });
}

var GetMetadataTypeDDList = function (controlId) {
  $.ajax({
    url: BCMConfig.API_METADATATYPES_DROPDOWNLIST,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response) {
        $.each(response.data, function (key, value) {
          if (controlId != "" && controlId != null && controlId != undefined) {
            $("#" + controlId).append($("<option></option>").val(value.keyHash).html(value.value));
          }
          else {
            $("#metadataTypeMasterHashId").append($("<option></option>").val(value.keyHash).html(value.value));
          }
        });
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    }
  });
}

var GetMetadataDynamicNounplusVerbDDList = function (controlId) {
  $.ajax({
    url: BCMConfig.API_METADATADYNAMICNOUNPLUSVERB_DROPDOWNLIST,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response) {
        $.each(response.data, function (key, value) {
          if (controlId != "" && controlId != null && controlId != undefined) {
            $("#" + controlId).append($("<option></option>").val(value.value).html(value.value));
          }
          else {
            $("#metadataDynamicNounplusVerb").append($("<option></option>").val(value.value).html(value.value));
          }
        });
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    }
  });
}

var GetUserDDList = function (controlId) {
  $.ajax({
    url: BCMConfig.API_USER_DROPDOWNLIST,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response) {
        $.each(response.data, function (key, value) {
          if (controlId != "" && controlId != null && controlId != undefined) {
            $("#" + controlId).append($("<option></option>").val(value.keyHash).html(value.value));
          }
          else {
            $("#userMasterHashId").append($("<option></option>").val(value.keyHash).html(value.value));
          }
        });
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    }
  });
}

var GetSchedulerFrequencyDDList = function (controlId) {
  $.ajax({
    url: BCMConfig.API_SCHEDULERFREQUENCY_DROPDOWNLIST,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response) {
        $.each(response.data, function (key, value) {
          if (controlId != "" && controlId != null && controlId != undefined) {
            $("#" + controlId).append($("<option></option>").val(value.keyHash).html(value.value));
          }
          else {
            $("#frequencyTypeMasterHashId").append($("<option></option>").val(value.keyHash).html(value.value));
          }
        });
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    }
  });
}

var GetSchedulerWebsiteTypeDDList = function (controlId) {
  $.ajax({
    url: BCMConfig.API_WEBSITETYPE_DROPDOWNLIST,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response) {
        $.each(response.data, function (key, value) {
          if (controlId != "" && controlId != null && controlId != undefined) {
            $("#" + controlId).append($("<option></option>").val(value.value).html(value.value));
          }
          else {
            $("#websiteType").append($("<option></option>").val(value.value).html(value.value));
          }
        });
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    }
  });
}

var GetActivityTypeDDList = function (controlId, metadataTypeMasterHashId) {
  $.ajax({
    url: BCMConfig.API_ACTIVITYTYPE_DROPDOWNLIST + "?actionTypeMasterHashId=" + metadataTypeMasterHashId,
    beforeSend: CommonJS.BeforeSendAjaxCall,
    dataType: "json",
    async:false,
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    processdata: true,
    success: function (response) {
      if (response) {
        $.each(response.data, function (key, value) {
          if (controlId != "" && controlId != null && controlId != undefined) {
            $("#" + controlId).append($("<option></option>").val(value.keyHash).html(value.value));
          }
          else {
            $("#activityTypeMasterHashId").append($("<option></option>").val(value.keyHash).html(value.value));
          }
        });
      }
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    },
    error: function (e) {
      CommonJS.HandleErrorStatus(e.status);
      CommonJS.SetPageLoader(PageLoaderActivity.HIDE);
    }
  });
}