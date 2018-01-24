# .net(c#)方法集合 v1.1.1
### 安装：npm install topunet-net-functions


方法列表：
-------------

        /// <summary>
        /// 生成随机码。返回字符串
        /// </summary>
        /// <param name="n">位数，数字+字母的组合时请键入偶数</param>
        /// <param name="Kind">生成种类。1-纯数字 2-纯大写字母 3-纯小写字母 4-数字+大写字母 5-数字+小写字母 6-大写字母或小写字母 7-数字+纯大写字母或小写字母</param>
        public static string createRandomStr(int n, int Kind)

        /// <summary>
        /// 文件转二进制流下载
        /// </summary>
        /// <param name="response">HttpResponse response</param>
        /// <param name="serverPath">文件地址（非硬盘路径，不用Server.MapPath）</param>
        /// <param name="encode">文件编码</param>
        public static bool DownloadFile(HttpResponse response, string serverPath, string encode)

        /// <summary>
        /// 获得文件代码
        /// </summary>
        /// <param name="path">文件地址（非硬盘路径，不用Server.MapPath）</param>
        /// <returns>Server.HtmlEncode(str)</returns>
        public static string getCode(string path)

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

        /// <summary>
        /// 自动传递地址栏参数。地址栏：?id=1＆n=abc＆page=5＆s=def --> func.transParameters("n||s")的返回值：id=1＆page=5
        /// </summary>
        /// <param name="Para">不传递的参数名，多个用||分隔，区分大小写</param>
        public static string transParameters(string Para)

        /// <summary>
        /// 省略字符串
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <param name="length">字节长度（一个汉字=2个字节）</param>
        /// <param name="more">是否显示省略号...</param>
        public static string shenglue(string text, int length, bool more)

        /// <summary>
        /// 判断字符串长度（占用的字节数，一个汉字占用两个字节）
        /// </summary>
        /// <param name="text">字符串</param>

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

        /// <summary>
        /// 一维数组升序排序（最优算法）
        /// </summary>
        /// <param name="arr">原始数组</param>
        /// <param name="l">最小脚码</param>
        /// <param name="u">最大脚码</param>
        public static string[] arrayRank(string[] arr, int l, int u)

        /// <summary>
        /// 二维数组升序排序
        /// </summary>
        /// <param name="arr">原始数组</param>
        /// <param name="c">排序依据列 序号</param>
        /// <param name="l">最小脚码</param>
        /// <param name="u">最大脚码</param>
        public static string[,] arrayRank2(string[,] arr, int c, int l, int u)

        /// <summary>
        /// 一维数组反序排序
        /// </summary>
        /// <param name="arr">原数组</param>
        /// <param name="l">最小脚码</param>
        /// <param name="u">最大脚码</param>
        public static string[] arrayRev(string[] arr, int l, int u)

        /// <summary>
        /// 二维数组反序排序
        /// </summary>
        /// <param name="arr">原数组</param>
        /// <param name="l">最小脚码</param>
        /// <param name="u">最大脚码</param>
        public static string[,] arrayRev2(string[,] arr, int l, int u)

        /// <summary>
        /// 转换表单里的单引号和尖括号，接收表单、地址栏时使用
        /// </summary>
        public static string convers(string text)

        /// <summary>
        /// 用空格拆分字符串。split_space("abc")的值为：a b c
        /// </summary>
        public static string split_space(string text)

        /// <summary>
        /// 验证页码合法性，其他涉及到当前页的方法里，都有验证，不用单独使用。
        /// </summary>
        /// <param name="Pc">总页数</param>
        /// <param name="Page">当前页</param>
        public static int pageValid(int Pc, int Page)

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
        
        /// <summary>
        /// 前台分页显示，返回分页html字符串
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
        
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="m">验证码位数</param>
        public static string MakeValid(int m)

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <param name="code">位数。16或者32</param>
        public static string md5(string str, int code)

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        public static string sha1(string str)

        /// <summary>
        /// 删除垃圾文件时使用。
        /// </summary>
        public static bool del_other(int start)

        /// <summary>
        /// 删除指定文件（删除记录时自动调用该方法删除相关图片，不用单独调用）
        /// </summary>
        /// <param name="filename">文件地址，非硬盘地址（不用Server.MapPath）</param>
        public static bool del_file(string filename)

        /// <summary>
        /// 根据当前日期，为新上传的文件生成新的随机文件名，包含路径，不包含siteHead，如：Uploadfile/Article/201408082341.jpg
        /// </summary>
        /// <param name="filepath">上传的文件名（为了获得文件后缀）</param>
        /// <param name="dir">保存的路径，不用传siteHead过来，以/结束，如：Uploadfile/Article/</param>
        public static string getFilename(string filepath, string dir)

        /// <summary>
        /// 根据当前日期，为新上传的文件生成新的随机文件名，包含路径，不包含siteHead，如：Uploadfile/Article/201408082341.jpg
        /// </summary>
        /// <param name="filepath">上传的文件名（为了获得文件后缀）</param>
        /// <param name="dir">保存的路径，不用传siteHead过来，以/结束，如：Uploadfile/Article/</param>
        /// <param name="r">实例化的随机数</param>
        public static string getFilename(string filepath, string dir, Random r)

        /// <summary>
        /// 转换变量为Int64
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static long ConvertInt64(object str)
        public static long ConvertInt64(object str, int i)

        /// <summary>
        /// 转换变量为Int32
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static int ConvertInt32(object str)
        public static int ConvertInt32(object str, int i)

        /// <summary>
        /// 转换变量为Int16
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static int ConvertInt16(object str)
        public static int ConvertInt16(object str, int i)

        /// <summary>
        /// 转换变量为Single
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static float ConvertSingle(object str)
        public static float ConvertSingle(object str, int i)

        /// <summary>
        /// 转换变量为DateTime
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static DateTime ConvertDateTime(object str)
        public static DateTime ConvertDateTime(object str, DateTime i)

        /// <summary>
        /// 转换变量为Decimal
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static Decimal ConvertDecimal(object str)
        public static Decimal ConvertDecimal(object str, Decimal i)

        /// <summary>
        /// 转换变量为String
        /// </summary>
        /// <param name="str">object</param>
        /// <param name="i">catch返值</param>
        public static string ConvertString(object str)
        public static string ConvertString(object str, string s)

        /// <summary>    
        /// 在图片上生成图片水印
        /// </summary>    
        /// <param name="Path">原服务器图片路径</param>   
        /// <param name="Path_syp">生成的带图片水印的图片路径</param>   
        /// <param name="Path_sypf">水印图片路径</param>
        /// <param name="position">水印位置1-左上，2-中上，3-右上，4-左中，5-中间，6-右中，7-左下，8-中下，9-右下 </param>
        /// <param name="padding">边距:默认10像素</param>
        public static void addWaterMark(string Path, string Path_syp, string Path_sypf, int position, int padding)

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
        
        /// <summary>
        /// 判断逗号分隔的主码中是否含有某一主码
        /// </summary>
        /// <param name="str">被比较字符串。可以为逗号分隔的多个主码，也可以是尖括号包围的多个主码</param>
        /// <param name="Pat">条件主码</param>
        public static bool checkHave(string str, string Pat)

        /// <summary>
        /// 更换原密码MD5加密后的字符
        /// </summary>
        /// <param name="Passwd">原密码，客户端接收。</param>
        /// <returns></returns>
        public static string updatePasswd(string Passwd)

        /// <summary>
        /// 服务器端request请求
        /// </summary>
        /// <param name="url">服务器端请求页面地址。http全路径，可以带地址栏参数</param>
        /// <param name="method">请求方法，GET/POST</param>
        /// <param name="Para">post参数，一个字符串</param>
        /// <param name="cert_path">证书物理路径。如：@"e:\\abc.pfx" 或 Server.MapPath("/abc.pfx")</param>
        /// <param name="cert_passwd">证书密码</param>
        public static string getXMLHTTP(string url, string method)
        public static string getXMLHTTP(string url, string method, string Para)
        public static string getXMLHTTP(string url, string method, string Para, string cert_path, string cert_passwd)
        
        /// <summary>
        /// 格式化价格（整数只显示整数部分；小数忽略最后的0；最多显示两位小数）
        /// </summary>
        /// <param name="Price">价格</param>
        /// <returns></returns>
        public static string formatPrice(string Price)

        /// <summary>
        /// 格式化价格保留2位小数，超过的位数不要了 如1.126545454 结果是 1.12
        ///  malongjun
        /// </summary>
        /// <param name="Price"></param>
        /// <returns></returns>
        public static string FormatPrice_ellipsis(string Price)

        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <param name="dt">日期。注意时差，可以用DateTime.UtcNow</param>
        public static string create_timestamp(DateTime dt)

        /// <summary>
        /// 将base64格式的图片保存到指定文件夹中
        /// </summary>
        /// <param name="filepath">要将图片保存的路径</param>
        /// <param name="base64">base64编码</param>
        public static void Base64StringToImage(string filepath, string base64)

        /// <summary>
        /// Base64 encrypt.
        /// </summary>
        /// <param name="sourthUrl">源码</param>
        public static string Base64Encrypt(string sourthUrl)

        /// <summary>
        /// Base64s decrypt.
        /// </summary>
        /// <param name="eStr">编过的码</param>
        public static string Base64Decrypt(string eStr)

        /// <summary>
        /// 是否是Base64字符串
        /// </summary>
        /// <param name="eStr"></param>
        /// <returns></returns>
        public static bool IsBase64(string eStr)

        /// <summary>
        /// 验证逗号或竖线分隔或尖括号包裹的主键串均为数字类型，不是的过滤掉。都不是的话返回空字符串
        /// </summary>
        /// <param name="pk">主键字符串，可逗号或竖线分隔，也可用尖括号包裹</param>
        public static string returnConvertPK(string pk)

        /// <summary>
        /// 验证签名方法
        /// </summary>
        /// <param name="signature">签名</param>
        /// <param name="non_str">随机数</param>
        /// <param name="stamp">时间戳</param>
        /// <param name="ParameterList">拼接好的参数键值对</param>
        /// <param name="IsOperationDB">需要验证数据库记录。1-不验证 0-验证</param>
        public static string VerifySigned(string signature, string non_str, string stamp, List<string> ParameterList)
        public static string VerifySigned(string signature, string non_str, string stamp, List<string> ParameterList, int IsOperationDB)
        
        /// <summary>
        /// 接口输出错误码到页面
        /// </summary>
        /// <param name="resultCode">错误码</param>

        /// <summary>
        /// 帮助BLL层处理参数，有总参数的话，拆分主扩&补充主参数；没有总参数则补充主参数
        /// </summary>
        /// <param name="P_total">总参数，不为空时拆分为主参数和扩展参数</param>
        /// <param name="P_main">主参数，总参数为空时必填，总参数不为空时没用</param>
        /// <param name="P_extend">扩展参数，总参数不为空时没用</param>
        /// <param name="demand">扩展参数和需要主参数  ｛"扩a,主1,主2","扩b,主1","扩c,主1","扩f,主1"｝</param>
        public static string[] Parameters_Filter(string P_total, string P_main, string P_extend, string[] demand)
        
        /// <summary>
        /// 获得Ip k=0 验证安卓和苹果手机，获取ip方法不一样 1-不验证 ip从 ServerVariables["HTTP_X_FORWARDED_FOR"] 中获取
        /// </summary>
        /// <param name="k"></param>
        public static string getIp()
        public static string getIp(int k)

        /// <summary>  
        /// 对二维数组排序————可能是刘超？
        /// </summary>  
        /// <param name="values">排序的二维数组</param>  
        /// <param name="orderColumnsIndexs">排序根据的列的索引号数组</param>  
        /// <param name="type">排序的类型，1代表降序，0代表升序</param>  
        /// <returns>返回排序后的二维数组</returns>  
        public static object[,] Orderby(object[,] values, int[] orderColumnsIndexs, int type)

        /// <summary>  
        /// 获取二维数组中一行的数据————可能是刘超？ 
        /// </summary>  
        /// <param name="values">二维数据</param>  
        /// <param name="rowID">行ID</param>  
        /// <returns>返回一行的数据</returns>  
        static object[] GetRowByID(object[,] values, int rowID)

        /// <summary>  
        /// 复制一行数据到二维数组指定的行上————可能是刘超？
        /// </summary>  
        /// <param name="values"></param>  
        /// <param name="rowID"></param>  
        /// <param name="row"></param>  
        static void CopyToRow(object[,] values, int rowID, object[] row)
        
        /// <summary>
        /// 利用DES加密算法加密字符串（可解密） 
        /// </summary>
        /// <param name="plaintext">被加密的字符串</param>
        /// <param name="key">密钥（只支持8个字节的密钥）</param>
        /// <param name="iv">向量</param>
        public static string EncryptString(string plaintext, string key, string iv)

        /// <summary>
        /// 利用DES解密算法解密密文
        /// </summary>
        /// <param name="ciphertext">被解密的字符串</param>
        /// <param name="key">密钥（只支持8个字节的密钥，同前面的加密密钥相同）</param>
        /// <param name="iv">向量</param>
        
更新日志：
-------------
v1.1.1

        1. 创建项目并发布到github
        2. 发布到npm
