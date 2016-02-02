using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Windows.Forms;
using System.Threading;
using System.IO.Compression;

///
/// 各种html下载工具
///
namespace FastCase_WebExtand
{
    /// <summary>
    /// 由各种采集相关函数所封装的类
    /// </summary>
    public class Ajax
    {
        #region private attributes
        private Boolean _AppendCss = true;
        private Boolean _AddFullPath = true;
        private WebProxy _Proxy = null;
        private string _Agent = "Mozilla/5.0 (Windows NT 5.1; rv:11.0) Gecko/20100101 Firefox/11.0";//Firefox
        // private string _Agent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
        //private string _Agent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";//IE 11
        private Boolean _error = false;
        private string _message = "";
        public string _result = "";
        private Boolean _event = false;
        private Encoding _charset = Encoding.UTF8;
        private CookieCollection _cookies = new CookieCollection();
        private Boolean _hasfilename = true;
        private DirectoryInfo _root = null;
        private Boolean _autosave = false;
        private Boolean _autoupdate = false;
        private string _filename = null;
        private string _host = null;
        private Boolean _autoEncoding = true;
        private Boolean _isUpdated = false;
        private Dictionary<string, string> _post = new Dictionary<string, string>();
        private string _action = null;
        private string _ForceEncoding = string.Empty;
        private bool _IsHaveCertification = false;
        //public HttpResult result;
        #endregion

        /// <summary>
        /// 可以用来捕捉采集完成的事件
        /// </summary>
        public event EventHandler Completed;

        #region public attributes

        /// <summary>
        /// 获取或设置一个值，表示当前Post方法采集时，action指向的地址
        /// </summary>
        public string FormAction
        {
            get
            {
                return _action;
            }
            set
            {
                _action = value;
            }
        }
        /// <summary>
        /// 获取一个值，列出当前用以POST方法的参数集合
        /// </summary>
        public Dictionary<string, string> PostData
        {
            get
            {
                return _post;
            }
        }
        /// <summary>
        /// 获取当前用以提交的表单的元素名集合
        /// </summary>
        public string[] PostDataKeys
        {
            get
            {
                string[] array = new string[_post.Keys.Count];
                _post.Keys.CopyTo(array, 0);
                return array;
            }
        }
        /// <summary>
        /// 采集网站内容时，参数提交使用的方法
        /// </summary>
        public enum Method
        {
            /// <summary>
            /// 使用 GET 方法向网站提交数据
            /// </summary>
            GET,
            /// <summary>
            /// 使用 POST 方法向网站提交数据
            /// </summary>
            POST
        };
        /// <summary>
        /// 获取或设置一个值，该值表示在采集时，是否将不完整的 Url 地址进行补全
        /// </summary>
        public Boolean AddFullPath
        {
            get
            {
                return _AddFullPath;
            }
            set
            {
                _AddFullPath = value;
            }
        }
        /// <summary>
        /// 获取或设置一个值，该值表示在采集时，是否追加采集样式信息
        /// </summary>
        public Boolean AppendCss
        {
            get
            {
                return _AppendCss;
            }
            set
            {
                _AppendCss = value;
            }
        }
        /// <summary>
        /// 获取或设置一个值，该值表示在采集时，所使用的代理地址
        /// </summary>
        public WebProxy Proxy
        {
            get
            {
                return _Proxy;
            }
            set
            {
                _Proxy = value;
            }
        }
        /// <summary>
        /// 获取或设置一个值，该值表示在采集时，所模拟的浏览器版本信息几系统信息
        /// </summary>
        public string Agent
        {
            get
            {
                return _Agent;
            }
            set
            {
                _Agent = value;
            }
        }
        /// <summary>
        /// 获取一个值，以描述当前采集的进程是否发生异常
        /// </summary>
        public Boolean Error
        {
            get
            {
                return _error;
            }
        }
        /// <summary>
        /// 获取一个值，以描述当前采集的进程异常的详细说明
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _message;
            }
        }
        /// <summary>
        /// 清除当前发生的错误信息
        /// </summary>
        public void ErrorClear()
        {
            _error = false;
            _message = "";
        }
        /// <summary>
        /// 获取或设置一个值，该值表示在采集完成时，是否发送采集完成事件，如需发送，则必须对该事件进行捕捉
        /// </summary>
        public Boolean EventHandle
        {
            get
            {
                return _event;
            }
            set
            {
                _event = value;
            }
        }
        /// <summary>
        /// 获取当前采集过程中，使用的 Cookie 设置
        /// </summary>
        public CookieCollection Cookies
        {
            get
            {
                return _cookies;
            }
        }
        /// <summary>
        /// 设置或获取一个值，表示当采集到的内容无法正确解析编码时所使用的默认编码
        /// </summary>
        public Encoding Charset
        {
            get
            {
                return _charset;
            }
            set
            {
                _charset = value;
            }
        }
        /// <summary>
        /// 设置或获取一个值，该值表示所采集的网站地址是否包含文件名
        /// </summary>
        public Boolean HasFileName
        {
            get
            {
                return _hasfilename;
            }
            set
            {
                _hasfilename = value;
            }
        }
        /// <summary>
        /// 获取当前采集的 Html 内容
        /// </summary>
        public string Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
            }
        }
        /// <summary>
        /// 设置或获取一个值，该值表示在采集的过程中是否自动将采集到的信息保存到本地
        /// <para>如果值设置为 true，则 RootPath 的值必须有效</para>
        /// </summary>
        public Boolean AutoSave
        {
            get
            {
                return _autosave;
            }
            set
            {
                _autosave = value;
            }
        }
        /// <summary>
        /// 设置或获取一个值，该值表示在自动保存时，如果已经存在本地文件时是否覆盖原有的文件
        /// <para>该设置仅在 AutoSave 为 true 时有效</para>
        /// </summary>
        public Boolean AutoUpdate
        {
            get
            {
                return _autoupdate;
            }
            set
            {
                _autoupdate = value;
            }
        }
        /// <summary>
        /// 设置或获取一个值，该值表示在自动保存时，文件存放的路径
        /// </summary>
        public DirectoryInfo RootPath
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;
            }
        }
        /// <summary>
        /// 获取一个值，该值表示在采集完成时，是否覆盖了原有文件
        /// </summary>
        public Boolean IsUpdated
        {
            get
            {
                return _isUpdated;
            }
        }
        /// <summary>
        /// 设置或获取一个值，该值表示在采集页面时，是否自动根据返回的内容设置编码
        /// </summary>
        public Boolean AutoEncoding
        {
            get
            {
                return _autoEncoding;
            }
            set
            {
                _autoEncoding = value;
            }
        }
        public string Filename
        {
            get
            {
                return _filename;
            }
        }

        /// <summary>
        /// 强制设置页面编码
        /// </summary>
        public string ForceEncoding
        {
            get
            {
                return _ForceEncoding;
            }
            set
            {
                _ForceEncoding = value;
            }
        }

        /// <summary>
        /// 强制绕过证书
        /// </summary>
        public bool IsHaveCertification
        {
            get
            {
                return _IsHaveCertification;
            }
            set
            {
                _IsHaveCertification = value;
            }
        }
        #endregion

        #region static method
        /// <summary>
        /// 将文本保存至指定的文件的静态方法
        /// </summary>
        /// <param name="file">所要保存的文件位置，一个包含完整路径的文件名</param>
        /// <param name="html">所要保存的文本内容</param>
        public static void Save(FileInfo file, string html)
        {
            string filepath = file.FullName;
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1, filepath.Length - filepath.LastIndexOf("\\") - 1);
            string pathinfo = filepath.Substring(0, filepath.LastIndexOf("\\"));
            Save(new DirectoryInfo(pathinfo), new FileInfo(filename), html);
        }
        /// <summary>
        /// 将文本保存至指定的文件的静态方法
        /// </summary>
        /// <param name="path">所要保存的位置，该信息不包含文件名信息</param>
        /// <param name="file">所要保存的文件名，不能包含任何路径信息</param>
        /// <param name="html">所要保存的文本内容</param>
        public static void Save(DirectoryInfo path, FileInfo file, string html)
        {
            string filename = file.Name;
            string filepath = path.FullName;
            if (!path.Exists)
            {
                string[] dirs = filepath.Split('\\');
                string md = dirs[0] + "\\" + dirs[1];
                for (int i = 1; i < dirs.Length; i++)
                {
                    if (!Directory.Exists(md))
                    {
                        Directory.CreateDirectory(md);
                    }
                    if (i < dirs.Length - 1)
                    {
                        md += "\\" + dirs[i + 1];
                    }
                }
            }
            StreamWriter sw = new StreamWriter(filepath + "\\" + filename, false, System.Text.Encoding.UTF8);
            sw.Write(html);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        #endregion

        #region private function
        /// <summary>
        /// 对采集到的内容进行 Url 补全
        /// </summary>
        /// <param name="url">所采集目标网站的 Uri 对象</param>
        /// <param name="html">所采集到的 HTML 文本</param>
        /// <returns></returns>
        private string UrlComplemented(Uri url, string html)
        {
            string result = html;
            string type = url.Scheme;
            string domain = url.Host;
            string script = url.LocalPath;
            string query = url.Query;
            string[] pathlist = url.Segments;
            string rootpath = type + "://" + domain + "/";
            string currpath = rootpath;
            string parentpath = rootpath;
            for (int i = 1; i < ((_hasfilename) ? (pathlist.Length - 1) : pathlist.Length); i++)
            {
                currpath += pathlist[i];
                if (i > 1)
                {
                    parentpath += pathlist[i - 1];
                }
            }
            // 特例，meta 跳转的 url 补全
            if (new Regex(@"<meta[^<>]*?http-equiv=['""]?refresh['""]?[^<>]*?>", RegexOptions.IgnoreCase).IsMatch(result))
            {
                result = new Regex(@"(?<=<meta[^<>]*?)(?<!(&|\?))url=(['""]?)/([^<>]*?)\2(?=[\s>'""])", RegexOptions.IgnoreCase).Replace(result, @"url=""" + rootpath + @"$3""");
                result = new Regex(@"(?<=<meta[^<>]*?)(?<!(&|\?))url=(?!['""]?[a-z]+://)(['""]?)(?:(?:\./)?)([^<>]*?)\2(?=[\s>'""])", RegexOptions.IgnoreCase).Replace(result, @"url=""" + currpath + @"$3""");
                result = new Regex(@"(?<=<meta[^<>]*?)(?<!(&|\?))url=(['""]?)\.\.\/([^<>]*?)\2(?=[\s>'""])", RegexOptions.IgnoreCase).Replace(result, @"url=""" + parentpath + @"$3""");
            }
            string[] replaceList = new string[] { "href", "src", "action", "url" };
            for (int i = 0; i < replaceList.Length; i++)
            {
                result = new Regex(@"(?<!(?:&|\?|[\w-]+=['""][^'""]+))" + replaceList[i] + @"=(['""]?)/([^<>]*?)\1(?=[\s>'""])", RegexOptions.IgnoreCase).Replace(result, replaceList[i] + @"=""" + rootpath + @"$2""");
                result = new Regex(@"(?<!(?:&|\?|[\w-]+=['""][^'""]+))" + replaceList[i] + @"=(?!['""]?#)(?!['""]?[\w]+:)(?!['""]?[a-z]+://)(['""]?)(?:(?:\./)?)([^<>]*?)\1(?=[\s>'""])", RegexOptions.IgnoreCase).Replace(result, replaceList[i] + @"=""" + currpath + @"$2""");
                result = new Regex(@"(?<!(?:&|\?|[\w-]+=['""][^'""]+))" + replaceList[i] + @"=(['""]?)\.\.\/([^<>]*?)\1(?=[\s>'""])", RegexOptions.IgnoreCase).Replace(result, replaceList[i] + @"=""" + parentpath + @"$2""");
            }
            return result;
        }
        /// <summary>
        /// 对所采集到的内容进行 Css 补全
        /// </summary>
        /// <param name="html">所采集到的 HTML 内容</param>
        /// <returns></returns>
        private string CssAppend(string html)
        {
            string result = html;
            _AppendCss = false;
            MatchCollection csslist = new Regex(@"(?<=<link(?=[^<>]*?rel=['""]?stylesheet['""]?)[^<>]*?href=['""]?)[^<>'""]*?(?=['""]?(\s[^<>]*?>|>))", RegexOptions.IgnoreCase).Matches(result);
            for (int i = 0; i < csslist.Count; i++)
            {
                string css = Http(csslist[i].Value);
                if (new Regex(@"<style[^<>]*?>[\s\S]*?</style>", RegexOptions.IgnoreCase).IsMatch(result))
                {
                    result = new Regex(@"<style[^<>]*?>", RegexOptions.IgnoreCase).Replace(result, "<style>\r\n\r\n/* ------------------------------- */\r\n/* " + csslist[i].Value + " */\r\n/* ------------------------------- */\r\n\r\n" + css + "\r\n\r\n/* ------------------------------- */\r\n/* Css Append End */\r\n/* ------------------------------- */\r\n\r\n", 1);
                }
                else
                {
                    result = "<style>\r\n\r\n/* ------------------------------- */\r\n/* " + csslist[i].Value + " */\r\n/* ------------------------------- */\r\n\r\n" + css + "\r\n</style>" + result;
                }
            }
            _AppendCss = true;
            return result;
        }
        #endregion

        public static Dictionary<string, string> SpiderHistory = new Dictionary<string, string>();

        public string encode(Match match)
        {
            switch (match.Value)
            {
                case "\\":
                case "/":
                case "\"":
                case "<":
                case ">":
                case "?":
                case "*":
                case ":":
                case "|":
                    byte[] b = ASCIIEncoding.ASCII.GetBytes(match.Value);
                    return "%" + Dec2Hex(b[0]);
                default:
                    break;
            }
            return match.Value;
        }

        public string Dec2Hex(int n)
        {
            if (n > 15)
            {
                int m = (int)(n - Math.Floor(((double)n / 16) * 16));
                n = n / 16;
                return Dec2Hex(n) + Dec2Hex(m);
            }
            else
            {
                if (n > 9)
                {
                    return Convert.ToString(65 + n - 10);
                }
                else
                {
                    return n.ToString();
                }
            }
        }
        #region public method
        /// <summary>
        /// 设置或修改一个 Cookie 项目的值
        /// </summary>
        /// <param name="uri">需要伪造的 Cookie 的 URI</param>
        /// <param name="key">Cookie 名</param>
        /// <param name="value">Cookie 值</param>
        public void SetCookie(Uri uri, string key, string value)
        {
            CookieContainer cc = new CookieContainer();
            cc.Add(uri, new Cookie(key, value));
            foreach (Cookie cookie in cc.GetCookies(uri))
            {
                _cookies.Add(cookie);
            }
        }
        /// <summary>
        /// 设置或修改一个 Cookie 项目的值
        /// </summary>
        /// <param name="key">Cookie 关键字</param>
        /// <param name="value">Cookie 值</param>
        public void SetCookie(string key, string value, string domain = "", Uri commenturi = null)
        {
            Cookie cookie = new Cookie(key, value);
            cookie.Expires = DateTime.Now.AddDays(1);
            if (domain.Length > 0)
            {
                cookie.Domain = domain;
            }
            if (commenturi != null)
            {
                cookie.CommentUri = commenturi;
                cookie.Domain = commenturi.Host;
                //cookie.Port = commenturi.Port.ToString();
            }
            if (_cookies.Count > 0)
            {
                Boolean exists = false;
                foreach (Cookie ck in _cookies)
                {
                    if (ck.Name == cookie.Name && ck.Domain == cookie.Domain)
                    {
                        ck.Value = cookie.Value;
                        cookie = ck;
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    foreach (Cookie ck in _cookies)
                    {
                        //cookie.Domain = ck.Domain;
                        cookie.Path = ck.Path;
                        cookie.Port = ck.Port;
                        cookie.Secure = ck.Secure;
                        cookie.Version = ck.Version;
                        //cookie.CommentUri = ck.CommentUri;
                        cookie.Comment = ck.Comment;
                        cookie.Discard = ck.Discard;
                        break;
                    }
                }
            }
            _cookies.Add(cookie);
        }
        /// <summary>
        /// 删除一个 Cookie 项目的内容
        /// </summary>
        /// <param name="key">Cookie 关键字</param>
        public void DelCookie(string key)
        {
            if (_cookies.Count > 0)
            {
                CookieCollection cc = new CookieCollection();
                foreach (Cookie cookie in _cookies)
                {
                    if (cookie.Name != key)
                    {
                        cc.Add(cookie);
                    }
                }
                _cookies = cc;
            }
        }
        /// <summary>
        /// 将现有的所有 Cookie 清除
        /// </summary>
        public void ClearCookie()
        {
            _cookies = new CookieCollection();
        }
        /// <summary>
        /// 将当前采集到的信息保存到本地文件中
        /// </summary>
        /// <param name="file">所采集到的内容要保存的文件位置，一个包含完整路径的文件名</param>
        public void Save(FileInfo file)
        {
            string filepath = file.FullName;
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1, filepath.Length - filepath.LastIndexOf("\\") - 1);
            string pathinfo = filepath.Substring(0, filepath.LastIndexOf("\\"));
            Save(new DirectoryInfo(pathinfo), new FileInfo(filename));
        }
        /// <summary>
        /// 将当前采集到的信息保存到本地文件中
        /// </summary>
        /// <param name="path">所采集到的内容要保存的位置，该信息不包含文件名信息</param>
        /// <param name="file">所采集到的内容要保存的文件名，不能包含任何路径信息</param>
        public void Save(DirectoryInfo path, FileInfo file)
        {
            string filename = file.Name;
            string filepath = path.FullName;
            if (!path.Exists)
            {
                string[] dirs = filepath.Split('\\');
                string md = dirs[0] + "\\" + dirs[1];
                for (int i = 1; i < dirs.Length; i++)
                {
                    if (!Directory.Exists(md))
                    {
                        Directory.CreateDirectory(md);
                    }
                    if (i < dirs.Length - 1)
                    {
                        md += "\\" + dirs[i + 1];
                    }
                }
            }
            StreamWriter sw = new StreamWriter(filepath + "\\" + filename, false, System.Text.Encoding.UTF8);
            sw.Write(_result);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        /// <summary>
        /// 将本地文件加载到类中
        /// </summary>
        /// <param name="file">包含完整路径的本地文件名</param>
        public void Load(FileInfo file)
        {
            if (file.Exists)
            {
                StreamReader sr = new StreamReader(file.FullName, Encoding.UTF8);
                _result = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
            }
        }
        /// <summary>
        /// 根据当前采集到的页面，生成指定顺序的表单内容，暂时只支持 Input Type Text|Password|Hidden|Button 及 Select Single，尚未支持的包括 Input Type Radio|CheckBox|File、Select Multi 及 TextArea
        /// </summary>
        /// <param name="no">指定表单的序号</param>
        public void BuildForm(int no = 0, string html = null)
        {
            string data = (html == null) ? Result : html;
            _action = null;
            _post.Clear();
            MatchCollection forms = new Regex(@"<form(?!\w)[\s\S]*?</form\s*?>", RegexOptions.IgnoreCase).Matches(data);
            if (forms.Count == 0)
            {
                return;
            }
            if (no >= forms.Count)
            {
                return;
            }
            string form = forms[no].Value;
            _action = new Regex(@"(?<=<form(?!\w)[^<>]*)(?<=action="")[^""]*(?="")", RegexOptions.IgnoreCase).Match(form).Value;
            MatchCollection inputs = new Regex(@"<input[^<>]*?>|<select[\s\S]*?</select\s*?>", RegexOptions.IgnoreCase).Matches(form);
            for (int j = 0; j < inputs.Count; j++)
            {
                if (new Regex(@"<select", RegexOptions.IgnoreCase).IsMatch(inputs[j].Value))
                {
                    // 有选中项，则提取选中项的值，否则以第一项为默认值
                    if (new Regex(@"<option(?=[^<>]*?selected)", RegexOptions.IgnoreCase).IsMatch(""))
                    {
                        _post.Add(new Regex(@"(?<=name="")[^<>]*?(?="")", RegexOptions.IgnoreCase).Match(inputs[j].Value).Value, new Regex(@"(?<=<option(?=[^<>]*?selected)[^<>]*?value="")[^<>""]*?(?="")", RegexOptions.IgnoreCase).Match(inputs[j].Value).Value);
                    }
                    else
                    {
                        _post.Add(new Regex(@"(?<=name="")[^<>]*?(?="")", RegexOptions.IgnoreCase).Match(inputs[j].Value).Value, new Regex(@"(?<=<option[^<>]*?value="")[^<>""]*?(?="")", RegexOptions.IgnoreCase).Match(inputs[j].Value).Value);
                    }
                    //postdata += (postdata.Length > 0 ? "&" : "") + new Regex(@"(?<=name="")[^<>]*?(?="")", RegexOptions.IgnoreCase).Match(inputs[j].Value).Value + "=" + new Regex(@"(?<=option(?=[^<>]*?selected)[^<>]*?value="")[^<>""]*?(?="")", RegexOptions.IgnoreCase).Match(inputs[j].Value).Value;
                }
                else
                {
                    _post.Add(new Regex(@"(?<=name="")[^<>]*?(?="")", RegexOptions.IgnoreCase).Match(inputs[j].Value).Value, new Regex(@"(?<=value="")[^<>]*?(?="")", RegexOptions.IgnoreCase).Match(inputs[j].Value).Value);
                    //postdata += (postdata.Length > 0 ? "&" : "") + new Regex(@"(?<=name="")[^<>]*?(?="")", RegexOptions.IgnoreCase).Match(inputs[j].Value).Value + "=" + new Regex(@"(?<=value="")[^<>]*?(?="")", RegexOptions.IgnoreCase).Match(inputs[j].Value).Value.Replace("+", "%2B").Replace("=", "%3D").Replace(" ", "%20").Replace("/", "%2F");    // 
                }
            }
        }
        #endregion

        #region public function
        public string PostForm()
        {
            string post = "";
            foreach (string key in _post.Keys)
            {
                post += (post.Length > 0 ? "&" : "") + key + "=" + _post[key].Replace("+", "%2B").Replace("=", "%3D").Replace(" ", "%20").Replace("/", "%2F");
            }
            return post;
        }
        #endregion

        #region old function
        /// <summary>
        /// 使用 Ajax 模式进行异步获取页面内容
        /// </summary>
        /// <param name="url">所要进行采集的网站的有效 Url 地址</param>
        /// <param name="proxy">如果需要使用代理进行连接，则输入代理地址，null表示不使用代理</param>
        /// <returns></returns>
        public string Request(string url, WebProxy proxy = null)
        {
            string html = "";
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:11.0) Gecko/20100101 Firefox/11.0";
            if (proxy != null)
            {
                wr.Proxy = proxy;
            }
            try
            {
                HttpWebResponse hwr = (HttpWebResponse)wr.GetResponse();
                Stream s = hwr.GetResponseStream();
                StreamReader sr = new StreamReader(s);
                html = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                s.Close();
                s.Dispose();
                hwr.Close();
            }
            catch (Exception ex)
            {
                html = ex.Message;
            }
            return html;
        }
        /// <summary>
        /// 使用 Ajax 模式进行异步获取页面内容
        /// </summary>
        /// <param name="url">所要进行采集的网站的有效 Url 地址</param>
        /// <param name="file">所采集到的内容要保存的文件位置，一个包含完整路径的文件名</param>
        /// <param name="proxy">如果需要使用代理进行连接，则输入代理地址，null表示不使用代理</param>
        /// <returns></returns>
        public string Request(string url, FileInfo file, WebProxy proxy = null)
        {
            string filepath = file.FullName;
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1, filepath.Length - filepath.LastIndexOf("\\") - 1);
            string pathinfo = filepath.Substring(0, filepath.LastIndexOf("\\"));
            return Request(url, new DirectoryInfo(pathinfo), new FileInfo(filename), proxy);
        }
        /// <summary>
        /// 使用 Ajax 模式进行异步获取页面内容
        /// </summary>
        /// <param name="url">所要进行采集的网站的有效 Url 地址</param>
        /// <param name="path">所采集到的内容要保存的位置，该信息不包含文件名信息</param>
        /// <param name="file">所采集到的内容要保存的文件名，不能包含任何路径信息</param>
        /// <param name="proxy">如果需要使用代理进行连接，则输入代理地址，null表示不使用代理</param>
        /// <returns></returns>
        public string Request(string url, DirectoryInfo path, FileInfo file, WebProxy proxy = null)
        {
            string filename = file.Name;
            string filepath = path.FullName;
            string html = Request(url, proxy);
            if (path.Exists)
            {
                StreamWriter sw = new StreamWriter(filepath + "\\" + filename, false, System.Text.Encoding.UTF8);
                sw.Write(html);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            else
            {
                string[] dirs = filepath.Split('\\');
                string md = dirs[0] + "\\" + dirs[1];
                for (int i = 1; i < dirs.Length; i++)
                {
                    if (!Directory.Exists(md))
                    {
                        Directory.CreateDirectory(md);
                    }
                    if (i < dirs.Length - 1)
                    {
                        md += "\\" + dirs[i + 1];
                    }
                }
                StreamWriter sw = new StreamWriter(md + "\\" + filename, false, System.Text.Encoding.UTF8);
                sw.Write(html);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            return html;
        }
        /// <summary>
        /// 使用 Ajax 模式进行异步获取页面内容
        /// </summary>
        /// <param name="url">所要进行采集的网站的有效 Url 地址</param>
        /// <param name="file">所采集到的内容要保存的文件位置，一个包含完整路径的文件名</param>
        /// <param name="update">设置是否更新本地数据，如果不需要更新，并且存在本地文件，则直接返回本地文件内容</param>
        /// <param name="proxy">如果需要使用代理进行连接，则输入代理地址，null表示不使用代理</param>
        /// <returns></returns>
        public string Request(string url, FileInfo file, Boolean update, WebProxy proxy = null)
        {
            if (file.Exists)
            {
                if (update)
                {
                    return Request(url, file, proxy);
                }
                else
                {
                    StreamReader sr = new StreamReader(file.FullName, System.Text.Encoding.UTF8);
                    string html = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                    return html;
                }
            }
            else
            {
                return Request(url, file, proxy);
            }
        }
        /// <summary>
        /// 使用 Ajax 模式进行异步获取页面内容
        /// </summary>
        /// <param name="url">所要进行采集的网站的有效 Url 地址</param>
        /// <param name="method">所要进行采集时使用的采集方式</param>
        /// <param name="elements">在采集时，传递给网站的参数，GET模式为QueryString，POST模式为所提交的表单元数据</param>
        /// <param name="proxy">如果需要使用代理进行连接，则输入代理地址，null表示不使用代理</param>
        /// <param name="file">所采集到的内容要保存的文件位置，一个包含完整路径的文件名，如果值为 null 则表示不保存至本地</param>
        /// <param name="update">设置是否更新本地数据，如果不需要更新，并且存在本地文件，则直接返回本地文件内容</param>
        /// <returns></returns>
        public string Request(string url, Method method, string elements, WebProxy proxy = null, FileInfo file = null, Boolean update = true)
        {
            if (method == Method.GET)
            {
                string uri = (url.IndexOf("?") > 0) ? (url + ((elements.Length > 0) ? ("&" + elements) : "")) : (url + ((elements.Length > 0) ? ("?" + elements) : ""));
                if (file == null)
                {
                    return Request(uri, proxy);
                }
                else
                {
                    return Request(uri, file, update, proxy);
                }
            }
            else
            {
                ServicePointManager.Expect100Continue = false;
                HttpWebRequest post = (HttpWebRequest)WebRequest.Create(url);
                post.Method = "POST";
                post.Accept = "application/json, text/javascript, */*";
                post.KeepAlive = true;
                post.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:11.0) Gecko/20100101 Firefox/11.0";
                if (proxy != null)
                {
                    post.Proxy = proxy;
                }
                byte[] pd = new System.Text.ASCIIEncoding().GetBytes(elements);
                post.ContentType = "application/x-www-form-urlencoded";
                post.ContentLength = pd.Length;
                Stream ps = post.GetRequestStream();
                ps.Write(pd, 0, pd.Length);
                ps.Close();
                ps.Dispose();
                string html = "";
                try
                {
                    HttpWebResponse hwr = (HttpWebResponse)post.GetResponse();
                    Stream ss = hwr.GetResponseStream();
                    StreamReader sr = new StreamReader(ss);
                    string text = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                    ss.Close();
                    ss.Dispose();
                    hwr.Close();
                    html = text;
                }
                catch (Exception ex)
                {
                    html = ex.Message;
                }
                if (file != null)
                {
                    string filepath = file.FullName;
                    string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1, filepath.Length - filepath.LastIndexOf("\\") - 1);
                    string pathinfo = filepath.Substring(0, filepath.LastIndexOf("\\"));
                    if (!Directory.Exists(pathinfo))
                    {
                        string[] dirs = pathinfo.Split('\\');
                        string md = dirs[0] + "\\" + dirs[1];
                        for (int i = 1; i < dirs.Length; i++)
                        {
                            if (!Directory.Exists(md))
                            {
                                Directory.CreateDirectory(md);
                            }
                            if (i < dirs.Length - 1)
                            {
                                md += "\\" + dirs[i + 1];
                            }
                        }
                    }
                    StreamWriter sw = new StreamWriter(pathinfo + "\\" + filename, false, System.Text.Encoding.UTF8);
                    sw.Write(html);
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
                return html;
            }
        }
        #endregion

        /// <summary>
        /// 进行异步数据采集的方法，支持 Cookie，Proxy，Post，EventHandler，Url补全，Css补全，Css替换，正文提取
        /// </summary>
        /// <param name="url">所要进行采集的网站的有效 Url 地址</param>
        /// <param name="method">提交数据所使用的方法</param>
        /// <param name="QueryString">作为提交的参数集合</param>
        /// <returns></returns>
        public string Http(string url, Method method = Method.GET, string QueryString = "", FileFilter fileFilter = null)
        {
            _message = null;
            _result = "";
            _error = false;
            _isUpdated = false;
        Redo:
            string filename = "";
            string full_url = ((Method.GET == method) ? ((QueryString.Length > 0) ? ((url.IndexOf("?") > 0) ? (url + "&" + QueryString) : (url + "?" + QueryString)) : url) : url);
            ServicePointManager.Expect100Continue = false;
            string html = "";
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(Uri.EscapeUriString(full_url));
            // http.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.None;// 是否执行压缩返回
            if (_IsHaveCertification)
            {
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                http = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                http.Method = "GET";
            }
            #region autosave filename settings
            if (_autosave)
            {
                if (_root == null)
                {
                    _error = true;
                    _message = "You must set a directory to save the file when your autosave setting is true.";
                    return null;
                }
                Uri uri = http.RequestUri;
                filename = _root.FullName + "\\" + uri.Host + uri.LocalPath.Replace("/", "\\");
                string fileName_Temp = filename;
                //string param = new Regex("(?<=(" + uri.LocalPath + "))[\\s\\S]*", RegexOptions.IgnoreCase).Match(uri.ToString()).Value.Trim();
                //filename = _root.FullName + "\\" + uri.Host + uri.LocalPath.Replace("/", "\\");
                //if (!String.IsNullOrEmpty(param))
                //{
                //    param = param.Replace("/", "\\").Replace("?", "\\");
                //    filename += param;
                //}


                string query = new Regex(@"^\?", RegexOptions.IgnoreCase).Replace(uri.Query, "").Trim();
                Dictionary<string, string> queryDict = new Dictionary<string, string>();
                MatchCollection queryList = new Regex(@"(^|(?<=&))[^&=;]+=[\s\S]*?(?=($|&[^&=;]+=))", RegexOptions.IgnoreCase).Matches(query);
                for (int i = 0; i < queryList.Count; i++)
                {
                    queryDict.Add(new Regex(@"^[^&=;]+(?==)", RegexOptions.IgnoreCase).Match(queryList[i].Value).Value, new Regex(@"(?<==)[\s\S]*", RegexOptions.IgnoreCase).Match(queryList[i].Value).Value);
                }
                if (queryList.Count + PostData.Count > 0)
                {
                    filename += "\\";
                    if (fileFilter != null)
                    {
                        for (int j = 0; j < fileFilter.Filter.Count; j++)
                        {
                            if ((Ajax.Method)((object[])fileFilter.Filter[j])[1] == Ajax.Method.POST)
                            {
                                if (PostData.ContainsKey((string)((object[])fileFilter.Filter[j])[0]))
                                {
                                    filename += (new Regex(@"\\$", RegexOptions.IgnoreCase).IsMatch(filename) ? "" : "&") + (string)((object[])fileFilter.Filter[j])[0] + "=" + new Regex(@"[\\/:\*\?<>""\|]", RegexOptions.IgnoreCase).Replace(PostData[(string)((object[])fileFilter.Filter[j])[0]], new MatchEvaluator(encode));
                                }
                            }
                            else
                            {
                                if (queryDict.ContainsKey((string)((object[])fileFilter.Filter[j])[0]))
                                {
                                    filename += (new Regex(@"\\$", RegexOptions.IgnoreCase).IsMatch(filename) ? "" : "&") + (string)((object[])fileFilter.Filter[j])[0] + "=" + new Regex(@"[\\/:\*\?<>""\|]", RegexOptions.IgnoreCase).Replace(queryDict[(string)((object[])fileFilter.Filter[j])[0]], new MatchEvaluator(encode));
                                }
                            }
                        }
                    }
                    else
                    {
                        filename += new Regex(@"[\\/:\*\?<>""\|]", RegexOptions.IgnoreCase).Replace(query, new MatchEvaluator(encode));
                        for (int j = 0; j < PostDataKeys.Length; j++)
                        {
                            filename += (new Regex(@"\\$", RegexOptions.IgnoreCase).IsMatch(filename) ? "" : "&") + PostDataKeys[j] + "=" + PostData[PostDataKeys[j]];
                        }
                    }
                }
                //if (query.Length > 0)
                //{
                //    filename += "\\" + new Regex(@"[\\/:\*\?<>""\|]", RegexOptions.IgnoreCase).Replace(query, new MatchEvaluator(encode)); //  System.Web.HttpUtility.UrlEncode(query);
                //}
                if (filename == fileName_Temp && !String.IsNullOrEmpty(query))
                {
                    filename = Path.Combine(filename, query);
                    string root = Path.GetPathRoot(filename);
                    filename = Path.Combine(root, filename.Replace(root, "").Replace(":", "_").Replace("?", "_").Replace("<", "_").Replace(">", "_").Replace("|", "_").Trim());
                }
                filename += ".html";
                //while (filename.Length >= 260)
                //{
                //    filename = new Regex(@"(Chapter \d+)[\s\S]*?\\", RegexOptions.IgnoreCase).Replace(filename, "$1\\");
                //    filename = new Regex(@"(title \d+)[\s\S]*?\\", RegexOptions.IgnoreCase).Replace(filename, "$1\\");
                //    filename = filename.Substring(0, filename.LastIndexOf("\\") + 1) + Encryption.SHA1(filename.Substring(filename.LastIndexOf("\\") + 1, filename.Length - filename.LastIndexOf("\\") - 6)) + ".html";
                //}

                _filename = filename;
            }
            #endregion
            _host = http.Host;
            FileInfo fileinfo = null;
            // filename = new Regex(@":", RegexOptions.IgnoreCase).Replace(filename, "_");E:\WorkSpace\BuildProcessTemplates\FastCase Tools\FastCase Tools\Program.cs
            if (filename.Length > 0)
            {
                if (filename.Length >= 260)
                {
                    filename = new Regex(@"(Chapter \d+)[\s\S]*?\\", RegexOptions.IgnoreCase).Replace(filename, "$1\\");
                    filename = new Regex(@"(title \d+)[\s\S]*?\\", RegexOptions.IgnoreCase).Replace(filename, "$1\\");
                }
                fileinfo = new FileInfo(filename);
            }
            if (fileinfo != null)
            {
                if (!_autoupdate && fileinfo.Exists)
                {
                    Load(fileinfo);
                    _isUpdated = true;
                    return _result;
                }
            }
            http.CookieContainer = new CookieContainer();
            if (_cookies.Count > 0)
            {
                http.CookieContainer.Add(_cookies);
            }
            http.UserAgent = _Agent;
            //http.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            http.Accept = "*/*";
            if (_Proxy != null)
            {
                http.Proxy = _Proxy;
            }
            #region post settings
            if (method == Method.POST)
            {
                http.Method = "POST";
                http.Accept = "application/json, text/javascript, */*";
                http.KeepAlive = true;
                byte[] pd = null;
                switch (_charset.EncodingName)
                {
                    case "Unicode (UTF-8)":
                        pd = new UTF8Encoding().GetBytes(QueryString);
                        break;
                    case "US-ASCII":
                        pd = new ASCIIEncoding().GetBytes(QueryString);
                        break;
                    case "Unicode":
                        pd = new UnicodeEncoding().GetBytes(QueryString);
                        break;
                    default:
                        pd = new UTF8Encoding().GetBytes(QueryString);
                        break;
                }
                http.ContentType = "application/x-www-form-urlencoded";
                http.ContentLength = pd.Length;
                Stream ps = http.GetRequestStream();
                ps.Write(pd, 0, pd.Length);
                ps.Close();
                ps.Dispose();
            }
            #endregion
            #region get responseStream
            try
            {
                HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                //result = new HttpResult();
                //#region base
                ////获取StatusCode
                //result.StatusCode = response.StatusCode;
                ////获取StatusDescription
                //result.StatusDescription = response.StatusDescription;
                ////获取Headers
                //result.Header = response.Headers;
                ////获取最后访问的URl
                //result.ResponseUri = response.ResponseUri.ToString();
                ////获取CookieCollection
                //if (response.Cookies != null) result.CookieCollection = response.Cookies;
                ////获取set-cookie
                //if (response.Headers["set-cookie"] != null) result.Cookie = response.Headers["set-cookie"];
                //#endregion
                foreach (Cookie cookie in response.Cookies)
                {
                    _cookies.Add(cookie);
                }
                if (response.Headers.GetValues("Location") != null)
                {
                    if (response.Headers.GetValues("Location")[0].Length > 0)
                    {
                        string jumpto = response.Headers.GetValues("Location")[0];
                    }
                }
                Stream s = response.GetResponseStream();
                if (response.CharacterSet.Length > 0 && _autoEncoding)
                {
                    if (String.IsNullOrEmpty(_ForceEncoding))
                    {
                        _charset = Encoding.GetEncoding(response.CharacterSet);
                    }
                    else
                    {
                        _charset = Encoding.GetEncoding(_ForceEncoding);
                    }
                }

                using (StreamReader sr = new StreamReader(s, _charset))
                {
                    html = sr.ReadToEnd();
                    if (_AddFullPath)
                    {
                        html = UrlComplemented(response.ResponseUri, html);
                    }
                    //if (new Regex(@"<meta(?=[^<>]*?http-equiv=['""]?refresh)(?=[^<>]*?url=['""]?)[^<>]*?>", RegexOptions.IgnoreCase).IsMatch(html))
                    //{
                    //    string dd = html;
                    //    string jumpto = new Regex(@"(?<=<meta[^<>]*)(?<=url=['""]?)(?!['""]).*?(?=['""])", RegexOptions.IgnoreCase).Match(dd).Value;
                    //    if (jumpto.EndsWith("&bhjs=0"))
                    //    {
                    //        jumpto = jumpto.Replace("&bhjs=0", "&bhjs=-1");
                    //    }
                    //    html = Http(jumpto);
                    //}
                    if (_AppendCss)
                    {
                        html = CssAppend(html);
                    }
                    _result = html;
                    sr.Close();
                    sr.Dispose();
                }
                s.Close();
                s.Dispose();
                response.Close();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The operation has timed out") || ex.Message == "The remote server returned an error: (504) Gateway Timeout")
                {
                    goto Redo;
                }
                _error = true;
                _message = ex.Message;
                return null;
            }
            #endregion
            if (_autosave)
            {
                if (_autoupdate || !fileinfo.Exists)
                {
                    Save(fileinfo);
                }
            }
            if (_event)
            {
                Completed(this, new EventArgs());
            }
            return html;
        }

        void web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser web = (WebBrowser)sender;
            System.IO.StreamReader getReader = new System.IO.StreamReader(web.DocumentStream, System.Text.Encoding.GetEncoding(web.Document.Encoding));
            string gethtml = getReader.ReadToEnd();
            _result = gethtml;
            web.Dispose();
        }

        public void DownloadByWebClient(string url, FileInfo file)
        {
            WebClient client = new WebClient();
            if (file.Exists)
            {
                if (!_autoupdate)
                {
                    return;
                }
                else
                {
                    File.Delete(file.FullName);
                }
            }
            client.DownloadFile(url, file.FullName);
            client.Dispose();
            //client.
        }

        public void DownloadByWebClientAsyn(string url, FileInfo file)
        {
            WebClient client = new WebClient();
            if (file.Exists)
            {
                if (!_autoupdate)
                {
                    return;
                }
                else
                {
                    File.Delete(file.FullName);
                }
            }
            client.DownloadFileAsync(new Uri(url), file.FullName, file);
            //client.
        }

        /// <summary>
        /// 下载指定的链接，并将采集到的信息存放到本地指定的文件
        /// </summary>
        /// <param name="url">所要进行下载的链接诶地址</param>
        /// <param name="file">存放下载内容的文件信息</param>
        public void Download(string url, FileInfo file)
        {
            _isUpdated = false;
            ServicePointManager.Expect100Continue = false;
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url);
            if (_IsHaveCertification)
            {
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                http = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                http.Method = "GET";
            }

            if (file.Exists)
            {
                if (!_autoupdate)
                {
                    return;
                }
                else
                {
                    File.Delete(file.FullName);
                }
            }
            http.CookieContainer = new CookieContainer();

            if (_cookies.Count > 0)
            {
                http.CookieContainer.Add(_cookies);
            }
            http.UserAgent = _Agent;
            if (_Proxy != null)
            {
                http.Proxy = _Proxy;
            }
            try
            {
                HttpWebResponse hwr = (HttpWebResponse)http.GetResponse();
                Stream s = hwr.GetResponseStream();
                FileStream fs = new FileStream(file.FullName, FileMode.CreateNew);
                fs.SetLength(hwr.ContentLength);
                int bufferlength = 1024 * 256;
                long sum = 0;
                while (sum < hwr.ContentLength)
                {
                    byte[] bytes = new byte[bufferlength];
                    int readbytes = s.Read(bytes, 0, bytes.Length);
                    if (readbytes == 0)
                    {
                        if (sum < hwr.ContentLength)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    fs.Write(bytes, 0, readbytes);
                    sum += readbytes;
                }
                fs.Flush();
                fs.Close();
                fs.Dispose();
                s.Close();
                s.Dispose();
                hwr.Close();
            }
            catch (Exception ex)
            {
                _error = true;
                _message = ex.Message;
            }
        }
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开 
            return true;
        }       

        public class FileFilter
        {
            private List<object[]> _list = new List<object[]>();

            public List<object[]> Filter
            {
                get
                {
                    return _list;
                }
            }

            public void Clear()
            {
                _list.Clear();
            }

            public void Add(string key, Ajax.Method method)
            {
                Boolean isExists = false;
                for (int i = 0; i < _list.Count; i++)
                {
                    object[] item = _list[i];
                    if ((string)item[0] == key && (Ajax.Method)item[1] == method)
                    {
                        isExists = true;
                        break;
                    }
                }
                if (!isExists)
                {
                    _list.Add(new object[] { key, method });
                }
            }

            public void Remove(string key, Ajax.Method method)
            {
                int itemNo = -1;
                for (int i = 0; i < _list.Count; i++)
                {
                    object[] item = _list[i];
                    if ((string)item[0] == key && (Ajax.Method)item[1] == method)
                    {
                        itemNo = i;
                        break;
                    }
                }
                if (itemNo > -1)
                {
                    _list.RemoveAt(itemNo);
                }
            }
        }
    }
}
