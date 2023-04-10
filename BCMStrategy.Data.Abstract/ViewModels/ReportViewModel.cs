using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
	public class ReportViewModel
	{
		public ReportViewModel()
		{
			this.ActionType = new List<ActionType>();
		}
		public string Lexicon { get; set; }
    ////public string CombinationValue { get; set; }
		public int LexiconId { get; set; }
		public decimal LexiconValuesSum { get; set; }
		public bool IsAggregateDisplay { get; set; }

		private string _lexiconHashId { get; set; }

		public string LexiconHashId
		{
			get
			{
				if (string.IsNullOrEmpty(_lexiconHashId))
				{
					_lexiconHashId = this.LexiconId == 0 ? string.Empty : this.LexiconId.ToEncrypt();
				}
				return _lexiconHashId;
			}
			set
			{
				_lexiconHashId = value;
			}
		}

		public string SelectedDate { get; set; }
		public List<string> SelectedLexicons { get; set; }

    ////public string LexiconHashId { get; set; }
		public List<ActionType> ActionType { get; set; }

		public int LexiconTypeId { get; set; }
		private string _lexiconTypeHashId { get; set; }

		public string LexiconTypeHashId
		{
			get
			{
				if (string.IsNullOrEmpty(_lexiconTypeHashId))
				{
					_lexiconTypeHashId = this.LexiconTypeId == 0 ? string.Empty : this.LexiconTypeId.ToEncrypt();
				}
				return _lexiconTypeHashId;
			}
			set
			{
				_lexiconTypeHashId = value;
			}
		}

		public string LexiconType { get; set; }

		public string RefHashId { get; set; }

		public string ScanDate { get; set; }

		public string Key { get; set; }

		public bool IsDirect { get; set; }

	}

	public class ActionType
	{
		public int ActionTypeMasterId { get; set; }

		private string _actionTypeHashId { get; set; }

		public string ActionTypeHashId
		{
			get
			{
				if (string.IsNullOrEmpty(_actionTypeHashId))
				{
					_actionTypeHashId = this.ActionTypeMasterId == 0 ? string.Empty : this.ActionTypeMasterId.ToEncrypt();
				}
				return _actionTypeHashId;
			}
			set
			{
				_actionTypeHashId = value;
			}
		}

		public string Name { get; set; }
		public decimal Value { get; set; }


    ////public bool isAggregateDisplay=false,
    ////  string lexiconTypeHashId="", string selectedDate = ""

    ////public List<ActivityType> ActivityType { get; set; }
	}

	public class ActivityType
	{
		public string Name { get; set; }
		public int Value { get; set; }
		public decial SearchValue { get; set; }
		public string AcivityType { get; set; }
		public string ColorCode { get; set; }
	}

	public class WebsiteUrl
	{
		public string Url { get; set; }
		public string ActivityType { get; set; }
	}

	public class ConsolidateList
	{
		public List<WebsiteUrl> websiteURL { get; set; }
		public List<ActivityType> activityTypeList { get; set; }
		public bool IsActivityTypeExists { get; set; }
		public string ActivityTypeName { get; set; }
		public string LexiconTerm { get; set; }
		public string Date { get; set; }
	}
}
