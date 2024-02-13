using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchlordMessenger
{
    public class SaveState
    {
        public int Delay { get; set; }
        public int Bar1Num { get; set; }
        public int Bar2Num { get; set; }
        public int XCoord { get; set; }
        public int YCoord { get; set; }
        public List<bool> Row1 { get; set; }
        public List<bool> Row2 { get; set; }
        public List<int> Row1OffsetX { get; set; }
        public List<int> Row1OffsetY { get; set; }
        public List<int> Row2OffsetX { get; set; }
        public List<int> Row2OffsetY { get; set; }
        public bool bUseNewBar { get; set; }
        public bool bSwitch { get; set; }
    }
}
