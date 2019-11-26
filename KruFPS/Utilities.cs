using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KruFPS
{
    static class Utilities
    {
        /// <summary>
        /// Checks if string contains any of the values provided in lookFor.
        /// </summary>
        /// <param name="lookIn">String in which we should look for</param>
        /// <param name="lookFor">Values that we want to look for in lookIn</param>
        /// <returns></returns>
        public static bool ContainsAny(this string lookIn, params string[] lookFor)
        {
            foreach (string sample in lookFor)
                // Value found? Return true.
                if (lookIn.Contains(sample))
                    return true;

            // Nothing has been found? Return false.
            return false;
        }
    }
}
