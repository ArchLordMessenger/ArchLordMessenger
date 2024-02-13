using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ArchlordMessenger
{
	public static class BotHelpers
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
		private const int VK_TAB = 0x09;
		private const uint WM_KEYUP = 0x0101;

		public static void PressTab(IntPtr alWindow)
		{
			uint vk = (uint)VK_TAB;
			uint scanCode = (MapVirtualKey(vk, MAPVK_VK_TO_VSC) << 16) & 0x00FF0000;

			// Simulate pressing the TAB key
			PostMessage(alWindow, WM_KEYDOWN, vk, scanCode);

			// Simulate releasing the TAB key
			PostMessage(alWindow, WM_KEYUP, vk, (scanCode));
		}

		public static void Pressz(IntPtr alWindow)
		{
			const uint vk = 0x5A; // Virtual key code for 'Z'
			uint scanCode = (MapVirtualKey(vk, MAPVK_VK_TO_VSC) << 16) & 0x00FF0000;

			// Simulate pressing the Z key
			PostMessage(alWindow, WM_KEYDOWN, vk, scanCode);

			// Wait for the specified duration
			//Thread.Sleep(2000);

			// Simulate releasing the Z key
			PostMessage(alWindow, WM_KEYUP, vk, (scanCode));
		}

	}
}
