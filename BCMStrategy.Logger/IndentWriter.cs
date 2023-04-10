using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Logger
{
  internal class IndentWriter
  {
    /// <summary>
    /// String builder holding the formatted version
    /// </summary>
    private System.Text.StringBuilder _sb = new System.Text.StringBuilder();

    /// <summary>
    /// Keep track of the indentation count
    /// </summary>
    private int _indent = 0;

    private String _indentString = "";

    /// <summary>
    /// Increase the indentation count
    /// </summary>
    internal void Indent()
    {
      _indent++;
      _indentString += "  ";
    }

    /// <summary>
    /// Increase the indentation count by count
    /// </summary>
    /// <param name="count">Number of indentations to increase by</param>
    internal void IndentBy(int count)
    {
      _indent += count;
      if (_indent < 0)
      {
        _indent = 0;
        _indentString += "";
      }
      else
      {
        _indentString = "";
        for (int i = 0; i < _indent; i++)
          _indentString += "  ";
      }
    }

    /// <summary>
    /// Decrease the indentation count
    /// </summary>
    internal void UnIndent()
    {
      if (_indent > 0)
      {
        _indent--;
        _indentString = _indentString.Substring(0, Math.Max(0, _indentString.Length - 2));
      }
      else
      {
        _indent = 0;
        _indentString = "";
      }
    }

    /// <summary>
    /// Add indented line to the final string
    /// </summary>
    /// <param name="line"></param>
    internal void WriteLine(string line)
    {
      _sb.AppendLine(_indentString + line);
    }

    /// <summary>
    /// Take a single string consisting of several lines, indent each one and add it to the final string
    /// </summary>
    /// <param name="lines">Single string concatenation of several lines</param>
    internal void WriteLines(string lines)
    {
      String[] splitLines = lines.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (String line in splitLines)
      {
        _sb.AppendLine(_indentString + line);
      }
    }

    /// <summary>
    /// Retrieve the final formatted string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return _sb.ToString();
    }

    /// <summary>
    /// Reset the IndentWriter so that it does not continuously append new values and
    /// the indentation does not accumulate.
    /// </summary>
    internal void Reset()
    {
      _sb.Length = 0;
      _indent = 0;
      _indentString = "";
    }
  }
}
