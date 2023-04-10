using BCMStrategy.ContentLoader.RSSFeeds.Abstract;
using BCMStrategy.ContentLoader.RSSFeeds.Repository;
using BCMStrategy.Logger;
using System;

namespace BCMStrategy.ContentLoader.RSSFeeds
{
  public class Program
  {
    /// <summary>
    /// Default Constructor
    /// </summary>
    protected Program()
    {

    }

    private static readonly EventLogger<Program> log = new EventLogger<Program>();

    private static IContentLoaderRss _contentLoaderProcess;

    private static IContentLoaderRss ContentLoaderProcess
    {
      get
      {
        if (_contentLoaderProcess == null)
        {
          _contentLoaderProcess = new ContentLoaderRSSRespository();
        }

        return _contentLoaderProcess;
      }
    }

    public static void Main(string[] args)
    {
      try
      {
        if (args != null && args[0] != null && args[1] != null)
        {
          int processId = Convert.ToInt32(args[0]);
          int processInstanceId = Convert.ToInt32(args[1]);

          ContentLoaderProcess.ContentLoaderRSSProcess(processId, processInstanceId);
        }
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in initiating the ContentLoaderRSSProcess method", ex, null);
      }
    }
  }
}