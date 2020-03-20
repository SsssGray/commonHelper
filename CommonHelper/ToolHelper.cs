using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Win32;
using System.Net;
using System.Xml;
using System.Security.Cryptography;
using Aspose.Cells;
using System.Data;
using System.Data.OleDb;
using System.Collections.Concurrent;

namespace CommonHelper
{
    /// <summary>
    /// 工具
    /// </summary>
    public static class MyTool
    {
        #region 时间相关
        public static class MyTime
        {
            /// <summary>
            /// 时间戳转化为时间
            /// </summary>
            /// <param name="timeStamp"></param>
            /// <returns></returns>
            public static DateTime ConvertToDateTime(string timeStamp)
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = long.Parse(timeStamp + "0000000");
                TimeSpan toNow = new TimeSpan(lTime);
                DateTime dtResult = dtStart.Add(toNow);
                return dtResult;
            }

            // <summary>
            /// DateTime时间格式转换为Unix时间戳格式
            /// </summary>
            /// <param name=”time”></param>
            /// <returns></returns>
            private static int ConvertDateTimeInt(System.DateTime time)
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                return (int)(time - startTime).TotalSeconds;
            }

            /// <summary>
            /// 获得当前时间的时间戳
            /// </summary>
            /// <returns></returns>
            public static long GetShiJianChuo()
            {
                long epoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                return epoch;
            }

            public static string DateToString(DateTime time)
            {
                return time.ToString("yyyy-MM-dd HH:mm:ss");
            }

            public static string DateToStringShort(DateTime time)
            {
                return time.ToString("yyyy-MM-dd");
            }

        }
        #endregion

        #region  字符转义

        public static class MyStrings
        {
            #region MD5加密
            /// <summary>
            /// MD5加密
            /// </summary>
            /// <param name="str">加密字符</param>
            /// <returns></returns>
            public static string Md5(string str)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] data = Encoding.Default.GetBytes(str);
                byte[] result = md5.ComputeHash(data);
                String ret = "";
                for (int i = 0; i < result.Length; i++)
                    ret += result[i].ToString("x").PadLeft(2, '0');
                return ret;
            }
            #endregion

            /// <summary>
            /// 实现汉字转化为拼音
            /// <remarks>实现的原理就是先将汉字转化成为内码，然后通过内码和拼音的对照来查找</remarks>
            /// </summary>
            public static class ChineseToSpell
            {
                private static Regex MyRegex = new Regex("^[一-龥]$"); //汉字的正则表达式.eg: if(MyRegex.IsMatch(chrstr.ToString()))
                private static int[] pyvalue = new int[]{-20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036,-20032,-20026,
-20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763,-19756,-19751,-19746,-19741,-19739,-19728,
-19725,-19715,-19540,-19531,-19525,-19515,-19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,
-19261,-19249,-19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006,-19003,-18996,
-18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735,-18731,-18722,-18710,-18697,-18696,-18526,
-18518,-18501,-18490,-18478,-18463,-18448,-18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183,
-18181,-18012,-17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752,-17733,-17730,
-17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482,-17468,-17454,-17433,-17427,-17417,-17202,
-17185,-16983,-16970,-16942,-16915,-16733,-16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,
-16452,-16448,-16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216,-16212,-16205,
-16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944,-15933,-15920,-15915,-15903,-15889,-15878,
-15707,-15701,-15681,-15667,-15661,-15659,-15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,
-15408,-15394,-15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150,-15149,-15144,
-15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109,-14941,-14937,-14933,-14930,-14929,-14928,
-14926,-14922,-14921,-14914,-14908,-14902,-14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,
-14663,-14654,-14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345,-14170,-14159,
-14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112,-14109,-14099,-14097,-14094,-14092,-14090,
-14087,-14083,-13917,-13914,-13910,-13907,-13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,
-13611,-13601,-13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343,-13340,-13329,
-13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076,-13068,-13063,-13060,-12888,-12875,-12871,
-12860,-12858,-12852,-12849,-12838,-12831,-12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,
-12320,-12300,-12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798,-11781,-11604,
-11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067,-11055,-11052,-11045,-11041,-11038,-11024,
-11020,-11019,-11018,-11014,-10838,-10832,-10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,
-10329,-10328,-10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254};
                private static string[] pystr = new string[]{"a","ai","an","ang","ao","ba","bai","ban","bang","bao","bei","ben","beng","bi","bian","biao",
"bie","bin","bing","bo","bu","ca","cai","can","cang","cao","ce","ceng","cha","chai","chan","chang","chao","che","chen",
"cheng","chi","chong","chou","chu","chuai","chuan","chuang","chui","chun","chuo","ci","cong","cou","cu","cuan","cui",
"cun","cuo","da","dai","dan","dang","dao","de","deng","di","dian","diao","die","ding","diu","dong","dou","du","duan",
"dui","dun","duo","e","en","er","fa","fan","fang","fei","fen","feng","fo","fou","fu","ga","gai","gan","gang","gao",
"ge","gei","gen","geng","gong","gou","gu","gua","guai","guan","guang","gui","gun","guo","ha","hai","han","hang",
"hao","he","hei","hen","heng","hong","hou","hu","hua","huai","huan","huang","hui","hun","huo","ji","jia","jian",
"jiang","jiao","jie","jin","jing","jiong","jiu","ju","juan","jue","jun","ka","kai","kan","kang","kao","ke","ken",
"keng","kong","kou","ku","kua","kuai","kuan","kuang","kui","kun","kuo","la","lai","lan","lang","lao","le","lei",
"leng","li","lia","lian","liang","liao","lie","lin","ling","liu","long","lou","lu","lv","luan","lue","lun","luo",
"ma","mai","man","mang","mao","me","mei","men","meng","mi","mian","miao","mie","min","ming","miu","mo","mou","mu",
"na","nai","nan","nang","nao","ne","nei","nen","neng","ni","nian","niang","niao","nie","nin","ning","niu","nong",
"nu","nv","nuan","nue","nuo","o","ou","pa","pai","pan","pang","pao","pei","pen","peng","pi","pian","piao","pie",
"pin","ping","po","pu","qi","qia","qian","qiang","qiao","qie","qin","qing","qiong","qiu","qu","quan","que","qun",
"ran","rang","rao","re","ren","reng","ri","rong","rou","ru","ruan","rui","run","ruo","sa","sai","san","sang",
"sao","se","sen","seng","sha","shai","shan","shang","shao","she","shen","sheng","shi","shou","shu","shua",
"shuai","shuan","shuang","shui","shun","shuo","si","song","sou","su","suan","sui","sun","suo","ta","tai",
"tan","tang","tao","te","teng","ti","tian","tiao","tie","ting","tong","tou","tu","tuan","tui","tun","tuo",
"wa","wai","wan","wang","wei","wen","weng","wo","wu","xi","xia","xian","xiang","xiao","xie","xin","xing",
"xiong","xiu","xu","xuan","xue","xun","ya","yan","yang","yao","ye","yi","yin","ying","yo","yong","you",
"yu","yuan","yue","yun","za","zai","zan","zang","zao","ze","zei","zen","zeng","zha","zhai","zhan","zhang",
"zhao","zhe","zhen","zheng","zhi","zhong","zhou","zhu","zhua","zhuai","zhuan","zhuang","zhui","zhun","zhuo",
"zi","zong","zou","zu","zuan","zui","zun","zuo"};
                /// <summary>
                /// 将汉字转化为全拼
                /// </summary>
                /// <param name="strChinese"></param>
                /// <returns></returns>
                public static string ConvertToAllSpell(string strChinese)
                {
                    Regex MyRegex = new Regex("^[一-龥]$");
                    byte[] array = new byte[2];
                    string returnstr = "";
                    int chrasc = 0;
                    int i1 = 0;
                    int i2 = 0;
                    char[] nowchar = strChinese.ToCharArray();
                    for (int j = 0; j < nowchar.Length; j++)
                    {
                        if (MyRegex.IsMatch(nowchar[j].ToString()))
                        {
                            array = System.Text.Encoding.Default.GetBytes(nowchar[j].ToString());
                            i1 = (short)(array[0]);
                            i2 = (short)(array[1]);
                            chrasc = i1 * 256 + i2 - 65536;
                            if (chrasc > 0 && chrasc < 160)
                            {
                                returnstr += nowchar[j];
                            }
                            else
                            {
                                for (int i = (pyvalue.Length - 1); i >= 0; i--)
                                {
                                    if (pyvalue[i] <= chrasc)
                                    {
                                        returnstr += pystr[i];
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            returnstr += nowchar[j].ToString();
                        }
                    }
                    return returnstr;
                }
                /// <summary>
                /// 将汉字转化为拼音首字母（大写）
                /// </summary>
                /// <param name="strChinese"></param>
                /// <returns></returns>
                public static string ConvertToFirstSpell(string strChinese)
                {
                    int len = strChinese.Length;
                    string myStr = "";
                    for (int i = 0; i < len; i++)
                    {
                        myStr += GetFirstSpell(strChinese.Substring(i, 1));
                    }
                    return myStr;
                }
                /// <summary>
                /// 获取第一个汉字的首字母（大写）；
                /// </summary>
                /// <param name="charChinese"></param>
                /// <returns></returns>
                public static string GetFirstSpell(string charChinese)
                {
                    byte[] arrCN = Encoding.Default.GetBytes(charChinese);
                    if (arrCN.Length > 1)
                    {
                        int area = (short)arrCN[0];
                        int pos = (short)arrCN[1];
                        int code = (area << 8) + pos;
                        int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                        for (int i = 0; i < 26; i++)
                        {
                            int max = 55290;
                            if (i != 25) max = areacode[i + 1];
                            if (areacode[i] <= code && code < max)
                            {
                                byte[] bytes = new byte[] { (byte)(65 + i) };
                                return Encoding.Default.GetString(bytes, 0, bytes.Length);
                            }
                        }
                        return "*";
                    }
                    else return charChinese;
                }
                /// <summary>
                /// 获取第一个汉字的拼音
                /// </summary>
                /// <param name="charChinese"></param>
                /// <returns></returns>
                public static string ConvertFirstSpell(string charChinese)
                {
                    Regex MyRegex = new Regex("^[一-龥]$");
                    byte[] array = new byte[2];
                    string returnstr = "";
                    int chrasc = 0;
                    int i1 = 0;
                    int i2 = 0;
                    char[] nowchar = charChinese.ToCharArray();
                    for (int j = 0; j < 1; j++)
                    {
                        if (MyRegex.IsMatch(nowchar[j].ToString()))
                        {
                            array = System.Text.Encoding.Default.GetBytes(nowchar[j].ToString());
                            i1 = (short)(array[0]);
                            i2 = (short)(array[1]);
                            chrasc = i1 * 256 + i2 - 65536;
                            if (chrasc > 0 && chrasc < 160)
                            {
                                returnstr += nowchar[j];
                            }
                            else
                            {
                                for (int i = (pyvalue.Length - 1); i >= 0; i--)
                                {
                                    if (pyvalue[i] <= chrasc)
                                    {
                                        returnstr += pystr[i];
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            returnstr += nowchar[j].ToString();
                        }
                    }
                    return returnstr;
                }
            }

            /// <summary>  
            /// unicode转中文
            /// </summary>  
            /// <param name="str"></param>  
            /// <returns></returns>  
            public static string UnicodeDencode(string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                    return str;
                return Regex.Unescape(str);
            }

            /// <summary>
            /// Unicode转字符串
            /// </summary>
            /// <param name="source">经过Unicode编码的字符串</param>
            /// <returns>正常字符串</returns>
            public static string UnicodeDencode2(string source)
            {
                return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                             source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
            }

            /// <summary>  
            /// 将字符串进行 unicode 编码  
            /// </summary>  
            /// <param name="str"></param>  
            /// <returns></returns>  
            public static string UnicodeEncode(string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                    return str;
                StringBuilder strResult = new StringBuilder();
                if (!string.IsNullOrEmpty(str))
                {
                    for (int i = 0; i < str.Length; i++)
                    {
                        strResult.Append("\\u");
                        strResult.Append(((int)str[i]).ToString("x4"));
                    }
                }
                return strResult.ToString();
            }

            /// <summary>
            /// Base64加密
            /// </summary>
            /// <param name="encodeType">加密采用的编码方式</param>
            /// <param name="text">待加密的明文</param>
            /// <returns></returns>
            public static string Base64Encode(Encoding encodeType, string text)
            {
                string encode = string.Empty;
                byte[] bytes = encodeType.GetBytes(text);
                try
                {
                    encode = Convert.ToBase64String(bytes);
                }
                catch
                {
                    encode = text;
                }
                return encode;
            }

            /// <summary>
            /// Base64解密
            /// </summary>
            /// <param name="encodeType">解密采用的编码方式，注意和加密时采用的方式一致</param>
            /// <param name="str">待解密的密文</param>
            /// <returns>解密后的字符串</returns>
            public static string Base64Decode(Encoding encodeType, string str)
            {
                string decode = string.Empty;
                byte[] bytes = Convert.FromBase64String(str);
                try
                {
                    decode = encodeType.GetString(bytes);
                }
                catch
                {
                    decode = str;
                }
                return decode;
            }
        }
        #endregion



        #region 正则
        /// <summary>
        /// 正则
        /// </summary>
        public static class MyRegex
        {
            /// <summary>
            /// 正则获得单个
            /// </summary>
            /// <param name="text"></param>
            /// <param name="regex"></param>
            /// <param name="group"></param>
            /// <returns></returns>
            public static string StringRegex(string text, string regex, int group = 1)
            {
                try
                {
                    text = Regex.Match(text, regex).Groups[group].Value.Trim();
                    return text;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            /// <summary>
            /// 正则获得多个
            /// </summary>
            /// <param name="text"></param>
            /// <param name="regex"></param>
            /// <param name="group"></param>
            /// <returns></returns>
            public static List<string> StringRegexs(string text, string regex, int group = 1)
            {
                List<string> regexList = new List<string>();
                try
                {
                    MatchCollection matches = Regex.Matches(text, regex);
                    foreach (Match match in matches)
                    {
                        regexList.Add(match.Groups[group].Value.Trim());
                    }
                }
                catch
                {
                    regexList.Add("正则出错");
                }
                return regexList;
            }

            /// <summary>
            /// 正则分割字符
            /// </summary>
            /// <param name="target">目标字符串</param>
            /// <param name="splitStr">分隔符</param>
            /// <returns></returns>
            public static List<string> Split(string target, string splitStr)
            {
                List<string> list = new List<string>();
                Regex reg = new Regex(splitStr);
                MatchCollection matches = reg.Matches(target);
                int index = 0;
                //循环增加
                foreach (Match match in matches)
                {
                    list.Add(target.Substring(index, match.Index - index));
                    index = match.Index + match.Length;
                }

                //最后一个
                if (!string.IsNullOrWhiteSpace(target.Substring(index, target.Length - index)))
                {
                    list.Add(target.Substring(index, target.Length - index));
                }

                return list;
            }
        }
        #endregion

        #region appConfig配置
        public static class MyAppconfig
        {
            /// <summary>
            /// 更改appconfig的值
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public static void ReadAppconfig(string key, string value, string exeName)
            {
                XmlDocument doc = new XmlDocument();
                //获得配置文件的全路径
                string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + exeName + ".exe.config";
                doc.Load(strFileName);
                //找出名称为“add”的所有元素
                XmlNodeList nodes = doc.GetElementsByTagName("add");
                for (int i = 0; i < nodes.Count; i++)
                {
                    //获得将当前元素的key属性
                    XmlAttribute att = nodes[i].Attributes["key"];
                    //根据元素的第一个属性来判断当前的元素是不是目标元素
                    if (att.Value == key)
                    {
                        //对目标元素中的第二个属性赋值
                        att = nodes[i].Attributes["value"];
                        att.Value = value;
                        break;
                    }
                }
                //保存上面的修改
                doc.Save(strFileName);
            }

            /// <summary>
            /// 更改appconfig的值
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public static void ChangeAppconfig(string key, string value)
            {
                var _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (!_config.HasFile)
                {
                    throw new ArgumentException("程序配置文件缺失！");
                }
                KeyValueConfigurationElement _key = _config.AppSettings.Settings[key];
                if (_key == null)
                    _config.AppSettings.Settings.Add(key, value);
                else
                    _config.AppSettings.Settings[key].Value = value;
                _config.Save(ConfigurationSaveMode.Modified);
            }

            /// <summary>
            /// 读取Appconfig的值
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static string ReadAppconfig(string key)
            {
                String str = ConfigurationManager.AppSettings[key];
                return str;
            }
        }
        #endregion

        #region 随机数
        public static class MyRanDom
        {
            public static string getGUID()
            {
                System.Guid guid = new Guid();
                guid = Guid.NewGuid();
                string str = guid.ToString();
                return str;
            }


            /// <summary>
            /// 生成不重复随机数
            /// </summary>
            /// <param name="number">数量</param>
            /// <param name="min">最小值</param>
            /// <param name="max">最大值（不包括）</param>
            public static List<string> GetRandom(int number, int min = 0, int max = 10)
            {
                Random random = new Random();
                string num = null;
                List<string> numList = new List<string>();
                while (numList.Count < number)
                {
                    num = random.Next(min, max).ToString();
                    if (!numList.Contains(num))
                    {
                        numList.Add(num);
                    }
                }
                return numList;
            }

            /// <summary>
            /// 生成不重复随机数
            /// </summary>
            /// <param name="number">数量</param>
            /// <param name="min">最小值</param>
            /// <param name="max">最大值（不包括）</param>
            public static string GetRandomStr(int number, int min = 0, int max = 10)
            {
                Random random = new Random();
                string num = null;
                StringBuilder sb = new StringBuilder();
                List<string> numList = new List<string>();
                while (numList.Count < number)
                {
                    num = random.Next(min, max).ToString();
                    if (!numList.Contains(num))
                    {
                        numList.Add(num);
                        sb.Append(num);
                    }
                }
                Thread.Sleep(30);
                return sb.ToString();
            }
        }
        #endregion

        #region 图片
        public static class MyPic
        {
            /// <summary>
            /// 图片帮助类
            /// </summary>


            /// <summary>
            /// 图片转化为字节流
            /// </summary>
            /// <param name="img"></param>
            /// <returns></returns>
            public static byte[] ImgToByt(Image img)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    if (img != null)
                    {
                        Bitmap t = new Bitmap(img);
                        t.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    return ms.ToArray();
                }
            }
            /// <summary>
            /// 字节流转换成图片
            /// </summary>
            /// <param name="byt">要转换的字节流</param>
            /// <returns>转换得到的Image对象</returns>
            public static Image BytToImg(byte[] byt)
            {
                MemoryStream ms = new MemoryStream(byt);
                Image img = Image.FromStream(ms);
                return img;
            }

            /// <summary>
            /// 根据图片路径返回图片的字节流byte[]
            /// </summary>
            /// <param name="imagePath">图片路径</param>
            /// <returns>返回的字节流</returns>
            public static byte[] getImageByte(string imagePath)
            {
                FileStream files = new FileStream(imagePath, FileMode.Open);
                byte[] imgByte = new byte[files.Length];
                files.Read(imgByte, 0, imgByte.Length);
                files.Close();
                return imgByte;
            }

            /// <summary>
            /// 图片转base64
            /// </summary>
            /// <param name="Imagefilename"></param>
            /// <returns></returns>
            public static string ImgToBase64(string Imagefilename)
            {
                try
                {
                    Bitmap bmp = new Bitmap(Imagefilename);

                    MemoryStream ms = new MemoryStream();
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();
                    return Convert.ToBase64String(arr);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            /// <summary>
            /// base64转图片
            /// </summary>
            /// <param name="strbase64"></param>
            /// <returns></returns>
            public static Bitmap Base64ToImage(string strbase64, bool save = false, string path = "default.png")
            {
                try
                {
                    byte[] arr = Convert.FromBase64String(strbase64);
                    MemoryStream ms = new MemoryStream(arr);
                    Bitmap bmp = new Bitmap(ms);
                    if (save)
                    {
                        bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    ms.Close();
                    return bmp;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            /// <summary>
            /// 改变图片大小
            /// </summary>
            /// <param name="sourceImage"></param>
            /// <param name="targetWidth"></param>
            /// <param name="targetHeight"></param>
            /// <returns></returns>
            public static Image ChangeImageSize(Image sourceImage, int targetWidth, int targetHeight)
            {
                int width;//图片最终的宽  
                int height;//图片最终的高  
                try
                {
                    System.Drawing.Imaging.ImageFormat format = sourceImage.RawFormat;
                    Bitmap targetPicture = new Bitmap(targetWidth, targetHeight);
                    Graphics g = Graphics.FromImage(targetPicture);
                    g.Clear(Color.White);

                    //计算缩放图片的大小  
                    if (sourceImage.Width > targetWidth && sourceImage.Height <= targetHeight)
                    {
                        width = targetWidth;
                        height = (width * sourceImage.Height) / sourceImage.Width;
                    }
                    else if (sourceImage.Width <= targetWidth && sourceImage.Height > targetHeight)
                    {
                        height = targetHeight;
                        width = (height * sourceImage.Width) / sourceImage.Height;
                    }
                    else if (sourceImage.Width <= targetWidth && sourceImage.Height <= targetHeight)
                    {
                        width = sourceImage.Width;
                        height = sourceImage.Height;
                    }
                    else
                    {
                        width = targetWidth;
                        height = (width * sourceImage.Height) / sourceImage.Width;
                        if (height > targetHeight)
                        {
                            height = targetHeight;
                            width = (height * sourceImage.Width) / sourceImage.Height;
                        }
                    }
                    g.DrawImage(sourceImage, (targetWidth - width) / 2, (targetHeight - height) / 2, width, height);
                    sourceImage.Dispose();

                    return targetPicture;
                }
                catch (Exception ex)
                {

                }
                return null;
            }


        }


        #endregion

        #region 注册表操作
        public static void ChangeZhuCeBiao(string keys, string values)
        {
            ///HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION 32位
            ///HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION   64位
            RegistryKey key = Registry.LocalMachine;
            RegistryKey software64 = key.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", true); //该项必须已存在
            software64.SetValue(keys, values);
            //在HKEY_LOCAL_MACHINE\SOFTWARE\test下创建一个名为“test”，值为“博客园”的键值。如果该键值原本已经存在，则会修改替换原来的键值，如果不存在则是创建该键值。
            // 注意：SetValue()还有第三个参数，主要是用于设置键值的类型，如：字符串，二进制，Dword等等~~默认是字符串。如：
            // software.SetValue("test", "0", RegistryValueKind.DWord); //二进制信息
            key.Close();
        }
        #endregion

        #region 进制转换
        public static class MyNumber
        {
            /// <summary>
            /// 16进制转10进制
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static int Data16To10(string data)
            {
                return Convert.ToInt32(data, 16);
            }

            /// <summary>
            /// 10进制转2进制
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static string Data10To2(string data)
            {
                int count = Convert.ToInt32(data);
                return Convert.ToString(count, 2);
            }

            /// <summary>
            /// 2进制转为10进制
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static int Data2To10(string data)
            {
                return Convert.ToInt32(data, 2);
            }
        }
        #endregion

        #region 文件相关
        /// <summary>
        /// 文件相关操作类
        /// </summary>
        public static class MyFile
        {
            #region Excel数据导入Datable
            /// <summary>
            /// Excel数据导入Datable
            /// </summary>
            /// <param name="fileUrl"></param>
            /// <param name="table"></param>
            /// <returns></returns>
            public static System.Data.DataTable GetExcelDatatable(string fileUrl, string table)
            {
                //office2007之前 仅支持.xls
                //const string cmdText = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1';";
                //支持.xls和.xlsx，即包括office2010等版本的   HDR=Yes代表第一行是标题，不是数据；
                const string cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";

                System.Data.DataTable dt = null;
                //建立连接
                OleDbConnection conn = new OleDbConnection(string.Format(cmdText, fileUrl));
                try
                {
                    //打开连接
                    if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }


                    System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    //获取Excel的第一个Sheet名称
                    string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString().Trim();

                    //查询sheet中的数据
                    string strSql = "select * from [" + sheetName + "]";
                    OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, table);
                    dt = ds.Tables[0];

                    return dt;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }

            }

            public static ConcurrentQueue<String> ReadLines(String fileName)
            {
                HashSet<String> set = new HashSet<string>();
                String[] strs = File.ReadAllLines(fileName, Encoding.UTF8);
                ConcurrentQueue<String> strQueue = new ConcurrentQueue<string>();
                foreach (var x in strs)
                {
                    if (String.IsNullOrWhiteSpace(x))
                    {
                        continue;
                    }
                    if (set.Add(x))
                    {
                        strQueue.Enqueue(x);
                    }
                }
                return strQueue;
            }

            #endregion

            #region
            /// <summary>
            /// 输出datatable的信息到excel
            /// </summary>
            /// <param name="dt"></param>
            /// <param name="fileName"></param>
            public static void OutExcelDatatable(DataTable dt, string fileName)
            {
                Workbook workbook = new Workbook();
                Cells cells = workbook.Worksheets[0].Cells;

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    cells[0, j].Value = dt.Columns[j].ColumnName;
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        cells[i + 1, j].Value = dt.Rows[i][j].ToString();
                    }
                }
                workbook.Save(fileName);
            }
            #endregion
        }
        #endregion

        public static class MyLog
        {
            public static void ERROR(Exception ex)
            {
                try
                {
                    File.AppendAllLines("error.log",new List<String> {DateTime.Now.ToString()+":"+ ex.Message + ex.StackTrace });
                }
                catch (Exception e) { }
            }
        }
    }
}

