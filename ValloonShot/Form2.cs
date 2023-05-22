using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace com.valloon.ValloonShot
{
    public partial class Form2 : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

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
        const uint WDA_NONE = 0x00000000;
        const uint WDA_MONITOR = 0x00000001;
        const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        //
        private void Form2_Load(object sender, EventArgs e)
        {
            //IntPtr handle = GetForegroundWindow();
            var handle = this.Handle;

            //هذه الدالة خاصة بمنع تصوير الفورم
            SetWindowDisplayAffinity(handle, WDA_MONITOR);
            //هذه الدوال تختص بجعل الفورم يمكن النقر من خلاله
            this.AllowTransparency = true;
            SetWindowLong(handle, GWL_EXSTYLE, (IntPtr)(GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_LAYERED | WS_EX_TRANSPARENT));

            //this.Bounds = Screen.AllScreens.First().Bounds;
            this.Bounds = Screen.AllScreens.Last().Bounds;
            this.Height -= 47;
            this.BackColor = Color.Crimson;
            this.TransparencyKey = Color.Crimson;
            this.TopMost = true;
        }
    }
}
