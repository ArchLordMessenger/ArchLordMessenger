using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MacroSwitch
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        //730 1036
        int baseX = 730;
        int YHeight = 1036;


        List<int> row1Indexes = new List<int>();
        List<int> row2Indexes = new List<int>();
        List<int> row1XOffsets = new List<int>();
        List<int> row1YOffsets = new List<int>();
        List<int> row2XOffsets = new List<int>();
        List<int> row2YOffsets = new List<int>();

        public IntPtr targetProcess = IntPtr.Zero;

        int MacroProgress;
        bool bRepeat = false;
        int CurrentBar;


        List<Color> row1PixelList = new List<Color>();
        List<Color> row2PixelList = new List<Color>();
        bool bPrepared = false;


        ColorPicker picker;
        SwitchTool switcher;
        SaveState state;

        string processName = "Archonia";
        public Form1()
        {
            InitializeComponent();
            SetWindowTitle();

            state = new SaveState();

            int FirstHotkeyId = 1;
            int FirstHotKeyKey = (int)Keys.F2;
            Boolean F2Registered = RegisterHotKey(
                this.Handle, FirstHotkeyId, 0x0000, FirstHotKeyKey
            );

            if (!F2Registered)
            {
                MessageBox.Show("Global Hotkey '[F2]' couldn't be registered !");
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("config.xml"))
            {
                loadConfig();
            }

            baseX = state.XCoord;
            YHeight = state.YCoord;
            txtXCoord.Text = state.XCoord.ToString();
            txtYCoord.Text = state.YCoord.ToString();
            txtDelay.Text = state.Delay.ToString();
            Row1BarNum.Value = state.Bar1Num;
            Row2BarNum.Value = state.Bar2Num;
            Row1Skill1.Checked = state.Row1.FirstOrDefault();
            Row1Skill2.Checked = state.Row1[1];
            Row1Skill3.Checked = state.Row1[2];
            Row1Skill4.Checked = state.Row1[3];
            Row1Skill5.Checked = state.Row1[4];
            Row1Skill6.Checked = state.Row1[5];
            Row1Skill7.Checked = state.Row1[6];
            Row1Skill8.Checked = state.Row1[7];
            Row1Skill9.Checked = state.Row1[8];
            Row1Skill10.Checked = state.Row1[9];
            Row2Skill1.Checked = state.Row2.FirstOrDefault();
            Row2Skill2.Checked = state.Row2[1];
            Row2Skill3.Checked = state.Row2[2];
            Row2Skill4.Checked = state.Row2[3];
            Row2Skill5.Checked = state.Row2[4];
            Row2Skill6.Checked = state.Row2[5];
            Row2Skill7.Checked = state.Row2[6];
            Row2Skill8.Checked = state.Row2[7];
            Row2Skill9.Checked = state.Row2[8];
            Row2Skill10.Checked = state.Row2[9];
            Row1Skill1X.Text = state.Row1OffsetX.FirstOrDefault().ToString();
            Row1Skill2X.Text = state.Row1OffsetX[1].ToString();
            Row1Skill3X.Text = state.Row1OffsetX[2].ToString();
            Row1Skill4X.Text = state.Row1OffsetX[3].ToString();
            Row1Skill5X.Text = state.Row1OffsetX[4].ToString();
            Row1Skill6X.Text = state.Row1OffsetX[5].ToString();
            Row1Skill7X.Text = state.Row1OffsetX[6].ToString();
            Row1Skill8X.Text = state.Row1OffsetX[7].ToString();
            Row1Skill9X.Text = state.Row1OffsetX[8].ToString();
            Row1Skill10X.Text = state.Row1OffsetX[9].ToString();
            Row1Skill1Y.Text = state.Row1OffsetY.FirstOrDefault().ToString();
            Row1Skill2Y.Text = state.Row1OffsetY[1].ToString();
            Row1Skill3Y.Text = state.Row1OffsetY[2].ToString();
            Row1Skill4Y.Text = state.Row1OffsetY[3].ToString();
            Row1Skill5Y.Text = state.Row1OffsetY[4].ToString();
            Row1Skill6Y.Text = state.Row1OffsetY[5].ToString();
            Row1Skill7Y.Text = state.Row1OffsetY[6].ToString();
            Row1Skill8Y.Text = state.Row1OffsetY[7].ToString();
            Row1Skill9Y.Text = state.Row1OffsetY[8].ToString();
            Row1Skill10Y.Text = state.Row1OffsetY[9].ToString();
            Row2Skill1X.Text = state.Row2OffsetX.FirstOrDefault().ToString();
            Row2Skill2X.Text = state.Row2OffsetX[1].ToString();
            Row2Skill3X.Text = state.Row2OffsetX[2].ToString();
            Row2Skill4X.Text = state.Row2OffsetX[3].ToString();
            Row2Skill5X.Text = state.Row2OffsetX[4].ToString();
            Row2Skill6X.Text = state.Row2OffsetX[5].ToString();
            Row2Skill7X.Text = state.Row2OffsetX[6].ToString();
            Row2Skill8X.Text = state.Row2OffsetX[7].ToString();
            Row2Skill9X.Text = state.Row2OffsetX[8].ToString();
            Row2Skill10X.Text = state.Row2OffsetX[9].ToString();
            Row2Skill1Y.Text = state.Row2OffsetY.FirstOrDefault().ToString();
            Row2Skill2Y.Text = state.Row2OffsetY[1].ToString();
            Row2Skill3Y.Text = state.Row2OffsetY[2].ToString();
            Row2Skill4Y.Text = state.Row2OffsetY[3].ToString();
            Row2Skill5Y.Text = state.Row2OffsetY[4].ToString();
            Row2Skill6Y.Text = state.Row2OffsetY[5].ToString();
            Row2Skill7Y.Text = state.Row2OffsetY[6].ToString();
            Row2Skill8Y.Text = state.Row2OffsetY[7].ToString();
            Row2Skill9Y.Text = state.Row2OffsetY[8].ToString();
            Row2Skill10Y.Text = state.Row2OffsetY[9].ToString();
            cbNewBar.Checked = state.bUseNewBar;
            cbSwitch.Checked = state.bSwitch;
        }

        private void loadConfig()
        {
            XmlSerializer ser = new XmlSerializer(typeof(SaveState));
            using (FileStream fs = File.OpenRead("config.xml"))
            {
                state = (SaveState)ser.Deserialize(fs);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            writeConfig();
        }

        private void writeConfig()
        {
            using (StreamWriter sw = new StreamWriter("config.xml"))
            {

                state.XCoord = Int32.Parse(txtXCoord.Text);
                state.YCoord = Int32.Parse(txtYCoord.Text);
                state.Delay = Int32.Parse(txtDelay.Text);
                state.Bar1Num = (int)Row1BarNum.Value;
                state.Bar2Num = (int)Row2BarNum.Value;
                state.Row1 = new List<bool>() { Row1Skill1.Checked, Row1Skill2.Checked, Row1Skill3.Checked, Row1Skill4.Checked, Row1Skill5.Checked, Row1Skill6.Checked, Row1Skill7.Checked, Row1Skill8.Checked, Row1Skill9.Checked, Row1Skill10.Checked };
                state.Row2 = new List<bool>() { Row2Skill1.Checked, Row2Skill2.Checked, Row2Skill3.Checked, Row2Skill4.Checked, Row2Skill5.Checked, Row2Skill6.Checked, Row2Skill7.Checked, Row2Skill8.Checked, Row2Skill9.Checked, Row2Skill10.Checked };
                state.Row1OffsetX = new List<int>() { Int32.Parse(Row1Skill1X.Text), Int32.Parse(Row1Skill2X.Text), Int32.Parse(Row1Skill3X.Text), Int32.Parse(Row1Skill4X.Text), Int32.Parse(Row1Skill5X.Text), Int32.Parse(Row1Skill6X.Text), Int32.Parse(Row1Skill7X.Text), Int32.Parse(Row1Skill8X.Text), Int32.Parse(Row1Skill9X.Text), Int32.Parse(Row1Skill10X.Text) };
                state.Row1OffsetY = new List<int>() { Int32.Parse(Row1Skill1Y.Text), Int32.Parse(Row1Skill2Y.Text), Int32.Parse(Row1Skill3Y.Text), Int32.Parse(Row1Skill4Y.Text), Int32.Parse(Row1Skill5Y.Text), Int32.Parse(Row1Skill6Y.Text), Int32.Parse(Row1Skill7Y.Text), Int32.Parse(Row1Skill8Y.Text), Int32.Parse(Row1Skill9Y.Text), Int32.Parse(Row1Skill10Y.Text) };
                state.Row2OffsetX = new List<int>() { Int32.Parse(Row2Skill1X.Text), Int32.Parse(Row2Skill2X.Text), Int32.Parse(Row2Skill3X.Text), Int32.Parse(Row2Skill4X.Text), Int32.Parse(Row2Skill5X.Text), Int32.Parse(Row2Skill6X.Text), Int32.Parse(Row2Skill7X.Text), Int32.Parse(Row2Skill8X.Text), Int32.Parse(Row2Skill9X.Text), Int32.Parse(Row2Skill10X.Text) };
                state.Row2OffsetY = new List<int>() { Int32.Parse(Row2Skill1Y.Text), Int32.Parse(Row2Skill2Y.Text), Int32.Parse(Row2Skill3Y.Text), Int32.Parse(Row2Skill4Y.Text), Int32.Parse(Row2Skill5Y.Text), Int32.Parse(Row2Skill6Y.Text), Int32.Parse(Row2Skill7Y.Text), Int32.Parse(Row2Skill8Y.Text), Int32.Parse(Row2Skill9Y.Text), Int32.Parse(Row2Skill10Y.Text) };
                state.bUseNewBar = cbNewBar.Checked;
                state.bSwitch = cbSwitch.Checked;
                XmlSerializer ser = new XmlSerializer(typeof(SaveState));
                ser.Serialize(sw, state);
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
                        if (bRepeat)
                        {
                            bRepeat = false;
                            MacroProgress = 999;
                            if (!cbNewBar.Checked)
                            {
                                while (CurrentBar != Convert.ToInt32(txtStartBar.Text))
                                {
                                    MacroHelper.SendBarUp(targetProcess);
                                    IncrementCurrBar();
                                }
                            }
                            if (cbSwitch.Checked) switcher?.MakeSwitch();
                        }
                        else
                        {
                            Macro();
                        }
                        break;
                }
                Thread.Sleep(200);
            }

            base.WndProc(ref m);
        }

        private void Macro()
        {
            InitMacroDefaults();
            Prepare();
            IntPtr procHandle = Process.GetProcessesByName(processName).Where(x => x.MainWindowHandle == targetProcess).ToList().FirstOrDefault().MainWindowHandle;

            if (procHandle == targetProcess)
            {
                this.Invoke(new ThreadStart(() => ExecuteMacro()));
            }
        }

        private void IncrementCurrBar()
        {
            _ = CurrentBar >= 4 ? CurrentBar = 1 : CurrentBar += 1;
        }

        public Color GetPixelColorSimplified(IntPtr hwnd, int x, int y)
        {
            return MacroHelper.GetPixelColor(hwnd, x, y);
        }

        private void Prepare()
        {
            //baseX = Int32.Parse(txtXCoord.Text);
            //YHeight = Int32.Parse(txtYCoord.Text);

            baseX = MacroHelper.GetCoords(targetProcess).Width;
            YHeight = MacroHelper.GetCoords(targetProcess).Height;

            if (!bPrepared)
            {
                row1PixelList.Clear();
                row2PixelList.Clear();


                CurrentBar = Convert.ToInt32(txtStartBar.Text);

                if (row1Indexes.Count > 0 && !cbNewBar.Checked)
                {
                    while (CurrentBar != Row1BarNum.Value)
                    {
                        MacroHelper.SendBarUp(targetProcess);
                        IncrementCurrBar();
                    }
                    Thread.Sleep(30);
                    for (int i = 0; i < row1Indexes.Count; i++)
                    {
                        int currX = baseX + row1XOffsets[i] + (row1Indexes[i] * 50) + (row1Indexes[i] * 2);
                        int currY = YHeight + row1YOffsets[i];
                        row1PixelList.Add(GetPixelColorSimplified(targetProcess, currX, currY));
                    }
                }
                else if (row1Indexes.Count > 0)
                {
                    for (int i = 0; i < row1Indexes.Count; i++)
                    {
                        int offset = 0;
                        if ((int)Row1BarNum.Value > 1 && row1Indexes[i] >= 5) offset = -1;
                        int currX = baseX + row1XOffsets[i] + (row1Indexes[i] * 50) + (row1Indexes[i] * 2) + offset;
                        int currY = YHeight + row1YOffsets[i] + MacroHelper.GetYOffsetFromBar((int)Row1BarNum.Value);
                        row1PixelList.Add(GetPixelColorSimplified(targetProcess, currX, currY));
                    }
                }
                if (row2Indexes.Count > 0 && !cbNewBar.Checked)
                {
                    while (CurrentBar != Row2BarNum.Value)
                    {
                        MacroHelper.SendBarUp(targetProcess);
                        IncrementCurrBar();
                    }
                    Thread.Sleep(30);
                    for (int i = 0; i < row2Indexes.Count; i++)
                    {
                        int currX = baseX + row2XOffsets[i] + (row2Indexes[i] * 50) + (row2Indexes[i] * 2);
                        int currY = YHeight + row2YOffsets[i];
                        row2PixelList.Add(GetPixelColorSimplified(targetProcess, currX, currY));
                    }
                }
                else if (row2Indexes.Count > 0)
                {
                    for (int i = 0; i < row2Indexes.Count; i++)
                    {
                        int offset = 0;
                        if ((int)Row2BarNum.Value > 1 && row2Indexes[i] >= 5) offset = -1;
                        int currX = baseX + row2XOffsets[i] + (row2Indexes[i] * 50) + (row2Indexes[i] * 2) + offset;
                        int currY = YHeight + row2YOffsets[i] + MacroHelper.GetYOffsetFromBar((int)Row2BarNum.Value);
                        row2PixelList.Add(GetPixelColorSimplified(targetProcess, currX, currY));
                    }
                }

                while (CurrentBar != Row1BarNum.Value && !cbNewBar.Checked)
                {
                    MacroHelper.SendBarUp(targetProcess);
                    IncrementCurrBar();
                }

                bPrepared = true;
            }
        }
        // I literally cannot be asked to make those long static functions shorter.

        private void InitMacroDefaults()
        {
            row1Indexes.Clear();
            row1XOffsets.Clear();
            row1YOffsets.Clear();
            if (Row1Skill1.Checked)
            {
                row1Indexes.Add(0);
                row1XOffsets.Add(Int32.Parse(Row1Skill1X.Text));
                row1YOffsets.Add(Int32.Parse(Row1Skill1Y.Text));
            }
            if (Row1Skill2.Checked)
            {
                row1Indexes.Add(1);
                row1XOffsets.Add(Int32.Parse(Row1Skill2X.Text));
                row1YOffsets.Add(Int32.Parse(Row1Skill2Y.Text));
            }
            if (Row1Skill3.Checked)
            {
                row1Indexes.Add(2);
                row1XOffsets.Add(Int32.Parse(Row1Skill3X.Text));
                row1YOffsets.Add(Int32.Parse(Row1Skill3Y.Text));
            }
            if (Row1Skill4.Checked)
            {
                row1Indexes.Add(3);
                row1XOffsets.Add(Int32.Parse(Row1Skill4X.Text));
                row1YOffsets.Add(Int32.Parse(Row1Skill4Y.Text));
            }
            if (Row1Skill5.Checked)
            {
                row1Indexes.Add(4);
                row1XOffsets.Add(Int32.Parse(Row1Skill5X.Text));
                row1YOffsets.Add(Int32.Parse(Row1Skill5Y.Text));
            }
            if (Row1Skill6.Checked)
            {
                row1Indexes.Add(5);
                row1XOffsets.Add(Int32.Parse(Row1Skill6X.Text));
                row1YOffsets.Add(Int32.Parse(Row1Skill6Y.Text));
            }
            if (Row1Skill7.Checked)
            {
                row1Indexes.Add(6);
                row1XOffsets.Add(Int32.Parse(Row1Skill7X.Text));
                row1YOffsets.Add(Int32.Parse(Row1Skill7Y.Text));
            }
            if (Row1Skill8.Checked)
            {
                row1Indexes.Add(7);
                row1XOffsets.Add(Int32.Parse(Row1Skill8X.Text));
                row1YOffsets.Add(Int32.Parse(Row1Skill8Y.Text));
            }
            if (Row1Skill9.Checked)
            {
                row1Indexes.Add(8);
                row1XOffsets.Add(Int32.Parse(Row1Skill9X.Text));
                row1YOffsets.Add(Int32.Parse(Row1Skill9Y.Text));
            }
            if (Row1Skill10.Checked)
            {
                row1Indexes.Add(9);
                row1XOffsets.Add(Int32.Parse(Row1Skill10X.Text));
                row1YOffsets.Add(Int32.Parse(Row1Skill10Y.Text));
            }

            row2Indexes.Clear();
            row2XOffsets.Clear();
            row2YOffsets.Clear();
            if (Row2Skill1.Checked)
            {
                row2Indexes.Add(0);
                row2XOffsets.Add(Int32.Parse(Row2Skill1X.Text));
                row2YOffsets.Add(Int32.Parse(Row2Skill1Y.Text));
            }
            if (Row2Skill2.Checked)
            {
                row2Indexes.Add(1);
                row2XOffsets.Add(Int32.Parse(Row2Skill2X.Text));
                row2YOffsets.Add(Int32.Parse(Row2Skill2Y.Text));
            }
            if (Row2Skill3.Checked)
            {
                row2Indexes.Add(2);
                row2XOffsets.Add(Int32.Parse(Row2Skill3X.Text));
                row2YOffsets.Add(Int32.Parse(Row2Skill3Y.Text));
            }
            if (Row2Skill4.Checked)
            {
                row2Indexes.Add(3);
                row2XOffsets.Add(Int32.Parse(Row2Skill4X.Text));
                row2YOffsets.Add(Int32.Parse(Row2Skill4Y.Text));
            }
            if (Row2Skill5.Checked)
            {
                row2Indexes.Add(4);
                row2XOffsets.Add(Int32.Parse(Row2Skill5X.Text));
                row2YOffsets.Add(Int32.Parse(Row2Skill5Y.Text));
            }
            if (Row2Skill6.Checked)
            {
                row2Indexes.Add(5);
                row2XOffsets.Add(Int32.Parse(Row2Skill6X.Text));
                row2YOffsets.Add(Int32.Parse(Row2Skill6Y.Text));
            }
            if (Row2Skill7.Checked)
            {
                row2Indexes.Add(6);
                row2XOffsets.Add(Int32.Parse(Row2Skill7X.Text));
                row2YOffsets.Add(Int32.Parse(Row2Skill7Y.Text));
            }
            if (Row2Skill8.Checked)
            {
                row2Indexes.Add(7);
                row2XOffsets.Add(Int32.Parse(Row2Skill8X.Text));
                row2YOffsets.Add(Int32.Parse(Row2Skill8Y.Text));
            }
            if (Row2Skill9.Checked)
            {
                row2Indexes.Add(8);
                row2XOffsets.Add(Int32.Parse(Row2Skill9X.Text));
                row2YOffsets.Add(Int32.Parse(Row2Skill9Y.Text));
            }
            if (Row2Skill10.Checked)
            {
                row2Indexes.Add(9);
                row2XOffsets.Add(Int32.Parse(Row2Skill10X.Text));
                row2YOffsets.Add(Int32.Parse(Row2Skill10Y.Text));
            }


            try
            {
                IntPtr currHandle = targetProcess;
                targetProcess = GetForegroundWindow();
                if (currHandle != targetProcess)
                {
                    bPrepared = false;
                    Prepare();
                }
            }
            catch (Exception)
            {
                // Should never happen but fuck it amirite?
                MessageBox.Show("No foreground window found. Try again.", "An error has occured.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void ProcessMacroEntryBar1(int key, int delay)
        {
            int val = (int)Row1BarNum.Value;
            if (val == 1) MacroHelper.PressBtnNormal(key, targetProcess);
            else if (val == 2) MacroHelper.PressBtnShift(key, targetProcess);
            else if (val == 3) MacroHelper.PressBtnCtrl(key, targetProcess);

            Thread.Sleep(delay);
        }
        private void ProcessMacroEntryBar2(int key, int delay)
        {
            int val = (int)Row2BarNum.Value;
            if (val == 1) MacroHelper.PressBtnNormal(key, targetProcess);
            else if (val == 2) MacroHelper.PressBtnShift(key, targetProcess);
            else if (val == 3) MacroHelper.PressBtnCtrl(key, targetProcess);

            Thread.Sleep(delay);
        }

        private void ExecuteMacro()
        {
            CurrentBar = Convert.ToInt32(txtStartBar.Text);
            int delay = Int32.Parse(txtDelay.Text);

            bRepeat = true;
            if (bRepeat)
            {
                Task.Factory.StartNew(() =>
                {
                    if (cbSwitch.Checked) switcher?.MakeSwitch();
                    while (bRepeat)
                    {
                        if (row1Indexes.Count > 0)
                        {
                            if (!cbNewBar.Checked)
                            {
                                while (CurrentBar != Row1BarNum.Value)
                                {
                                    MacroHelper.SendBarUp(targetProcess);
                                    IncrementCurrBar();
                                }
                                Thread.Sleep(30);
                            }
                            for (MacroProgress = 0; MacroProgress < row1Indexes.Count; MacroProgress++)
                            {
                                if (!cbNewBar.Checked)
                                {
                                    while (MacroHelper.IsColorsEqual(row1PixelList[MacroProgress], GetPixelColorSimplified(targetProcess, baseX + row1XOffsets[MacroProgress] + (row1Indexes[MacroProgress] * 50) + (row1Indexes[MacroProgress] * 2), YHeight + row1YOffsets[MacroProgress])))
                                    {
                                        this.Invoke(new ThreadStart(() => ProcessMacroEntryBar1(row1Indexes[MacroProgress], delay)));
                                    }
                                }
                                else
                                {
                                    int offset = 0;
                                    if ((int)Row1BarNum.Value > 1 && row1Indexes[MacroProgress] >= 5) offset = -1;
                                    while (MacroHelper.IsColorsEqual(row1PixelList[MacroProgress], GetPixelColorSimplified(targetProcess, baseX + row1XOffsets[MacroProgress] + (row1Indexes[MacroProgress] * 50) + (row1Indexes[MacroProgress] * 2) + offset, YHeight + row1YOffsets[MacroProgress] + MacroHelper.GetYOffsetFromBar((int)Row1BarNum.Value))))
                                    {
                                        this.Invoke(new ThreadStart(() => ProcessMacroEntryBar1(row1Indexes[MacroProgress], delay)));
                                    }
                                }
                            }
                        }
                        if (row2Indexes.Count > 0)
                        {
                            if (!cbNewBar.Checked)
                            {
                                while (CurrentBar != Row2BarNum.Value)
                                {
                                    MacroHelper.SendBarUp(targetProcess);
                                    IncrementCurrBar();
                                }
                                Thread.Sleep(30);
                            }
                            for (MacroProgress = 0; MacroProgress < row2Indexes.Count; MacroProgress++)
                            {
                                if (!cbNewBar.Checked)
                                {
                                    while (MacroHelper.IsColorsEqual(row2PixelList[MacroProgress], GetPixelColorSimplified(targetProcess, baseX + row2XOffsets[MacroProgress] + (row2Indexes[MacroProgress] * 50) + (row2Indexes[MacroProgress] * 2), YHeight + row2YOffsets[MacroProgress])))
                                    {
                                        this.Invoke(new ThreadStart(() => ProcessMacroEntryBar2(row2Indexes[MacroProgress], delay)));
                                    }
                                }
                                else
                                {
                                    int offset = 0;
                                    if ((int)Row2BarNum.Value > 1 && row2Indexes[MacroProgress] >= 5) offset = -1;
                                    while (MacroHelper.IsColorsEqual(row2PixelList[MacroProgress], GetPixelColorSimplified(targetProcess, baseX + row2XOffsets[MacroProgress] + (row2Indexes[MacroProgress] * 50) + (row2Indexes[MacroProgress] * 2) + offset, YHeight + row2YOffsets[MacroProgress] + MacroHelper.GetYOffsetFromBar((int)Row2BarNum.Value))))
                                    {
                                        this.Invoke(new ThreadStart(() => ProcessMacroEntryBar2(row2Indexes[MacroProgress], delay)));
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }




        private void Row1Skill1X_TextChanged(object sender, EventArgs e)
        {
            bPrepared = false;
        }

        private void Row2Skill1_CheckedChanged(object sender, EventArgs e)
        {
            bPrepared = false;
        }

        private void Row1BarNum_ValueChanged(object sender, EventArgs e)
        {
            txtStartBar.Text = Row1BarNum.Value.ToString();
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            picker?.Close();
            picker = new ColorPicker();
            picker.alWindow = targetProcess;
            picker.Show();
        }

        private void btnSwitchOpen_Click(object sender, EventArgs e)
        {
            switcher?.Close();
            switcher = new SwitchTool();
            switcher.alWindow = targetProcess;
            switcher.Show();
        }

        private void cbNewBar_CheckedChanged(object sender, EventArgs e)
        {
            bPrepared = false;
        }

        private void SetWindowTitle()
        {
            Random rnd = new Random();
            this.Text = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 20).Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
