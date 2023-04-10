using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCMStrategy.API.Enums
{
  public class EnumHelper
  {
    public enum ApplicationType
    {
      JavaScript = 0,
      NativeConfidential = 1
    };

    public enum ErrorType
    {
      Exception = 10,
      ArgumentException = 20
    };

    public enum QueueType
    {
      ContentLoader = 0,
      Scraper = 2
    }
  }
}
