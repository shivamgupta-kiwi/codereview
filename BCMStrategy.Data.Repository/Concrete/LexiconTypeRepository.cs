using BCMStrategy.Common.Kendo;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class LexiconTypeRepository : ILexiconType
  {

    public async Task<ApiOutput> GetDDLLexiconTypeList()
    {
      ApiOutput apiOutput = new ApiOutput();
      List<DropdownMaster> lexiconTypeList;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<DropdownMaster> query = db.lexicontype
            .Select(x => new DropdownMaster()
            {
              Key = x.Id,
              Value = x.Type
            }).OrderBy(x => x.Value);
        lexiconTypeList = await query.ToListAsync();
      }
      apiOutput.Data = lexiconTypeList;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = lexiconTypeList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblLexiconType);
      return apiOutput;
    }

    /// <summary>
    /// Get all the list of lexicon type 
    /// </summary>
    /// <param name="parametersJson">Grid Parameter to filter or sorting</param>
    /// <returns>return the list</returns>
    public async Task<ApiOutput> GetAllLexiconTypeList(GridParameters parametersJson)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<LexiconTypeModel> lexiconTypeList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<LexiconTypeModel> query = db.lexicontype
            .Select(x => new LexiconTypeModel()
            {
              LexiconTypeMasterId = x.Id,
              LexiconType = x.Type
            });
        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.LexiconTypeMasterId);
        }
        lexiconTypeList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }
      apiOutput.Data = lexiconTypeList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }


  }
}
