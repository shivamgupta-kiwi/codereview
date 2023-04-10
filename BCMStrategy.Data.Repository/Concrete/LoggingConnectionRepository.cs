using SolrNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class LoggingConnectionRepository : ISolrConnection
  {
    private readonly ISolrConnection connection;

    public LoggingConnectionRepository(ISolrConnection connection)
    {
      this.connection = connection;
    }

    public string Post(string relativeUrl, string s)
    {
      return connection.Post(relativeUrl, s);
    }

    public string PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
    {
      return connection.PostStream(relativeUrl, contentType, content, getParameters);
    }

    public string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters)
    {
      ////var stringParams = string.Join("&", parameters.Select(p => string.Format("{0}={1}", p.Key, p.Value)).ToArray());

      return connection.Get(relativeUrl, parameters);
    }

    public Task<string> PostAsync(string relativeUrl, string s)
    {
      throw new NotImplementedException();
    }

    public Task<string> PostStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters)
    {
      throw new NotImplementedException();
    }
  }
}
