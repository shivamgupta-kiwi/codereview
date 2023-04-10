using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
using System.Linq;

namespace BCMStrategy.Logger
{
  /// <summary>
  /// Enum LoggingLevel
  /// </summary>
  public enum LoggingLevel
  {
    /// <summary>
    /// Debug Event Entry
    /// </summary>
    Debug = 0,

    /// <summary>
    /// Information Event Entry
    /// </summary>
    Information = 1,

    /// <summary>
    /// Warning Event Entry
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Error Event Entry
    /// </summary>
    Error = 3,

    /// <summary>
    /// Fatal Event Entry
    /// </summary>
    Fatal = 4
  }

  /// <summary>
  /// Class EventLogger.
  /// </summary>
  public class EventLogger<T> //: IEventLog<T> where T : class
  {
    #region Variable and Property-

    /// <summary>
    /// The logger
    /// </summary>
    private readonly ILog logger;

    private readonly string logRepositoryName;

    /// <summary>
    /// Gets the logger.
    /// </summary>
    /// <value>The logger.</value>
    ////private ILog Logger
    ////{
    ////  get
    ////  {
    ////    if (logger == null)
    ////    {
    ////      logger = log4net.LogManager.GetLogger(typeof(EventLogger<T>));
    ////    }

    ////    return logger;
    ////  }
    ////}

    #endregion Variable and Property-

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="EventLogger{T}"/> class.
    /// </summary>
    public EventLogger()
    {
      XmlConfigurator.Configure();
      logger = LogManager.GetLogger(typeof(T));
      log4net.Config.BasicConfigurator.Configure();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventLogger{T}"/> class.
    /// </summary>
    /// <param name="FileName">Name of the file.</param>
    public EventLogger(string FileName)
    {
      logRepositoryName = FileName + "Repository";
      log4net.Repository.ILoggerRepository loggerRepository = LogManager.GetAllRepositories().FirstOrDefault(x => x.Name == logRepositoryName);
      if (loggerRepository == null)
      {
        loggerRepository = LogManager.CreateRepository(logRepositoryName);
      }

      ThreadContext.Properties["LogName"] = FileName;
      XmlConfigurator.Configure(loggerRepository);
      logger = LogManager.GetLogger(FileName + "Repository", typeof(T).Name);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventLogger{T}"/> class.
    /// </summary>
    /// <param name="configFile">The configuration file.</param>
    public EventLogger(System.IO.FileInfo configFile)
    {
      XmlConfigurator.Configure(configFile);
      logger = LogManager.GetLogger(typeof(T));
    }

    #endregion Constructor

    #region Public Methods

    /// <summary>
    /// Log messages at the specified logging level. Allows arbitrary objects to be logged via automatic serialization.
    /// </summary>
    /// <param name="loggingLevel">Level at which the message will be logged</param>
    /// <param name="message">Message to log</param>
    /// <param name="args">Any parameters to log with the debug message</param>
    public void Log(LoggingLevel loggingLevel, string message, params object[] args)
    {
      // Only build the message string if logging of the specified error level is enabled
      if ((loggingLevel == LoggingLevel.Debug && logger.IsDebugEnabled) ||
          (loggingLevel == LoggingLevel.Information && logger.IsInfoEnabled) ||
          (loggingLevel == LoggingLevel.Warning && logger.IsWarnEnabled))
      {
        // Creating the debug message using a function
        String formattedDebugMessage = FormatLoggingStatement(BuildDebuggingInfo(2), message, string.Empty, args);

        switch (loggingLevel)
        {
          case LoggingLevel.Debug:
            logger.Debug(formattedDebugMessage);
            break;

          case LoggingLevel.Information:
            logger.Info(formattedDebugMessage);
            break;

          case LoggingLevel.Warning:
            logger.Warn(formattedDebugMessage);
            break;
        }
      }
    }

    public void LogSimple(LoggingLevel loggingLevel, string message)
    {
      // Only build the message string if logging of the specified error level is enabled
      if ((loggingLevel == LoggingLevel.Debug && logger.IsDebugEnabled) ||
          (loggingLevel == LoggingLevel.Information && logger.IsInfoEnabled) ||
          (loggingLevel == LoggingLevel.Warning && logger.IsWarnEnabled))
      {
        // Creating the debug message using a function
        String formattedDebugMessage = FormatLoggingStatementSimple(message);

        switch (loggingLevel)
        {
          case LoggingLevel.Debug:
            logger.Debug(formattedDebugMessage);
            break;

          case LoggingLevel.Information:
            logger.Info(formattedDebugMessage);
            break;

          case LoggingLevel.Warning:
            logger.Warn(formattedDebugMessage);
            break;
        }
      }
    }

    /// <summary>
    /// Logs the error.
    /// </summary>
    /// <param name="loggingLevel">The logging level.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="args">The arguments.</param>
    public void LogError(LoggingLevel loggingLevel, string errorCode, string message, Exception exception, params object[] args)
    {
      // Only build the message string if logging of the specified error level is enabled
      if ((loggingLevel == LoggingLevel.Error && logger.IsErrorEnabled) ||
          (loggingLevel == LoggingLevel.Fatal && logger.IsFatalEnabled))
      {
        IndentWriter completeErrorMessage = new IndentWriter();

        // Creating the error message using a function
        completeErrorMessage.WriteLine("");
        completeErrorMessage.WriteLines(FormatLoggingStatement(BuildDebuggingInfo(3), message, errorCode, exception, args));

        switch (loggingLevel)
        {
          case LoggingLevel.Error:
            logger.Error(completeErrorMessage.ToString());
            break;

          case LoggingLevel.Fatal:
            logger.Fatal(completeErrorMessage.ToString());
            break;
        }
      }
    }

    /// <summary>
    /// Log entry into a method, along with its parameters
    /// </summary>
    /// <param name="parameters">Array of parameter values to log</param>
    public void LogEntry(params object[] parameters)
    {
      // Only build the message string if debug logging is enabled
      if (logger.IsDebugEnabled)
      {
        // Creating the debug message using a function
        DebuggingInfo debuggingInfo = BuildDebuggingInfo(2);

        // Building parameter list, if any
        string extraDataName = "Extra data not matching a parameter was found.  Values";
        System.Collections.ArrayList namedParameters = new System.Collections.ArrayList();
        try
        {
          System.Reflection.ParameterInfo[] paramInfo = debuggingInfo.Method.GetParameters();
          if (paramInfo != null &&
              paramInfo.Length > 0)
          {
            for (int count = 0; count < paramInfo.Length; count++)
            {
              // More data passed than method has parameters
              if (paramInfo.Length < parameters.Length &&
                  count == paramInfo.Length - 1)
              {
                object[] args;

                // Parameter is an array due to the "params" keyword
                if (Attribute.IsDefined(paramInfo[count], typeof(ParamArrayAttribute)))
                {
                  args = new object[parameters.Length - paramInfo.Length + 1];
                  parameters.CopyTo(args, count);
                  namedParameters.Add(new ParamArg(paramInfo[count].Name, args));
                }
                else
                {
                  // Too much data passed for number of parameters
                  namedParameters.Add(new ParamArg(paramInfo[count].Name, parameters[count]));
                  args = new object[parameters.Length - paramInfo.Length];
                  Array.Copy(parameters, count + 1, args, 0, parameters.Length - paramInfo.Length);
                  namedParameters.Add(new ParamArg(extraDataName, args));
                }
              }
              else if (count >= parameters.Length)
              {
                // No data was passed for this parameter
                // Handle when an empty array is passed as the only parameter so that it does not get
                // subsumed as if it was the array of parameters.
                if (count == 0 &&
                    parameters.GetType().ToString() == paramInfo[count].ParameterType.ToString())
                {
                  namedParameters.Add(new ParamArg(paramInfo[count].Name, parameters));
                }
                else
                {
                  namedParameters.Add(new ParamArg(paramInfo[count].Name, "Unknown, no value passed for logging."));
                }
              }
              else
              {
                // Match parameter name with the data
                namedParameters.Add(new ParamArg(paramInfo[count].Name, parameters[count]));
              }
            }
          }
          else if (parameters != null && parameters.Length > 0)
          {
            // No method parameters are defined, but data was passed for logging anyway
            namedParameters.Add(new ParamArg(extraDataName, parameters));
          }
        }
        catch
        {
          throw;
        }

        String formattedDebugMessage =
          FormatLoggingStatement(debuggingInfo, "Executing " + debuggingInfo.FullMethodName, string.Empty,
                                 (object[])namedParameters.ToArray(typeof(object)));
        logger.Debug(formattedDebugMessage);
      }
    }

    /// <summary>
    /// Log return from a method and any returned value
    /// </summary>
    /// <param name="result">The result.</param>
    public void LogReturn(params object[] result)
    {
      // Only build the message string if debug logging is enabled
      if (logger.IsDebugEnabled)
      {
        // Creating the debug message using a function
        DebuggingInfo methodInfo = BuildDebuggingInfo(2);
        String formattedDebugMessage =
          FormatLoggingStatement(methodInfo, "Returning from " + methodInfo.FullMethodName, string.Empty, result);
        logger.Debug(formattedDebugMessage);
      }
    }

    /// <summary>
    /// Logs the SQL parameters.
    /// </summary>
    /// <param name="parameters">The SQL, MYSQL query parameters.</param>
    public void LogSQLParameters(System.Data.IDataParameterCollection parameters)
    {
      // Only build the message string if debug logging is enabled
      if (logger.IsDebugEnabled)
      {
        System.Collections.ArrayList namedParameters = new System.Collections.ArrayList();
        if (parameters != null &&
            parameters.Count > 0)
        {
          for (int index = 0; index < parameters.Count; index++)
          {
            namedParameters.Add(new ParamArg(((System.Data.IDataParameter)parameters[index]).ParameterName,
                                             ((System.Data.IDataParameter)parameters[index]).Value));
          }
        }

        string parametersMessage =
          FormatLoggingStatement(BuildDebuggingInfo(2),
                                 "SQL query parameters:", string.Empty,
                                 (object[])namedParameters.ToArray(typeof(object)));
        logger.Debug(parametersMessage);
      }
    }

    #endregion Public Methods

    #region Private Methods

    private string FormatLoggingStatementSimple(string message)
    {
      IndentWriter statement = new IndentWriter();
      statement.WriteLine(message == null ? "NULL" : message);
      return statement.ToString();
    }

    /// <summary>
    /// Formats the message as they need to be displayed then based on the function that uses
    /// this will log it as an error or a debug statement
    /// </summary>
    /// <param name="debuggingInfo">Debugging information obtained through reflection</param>
    /// <param name="message">Message to log</param>
    /// <param name="messageData">Argument data to dump into the message</param>
    /// <returns>Formatted message to return</returns>
    private string FormatLoggingStatement(DebuggingInfo debuggingInfo, string message, string errorCode, params object[] messageData)
    {
      IndentWriter statement = new IndentWriter();

      // Beginning to build complete debug message
      statement.WriteLine("");  // Add a line break so the file name goes on a new line.
      statement.IndentBy(2);
      statement.WriteLine("File Name  : " + debuggingInfo.FileName);
      statement.WriteLine("Line Number: " + debuggingInfo.LineNumber);
      statement.WriteLine("Class Name : " + debuggingInfo.ClassName);
      statement.WriteLine("Method     : " + debuggingInfo.MethodName);
      if (!string.IsNullOrEmpty(errorCode))
      {
        statement.WriteLine("Error Code : " + errorCode);
      }
      statement.WriteLine("Message    : " + (message == null ? "NULL" : message));

      // Building parameter list if any
      if (messageData != null &&
          messageData.Length > 0)
      {
        // Keep track of where the actual arguments start.
        int argsStart = 0;

        // Handle an exception if it was passed as an argument to the logging call.
        if (messageData[argsStart] != null &&
            messageData[argsStart] is Exception)
        {
          Exception exception = messageData[argsStart] as Exception;
          argsStart = 1;
          statement.WriteLines("Exception  : " + exception.Message);
          statement.Indent();
          statement.WriteLines((exception.StackTrace == null) ? "null" : exception.StackTrace.ToString());
          statement.UnIndent();
          statement.WriteLine("Inner Exception :" + ((exception.InnerException != null) ? "" : " null"));
          if (exception.InnerException != null)
          {
            statement.Indent();
            statement.WriteLines(exception.InnerException.ToString());
            statement.UnIndent();
          }
        }

        // Serialize any method parameters or other objects passed for logging.
        if (messageData.Length > argsStart)
        {
          statement.WriteLine("Data       : ");
          statement.Indent();
          for (int count = argsStart; count < messageData.Length; count++)
          {
            String serializedParameter = "";
            if (messageData[count] != null &&
                messageData[count].GetType() == typeof(ParamArg))
            {
              ParamArg param = (ParamArg)messageData[count];
              serializedParameter = LogSerializer.SerializeParameter(param.Name, param.Value);
            }
            else
            {
              serializedParameter = LogSerializer.Serialize(messageData[count]);
            }

            if (count < messageData.Length - 1)
            {
              serializedParameter += ", ";
            }

            statement.WriteLines(serializedParameter);
          }
        }
      }

      return statement.ToString();
    }

    /// <summary>
    /// Extract class, method, file and line number information using reflection.
    /// </summary>
    /// <param name="frameNumber">Stack trace frame number from which to read the debugging information</param>
    /// <returns>Debug information</returns>
    private DebuggingInfo BuildDebuggingInfo(int frameNumber)
    {
      DebuggingInfo info = new DebuggingInfo();
      try
      {
        info.Trace = new StackTrace(true);
        info.Frame = info.Trace.GetFrame(frameNumber);  // The frame number varies based on where the request is made
        info.Method = info.Frame.GetMethod();
        info.ClassName = info.Method.DeclaringType != null ? info.Method.DeclaringType.ToString() : "NULL";
        info.MethodName = info.Method != null ? info.Method.Name : "NULL";

        // Handle the constructor method specially so that the method name is more descriptive
        if (info.Method.DeclaringType != null)
        {
          if (info.MethodName == ".cctor")
          {
            info.MethodName = info.Method.DeclaringType.Name;
          }

          info.FullMethodName = info.Method.DeclaringType.Name + "." + info.MethodName + "()";
        }
        else
        {
          info.FullMethodName = info.ClassName + "." + info.MethodName + "()";
        }

        info.FileName = info.Frame.GetFileName() != null ? info.Frame.GetFileName().ToString() : "NULL";
        info.LineNumber = info.Frame.GetFileLineNumber().ToString();
      }
      catch
      {
      }

      return info;
    }

    #endregion Private Methods

    #region structure

    /// <summary>
    /// Structure used to encapsulate various debugging information
    /// </summary>
    internal struct DebuggingInfo
    {
      /// <summary>
      /// The trace
      /// </summary>
      internal StackTrace Trace;

      /// <summary>
      /// The frame
      /// </summary>
      internal StackFrame Frame;

      /// <summary>
      /// The method
      /// </summary>
      internal System.Reflection.MethodBase Method;

      /// <summary>
      /// The class name
      /// </summary>
      internal string ClassName;

      /// <summary>
      /// The method name
      /// </summary>
      internal string MethodName;

      /// <summary>
      /// The full method name
      /// </summary>
      internal string FullMethodName;

      /// <summary>
      /// The file name
      /// </summary>
      internal string FileName;

      /// <summary>
      /// The line number
      /// </summary>
      internal string LineNumber;
    }

    /// <summary>
    /// Structure used to allow specification of parameter/argument names for logging
    /// </summary>
    internal struct ParamArg
    {
      /// <summary>
      /// The name
      /// </summary>
      internal string Name;

      /// <summary>
      /// The value
      /// </summary>
      internal object Value;

      /// <summary>
      /// Initializes a new instance of the <see cref="ParamArg"/> struct.
      /// </summary>
      /// <param name="name">The name.</param>
      /// <param name="value">The value.</param>
      internal ParamArg(string name, object value)
      {
        Name = name;
        Value = value;
      }
    }

    #endregion structure
  }
}
