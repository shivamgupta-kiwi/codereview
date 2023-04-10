using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class Events
  {
    [JsonIgnore]
    public int Id { get; set; }

    public int EventId { get; set; }

    public int ProcessEventId { get; set; }

    public int ProcessTypeId { get; set; }

    public int? ProcessInstanceId { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public int PagesProcessed { get; set; }
  }
}