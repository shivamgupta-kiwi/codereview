using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IWebDocument
  {
    /// <summary>
    /// Returns Web Document Count
    /// </summary>
    /// <param name="processId">Process Id</param>
    /// <returns>Web Document count</returns>
    int WebDocumentCount(int processId);

    /// <summary>
    /// Get Document List
    /// </summary>
    /// <param name="processId">Process Id of the document</param>
    /// <param name="processInstanceId">Process Instance Id of the document</param>
    /// <returns>List of Document links to be scrapped</returns>
    List<LoaderLinkQueue> GetDocumentList(int processId, int processInstanceId);
  }
}