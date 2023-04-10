using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BCMStrategy.DAL.Context;
using BCMStrategy.Resources;
using Newtonsoft.Json;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class SearchablePdfParameters
	{
    [Required(ErrorMessageResourceName = "ValidationDate", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
		[Display(Name = "LblFrom", ResourceType = typeof(Resource))]
		public string FromDate { get; set; }

    [Required(ErrorMessageResourceName = "ValidationDate", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
		[Display(Name = "LblTo", ResourceType = typeof(Resource))]
		public string ToDate { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
		[Display(Name = "LblLexicon", ResourceType = typeof(Resource))]
		public string Lexicon { get; set; }

		public string ActionType { get; set; }
	}

	public class SearchablePdfParametersForDrillDown
	{
		public string MonthDate { get; set; }

		public string FromDate { get; set; }

		public string ToDate { get; set; }

		public string Lexicon { get; set; }
	}

	public class SearchablePdfModelRaw
	{
		public string WebSiteURL { get; set; }

		public string PDFURL { get; set; }

		private DateTime _created { get; set; }
		public DateTime? Created
		{
			get
			{
				return _created;
			}
			set
			{
				if (value.HasValue)
				{
					_created = value.Value;
					_createdString = value.Value.ToFormatedDateTime("dd-MMM-yy");
					_createdMonthString = value.Value.ToFormatedDateTime("MMM-yy");
				}

			}
		}
		private string _createdString { get; set; }
		public string CreatedString
		{
			get
			{
				return _createdString;
			}
			set
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					_createdString = value;
					DateTime? date = Helper.FromFormatedDateTime(value, "MM/dd/yyyy");
					if (date.HasValue)
						_created = Convert.ToDateTime(value);

				}
			}
		}

		private string _createdMonthString { get; set; }
		public string CreatedMonthString
		{
			get
			{
				return _createdMonthString;
			}
		}

		public int ScrappedContentMappingId { get; set; }

		public int ScanningLinkDetailId { get; set; }

		public scrappedproprietorytagsmapping ScrappedProprietoryTagsMapping { get; set; }
	}

	public class SearchablePdfModel
	{
		public string WebSiteURL { get; set; }

		public string PDFURL { get; set; }

		public string ProprietaryTags { get; set; }

		private DateTime _createdDate { get; set; }
		public DateTime? CreatedDate
		{
			get
			{
				return _createdDate;
			}
			set
			{
				if (value.HasValue)
				{
					_createdDate = value.Value;
					_created = value.Value.ToFormatedDateTime("dd-MMM-yy");
					_createdMonthString = value.Value.ToFormatedDateTime("MMM-yy");
				}

			}
		}
		private string _created { get; set; }
		public string Created
		{
			get
			{
				return _created;
			}
			set
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					_created = value;
					DateTime? date = Helper.FromFormatedDateTime(value, "MM/dd/yyyy");
					if (date.HasValue)
						_createdDate = Convert.ToDateTime(value);

				}
			}
		}

		private string _createdMonthString { get; set; }
		public string CreatedMonthString
		{
			get
			{
				return _createdMonthString;
			}
		}

	}

	public class SearchablePdfLineChartModel
	{
		public SearchablePdfLineChartModel()
		{
			this.ProprietaryModel = new List<SearchablePdfLineChartProprietaryModel>();
		}


		public string CreatedString { get; set; }

		public string FromDate { get; set; }

		public string ToDate { get; set; }

		public List<SearchablePdfLineChartProprietaryModel> ProprietaryModel { get; set; }

	}

	public class SearchablePdfLineChartProprietaryModel
	{
		public string ProprietaryTagType { get; set; }

		public decimal ProprietaryTagValue { get; set; }
	}
}