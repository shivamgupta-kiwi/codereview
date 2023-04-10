using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.PDFGenerator.Abstract
{
  public interface IPdfOperationRepository
  {
    void GeneratePDF(int processId, int processInstanceId);
  }
}
