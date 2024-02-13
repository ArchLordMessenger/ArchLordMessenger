using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MacroSwitch
{
    public partial class ColorPicker : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator System.Windows.Point(POINT point)
            {
                return new System.Windows.Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);


        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        public IntPtr alWindow;
        int xx = 500, yy = 500;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public ColorPicker()
        {
            InitializeComponent();
            prgColor.Value = 100;
            prgColor.Style = ProgressBarStyle.Continuous;
            timer.Interval = 10;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Color c;
            if (alWindow != null)
            {
                c = GetPixelColor(alWindow, xx, yy);
                prgColor.ForeColor = Color.FromArgb(c.R, c.G, c.B);
                lblColor.Text = c.ToString();
            }

            POINT point;
            GetCursorPos(out point);
            ScreenToClient(alWindow, ref point);
            ScreenToClient(alWindow, ref point);
            lblDebug.Text = $"X: {point.X}    Y: {point.Y}";
        }

        static public Color GetPixelColor(IntPtr hwnd, int x, int y)
        {
            IntPtr hdc = GetDC(hwnd);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(hwnd, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                            (int)(pixel & 0x0000FF00) >> 8,
                            (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            xx = Int32.Parse(txtX.Text);
            yy = Int32.Parse(txtY.Text);
        }
    }
}
