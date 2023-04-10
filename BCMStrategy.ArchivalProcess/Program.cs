using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.ArchivalProcess;

namespace BCMStrategy.ArchivalProcess
{
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		private static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
								new ArchivalProcess()
			};
			ServiceBase.Run(ServicesToRun);
			////ArchivalProcess myServ = new ArchivalProcess();
			////myServ.StartService();
			System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
		}
	}
}
