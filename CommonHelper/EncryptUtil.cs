using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace CommonHelper
{
    public class EncryptUtil
    {
        public static string AESEncrypt(string str, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(str)) return null;
                Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

                RijndaelManaged rm = new RijndaelManaged
                {
                    Key = Encoding.UTF8.GetBytes(key),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = rm.CreateEncryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string AESDecrypt(string str, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(str)) return null;
                Byte[] toEncryptArray = Convert.FromBase64String(str);

                RijndaelManaged rm = new RijndaelManaged
                {
                    Key = Encoding.UTF8.GetBytes(key),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = rm.CreateDecryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                return "";
            }

        }
    }

}
