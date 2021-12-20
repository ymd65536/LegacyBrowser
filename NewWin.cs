using System;
using System.Windows.Forms;
using WebBrowserExtended;

namespace NewWebWin
{
  public partial class NewWin : Form
  {
    private MyWebBrowser Browser;
    private NewWin frmWB;
    public NewWin()
    {
      Browser = new MyWebBrowser();
      Browser.ScriptErrorsSuppressed = true;
      InitializeComponent();
      Browser.Dock = DockStyle.Fill;
      this.Controls.Add(Browser);
      Browser.NewWindow2 += new NewWindow2EventHandler(Browser_NewWindow2);
      Browser.Closing += new MyWebBrowser.FormClosingEventHandler(OnQuit);
    }
    public void ScriptErrorsSuppressed()
    {
      Browser.ScriptErrorsSuppressed = false;
    }
    public void RegisterAsBrowser()
    {
      Browser.RegisterAsBrowser = true;
    }
    public object getApplication()
    {
      return this.Browser.Application;
    }
    private void OnQuit(object sender, EventArgs e)
    {
      this.Browser.RegisterAsBrowser = false;
      this.Browser.Dispose();
      Browser = null;
      this.Close();
    }
    private void Browser_NewWindow2(object sender, NewWindow2EventArgs e)
    {
      frmWB = new NewWin();
      frmWB.Visible = true;
      frmWB.Browser.ScriptErrorsSuppressed = true;
      frmWB.Browser.RegisterAsBrowser = true;
      e.PpDisp = frmWB.Browser.Application;
    }
  }
}