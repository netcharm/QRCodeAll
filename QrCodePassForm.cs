using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace QrCodeAny{
  /// <summary>
  /// The container form for the QR code.
  /// Aspect ratio preseving resizing taken from: http://www.vcskicks.com/maintain-aspect-ratio.php
  /// </summary>
  public partial class QrCodePassForm : Form {
    
    const double widthRatio  = 1;
    const double heightRatio = 1;

    const int WM_SIZING   = 0x214;
    const int WMSZ_LEFT   = 1;
    const int WMSZ_RIGHT  = 2;
    const int WMSZ_TOP    = 3;
    const int WMSZ_BOTTOM = 6;

    public struct RECT {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
    }

    public QrCodePassForm() {
      InitializeComponent();
      ClientSize = new Size( 200, 200 );
    }

    protected override void WndProc( ref Message m ) {
      Rectangle screenRectangle = RectangleToScreen( ClientRectangle );
      int titleHeight = screenRectangle.Top - Top;

      if( m.Msg == WM_SIZING ) {
        RECT rc = (RECT)Marshal.PtrToStructure( m.LParam, typeof( RECT ) );
        int res = m.WParam.ToInt32();
        if( res == WMSZ_LEFT || res == WMSZ_RIGHT ) {
          //Left or right resize -> adjust height (bottom)
          rc.Bottom = rc.Top + (int)( heightRatio * this.Width / widthRatio ) + titleHeight;
        } else if( res == WMSZ_TOP || res == WMSZ_BOTTOM ) {
          //Up or down resize -> adjust width (right)
          rc.Right = rc.Left + (int)( widthRatio * this.Height / heightRatio );
        } else if( res == WMSZ_RIGHT + WMSZ_BOTTOM ) {
          //Lower-right corner resize -> adjust height (could have been width)
          rc.Bottom = rc.Top + (int)( heightRatio * this.Width / widthRatio ) + titleHeight;
        } else if( res == WMSZ_LEFT + WMSZ_TOP ) {
          //Upper-left corner -> adjust width (could have been height)
          rc.Left = rc.Right - (int)( widthRatio * this.Height / heightRatio );
        }
        Marshal.StructureToPtr( rc, m.LParam, true );
      }

      base.WndProc( ref m );
    }
  }
}
