using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Resources;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class CountryMasterRepository: ICountryMaster
  {
    /// <summary>
    /// Get the Country in Dropdown list from Country code
    /// </summary>
    /// <returns>Country Dropdown List</returns>
    public async Task<ApiOutput> GetDDLCountryList()
    {
      ApiOutput apiOutput = new ApiOutput();
      List<DropdownMaster> countryDropdownList;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<DropdownMaster> query = db.country
            .Select(x => new DropdownMaster()
            {
              Key = x.Id,
              Value = x.Name
            }).OrderBy(x => x.Value);
        countryDropdownList = await query.ToListAsync();
      }
      apiOutput.Data = countryDropdownList;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = countryDropdownList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblCountry);
      return apiOutput;
    }

    /// <summary>
    /// Get Country Id By Name
    /// </summary>
    /// <param name="countryName">Pass Country Name</param>
    /// <returns>Return Id</returns>
    public int GetCountryIdByName(string countryName)
    {
      int countryId = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (!string.IsNullOrEmpty(countryName))
        {
          countryId = db.country.Where(x => x.Name == countryName).Select(x=>x.Id).FirstOrDefault();
        }
      }
      return countryId;
    }
  }
}
