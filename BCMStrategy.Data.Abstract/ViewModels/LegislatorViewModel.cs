using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class LegislatorViewModel
  {
    public int LegislatorMasterId { get; set; }
    private string _legislatorMasterHashId { get; set; }

    public string LegislatorHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_legislatorMasterHashId))
        {
          _legislatorMasterHashId = this.LegislatorMasterId == 0 ? string.Empty : this.LegislatorMasterId.ToEncrypt();
        }
        return _legislatorMasterHashId;
      }
      set
      {
        _legislatorMasterHashId = value;
      }
    }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblFirstName", ResourceType = typeof(Resource))]
    public string FirstName { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLastName", ResourceType = typeof(Resource))]
    public string LastName { get; set; }

    public List<DropdownMaster> DesignationDDL { get; set; }


    [ListHasElementsAttribute(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblDesignation", ResourceType = typeof(Resource))]
    public List<string> DesignationHashIds { get; set; }

    public List<DropdownMaster> CommiteeDDL { get; set; }

    public List<string> CommiteeHashIds { get; set; }

    public int SectorId { get; set; }

    public List<DropdownMaster> SectorDDL { get; set; }

    private string _sectorHashId { get; set; }

    /// <summary>
    /// Created Date
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Sector Hash Id
    /// </summary>
    public string SectorHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_sectorHashId))
        {
          _sectorHashId = this.SectorId == 0 ? string.Empty : this.SectorId.ToEncrypt();
        }
        return _sectorHashId;
      }
      set
      {
        _sectorHashId = value;
      }
    }

    public int EntityId { get; set; }

    public List<DropdownMaster> EntityDDL { get; set; }

    private string _entityHashId { get; set; }

    /// <summary>
    /// Sector Hash Id
    /// </summary>
    public string EntityHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_entityHashId))
        {
          _entityHashId = this.EntityId == 0 ? string.Empty : this.EntityId.ToEncrypt();
        }
        return _entityHashId;
      }
      set
      {
        _entityHashId = value;
      }
    }


    /// <summary>
    /// Country id
    /// </summary>
    public int CountryId { get; set; }

    private string _countryHashId { get; set; }


    /// <summary>
    /// Entity Type HashId
    /// </summary>
    //[Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblCountry", ResourceType = typeof(Resource))]
    public string CountryHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_countryHashId))
        {
          _countryHashId = this.CountryId == 0 ? string.Empty : this.CountryId.ToEncrypt();
        }
        return _countryHashId;
      }
      set
      {
        _countryHashId = value;
      }
    }

    public string Entity { get; set; }

    public string Sector { get; set; }

    public string Country { get; set; }

   
  }
}
