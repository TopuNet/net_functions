using System;
using System.Data;
//using System.Data.OleDb;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
//using System.Web.Util;
using jmail;
using System.Web.Mail;
using System.IO;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Drawing;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Common
{
	/// <summary>
	/// 方法集。
	/// 1.1.1
	/// https://github.com/TopuNet/net_functions
	/// 需要添加、修改，需向上申请
	/// </summary>
	public class Functions
    {
        //[DllImport("Iphlpapi.dll")]
        //private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        //[DllImport("Ws2_32.dll")]
        //private static extern Int32 inet_addr(string ip);
        private Common.Config cfg = new Config();

        #region ==生成随机码==
        /// <summary>
        /// 生成随机码。返回字符串
        /// </summary>
        /// <param name="n">位数，数字+字母的组合时请键入偶数</param>
        /// <param name="Kind">生成种类。1-纯数字 2-纯大写字母 3-纯小写字母 4-数字+大写字母 5-数字+小写字母 6-大写字母或小写字母 7-数字+纯大写字母或小写字母</param>
        public static string createRandomStr(int n, int Kind)
        {
            Random r = new Random();
            string code = "";
            int a = 0;
            switch (Kind)
            {
                case 1:
                    for (int i = 0; i < n; i++)
                        code += r.Next(1, 10).ToString();
                    break;
                case 2:
                    for (int i = 0; i < n; i++)
                    {
                        code += ((char)r.Next(65, 91)).ToString();
                    }
                    break;
                case 3:
                    for (int i = 0; i < n; i++)
                    {
                        code += ((char)r.Next(97, 123)).ToString();
                    }
                    break;
                case 4:
                    for (int i = 0; i < n / 2; i++)
                    {
                        code += r.Next(1, 10).ToString() + ((char)r.Next(65, 91)).ToString();
                    }
                    break;
                case 5:
                    for (int i = 0; i < n / 2; i++)
                    {
                        code += r.Next(1, 10).ToString() + ((char)r.Next(97, 123)).ToString();
                    }
                    break;
                case 6:
                    for (int i = 0; i < n; i++)
                    {
                        a = r.Next(65, 123);
                        if (a > 90 && a < 97)
                        {
                            i--;
                            continue;
                        }
                        code += ((char)a).ToString();
                    }
                    break;
                case 7:
                    for (int i = 0; i < n / 2; i++)
                    {
                        a = r.Next(65, 123);
                        if (a > 90 && a < 97)
                        {
                            i--;
                            continue;
                        }
                        code += r.Next(1, 10).ToString() + ((char)a).ToString();
                    }
                    break;
            }
            return code;
        }
        #endregion

        #region ==文件下载 bool DownloadFile(HttpResponse response, string serverPath, string encode)==
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="response">HttpResponse response</param>
        /// <param name="serverPath">文件地址（非硬盘路径，不用Server.MapPath）</param>
        /// <param name="encode">文件编码</param>
        public static bool DownloadFile(HttpResponse response, string serverPath, string encode)
        {

            FileStream fs = null;
            try
            {
                HttpContext.Current.Response.Clear();
                fs = File.OpenRead(HttpContext.Current.Server.MapPath(serverPath));
                byte[] buffer = new byte[1024];
                long count = 1024;
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.AddHeader("Connection", "Keep-Alive");
                HttpContext.Current.Response.AddHeader("Accept-Charset", encode);
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(HttpContext.Current.Server.MapPath(serverPath)));//下载时要保存的默认文件名
                HttpContext.Current.Response.AddHeader("Content-Length", fs.Length.ToString());
                while (count == 1024)
                {
                    count = fs.Read(buffer, 0, 1024);
                    HttpContext.Current.Response.BinaryWrite(buffer);
                }
                fs.Close();
                return true;
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Write(e.ToString());
                return false;
            }
        }
        #endregion

        #region ==获得文件代码 string getCode(string path)==
        /// <summary>
        /// 获得文件代码
        /// </summary>
        /// <param name="path">文件地址（非硬盘路径，不用Server.MapPath）</param>
        /// <returns>Server.HtmlEncode(str)</returns>
        public static string getCode(string path)
        {
            string str = "";
            if (File.Exists(HttpContext.Current.Server.MapPath(path)))
            {
                str = File.ReadAllText(HttpContext.Current.Server.MapPath(path), Encoding.Unicode);
            }
            return HttpContext.Current.Server.HtmlEncode(str);
        }
        #endregion

        #region ==删除记录 bool recordDel(string t,string pk_n,string pk_v,int pic_c,string info_n,string column_n,string sortTitle,SqlConnection conn,bool del)==
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="t">表名</param>
        /// <param name="pk_n">主码字段名</param>
        /// <param name="pk_v">主码值</param>
        /// <param name="pic_c">图片数量，没有写0</param>
        /// <param name="info_n">使用编辑器的字段名，多个字段用逗号分隔</param>
        /// <param name="column_n">标题列字段名，主要是为了写入操作记录。如不写则没用，不过还是要写一个字段名</param>
        /// <param name="sortTitle">此分类的分类标题</param>
        /// <param name="conn">conn</param>
        /// <param name="del">true-删除记录；false-不删除记录，将isDel改为True</param>
        /// <returns>true</returns>
        public static bool recordDel(string t, string pk_n, string pk_v, int pic_c, string info_n, string column_n, string sortTitle, SqlConnection conn, bool del)
        {
            string sql;
            int i;
            SqlCommand sc;
            SqlDataReader dr = null;
            sql = "select " + column_n;
            if (info_n != "")
            {
                sql += "," + info_n;
            }
            for (i = 1; i <= pic_c; i++)
            {
                sql += ",Pic" + i;
            }
            sql += " from " + t + " where " + pk_n + " in(" + pk_v + ")";
            sc = new SqlCommand(sql, conn);
            try
            {
                dr = sc.ExecuteReader();
            }
            catch
            {
                HttpContext.Current.Response.Write(sql.ToString());
                HttpContext.Current.Response.End();
            }

            while (dr.Read())
            {
                if (!Para.pics_view)
                {
                    for (i = 1; i <= pic_c; i++)
                    {
                        del_file("/" + Common.Para.siteHead + dr["Pic" + i].ToString().Trim());
                    }
                }
                /*
                if (info_n != "")
                {
                    string[] a = Regex.Split(info_n, ",");
                    for (i = 0; i < a.Length; i++)
                        del_url(dr[a[i]].ToString());
                }
                */
            }
            dr.Close();
            dr.Dispose();
            sc.Dispose();
            if (del)
                sql = "delete from " + t + " where " + pk_n + " in(" + pk_v + ")";
            else
                sql = "update " + t + " set isDel=1 where " + pk_n + " in(" + pk_v + ")";
            sc = new SqlCommand(sql, conn);
            sc.ExecuteNonQuery();
            sc.Dispose();
            sc = null;

            return true;
        }
        #endregion

        #region ==自动传递地址栏参数 string transParameters(string Para) Para代表不传递的参数，用 || 分隔==
        /// <summary>
        /// 自动传递地址栏参数。地址栏：?id=1＆n=abc＆page=5＆s=def --> func.transParameters("n||s")的返回值：id=1＆page=5
        /// </summary>
        /// <param name="Para">不传递的参数名，多个用||分隔，区分大小写</param>
        public static string transParameters(string Para)
        {
            string[] addrPara;	//地址栏全部参数
            string exPara;	//传递过来的参数
            string str;
            int i;

            #region  ==获得地址栏全部参数==
            addrPara = HttpContext.Current.Request.QueryString.AllKeys;
            string[] v = new string[addrPara.Length];
            for (i = 0; i < addrPara.Length; i++)
            {
                v[i] = convers(HttpContext.Current.Request.QueryString[i]);
            }
            #endregion

            #region ==获得传递过来的参数==
            exPara = "<" + Para.Replace("||", "><") + ">";
            #endregion

            #region ==获得最终地址栏参数==
            str = "";
            for (i = 0; i < addrPara.Length; i++)
            {
                if (addrPara[i] == null || addrPara[i].ToString().ToLower() == "__viewstate")
                {
                    continue;
                }
                if (exPara.IndexOf("<" + addrPara[i].ToString() + ">") == -1)
                {
                    if (str != "")
                    {
                        str += "&";
                    }
                    str += addrPara[i].ToString() + "=" + v[i];
                }
            }
            #endregion

            return str;
        }
        #endregion

        #region ==省略字符串 string shenglue(string text,int length,bool more)==
        /// <summary>
        /// 省略字符串
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <param name="length">字节长度（一个汉字=2个字节）</param>
        /// <param name="more">是否显示省略号...</param>
        public static string shenglue(string text, int length, bool more)
        {
            int num;
            int i;
            string str = "";
            byte[] chars = Encoding.ASCII.GetBytes(text);
            num = 0;
            for (i = 0; i < chars.Length; i++)
            {
                if ((int)chars[i] == 63)
                {
                    num += 2;
                }
                else
                {
                    num += 1;
                }
                if (num == length && i < chars.Length - 1)
                {
                    str = text.Substring(0, i + 1);
                    if (more)
                    {
                        str += "...";
                    }
                    break;
                }
                else if (num > length)
                {
                    str = text.Substring(0, i);
                    if (more)
                    {
                        str += "...";
                    }
                    break;
                }
            }
            if (str == "")
            {
                str = text;
            }
            return str;
        }
        #endregion

        #region ==判断字符串长度（占用的字节数，一个汉字占用两个字节） int strLength(string text)==
        /// <summary>
        /// 判断字符串长度（占用的字节数，一个汉字占用两个字节）
        /// </summary>
        /// <param name="text">字符串</param>
        public static int strLength(string text)
        {
            int num = 0;
            int i;
            byte[] chars = Encoding.ASCII.GetBytes(text);
            for (i = 0; i < chars.Length; i++)
            {
                if ((int)chars[i] == 63)
                {
                    num += 2;
                }
                else
                {
                    num += 1;
                }
            }
            return num;
        }
        #endregion

        #region ==检查Email合法性 bool checkEmail(string email)==
        /// <summary>
        /// 正则验证Email的合法性。true为合法
        /// </summary>
        public static bool checkEmail(string email)
        {

            Regex r = new Regex(@"^([\w^_])+([-]?[\w]+)*[\@]([\w^_])+([-][\w]+)*([.][\w]+)+$");
            Match m = r.Match(email);
            if (m.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region ==过滤非法字符 string unchar(string text)==
        /// <summary>
        /// 正则验证非法字符，true为合法
        /// </summary>
        public static bool unchar(string text)
        {
            Regex r = new Regex(@"^([\w^_]+(\w)*[-\w]?)+$");
            Match m = r.Match(text);
            if (m.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region ==判断变量是否为数值型(数组排序要用) bool isNum(string str)==
        /// <summary>
        /// 验证是否为数值型变量，true为是
        /// </summary>
        public static bool isNum(string str)
        {
            try
            {
                int a = Convert.ToInt32(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ==一维数组升序排序 string[] arrayRank(string[] arr,int l,int u)==
        /// <summary>
        /// 一维数组升序排序（最优算法）
        /// </summary>
        /// <param name="arr">原始数组</param>
        /// <param name="l">最小脚码</param>
        /// <param name="u">最大脚码</param>
        public static string[] arrayRank(string[] arr, int l, int u)
        {
            int i = l;
            int j = u;
            int k;
            string t;
            int limit = 12;
            string middle;		//数组中间角标的值
            bool isNumFlag = true;     //数组值是否为数值型

            #region ==判断数组值是否为数字型==
            for (k = l; k <= u; k++)
            {
                if (!isNum(arr[k]))
                {
                    isNumFlag = false;
                    break;
                }
            }
            #endregion

            if (u - l + 1 <= limit)
            {
                #region ==对绝对少的数组进行冒泡排序==
                for (j = l; j <= u; j++)
                {
                    for (i = l + 1; i <= u; i++)
                    {
                        if (isNumFlag)
                        {
                            if (Convert.ToInt64(arr[i]) < Convert.ToInt64(arr[i - 1]))
                            {
                                t = arr[i];
                                arr[i] = arr[i - 1];
                                arr[i - 1] = t;
                            }
                        }
                        else
                        {
                            if (arr[i].CompareTo(arr[i - 1]) < 0)
                            {
                                t = arr[i];
                                arr[i] = arr[i - 1];
                                arr[i - 1] = t;
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region ==对数组进行拆分重组==
                middle = arr[(l + u) / 2];	//中间点的值
                while (true)		//无限循环
                {
                    //==从数组最小角标开始寻找比中间点的值大的点==
                    if (isNumFlag)
                    {
                        while (i < u && Convert.ToInt64(arr[i]) < Convert.ToInt64(middle))
                        {
                            i += 1;
                        }
                    }
                    else
                    {
                        while (i < u && arr[i].CompareTo(middle) < 0)
                        {
                            i += 1;
                        }
                    }

                    //==从数组最大角标开始寻找比中间点的值小的点==
                    if (isNumFlag)
                    {
                        while (j > l && Convert.ToInt64(arr[j]) > Convert.ToInt64(middle))
                        {
                            j -= 1;
                        }
                    }
                    else
                    {
                        while (j > l && arr[j].CompareTo(middle) > 0)
                        {
                            j -= 1;
                        }
                    }

                    //==如果比数组中间点的值大的点不大于比数组中间点的值小的点，交换两个点的值，并从下一个点继续寻找==						
                    if (i <= j)
                    {
                        t = arr[i];
                        arr[i] = arr[j];
                        arr[j] = t;
                        i += 1;
                        j -= 1;
                    }

                    //==如果从小角标开始寻找的点大于从大角标开始寻找的点，退出循环==
                    if (i > j)
                    {
                        break;
                    }
                }

                //==再次排序==
                if (i < u)
                {
                    arrayRank(arr, i, u);
                }
                if (j > l)
                {
                    arrayRank(arr, l, j);
                }
                #endregion
            }

            return arr;
        }
        #endregion

        #region ==二维数组升序排序 string[,] arrayRank2(string[,] arr, int c, int l,int u)==
        /// <summary>
        /// 二维数组升序排序
        /// </summary>
        /// <param name="arr">原始数组</param>
        /// <param name="c">排序依据列 序号</param>
        /// <param name="l">最小脚码</param>
        /// <param name="u">最大脚码</param>
        public static string[,] arrayRank2(string[,] arr, int c, int l, int u)
        {
            int i = l;
            int j = u;
            int k;
            string t;
            int limit = 12;
            string middle;		//数组中间角标的值
            bool isNumFlag = true;     //数组值是否为数值型

            #region ==判断数组值是否为数字型==
            for (k = l; k <= u; k++)
            {
                if (!isNum(arr[k, c]))
                {
                    isNumFlag = false;
                    break;
                }
            }
            #endregion

            if (u - l + 1 <= limit)
            {
                #region ==对绝对少的数组进行冒泡排序==
                for (j = l; j <= u; j++)
                {
                    for (i = l + 1; i <= u; i++)
                    {
                        if (isNumFlag)
                        {
                            if (Convert.ToInt64(arr[i, c]) < Convert.ToInt64(arr[i - 1, c]))
                            {
                                t = arr[i, c];
                                arr[i, c] = arr[i - 1, c];
                                arr[i - 1, c] = t;
                            }
                        }
                        else
                        {
                            if (arr[i, c].CompareTo(arr[i - 1, c]) < 0)
                            {
                                t = arr[i, c];
                                arr[i, c] = arr[i - 1, c];
                                arr[i - 1, c] = t;
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region ==对数组进行拆分重组==
                middle = arr[c, (l + u) / 2];	//中间点的值
                while (true)		//无限循环
                {
                    //==从数组最小角标开始寻找比中间点的值大的点==
                    if (isNumFlag)
                    {
                        while (i < u && Convert.ToInt64(arr[i, c]) < Convert.ToInt64(middle))
                        {
                            i += 1;
                        }
                    }
                    else
                    {
                        while (i < u && arr[i, c].CompareTo(middle) < 0)
                        {
                            i += 1;
                        }
                    }

                    //==从数组最大角标开始寻找比中间点的值小的点==
                    if (isNumFlag)
                    {
                        while (j > l && Convert.ToInt64(arr[j, c]) > Convert.ToInt64(middle))
                        {
                            j -= 1;
                        }
                    }
                    else
                    {
                        while (j > l && arr[j, c].CompareTo(middle) > 0)
                        {
                            j -= 1;
                        }
                    }

                    //==如果比数组中间点的值大的点不大于比数组中间点的值小的点，交换两个点的值，并从下一个点继续寻找==						
                    if (i <= j)
                    {
                        t = arr[i, c];
                        arr[i, c] = arr[j, c];
                        arr[j, c] = t;
                        i += 1;
                        j -= 1;
                    }

                    //==如果从小角标开始寻找的点大于从大角标开始寻找的点，退出循环==
                    if (i > j)
                    {
                        break;
                    }
                }

                //==再次排序==
                if (i < u)
                {
                    arrayRank2(arr, c, i, u);
                }
                if (j > l)
                {
                    arrayRank2(arr, c, l, j);
                }
                #endregion
            }

            return arr;
        }
        #endregion

        #region==一维数组反序排序 string[] arrayRev(string[] arr,int l,int u)==
        /// <summary>
        /// 一维数组反序排序
        /// </summary>
        /// <param name="arr">原数组</param>
        /// <param name="l">最小脚码</param>
        /// <param name="u">最大脚码</param>
        public static string[] arrayRev(string[] arr, int l, int u)
        {
            string[] t = new string[u + 1];
            int i;
            for (i = l; i <= u; i++)
            {
                t[u - i + l] = arr[i];
            }
            return t;
        }
        #endregion

        #region==二维数组反序排序 string[,] arrayRev2(string[,] arr,int l,int u)==
        /// <summary>
        /// 二维数组反序排序
        /// </summary>
        /// <param name="arr">原数组</param>
        /// <param name="l">最小脚码</param>
        /// <param name="u">最大脚码</param>
        public static string[,] arrayRev2(string[,] arr, int l, int u)
        {
            string[,] t = new string[u + 1, arr.GetLength(1)];
            int i, j;
            for (i = l; i <= u; i++)
            {
                for (j = 0; j < t.GetLength(1); j++)
                {
                    t[u - i + l, j] = arr[i, j];
                }
            }
            return t;
        }
        #endregion

        #region ==转换表单里的单引号和尖括号  string convers(string text)==
        /// <summary>
        /// 转换表单里的单引号和尖括号，接收表单、地址栏时使用
        /// </summary>
        public static string convers(string text)
        {
            if (text == null)
                return "";
            else
            {
                //全面替换字符 防止sql注入
                return text.Replace("'", "&acute;").Replace("<", "&lt;").Replace("\"", "&quot;");

            }
        }
        #endregion

        #region==用空格拆分字符串 string split_space(string text)==
        /// <summary>
        /// 用空格拆分字符串。split_space("abc")的值为：a b c
        /// </summary>
        public static string split_space(string text)
        {
            int i;
            string str = "";
            byte[] b = Encoding.ASCII.GetBytes(text);
            char[] c = text.ToCharArray();
            for (i = 0; i < b.Length; i++)
            {
                //HttpContext.Current.Response.Write(b[i].ToString() + "<br />");
                if (i > 0)
                {
                    if ((Convert.ToInt32(b[i]) == 63 && Convert.ToInt32(b[i - 1]) != 32) || Convert.ToInt32(b[i - 1]) == 63)
                        str += "&nbsp;";
                    else if (Convert.ToInt32(b[i]) == 32)
                        str += "&nbsp;";
                }
                if (Convert.ToInt32(b[i]) != 32)
                    str += c[i].ToString();
            }
            return str;
        }
        #endregion

        #region==验证页码合法性 int pageValid(int Pc,int Page)==
        /// <summary>
        /// 验证页码合法性，其他涉及到当前页的方法里，都有验证，不用单独使用。
        /// </summary>
        /// <param name="Pc">总页数</param>
        /// <param name="Page">当前页</param>
        public static int pageValid(int Pc, int Page)
        {
            if (Page > Pc)
            {
                Page = Pc;
            }
            if (Page < 1)
            {
                Page = 1;
            }
            return Page;
        }
        #endregion

        #region==后台分页显示 string Pagination(intPc,int Page,string Tp,string Pname,string Previous,string Next,string pageName, string method)==
        /// <summary>
        /// 后台分页显示，返回分页html字符串
        /// </summary>
        /// <param name="Pc">总页数</param>
        /// <param name="Page">当前页码</param>
        /// <param name="Tp">不自动传递的地址栏参数。多个参数用||分隔</param>
        /// <param name="Pname">页码参数name</param>
        /// <param name="Previous">上一页</param>
        /// <param name="Next">下一页</param>
        /// <param name="pageName">页</param>
        /// <param name="inputHeight">文本框的高</param>
        /// <param name="sk">容器样式</param>
        /// <param name="method">表单提交方式post/get</param>
        /// <returns></returns>
        public static string Pagination(int Pc, int Page, string Tp, string Pname, string Previous, string Next, string pageName, int inputHeight, string sk, string method)
        {
            string str;
            Page = pageValid(Pc, Page);        //验证页码合法性

            /*
            if (Tp == "")
                Tp += "||";
            Tp += Pname;
            */

            #region==获得分页字符串==
            if (Pc <= 1)
                str = "&nbsp;";
            else
            {
                str = "<div class=\"" + sk + "\">";
                if (method == "post")
                {
                    str += "&nbsp;<input class='skip' style=\"line-height:12px; font-size:9pt; padding:0px; width:20px\" name=\"page\" type=\"text\" id=\"page\"";
                    str += " value=\"" + Page + "\" onKeyDown=\"if((event.which == 13)||(event.keyCode == 13))";
                    str += "{document.getElementById('form1').submit();}\">";
                    str += "&nbsp;/&nbsp;" + Pc + "&nbsp;" + pageName + "&nbsp;&nbsp;";

                    if (Page == 1)
                        str += "<font color=\"#999999\">" + Previous + "</font>";
                    else
                    {
                        str += "<a href='' onclick='document.getElementById(\"page\").value=" + (Page - 1) + ";document.getElementById(\"form1\").submit();return false;'";
                        str += ">" + Previous + "</a>";
                    }

                    str += "&nbsp;&nbsp;";

                    if (Page == Pc)
                        str += "<font color=\"#999999\">" + Next + "</font>";
                    else
                    {
                        str += "<a href='' onclick='document.getElementById(\"page\").value=" + (Page + 1) + ";document.getElementById(\"form1\").submit();return false';";
                        str += ">" + Next + "</a>";
                    }
                }
                else
                {
                    str += "&nbsp;<input class='skip' style=\"line-height:12px; font-size:9pt; padding:0px; width:20px\" name=\"page\" type=\"text\" id=\"page\"";
                    str += " value=\"" + Page + "\" onKeyDown=\"if((event.which == 13)||(event.keyCode == 13))";
                    str += "{var pixt=window.location.href;var pix=pixt.indexOf('?');if(pix>0){pixt=pixt.substr(0,pix);}window.location.href=pixt+'?" + Pname + "='+this.value";
                    //str += "{window.navigate('?" + Pname + "=' + this.value";
                    str += " + '&" + transParameters(Tp) + "'";
                    str += ";}\">";
                    str += "&nbsp;/&nbsp;" + Pc + "&nbsp;" + pageName + "&nbsp;&nbsp;";

                    if (Page == 1)
                        str += "<font color=\"#999999\">" + Previous + "</font>";
                    else
                    {
                        str += "<a href=\"?" + Pname + "=" + (Page - 1).ToString();
                        str += "&" + transParameters(Tp);
                        str += "\">" + Previous + "</a>";
                    }

                    str += "&nbsp;&nbsp;";

                    if (Page == Pc)
                        str += "<font color=\"#999999\">" + Next + "</font>";
                    else
                    {
                        str += "<a href=\"?" + Pname + "=" + (Page + 1).ToString();
                        str += "&" + transParameters(Tp);
                        str += "\">" + Next + "</a>";
                    }
                }
                str += "</div>";
            }
            #endregion

            return str;
        }
        #endregion

        #region==前台分页显示 string Pagination1(int Pc, int Page, string Tp, string Pname, string Previous, string Next, string pageName, string sk, int c)==
        /// <summary>
        /// 前台分页显示
        /// </summary>
        /// <param name="Pc">总页数</param>
        /// <param name="Page">当前页码</param>
        /// <param name="Tp">不自动传递的地址栏参数。多个参数用||分隔</param>
        /// <param name="Pname">页码参数name</param>
        /// <param name="Previous">上一页</param>
        /// <param name="Next">下一页</param>
        /// <param name="pageName">页</param>
        /// <param name="sk">容器样式</param>
        /// <param name="start">显示页码数量</param>
        /// <returns></returns>
        public static string Pagination1(int Pc, int Page, string Tp, string Pname, string First, string Last, string Previous, string Next, string pageName, string sk, int c)
        {
            string str = "";
            int i;
            int j;
            int k;
            Tp += "||" + Pname;
            Page = pageValid(Pc, Page);        //验证页码合法性

            #region ==获得显示范围开始页==
            int c2 = Common.Functions.ConvertInt16(c / 2, 2);
            for (j = Page - c2; j < 1; j++) { }
            for (k = j; k > 1 && j - k < c2 - (Pc - Page); k--) { };
            #endregion

            #region ==获得分页字符串==
            str += "<div class=\"" + sk + "\" id=\"" + sk + "\"><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
            str += "<tr><td><ul>";
            str += "<li><a href=\"?" + Pname + "=1&" + transParameters(Tp) + "\">" + First + "</a></li>";
            str += "<li><a href=\"?" + Pname + "=" + (Page - 1) + "&" + transParameters(Tp) + "\">" + Previous + "</a></li>";
            for (i = k; i < k + c; i++)
            {
                if (i > Pc)
                    break;
                str += "<li";
                if (i == Page)
                    str += " class=\"now\"";
                str += ">";
                if (i != Page)
                    str += "<a href=\"?" + Pname + "=" + i + "&" + transParameters(Tp) + "\">";
                str += i + "</a></li>";
            }
            str += "<li><a href=\"?" + Pname + "=" + (Page + 1) + "&" + transParameters(Tp) + "\"";
            if (Page == Pc)
                str += "onclick=\"return false;\"";
            str += ">" + Next + "</a></li>";
            str += "<li><a href=\"?" + Pname + "=" + Pc + "&" + transParameters(Tp) + "\">" + Last + "</a></li>";
            str += "</ul></td></tr></table></div>";
            #endregion

            return str;
        }
        #endregion

        #region ==用JMAIL发送邮件 bool JmailSend==
        /// <summary>
        /// 用JMAIL发送邮件
        /// </summary>
        /// <param name="Subject">主题</param>
        /// <param name="Body">内容（isHtml为false时显示）</param>
        /// <param name="isHtml">是否使用html格式</param>
        /// <param name="HtmlBody">内容（isHtml为true时显示）</param>
        /// <param name="MailTo">收件人邮件地址</param>
        /// <param name="From">发件人邮件地址</param>
        /// <param name="FromName">发件人显示名称</param>
        /// <param name="Smtp">SMTP服务器地址</param>
        /// <param name="Username">发件人用户名</param>
        /// <param name="Password">发件人密码</param>
        /// <param name="Reply">收件人回复邮件时的回复地址</param>
        /// <param name="Bcc">秘密抄送地址</param>
        public static void JmailSend(string Subject, string Body, bool isHtml, string HtmlBody, string MailTo, string From, string FromName, string Smtp, string Username, string Password, string Reply, string[] Bcc)
        {
            bool sed = true;
            //POP3  pop3=new POP3();
            Message JmailMsg = new Message();
            JmailMsg.Silent = true;
            JmailMsg.Logging = true;
            JmailMsg.Charset = "gb2312";
            //JmailMsg.AppendHTML(HtmlBody);
            JmailMsg.MailServerUserName = Username;
            JmailMsg.MailServerPassWord = Password;
            JmailMsg.AddRecipient(MailTo, "", "");
            JmailMsg.From = From;
            JmailMsg.FromName = FromName;

            JmailMsg.Charset = "gb2312";
            JmailMsg.Logging = true;
            JmailMsg.Silent = true;

            JmailMsg.ReplyTo = Reply;
            for (int i = 0; i < Bcc.Length; i++)
            {
                JmailMsg.AddRecipientBCC(Bcc[i], "");
            }

            JmailMsg.Subject = Subject;
            JmailMsg.Body = Body;
            if (isHtml)
                JmailMsg.HTMLBody = HtmlBody;

            if (!JmailMsg.Send(Smtp, false))
                sed = false;

            JmailMsg.Close();
        }
        #endregion

        #region==生成验证码 string MakeValid(int m)==
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="m">验证码位数</param>
        public static string MakeValid(int m)
        {
            string[] validcode = new string[m];		//验证码
            int rndnum;					            //随机数

            Random random = new Random();

            for (int i = 0; i < m;)
            {
                rndnum = random.Next(49, 89);
                if (!((rndnum > (byte)'9') && (rndnum < (byte)'A')) && (rndnum <= (byte)'Z') && (rndnum != (byte)'0') && (rndnum != (byte)'1') && (rndnum != (byte)'O'))
                {
                    char t = (char)rndnum;
                    validcode[i++] = t.ToString();
                }
            }
            string tt = string.Join("", validcode);
            if (HttpContext.Current.Session["validcode"] == null)
                HttpContext.Current.Session.Add("validcode", tt);
            else
                HttpContext.Current.Session["validcode"] = tt;
            return String.Join("", validcode);
        }
        #endregion

        #region==MD5加密 string md5(string str,int code)==
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <param name="code">位数。16或者32</param>
        public static string md5(string str, int code)
        {
            if (code == 16)
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 16);
            }

            else if (code == 32)
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            }

            else
            {
                return "error";
            }
        }
        #endregion

        #region==SHA1加密 string sha1(string str)==
        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        public static string sha1(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1").ToLower();
        }
        #endregion

        #region ==删除多余上传文件 bool del_other(int start)==
        /// <summary>
        /// 删除垃圾文件时使用。
        /// </summary>
        public static bool del_other(int start)
        {
            string url;

            try
            {
                url = "/" + Common.Para.siteHead + "Uploadfile/temp";
                Directory.Delete(HttpContext.Current.Server.MapPath(url), true);
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(url));
            }
            catch { }

            return true;
        }
        #endregion

        #region ==删除指定文件 bool del_file(string filename)==
        /// <summary>
        /// 删除指定文件（删除记录时自动调用该方法删除相关图片，不用单独调用）
        /// </summary>
        /// <param name="filename">文件地址，非硬盘地址（不用Server.MapPath）</param>
        public static bool del_file(string filename)
        {
            if (filename != null && !Common.Para.pics_view)
            {
                if (filename != "")
                {
                    if (File.Exists(HttpContext.Current.Server.MapPath(filename)))
                    {
                        try
                        {
                            File.Delete(HttpContext.Current.Server.MapPath(filename));
                        }
                        catch { return false; }
                    }
                }
            }
            return true;
        }
        #endregion

        #region ==获得文件名重载 string getFilename(string filepath, string dir)==
        /// <summary>
        /// 根据当前日期，为新上传的文件生成新的随机文件名，包含路径，不包含siteHead，如：Uploadfile/Article/201408082341.jpg
        /// </summary>
        /// <param name="filepath">上传的文件名（为了获得文件后缀）</param>
        /// <param name="dir">保存的路径，不用传siteHead过来，以/结束，如：Uploadfile/Article/</param>
        public static string getFilename(string filepath, string dir)
        {
            Random r = new Random();
            r.Next(1000, 9999);
            return getFilename(filepath, dir, r);
        }
        #endregion

        #region ==获得文件名 string getFilename(string filepath, string dir, Random r)==
        /// <summary>
        /// 根据当前日期，为新上传的文件生成新的随机文件名，包含路径，不包含siteHead，如：Uploadfile/Article/201408082341.jpg
        /// </summary>
        /// <param name="filepath">上传的文件名（为了获得文件后缀）</param>
        /// <param name="dir">保存的路径，不用传siteHead过来，以/结束，如：Uploadfile/Article/</param>
        /// <param name="r">实例化的随机数</param>
        public static string getFilename(string filepath, string dir, Random r)
        {

            //取得文件名(包括路径)里最后一个"."的索引
            int intExt = filepath.LastIndexOf(".");

            //取得文件扩展名
            string strExt = filepath.Substring(intExt);

            //这里自动根据日期和随机数不同为文件命名,确保文件名不重复
            DateTime datNow = DateTime.Now;
            string strNewName;
            while (true)
            {
                strNewName = datNow.Year.ToString() + datNow.Month.ToString() + datNow.Day.ToString() + r.Next(1000, 9999) + strExt;
                //HttpContext.Current.Response.Write(strNewName + "<br />");
                if (!File.Exists(HttpContext.Current.Server.MapPath("/" + Common.Para.siteHead + dir + strNewName)))
                    break;
            }
            return dir + strNewName;
        }
        #endregion

        #region ==各种Convert==
        /// <summary>
        /// 转换变量为Int64
        /// </summary>
        /// <param name="str">object</param>
        public static long ConvertInt64(object str)
        {
            return ConvertInt64(str, 0);
        }
        /// <summary>
        /// 转换变量为Int64
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static long ConvertInt64(object str, int i)
        {
            try
            {
                return Convert.ToInt64(str);
            }
            catch
            {
                return i;
            }
        }
        /// <summary>
        /// 转换变量为Int32
        /// </summary>
        /// <param name="str">object</param>
        public static int ConvertInt32(object str)
        {
            return ConvertInt32(str, 0);
        }
        /// <summary>
        /// 转换变量为Int32
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static int ConvertInt32(object str, int i)
        {
            try
            {
                return Convert.ToInt32(str + "");
            }
            catch
            {
                return i;
            }
        }
        /// <summary>
        /// 转换变量为Int16
        /// </summary>
        /// <param name="str">object</param>
        public static int ConvertInt16(object str)
        {
            return ConvertInt16(str, 0);
        }
        /// <summary>
        /// 转换变量为Int16
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static int ConvertInt16(object str, int i)
        {
            try
            {
                return Convert.ToInt16(str);
            }
            catch
            {
                return i;
            }
        }
        /// <summary>
        /// 转换变量为Single
        /// </summary>
        /// <param name="str">object</param>
        public static float ConvertSingle(object str)
        {
            return ConvertSingle(str, 0);
        }
        /// <summary>
        /// 转换变量为Single
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static float ConvertSingle(object str, int i)
        {
            try
            {
                return Convert.ToSingle(str);
            }
            catch
            {
                return i;
            }
        }
        /// <summary>
        /// 转换变量为DateTime
        /// </summary>
        /// <param name="str">object</param>
        public static DateTime ConvertDateTime(object str)
        {
            return ConvertDateTime(str, DateTime.Now);
        }
        /// <summary>
        /// 转换变量为DateTime
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static DateTime ConvertDateTime(object str, DateTime i)
        {
            try
            {
                return Convert.ToDateTime(str);
            }
            catch
            {
                return i;
            }
        }
        /// <summary>
        /// 转换变量为Decimal
        /// </summary>
        /// <param name="str">object</param>
        public static Decimal ConvertDecimal(object str)
        {
            return ConvertDecimal(str, 0);
        }
        /// <summary>
        /// 转换变量为Decimal
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static Decimal ConvertDecimal(object str, Decimal i)
        {
            try
            {
                return Convert.ToDecimal(str);
            }
            catch
            {
                return i;
            }
        }
        /// <summary>
        /// 转换变量为String
        /// </summary>
        /// <param name="str">object</param>
        public static string ConvertString(object str)
        {
            return ConvertString(str, "");
        }
        /// <summary>
        /// 转换变量为String
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static string ConvertString(object str, string s)
        {
            try
            {
                return str.ToString();
            }
            catch
            {
                return s;
            }
        }
        #endregion

        #region ==在图片上生成图片水印==
        /// <summary>    
        /// 在图片上生成图片水印
        /// </summary>    
        /// <param name="Path">原服务器图片路径</param>   
        /// <param name="Path_syp">生成的带图片水印的图片路径</param>   
        /// <param name="Path_sypf">水印图片路径</param>
        /// <param name="position">水印位置1-左上，2-中上，3-右上，4-左中，5-中间，6-右中，7-左下，8-中下，9-右下 </param>
        /// <param name="padding">边距:默认10像素</param>
        public static void addWaterMark(string Path, string Path_syp, string Path_sypf, int position, int padding)
        {
            try
            {
                System.Drawing.Image copyImage = System.Drawing.Image.FromFile(Path_sypf);
                System.Drawing.Image image = System.Drawing.Image.FromFile(Path);

                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
                int x = 0;
                int y = 0;
                int width = copyImage.Width;
                int height = copyImage.Height;
                switch (position)
                {
                    case 1:
                        x = 0 + padding;
                        y = 0 + padding;
                        break;
                    case 2:
                        x = image.Width / 2 - copyImage.Width / 2;
                        y = 0 + padding;
                        break;
                    case 3:
                        x = image.Width - copyImage.Width - padding;
                        y = 0 + padding;
                        break;
                    case 4:
                        x = padding;
                        y = image.Height / 2 - copyImage.Height / 2;
                        break;
                    case 5:
                        x = image.Width / 2 - copyImage.Width / 2;
                        y = image.Height / 2 - copyImage.Height / 2;
                        break;
                    case 6:
                        x = image.Width - copyImage.Width - padding;
                        y = image.Height / 2 - copyImage.Height / 2;
                        break;
                    case 7:
                        x = padding;
                        y = image.Height - copyImage.Height - padding;
                        break;
                    case 8:
                        x = image.Width / 2 - copyImage.Width / 2;
                        y = image.Height - copyImage.Height - padding;
                        break;
                    case 9:
                        x = image.Width - copyImage.Width - padding;
                        y = image.Height - copyImage.Height - padding;
                        break;
                }
                System.Drawing.Rectangle r = new System.Drawing.Rectangle(x, y, width, height);
                g.DrawImage(copyImage, r, 0, 0, copyImage.Width, copyImage.Height, System.Drawing.GraphicsUnit.Pixel);
                g.Dispose();

                image.Save(Path_syp);
                image.Dispose();
            }
            catch (Exception)
            {
                File.Move(Path, Path_syp);
            }
        }
        #endregion

        #region ==生成缩略图==
        /// <summary> 
        /// 生成缩略图 
        /// </summary> 
        /// <param name="originalImagePath">源图路径(要Server.MapPath())</param> 
        /// <param name="thumbnailPath">缩略图路径(要Server.MapPath())</param> 
        /// <param name="width">缩略图宽度</param> 
        /// <param name="height">缩略图高度</param> 
        /// <param name="mode">生成缩略图的方式:HW指定高宽缩放(可能变形);W指定宽，高按比例 H指定高，宽按比例 Cut指定高宽裁减(不变形)</param>　　 
        /// <param name="imageType">要缩略图保存的格式(gif,jpg,bmp,png) 为空或未知类型都视为jpg</param>　　 
        public static void MakeThumb(string originalImagePath, string thumbnailPath, int width, int height, string mode, string imageType)
        {
            Image originalImage = Image.FromFile(originalImagePath);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）　　　　　　　　 
                    break;
                case "W"://指定宽，高按比例　　　　　　　　　　 
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例 
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）　　　　　　　　 
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }
            //新建一个bmp图片 
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板 
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
            new Rectangle(x, y, ow, oh),
            GraphicsUnit.Pixel);
            try
            {
                //以jpg格式保存缩略图 
                switch (imageType.ToLower())
                {
                    case "gif":
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case "jpg":
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case "bmp":
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case "png":
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                }
            }
            catch (System.Exception e)
            {
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
        #endregion

        #region ==判断逗号分隔的主码中是否含有某一主码==
        /// <summary>
        /// 判断逗号分隔的主码中是否含有某一主码
        /// </summary>
        /// <param name="str">被比较字符串。可以为逗号分隔的多个主码，也可以是尖括号包围的多个主码</param>
        /// <param name="Pat">条件主码</param>
        public static bool checkHave(string str, string Pat)
        {
            if (str == "" || str == null)
                return false;
            str = "<" + str.Replace(",", "><") + ">";
            if (str.IndexOf("<" + Pat + ">") > -1)
                return true;
            else
                return false;
        }
        #endregion

        #region ==更换原密码MD5加密后的字符==
        /// <summary>
        /// 更换原密码MD5加密后的字符
        /// </summary>
        /// <param name="Passwd">原密码，客户端接收。</param>
        /// <returns></returns>
        public static string updatePasswd(string Passwd)
        {
            if (Passwd == "" || Passwd == null)
                return "";
            string pwd = Common.Functions.md5(Passwd, 32);
            string repwd = Para.adminUrl.Substring(0, Para.adminUrl.Length - 1);
            return pwd = pwd.Substring(0, 8) + repwd + pwd.Remove(0, 24);
        }
        #endregion

        #region ==服务器端request请求==
        /// <summary>
        /// 服务器端request请求
        /// </summary>
        /// <param name="url">服务器端请求页面地址。http全路径，可以带地址栏参数</param>
        /// <param name="method">请求方法，GET/POST</param>
        public static string getXMLHTTP(string url, string method)
        {
            return getXMLHTTP(url, method, "", "", "");
        }
        /// <summary>
        /// 服务器端request请求
        /// </summary>
        /// <param name="url">服务器端请求页面地址。http全路径，可以带地址栏参数</param>
        /// <param name="method">请求方法，GET/POST</param>
        /// <param name="Para">post参数，一个字符串</param>
        public static string getXMLHTTP(string url, string method, string Para)
        {
            return getXMLHTTP(url, method, Para, "", "");
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   // 总是接受    
            return true;
        }
        /// <summary>
        /// 服务器端request请求
        /// </summary>
        /// <param name="url">服务器端请求页面地址。http全路径，可以带地址栏参数</param>
        /// <param name="method">请求方法，GET/POST</param>
        /// <param name="Para">post参数，一个字符串</param>
        /// <param name="cert_path">证书物理路径。如：@"e:\\abc.pfx" 或 Server.MapPath("/abc.pfx")</param>
        /// <param name="cert_passwd">证书密码</param>
        public static string getXMLHTTP(string url, string method, string Para, string cert_path, string cert_passwd)
        {
            string responseFromServer;
            X509Certificate2 cert;
            try
            {
                cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(cert_path, cert_passwd, X509KeyStorageFlags.MachineKeySet);
            }
            catch
            {
                cert = null;
            }
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            if (Para == "")
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = method;
                WebResponse rep = req.GetResponse();
                Stream webstream = rep.GetResponseStream();
                StreamReader sr = new StreamReader(webstream);
                responseFromServer = sr.ReadToEnd();
            }
            else
            {
                // Create a request using a URL that can receive a post. 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                // Set the Method property of the request to POST.
                request.Method = method;

                if (cert != null)
                    request.ClientCertificates.Add(cert);
                // Create POST data and convert it to a byte array.
                string postData = Para;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/x-www-form-urlencoded";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                responseFromServer = reader.ReadToEnd();
                // Display the content.
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
            }

            return responseFromServer;
        }
        #endregion

        #region ==格式化价格（整数只显示整数部分；小数忽略最后的0；最多显示两位小数）==
        /// <summary>
        /// 格式化价格（整数只显示整数部分；小数忽略最后的0；最多显示两位小数）
        /// </summary>
        /// <param name="Price">价格</param>
        /// <returns></returns>
        public static string formatPrice(string Price)
        {
            string str = "";
            if (Price.Split('.').Length == 1)
                str = Price;
            else if (Convert.ToInt32(Price.Split('.')[1]) == 0)
                str = Convert.ToInt32(Price.Split('.')[0]).ToString("0");
            else
                str = Convert.ToDecimal(Price).ToString("0.00").TrimEnd('0');

            return str;
        }
        #endregion

        #region 格式化价格保留2位小数，不四舍五入
        /// <summary>
        /// 格式化价格保留2位小数，超过的位数不要了 如1.126545454 结果是 1.12
        ///  malongjun
        /// </summary>
        /// <param name="Price"></param>
        /// <returns></returns>
        public static string FormatPrice_ellipsis(string Price)
        {
            //先判断有没有点
            int index = Price.IndexOf(".");
            if (index == -1)
            {
                return Price;
            }
            string integer_str = "";
            string decimal_str = "";

            //整数部分
            integer_str = Price.Substring(0, index);
            //小数部分
            decimal_str = Price.Substring(index + 1);

            //如果小数大于2位就保留2位
            if (decimal_str.Length > 2)
            {
                decimal_str = decimal_str.Substring(0, 2).TrimEnd('0'); ;
            }
            if (decimal_str == "")
            {
                return integer_str;
            }
            return integer_str + "." + decimal_str;
        }
        #endregion

        #region ==生成时间戳==
        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <param name="dt">日期。注意时差，可以用DateTime.UtcNow</param>
        public static string create_timestamp(DateTime dt)
        {
            return ((dt.Ticks - Convert.ToDateTime("1970-1-1").Ticks) / 10000000).ToString();
        }
        #endregion

        #region ==获得服务器的Mac地址==
        /// <summary>
        /// 获得服务器的Mac地址
        /// </summary>
        /*
        public static string GetMacAddress_Server()
        {
            string mac = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    mac = mo["MacAddress"].ToString();
                    break;
                }
            }
            return mac;
        }
        */
        #endregion

        #region ==获得客户端的Mac地址==
        /// <summary>
        /// 获得客户端的Mac地址
        /// </summary>
        /*
        public static string GetMacAddress_Browser()
        {
            // 在此处放置用户代码以初始化页面
            try
            {
                string strClientIP = HttpContext.Current.Request.UserHostAddress.ToString().Trim();
                Int32 ldest = inet_addr(strClientIP); //目的地的ip 
                Int32 lhost = inet_addr("");   //本地服务器的ip 
                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                string mac_src = macinfo.ToString("X");
                while (mac_src.Length < 12)
                {
                    mac_src = mac_src.Insert(0, "0");
                }
                string mac_dest = "";
                for (int i = 0; i < 11; i++)
                {
                    if (0 == (i % 2))
                    {
                        if (i == 10)
                        {
                            mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                        else
                        {
                            mac_dest = ":" + mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                    }
                }
                return mac_dest;
            }
            catch (Exception err)
            {
                return "";
            }

        }
        */
        #endregion

        #region ==获得客户端的Mac地址2==
        /// <summary>
        /// 获得客户端的Mac地址2
        /// </summary>
        /*
        public static string GetMacAddress_Browser2()
        {
            // 在此处放置用户代码以初始化页面
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac + "aaa";
            }
            catch
            {
                return "error";
            }

        }
        */
        #endregion

        #region 将base64格式的图片保存到指定文件夹中
        /// <summary>
        /// 将base64格式的图片保存到指定文件夹中
        /// </summary>
        /// <param name="filepath">要将图片保存的路径</param>
        /// <param name="base64">base64编码</param>
        public static void Base64StringToImage(string filepath, string base64)
        {
            //将base64转化为byte数组：
            byte[] data = Convert.FromBase64String(base64);
            try
            {
                FileStream ifs = new FileStream(System.Web.HttpContext.Current.Server.MapPath(filepath), FileMode.Create, FileAccess.Write);
                ifs.Write(data, 0, data.Length);
                ifs.Close();
            }
            catch
            {
            }
        }
        #endregion

        #region ==Base64 encrypt==
        /// <summary>
        /// Base64 encrypt.
        /// </summary>
        /// <param name="sourthUrl">源码</param>
        public static string Base64Encrypt(string sourthUrl)
        {
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            string eurl = HttpUtility.UrlEncode(sourthUrl);
            eurl = Convert.ToBase64String(encoding.GetBytes(eurl));
            return eurl;
        }
        #endregion

        #region ==Base64 decrypt==
        /// <summary>
        /// Base64s decrypt.
        /// </summary>
        /// <param name="eStr">编过的码</param>
        public static string Base64Decrypt(string eStr)
        {
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            byte[] buffer = Convert.FromBase64String(eStr);
            string sourthUrl = encoding.GetString(buffer);
            sourthUrl = HttpUtility.UrlDecode(sourthUrl);
            return sourthUrl;
        }
        #endregion

        #region 是否是Base64字符串
        /// <summary>
        /// 是否是Base64字符串
        /// </summary>
        /// <param name="eStr"></param>
        /// <returns></returns>
        public static bool IsBase64(string eStr)
        {
            if ((eStr.Length % 4) != 0)
            {
                return false;
            }
            if (!Regex.IsMatch(eStr, "^[A-Z0-9/+=]*$", RegexOptions.IgnoreCase))
            {
                return false;
            }
            return true;
        }
		#endregion

		#region ==验证逗号或竖线分隔或尖括号包围的主键串均为数字类型，不是的过滤掉。都不是的话返回空字符串==
		/// <summary>
		/// 验证逗号或竖线分隔或尖括号包裹的主键串均为数字类型，不是的过滤掉。都不是的话返回空字符串
		/// </summary>
		/// <param name="pk">主键字符串，可逗号或竖线分隔，也可用尖括号包裹</param>
		public static string returnConvertPK(string pk)
        {
            if (pk == null)
            {
                return "";
            }
            int i = 0;
            int u;
            string[] arr;
            string str;
            arr = pk.Replace("|", ",").Replace("><", ",").Replace("<", "").Replace(">", "").Split(',');
            u = arr.Length;
            str = "";
            for (; i < u; i++)
            {
                if (Common.Functions.ConvertInt64(arr[i], 0) == 0)
                    continue;
                if (i > 0)
                    str += ",";
                str += arr[i].ToString();
            }
            return str;
        }
        #endregion

        #region ==验证签名方法==
        /// <summary>
        /// 验证签名方法重载，不验证数据库
        /// </summary>
        /// <param name="signature">签名</param>
        /// <param name="non_str">随机数</param>
        /// <param name="stamp">时间戳</param>
        /// <param name="ParameterList">拼接好的参数键值对</param>
        public static string VerifySigned(string signature, string non_str, string stamp, List<string> ParameterList)
        {
            return VerifySigned(signature, non_str, stamp, ParameterList, 1);
        }
        /// <summary>
        /// 验证签名方法
        /// </summary>
        /// <param name="signature">签名</param>
        /// <param name="non_str">随机数</param>
        /// <param name="stamp">时间戳</param>
        /// <param name="ParameterList">拼接好的参数键值对</param>
        /// <param name="IsOperationDB">需要验证数据库记录。1-不验证 0-验证</param>
        public static string VerifySigned(string signature, string non_str, string stamp, List<string> ParameterList, int IsOperationDB)
        {
            string result = sign_topu.Sign.VerifySigned(signature, non_str, stamp, ParameterList, IsOperationDB);
            if (result == "FAIL")
                result = "30001";

            if (result != "SUCCESS")
            {
                returnResult(result);
            }

            return result;
        }
        #endregion

        #region ==输出错误码到页面==
        /// <summary>
        /// 输出错误码到页面
        /// </summary>
        /// <param name="resultCode">错误码</param>
        public static void returnResult(string resultCode)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write("{\"error\":\"" + resultCode + "\"}");
            HttpContext.Current.Response.End();
        }
		#endregion

		#region ==帮助BLL层处理参数，有总参数的话，拆分主扩&补充主参数；没有总参数则补充主参数==
		/// <summary>
		/// 帮助BLL层处理参数，有总参数的话，拆分主扩&补充主参数；没有总参数则补充主参数
		/// </summary>
		/// <param name="P_total">总参数，不为空时拆分为主参数和扩展参数</param>
		/// <param name="P_main">主参数，总参数为空时必填，总参数不为空时没用</param>
		/// <param name="P_extend">扩展参数，总参数不为空时没用</param>
		/// <param name="demand">扩展参数和需要主参数  ｛"扩a,主1,主2","扩b,主1","扩c,主1","扩f,主1"｝</param>
		public static string[] Parameters_Filter(string P_total, string P_main, string P_extend, string[] demand)
        {
            if (P_total == null)
                P_total = "";
            if (P_main == null)
                P_main = "";
            if (P_extend == null)
                P_extend = "";

            if (P_total == "")
            {
                string[] return_arr = new string[2];
                return_arr[0] = Supply_Parameter_Main(P_main, P_extend, demand);
                return_arr[1] = P_extend;
                return return_arr;
            }
            else
            {
                return Parameter_Split(P_total, demand);
            }
        }
        #endregion

        #region ==根据扩展参数，补充参数到主参数==
        /// <summary>
        /// 根据扩展参数，补充参数到主参数。返回主参数
        /// </summary>
        /// <param name="Main">原有主参数</param>
        /// <param name="Extended">扩展参数</param> "a,b,c,f"
        /// <param name="demand">扩展参数和需要主参数  ｛"扩a,主1,主2","扩b,主1","扩c,主1","扩f,主1"｝</param>
        private static string Supply_Parameter_Main(string Main, string Extended, string[] demand)
        {
            // 如果扩展参数为空或拆分参照为空，直接退出
            if (Extended == "" || demand.Length == 0)
                return Main;

            #region ==遍历demand，如扩展参数存在，且对应主参数不存在，则补充到主参数列表==
            string[] Paras_arr;
            int i, len;
            foreach (string Paras in demand)
            {
                Paras_arr = Paras.Split(',');

                // 如果参数不够2个，continue
                if (Paras_arr.Length < 2)
                    continue;

                if (checkHave(Extended, Paras_arr[0]))
                {
                    i = 1;
                    len = Paras_arr.Length;
                    for (; i < len; i++)
                    {
                        if (!checkHave(Main, Paras_arr[i]))
                        {
                            if (Main != "")
                                Main += ",";
                            Main += Paras_arr[i];
                        }
                    }
                }
            }

            return Main;
            #endregion
        }
        #endregion

        #region ==将总参数拆分成主参数和扩展参数，并将扩展参数需要主参数补充到主参数==
        /// <summary>
        /// 将总参数拆分成主参数和扩展参数，并将扩展参数需要主参数补充到主参数。返回数组[主参数,扩展参数]
        /// </summary>
        /// <param name="parameter">总参数，逗号分隔</param> a,b,c,d,e
        /// <param name="demand">扩展参数和需要主参数  ｛"扩a,主1,主2","扩b,主1","扩c,主1","扩f,主1"｝</param>
        private static string[] Parameter_Split(string parameter, string[] demand)
        {
            string[] return_arr = { "", "" };

            // 总参数为空
            if (parameter == "")
                return return_arr;

            // 拆分参照为空
            if (demand.Length == 0)
            {
                return_arr[0] = parameter;
                return return_arr;
            }

            #region ==遍历parameter，判断每个参数是否为扩展参数，不是进入主参数，是的话判断是否要为主参数补充参数==
            string Main = "", Extend = "";
            string[] Para_arr = parameter.Split(','); // 主参数数组

            // 将拆分参照中的主参数抹去，只留下扩展参数
            List<string> demand_extend = new List<string>();
            int i = 0, len = demand.Length;
            string temp; // 临时使用
            for (; i < len; i++)
            {
                temp = demand[i].Split(',')[0];
                demand_extend.Add("<" + temp + ">");
            }
            #endregion

            i = 0;
            len = Para_arr.Length;
            int j, len2;
            int index;
            string[] demand_arr;
            for (; i < len; i++)
            {

                index = demand_extend.FindIndex(delegate (string T) { return T == "<" + Para_arr[i] + ">"; });
                if (index != -1 && !checkHave(Extend, Para_arr[i]))
                {   // 如果(该参数属于扩展参数&&Extend中无此参数)，则添加参数到Extend，并判断需要的配套主参数是否需要添加到主参数
                    if (Extend != "")
                        Extend += ",";
                    Extend += Para_arr[i];

                    j = 1;
                    demand_arr = demand[index].Split(',');
                    len2 = demand_arr.Length;
                    for (; j < len2; j++)
                    {
                        if (!checkHave(Main, demand_arr[j]))
                        {
                            if (Main != "")
                                Main += ",";
                            Main += demand_arr[j];
                        }
                    }
                }
                else if (index == -1 && !checkHave(Main, Para_arr[i]))
                {   // 如果(该参数不属于扩展参数&&Main中无此参数)，则添加参数到Main
                    if (Main != "")
                        Main += ",";
                    Main += Para_arr[i];
                }
            }

            return_arr[0] = Main;
            return_arr[1] = Extend;
            return return_arr;
        }
        #endregion

        #region 获得Ip
        /// <summary>
        /// 获得Ip k=0 验证安卓和苹果手机，获取ip方法不一样 1-不验证 ip从 ServerVariables["HTTP_X_FORWARDED_FOR"] 中获取
        /// </summary>
        /// <param name="k"></param>
        public static string getIp(int k)
        {
            string IpAddr = "";
            HttpRequest request = HttpContext.Current.Request;

            if (k == 0)
            {
                string agent = HttpContext.Current.Request.UserAgent;
                try
                {
                    //安卓系统
                    if (agent.Contains("Android"))
                    {
                        IpAddr = request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
                    }
                    //苹果或者Pc端
                    else
                    {
                        IpAddr = request.ServerVariables["REMOTE_ADDR"];
                    }
                }
                catch
                {
                    IpAddr = request.ServerVariables["REMOTE_ADDR"];
                }
            }
            else if (k == 1)
            {
                IpAddr = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            return IpAddr;
        }
        /// <summary>
        /// 获得Ip
        /// </summary>
        /// <returns></returns>
        public static string getIp()
        {
            return getIp(0);
        }
        #endregion

        #region ==对二维数组排序==
        /// <summary>  
        /// 对二维数组排序  
        /// </summary>  
        /// <param name="values">排序的二维数组</param>  
        /// <param name="orderColumnsIndexs">排序根据的列的索引号数组</param>  
        /// <param name="type">排序的类型，1代表降序，0代表升序</param>  
        /// <returns>返回排序后的二维数组</returns>  
        public static object[,] Orderby(object[,] values, int[] orderColumnsIndexs, int type)
        {
            object[] temp = new object[values.GetLength(1)];
            int k;
            int compareResult;
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (k = i + 1; k < values.GetLength(0); k++)
                {
                    if (type.Equals(1))
                    {
                        for (int h = 0; h < orderColumnsIndexs.Length; h++)
                        {
                            compareResult = Comparer.Default.Compare(GetRowByID(values, k).GetValue(orderColumnsIndexs[h]), GetRowByID(values, i).GetValue(orderColumnsIndexs[h]));
                            if (compareResult.Equals(1))
                            {
                                temp = GetRowByID(values, i);
                                Array.Copy(values, k * values.GetLength(1), values, i * values.GetLength(1), values.GetLength(1));
                                CopyToRow(values, k, temp);
                            }
                            if (compareResult != 0)
                                break;
                        }
                    }
                    else
                    {
                        for (int h = 0; h < orderColumnsIndexs.Length; h++)
                        {
                            compareResult = Comparer.Default.Compare(GetRowByID(values, k).GetValue(orderColumnsIndexs[h]), GetRowByID(values, i).GetValue(orderColumnsIndexs[h]));
                            if (compareResult.Equals(-1))
                            {
                                temp = GetRowByID(values, i);
                                Array.Copy(values, k * values.GetLength(1), values, i * values.GetLength(1), values.GetLength(1));
                                CopyToRow(values, k, temp);
                            }
                            if (compareResult != 0)
                                break;
                        }
                    }
                }
            }
            return values;

        }
        /// <summary>  
        /// 获取二维数组中一行的数据  
        /// </summary>  
        /// <param name="values">二维数据</param>  
        /// <param name="rowID">行ID</param>  
        /// <returns>返回一行的数据</returns>  
        static object[] GetRowByID(object[,] values, int rowID)
        {
            if (rowID > (values.GetLength(0) - 1))
                throw new Exception("rowID超出最大的行索引号!");

            object[] row = new object[values.GetLength(1)];
            for (int i = 0; i < values.GetLength(1); i++)
            {
                row[i] = values[rowID, i];

            }
            return row;

        }
        /// <summary>  
        /// 复制一行数据到二维数组指定的行上  
        /// </summary>  
        /// <param name="values"></param>  
        /// <param name="rowID"></param>  
        /// <param name="row"></param>  
        static void CopyToRow(object[,] values, int rowID, object[] row)
        {
            if (rowID > (values.GetLength(0) - 1))
                throw new Exception("rowID超出最大的行索引号!");
            if (row.Length > (values.GetLength(1)))
                throw new Exception("row行数据列数超过二维数组的列数!");
            for (int i = 0; i < row.Length; i++)
            {
                values[rowID, i] = row[i];
            }
        }
        #endregion

        #region ==利用DES加密解密==
        /// <summary>
        /// 利用DES加密算法加密字符串（可解密） 
        /// </summary>
        /// <param name="plaintext">被加密的字符串</param>
        /// <param name="key">密钥（只支持8个字节的密钥）</param>
        /// <param name="iv">向量</param>
        public static string EncryptString(string plaintext, string key, string iv)
        {
            DES des = new DESCryptoServiceProvider();
            des.Key = Encoding.UTF8.GetBytes(key);
            des.IV = Encoding.UTF8.GetBytes(iv);
            byte[] bytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] resultBytes = des.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            return Convert.ToBase64String(resultBytes);
        }

        /// <summary>
        /// 利用DES解密算法解密密文
        /// </summary>
        /// <param name="ciphertext">被解密的字符串</param>
        /// <param name="key">密钥（只支持8个字节的密钥，同前面的加密密钥相同）</param>
        /// <param name="iv">向量</param>
        /// <returns></returns>
        public static string DecryptString(string ciphertext, string key, string iv)
        {
            DES des = new DESCryptoServiceProvider();
            des.Key = Encoding.UTF8.GetBytes(key);
            des.IV = Encoding.UTF8.GetBytes(iv);
            byte[] bytes = Convert.FromBase64String(ciphertext);
            byte[] resultBytes = des.CreateDecryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(resultBytes);
        }
        #endregion
    }
}
