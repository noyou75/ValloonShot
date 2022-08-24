using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.valloon.ValloonShot
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            var screenBounds = Screen.AllScreens.Last().Bounds;
            this.Bounds = new Rectangle
            {
                X = screenBounds.X + 74,
                Y = screenBounds.Y + screenBounds.Height - 30,
                Width = screenBounds.Width - 74 - 40,
                Height = 30,
            };
        }
    }
}
