using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaldHead
{
    class Utilities
    {
        /// <summary>
        /// Put all exceptions (including inner ones) in a string formatted
        /// for a MessageBox.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetAllExceptions(Exception ex)
        {
            string msg = "Exception: " + ex.Message;
            int index = 1;

            Exception inner = ex.InnerException;
            while (inner != null)
            {
                msg += Environment.NewLine + Environment.NewLine + "Inner[" + index.ToString() + "]: " + inner.Message;
                inner = inner.InnerException;
                index++;
            }

            return msg;
        }
    }
}
