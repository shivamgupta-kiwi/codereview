using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.CustomAPI.Abstract
{
  public interface IThomsonReutersOperation
  {
    void GetTRIdList(int processId, int processInstanceId);
  }
}
