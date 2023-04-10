using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.CustomAPI.ViewModel
{
  public class ThomsonReutersOperationViewModel
  {
    public string ItemsId { get; set; }
    public string PublishedDate { get; set; }
  }
  public class ChannelViewModel
  {
    public string ChannelCode { get; set; }
  }
  public class ItemList
  {
    public string ItemId { get; set; }
    public string URL { get; set; }
    public string PublishedDate { get; set; }
    public string Title { get; set; }
    public string BodyContent { get; set; }
  }
}
