using BCMStrategy.ContentLoader.HtmlPages.Abstract;
using BCMStrategy.ContentLoader.HtmlPages.Repository;
using BCMStrategy.Logger;
using System;

namespace BCMStrategy.ContentLoader.HtmlPages
{
  /// <summary>
  /// Program will fetch the simple HTML pages
  /// </summary>
  public class Program
  {
    /// <summary>
    /// Default Constructor
    /// </summary>
    protected Program()
    {

    }

    private static readonly EventLogger<Program> log = new EventLogger<Program>();

    private static IContentLoader _contentLoaderProcess;

    private static IContentLoader ContentLoaderProcess
    {
      get
      {
        if (_contentLoaderProcess == null)
        {
          _contentLoaderProcess = new ContentLoaderRepository();
        }

        return _contentLoaderProcess;
      }
    }

    /// <summary>
    /// Main Program that will be executed initially
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
      try
      {
        if (args != null && args[0] != null && args[1] != null)
        {
          int processId = Convert.ToInt32(args[0]);
          int processInstanceId = Convert.ToInt32(args[1]);
          ContentLoaderProcess.ContentLoaderProcess(processId, processInstanceId);
        }
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in initiating the ContentLoaderProcess method", ex, null);
      }
    }
  }
}