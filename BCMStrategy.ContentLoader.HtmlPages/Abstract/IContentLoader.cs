using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.ContentLoader.HtmlPages.Abstract
{
  public interface IContentLoader
  {
    void ContentLoaderProcess(int processId, int processInstanceId);
  }
}
