using System;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class DefaultLexicon
  {
    public int Id { get; set; }

    public int UserId { get; set; }

    public int LexiconIssueId { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; }
  }
}