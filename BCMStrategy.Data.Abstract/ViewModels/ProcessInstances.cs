using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ProcessInstances
  {
    public int Id { get; set; }

    public int ProcessId { get; set; }

    public string ProcessInstanceName { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; }
  }

  public class ProcessConfiguration
  {
    public int ProcessId { get; set; }

    public int TotalWebPages { get; set; }

    public int WebPagesPerProcess { get; set; }

    public string ProcessName { get; set; }
  }
}