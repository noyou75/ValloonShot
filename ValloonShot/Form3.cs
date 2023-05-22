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
    public partial class Form3 : Form
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

        public static Rectangle R;

        public Form3()
        {
            InitializeComponent();
            //var screenBounds = Screen.AllScreens.First().Bounds;
            var screenBounds = Screen.AllScreens.Last().Bounds;
            R = new Rectangle
            {
                X = screenBounds.X,
                Y = screenBounds.Y + screenBounds.Height - 47,
                Width = screenBounds.Width - 82,
                Height = 47,
            };
            //{
            //    X = screenBounds.X + 74,
            //    Y = screenBounds.Y + screenBounds.Height - 30,
            //    Width = screenBounds.Width - 74 - 40,
            //    Height = 30,
            //};
        }

        //public enum GWL
        //{
        //    ExStyle = -20
        //}

        //public enum WS_EX
        //{
        //    Transparent = 0x20,
        //    Layered = 0x80000
        //}

        //public enum LWA
        //{
        //    ColorKey = 0x1,
        //    Alpha = 0x2
        //}

        //[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        //public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        //[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        //public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        //[DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        //public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);


        private void Form3_Load(object sender, EventArgs e)
        {
            this.Bounds = R;
            //int wl = GetWindowLong(this.Handle, GWL.ExStyle);
            //wl = wl | (int)WS_EX.Layered | (int)WS_EX.Transparent;
            //SetWindowLong(this.Handle, GWL.ExStyle, wl);
            //SetLayeredWindowAttributes(this.Handle, 0, 255, LWA.Alpha);
        }
    }
}
