using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.valloon.ValloonShot
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;
        //

        //هذه الدالة خاصة بمنع تصوير الفورم
        const uint WDA_MONITOR = 1;
        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);
        //
        private void Form2_Load(object sender, EventArgs e)
        {
            //هذه الدالة خاصة بمنع تصوير الفورم
            SetWindowDisplayAffinity(this.Handle, WDA_MONITOR);
            //هذه الدوال تختص بجعل الفورم يمكن النقر من خلاله
            this.AllowTransparency = true;
            SetWindowLong(this.Handle, GWL_EXSTYLE, (IntPtr)(GetWindowLong(this.Handle, GWL_EXSTYLE) | WS_EX_LAYERED | WS_EX_TRANSPARENT));

            this.Bounds = Screen.AllScreens.Last().Bounds;
            this.BackColor = Color.Crimson;
            this.TransparencyKey = Color.Crimson;
            this.TopMost = true;
        }
    }
}
