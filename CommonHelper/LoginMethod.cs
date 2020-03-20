using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper
{
    public static class LoginMethod
    {
        private static String htUrl = "http://183.247.235.68:8888";
        //private static String htUrl = "http://localhost:20859";

        public static string Login(string userName, string passWord, string softName)
        {
            string machineCode = MachineCode.GetMachineCodeString();
            string url = htUrl + string.Format("/ClientApi/AccountLogin?userName={0}&passWord={1}&softName={2}&machineCode={3}",
                userName, passWord, softName,machineCode);
            string html = HttpAdd.OnlyGetHtml(url);
            return html;
        }

        public static string kd(string softName, string userName, string passWord)
        {
            string url = htUrl + string.Format("/ClientApi/DedPoint?userName={0}&passWord={1}&softName={2}",
               userName, passWord, softName);
            string html = HttpAdd.OnlyGetHtml(url);
            return html;
        }

        public static string getHeadInfo(string softName, string userName, string type)
        {
            string url = htUrl + string.Format("/ClientApi/GetHeadInfo?userName={0}&type={1}&softName={2}",
              userName, type, softName);
            string html = HttpAdd.OnlyGetHtml(url);
            return html;
        }
    }
}
