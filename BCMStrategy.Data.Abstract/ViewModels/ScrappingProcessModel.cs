using BCMStrategy.DAL.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BCMStrategy.Data.Abstract.ViewModels
{
	public class ScrappingProcessModel
	{
		public int ScrappingId { get; set; }

		[JsonIgnore]
		public int WebSiteId { get; set; }

		private string _webSiteHashId { get; set; }

		public string WebSiteHashId
		{
			get
			{
				if (string.IsNullOrEmpty(_webSiteHashId))
				{
					_webSiteHashId = this.WebSiteId == 0 ? string.Empty : this.WebSiteId.ToEncrypt();
				}
				return _webSiteHashId;
			}
			set
			{
				_webSiteHashId = value;
			}
		}

		public string WebsiteURL { get; set; }

		public string GUID { get; set; }

		public int ProcessId { get; set; }
		////public DateTime? _ScanDate { get; set; }

		////public string ScannedDate
		////{
		////  get
		////  {
		////    return _ScanDate.HasValue ? _ScanDate.Value.ToString("ddd, dd MMMM yyyy HH:mm:ss") : "";
		////  }
		////}

		public int ReadTaken { get; set; }

		public List<ScrappingData> ChildURLList { get; set; }
	}

	public class ScrappingData
	{
		public int ScrappedId { get; set; }

		public string ScrappedWebsiteURL { get; set; }

		public byte[] Content { get; set; }

		public string ContentString
		{
			get
			{
				return Content != null ? System.Text.Encoding.UTF8.GetString(Content) : "";
			}
		}

		public List<ProprietoryTagData> ProprietoryTagList { get; set; }

		public StandardTagData StandardTagData { get; set; }

		public List<LexiconTagData> LexiconTagList { get; set; }

		public string DocumentURL { get; set; }
	}

	public class ConsolidateModel
	{
		public List<ScrappingProcessModel> scrappingModelList { get; set; }
	}

	public class LexiconTagData
	{
		public string LexiconTerm { get; set; }
		public string CombinationValues { get; set; }
		public int SearchCount { get; set; }
	}

	public class ProprietoryTagData
	{
		public int? ProcessId { get; set; }
		public int ProprietoryTagDataId { get; set; }
		public int MetadatatypeId { get; set; }
		public string MetadataTypeName { get; set; }
		public int SearchTypeId { get; set; }
		public int? ActivityTypeId { get; set; }
		public string SearchByType { get; set; }
		public string LexiconTerm { get; set; }
		public string WebsiteUrl { get; set; }
		public string CombinationValue { get; set; }

    public string SearchType
    {
      get
      {
        string data = string.Empty;
        using (BCMStrategyEntities db = new BCMStrategyEntities())
        {
          if (SearchByType != null && SearchTypeId > 0)
          {
            if (SearchByType == BCMStrategy.Data.Abstract.Helper.ScrappingTypes.BY_PHRASE.ToString().Replace("_", " "))
            {
              data = db.metadataphrases.Where(w => w.Id == SearchTypeId).Select(s => s.ActivityTypeId.HasValue ? s.activitytype.ActivityName : s.Phrases).FirstOrDefault();
              ////data = db.metadataphrases.Where(w => w.Id == SearchTypeId).Select(s => s.Phrases).FirstOrDefault();
            }
            else if (SearchByType == BCMStrategy.Data.Abstract.Helper.ScrappingTypes.BY_ACTIVITY_TYPE.ToString().Replace("_", " "))
            {
              data = db.activitytype.Where(w => w.Id == SearchTypeId).Select(s => s.ActivityName).FirstOrDefault();
            }
            else if (SearchByType == BCMStrategy.Data.Abstract.Helper.ScrappingTypes.BY_NOUN_VERB.ToString().Replace("_", " "))
            {
              var data1 = db.metadatanounplusverb.Where(w => w.Id == SearchTypeId).Select(x => new { Noun = x.Noun, Verb = x.Verb, ActivityName = x.ActivityTypeId.HasValue ? x.activitytype.ActivityName : string.Empty }).FirstOrDefault();
              data = string.IsNullOrEmpty(data1.ActivityName) ? string.Format("{0} {1}", data1.Noun, data1.Verb) : data1.ActivityName;
            }
            else
            {
              data = SearchTypeId > 0 ? db.activitytype.Where(w => w.Id == SearchTypeId).Select(s => s.ActivityName).FirstOrDefault() : string.Empty;
            }
          }
        }
        return data;
      }
      ////set;
    }

		public string ColorCode
		{
			get
			{
				string colorCode = string.Empty;
				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					if (SearchByType != null && SearchTypeId > 0)
					{
						if (SearchByType == BCMStrategy.Data.Abstract.Helper.ScrappingTypes.BY_ACTIVITY_TYPE.ToString().Replace("_", " "))
						{
							colorCode = db.activitytype.Where(w => w.Id == SearchTypeId).Select(s => s.ColorCode).FirstOrDefault();
						}
						else if (SearchByType == BCMStrategy.Data.Abstract.Helper.ScrappingTypes.BY_NOUN_VERB.ToString().Replace("_", " "))
						{
							var data1 = db.metadatanounplusverb.Where(w => w.Id == SearchTypeId).Select(x => new { Noun = x.Noun, Verb = x.Verb, ColorCode = x.ActivityTypeId.HasValue ? x.activitytype.ColorCode : string.Empty }).FirstOrDefault();
							colorCode = string.IsNullOrEmpty(data1.ColorCode) ? string.Empty : data1.ColorCode;
						}
						else
						{
							colorCode = SearchTypeId > 0 ? db.activitytype.Where(w => w.Id == SearchTypeId).Select(s => s.ColorCode).FirstOrDefault() : string.Empty;
						}
					}
				}
				return colorCode;
			}
		}

		public decimal SearchValue { get; set; }
		public int SearchCount { get; set; }
	}

	public class StandardTagData
	{
		public string CountryName { get; set; }
		public string EntityName { get; set; }
		public string EntityTypeName { get; set; }
		public string Sectors { get; set; }
		public string Individual { get; set; }
		public string SearchType { get; set; }
		public string DateOfIssue { get; set; }
	}

	public class ScrappingProcessSummaryModel
	{
		public int ScrappingId { get; set; }

		[JsonIgnore]
		public int WebSiteId { get; set; }

		private string _webSiteHashId { get; set; }

		public string WebSiteHashId
		{
			get
			{
				if (string.IsNullOrEmpty(_webSiteHashId))
				{
					_webSiteHashId = this.WebSiteId == 0 ? string.Empty : this.WebSiteId.ToEncrypt();
				}
				return _webSiteHashId;
			}
			set
			{
				_webSiteHashId = value;
			}
		}

		public string WebsiteURL { get; set; }

		public string GUID { get; set; }

		public int ProcessId { get; set; }
		public DateTime? _ScanDate { get; set; }

		public string ScannedDate
		{
			get
			{
				return _ScanDate.HasValue ? _ScanDate.Value.ToString("ddd, dd MMMM yyyy HH:mm:ss") : "";
				////return _ScanDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(_ScanDate.Value, TimeZoneInfo.Local).ToString("ddd, dd MMMM yyyy HH:mm:ss") : "";
			}
		}

		public DateTime? PreviousProcessDate { get; set; }

		public string PreviousProcessDateFormated
		{
			get
			{
				return PreviousProcessDate.HasValue ? PreviousProcessDate.Value.ToString("ddd, dd MMMM yyyy HH:mm:ss") : "";
			}
		}

		public bool ChildURLAvaliable { get; set; }

		public List<ScrappingSummaryData> ChildURLList { get; set; }
	}

	public class ScrappingSummaryData
	{
		public int LexiconCount { get; set; }

		public int ProprietaryTagsCount { get; set; }

		public int StandardTagsCount { get; set; }
	}

	public class SummaryModel
	{
		public int SrNo { get; set; }

		[JsonIgnore]
		public int WebSiteId { get; set; }

		private string _webSiteHashId { get; set; }

		public string WebSiteHashId
		{
			get
			{
				if (string.IsNullOrEmpty(_webSiteHashId))
				{
					_webSiteHashId = this.WebSiteId == 0 ? string.Empty : this.WebSiteId.ToEncrypt();
				}
				return _webSiteHashId;
			}
			set
			{
				_webSiteHashId = value;
			}
		}

		public bool IsStandardTagsAvaialble { get; set; }
		public bool IsProprietaryTagsAvaialble { get; set; }
		public bool IsLexiconAvaialble { get; set; }
		public bool ChildURLAvaliable { get; set; }
		public string WebSiteURL { get; set; }
		public string Scan1 { get; set; }
		public string Scan2 { get; set; }
		public string ProprietoryTag { get; set; }
		public string StandardTag { get; set; }
		public string LastScanDate { get; set; }
		public string PreviousScanDate { get; set; }
	}
}