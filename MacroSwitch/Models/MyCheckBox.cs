using System;
using System.Drawing;
using System.Windows.Forms;

namespace MacroSwitch
{
    public class MyCheckBox : CheckBox
    {
        public MyCheckBox()
        {
            this.TextAlign = ContentAlignment.MiddleRight;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = false; }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);
            int h = this.ClientSize.Height - 2;
            var rc = new Rectangle(new Point(0, this.Height / 2 - h / 2), new Size(h, h));
            ControlPaint.DrawCheckBox(e.Graphics, rc,
                this.CheckState == CheckState.Checked ? ButtonState.Checked : (this.CheckState == CheckState.Unchecked ? ButtonState.Normal : ButtonState.Flat));
        }
    }
}
