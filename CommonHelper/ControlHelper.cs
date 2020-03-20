using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonHelper
{
    /// <summary>
    /// 双缓冲ListView
    /// </summary>
    public class NFListView : ListView
    {
        public NFListView()
        {
            SetStyle(ControlStyles.DoubleBuffer |
                              ControlStyles.OptimizedDoubleBuffer |
                              ControlStyles.AllPaintingInWmPaint,
                              true);
            UpdateStyles();
        }
    }


    #region 跨线程访问控件
    /// <summary>
    /// 跨线程操作控件
    /// </summary>
    public class ThreadControl
    {
        #region 改变控件的文字/text
        /// <summary>
        /// 改变控件的文字/text
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="text">文字</param>
        public static void ChangeText(Control control, string text)
        {
            if (control.InvokeRequired)
            {
                Action<string> action = x => { control.Text = x.Trim(); };
                //control.BeginInvoke(action, text);
                control.Invoke(action, text);
            }
            else
            {
                control.Text = text.Trim();
            }
        }
        #endregion

        #region 增加文字到textbox
        /// <summary>
        /// 增加文字到textbox
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text">增加的文字</param>
        /// <param name="time">是否在前面加上时间</param>
        public static void AppendText(TextBoxBase control, string text,bool time = false)
        {
            if (control.InvokeRequired)
            {
                Action<string> action = x =>
                {
                    if(time)
                    {
                        control.AppendText(DateTime.Now.ToString()+"  "+ x + "\r\n");
                    }
                    else
                    {
                        control.AppendText(x + "\r\n");
                    }
                    control.SelectionStart = control.Text.Length;
                    control.SelectionLength = 0;
                    control.Focus();
                };
                control.Invoke(action, text);
            }
            else
            {
                if (time)
                {
                    control.AppendText(DateTime.Now.ToString() + "  " + text+"\r\n");
                }
                else
                {
                    control.AppendText(text + "\r\n");
                }
                control.SelectionStart = control.Text.Length;
                control.SelectionLength = 0;
                control.Focus();
            }
        }
        #endregion

        #region 控件是否显示
        /// <summary>
        /// 控件是否显示
        /// </summary>
        /// <param name="control"></param>
        /// <param name="statu"></param>
        public static void ChangeVisable(Control control, bool statu)
        {
            if (control.InvokeRequired)
            {
                Action<bool> action = x => { control.Visible = x; };
                control.Invoke(action, statu);
            }
            else
            {
                control.Visible = statu;
            }
        }
        #endregion

        #region 改变控件Enable状态
        /// <summary>
        /// 改变控件Enable状态
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="statu">true/false</param>
        public static void ChangeEnable(Control control, bool statu)
        {
            if (control.InvokeRequired)
            {
                Action<bool> action = x => { control.Enabled = x; };
                control.Invoke(action, statu);
            }
            else
            {
                control.Enabled = statu;
            }
        }
        #endregion

        #region 读取控件的值
        private delegate string delegateRead(Control control);
        private static string readControlText(Control control)
        {
            return control.Text.Trim();
        }

        /// <summary>
        /// 读取控件的text值
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static string ReadText(Control control)
        {
            if (control.InvokeRequired)
            {
                delegateRead reads = new delegateRead(readControlText);
                object text = control.Invoke(reads, control);
                return (string)text;
            }
            else
            {
                return control.Text;
            }
        }
        #endregion

        /// <summary>
        /// 清空listview的item
        /// </summary>
        /// <param name="listview">所需要清空的listview</param>
        public static void ClearListviewItem(ListView listview)
        {
            if(listview.InvokeRequired)
            {
                Action<ListView> action = x => { x.Items.Clear(); };
                listview.Invoke(action, listview);
            }
            else
            {
                listview.Items.Clear();
            }
        }

        /// <summary>
        /// 改变listview中的信息
        /// </summary>
        /// <param name="listview"></param>
        /// <param name="itemIndex">行号</param>
        /// <param name="subItemIndex">列号</param>
        /// <param name="text"></param>
        public static void ChangeListViewItems(ListView listview, int itemIndex, int subItemIndex, string text)
        {
            if (listview.InvokeRequired)
            {
                Action<int, int, string> action = (x, y, z) => { listview.Items[x].SubItems[y].Text = z; };
                listview.Invoke(action, itemIndex, subItemIndex, text);
            }
            else
            {
                listview.Items[itemIndex].SubItems[subItemIndex].Text = text;
            }
        }


        public static void AddListViewItems(ListView listview,ListViewItem listViewItem)
        {
            if (listview.InvokeRequired)
            {
                Action<ListViewItem> action = (x) => { listview.Items.Add(x); };
                listview.Invoke(action, listViewItem);
            }
            else
            {
                listview.Items.Add(listViewItem);
            }
        }

        public static void AddRangeListViewItem(ListView listview, List<ListViewItem> listViewItemList)
        {
            if (listview.InvokeRequired)
            {
                Action<List<ListViewItem>> action = (x) => { listview.Items.AddRange(x.ToArray()); };
                listview.Invoke(action, listViewItemList);
            }
            else
            {
                listview.Items.AddRange(listViewItemList.ToArray());
            }
        }

        /// <summary>
        /// 获取listview的信息
        /// </summary>
        /// <param name="listview"></param>
        /// <param name="itemIndex"></param>
        /// <param name="subItemIndex"></param>
        /// <returns></returns>
        public static string ReadListViewItems(ListView listview, int itemIndex, int subItemIndex)
        {
            string text = null;
            if (listview.InvokeRequired)
            {
                Action<int, int> action = (x, y) => { text = listview.Items[x].SubItems[y].Text; };
                listview.Invoke(action, itemIndex, subItemIndex);
            }
            else
            {
               text = listview.Items[itemIndex].SubItems[subItemIndex].Text;
            }
            return text;
        }

        /// <summary>
        /// 将listview的信息存储到excel中
        /// </summary>
        /// <param name="view">需要存储的listview名</param>
        /// <param name="outname">输出的名字</param>
        /// <param name="HaveTime">是否需要在最前面加上时间</param>
        /// <param name="action">是否替换excel名字中不允许的字符</param>
        /// <returns>成功返回“成功保存”，失败返回文件名</returns>
        public static string ListViewSaveExcel(ListView view, string outname, bool HaveTime = false, bool action = false)
        {
            if (File.Exists(outname))
            {
                //如果存在则删除
                File.Delete(outname);
            }
            if (HaveTime)
            {
                string time = DateTime.Now.ToString("yyyyMMdd HHmmss ");
                outname = time + outname;
            }
            ListView listview = view;
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];
            Cells cells = worksheet.Cells;
            if (view.InvokeRequired)
            {
                Action<string> an = x =>
                {
                    for (int t = 0; t < listview.Columns.Count; t++)
                    {
                        cells[0, t].PutValue(listview.Columns[t].Text);
                    }
                    for (int i = 0; i < listview.Items.Count; i++)
                    {
                        for (int j = 0; j < listview.Items[i].SubItems.Count; j++)
                        {
                            cells[i + 1, j].PutValue(listview.Items[i].SubItems[j].Text);
                        }
                    }
                };
                view.Invoke(an, "");
            }
            else
            {
                for (int t = 0; t < listview.Columns.Count; t++)
                {
                    cells[0, t].PutValue(listview.Columns[t].Text);
                }
                for (int i = 0; i < listview.Items.Count; i++)
                {
                    for (int j = 0; j < listview.Items[i].SubItems.Count; j++)
                    {
                        cells[i + 1, j].PutValue(listview.Items[i].SubItems[j].Text);
                    }
                }
            }


            //是否替换
            if (action)
            {
                outname = Regex.Replace(outname, "\\|/|:|\\*|\\?|\"|<|>|\\|", "_");
            }

            //保存
            try
            {
                workbook.Save(outname);
                return "成功保存";
            }
            catch
            {
                return (outname);
            }
        }


        public static string DateTableSaveExcel(DataTable dataTable, string outname, bool HaveTime = false, bool action = false)
        {
            if (File.Exists(outname))
            {
                //如果存在则删除
                File.Delete(outname);
            }
            if (HaveTime)
            {
                string time = DateTime.Now.ToString("yyyyMMdd HHmmss ");
                outname = time + outname;
            }
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];
            Cells cells = worksheet.Cells;
         
                for (int t = 0; t < dataTable.Columns.Count; t++)
                {
                    cells[0, t].PutValue(dataTable.Columns[t]);
                }
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        cells[i + 1, j].PutValue(dataTable.Rows[i][j]);
                    }
                }


            //是否替换
            if (action)
            {
                outname = Regex.Replace(outname, "\\|/|:|\\*|\\?|\"|<|>|\\|", "_");
            }

            //保存
            try
            {
                workbook.Save(outname);
                return "成功保存";
            }
            catch
            {
                return (outname);
            }
        }
    }
    #endregion


 
}
