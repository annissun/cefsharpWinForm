using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using hap = HtmlAgilityPack;
using CefSharp.WinForms;
using CefSharp;
using System.Threading.Tasks;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private object EvaluateJavaScriptResult;

        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            SetWebBrowserFeatures(11);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // webBrowser1
            // 
            this.webBrowser1 = new ExtChromiumBrowser("http://www.22gvb.com/");
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.FrameLoadEnd += Browser_FrameLoadEnd; //加载结束监听
            this.webBrowser1.StartNewWindow += Browser_StartNewWindow; //打开新窗口监听
            this.webBrowser1.Size = new System.Drawing.Size(1008, 657);
            this.webBrowser1.TabIndex = 0;            // 
            this.panel1.Controls.Add(this.webBrowser1);
        }

        /// <summary>  
        /// 打卡新窗口时候，在当前窗口打开，将URL显示出来 
        ///   
        /// </summary>  
        private void Browser_StartNewWindow(object sender, NewWindowEventArgs e)
        {
            this.textBox1.Text = e.url;
            this.webBrowser1.Load(e.url);
        }

        /// <summary> 
        /// 页面加载结束
        /// 
        /// <summary> 
        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            this.textBox1.Text = this.webBrowser1.GetBrowser().MainFrame.Url;
        }

        /// <summary> 
        ///访问按钮点击
        ///
        /// <summary> 
        private void Button1_Click(object sender, EventArgs e)
        {
            //自动填充帐号密码
            //js调用方式2，不用获取返回值
            this.webBrowser1.GetBrowser().MainFrame.ExecuteJavaScriptAsync("document.getElementsByClassName('_2qJn7')[0].value='wzbb2019'");
            this.webBrowser1.GetBrowser().MainFrame.ExecuteJavaScriptAsync("document.getElementsByClassName('_1VA9m')[0].value='555666888'");
            //this.webBrowser1.Load(this.textBox1.Text);
        }

        /// <summary> 
        ///解析按钮点击
        ///
        /// <summary> 
        private void Button2_Click(object sender, EventArgs e)
        {
            GetHtmlDoc();
        }

        /// <summary> 
        ///获取网页html数据
        ///
        /// <summary> 
        private void GetHtmlDoc()
        {
            var docment = this.webBrowser1.GetSourceAsync();
            docment.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    string resultStr = t.Result;
                    GetClassElenemt(resultStr);
                }
            });
        }

        /// <summary> 
        ///解析html数据
        ///
        /// <summary> 
        private void GetClassElenemt(string resultStr) {
            //js调用方式1，当需要返回值的时候
            //获取自己余额
            var task = this.webBrowser1.GetBrowser().MainFrame.EvaluateScriptAsync("(function() { return document.getElementsByClassName('_2HLqr')[0].innerText;})();", null);
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    EvaluateJavaScriptResult = response.Success ? (response.Result ?? "null") : response.Message;
                    var my = GetFormatMoney(EvaluateJavaScriptResult.ToString());
                }
            });

            //html解析器
            hap.HtmlDocument htmlDocument = new hap.HtmlDocument();
            htmlDocument.LoadHtml(resultStr);
            hap.HtmlNodeCollection formNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='_3a3_c']");
            if (formNodes == null)
            {
                return;
            }
            //对抓取到的几个房间进行解析
            int index = 0;
            foreach (var item in formNodes)
            {
                var time = item.SelectSingleNode("//div[@class='_2rb_p']");
                var state = item.SelectSingleNode("//div[@class='_1sIzQ']");
                //测试，通过C# node传入js，在js里面修改node的值
                if (time == null)
                {
                    return;
                }
                //如果这个区域已经下了注
                if (this.isBet1[index])
                {
                    return;
                }
                this.isBet1[index] = true;
                //投注
                //选择筹码
                //                   webBrowser1.Document.GetElementById("").RaiseEvent("click");
                //                 //选择庄、闲
                //               webBrowser1.Document.GetElementById("").RaiseEvent("click");
                //             //确定
                //           webBrowser1.Document.GetElementById("").RaiseEvent("click");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.webBrowser1.GetBrowser().MainFrame.ExecuteJavaScriptAsync("document.getElementsByClassName('_34Nqi _7vTww')[0].click");
            
            /*               string path = AppDomain.CurrentDomain.BaseDirectory + "test.js";
                           string str2 = File.ReadAllText(path);

                           string fun = string.Format(@"sayHello('{0}')", this.textBox1.Text.Trim());
                           string result = ExecuteScript(fun, str2);

                           MessageBox.Show(result); */
        }

        /// <summary>
        /// 执行JS
        /// </summary>
        /// <param name="sExpression">参数体</param>
        /// <param name="sCode">JavaScript代码的字符串</param>
        /// <returns></returns>
 /*       private string ExecuteScript(string sExpression, string sCode)
        {
            //MSScriptControl.ScriptControl scriptControl = new MSScriptControl.ScriptControl();
            scriptControl.UseSafeSubset = true;
            scriptControl.Language = "JScript";
            scriptControl.AddCode(sCode);
            try
            {
                string str = scriptControl.Eval(sExpression).ToString();
                return str;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return null;
        } */

        private string GetFormatMoney(string _money) {
            var cntchar = _money.Length;
            string strnew = "";
            for (int i = 1; i<= cntchar; i++) {
                var strnum = _money.Substring(i - 1, 1);
                if (strnum != ",") {
                    strnew = strnew+strnum;
                }
             }

            return strnew;
        }
        /// <summary>  
        /// 修改注册表信息来兼容当前程序  
        ///   
        /// </summary>  
        static void SetWebBrowserFeatures(int ieVersion)
        {
            // don't change the registry if running in-proc inside Visual Studio  
            if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
                return;
            //获取程序及名称  
            var appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //得到浏览器的模式的值  
            UInt32 ieMode = GeoEmulationModee(ieVersion);
            var featureControlRegKey = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\";
            //设置浏览器对应用程序（appName）以什么模式（ieMode）运行  
            Registry.SetValue(featureControlRegKey + "FEATURE_BROWSER_EMULATION",
                appName, ieMode, RegistryValueKind.DWord);
            // enable the features which are "On" for the full Internet Explorer browser  
            //不晓得设置有什么用  
            Registry.SetValue(featureControlRegKey + "FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION",
                appName, 1, RegistryValueKind.DWord);


            //Registry.SetValue(featureControlRegKey + "FEATURE_AJAX_CONNECTIONEVENTS",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_GPU_RENDERING",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_WEBOC_DOCUMENT_ZOOM",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_NINPUT_LEGACYMODE",  
            //    appName, 0, RegistryValueKind.DWord);  
        }
        /// <summary>  
        /// 获取浏览器的版本  
        /// </summary>  
        /// <returns></returns>  
        static int GetBrowserVersion()
        {
            int browserVersion = 0;
            using (var ieKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer",
                RegistryKeyPermissionCheck.ReadSubTree,
                System.Security.AccessControl.RegistryRights.QueryValues))
            {
                var version = ieKey.GetValue("svcVersion");
                if (null == version)
                {
                    version = ieKey.GetValue("Version");
                    if (null == version)
                        throw new ApplicationException("Microsoft Internet Explorer is required!");
                }
                int.TryParse(version.ToString().Split('.')[0], out browserVersion);
            }
            //如果小于7  
            if (browserVersion < 7)
            {
                throw new ApplicationException("不支持的浏览器版本!");
            }
            return browserVersion;
        }
        /// <summary>  
        /// 通过版本得到浏览器模式的值  
        /// </summary>  
        /// <param name="browserVersion"></param>  
        /// <returns></returns>  
        static UInt32 GeoEmulationModee(int browserVersion)
        {
            UInt32 mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode.   
            switch (browserVersion)
            {
                case 7:
                    mode = 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode.   
                    break;
                case 8:
                    mode = 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode.   
                    break;
                case 9:
                    mode = 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode.                      
                    break;
                case 10:
                    mode = 10000; // Internet Explorer 10.  
                    break;
                case 11:
                    mode = 11000; // Internet Explorer 11  
                    break;
            }
            return mode;
        }
    }
}