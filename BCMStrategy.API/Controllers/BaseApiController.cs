using BCMStrategy.API.Helpers;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Abstract = BCMStrategy.Data.Abstract;

namespace BCMStrategy.API.Controllers
{
  public class BaseApiController : ApiController
  {

    private ICommonRepository _commonRepository;

    private ICommonRepository CommonRepository
    {
      get
      {
        if (_commonRepository == null)
        {
          _commonRepository = new CommonRepository();
        }

        return _commonRepository;
      }
    }

    /// <summary>
    /// To run asyc task of the controllers
    /// </summary>
    /// <param name="controllerContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
    {
      ////string language = null;
      ////string langHeader = controllerContext.Request.Headers.AcceptLanguage.Select(x => x.Value).FirstOrDefault();

      ////if (!string.IsNullOrEmpty(langHeader))
      ////{
      ////  language = langHeader;
      ////}

      string host = (HttpContext.Current.Request.Url.IsDefaultPort) ?
          HttpContext.Current.Request.Url.Host :
          HttpContext.Current.Request.Url.Authority;

      host = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, host);

      if (HttpContext.Current.Request.ApplicationPath == "/")
        Abstract.CommonUtilities.ApplicationBasePath = host + "/";
      else
        Abstract.CommonUtilities.ApplicationBasePath = host + HttpContext.Current.Request.ApplicationPath + "/";

      return base.ExecuteAsync(controllerContext, cancellationToken);
    }

    #region Format API Output
    protected ApiOutput FormatResult(object data)
    {
      return new ApiOutput()
      {
        Data = data,
      };
    }

    protected ApiOutput FormatResult(object data, string errorMessage)
    {
      return new ApiOutput()
      {
        Data = data,
        ErrorMessage = errorMessage
      };
    }

    protected ApiOutput FormatResult(object data, ModelStateDictionary ModelState)
    {
      return new ApiOutput()
      {
        Data = data,
        ErrorModel = CommonUtilities.HandleModelState(ModelState).ToDictionary(pair => pair.Key, pair => pair.Value),
      };
    }

    protected string ImportCSV(HttpRequest httpRequest, int validateColCount, out List<IList<string>> validateLines)
    {
      if (httpRequest.Files.Count > 0)
      {
        var postedFile = httpRequest.Files[0];
        var MaximumFileUploadCSV = CommonRepository.GetCSVFileSize();
        using (var reader = new StreamReader(postedFile.InputStream))
        {
          var data = CsvParser.ParseHeadAndTail(reader, ',', '"');
          var header = data.Item1;
          var lines = data.Item2;
          validateLines = lines.ToList();
          if (postedFile != null && postedFile.ContentLength > 0)
          {
            int MaxContentLength = 1024 * 1024 * Convert.ToInt32(MaximumFileUploadCSV.Result);
            IList<string> AllowedFileExtensions = new List<string>();
            AllowedFileExtensions.Add(".csv");

            var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
            var extension = ext.ToLower();
            if (!AllowedFileExtensions.Contains(extension))
            {
              return Resource.ValidateCSVFileType;
            }
            else if (postedFile.ContentLength > MaxContentLength)
            {
              return string.Format(Resource.ValidateFileSize, MaximumFileUploadCSV);
            }
            else
            {
              if (header == null || header.Count() != validateColCount || validateLines.Count == 0 || validateLines.Any(x => x.Count() != validateColCount))
              {
                return Resource.ValidateImportCSV;
              }
            }
          }
        }
      }
      else
      {
        validateLines = new  List<IList<string>>();
        return Resource.ValidateToSelectFile;
      }
      return string.Empty;
    }

    #endregion Format API Output
  }
}