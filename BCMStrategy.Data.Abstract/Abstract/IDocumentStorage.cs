using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IDocumentStorage
  {
    /// <summary>
    /// Add AWS file to storage
    /// </summary>
    /// <param name="pdfGeneratorModel">PDF generartor Model</param>
    /// <returns>Is Saved or not</returns>
    Task<bool> SaveFileToStorage(PdfGeneratorModel pdfGeneratorModel);

    /// <summary>
    /// Get all document list.
    /// </summary>
    /// <returns>return the list of document</returns>
    Task<ApiOutput> GetAllDocumentList();
  }
}
