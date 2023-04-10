using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class DocumentStorageRepository: IDocumentStorage
  {
    /// <summary>
    /// Add AWS file to storage
    /// </summary>
    /// <param name="pdfGeneratorModel">PDF generator Model</param>
    /// <returns>Is Saved or not</returns>
    public async Task<bool> SaveFileToStorage(PdfGeneratorModel pdfGeneratorModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();
        documentstorage objDocumentStorage = new documentstorage()
        {
          ScanningLinkDetailid = pdfGeneratorModel.ScanningLinkDetailId,
          URL = pdfGeneratorModel.URL,
          CreatedDate = currentTimeStamp,
        };
        db.documentstorage.Add(objDocumentStorage);

        isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return isSave;
    }


    /// <summary>
    /// Get all document list.
    /// </summary>
    /// <returns>return the list of document</returns>
    public async Task<ApiOutput> GetAllDocumentList()
    {
      ApiOutput apiOutput = new ApiOutput();
      List<PdfGeneratorModel> documentList;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<PdfGeneratorModel> query = db.documentstorage.Select(x => new PdfGeneratorModel()
            {
             CreatedDate = x.CreatedDate,
             Id = x.Id,
             ScanningLinkDetailId = x.ScanningLinkDetailid,
             URL = x.URL
            });

        documentList = await query.ToListAsync();
      }
      apiOutput.Data = documentList;
      apiOutput.TotalRecords = documentList.Count;
      return apiOutput;
    }

  }
}
