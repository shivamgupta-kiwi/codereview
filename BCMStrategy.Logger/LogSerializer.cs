using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Serialization = System.Web.Script.Serialization;

namespace BCMStrategy.Logger
{
  public class LogSerializer
  {
    /// <summary>
    /// Define a static logger so that it references the Logger instance for LogSerializer
    /// </summary>
    ////private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogSerializer));

    /// <summary>
    /// Converter for user-specified enum types
    /// </summary>
    private static LogEnumConverter enumConverter = new LogEnumConverter();

    /// <summary>
    /// Converter for all built-in and user-specified exception types
    /// </summary>
    private static LogExceptionConverter exceptionConverter = new LogExceptionConverter();

    /// <summary>
    /// Type resolver
    /// </summary>
    private static LogTypeResolver typeResolver = new LogTypeResolver();

    /// <summary>
    /// JSON string formatter
    /// </summary>
    private static JsonFormatter jsonFormatter = new JsonFormatter();

    /// <summary>
    /// JSON serialize used to convert objects into logable strings.
    /// </summary>
    private static Serialization.JavaScriptSerializer jsonSerializer = new Serialization.JavaScriptSerializer(typeResolver);

    /// <summary>
    /// Initializes static members of the <see cref="LogSerializer"/> class.
    /// </summary>
    static LogSerializer()
    {
      jsonSerializer.RegisterConverters(new Serialization.JavaScriptConverter[] { exceptionConverter });
    }

    /// <summary>
    /// Add enumerator types
    /// </summary>
    /// <param name="enumTypes">enum types</param>
    public static void AddEnumTypes(Type[] enumTypes)
    {
      enumConverter.AddEnumTypes(enumTypes);
      typeResolver.ExcludeEnumTypes(enumTypes);
      jsonSerializer.RegisterConverters(
        new System.Web.Script.Serialization.JavaScriptConverter[] { enumConverter });
    }

    /// <summary>
    /// Add enumerator types
    /// </summary>
    /// <param name="exceptionTypes">exception types</param>
    public static void AddExceptionTypes(Type[] exceptionTypes)
    {
      exceptionConverter.AddExceptionTypes(exceptionTypes);
      jsonSerializer.RegisterConverters(new Serialization.JavaScriptConverter[] { exceptionConverter });
    }

    /// <summary>
    /// Register a custom converter in the event that the JavaScriptSerializer cannot handle the data type natively.
    /// </summary>
    /// <remarks>Legacy method signature from .NET 2.0. Deprecated.</remarks>
    /// <param name="converters">string to convert</param>
    public static void RegisterCustomConverters(System.Web.Script.Serialization.JavaScriptConverter[] converters)
    {
      var convertersCollection = new List<Serialization.JavaScriptConverter>();
      foreach (var converter in converters)
      {
        convertersCollection.Add(converter);
      }

      RegisterCustomConverters(convertersCollection);
    }

    /// <summary>
    /// Register a custom converter in the event that the JavaScriptSerializer cannot handle the data type natively.
    /// </summary>
    /// <param name="converters">string to convert</param>
    public static void RegisterCustomConverters(IEnumerable<Serialization.JavaScriptConverter> converters)
    {
      jsonSerializer.RegisterConverters(converters);
    }

    /// <summary>
    /// Serialize a parameter using the JSON format.
    /// </summary>
    /// <param name="name">Name of the parameter</param>
    /// <param name="value">parameter value</param>
    /// <returns>result string</returns>
    public static string SerializeParameter(String name, object value)
    {
      String result = name + " = ";
      result += LogSerializer.Serialize(value);
      return result;
    }

    /// <summary>
    /// Serialize an object into a string using the JSON format.
    /// </summary>
    /// <param name="value">value object</param>
    /// <returns>JSON serialized and formatted equivalent of the specified object</returns>
    public static string Serialize(object value)
    {
      String result = "";
      try
      {
        result = jsonFormatter.Format(jsonSerializer.Serialize(value));
      }
      catch (System.Exception error)
      {
        result = "{ ERROR: Unable to convert value of type [" + value.GetType() + "] due to the following problem:\n" +
          "  " + error.Message + ".\n" +
          "  Please provide a custom converter by extending the System.Web.Script.Serialization.JavaScriptConverter class\n" +
          "  and register it via the LogSerializer.RegisterCustomConverters() method }";
      }

      return result;
    }

    /// <summary>
    /// Internal class used to shorten the logged object types.
    /// </summary>
    internal class LogTypeResolver : System.Web.Script.Serialization.SimpleTypeResolver
    {
      /// <summary>
      /// Define a static logger so that it references the Logger instance for LoggerTypeResolver
      /// </summary>
      ////private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogTypeResolver));

      /// <summary>
      /// The excluded types
      /// </summary>
      protected Dictionary<Type, Type> excludedTypes = new Dictionary<Type, Type>();

      /// <summary>
      /// Enumeration types which are being handled by the LogEnumConverter class instead of the LogTypeResolver class
      /// </summary>
      /// <param name="enumTypes">enum types</param>
      public void ExcludeEnumTypes(Type[] enumTypes)
      {
        if (enumTypes != null)
        {
          foreach (Type type in enumTypes)
          {
            excludedTypes[type] = type;
          }
        }
      }

      /// <summary>
      /// Data type for which to return the type name string
      /// </summary>
      /// <param name="type">Type</param>
      /// <returns>result string</returns>
      public override string ResolveTypeId(Type type)
      {
        String result = null;
        if (!excludedTypes.ContainsKey(type))
        {
          result = type.Name;
        }

        return result;
      }
    }

    /// <summary>
    /// Internal class used to allow logging of the enumeration names instead of the raw numeric values
    /// </summary>
    private class LogEnumConverter : System.Web.Script.Serialization.JavaScriptConverter
    {
      /// <summary>
      /// Define a static logger so that it references the Logger instance for JsonEnumConverter
      /// </summary>
      ////private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(LogEnumConverter));

      /// <summary>
      /// Gets the log.
      /// </summary>
      /// <value>
      /// The log.
      /// </value>
      ////private static log4net.ILog Log
      ////{
      ////  get { return _log; }
      ////}

      /// <summary>
      /// Enumeration types for which to generate value names instead of raw numeric values.
      /// </summary>
      protected Dictionary<Type, Type> supportedTypes = new Dictionary<Type, Type>();

      /// <summary>
      /// List of the supported enumeration types
      /// </summary>
      public override IEnumerable<Type> SupportedTypes
      {
        get { return supportedTypes.Values; }
      }

      /// <summary>
      /// Add the enumerated types for which the value names should be serialized instead of the raw numeric values
      /// </summary>
      /// <param name="enumTypes">enum types</param>
      public void AddEnumTypes(IEnumerable<Type> enumTypes)
      {
        if (enumTypes != null)
        {
          foreach (Type type in enumTypes)
          {
            supportedTypes[type] = type;
          }
        }
      }

      /// <summary>
      /// Convert one of the supported enumeration types to serializable values.
      /// </summary>
      /// <param name="obj">Object to be serialized</param>
      /// <param name="serializer">Serialize. Not used.</param>
      /// <returns>Serialized data</returns>
      public override IDictionary<string, object> Serialize(object obj,
        System.Web.Script.Serialization.JavaScriptSerializer serializer)
      {
        IDictionary<string, object> results = new Dictionary<string, object>();
        if (supportedTypes.ContainsKey(obj.GetType()))
        {
          results[obj.GetType().Name] = System.Enum.GetName(obj.GetType(), obj);
        }

        return results;
      }

      /// <summary>
      /// Desterilized an enumeration value back to the correct enum type object.
      /// Purposely not implemented as it is not needed for logging.
      /// </summary>
      /// <param name="dictionary">An <see cref="T:System.Collections.Generic.IDictionary`2" /> instance of property data stored as name/value pairs.</param>
      /// <param name="type">The type of the resulting object.</param>
      /// <param name="serializer">The <see cref="T:System.Web.Script.Serialization.JavaScriptSerializer" /> instance.</param>
      /// <returns>
      /// The Desterilized object.
      /// </returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override object Deserialize(IDictionary<string, object> dictionary, Type type,
        System.Web.Script.Serialization.JavaScriptSerializer serializer)
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Internal class used to allow logging of exceptions
    /// </summary>
    private class LogExceptionConverter : System.Web.Script.Serialization.JavaScriptConverter
    {
      // Define a static logger so that it references the Logger instance for JsonEnumConverter
      ////private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogExceptionConverter));

      /// <summary>
      /// Enumeration types for which to generate value names instead of raw numeric values.
      /// </summary>
      protected Dictionary<Type, Type> supportedTypes = new Dictionary<Type, Type>();

      /// <summary>
      /// List of the supported enumeration types
      /// </summary>
      public override IEnumerable<Type> SupportedTypes
      {
        get { return supportedTypes.Values; }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="LogExceptionConverter"/> class.
      /// </summary>
      public LogExceptionConverter()
      {
        supportedTypes.Add(typeof(Exception), typeof(Exception));
        supportedTypes.Add(typeof(SystemException), typeof(SystemException));
        supportedTypes.Add(typeof(IndexOutOfRangeException), typeof(IndexOutOfRangeException));
        supportedTypes.Add(typeof(NullReferenceException), typeof(NullReferenceException));
        supportedTypes.Add(typeof(AccessViolationException), typeof(AccessViolationException));
        supportedTypes.Add(typeof(InvalidOperationException), typeof(InvalidOperationException));
        supportedTypes.Add(typeof(ArgumentException), typeof(ArgumentException));
        supportedTypes.Add(typeof(ArgumentNullException), typeof(ArgumentNullException));
        supportedTypes.Add(typeof(ArgumentOutOfRangeException), typeof(ArgumentOutOfRangeException));
        supportedTypes.Add(typeof(ExternalException), typeof(ExternalException));
        supportedTypes.Add(typeof(COMException), typeof(COMException));
        supportedTypes.Add(typeof(SEHException), typeof(SEHException));
      }

      /// <summary>
      /// Adds the exception types.
      /// </summary>
      /// <param name="exceptionTypes">The exception types.</param>
      public void AddExceptionTypes(IEnumerable<Type> exceptionTypes)
      {
        if (exceptionTypes != null)
        {
          foreach (Type type in exceptionTypes)
          {
            supportedTypes[type] = type;
          }
        }
      }

      /// <summary>
      /// Convert one of the supported enumeration types to serializable values.
      /// </summary>
      /// <param name="obj">Object to be serialized</param>
      /// <param name="serializer">Serialize. Not used.</param>
      /// <returns>Serialize string</returns>
      public override IDictionary<string, object> Serialize(object obj,
        System.Web.Script.Serialization.JavaScriptSerializer serializer)
      {
        IDictionary<string, object> results = new Dictionary<string, object>();
        if (supportedTypes.ContainsKey(obj.GetType()))
        {
          Exception error = (Exception)obj;
          results.Add("Message", error.Message);
          results.Add("Source", error.Source);
          results.Add("InnerException", error.InnerException);
          results.Add("StackTrace", error.StackTrace.ToString()
                                      .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
          if (error.Data != null &&
              error.Data.Count > 0)
          {
            results.Add("Data", error.Data);
          }
        }

        return results;
      }

      /// <summary>
      /// De serialize an enumeration value back to the correct exception type object.
      /// Purposely not implemented as it is not needed for logging.
      /// </summary>
      /// <param name="dictionary">An <see cref="T:System.Collections.Generic.IDictionary`2" /> instance of property data stored as name/value pairs.</param>
      /// <param name="type">The type of the resulting object.</param>
      /// <param name="serializer">The <see cref="T:System.Web.Script.Serialization.JavaScriptSerializer" /> instance.</param>
      /// <returns>
      /// The desterilized object.
      /// </returns>
      /// <exception cref="System.NotImplementedException">Not Implemented Exception</exception>
      public override object Deserialize(IDictionary<string, object> dictionary, Type type,
        System.Web.Script.Serialization.JavaScriptSerializer serializer)
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Internal class used to pretty print a serialized, JSON formatted object
    /// </summary>
    private class JsonFormatter
    {
      /// <summary>
      /// Define a static logger so that it references the Logger instance for JsonFormatter
      /// </summary>
      ////private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(JsonFormatter));

      /// <summary>
      /// String walker that iterates over a string one character at a time
      /// </summary>
      private StringWalker _walker;

      /// <summary>
      /// Class that indents each line by the specified amount
      /// </summary>
      private readonly IndentWriter _writer = new IndentWriter();

      /// <summary>
      /// String builder for the current line
      /// </summary>
      private readonly System.Text.StringBuilder _currentLine = new System.Text.StringBuilder();

      /// <summary>
      /// Boolean used to determine when a string is being read since the contents of strings are
      /// formatted differently.
      /// </summary>
      private bool _quoted;

      /// <summary>
      /// Format a JSON string into a pretty-printed style for ease of readability.
      /// </summary>
      /// <param name="json">JSON encoded object string</param>
      /// <returns>Formatted JSON encoded object string</returns>
      public string Format(string json)
      {
        _quoted = false;
        _writer.Reset();
        _walker = new StringWalker(json);
        ResetLine();
        while (MoveNextChar())
        {
          if (!this._quoted &&
              this.IsOpenBracket())
          {
            this.WriteCurrentLine();
            this.AddCharToLine();
            this.WriteCurrentLine();
            _writer.Indent();
          }
          else if (!this._quoted &&
                   this.IsCloseBracket())
          {
            this.WriteCurrentLine();
            _writer.UnIndent();
            this.AddCharToLine();
          }
          else if (!this._quoted &&
                   this.IsComma())
          {
            this.AddCharToLine();
            this.WriteCurrentLine();
          }
          else
          {
            AddCharToLine();
          }
        }

        this.WriteCurrentLine();
        String results = _writer.ToString().Trim();
        return results;
      }

      /// <summary>
      /// Reset the line StringBuilder
      /// </summary>
      private void ResetLine()
      {
        _currentLine.Length = 0;
      }

      /// <summary>
      /// Advance the string walker to the next character, accounting for quoted substrings
      /// </summary>
      /// <returns>true or false</returns>
      private bool MoveNextChar()
      {
        bool success = _walker.MoveNext();
        if (success && this.IsDoubleQuote())
        {
          this._quoted = !_quoted;
        }

        return success;
      }

      /// <summary>
      /// Predicate function to determine if the current character is double-quote
      /// </summary>
      /// <returns>true or false</returns>
      private bool IsDoubleQuote()
      {
        bool result = this._walker.CharAtIndex() == '"';
        return result;
      }

      /// <summary>
      /// Predicate function to determine if the current character is an open bracket or brace
      /// </summary>
      /// <returns>true or false</returns>
      private bool IsOpenBracket()
      {
        bool result = this._walker.CharAtIndex() == '{' || this._walker.CharAtIndex() == '[';
        return result;
      }

      /// <summary>
      /// Predicate function to determine if the current character is an close bracket or brace
      /// </summary>
      /// <returns>true or false</returns>
      private bool IsCloseBracket()
      {
        bool result = this._walker.CharAtIndex() == '}' || this._walker.CharAtIndex() == ']';
        return result;
      }

      /// <summary>
      /// Predicate function to determine if the current character is comma
      /// </summary>
      /// <returns>true or false</returns>
      private bool IsComma()
      {
        bool result = this._walker.CharAtIndex() == ',';
        return result;
      }

      /// <summary>
      /// Add the current character to the output
      /// </summary>
      private void AddCharToLine()
      {
        this._currentLine.Append(_walker.CharAtIndex());
      }

      /// <summary>
      /// Add the current line to the formatted output
      /// </summary>
      private void WriteCurrentLine()
      {
        string line = this._currentLine.ToString().Trim();
        if (line.Length > 0)
        {
          _writer.WriteLine(line);
        }

        this.ResetLine();
      }

      /// <summary>
      /// Internal class used to walk each character in a string
      /// </summary>
      private class StringWalker
      {
        /// <summary>
        /// String being walked
        /// </summary>
        private readonly string _s;

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        internal int Index { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringWalker"/> class.
        /// </summary>
        /// <param name="s">The string.</param>
        internal StringWalker(string s)
        {
          _s = s;
          Index = -1;
        }

        /// <summary>
        /// Move to the next character in the string
        /// </summary>
        /// <returns>True or False</returns>
        internal bool MoveNext()
        {
          bool results = false;
          if (Index < _s.Length - 1)
          {
            Index++;
            results = true;
          }

          return results;
        }

        /// <summary>
        /// Retrieve the current character
        /// </summary>
        /// <returns>Result character</returns>
        internal char CharAtIndex()
        {
          char result = _s[Index];
          return result;
        }
      }
    }
  }
}