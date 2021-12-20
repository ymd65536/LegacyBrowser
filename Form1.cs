using System;
using System.Windows.Forms;
using WebBrowserExtended;
using NewWebWin;

namespace WebForms
{
    public partial class WebForm : Form
    {
        private MyWebBrowser Browser;
        private WebForm frmWB;

        public WebForm(bool InitFlag = true)
        {
            Browser = new MyWebBrowser();
            Browser.ScriptErrorsSuppressed = true;
            if (InitFlag)
            {
                Browser.Navigate("https://www.google.com/?hl=ja");
            }
            InitializeComponent();

            Browser.Dock = DockStyle.Fill;
            this.Controls.Add(Browser);
            Browser.NewWindow2 += new NewWindow2EventHandler(Browser_NewWindow2);
            Browser.Closing += new MyWebBrowser.FormClosingEventHandler(OnQuit);
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
