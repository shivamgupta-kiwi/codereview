using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BCMStrategy.Data.Abstract.ViewModels
{
	public class WebLinkMetaDataModel
	{
		private string _activityTypeMasterHashId;

		[JsonIgnore]
		public int? ActivityTypeMasterId { get; set; }

		public string ActivityTypeMasterHashId
		{
			get
			{
				if (string.IsNullOrEmpty(_activityTypeMasterHashId))
				{
					_activityTypeMasterHashId = this.ActivityTypeMasterId == 0 ? string.Empty : this.ActivityTypeMasterId.ToEncrypt();
				}
				return _activityTypeMasterHashId;
			}
			set
			{
				_activityTypeMasterHashId = value;
			}
		}

		public string ActivityName { get; set; }

		public int MetaDataMasterId { get; set; }

		public string MetaDataName { get; set; }

		public decimal ActivityValue { get; set; }

		public bool IsActivityTypeAssignedWebLink { get; set; }

		public bool IsFullSearchRequired { get; set; }
	}

	public class WebLinkPhraseModel
	{
		private string _phraseMasterHashId;

		[JsonIgnore]
		public int? PhraseMasterId { get; set; }

		public string PhraseMasterHashId
		{
			get
			{
				if (string.IsNullOrEmpty(_phraseMasterHashId))
				{
					_phraseMasterHashId = this.PhraseMasterId == 0 ? string.Empty : this.PhraseMasterId.ToEncrypt();
				}
				return _phraseMasterHashId;
			}
			set
			{
				_phraseMasterHashId = value;
			}
		}

		public string Phrase { get; set; }

		public int MetaDataMasterId { get; set; }

		public string MetaDataName { get; set; }

		public decimal MetaDataValue { get; set; }
	}

	public class WebLinkNounPVerbModel
	{
		private string _nounPVerbMasterHashId;

		[JsonIgnore]
		public int? NounPVerbMasterId { get; set; }

		public string NounPVerbMasterHashId
		{
			get
			{
				if (string.IsNullOrEmpty(_nounPVerbMasterHashId))
				{
					_nounPVerbMasterHashId = this.NounPVerbMasterId == 0 ? string.Empty : this.NounPVerbMasterId.ToEncrypt();
				}
				return _nounPVerbMasterHashId;
			}
			set
			{
				_nounPVerbMasterHashId = value;
			}
		}

		public string Noun { get; set; }

		public string Verb { get; set; }

		public int MetaDataMasterId { get; set; }

		public string MetaDataName { get; set; }

		public decimal MetaDataValue { get; set; }

		public bool IsHardCode { get; set; }

		public int? ActivityTypeId { get; set; }
	}

	public class DynamicNounPVerbResultModel
	{
		public string Name { get; set; }
	}

	public class WebLinkStandardTagsModel
	{
		public List<WebLinkEntityModel> EntityTypeList { get; set; }

		public List<WebLinkSectorsModel> SectorsList { get; set; }

		public List<WebLinkIndividualsModel> IndividualsList { get; set; }

		public string Content { get; set; }

		public DateTime? DateOfIssue { get; set; }
	}

	public class WebLinkEntityModel
	{
		public int? CountryId { get; set; }

		public string CountryName { get; set; }

		public int? EntityId { get; set; }

		public string EntityName { get; set; }

		public int? EntityTypeId { get; set; }

		public string EntityTypeName { get; set; }
	}

	public class WebLinkSectorsModel
	{
		public int SectorId { get; set; }

		public string SectorName { get; set; }
	}

	public class WebLinkIndividualsModel
	{
		public int IndividualId { get; set; }

		public string IndividualFirstName { get; set; }

		public string IndividualLastName { get; set; }

		public string Designation { get; set; }

		public string IndividualFirstNLastName
		{
			get
			{
				return string.Format("{0} {1}", IndividualFirstName, IndividualLastName).Trim();
			}
		}

		public string IndividualDesignationNLName
		{
			get
			{
				return string.Format("{0} {1}", Designation.Trim(), IndividualLastName).Trim();
			}
		}

		public string IndividualFullName
		{
			get
			{
				return string.Format("{0} {1} {2}", Designation.Trim(), IndividualFirstName, IndividualLastName).Trim();
			}
		}

		public int? CountryId { get; set; }

		public int? InstiutionTypeId { get; set; }
	}

	public class StandardTagDates
	{
		public string ActualDate { get; set; }
		public DateTime FormatedDate { get; set; }
	}

	public class DynamicValuesOfNounPVerb
	{
		public string PageContent { get; set; }

		public List<string> SearchedBy { get; set; }
	}

	public class WebLinkStandardTagsMasterModel
	{
		public List<MediaSectorIndividualsModel> PolicyMakersList { get; set; }

		public List<MediaSectorStateHeadModel> StateHeadList { get; set; }

		public List<MediaSectorInstitutionsModel> InstitutionsList { get; set; }

	}

	public class MediaSectorIndividualsModel
	{
		public int IndividualId { get; set; }

		public string IndividualFirstName { get; set; }

		public string IndividualLastName { get; set; }

		public string Designation { get; set; }

		public string IndividualFirstNLastName
		{
			get
			{
				return string.Format("{0} {1}", IndividualFirstName, IndividualLastName).Trim();
			}
		}

		public string IndividualDesignationNLName
		{
			get
			{
				return string.Format("{0} {1}", Designation.Trim(), IndividualLastName).Trim();
			}
		}

		public string IndividualFullName
		{
			get
			{
				return string.Format("{0} {1} {2}", Designation.Trim(), IndividualFirstName, IndividualLastName).Trim();
			}
		}

		public int? CountryId { get; set; }

		public int? InstiutionTypeId { get; set; }

		public List<string> InstitutionId { get; set; }
	}

	public class MediaSectorInstitutionsModel
	{
		public int InstitutionsId { get; set; }

		public string InstitutionsName { get; set; }

		public string InstitutionsAbbreviation { get; set; }

		public int? CountryId { get; set; }

		public int? InstiutionTypeId { get; set; }
	}

	public class MediaSectorStateHeadModel
	{
		public int StateHeadId { get; set; }

		public string StateHeadFirstName { get; set; }

		public string StateHeadLastName { get; set; }

		public string Designation { get; set; }

		public string StateHeadFirstNLastName
		{
			get
			{
				return string.Format("{0} {1}", StateHeadFirstName, StateHeadLastName).Trim();
			}
		}

		public string StateHeadDesignationNLName
		{
			get
			{
				return string.Format("{0} {1}", Designation.Trim(), StateHeadLastName).Trim();
			}
		}

		public int CountryId { get; set; }
	}

	public class ProrietarySearchModel
	{
		public string SearchType { get; set; }

		public StringBuilder HtmlResult { get; set; }

		public Object ObjectList { get; set; }

		/*Only for Noun + Verb Search*/
		public List<string> SearchedBy { get; set; }
	}

	////public class MediaSectorLegislatorModel
	////{
	////  public int LegislatorId { get; set; }

	////  public string LegislatorFirstName { get; set; }

	////  public string LegislatorLastName { get; set; }

	////  public string Designation { get; set; }

	////  public string LegislatorFirstNLastName
	////  {
	////    get
	////    {
	////      return string.Format("{0} {1}", LegislatorFirstName, LegislatorLastName).Trim();
	////    }
	////  }

	////  public string LegislatorDesignationNLName
	////  {
	////    get
	////    {
	////      return string.Format("{0} {1}", Designation.Trim(), LegislatorLastName).Trim();
	////    }
	////  }

	////  public int? CountryId { get; set; }
	////}

}