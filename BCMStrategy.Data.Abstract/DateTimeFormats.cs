using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract
{
  public static class DateTimeFormats
  {
    public static List<string> find_date(string htmlContent)
    {
      RegexOptions options = RegexOptions.Multiline;
      List<string> dateList = new List<string>();

      ////string regex = @"(?:(?:(?:(?:(?:[1-9]\d)(?:0[48]|[2468][048]|[13579][26])|(?:(?:[2468][048]|[13579][26])00))(\/|-|\.)(?:0?2\1(?:29)))|(?:(?:[1-9]\d{3})(\/|-|\.)(?:(?:(?:0?[13578]|1[02])\2(?:31))|(?:(?:0?[13-9]|1[0-2])\2(?:29|30))|(?:(?:0?[1-9])|(?:1[0-2]))\2(?:0?[1-9]|1\d|2[0-8])))))";
      /*Examples YYYY-MM-dd*/
      string regex = @"(?:19|20)[0-9]{2}(\/|-|\.)(?:(?:0[1-9]|1[0-2])(\/|-|\.)(?:0[1-9]|1[0-9]|2[0-9])|(?:(?!02)(?:0[1-9]|1[0-2])(\/|-|\.)(?:30))|(?:(?:0[13578]|1[02])-31))";
      foreach (Match m in Regex.Matches(htmlContent, regex, options))
      {
        dateList.Add(m.Value);
      }

      ////string regex1 = @"([0-9]?[0-9])[\.\-\/ ]+([0-1]?[0-9])[\.\-\/ ]+([0-9]{2,4})";
      /*Examples dd-MM-yyyy*/
      ////string regex1 = @"\s*(3[01]|[12][0-9]|0?[1-9])(\/|-|\.)(1[012]|0?[1-9])(\/|-|\.)((?:19|20)\d{2})\s*";
      string regex1 = @"\s*(3[01]|[12][0-9]|0?[1-9])(\/|-|\.)(1[012]|0?[1-9])(\/|-|\.)(((?:19|20))?\d{2})\s*";

      foreach (Match m in Regex.Matches(htmlContent, regex1, options))
      {
        dateList.Add(m.Value);
      }

      /*Examples MM-dd-yyyy*/
      ////string regex5 = @"\s*(1[012]|0?[1-9])(\/|-|\.)(3[01]|[12][0-9]|0?[1-9])(\/|-|\.)((?:19|20)\d{2})\s*";
      string regex5 = @"\s*(1[012]|0?[1-9])(\/|-|\.)(3[01]|[12][0-9]|0?[1-9])(\/|-|\.)(((?:19|20))?\d{2})\s*";
      foreach (Match m in Regex.Matches(htmlContent, regex5, options))
      {
        dateList.Add(m.Value);
      }

      ////string regex2 = @"(?:(?:"+ string.Join("|", day_names) + "|"+ string.Join("|", short_day_names) + @")[ ,\-_\/]*)?([0-9]?[0-9])[ ,\-_\/]*(?:"+ string.Join("|", ordinal_number) + @")?[ ,\-_\/]*("+ string.Join("|", month_names) + "|"+ string.Join("|", short_month_names) + @")[ ,\-_\/]+([0-9]{4})";
      string regex2 = @"(?:(((Jan(uary)?|Ma(r(ch)?|y)|Jul(y)?|Aug(ust)?|Oct(ober)?|Dec(ember)?)\ 31(st)?)|((Jan(uary)?|Ma(r(ch)?|y)|Apr(il)?|Ju((ly?)|(ne?))|Aug(ust)?|Oct(ober)?|(Sept|Nov|Dec)(ember)?)\ (0?[1-9]|([12]\d)|30))(st|nd|rd|th)?|(Feb(ruary)?\ (0?[1-9]|1\d|2[0-8]|(29(th)?(?=,\ ((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)))))(st|nd|rd|th)?))\,[\ ]((1[6-9]|[2-9]\d)\d{2}))";
      foreach (Match m in Regex.Matches(htmlContent, regex2, options))
      {
        dateList.Add(m.Value);
      }

      ////string regex3 = @"(" + string.Join("|", month_names) + "|" + string.Join("|", short_month_names) + @")[ ,\-_\/]*([0-9]?[0-9])[ ,\-_\/]*(?:" + string.Join("|", ordinal_number) + @")?[ ,\-_\/]+([0-9]{4})";
      string regex3 = @"(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[a-z]*[ ,./-]*\d{1,2}[ ,./-]*\d{4}";
      foreach (Match m in Regex.Matches(htmlContent, regex3, options))
      {
        dateList.Add(m.Value);
      }

      string regex4 = @"([012]?\d|3[01])[/\-\s.]([Jj][Aa][Nn]([Uu][Aa][Rr][Yy])?|[Ff][Ee][Bb]([Rr][Uu][Aa][Rr][Yy])?|[Mm][Aa][Rr]([Cc][Hh])?|[Aa][Pp][Rr]([Ii][Ll])?|[Mm][Aa][Yy]?|[Jj][Uu][Nn]([Ee])?|[Jj][Uu][Ll]([Yy])?|[aA][Uu][gG]([Uu][Ss][Tt])?|[Ss][eE][pP]([Tt][Ee][Mm][Bb][Ee][Rr])?|[Oo][Cc][Tt]([Oo][Bb][Ee][Rr])?|[Nn][oO][Vv]([Ee][Mm][Bb][Ee][Rr])?|[Dd][Ee][Cc]([Ee][Mm][Bb][Ee][Rr])?)[/\-\s.](19|20)\d\d";

      foreach (Match m in Regex.Matches(htmlContent, regex4, options))
      {
        dateList.Add(m.Value);
      }

      string regex6 = @"\d{4}[ ,./-]([Jj][Aa][Nn]([Uu][Aa][Rr][Yy])?|[Ff][Ee][Bb]([Rr][Uu][Aa][Rr][Yy])?|[Mm][Aa][Rr]([Cc][Hh])?|[Aa][Pp][Rr]([Ii][Ll])?|[Mm][Aa][Yy]?|[Jj][Uu][Nn]([Ee])?|[Jj][Uu][Ll]([Yy])?|[aA][Uu][gG]([Uu][Ss][Tt])?|[Ss][eE][pP]([Tt][Ee][Mm][Bb][Ee][Rr])?|[Oo][Cc][Tt]([Oo][Bb][Ee][Rr])?|[Nn][oO][Vv]([Ee][Mm][Bb][Ee][Rr])?|[Dd][Ee][Cc]([Ee][Mm][Bb][Ee][Rr])?)[ ,./-](0[1-9]|[1-2][0-9]|3[01])";
      foreach (Match m in Regex.Matches(htmlContent, regex6, options))
      {
        dateList.Add(m.Value);
      }

      string regex7 = @"(((0[13-9]|1[012])[-/]?(0[1-9]|[12][0-9]|30)|(0[13578]|1[02])[-/]?31|02[-/]?(0[1-9]|1[0-9]|2[0-8]))[-/]?[0-9]{4}|02[-/]?29[-/]?([0-9]{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0[0-9]|1[0-6])00))";
      foreach (Match m in Regex.Matches(htmlContent, regex7, options))
      {
        dateList.Add(m.Value);
      }

      string regex8 = @"(((0[1-9]|[12][0-9]|30)[-/]?(0[13-9]|1[012])|31[-/]?(0[13578]|1[02])|(0[1-9]|1[0-9]|2[0-8])[-/]?02)[-/]?[0-9]{4}|29[-/]?02[-/]?([0-9]{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0[0-9]|1[0-6])00))";
      foreach (Match m in Regex.Matches(htmlContent, regex8, options))
      {
        dateList.Add(m.Value);
      }

      string regex9 = @"([0-9]{4}[-/]?((0[13-9]|1[012])[-/]?(0[1-9]|[12][0-9]|30)|(0[13578]|1[02])[-/]?31|02[-/]?(0[1-9]|1[0-9]|2[0-8]))|([0-9]{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0[0-9]|1[0-6])00)[-/]?02[-/]?29)";
      foreach (Match m in Regex.Matches(htmlContent, regex9, options))
      {
        dateList.Add(m.Value);
      }

      string regex10 = @"([12]\d{3},\s*([Jj][Aa][Nn]([Uu][Aa][Rr][Yy])?|[Ff][Ee][Bb]([Rr][Uu][Aa][Rr][Yy])?|[Mm][Aa][Rr]([Cc][Hh])?|[Aa][Pp][Rr]([Ii][Ll])?|[Mm][Aa][Yy]?|[Jj][Uu][Nn]([Ee])?|[Jj][Uu][Ll]([Yy])?|[aA][Uu][gG]([Uu][Ss][Tt])?|[Ss][eE][pP]([Tt][Ee][Mm][Bb][Ee][Rr])?|[Oo][Cc][Tt]([Oo][Bb][Ee][Rr])?|[Nn][oO][Vv]([Ee][Mm][Bb][Ee][Rr])?|[Dd][Ee][Cc]([Ee][Mm][Bb][Ee][Rr])?)\s*(0[1-9]|[12]\d|3[01]))";
      foreach (Match m in Regex.Matches(htmlContent, regex10, options))
      {
        dateList.Add(m.Value);
      }

      string regex11 = @"((0[1-9]|[12]\d|3[01])\s*([Jj][Aa][Nn]([Uu][Aa][Rr][Yy])?|[Ff][Ee][Bb]([Rr][Uu][Aa][Rr][Yy])?|[Mm][Aa][Rr]([Cc][Hh])?|[Aa][Pp][Rr]([Ii][Ll])?|[Mm][Aa][Yy]?|[Jj][Uu][Nn]([Ee])?|[Jj][Uu][Ll]([Yy])?|[aA][Uu][gG]([Uu][Ss][Tt])?|[Ss][eE][pP]([Tt][Ee][Mm][Bb][Ee][Rr])?|[Oo][Cc][Tt]([Oo][Bb][Ee][Rr])?|[Nn][oO][Vv]([Ee][Mm][Bb][Ee][Rr])?|[Dd][Ee][Cc]([Ee][Mm][Bb][Ee][Rr])?),\s*[12]\d{3})";
      foreach (Match m in Regex.Matches(htmlContent, regex11, options))
      {
        dateList.Add(m.Value);
      }

      string regex12 = @"((0[1-9]|[12]\d|3[01])([Jj][Aa][Nn]([Uu][Aa][Rr][Yy])?|[Ff][Ee][Bb]([Rr][Uu][Aa][Rr][Yy])?|[Mm][Aa][Rr]([Cc][Hh])?|[Aa][Pp][Rr]([Ii][Ll])?|[Mm][Aa][Yy]?|[Jj][Uu][Nn]([Ee])?|[Jj][Uu][Ll]([Yy])?|[aA][Uu][gG]([Uu][Ss][Tt])?|[Ss][eE][pP]([Tt][Ee][Mm][Bb][Ee][Rr])?|[Oo][Cc][Tt]([Oo][Bb][Ee][Rr])?|[Nn][oO][Vv]([Ee][Mm][Bb][Ee][Rr])?|[Dd][Ee][Cc]([Ee][Mm][Bb][Ee][Rr])?)[12]\d{3})";
      foreach (Match m in Regex.Matches(htmlContent, regex12, options))
      {
        dateList.Add(m.Value);
      }

      string regex13 = @"([12]\d{3}([Jj][Aa][Nn]([Uu][Aa][Rr][Yy])?|[Ff][Ee][Bb]([Rr][Uu][Aa][Rr][Yy])?|[Mm][Aa][Rr]([Cc][Hh])?|[Aa][Pp][Rr]([Ii][Ll])?|[Mm][Aa][Yy]?|[Jj][Uu][Nn]([Ee])?|[Jj][Uu][Ll]([Yy])?|[aA][Uu][gG]([Uu][Ss][Tt])?|[Ss][eE][pP]([Tt][Ee][Mm][Bb][Ee][Rr])?|[Oo][Cc][Tt]([Oo][Bb][Ee][Rr])?|[Nn][oO][Vv]([Ee][Mm][Bb][Ee][Rr])?|[Dd][Ee][Cc]([Ee][Mm][Bb][Ee][Rr])?)(0[1-9]|[12]\d|3[01]))";
      foreach (Match m in Regex.Matches(htmlContent, regex13, options))
      {
        dateList.Add(m.Value);
      }

      return dateList;
    }
  }
}
