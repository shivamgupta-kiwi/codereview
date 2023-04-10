using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.ContentLoader.RSSFeeds.Abstract
{
  public interface IContentLoaderRss
  {
    void ContentLoaderRSSProcess(int processId, int processInstanceId);
  }
}
