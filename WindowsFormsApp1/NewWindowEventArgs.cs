
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
public class NewWindowEventArgs : EventArgs
{
    private IWindowInfo _windowInfo;
    public IWindowInfo WindowInfo
    {
        get { return _windowInfo; }
        set { value = _windowInfo; }
    }
    public string url { get; set; }
    public NewWindowEventArgs(IWindowInfo windowInfo, string url)
    {
        _windowInfo = windowInfo;
        this.url = url;
    }
}