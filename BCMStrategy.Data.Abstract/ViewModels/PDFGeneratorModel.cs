using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class PdfGeneratorModel
  {
    public int Id { get; set; }

    public int ScanningLinkDetailId { get; set; }

    public string FileName { get; set; }

    public string URL { get; set; }

    public DateTime? CreatedDate { get; set; }


    public string CreatedDateString
    {
      get
      {
        return CreatedDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(CreatedDate.Value, TimeZoneInfo.Local).ToString("ddd, dd MMMM yyyy HH:mm:ss") : "";
      }
    }

  }
}
