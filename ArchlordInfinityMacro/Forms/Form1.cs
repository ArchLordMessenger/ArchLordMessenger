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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ArchlordInfinityMacro
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
		Settings state;

		string processName = "Archonia";


		public Form1()
		{
			InitializeComponent();
			SetWindowTitle();

			state = new Settings();

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
		private protected string txtXCoord_Text = "0";
		private protected string txtYCoord_Text = "0";
		private protected string txtDelay_Text = "0";
		private protected string txtStartBar_Text = "1";
		private protected int Row1BarNum_Value = 1;
		private protected int Row2BarNum_Value = 2;
		private protected int Row3BarNum_Value = 3;
		private protected bool cbNewBar_Checked = true;
		private protected bool cbSwitch_Checked = false;
		private void Form1_Load(object sender, EventArgs e)
		{
			if (File.Exists("settings.xml"))
			{
				loadConfig();
			}

			baseX = 0;
			YHeight = 0;

			Row1Skill1.Checked = state.Row1 != null && state.Row1.FirstOrDefault();
			Row1Skill2.Checked = state.Row1 != null && state.Row1[1];
			Row1Skill3.Checked = state.Row1 != null && state.Row1[2];
			Row1Skill4.Checked = state.Row1 != null && state.Row1[3];
			Row1Skill5.Checked = state.Row1 != null && state.Row1[4];
			Row1Skill6.Checked = state.Row1 != null && state.Row1[5];
			Row1Skill7.Checked = state.Row1 != null && state.Row1[6];
			Row1Skill8.Checked = state.Row1 != null && state.Row1[7];
			Row1Skill9.Checked = state.Row1 != null && state.Row1[8];
			Row1Skill10.Checked = state.Row1 != null && state.Row1[9];
			Row2Skill1.Checked = state.Row1 != null && state.Row2.FirstOrDefault();
			Row2Skill2.Checked = state.Row1 != null && state.Row2[1];
			Row2Skill3.Checked = state.Row1 != null && state.Row2[2];
			Row2Skill4.Checked = state.Row1 != null && state.Row2[3];
			Row2Skill5.Checked = state.Row1 != null && state.Row2[4];
			Row2Skill6.Checked = state.Row1 != null && state.Row2[5];
			Row2Skill7.Checked = state.Row1 != null && state.Row2[6];
			Row2Skill8.Checked = state.Row1 != null && state.Row2[7];
			Row2Skill9.Checked = state.Row1 != null && state.Row2[8];
			Row2Skill10.Checked = state.Row1 != null && state.Row2[9];
			//Row3Skill1.Checked = state.Row1 != null && state.Row3.FirstOrDefault();
			//Row3Skill2.Checked = state.Row1 != null && state.Row3[1];
			//Row3Skill3.Checked = state.Row1 != null && state.Row3[2];
			//Row3Skill4.Checked = state.Row1 != null && state.Row3[3];
			//Row3Skill5.Checked = state.Row1 != null && state.Row3[4];
			//Row3Skill6.Checked = state.Row1 != null && state.Row3[5];
			//Row3Skill7.Checked = state.Row1 != null && state.Row3[6];
			//Row3Skill8.Checked = state.Row1 != null && state.Row3[7];
			//Row3Skill9.Checked = state.Row1 != null && state.Row3[8];
			//Row3Skill10.Checked = state.Row1 != null && state.Row3[9];


			listBox_ServerSelect.SelectedItem = listBox_ServerSelect.Items[0];
		}
		private void loadConfig()
		{
			try
			{
				XmlSerializer ser = new XmlSerializer(typeof(Settings));
				using (FileStream fs = File.OpenRead("settings.xml"))
				{
					state = (Settings)ser.Deserialize(fs);
				}
			}
			catch (Exception)
			{


			}

		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			writeConfig();
		}

		private void writeConfig()
		{
			using (StreamWriter sw = new StreamWriter("settings.xml"))
			{

				state.Row1 = new List<bool>() { Row1Skill1.Checked, Row1Skill2.Checked, Row1Skill3.Checked, Row1Skill4.Checked, Row1Skill5.Checked, Row1Skill6.Checked, Row1Skill7.Checked, Row1Skill8.Checked, Row1Skill9.Checked, Row1Skill10.Checked };
				state.Row2 = new List<bool>() { Row2Skill1.Checked, Row2Skill2.Checked, Row2Skill3.Checked, Row2Skill4.Checked, Row2Skill5.Checked, Row2Skill6.Checked, Row2Skill7.Checked, Row2Skill8.Checked, Row2Skill9.Checked, Row2Skill10.Checked };
				//state.Row3 = new List<bool>() { Row3Skill1.Checked, Row3Skill2.Checked, Row3Skill3.Checked, Row3Skill4.Checked, Row3Skill5.Checked, Row3Skill6.Checked, Row3Skill7.Checked, Row3Skill8.Checked, Row3Skill9.Checked, Row3Skill10.Checked };
				XmlSerializer ser = new XmlSerializer(typeof(Settings));
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
							//off
							//this.SetSkillsEnabled(true);
							picbox_running_off.Visible = true;
							picbox_running_on.Visible = false;

							bRepeat = false;
							MacroProgress = 999;
							if (!cbNewBar_Checked)
							{
								while (CurrentBar != Convert.ToInt32(txtStartBar_Text))
								{
									MacroHelper.SendBarUp(targetProcess);
									IncrementCurrBar();
								}
							}
							if (cbSwitch_Checked) switcher?.MakeSwitch();
						}
						else
						{
							//on
							//this.SetSkillsEnabled(false);
							picbox_running_off.Visible = false;
							picbox_running_on.Visible = true;
							Macro();
						}
						break;
				}
				Thread.Sleep(200);
			}

			base.WndProc(ref m);
		}
		private void SetSkillsEnabled(bool Enabled)
		{
			Row1Skill1.Enabled = Enabled;
			Row1Skill2.Enabled = Enabled;
			Row1Skill3.Enabled = Enabled;
			Row1Skill4.Enabled = Enabled;
			Row1Skill5.Enabled = Enabled;
			Row1Skill6.Enabled = Enabled;
			Row1Skill7.Enabled = Enabled;
			Row1Skill8.Enabled = Enabled;
			Row1Skill9.Enabled = Enabled;
			Row1Skill10.Enabled = Enabled;

			Row2Skill1.Enabled = Enabled;
			Row2Skill2.Enabled = Enabled;
			Row2Skill3.Enabled = Enabled;
			Row2Skill4.Enabled = Enabled;
			Row2Skill5.Enabled = Enabled;
			Row2Skill6.Enabled = Enabled;
			Row2Skill7.Enabled = Enabled;
			Row2Skill8.Enabled = Enabled;
			Row2Skill9.Enabled = Enabled;
			Row2Skill10.Enabled = Enabled;
		}
		private void Macro()
		{
			InitMacroDefaults();
			Prepare();

			var clients = Process.GetProcessesByName(processName).ToList();
			if (clients.Count == 0)
			{
				this.Log($"No process running named {processName}");
				return;
			}

			var focusedClients = clients.Where(x => x.MainWindowHandle == targetProcess).ToList();
			if (!focusedClients.Any())
			{
				this.Log("client is not focused.");
				picbox_focused_off.Visible = true;
				picbox_focused_on.Visible = false;

				picbox_running_off.Visible = true;
				picbox_running_on.Visible = false;
				return;
			}
			else
			{
				picbox_focused_off.Visible = false;
				picbox_focused_on.Visible = true;

			}

			IntPtr procHandle = focusedClients.FirstOrDefault().MainWindowHandle;

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


				CurrentBar = Convert.ToInt32(txtStartBar_Text);

				if (row1Indexes.Count > 0 && !cbNewBar_Checked)
				{
					while (CurrentBar != Row1BarNum_Value)
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
						if ((int)Row1BarNum_Value > 1 && row1Indexes[i] >= 5) offset = -1;
						int currX = baseX + row1XOffsets[i] + (row1Indexes[i] * 50) + (row1Indexes[i] * 2) + offset;
						int currY = YHeight + row1YOffsets[i] + MacroHelper.GetYOffsetFromBar((int)Row1BarNum_Value);
						row1PixelList.Add(GetPixelColorSimplified(targetProcess, currX, currY));
					}
				}
				if (row2Indexes.Count > 0 && !cbNewBar_Checked)
				{
					while (CurrentBar != Row2BarNum_Value)
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
						if ((int)Row2BarNum_Value > 1 && row2Indexes[i] >= 5) offset = -1;
						int currX = baseX + row2XOffsets[i] + (row2Indexes[i] * 50) + (row2Indexes[i] * 2) + offset;
						int currY = YHeight + row2YOffsets[i] + MacroHelper.GetYOffsetFromBar((int)Row2BarNum_Value);
						row2PixelList.Add(GetPixelColorSimplified(targetProcess, currX, currY));
					}
				}

				while (CurrentBar != Row1BarNum_Value && !cbNewBar_Checked)
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

			Row1Skill1X.Text = "-5";
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

		private void ProcessMacroEntryBar1(List<int> list, int MacroProgress, int delay, string position = "")
		{
			if (MacroProgress == 999)
			{
				return;
			}
			var key = list[MacroProgress];
			int val = (int)Row1BarNum_Value;
			if (val == 1) MacroHelper.PressBtnNormal(key, targetProcess);
			else if (val == 2) MacroHelper.PressBtnShift(key, targetProcess);
			else if (val == 3) MacroHelper.PressBtnCtrl(key, targetProcess);
			this.Log($"pressed {++key} on position {position}");
			Thread.Sleep(delay);
		}
		private void ProcessMacroEntryBar2(int key, int delay)
		{
			int val = (int)Row2BarNum_Value;
			if (val == 1) MacroHelper.PressBtnNormal(key, targetProcess);
			else if (val == 2) MacroHelper.PressBtnShift(key, targetProcess);
			else if (val == 3) MacroHelper.PressBtnCtrl(key, targetProcess);

			Thread.Sleep(delay);
		}

		private void ExecuteMacro()
		{
			CurrentBar = Convert.ToInt32(txtStartBar_Text);
			int delay = Int32.Parse(txtDelay_Text);

			bRepeat = true;
			if (bRepeat)
			{
				Task.Factory.StartNew(() =>
				{
					if (cbSwitch_Checked) switcher?.MakeSwitch();
					while (bRepeat)
					{
						if (row1Indexes.Count > 0)
						{
							if (!cbNewBar_Checked)
							{
								while (CurrentBar != Row1BarNum_Value)
								{
									MacroHelper.SendBarUp(targetProcess);
									IncrementCurrBar();
								}
								Thread.Sleep(30);
							}
							for (MacroProgress = 0; MacroProgress < row1Indexes.Count; MacroProgress++)
							{
								if (!cbNewBar_Checked)
								{
									while (MacroProgress != 999 && MacroHelper.IsColorsEqual(row1PixelList[MacroProgress], GetPixelColorSimplified(targetProcess, baseX + row1XOffsets[MacroProgress] + (row1Indexes[MacroProgress] * 50) + (row1Indexes[MacroProgress] * 2), YHeight + row1YOffsets[MacroProgress])))
									{
										if (MacroProgress != 999)
										{
											this.Invoke(new ThreadStart(() => ProcessMacroEntryBar1(row1Indexes, MacroProgress, delay)));
										}
									}
								}
								else
								{
									int offset = 0;
									if ((int)Row1BarNum_Value > 1 && row1Indexes[MacroProgress] >= 5) offset = -1;
									while (MacroProgress != 999 && MacroHelper.IsColorsEqual(row1PixelList[MacroProgress], GetPixelColorSimplified(targetProcess, baseX + row1XOffsets[MacroProgress] + (row1Indexes[MacroProgress] * 50) + (row1Indexes[MacroProgress] * 2) + offset, YHeight + row1YOffsets[MacroProgress] + MacroHelper.GetYOffsetFromBar((int)Row1BarNum_Value))))
									{
										if (MacroProgress != 999)
										{
											var x = baseX + row1XOffsets[MacroProgress] + (row1Indexes[MacroProgress] * 50) + (row1Indexes[MacroProgress] * 2) + offset;
											var y = YHeight + row1YOffsets[MacroProgress] + MacroHelper.GetYOffsetFromBar((int)Row1BarNum_Value);
											this.Invoke(new ThreadStart(() => ProcessMacroEntryBar1(row1Indexes, MacroProgress, delay, $"x={x},y={y}")));

										}
									}
								}
							}
						}
						if (row2Indexes.Count > 0)
						{
							if (!cbNewBar_Checked)
							{
								while (CurrentBar != Row2BarNum_Value)
								{
									MacroHelper.SendBarUp(targetProcess);
									IncrementCurrBar();
								}
								Thread.Sleep(30);
							}
							for (MacroProgress = 0; MacroProgress < row2Indexes.Count; MacroProgress++)
							{
								if (!cbNewBar_Checked)
								{
									while (MacroHelper.IsColorsEqual(row2PixelList[MacroProgress], GetPixelColorSimplified(targetProcess, baseX + row2XOffsets[MacroProgress] + (row2Indexes[MacroProgress] * 50) + (row2Indexes[MacroProgress] * 2), YHeight + row2YOffsets[MacroProgress])))
									{
										this.Invoke(new ThreadStart(() => ProcessMacroEntryBar2(row2Indexes[MacroProgress], delay)));
									}
								}
								else
								{
									int offset = 0;
									if ((int)Row2BarNum_Value > 1 && row2Indexes[MacroProgress] >= 5) offset = -1;
									while (MacroHelper.IsColorsEqual(row2PixelList[MacroProgress], GetPixelColorSimplified(targetProcess, baseX + row2XOffsets[MacroProgress] + (row2Indexes[MacroProgress] * 50) + (row2Indexes[MacroProgress] * 2) + offset, YHeight + row2YOffsets[MacroProgress] + MacroHelper.GetYOffsetFromBar((int)Row2BarNum_Value))))
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
			txtStartBar_Text = Row1BarNum_Value.ToString();
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
			this.Text = "AIM.exe";
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//BotHelpers.PressTab(targetProcess);

			tab_Offsets.Visible = !tab_Offsets.Visible;
			btnDebug.Visible = !btnDebug.Visible;
			btnSwitchOpen.Visible = !btnSwitchOpen.Visible;

		}

		private void btn_ShowLogs_Click(object sender, EventArgs e)
		{
			LOGGING.Visible = !LOGGING.Visible;
			picbox_focused_off.Visible = !picbox_focused_off.Visible;
			picbox_focused_on.Visible = !picbox_focused_on.Visible;

		}

		private void Log(string log)
		{
			LOGGING.Text += $"[{DateTime.Now.ToString("mm:ss.fff")}]: {log}\n";
			LOGGING.SelectionStart = LOGGING.TextLength;
			LOGGING.ScrollToCaret();


		}

		private void btn_settings_Click(object sender, EventArgs e)
		{
			this.Log("Settings are disabled in version 1.");
			return;
			tab_Offsets.Visible = !tab_Offsets.Visible;
			btnDebug.Visible = !btnDebug.Visible;
			btnSwitchOpen.Visible = !btnSwitchOpen.Visible;
		}

		private void btn_close_Click(object sender, EventArgs e)
		{
			var processes = Process.GetProcessesByName(processName);
			foreach (var process in processes)
			{
				process.Kill();
			}

			this.Log($"Killed {processes.Count()} processes named {processName}.exe.");
		}

		private void btn_minimaze_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;

		}

		private void btn_exit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void btn_help_Click(object sender, EventArgs e)
		{
			var help = "Help menu:\n";
			help += "Press F2 to start\n";
			help += "Skills must be on cooldown on first time F2\n";
			help += "Only check skills u want to use (no auto attack or buffs)\n";
			help += "Every time u change skills in game, restart macro\n";
			help += "Report an issue ? https://github.com/ArchlordInfinityMacro";

			MessageBox.Show(help);
		}


		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool ReleaseCapture();
		private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void picbox_focused_Click(object sender, EventArgs e)
		{
			
				

		}

		private void picbox_focused_off_Click(object sender, EventArgs e)
		{

		}
	}
}
