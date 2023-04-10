using BCMStrategy.DAL.Context;
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
 public class WebsiteTypesRepository : IWebsiteTypes
  {
    public async Task<ApiOutput> GetDDLWebsiteTypesList()
    {
      ApiOutput apiOutput = new ApiOutput();
      List<DropdownMaster> websiteTypesList;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<DropdownMaster> query = db.websitetypes
            .Select(x => new DropdownMaster()
            {
              Key = x.Id,
              Value = x.TypeName
            }).OrderBy(x => x.Value);
        websiteTypesList = await query.ToListAsync();
      }
      apiOutput.Data = websiteTypesList;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = websiteTypesList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblWebsiteTypes);
      return apiOutput;
    }
  }
}
