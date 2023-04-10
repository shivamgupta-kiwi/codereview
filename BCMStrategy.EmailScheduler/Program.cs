using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.EmailScheduler.Abstract;
using BCMStrategy.EmailScheduler.Repository;
using BCMStrategy.Logger;
using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace BCMStrategy.EmailScheduler
{
  public class Program
  {
    /// <summary>
    /// Default Constructor
    /// </summary>
    protected Program()
    {

    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    private static void Main()
    {
      ServiceBase[] ServicesToRun;
      ServicesToRun = new ServiceBase[]
							{
								new EmailService()
							};
      ServiceBase.Run(ServicesToRun);

      ////EmailService myServ = new EmailService();
      ////myServ.StartService();
      ////System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);


      ////List<EmailServiceModel> emailServiceModel = emailServiceRepository.GetCustomerDataForEmail();

    }

  }
}
