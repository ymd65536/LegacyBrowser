using System;
using System.Windows.Forms;
using WebBrowserExtended;

namespace WebForms
{
  public partial class WebForm : Form
  {
    private MyWebBrowser Browser;
    private WebForm frmWB;

    public WebForm()
    {
      Browser = new MyWebBrowser();
      Browser.ScriptErrorsSuppressed = true;
      Browser.Navigate("C:\\web_forms\\index.html");
      InitializeComponent();

      Browser.Dock = DockStyle.Fill;
      this.Controls.Add(Browser);
      Browser.NewWindow2 += new NewWindow2EventHandler(Browser_NewWindow2);
      Browser.Closing += new MyWebBrowser.ClosingEventHandler(OnQuit);
    }
    private void OnQuit(object sender, EventArgs e)
    {
      this.Browser.Dispose();
      Browser = null;
      this.Close();
    }
    private void Browser_NewWindow2(object sender, NewWindow2EventArgs e)
    {
      // WebBrowser.AttachInterfaces メソッドは Visible
      // プロパティを true にすると呼び出される。なので、
      // ここで設定しないと RegisterAsBrowser プロパティ、
      // Application プロパティで例外がスローされてしまう。
      frmWB = new WebForm();
      frmWB.Visible = true;
      frmWB.Browser.ScriptErrorsSuppressed = true;
      frmWB.Browser.RegisterAsBrowser = true;
      e.PpDisp = frmWB.Browser.Application;
    }
  }
}
