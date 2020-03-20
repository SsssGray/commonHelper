using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonHelper
{
    public class Log
    {
        public static void Info(String str)
        {
            try
            {
                File.AppendAllText("info.log", DateTime.Now.ToString() + ":" + str + "\r\n", Encoding.UTF8);

            } catch (Exception e) { }
        }

        public static void Error(Exception ex)
        {
            try
            {
                File.AppendAllText("error.log", DateTime.Now.ToString() + ":" + ex.Message + ex.StackTrace + "\r\n", Encoding.UTF8);

            }catch(Exception e) { }
        }
    }
}
