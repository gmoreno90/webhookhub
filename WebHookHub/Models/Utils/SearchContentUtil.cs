using System;
using System.Text.RegularExpressions;

namespace WebHookHub.Models.Utils
{
    /// <summary>
    /// SearchContentUtil
    /// </summary>
    public static class SearchContentUtil
    {
        /// <summary>
        /// SearchContent
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string SearchContent(string input, string pattern)
        {
            try
            {
                var resMatch1 = Regex.Match(input, pattern, RegexOptions.CultureInvariant);
                if (resMatch1.Success)
                    if(resMatch1.Groups.Count > 1) 
                        return resMatch1.Groups[1].Value;
                    else
                        return resMatch1.Value;
                return "";
            }
            catch (Exception ex)
            {
                var str = ex.Message;
                return "";
            }

        }
    }
}
