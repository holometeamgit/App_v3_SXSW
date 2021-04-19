using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Beem {

    public class TimeSpanString {
        // return how much time passed since date object
        public static string GetTimeSince(DateTime objDateTime) {
            // here we are going to subtract the passed in DateTime from the current time converted to UTC
            TimeSpan ts = DateTime.Now.ToUniversalTime().Subtract(objDateTime);
            int intDays = ts.Days;
            int intHours = ts.Hours;
            int intMinutes = ts.Minutes;
            int intSeconds = ts.Seconds;

            if (intDays > 0)
                return string.Format("{0}d", intDays);

            if (intHours > 0)
                return string.Format("{0}h", intHours);

            if (intMinutes > 0)
                return string.Format("{0}min", intMinutes);

            if (intSeconds > 0)
                return string.Format("{0}s", intSeconds);

            // let's handle future times
            if (intDays < 0)
                return string.Format("in {0}d", Math.Abs(intDays));

            if (intHours < 0)
                return string.Format("in {0}h", Math.Abs(intHours));

            if (intMinutes < 0)
                return string.Format("in {0}min", Math.Abs(intMinutes));

            if (intSeconds < 0)
                return string.Format("in {0}s", Math.Abs(intSeconds));

            return "now";
        }

    }
}
