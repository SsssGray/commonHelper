using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CommonHelper
{
    public class CustomException : ApplicationException
    {
        public CustomException()
        {
        }

        public CustomException(string message) : base(message)
        {
        }

        public CustomException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CustomException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public static void AddLog(Exception ex)
        {
            File.AppendAllText("error.log",DateTime.Now.ToString()+ ex.Message + ex.StackTrace + "\r\n");
        }
    }
}
