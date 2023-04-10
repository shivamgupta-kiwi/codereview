using System.Collections.Generic;
using System.Text;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ScrappedContentMapping
  {
    public int LexiconId { get; set; }

    public string LexiconTerms { get; set; }

    public int IssueCount { get; set; }
  }

  public class ScrappedProprietoryTagsMapping
  {
    public int? SearchTypeId { get; set; }

    public int MetaDataId { get; set; }

    public int SearchCount { get; set; }

    public string SearchType { get; set; }

    public string MetaDataName { get; set; }

    public decimal SearchValue { get; set; }

    public int? ActivityTypeId { get; set; }

		public StringBuilder HtmlResult { get; set; }

		public bool IsFullSearchRequired { get; set; }

	}

	public class ScrappedContentMappingCounts
	{
		public StringBuilder HtmlContents { get; set; }

		public List<ScrappedContentMapping> ActaulLexiconCounts { get; set; }
	}
}