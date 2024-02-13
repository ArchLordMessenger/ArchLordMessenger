using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ArchlordMessenger
{
    public partial class SwitchTool : Form
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        List<HotKey> hotKeys = new List<HotKey>();
        ALProcess gameInstance;
        public IntPtr alWindow;
        public SwitchTool()
        {
            InitializeComponent();
            SetWindowTitle();
            int FirstHotkeyId = 1;
            int FirstHotKeyKey = (int)Keys.F3;
            Boolean F3Registered = RegisterHotKey(
                this.Handle, FirstHotkeyId, 0x0000, FirstHotKeyKey
            );
            if (!F3Registered)
            {
                MessageBox.Show("Global Hotkey '[F3]' couldn't be registered !");
            }
            int SecondHotkeyId = 2;
            int SecondtHotKeyKey = (int)Keys.F4;
            Boolean F4Registered = RegisterHotKey(
                this.Handle, SecondHotkeyId, 0x0000, SecondtHotKeyKey
            );
            if (!F4Registered)
            {
                MessageBox.Show("Global Hotkey '[F4]' couldn't be registered !");
            }
            int ThirdHotkeyId = 3;
            int ThirdHotKeyKey = (int)Keys.F5;
            Boolean F5Registered = RegisterHotKey(
                this.Handle, ThirdHotkeyId, 0x0000, ThirdHotKeyKey
            );
            if (!F5Registered)
            {
                MessageBox.Show("Global Hotkey '[F5]' couldn't be registered !");
            }
            int FourthHotkeyId = 4;
            int FourthHotKeyKey = (int)Keys.F6;
            Boolean F6Registered = RegisterHotKey(
                this.Handle, FourthHotkeyId, 0x0000, FourthHotKeyKey
            );
            if (!F6Registered)
            {
                MessageBox.Show("Global Hotkey '[F6]' couldn't be registered !");
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();
                switch (id)
                {
                    case 1:
                        SelectTargetWindow();
                        break;
                    case 2:
                        InitializeInventory();
                        break;
                    case 3:
                        InitializeEquipment();
                        break;
                    case 4:
                        MakeSwitch();
                        break;
                }
            }

            base.WndProc(ref m);
        }

        private void SelectTargetWindow()
        {
            gameInstance = new ALProcess(alWindow);

            gameInstance.Controls = Controls;

        }
        private void InitializeInventory()
        {
            gameInstance?.InitializeFirstInventorySlot();
            gameInstance?.RefreshInventory();
        }

        private void InitializeEquipment()
        {
            gameInstance?.InitializeHelmetSlot();
            gameInstance?.InitializeUsedEquipment();
            gameInstance?.RefreshEquipment();
        }

        public void MakeSwitch()
        {
            gameInstance?.MakeSwitchReplace();
        }

        private void SetWindowTitle()
        {
            Random rnd = new Random();
            this.Text = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 20).Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
