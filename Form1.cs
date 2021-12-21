﻿using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using WebBrowserExtended;

// フォームコントロール
using System.Drawing;

namespace WebForms
{
  public partial class WebForm : Form
  {
    private MyWebBrowser Browser;
    private WebForm frmWB;
    TextBox UrlField;
    Button BackBtn;
    Button ForwardBtn;

    public WebForm(bool InitFlag = true)
    {
      Browser = new MyWebBrowser();
      Browser.ScriptErrorsSuppressed = true;
      InitializeComponent();
      //Browser.Dock = DockStyle.Fill;
      Browser.ClientSize = new Size(980, 440);
      Browser.Location = new Point(10, 50);

      BackBtn = new Button();
      BackBtn.Text = "戻る";
      BackBtn.ClientSize = new Size(50, 30);
      BackBtn.Location = new Point(10, 10);

      ForwardBtn = new Button();
      ForwardBtn.Text = "進む";
      ForwardBtn.ClientSize = new Size(50, 30);
      ForwardBtn.Location = new Point(BackBtn.Width + BackBtn.Location.X + 5, 10);

      UrlField = new TextBox();
      UrlField.ClientSize = new Size(820, 30);
      UrlField.Location = new Point(ForwardBtn.Width + ForwardBtn.Location.X + 5, 15);

      // Configを取得
      string CurDir = Environment.CurrentDirectory.ToString();
      string HomePage = CurDir + "\\" + "config\\homepage.txt";

      StreamReader HomePageSr = new StreamReader(HomePage, Encoding.GetEncoding("UTF-8"));
      string HomePageUrl = HomePageSr.ReadToEnd();

      if (InitFlag)
      {
        // GoHome とするとPCに登録されているホームページを見に行く
        //Browser.Navigate(HomePageUrl);
        Browser.GoHome();
      }
      this.Controls.Add(BackBtn);
      this.Controls.Add(ForwardBtn);
      this.Controls.Add(UrlField);
      this.Controls.Add(Browser);
      this.SizeChanged += WebForm_SizeChanged;
      Browser.Navigated += Browser_Navigated;
      Browser.NewWindow2 += new NewWindow2EventHandler(Browser_NewWindow2);
      Browser.Closing += new MyWebBrowser.FormClosingEventHandler(OnQuit);
    }
    private void WebForm_SizeChanged(object sender, EventArgs e)
    {
      this.Browser.ClientSize = new Size(this.ClientSize.Width - 20, this.Height - 60);
    }
    private void OnQuit(object sender, EventArgs e)
    {
      this.Browser.Dispose();
      Browser = null;
      this.Close();
    }
    private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
    {
      UrlField.Text = this.Browser.Url.ToString();
    }
    private void Browser_NewWindow2(object sender, NewWindow2EventArgs e)
    {
      // WebBrowser.AttachInterfaces メソッドは Visible
      // プロパティを true にすると呼び出される。なので、
      // ここで設定しないと RegisterAsBrowser プロパティ、
      // Application プロパティで例外がスローされてしまう。
      frmWB = new WebForm(false);
      frmWB.Visible = true;
      frmWB.Browser.ScriptErrorsSuppressed = true;
      frmWB.Browser.RegisterAsBrowser = true;
      e.PpDisp = frmWB.Browser.Application;
    }
  }
}
