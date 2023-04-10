using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Logger;
using BCMStrategy.PDFGenerator.Abstract;
using BCMStrategy.PDFGenerator.Repository;
using System;

namespace BCMStrategy.PDFGenerator
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
    private static IPdfOperationRepository _pdfGenerator;

    private static IPdfOperationRepository PDFGenerator
    {
      get
      {
        if (_pdfGenerator == null)
        {
          _pdfGenerator = new PDFOperationRepository();
        }

        return _pdfGenerator;
      }
    }

    static void Main(string[] args)
    {
      if (args.Length > 0 && args[0] != null && args[1] != null)
      {
        int processId = string.IsNullOrEmpty(args[0]) ? 0 : Convert.ToInt32(args[0]);
        int processInstanceId = string.IsNullOrEmpty(args[1]) ? 0 : Convert.ToInt32(args[1]);
        if (processId > 0 && processInstanceId > 0)
        {
          log.LogSimple(LoggingLevel.Information, string.Format("Generate PDF process has been started with Process-Id : {0} and Process Instance-Id : {1}", processId, processInstanceId));
          PDFGenerator.GeneratePDF(processId, processInstanceId);
        }
      }
    }
  }
}
