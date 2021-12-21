using System;
using System.Windows.Forms;
using WebBrowserExtended;

// フォームコントロール
using System.Drawing;

namespace LegacyWeb
{
  public partial class LegacyBrowser : Form
  {
    private MyWebBrowser Browser;
    private LegacyBrowser frmWB;
    TextBox UrlField;
    Button BackBtn;
    Button ForwardBtn;
    Button GoUrlBtn;
    Button HomeBtn;

    public LegacyBrowser(bool InitFlag = true)
    {
      Browser = new MyWebBrowser();
      Browser.ScriptErrorsSuppressed = true;
      Browser.WebBrowserShortcutsEnabled = true;
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
      UrlField.ClientSize = new Size(750, 30);
      UrlField.Location = new Point(ForwardBtn.Width + ForwardBtn.Location.X + 5, 15);

      GoUrlBtn = new Button();
      GoUrlBtn.ClientSize = new Size(50, 30);
      GoUrlBtn.Text = "Go";
      GoUrlBtn.Location = new Point(UrlField.Width + UrlField.Location.X + 5, 10);

      HomeBtn = new Button();
      HomeBtn.ClientSize = new Size(50, 30);
      HomeBtn.Text = "Home";
      HomeBtn.Location = new Point(GoUrlBtn.Width + GoUrlBtn.Location.X + 5, 10);

      // Configを取得
      if (InitFlag)
      {
        // GoHome とするとPCに登録されているホームページを見に行く
        Browser.GoHome();
      }
      this.Controls.Add(BackBtn);
      this.Controls.Add(ForwardBtn);
      this.Controls.Add(UrlField);
      this.Controls.Add(GoUrlBtn);
      this.Controls.Add(HomeBtn);
      this.Controls.Add(Browser);

      // イベントリスナの登録
      this.ForwardBtn.Click += ForwardBtn_Click;
      this.BackBtn.Click += BackBtn_Click;
      this.GoUrlBtn.Click += UrlGo_Click;
      this.HomeBtn.Click += HomeBtn_Click;
      this.SizeChanged += Browser_SizeChanged;
      Browser.Navigated += Browser_Navigated;
      Browser.NewWindow2 += new NewWindow2EventHandler(Browser_NewWindow2);
      Browser.Closing += new MyWebBrowser.FormClosingEventHandler(OnQuit);
    }
    private void BackBtn_Click(object sender, EventArgs e)
    {
      if (Browser.CanGoBack)
      {
        Browser.GoBack();
      }
    }
    private void ForwardBtn_Click(object sender, EventArgs e)
    {
      if (Browser.CanGoForward)
      {
        Browser.GoForward();
      }
    }
    private void UrlGo_Click(object sender, EventArgs e)
    {
      Browser.Navigate(UrlField.Text.ToString());
    }
    private void HomeBtn_Click(object sender, EventArgs e)
    {
      Browser.GoHome();
    }
    private void Browser_SizeChanged(object sender, EventArgs e)
    {
      Browser.ClientSize = new Size(this.ClientSize.Width - 20, this.Height - 100);
      UrlField.ClientSize = new Size(this.ClientSize.Width - 250, 30);
      GoUrlBtn.Location = new Point(UrlField.Width + UrlField.Location.X + 5, 10);
      HomeBtn.Location = new Point(GoUrlBtn.Width + GoUrlBtn.Location.X + 5, 10);
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
      this.Text = this.Browser.Document.Title.ToString();
      /*
      // meta タグ挿入による互換表示の変更はできない。
      var meta = Browser.Document.GetElementsByTagName("head")[0].Document.CreateElement("meta");
      meta.SetAttribute("http-equiv", "X-UA-Compatible");
      meta.SetAttribute("content", "\"IE=edge\"");
      */
    }
    private void Browser_NewWindow2(object sender, NewWindow2EventArgs e)
    {
      // WebBrowser.AttachInterfaces メソッドは Visible
      // プロパティを true にすると呼び出される。なので、
      // ここで設定しないと RegisterAsBrowser プロパティ、
      // Application プロパティで例外がスローされてしまう。
      frmWB = new LegacyBrowser(false);
      frmWB.Visible = true;
      frmWB.Browser.ScriptErrorsSuppressed = true;
      frmWB.Browser.RegisterAsBrowser = true;
      e.PpDisp = frmWB.Browser.Application;
    }
  }
}
