using CommonHelper.Attrubute;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommonHelper
{
    public static class SqlHelper
    {
        private static readonly string conStr = ConfigurationManager.AppSettings["connStr"];

        /// <summary>
        /// 执行增删改的
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql, CommandType cmdType = CommandType.Text, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = cmdType;
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 封装一个执行返回单个值的方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string sql, CommandType cmdType = CommandType.Text, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = cmdType;
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    con.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// 返回SqlDataReader对象的方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string sql, CommandType cmdType = CommandType.Text, params SqlParameter[] pms)
        {
            SqlConnection con = new SqlConnection(conStr);
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.CommandType = cmdType;
                if (pms != null)
                {
                    cmd.Parameters.AddRange(pms);
                }
                try
                {
                    con.Open();
                    //这里第二个参数代表在函数外部，如果掉了close()方法，则con会随之一起销毁
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception)
                {
                    con.Close();
                    con.Dispose();
                    throw;
                }
            }
        }


        /// <summary>
        /// //执行操作SQL语句,返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public static DataTable ExecuteToDataTable(string sql, CommandType cmdType = CommandType.Text, params SqlParameter[] pms)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conStr))
            {
                adapter.SelectCommand.CommandType = cmdType;
                if (pms != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(pms);
                }
                adapter.Fill(dt);
            }

            return dt;
        }

        /// <summary>
        /// 执行操作SQL语句,返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        public static DataSet ExecuteToDataSet(string sql, CommandType cmdType = CommandType.Text, params IDataParameter[] pms)
        {
            DataSet ds = new DataSet();
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conStr))
            {
                adapter.SelectCommand.CommandType = cmdType;
                if (pms != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(pms);
                }
                adapter.Fill(ds);
            }
            return ds;
        }

        /// <summary>
        /// DataTable 转换为List 集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static IList<T> DataTableToList<T>(DataTable dt) where T : class, new()
        {
            //创建一个属性的列表
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取T的类型实例  反射的入口
            Type t = typeof(T);
            //获得T 的所有的Public 属性 并找出T属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表 
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合
            List<T> oblist = new List<T>();

            foreach (DataRow row in dt.Rows)
            {
                //创建T的实例
                T ob = new T();
                //找到对应的数据  并赋值
                prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                //放入到返回的集合中.
                oblist.Add(ob);
            }
            return oblist;
        }

        /// <summary>
        /// List集合 转换为一个DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(IEnumerable<T> value) where T : class
        {
            //创建属性的集合
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //获得反射的入口
            Type type = typeof(T);
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列
            Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name); });
            foreach (var item in value)
            {
                //创建一个DataRow实例
                DataRow row = dt.NewRow();
                //给row 赋值
                pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                //加入到DataTable
                dt.Rows.Add(row);
            }
            return dt;
        }

        //IDataReder转实体类列表
        public static List<T> ReaderToList<T>(IDataReader DataReader)
        {
            using (DataReader)
            {
                List<string> field = new List<string>(DataReader.FieldCount);
                for (int i = 0; i < DataReader.FieldCount; i++)
                {
                    field.Add(DataReader.GetName(i).ToLower());
                }
                List<T> list = new List<T>();
                while (DataReader.Read())
                {
                    T model = Activator.CreateInstance<T>();
                    foreach (PropertyInfo property in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (field.Contains(property.Name.ToLower()))
                        {
                            //if (!IsNullOrDBNull(DataReader[property.Name]))
                            if (DataReader[property.Name] != DBNull.Value)
                            {
                                property.SetValue(model, HackType(DataReader[property.Name], property.PropertyType), null);
                            }
                        }
                    }
                    list.Add(model);
                }
                return list;
            }
        }

        /// <summary>
        /// IDataReder转实体类列表 空字符串和dbnull都返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DataReader"></param>
        /// <returns></returns>
        public static List<T> ReaderToListNull<T>(IDataReader DataReader)
        {
            using (DataReader)
            {
                List<string> field = new List<string>(DataReader.FieldCount);
                for (int i = 0; i < DataReader.FieldCount; i++)
                {
                    field.Add(DataReader.GetName(i).ToLower());
                }
                List<T> list = new List<T>();
                while (DataReader.Read())
                {
                    T model = Activator.CreateInstance<T>();
                    foreach (PropertyInfo property in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (field.Contains(property.Name.ToLower()))
                        {
                            if (!IsNullOrDBNull(DataReader[property.Name]))
                            //if (DataReader[property.Name] != DBNull.Value)
                            {
                                property.SetValue(model, HackType(DataReader[property.Name], property.PropertyType), null);
                            }
                        }
                    }
                    list.Add(model);
                }
                return list;
            }
        }

  

        //这个类对可空类型进行判断转换，要不然会报错
        private static object HackType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;

                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(value, conversionType);
        }

        private static bool IsNullOrDBNull(object obj)
        {
            return ((obj is DBNull) || string.IsNullOrEmpty(obj.ToString())) ? true : false;
        }

        #region 自定义注解的使用

        /// <summary>
        /// 根据反射获取repalce sql语句
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static String GetReplaceIntoSql(Object obj, String tableName)
        {
            StringBuilder filedSb = new StringBuilder();
            StringBuilder valueSb = new StringBuilder();
            Type type = obj.GetType();
            //获取所有公有属性
            PropertyInfo[] info = type.GetProperties();

            bool myFiledName = false;
            foreach (var p in info)
            {
                myFiledName = false;
                //取得属性的特性标签，false表示不获取因为继承而得到的标签
                Object[] attr = p.GetCustomAttributes(false);
                if (attr.Length > 0)
                {
                    //从注解数组中取第一个注解(一个属性可以包含多个注解)
                    SqlAttribute myattr = attr[0] as SqlAttribute;
                    if (myattr.primaryKey == true)
                    {
                        continue;
                    }
                    //如果使用了自定义字段名
                    if (!String.IsNullOrEmpty(myattr.fieldName))
                    {
                        filedSb.Append(myattr.fieldName + ",");
                        myFiledName = true;
                    }
                }
                //如果没用自定义字段名
                if (!myFiledName)
                {
                    filedSb.Append(FiledToLower(p.Name) + ",");
                }
                valueSb.Append("'" + p.GetValue(obj, null) + "',");
            }
            String sql = $"relpace into {tableName}  ({filedSb.ToString()}) values ({valueSb.ToString()})";
            return sql;
        }

        /// <summary>
        /// 驼峰转下划线
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static String FiledToLower(String name)
        {
            StringBuilder result = new StringBuilder();
            if (name != null && name.Length > 0)
            {
                // 将第一个字符处理成小写
                result.Append(name.Substring(0, 1).ToLower());
                // 循环处理其余字符
                for (int i = 1; i < name.Length; i++)
                {
                    String s = name.Substring(i, 1);
                    // 在大写字母前添加下划线
                    if (Convert.ToChar(s) >= 'A' && Convert.ToChar(s) <= 'Z')
                    {
                        result.Append("_");
                    }
                    //其他字符直接转成小写
                    result.Append(s.ToLower());
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 下划线转驼峰
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static String LowerToFiled(String name)
        {
            var arr = name.Split('-');
            StringBuilder sb = new StringBuilder();
            foreach(var x in arr)
            {
                if (x.Length == 0)
                {
                    continue;
                }
                else if(x.Length == 1)
                {
                    sb.Append(x.ToUpper());
                }
                else if(x.Length > 1){
                    sb.Append(x.Substring(0, 1).ToUpper() + x.Substring(1).ToLower());
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// DataTable转实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> DataTableToBean<T>(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            List<T> modelList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T model = Activator.CreateInstance<T>();
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    PropertyInfo propertyInfo = model.GetType().GetProperty(LowerToFiled(dr.Table.Columns[i].ColumnName));
                    if (propertyInfo != null && dr[i] != DBNull.Value)
                        propertyInfo.SetValue(model, dr[i], null);
                }

                modelList.Add(model);
            }
            return modelList;
        }

        #endregion
    }
}
