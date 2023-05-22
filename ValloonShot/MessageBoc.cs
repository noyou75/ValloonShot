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
    public partial class MessageBoc : Form
    {
        public MessageBoc()
        {
            InitializeComponent();
        }

        public MessageBoc(string title, string description)
        {
            InitializeComponent();

            this.Text = title;
            this.textBox_Description.Text = description;
        }

        public static DialogResult Show(string title, string description)
        {
            // using construct ensures the resources are freed when form is closed
            using (var form = new MessageBoc(title, description))
            {
                var result = form.ShowDialog();
                form.Activate();
                form.Focus();
                form.TopMost = true;
                form.button_OK.Focus();
                return result;
            }
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        const uint WDA_NONE = 0x00000000;
        const uint WDA_MONITOR = 0x00000001;
        const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        public static void Verify(IntPtr handle)
        {
            _ = SetWindowDisplayAffinity(handle, WDA_EXCLUDEFROMCAPTURE);
        }

        private void MessageBoc_Load(object sender, EventArgs e)
        {
            Verify(this.Handle);
        }
    }
}
