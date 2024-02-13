using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.Control;

namespace MacroSwitch
{
    public class ALProcess
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

        public enum EquipmentLayout
        {
            CAP,
            TOP,
            KILT,
            BOOTS,
            GLOVES,
            WEAPON_1,
            WEAPON_2,
            RING_1,
            RING_2,
            NECKLACE
        }

        public List<System.Windows.Point> EquipmentLayoutOffsets = new List<System.Windows.Point>
        {
            new System.Windows.Point(0, 0), // CAP
            new System.Windows.Point(0, 61), // TOP
            new System.Windows.Point(0, 161), // KILT
            new System.Windows.Point(0, 294), // BOOTS
            new System.Windows.Point(62, 111), // GLOVES
            new System.Windows.Point(-68, 171), // WEAPON_1
            new System.Windows.Point(-68, 171), // WEAPON_2
            new System.Windows.Point(-68, 0), // RING_1
            new System.Windows.Point(-68, 59), // RING_2
            new System.Windows.Point(62, 0), // NECKLACE
        };

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("User32.Dll")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);
        private ControlCollection controls;
        public ControlCollection Controls
        {
            get { return controls; }
            set
            {
                if (controls != null) return;
                controls = value;
                foreach (Control child in controls)
                {
                    if ( child.GetType() == typeof(ComboBox) )
                    {
                        var x = child as ComboBox;
                        x.Items.Add("Empty");
                        x.Items.Add("Cap");
                        x.Items.Add("Top");
                        x.Items.Add("Kilt");
                        x.Items.Add("Boots");
                        x.Items.Add("Gloves");
                        x.Items.Add("Weapon 1");
                        x.Items.Add("Weapon 2");
                        x.Items.Add("Ring 1");
                        x.Items.Add("Ring 2");
                        x.Items.Add("Necklace");
                        x.Items.Add("Other (IGNORE)");
                        x.SelectedIndex = 0;
                    }
                }
            }
        }
        public IntPtr WindowHandle { get; set; }
        public System.Windows.Point FirstInventorySlotPosition { get; set; }
        public System.Windows.Point HelmSlotPosition { get; set; }
        public List<Color> CurrentInventory = new List<Color>();
        public List<Color> CurrentEquipment = new List<Color>();
        public List<bool> UsedArmory = new List<bool>();
        public Vector IconOffsetEquipment { get; set; }
        public Vector IconOffsetInventory { get; set; }
        public ALProcess(IntPtr Handle)
        {
            WindowHandle = Handle;
        }

        public ALProcess()
        {
            WindowHandle = IntPtr.Zero;
        }

        public void SetTargetHandle(IntPtr Handle)
        {
            WindowHandle = Handle;
        }

        public IntPtr GetTargetHandle()
        {
            return WindowHandle;
        }

        public static IntPtr GetActiveALWindow()
        {
            var clients = Process.GetProcessesByName("Alefclient")/*.Where(x => x.MainWindowHandle == GetForegroundWindow())*/;
            if (clients.Count() == 0)
            {
                MessageBox.Show("Error setting target window, defaulting to no target.");
                return IntPtr.Zero;
            }

            return clients.FirstOrDefault().MainWindowHandle;
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
        public System.Windows.Point GetRealLocationFor(int x, int y)
        {
            POINT point;
            point.X = x;
            point.Y = y;
            ClientToScreen(WindowHandle, ref point);

            return new System.Windows.Point(point.X, point.Y);
        }

        public System.Windows.Point GetLocationForEquipmentIdx(int index)
        {
            var point = new System.Windows.Point();
            point.X = HelmSlotPosition.X + EquipmentLayoutOffsets[index].X;
            point.Y = HelmSlotPosition.Y + EquipmentLayoutOffsets[index].Y;

            return point;
        }

        public void MoveToRealLocation(int x, int y)
        {
            var point = GetRealLocationFor(x, y);
            SetCursorPos((int)point.X, (int)point.Y);
        }

        public void InitializeFirstInventorySlot()
        {
            POINT point;
            GetCursorPos(out point);
            ScreenToClient(WindowHandle, ref point);

            var offset = GetIconOffsetFromPoint(point);

            FirstInventorySlotPosition = new System.Windows.Point(point.X - offset.X + 25, point.Y + offset.Y - 25);
            IconOffsetInventory = offset;
        }

        public void RefreshInventory()
        {
            CurrentInventory.Clear();

            for (int i = 0; i < 16; i++)
            {
                CurrentInventory.Add(GetPixelColor(WindowHandle, (int)GetPointForInventorySlot(i).X, (int)GetPointForInventorySlot(i).Y));
            }
        }

        public System.Windows.Point GetPointForInventorySlot(int index)
        {
            if (index < 4)
                return new System.Windows.Point((int)(FirstInventorySlotPosition.X + (52 * index)), (int)FirstInventorySlotPosition.Y);
            else if (index < 8)
                return new System.Windows.Point((int)(FirstInventorySlotPosition.X + (52 * (index - 4))), (int)FirstInventorySlotPosition.Y + 52);
            else if (index < 12)
                return new System.Windows.Point((int)(FirstInventorySlotPosition.X + (52 * (index - 8))), (int)FirstInventorySlotPosition.Y + 104);
            else
                return new System.Windows.Point((int)(FirstInventorySlotPosition.X + (52 * (index - 12))), (int)FirstInventorySlotPosition.Y + 156);
        }

        public void InitializeHelmetSlot()
        {
            POINT point;
            GetCursorPos(out point);
            ScreenToClient(WindowHandle, ref point);

            var offset = GetIconOffsetFromPoint(point);

            HelmSlotPosition = new System.Windows.Point(point.X - offset.X + 25, point.Y + offset.Y - 25);
            IconOffsetEquipment = offset;
        }

        public static IEnumerable<T> GetControlsOfType<T>(Control root)
            where T : Control
        {
            var t = root as T;
            if (t != null)
                yield return t;

            var container = root as ContainerControl;
            if (container != null)
                foreach (Control c in container.Controls)
                    foreach (var i in GetControlsOfType<T>(c))
                        yield return i;
        }

        public void InitializeUsedEquipment()
        {
            var boxes = new List<CheckBox>();
            UsedArmory.Clear();

            for (int i = 0; i < 10; i++)
            {
                var c = Controls.OfType<CheckBox>().ToList().Where(x => x.Name == $"EquipmentSlot{i+1}").FirstOrDefault();
                boxes.Add(c);
            }

            boxes.ForEach(cb => UsedArmory.Add(cb.Checked));
        }

        public void RefreshEquipment()
        {
            CurrentEquipment.Clear();
            for (int i = 0; i < EquipmentLayoutOffsets.Count; i++)
            {
                CurrentEquipment.Add(GetPixelColor(WindowHandle, (int)(HelmSlotPosition.X + EquipmentLayoutOffsets[i].X), (int)(HelmSlotPosition.Y + EquipmentLayoutOffsets[i].Y)));
            }
        }

        public int MakeLParam(int LoWord, int HiWord)
        {
            return (int)((HiWord << 16) | (LoWord & 0xFFFF));
        }

        public void EquipAtPosition(int x, int y)
        {
            int WM_LBUTTONDBLCLK = 0x0203;
            Color c = GetPixelColor(WindowHandle, x, y);
            while (GetPixelColor(WindowHandle, x, y) == c)
            {
                PostMessage(WindowHandle, (uint)WM_LBUTTONDBLCLK, 1, (uint)MakeLParam(x, y));
                PostMessage(WindowHandle, (uint)WM_LBUTTONDBLCLK, 1, (uint)MakeLParam(x, y));
            }
        }

        public void EquipGearSlot(int idx)
        {
            int WM_LBUTTONDBLCLK = 0x0203;
            int x = (int)GetLocationForEquipmentIdx(idx).X;
            int y = (int)GetLocationForEquipmentIdx(idx).Y;
            Color c = GetPixelColor(WindowHandle, x, y);
            while (GetPixelColor(WindowHandle, x, y) == c)
            {
                PostMessage(WindowHandle, (uint)WM_LBUTTONDBLCLK, 1, (uint)MakeLParam(x, y));
                PostMessage(WindowHandle, (uint)WM_LBUTTONDBLCLK, 1, (uint)MakeLParam(x, y));
                Thread.Sleep(15);
            }
            //RefreshEquipment();
            //RefreshInventory();
        }

        public void EquipInventorySlot(int idx)
        {
            int WM_LBUTTONDBLCLK = 0x0203;
            int x = (int)GetPointForInventorySlot(idx).X;
            int y = (int)GetPointForInventorySlot(idx).Y;
            Color c = GetPixelColor(WindowHandle, x, y);
            while (GetPixelColor(WindowHandle, x, y) == c)
            {
                PostMessage(WindowHandle, (uint)WM_LBUTTONDBLCLK, 1, (uint)MakeLParam(x, y));
                PostMessage(WindowHandle, (uint)WM_LBUTTONDBLCLK, 1, (uint)MakeLParam(x, y));
                Thread.Sleep(15);
            }
            //RefreshEquipment();
            //RefreshInventory();
        }

        public void UpdateInventory(int idx, Color newItem)
        {
            CurrentInventory[idx] = newItem;
        }

        public void UpdateEquipment(int idx, Color newItem)
        {
            CurrentEquipment[idx] = newItem;
        }

        public void UpdateComboBoxAtIdx(int idx, int newType)
        {
            var c = Controls.OfType<ComboBox>().ToList().Where(x => x.Name == $"InventorySlot{idx + 1}").FirstOrDefault();
            c.SelectedIndex = newType;
        }

        public bool IsBlack(Color c)
        {
            return c.R == 0 && c.G == 0 && c.B == 0;
        }

        public Vector GetIconOffset()
        {
            POINT point;
            GetCursorPos(out point);
            ScreenToClient(WindowHandle, ref point);

            int x, y;

            x = point.X;
            y = point.Y;

            while (!IsBlack(GetPixelColor(WindowHandle, x, y)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 1)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 2)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 3)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 4)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 5)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 6)))
            {
                x--;
            }

            while (!IsBlack(GetPixelColor(WindowHandle, point.X, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 1, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 2, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 3, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 4, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 5, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 6, y)))
            {
                y++;
            }

            return new Vector(point.X - x, y - point.Y);
        }

        public Vector GetIconOffsetFromPoint(POINT p)
        {
            var point = p;

            int x, y;

            x = point.X;
            y = point.Y;

            while (!IsBlack(GetPixelColor(WindowHandle, x, y)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 1)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 2)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 3)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 4)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 5)) && !IsBlack(GetPixelColor(WindowHandle, x, y + 6)))
            {
                x--;
            }

            while (!IsBlack(GetPixelColor(WindowHandle, point.X, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 1, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 2, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 3, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 4, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 5, y)) && !IsBlack(GetPixelColor(WindowHandle, point.X + 6, y)))
            {
                y++;
            }

            return new Vector(point.X - x, y - point.Y);
        }

        public int GetFirstSlotWithIdx(int idx)
        {
            var c = Controls.OfType<ComboBox>().ToList();
            for (int i = 0; i < 16; i++)
            {
                if (c.Where(x => x.Name == $"InventorySlot{i + 1}").FirstOrDefault()?.SelectedIndex == idx)
                    return i;
            }

            throw new Exception($"Failed to find a slot with item {(EquipmentLayout)idx - 1}");
        }
        public int GetFirstSlotFromIdx(int start, int idx)
        {
            for (int i = start; i < 16; i++)
            {
                if (Controls.OfType<ComboBox>().ToList().Where(x => x.Name == $"InventorySlot{i + 1}").FirstOrDefault()?.SelectedIndex == idx)
                    return i;
            }

            throw new Exception($"Failed to find a slot with item {(EquipmentLayout)idx - 1}");
        }

        public int GetLastSlotWithIdx(int idx)
        {
            for (int i = 15; i > 0; i--)
            {
                if (Controls.OfType<ComboBox>().ToList().Where(x => x.Name == $"InventorySlot{i + 1}").FirstOrDefault()?.SelectedIndex == idx)
                    return i;
            }

            throw new Exception($"Failed to find a slot with item {(EquipmentLayout)idx - 1}");
        }

        public void MakeSwitchReplace()
        {
            InitializeUsedEquipment();
            for (int i = 0; i < UsedArmory.Count(); i++)
            {
                if (i == (int)EquipmentLayout.RING_1 && UsedArmory[i] && UsedArmory[i + 1])
                {
                    SwapRings();
                    SwapGearForArmorIdx(i + 1);
                    i++;
                    continue;
                }

                if (UsedArmory[i])
                {
                    SwapGearForArmorIdx(i);
                }
            }
        }

        public void SwapRings()
        {
            int idxR1 = GetFirstSlotWithIdx((int)EquipmentLayout.RING_1 + 1);

            EquipGearSlot((int)EquipmentLayout.RING_1);
            UpdateComboBoxAtIdx(GetFirstSlotWithIdx(0), (int)EquipmentLayout.RING_1 + 1);

            int idxEmpty = GetFirstSlotWithIdx(0);

            EquipInventorySlot(idxR1);
            UpdateComboBoxAtIdx(idxR1, 0);
        }

        public void SwapGearForArmorIdx(int idx)
        {
            var c = CurrentInventory[idx];
            EquipInventorySlot(GetFirstSlotWithIdx(idx + 1));
            int idxEmpty = GetFirstSlotWithIdx(0);
            int idxItem = GetFirstSlotWithIdx(idx + 1);

            if (idxEmpty > idxItem)
            {
                UpdateComboBoxAtIdx(GetFirstSlotWithIdx(0), idx + 1);
                UpdateComboBoxAtIdx(GetFirstSlotWithIdx(idx + 1), 0);
            }
            else
            {
                UpdateComboBoxAtIdx(GetFirstSlotWithIdx(idx + 1), 0);
                UpdateComboBoxAtIdx(GetFirstSlotWithIdx(0), idx + 1);
            }
            UpdateInventory(GetFirstSlotWithIdx(0), new Color());
            UpdateEquipment(idx, c);
        }
    }
}
