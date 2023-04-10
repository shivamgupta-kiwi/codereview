//using BCMStrategy.API.Models;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BCMStrategy.API.Controllers
{
  [RoutePrefix("api/Test")]
  public class TestApiController : ApiController
  {
    [HttpGet]
    [Route("GetDummydata")]
    public async Task<IHttpActionResult> GetDummydata()
    {
      List<ReportViewModel> models = GetDummyData();
      return Json(models);
    }

    public List<ReportViewModel> GetDummyData()
    {
      List<ReportViewModel> models = new List<ReportViewModel>();
      models.Add(new ReportViewModel()
      {
        Lexicon = "Canada",
        LexiconHashId = "123",
        ActionType = new List<ActionType>() { 
             new ActionType()
             {
               Name = "Judicial",
               ActionTypeHashId = "GWSYWX7FAQ12",
               Value = 11
             },
             new ActionType()
             {
               Name = "Action",
               ActionTypeHashId = "GWSYWX7FAQ143",
               Value = 21
             },
             new ActionType()
             {
               Name = "Data",
               ActionTypeHashId = "GWSYWX7FGQ12",
               Value = 31
             },
             new ActionType()
             {
               Name = "Leaks",
               ActionTypeHashId = "GWSYWX7FAAQ12",
               Value = 56
             },
             new ActionType()
             {
               Name = "Rhetoric",
               ActionTypeHashId = "GWSYWX7WAQ12",
               Value = 11
             }
        }
      });

      models.Add(new ReportViewModel()
      {
        Lexicon = "China",
        LexiconHashId = "456",
        
        ActionType = new List<ActionType>() { 
             new ActionType()
             {
               Name = "Judicial",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 21
             },
             new ActionType()
             {
               Name = "Action",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 23
             },
             new ActionType()
             {
               Name = "Data",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 3
             },
             new ActionType()
             {
               Name = "Leaks",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 40
             },
             new ActionType()
             {
               Name = "Rhetoric",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 12
             }
        }
      });
      models.Add(new ReportViewModel()
      {
        Lexicon = "India",
        LexiconHashId = "879",
        ActionType = new List<ActionType>() { 
             new ActionType()
             {
               Name = "Judicial",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 12
             },
             new ActionType()
             {
               Name = "Action",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 21
             },
             new ActionType()
             {
               Name = "Data",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 32
             },
             new ActionType()
             {
               Name = "Leaks",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 33
             },
             new ActionType()
             {
               Name = "Rhetoric",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 11
             }
        }
      });
      models.Add(new ReportViewModel()
      {
        Lexicon = "Japan",
        LexiconHashId = "777",
        ActionType = new List<ActionType>() { 
             new ActionType()
             {
               Name = "Judicial",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 33
             },
             new ActionType()
             {
               Name = "Action",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 11
             },
             new ActionType()
             {
               Name = "Data",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 45
             },
             new ActionType()
             {
               Name = "Leaks",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 23
             },
             new ActionType()
             {
               Name = "Rhetoric",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 15
             }
        }
      });
      models.Add(new ReportViewModel()
      {
        Lexicon = "Germany",
        LexiconHashId = "999",
        ActionType = new List<ActionType>() { 
             new ActionType()
             {
               Name = "Judicial",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 2
             },
             new ActionType()
             {
               Name = "Action",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 32
             },
             new ActionType()
             {
               Name = "Data",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 12
             },
             new ActionType()
             {
               Name = "Leaks",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 45
             },
             new ActionType()
             {
               Name = "Rhetoric",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 23
             }
        }
      });
      models.Add(new ReportViewModel()
      {
        Lexicon = "",
        ActionType = new List<ActionType>() { 
             new ActionType()
             {
               Name = "Judicial",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 7
             },
             new ActionType()
             {
               Name = "Action",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 32
             },
             new ActionType()
             {
               Name = "Data",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 19
             },
             new ActionType()
             {
               Name = "Leaks",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 9
             },
             new ActionType()
             {
               Name = "Rhetoric",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 14
             }
        }
      });
      models.Add(new ReportViewModel()
      {
        Lexicon = "",
        ActionType = new List<ActionType>() { 
             new ActionType()
             {
               Name = "Judicial",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 41
             },
             new ActionType()
             {
               Name = "Action",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 22
             },
             new ActionType()
             {
               Name = "Data",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 11
             },
             new ActionType()
             {
               Name = "Leaks",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 5
             },
             new ActionType()
             {
               Name = "Rhetoric",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 19
             }
        }
      });
      models.Add(new ReportViewModel()
      {
        Lexicon = "",
        ActionType = new List<ActionType>() { 
             new ActionType()
             {
               Name = "Judicial",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 10
             },
             new ActionType()
             {
               Name = "Action",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 20
             },
             new ActionType()
             {
               Name = "Data",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 30
             },
             new ActionType()
             {
               Name = "Leaks",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 40
             },
             new ActionType()
             {
               Name = "Rhetoric",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 50
             }
        }
      });
      models.Add(new ReportViewModel()
      {
        Lexicon = "",
        ActionType = new List<ActionType>() { 
             new ActionType()
             {
               Name = "Judicial",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 11
             },
             new ActionType()
             {
               Name = "Action",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 12
             },
             new ActionType()
             {
               Name = "Data",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 33
             },
             new ActionType()
             {
               Name = "Leaks",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 51
             },
             new ActionType()
             {
               Name = "Rhetoric",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 23
             }
        }
      });
      models.Add(new ReportViewModel()
      {
        Lexicon = "",
        ActionType = new List<ActionType>() { 
             new ActionType()
             {
               Name = "Judicial",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 11
             },
             new ActionType()
             {
               Name = "Action",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 23
             },
             new ActionType()
             {
               Name = "Data",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 43
             },
             new ActionType()
             {
               Name = "Leaks",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 12
             },
             new ActionType()
             {
               Name = "Rhetoric",
               ActionTypeHashId = "GWSYWX7FEQ12",
               Value = 34
             }
        }
      });


      //models.Where(a=> !a.IsEuropeanUnion && string.IsNullOrEmpty(a.Lexicon)).ToList().ForEach(a =>
      //{
      //  a.Lexicon = "NA";
      //});
      //models.Where(a => a.IsEuropeanUnion).ToList().ForEach(a =>
      //{
      //  a.Lexicon = "European Union";
      //});

      return models;
    }

    [HttpGet]
    [Route("GetActivityTypeList")]
    public async Task<List<ActivityType>> GetActivityTypeList(int? officialSectorId, int? mediaSectorId, bool isEropionUnion, string countryHashId, string actionTypeHashId)
    {
      List<ActivityType> model = new List<ActivityType>();
      model.Add(new ActivityType()
      {
        Name = "Speech",
        Value = 12
      });
      model.Add(new ActivityType()
      {
        Name = "Testiomony",
        Value = 41
      });
      model.Add(new ActivityType()
      {
        Name = "Law",
        Value = 43
      });
      model.Add(new ActivityType()
      {
        Name = "Treaty",
        Value = 16
      });
      model.Add(new ActivityType()
      {
        Name = "Agreement",
        Value = 75
      });
      model.Add(new ActivityType()
      {
        Name = "Vote",
        Value = 19
      });
      model.Add(new ActivityType()
      {
        Name = "Communique",
        Value = 62
      });
      model.Add(new ActivityType()
      {
        Name = "Proposal",
        Value = 41
      });
      model.Add(new ActivityType()
      {
        Name = "Working Paper",
        Value = 62
      });
      model.Add(new ActivityType()
      {
        Name = "Decision Note",
        Value = 52
      });
      return model;
    }
  }

  public class ActivityType
  {
    public string Name { get; set; }
    public int Value { get; set; }
  }

  public class ActionType
  {
    public string Name { get; set; }
    public int Value { get; set; }
    public string ActionTypeHashId { get; set; }
    public List<ActivityType> ActivityType { get; set; }
  }

  public class ReportViewModel
  {
    public string Lexicon { get; set; }
    public string LexiconHashId { get; set; }
    public List<ActionType> ActionType { get; set; }
  }
}