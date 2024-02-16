using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArchlordInfinityMacro
{
    public static class MacroHelper
    {
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MapVirtualKey(int uCode, int uMapType);

        [DllImport("user32.dll")]
        static extern byte VkKeyScan(char ch);

        [DllImport("User32.Dll")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]

        static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, IntPtr lParam);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        const uint WM_KEYDOWN = 0x100;
        const uint WM_UP = 0x101;
        const uint MAPVK_VK_TO_VSC = 0;

        public static void SendBarUp(IntPtr targetProcess)
        {
            PostMessage(targetProcess, 0x100, (byte)MapVirtualKey(0x29, 1), new IntPtr((MapVirtualKey((byte)MapVirtualKey(0x29, 1), 0) << 16) & 0x00FF0000));
        }

        public static int[] GetArchlordBarKeys()
        {
            var keys = new int[10];

            for (var i = 0; i < 10; i++)
            {
                keys[i] = MapVirtualKey(0x4 + i, 1);
            }

            return keys;
        }

        public static void PressBtnNormal(int key, IntPtr targetProcess)
        {
            uint vk = (uint)GetArchlordBarKeys()[key];
            uint scanCode = (MapVirtualKey(vk, MAPVK_VK_TO_VSC) << 16) & 0x00FF0000;

            for (int i = 0; i < 2; i++)
            {
                PostMessage(targetProcess, WM_KEYDOWN, vk, scanCode);
                PostMessage(targetProcess, WM_UP, vk, (scanCode));
            }
        }

        public static void PressBtnShift(int key, IntPtr targetProcess)
        {
            uint vk = (uint)GetArchlordBarKeys()[key];
            uint scanCode = (MapVirtualKey(vk, MAPVK_VK_TO_VSC) << 16) & 0x00FF0000;

            for (int i = 0; i < 2; i++)
            {
                PostMessage(targetProcess, WM_KEYDOWN, 0x10, 0x002A0001);
                PostMessage(targetProcess, WM_KEYDOWN, vk, scanCode);
            }
        }

        public static void PressBtnCtrl(int key, IntPtr targetProcess)
        {
            uint vk = (uint)GetArchlordBarKeys()[key];
            uint scanCode = (MapVirtualKey(vk, MAPVK_VK_TO_VSC) << 16) & 0x00FF0000;

            for (int i = 0; i < 2; i++)
            {
                PostMessage(targetProcess, WM_KEYDOWN, 0x11, 0x001D0001);
                PostMessage(targetProcess, WM_KEYDOWN, vk, scanCode);
            }
        }

        public static void PressBtnAlt(int key, IntPtr targetProcess)
        {
            uint vk = (uint)GetArchlordBarKeys()[key];
            uint scanCode = (MapVirtualKey(vk, MAPVK_VK_TO_VSC) << 16) & 0x00FF0000;

            for (int i = 0; i < 2; i++)
            {
                PostMessage(targetProcess, WM_KEYDOWN, 0x12, 0x0F380005);
                PostMessage(targetProcess, WM_KEYDOWN, vk, scanCode);
            }
        }

        public static bool IsColorsEqual(Color c1, Color c2)
        {
            return c1.ToArgb() == c2.ToArgb();
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

        public static System.Drawing.Size GetCoords(IntPtr hWnd)
        {
            RECT pRect;
            System.Drawing.Size cSize = new System.Drawing.Size();
            // get coordinates relative to window
            GetWindowRect(hWnd, out pRect);

            cSize.Width = pRect.Right - pRect.Left;
            cSize.Height = pRect.Bottom - pRect.Top;

            cSize.Width /= 2;
            cSize.Width -= 238;

            cSize.Height -= 82;

            return cSize;
        }
        public static int GetYOffsetFromBar(int barNum)
        {
            if (barNum == 2) return -53;
            else if (barNum == 3) return -106;
            else if (barNum == 4) return -158;

            return 0;
        }
    }
}
