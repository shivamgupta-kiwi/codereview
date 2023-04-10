using Newtonsoft.Json;
namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class IndividualPersonViewModel
  {
    [JsonIgnore]
    public int PolicyMakerMasterId { get; set; }
    private string _individualPersonMasterHashId;
    public string PolicyMakerMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_individualPersonMasterHashId))
        {
          _individualPersonMasterHashId = this.PolicyMakerMasterId == 0 ? string.Empty : this.PolicyMakerMasterId.ToEncrypt();
        }
        return _individualPersonMasterHashId;
      }
      set
      {
        _individualPersonMasterHashId = value;
      }
    }

    /// <summary>
    /// first Name
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Last Name
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Designation
    /// </summary>
    public string Designation { get; set; }
  }
}