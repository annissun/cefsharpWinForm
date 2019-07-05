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


public class CefLifeSpanHandler : CefSharp.ILifeSpanHandler
{
    public CefLifeSpanHandler()
    {

    }

    public bool DoClose(IWebBrowser browserControl, CefSharp.IBrowser browser)
    {
        if (browser.IsDisposed || browser.IsPopup)
        {
            return false;
        }

        return true;
    }

    public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
    {

    }

    public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
    {
    }


    public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
    {
        var chromiumWebBrowser = (ExtChromiumBrowser)browserControl;

        chromiumWebBrowser.Invoke(new Action(() =>
        {
            NewWindowEventArgs e = new NewWindowEventArgs(windowInfo, targetUrl);
            chromiumWebBrowser.OnNewWindow(e);
        }));

        newBrowser = null;
        return true;
    }
}