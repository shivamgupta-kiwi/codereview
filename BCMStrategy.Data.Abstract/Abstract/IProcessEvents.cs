using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IProcessEvents
  {
    /// <summary>
    /// Insert Events to the database
    /// </summary>
    /// <param name="scraperEvents">Event to insert in the database</param>
    /// <returns>Returns true or false for inserting events in the database</returns>
    int InsertEvents(Events scraperEvents);

    /// <summary>
    /// Update Events to the database
    /// </summary>
    /// <param name="scraperEvents">Event to update in the database</param>
    /// <returns>Returns true or false for updating events in the database</returns>
    bool UpdateEvents(Events scraperEvents);

    /// <summary>
    /// Insert Process Events to the database
    /// </summary>
    /// <param name="processEvents">Process Event to insert in the database</param>
    /// <returns>Returns true or false for inserting events in the database</returns>
    int InsertProcessEvents(ProcessEvents processEvents);

    /// <summary>
    /// Insert Process Instances
    /// </summary>
    /// <param name="processConfig">Process Configurations</param>
    /// <returns>List of Processes</returns>
    List<ProcessInstances> InsertProcesssInstances(ProcessConfiguration processConfig, string instanceName = "p");

    /// <summary>
    /// Insert Process Event Log
    /// </summary>
    /// <param name="eventLog">event log</param>
    /// <returns>Returns boolean value containing whether insert has been done successful or not</returns>
    bool InsertProcessEventLog(ProcessEventLog eventLog);

    /// <summary>
    /// Get the List of Process Instance Id's Based on Process Id
    /// </summary>
    /// <param name="processId">Pass parameter As ProcessId</param>
    /// <returns>List of ProcessInstance Id</returns>
    int[] GetListOfProcessInstanceId(int processId);

    bool CheckIsContentLoaderCompleted();

    string CheckForScrapperEngine(int processId);

    ////bool CheckFullProcessCompleted(int processId, int processInstanceId);

    ////bool SaveToSQS(string engineName, int type);

    ////int GetMessageCount(string engineName, int type);

    ////bool IsEmailGenerated(DateTime reportDate);
	}
}