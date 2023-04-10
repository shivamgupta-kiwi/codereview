using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.ArchivalProcess.Abstract
{
	public interface IArchivalProcess
	{
		Task RemoveDataFromSolrDB();
	}
}
