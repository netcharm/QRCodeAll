using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using KeePass.Plugins;
using KeePass.Util.Spr;
using KeePassLib;
using KeePassLib.Security;

using ZXing;
using ZXing.QrCode.Internal;
using ZXing.Rendering;
using ZXing.Common;

using System.Threading;
using System.Globalization;
using System.IO;

using NGettext;

namespace QrCodeAny {
    /// <summary>
    /// QrCodeGeneratorExt provides a KeePass interface to the ThoughtWorks.QRCode QR Code library.
    /// The QRCode library was taken from http://www.codeproject.com/KB/cs/qrcode.aspx and is licenced
    /// under the Code Project Open Licence (which should be included with this package).
    /// </summary>
    public sealed class QrCodeAnyExt : Plugin 
    {
        private static string resourceBaseName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        private static string resourceCultureName = Thread.CurrentThread.CurrentUICulture.Name;
        //private static string resourcePath = AppDomain.CurrentDomain.BaseDirectory + "locale";
        private static string resourcePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "locale");

        private ICatalog catalog = new Catalog(resourceBaseName, resourcePath, CultureInfo.CurrentCulture);

        /// <summary>
        /// A references to KeePass itself.
        /// </summary>
        private IPluginHost Host { get; set; }

        /// <summary>
        /// A separator inserted above our root tool strip menu item.
        /// </summary>
        private ToolStripSeparator SeparatorMenu { get; set; }
        private ToolStripSeparator SeparatorContextMenu { get; set; }

        /// <summary>
        /// The tool strip menu root for our plugin
        /// </summary>
        private ToolStripMenuItem ShowQrCodePassMenuItem { get; set; }
        private ToolStripMenuItem ShowQrCodeAllMenuItem { get; set; }
        private ToolStripMenuItem ShowQrCodePassContextMenuItem { get; set; }
        private ToolStripMenuItem ShowQrCodeAllContextMenuItem { get; set; }

        /// <summary>
        /// Initializes the plugin.
        /// Called by KeePass.
        /// </summary>
        /// <param name="host">A reference to KeePass.</param>
        /// <returns><see langword="true"/> if initialization was successful; <see langword="false"/> otherwise.</returns>
        public override bool Initialize( IPluginHost host ) 
        {
            Debug.Assert( host != null );
            if( host == null ) return false;
            Host = host;

            //MessageBox.Show(System.Reflection.Assembly.GetEntryAssembly().Location, "Entry");
            //MessageBox.Show(System.Reflection.Assembly.GetCallingAssembly().Location, "Calling");
            //MessageBox.Show(System.Reflection.Assembly.GetExecutingAssembly().Location, "Executing");
#if DEBUG
            MessageBox.Show(resourceBaseName, "ResourceBaseName");
            MessageBox.Show(resourcePath, "ResourcePath");
#endif
            SeparatorMenu = new ToolStripSeparator();
            SeparatorContextMenu = new ToolStripSeparator();
            Host.MainWindow.ToolsMenu.DropDownItems.Add( SeparatorMenu );
            Host.MainWindow.EntryContextMenu.Items.Add( SeparatorContextMenu );

            ShowQrCodePassMenuItem = new ToolStripMenuItem 
            {
                Image = Resources.QrCode, 
                Text  = catalog.GetString("QR Password")
            };
            ShowQrCodeAllMenuItem = new ToolStripMenuItem
            {
                Image = Resources.QrCode,
                Text = catalog.GetString("QR All")
            };

            ShowQrCodePassContextMenuItem = new ToolStripMenuItem 
            {
                Image = Resources.QrCode,
                Text = catalog.GetString("QR Password")
            };
            ShowQrCodeAllContextMenuItem = new ToolStripMenuItem
            {
                Image = Resources.QrCode,
                Text = catalog.GetString("QR All")
            };

            ShowQrCodePassMenuItem.Click        += OnShowQrCodePass;
            ShowQrCodeAllMenuItem.Click         += OnShowQrCodeAll;
            ShowQrCodePassContextMenuItem.Click += OnShowQrCodePass;
            ShowQrCodeAllContextMenuItem.Click  += OnShowQrCodeAll;

            Host.MainWindow.ToolsMenu.DropDownItems.Add( SeparatorMenu );
            Host.MainWindow.ToolsMenu.DropDownItems.Add(ShowQrCodePassMenuItem);
            Host.MainWindow.ToolsMenu.DropDownItems.Add(ShowQrCodeAllMenuItem);

            Host.MainWindow.EntryContextMenu.Items.Add( SeparatorContextMenu );
            Host.MainWindow.EntryContextMenu.Items.Add(ShowQrCodePassContextMenuItem);
            Host.MainWindow.EntryContextMenu.Items.Add(ShowQrCodeAllContextMenuItem);

            return true;
        }

        /// <summary>
        /// The <c>Terminate</c> function is called by KeePass when
        /// you should free all resources, close open files/streams,
        /// etc. It is also recommended that you remove all your
        /// plugin menu items from the KeePass menu.
        /// </summary>
        public override void Terminate() 
        {
            // Remove all of our menu items
            ShowQrCodePassMenuItem.Click        -= OnShowQrCodePass;
            ShowQrCodeAllMenuItem.Click         -= OnShowQrCodeAll;
            ShowQrCodePassContextMenuItem.Click -= OnShowQrCodePass;
            ShowQrCodeAllContextMenuItem.Click  -= OnShowQrCodeAll;


            Host.MainWindow.ToolsMenu.DropDownItems.Remove( SeparatorMenu );
            Host.MainWindow.ToolsMenu.DropDownItems.Remove( ShowQrCodePassMenuItem );
            Host.MainWindow.ToolsMenu.DropDownItems.Remove( ShowQrCodeAllMenuItem );

            Host.MainWindow.EntryContextMenu.Items.Remove( SeparatorContextMenu );
            Host.MainWindow.EntryContextMenu.Items.Remove( ShowQrCodePassContextMenuItem );
            Host.MainWindow.EntryContextMenu.Items.Remove( ShowQrCodeAllContextMenuItem );
        }

        /// <summary>
        /// Invoked when the user invokes the "Show QR Code Pass" action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowQrCodePass( object sender, EventArgs e ) 
        {
            if( !Host.Database.IsOpen ) {
                return;
            }

            PwEntry pwEntry = Host.MainWindow.GetSelectedEntry( true );
            if( null == pwEntry ) return;

      
            string          title           = pwEntry.Strings.Get( PwDefs.TitleField ).ReadString();
            ProtectedString protectedString = pwEntry.Strings.GetSafe( PwDefs.PasswordField );
            byte[]          bytes           = protectedString.ReadUtf8();
            UTF8Encoding    encoding        = new UTF8Encoding();
            string          password        = encoding.GetString( bytes );

            SprContext context = new SprContext( pwEntry, Host.Database, SprCompileFlags.All );
            char chScan, chWanted;
            PwEntry peRef = SprEngine.FindRefTarget( password, context, out chScan, out chWanted );
            if( null != peRef ) 
            {
                protectedString = peRef.Strings.GetSafe( PwDefs.PasswordField );
                bytes = protectedString.ReadUtf8();
                password = encoding.GetString( bytes );
            }

            Bitmap bitmap = GetQrCode( password, Resources.QRCode_Pass );
            if( null == bitmap ) 
            {
                MessageBox.Show(
                    Host.MainWindow,
                    catalog.GetString( "The given password does not fit into a QR code." ), 
                    catalog.GetString( "QR Code Error" ), 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error );
                return;
            }

            using (QrCodePassForm form = new QrCodePassForm
            {
                BackgroundImage = bitmap,
                StartPosition = FormStartPosition.CenterParent,
                Text = String.Format("{0} - {1}", Application.ProductName, title)
            })
            {

                form.ShowDialog( Host.MainWindow );
                form.Close();
                form.Dispose();
            }
        }

        /// <summary>
        /// Invoked when the user invokes the "Show QR Code All" action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowQrCodeAll(object sender, EventArgs e)
        {
            if (!Host.Database.IsOpen)
            {
                return;
            }

            PwEntry pwEntry = Host.MainWindow.GetSelectedEntry(true);
            if (null == pwEntry) return;


            string title = pwEntry.Strings.Get(PwDefs.TitleField).ReadString();

            ProtectedString protectedStringUser = pwEntry.Strings.GetSafe(PwDefs.UserNameField);
            byte[] bytesUser = protectedStringUser.ReadUtf8();
            UTF8Encoding encodingUser = new UTF8Encoding();
            string username = encodingUser.GetString(bytesUser);

            ProtectedString protectedStringPass = pwEntry.Strings.GetSafe(PwDefs.PasswordField);
            byte[] bytesPass = protectedStringPass.ReadUtf8();
            UTF8Encoding encodingPass = new UTF8Encoding();
            string password = encodingPass.GetString(bytesPass);

            ProtectedString protectedStringUrl = pwEntry.Strings.GetSafe(PwDefs.UrlField);
            byte[] bytesUrl = protectedStringUrl.ReadUtf8();
            UTF8Encoding encodingUrl = new UTF8Encoding();
            string url = encodingUrl.GetString(bytesUrl);

            ProtectedString protectedStringNote = pwEntry.Strings.GetSafe(PwDefs.NotesField);
            byte[] bytesNote = protectedStringNote.ReadUtf8();
            UTF8Encoding encodingNote = new UTF8Encoding();
            string note = encodingNote.GetString(bytesNote);

            SprContext context = new SprContext(pwEntry, Host.Database, SprCompileFlags.All);
            char chScan, chWanted;

            PwEntry peRefUser = SprEngine.FindRefTarget(username, context, out chScan, out chWanted);
            if (null != peRefUser)
            {
                protectedStringUser = peRefUser.Strings.GetSafe(PwDefs.UserNameField);
                bytesUser = protectedStringUser.ReadUtf8();
                username = encodingUser.GetString(bytesUser);
            }

            PwEntry peRefPass = SprEngine.FindRefTarget(password, context, out chScan, out chWanted);
            if (null != peRefPass)
            {
                protectedStringPass = peRefPass.Strings.GetSafe(PwDefs.PasswordField);
                bytesPass = protectedStringPass.ReadUtf8();
                password = encodingPass.GetString(bytesPass);
            }

            PwEntry peRefUrl = SprEngine.FindRefTarget(url, context, out chScan, out chWanted);
            if (null != peRefUrl)
            {
                protectedStringUrl = peRefUrl.Strings.GetSafe(PwDefs.UrlField);
                bytesUrl = protectedStringUrl.ReadUtf8();
                url = encodingUrl.GetString(bytesUrl);
            }

            PwEntry peRefNote = SprEngine.FindRefTarget(note, context, out chScan, out chWanted);
            if (null != peRefNote)
            {
                protectedStringNote = peRefNote.Strings.GetSafe(PwDefs.NotesField);
                bytesNote = protectedStringNote.ReadUtf8();
                note = encodingNote.GetString(bytesNote);
            }

            Bitmap bitmapPass = GetQrCode(password, Resources.QRCode_Pass);
            if (null == bitmapPass)
            {
                //Error( catalog.GetString("QR Code Error"), catalog.GetString("The given password does not fit into a QR code.") );
                MessageBox.Show(
                    Host.MainWindow,
                    catalog.GetString( "The given password does not fit into a QR code." ), 
                    catalog.GetString( "QR Code Error" ), 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Bitmap bitmapUser = GetQrCode(username, Resources.QRCode_User);
            if (null == bitmapUser)
            {
                //Error( catalog.GetString("QR Code Error"), catalog.GetString("The given username does not fit into a QR code.") );
                MessageBox.Show(
                    Host.MainWindow,
                    catalog.GetString( "The given username does not fit into a QR code." ),
                    catalog.GetString( "QR Code Error" ), 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Bitmap bitmapUrl = GetQrCode(url, Resources.QRCode_Url);
            if (null == bitmapUrl)
            {
                //Error( catalog.GetString("QR Code Error"), catalog.GetString("The given url does not fit into a QR code.") );
                MessageBox.Show(
                    Host.MainWindow,
                    catalog.GetString( "The given url does not fit into a QR code." ),
                    catalog.GetString( "QR Code Error" ), 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Bitmap bitmapNote = GetQrCode(note, Resources.QRCode_Note);
            if (null == bitmapNote)
            {
                //Error( "QR Code Error", "The given note does not fit into a QR code." );
                //Error( catalog.GetString("QR Code Error"), catalog.GetString("The given note does not fit into a QR code.") );
                MessageBox.Show(
                    Host.MainWindow,
                    catalog.GetString( "The given note does not fit into a QR code." ),
                    catalog.GetString( "QR Code Error" ), 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            using( QrCodeAllForm form = new QrCodeAllForm
                {
                    //BackgroundImage = bitmapPass,
                    StartPosition = FormStartPosition.CenterParent,
                    Text = String.Format("{0} - {1}", Application.ProductName, title)
                })
            {
                form.picUser.Image = bitmapUser;
                form.picPass.Image = bitmapPass;
                form.picUrl.Image = bitmapUrl;
                form.picNote.Image = bitmapNote;

                form.ShowDialog(Host.MainWindow);
                form.Close();
                form.Dispose();
            }
        }

        private DialogResult Error(string caption, string content)
        {
            return MessageBox.Show(
                    Host.MainWindow,
                    catalog.GetString( content ),
                    catalog.GetString( caption ), 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);        
        }
        
        private Bitmap GetQrCode( string text, Image overlay )
        {
            var width = 512;
            var height = 512;
            var margin = 1;

            int MAX_TEXT = 750;

            if (String.IsNullOrEmpty(text))
            {
                Bitmap blankBitmap = new Bitmap(width, height);
                return (blankBitmap);
            }
            var qrText = text;
            if (qrText.Length > MAX_TEXT)
            {
                qrText = qrText.Substring(0, MAX_TEXT);
            }

            var bw = new ZXing.BarcodeWriter();

            var encOptions = new ZXing.Common.EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = margin,
                PureBarcode = true
            };

            encOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.Q);
            encOptions.Hints.Add(EncodeHintType.DISABLE_ECI, true);
            encOptions.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            encOptions.Hints.Add(EncodeHintType.PDF417_COMPACT, true);
            encOptions.Hints.Add(EncodeHintType.PDF417_COMPACTION, ZXing.PDF417.Internal.Compaction.AUTO);

            bw.Renderer = new BitmapRenderer();
            bw.Options = encOptions;
            bw.Format = BarcodeFormat.QR_CODE;

            try
            {
                BitMatrix bm = bw.Encode( text );
                int[] rectangle = bm.getEnclosingRectangle();
                var bmW = rectangle[2];
                var bmH = rectangle[3];
                //bw.Options.Width = (int) ( bmW * 1.1 );
                //bw.Options.Height = (int) ( bmH * 1.1 );

                Bitmap barcodeBitmap = bw.Write(qrText);

                int deltaHeigth = barcodeBitmap.Height - overlay.Height;
                int deltaWidth = barcodeBitmap.Width - overlay.Width;

                using (Graphics g = Graphics.FromImage(barcodeBitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.DrawImage(overlay, new Point(deltaWidth / 2, deltaHeigth / 2));
                }

                return (barcodeBitmap);
            }
            catch (WriterException)
            {
                return (new Bitmap(width, height));
            }
        }
    }
}