using BCMStrategy.ContentLoader.DynamicContents.Abstract;
using BCMStrategy.ContentLoader.DynamicContents.Repository;
using BCMStrategy.Logger;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;

namespace BCMStrategy.ContentLoader.DynamicContents
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

    private static IDynamicContent _dynamicContentLoader;

    private static IDynamicContent DynamicContentLoader
    {
      get
      {
        if (_dynamicContentLoader == null)
        {
          _dynamicContentLoader = new DynamicContentRepository();
        }

        return _dynamicContentLoader;
      }
    }

    private static void Main(string[] args)
    {
      try
      {
        IWebDriver webDriver = new InternetExplorerDriver();

        webDriver.Manage().Window.Maximize();

        if (args != null && args[0] != null && args[1] != null)
        {
          int processId = Convert.ToInt32(args[0]);
          int processInstanceId = Convert.ToInt32(args[1]);

          DynamicContentLoader.DynamicContentLoaderProcess(processId, processInstanceId, webDriver);

          webDriver.Close();
          webDriver.Quit();
          webDriver.Dispose();
        }
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in GetInnerChildContents method", ex, null);
      }
      finally
      {
        Environment.Exit(0);
      }
    }
  }
}
