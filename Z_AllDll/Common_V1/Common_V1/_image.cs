using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Collections.Specialized;

namespace Common_V1
{
    internal class _image
    {
        internal Dictionary<string, object> Config = new Dictionary<string, object>();
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(
        IntPtr hdc, // handle to DC
        int nIndex // index of capability
        );
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        const int HORZRES = 8;
        const int VERTRES = 10;
        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        const int DESKTOPVERTRES = 117;
        const int DESKTOPHORZRES = 118;

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hCursor;
            public Point ptScreenPos;
        }
        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);
        private const int CURSOR_SHOWING = 0x00000001;
        int Width = 0;
        int Height = 0;
        string exePath = $"{System.Windows.Forms.Application.StartupPath}";
        ImageCodecInfo ici;
        EncoderParameters eptS;
        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="bitmap">原图片</param>
        /// <param name="ms">内存流</param>
        public void CompressImage(bool isUpload)
        {
            string SavePath = $@"{exePath}\LOG\{Config["Name"]}_Screenshot_{DateTime.Now.DayOfYear / 7 + 1}\{Config["SN"]}_{DateTime.Now:dd_HH_mm_ss}.png";
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
            if (ici == null)
                init("image/png");
            using (Bitmap bit = new Bitmap(Width, Height))
            {

                Graphics g = Graphics.FromImage(bit);
                g.CopyFromScreen(0, 0, 0, 0, bit.Size);
                DrawCursorImageToScreenImage(ref g);
                bit.Save(SavePath);
                g.Dispose();
            }

            if (isUpload)
            {
                NameValueCollection stringDict = new NameValueCollection() { };
                stringDict.Add("Path", $@"D:Image\{Config["Name"]}_{DateTime.Now.ToString("yyyy")}_{DateTime.Now.DayOfYear / 7 + 1}");
                Console.WriteLine(HttpPostAPI.HttpPostData("http://10.55.22.160:8070/api/UploadFile/UploadProject", 2000, "File", Path.GetFileName(SavePath), SavePath, stringDict));

            }






        }
        void init(string CoderType)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            Width = GetDeviceCaps(hdc, DESKTOPHORZRES);
            Height = GetDeviceCaps(hdc, DESKTOPVERTRES);
            Encoder ecd = null;
            EncoderParameter ept = null;
            ici = this.getImageCoderInfo(CoderType);
            ecd = Encoder.Quality;
            eptS = new EncoderParameters(1);
            ept = new EncoderParameter(ecd, 10L);
            eptS.Param[0] = ept;
            ept.Dispose();
        }
        /// <summary>
        /// 将鼠标指针形状绘制到屏幕截图上
        /// </summary>
        /// <param name="g"></param>
        private void DrawCursorImageToScreenImage(ref Graphics g)
        {
            CURSORINFO vCurosrInfo;
            vCurosrInfo.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
            GetCursorInfo(out vCurosrInfo);
            if ((vCurosrInfo.flags & CURSOR_SHOWING) != CURSOR_SHOWING) return;
            Cursor vCursor = new Cursor(vCurosrInfo.hCursor);
            Rectangle vRectangle = new Rectangle(new Point(vCurosrInfo.ptScreenPos.X - vCursor.HotSpot.X, vCurosrInfo.ptScreenPos.Y - vCursor.HotSpot.Y), vCursor.Size);
            vCursor.Draw(g, vRectangle);
        }
        /// <summary>  
        /// 获取图片编码信息  
        /// </summary>  
        /// <param name="coderType">编码类型</param>  
        /// <returns>ImageCodecInfo</returns>  
        private ImageCodecInfo getImageCoderInfo(string coderType)
        {
            ImageCodecInfo[] iciS = ImageCodecInfo.GetImageEncoders();

            ImageCodecInfo retIci = null;

            foreach (ImageCodecInfo ici in iciS)
            {
                if (ici.MimeType.Equals(coderType))
                {
                    retIci = ici;
                }

            }
            return retIci;
        }

    }
}
