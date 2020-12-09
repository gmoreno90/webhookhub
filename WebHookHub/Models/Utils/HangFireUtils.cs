using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHookHub.Models.Utils
{
    /// <summary>
    /// HangFire Utils
    /// </summary>
    public static class HangFireUtils
    {
        /// <summary>
        /// SetMaxArgumentToRenderSize
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetMaxArgumentToRenderSize(int value)
        {
            var type = Type.GetType("Hangfire.Dashboard.JobMethodCallRenderer, Hangfire.Core", throwOnError: false);
            if (type == null) return false;

            var field = type.GetField("MaxArgumentToRenderSize", BindingFlags.Static | BindingFlags.NonPublic);
            if (field == null) return false;

            field.SetValue(null, value);
            return true;
        }
    }
}
