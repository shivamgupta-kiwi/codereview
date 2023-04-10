using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.ContentLoader.DynamicContents.Abstract
{
  public interface IDynamicContent
  {
    void DynamicContentLoaderProcess(int processId, int processInstanceId, IWebDriver webDriver);
  }
}
