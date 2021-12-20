using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WebBrowserExtended
{
  public class MyWebBrowser : WebBrowser
  {
    // WebBrowser の AxtiveX への参照
    public delegate void FormClosingEventHandler(object sender, EventArgs e);

    private IWebBrowser2 axIWebBrowser2;
    public event FormClosingEventHandler Closing;

    // WebBrowser の AxtiveX が作成されたとき呼び出される
    protected override void AttachInterfaces(object nativeActiveXObject)
    {
      this.axIWebBrowser2 =
        (IWebBrowser2)nativeActiveXObject;
      base.AttachInterfaces(nativeActiveXObject);
    }

    protected override void WndProc(ref Message m)
    {
      switch (m.Msg)
      {
        case (int)WindowsMessages.WM_PARENTNOTIFY:
          if (!DesignMode)
          {
            if (m.WParam.ToInt32() == (int)WindowsMessages.WM_DESTROY)
            {
              Closing(this, EventArgs.Empty);
            }
          }
          DefWndProc(ref m);
          break;
        default:
          base.WndProc(ref m);
          break;
      }
    }

    //[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void DetachInterfaces()
    {
      this.axIWebBrowser2 = null;
      base.DetachInterfaces();
    }

    public object Application
    {
      get
      {
        if ((this.axIWebBrowser2 == null))
        {
          throw new
            AxHost.InvalidActiveXStateException(
              "Application",
              AxHost.ActiveXInvokeKind.PropertyGet);
        }

        // この Application プロパティは COM の
        // マネージラッパー
        return this.axIWebBrowser2.Application;
      }
    }

    public bool state
    {
      get
      {
        if ((this.axIWebBrowser2 == null))
        {
          throw new
            AxHost.InvalidActiveXStateException(
              "RegisterAsBrowser",
              AxHost.ActiveXInvokeKind.PropertyGet);
        }

        // この RegisterAsBrowser プロパティは
        // COM のマネージラッパー
        return this.axIWebBrowser2.RegisterAsBrowser;
      }
      set
      {
        if ((this.axIWebBrowser2 == null))
        {
          throw new
            AxHost.InvalidActiveXStateException(
              "RegisterAsBrowser",
              AxHost.ActiveXInvokeKind.PropertySet);
        }

        // この RegisterAsBrowser プロパティは
        // COM のマネージラッパー
        this.axIWebBrowser2.RegisterAsBrowser = value;
      }
    }

    public bool RegisterAsBrowser
    {
      get
      {
        if ((this.axIWebBrowser2 == null))
        {
          throw new
            AxHost.InvalidActiveXStateException(
              "RegisterAsBrowser",
              AxHost.ActiveXInvokeKind.PropertyGet);
        }

        // この RegisterAsBrowser プロパティは
        // COM のマネージラッパー
        return this.axIWebBrowser2.RegisterAsBrowser;
      }
      set
      {
        if ((this.axIWebBrowser2 == null))
        {
          throw new
            AxHost.InvalidActiveXStateException(
              "RegisterAsBrowser",
              AxHost.ActiveXInvokeKind.PropertySet);
        }

        // この RegisterAsBrowser プロパティは
        // COM のマネージラッパー
        this.axIWebBrowser2.RegisterAsBrowser = value;
      }
    }

    // シンクオブジェクトへの参照
    private MyWebBrowserEventSink sink;

    // HTTP 通信の cookie とは関係ないので注意
    private AxHost.ConnectionPointCookie cookie;

    // シンクをサブスクライバ・リストに追加
    //[PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void CreateSink()
    {
      base.CreateSink();

      if ((this.axIWebBrowser2 == null))
      {
        throw new AxHost.InvalidActiveXStateException(
            "CreateSink",
            AxHost.ActiveXInvokeKind.MethodInvoke);
      }

      this.sink = new MyWebBrowserEventSink(this);
      this.cookie = new AxHost.ConnectionPointCookie(
                          this.axIWebBrowser2,
                          this.sink,
                          typeof(DWebBrowserEvents2));
    }

    // シンクのサブスクライブを解除
    protected override void DetachSink()
    {
      if (cookie != null)
      {
        this.cookie.Disconnect();
        this.cookie = null;
      }
      base.DetachSink();
    }

    // NewWindow2 イベントの定義
    public event NewWindow2EventHandler NewWindow2;

    // .NET 側の NewWindow2 イベントを発動するメソッド
    protected virtual void
      OnNewWindow2(NewWindow2EventArgs e)
    {
      if ((this.NewWindow2 != null))
      {
        this.NewWindow2(this, e);
      }
    }

    // コネクションポイントからの呼び出しを受け取る
    // クライアント・シンクのクラス定義
    [ClassInterface(ClassInterfaceType.None)]
    public class MyWebBrowserEventSink :
      StandardOleMarshalObject, DWebBrowserEvents2
    {
      private MyWebBrowser browser;

      public MyWebBrowserEventSink(MyWebBrowser browser)
      {
        this.browser = browser;
      }

      // COM ソースから発生したイベントから呼び出される
      // メソッド
      public void NewWindow2(ref object ppDisp, ref bool cancel)
      {
        NewWindow2EventArgs e =
          new NewWindow2EventArgs(ppDisp, cancel);

        // .NET 側の NewWindow2 イベントを発動
        this.browser.OnNewWindow2(e);

        ppDisp = e.PpDisp;
        cancel = e.Cancel;
      }
    }
  }

  // NewWindow2 イベントのハンドラのデリゲート
  public delegate void NewWindow2EventHandler(object sender,
    NewWindow2EventArgs e);

  // NewWindow2 イベントハンドラ引数のクラス定義
  public class NewWindow2EventArgs : EventArgs
  {
    public object PpDisp { get; set; }
    public bool Cancel { get; set; }

    public NewWindow2EventArgs(object ppDisp, bool cancel)
    {
      this.PpDisp = ppDisp;
      this.Cancel = cancel;
    }
  }

  // DWebBrowserEvents2 インターフェイスの NewWindow2 メ
  // ソッド、IWebBrowser2 インターフェイスの Application
  // プロパティと RegisterAsBrowser プロパティをインポー
  // ト（つまり、マネージラッパーをコンパイル時に生成）。
  // ComImport, InterfaceType, Guid 指定は必須らしい。
  [ComImport,
  InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
  Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
  public interface DWebBrowserEvents2
  {
    [DispId(0xfb)]
    void NewWindow2(
      [In, Out, MarshalAs(UnmanagedType.IDispatch)]
      ref object ppDisp,
      [In, Out, MarshalAs(UnmanagedType.VariantBool)]
      ref bool Cancel);
  }

  [ComImport,
  Guid("D30C1661-CDAF-11D0-8A3E-00C04FC9E26E"),
  InterfaceType(ComInterfaceType.InterfaceIsIDispatch),]
  public interface IWebBrowser2
  {
    [DispId(200)]
    object Application
    {
      [return: MarshalAs(UnmanagedType.IDispatch)]
      get;
    }

    [DispId(0x228)]
    bool RegisterAsBrowser
    {
      get;
      set;
    }
  }

  enum WindowsMessages
  {
    WM_ACTIVATE = 0x6,
    WM_ACTIVATEAPP = 0x1C,
    WM_AFXFIRST = 0x360,
    WM_AFXLAST = 0x37F,
    WM_APP = 0x8000,
    WM_ASKCBFORMATNAME = 0x30C,
    WM_CANCELJOURNAL = 0x4B,
    WM_CANCELMODE = 0x1F,
    WM_CAPTURECHANGED = 0x215,
    WM_CHANGECBCHAIN = 0x30D,
    WM_CHAR = 0x102,
    WM_CHARTOITEM = 0x2F,
    WM_CHILDACTIVATE = 0x22,
    WM_CLEAR = 0x303,
    WM_CLOSE = 0x10,
    WM_COMMAND = 0x111,
    WM_COMPACTING = 0x41,
    WM_COMPAREITEM = 0x39,
    WM_CONTEXTMENU = 0x7B,
    WM_COPY = 0x301,
    WM_COPYDATA = 0x4A,
    WM_CREATE = 0x1,
    WM_CTLCOLORBTN = 0x135,
    WM_CTLCOLORDLG = 0x136,
    WM_CTLCOLOREDIT = 0x133,
    WM_CTLCOLORLISTBOX = 0x134,
    WM_CTLCOLORMSGBOX = 0x132,
    WM_CTLCOLORSCROLLBAR = 0x137,
    WM_CTLCOLORSTATIC = 0x138,
    WM_CUT = 0x300,
    WM_DEADCHAR = 0x103,
    WM_DELETEITEM = 0x2D,
    WM_DESTROY = 0x2,
    WM_DESTROYCLIPBOARD = 0x307,
    WM_DEVICECHANGE = 0x219,
    WM_DEVMODECHANGE = 0x1B,
    WM_DISPLAYCHANGE = 0x7E,
    WM_DRAWCLIPBOARD = 0x308,
    WM_DRAWITEM = 0x2B,
    WM_DROPFILES = 0x233,
    WM_ENABLE = 0xA,
    WM_ENDSESSION = 0x16,
    WM_ENTERIDLE = 0x121,
    WM_ENTERMENULOOP = 0x211,
    WM_ENTERSIZEMOVE = 0x231,
    WM_ERASEBKGND = 0x14,
    WM_EXITMENULOOP = 0x212,
    WM_EXITSIZEMOVE = 0x232,
    WM_FONTCHANGE = 0x1D,
    WM_GETDLGCODE = 0x87,
    WM_GETFONT = 0x31,
    WM_GETHOTKEY = 0x33,
    WM_GETICON = 0x7F,
    WM_GETMINMAXINFO = 0x24,
    WM_GETOBJECT = 0x3D,
    WM_GETTEXT = 0xD,
    WM_GETTEXTLENGTH = 0xE,
    WM_HANDHELDFIRST = 0x358,
    WM_HANDHELDLAST = 0x35F,
    WM_HELP = 0x53,
    WM_HOTKEY = 0x312,
    WM_HSCROLL = 0x114,
    WM_HSCROLLCLIPBOARD = 0x30E,
    WM_ICONERASEBKGND = 0x27,
    WM_IME_CHAR = 0x286,
    WM_IME_COMPOSITION = 0x10F,
    WM_IME_COMPOSITIONFULL = 0x284,
    WM_IME_CONTROL = 0x283,
    WM_IME_ENDCOMPOSITION = 0x10E,
    WM_IME_KEYDOWN = 0x290,
    WM_IME_KEYLAST = 0x10F,
    WM_IME_KEYUP = 0x291,
    WM_IME_NOTIFY = 0x282,
    WM_IME_REQUEST = 0x288,
    WM_IME_SELECT = 0x285,
    WM_IME_SETCONTEXT = 0x281,
    WM_IME_STARTCOMPOSITION = 0x10D,
    WM_INITDIALOG = 0x110,
    WM_INITMENU = 0x116,
    WM_INITMENUPOPUP = 0x117,
    WM_INPUTLANGCHANGE = 0x51,
    WM_INPUTLANGCHANGEREQUEST = 0x50,
    WM_KEYDOWN = 0x100,
    WM_KEYFIRST = 0x100,
    WM_KEYLAST = 0x108,
    WM_KEYUP = 0x101,
    WM_KILLFOCUS = 0x8,
    WM_LBUTTONDBLCLK = 0x203,
    WM_LBUTTONDOWN = 0x201,
    WM_LBUTTONUP = 0x202,
    WM_MBUTTONDBLCLK = 0x209,
    WM_MBUTTONDOWN = 0x207,
    WM_MBUTTONUP = 0x208,
    WM_MDIACTIVATE = 0x222,
    WM_MDICASCADE = 0x227,
    WM_MDICREATE = 0x220,
    WM_MDIDESTROY = 0x221,
    WM_MDIGETACTIVE = 0x229,
    WM_MDIICONARRANGE = 0x228,
    WM_MDIMAXIMIZE = 0x225,
    WM_MDINEXT = 0x224,
    WM_MDIREFRESHMENU = 0x234,
    WM_MDIRESTORE = 0x223,
    WM_MDISETMENU = 0x230,
    WM_MDITILE = 0x226,
    WM_MEASUREITEM = 0x2C,
    WM_MENUCHAR = 0x120,
    WM_MENUCOMMAND = 0x126,
    WM_MENUDRAG = 0x123,
    WM_MENUGETOBJECT = 0x124,
    WM_MENURBUTTONUP = 0x122,
    WM_MENUSELECT = 0x11F,
    WM_MOUSEACTIVATE = 0x21,
    WM_MOUSEFIRST = 0x200,
    WM_MOUSEHOVER = 0x2A1,
    WM_MOUSELAST = 0x20A,
    WM_MOUSELEAVE = 0x2A3,
    WM_MOUSEMOVE = 0x200,
    WM_MOUSEWHEEL = 0x20A,
    WM_MOVE = 0x3,
    WM_MOVING = 0x216,
    WM_NCACTIVATE = 0x86,
    WM_NCCALCSIZE = 0x83,
    WM_NCCREATE = 0x81,
    WM_NCDESTROY = 0x82,
    WM_NCHITTEST = 0x84,
    WM_NCLBUTTONDBLCLK = 0xA3,
    WM_NCLBUTTONDOWN = 0xA1,
    WM_NCLBUTTONUP = 0xA2,
    WM_NCMBUTTONDBLCLK = 0xA9,
    WM_NCMBUTTONDOWN = 0xA7,
    WM_NCMBUTTONUP = 0xA8,
    WM_NCMOUSEHOVER = 0x2A0,
    WM_NCMOUSELEAVE = 0x2A2,
    WM_NCMOUSEMOVE = 0xA0,
    WM_NCPAINT = 0x85,
    WM_NCRBUTTONDBLCLK = 0xA6,
    WM_NCRBUTTONDOWN = 0xA4,
    WM_NCRBUTTONUP = 0xA5,
    WM_NEXTDLGCTL = 0x28,
    WM_NEXTMENU = 0x213,
    WM_NOTIFY = 0x4E,
    WM_NOTIFYFORMAT = 0x55,
    WM_NULL = 0x0,
    WM_PAINT = 0xF,
    WM_PAINTCLIPBOARD = 0x309,
    WM_PAINTICON = 0x26,
    WM_PALETTECHANGED = 0x311,
    WM_PALETTEISCHANGING = 0x310,
    WM_PARENTNOTIFY = 0x210,
    WM_PASTE = 0x302,
    WM_PENWINFIRST = 0x380,
    WM_PENWINLAST = 0x38F,
    WM_POWER = 0x48,
    WM_PRINT = 0x317,
    WM_PRINTCLIENT = 0x318,
    WM_QUERYDRAGICON = 0x37,
    WM_QUERYENDSESSION = 0x11,
    WM_QUERYNEWPALETTE = 0x30F,
    WM_QUERYOPEN = 0x13,
    WM_QUEUESYNC = 0x23,
    WM_QUIT = 0x12,
    WM_RBUTTONDBLCLK = 0x206,
    WM_RBUTTONDOWN = 0x204,
    WM_RBUTTONUP = 0x205,
    WM_RENDERALLFORMATS = 0x306,
    WM_RENDERFORMAT = 0x305,
    WM_SETCURSOR = 0x20,
    WM_SETFOCUS = 0x7,
    WM_SETFONT = 0x30,
    WM_SETHOTKEY = 0x32,
    WM_SETICON = 0x80,
    WM_SETREDRAW = 0xB,
    WM_SETTEXT = 0xC,
    WM_SETTINGCHANGE = 0x1A,
    WM_SHOWWINDOW = 0x18,
    WM_SIZE = 0x5,
    WM_SIZECLIPBOARD = 0x30B,
    WM_SIZING = 0x214,
    WM_SPOOLERSTATUS = 0x2A,
    WM_STYLECHANGED = 0x7D,
    WM_STYLECHANGING = 0x7C,
    WM_SYNCPAINT = 0x88,
    WM_SYSCHAR = 0x106,
    WM_SYSCOLORCHANGE = 0x15,
    WM_SYSCOMMAND = 0x112,
    WM_SYSDEADCHAR = 0x107,
    WM_SYSKEYDOWN = 0x104,
    WM_SYSKEYUP = 0x105,
    WM_TCARD = 0x52,
    WM_TIMECHANGE = 0x1E,
    WM_TIMER = 0x113,
    WM_UNDO = 0x304,
    WM_UNINITMENUPOPUP = 0x125,
    WM_USER = 0x400,
    WM_USERCHANGED = 0x54,
    WM_VKEYTOITEM = 0x2E,
    WM_VSCROLL = 0x115,
    WM_VSCROLLCLIPBOARD = 0x30A,
    WM_WINDOWPOSCHANGED = 0x47,
    WM_WINDOWPOSCHANGING = 0x46,
    WM_WININICHANGE = 0x1A
  }

}