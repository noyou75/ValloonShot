using com.valloon.ValloonShot.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace com.valloon.ValloonShot
{
    public partial class Form1 : Form
    {

        private String extension = @"png";
        private readonly String path = Application.StartupPath + @"\";
        private readonly String path2 = Application.StartupPath + @"\_shot\";
        private Boolean StealthMode = false;
        private int interval1 = 10;
        private int interval2 = 1799;
        private readonly int hotkey = (int)Keys.Space;
        private String[] exceptWindowTitles = null;

        public Form1()
        {
            InitializeComponent();
        }

        public static String CorrectFileNames(String input)
        {
            String filename = input.Replace('/', ' ').Replace('\\', ' ').Replace(':', '.').Replace('*', ' ').Replace('?', ' ').Replace('\"', ' ').Replace('<', ' ').Replace('>', ' ').Replace('|', ' ');
            if (filename.Length > 200) filename = filename.Substring(0, 200) + "...";

            //string illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            filename = r.Replace(filename, "");

            return filename;
        }

        [DllImport("User32")]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,
            int id,
            int fsModifiers,
            int vk
        );
        [DllImport("User32")]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,
            int id
        );

        public const int MOD_WIN = 0x8;
        public const int MOD_SHIFT = 0x4;
        public const int MOD_CONTROL = 0x2;
        public const int MOD_ALT = 0x1;
        public const int WM_HOTKEY = 0x312;
        public const int WM_DESTROY = 0x0002;

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetWindowTitle(IntPtr handle)
        {
            try
            {
                const int nChars = 256;
                StringBuilder Buff = new StringBuilder(nChars);
                if (GetWindowText(handle, Buff, nChars) > 0)
                {
                    return Buff.ToString();
                }
            }
            catch { }
            return null;
        }

        private string GetActiveWindowTitle()
        {
            IntPtr handle = GetForegroundWindow();
            return GetWindowTitle(handle);
        }

        private Boolean ExceptThis(String SubActiveWindowTitle)
        {
            if (SubActiveWindowTitle == null) return true;
            foreach (String one in exceptWindowTitles)
            {
                if (SubActiveWindowTitle.IndexOf(one, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void ScreenShot(string path, string filename, string extension = "png")
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists) dir.Create();
            if (!path.EndsWith(@"\")) path += @"\";
            int screenIndex = 0;
            foreach (Screen screen in Screen.AllScreens)
            {
                string fullname;
                if (screenIndex == 0)
                    fullname = path + filename + "." + extension;
                else
                    fullname = path + filename + $" ({screenIndex + 1})." + extension;
                var bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);
                var graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);
                graphics.Flush();
                bitmap.Save(fullname, ImageFormat.Png);
                bitmap.Dispose();
                graphics.Dispose();
                screenIndex++;
            }
        }

        private void hotkeyShot()
        {
            DateTime time = DateTime.Now;
            String filename = time.ToString("yyyy-MM-dd  HH.mm.ss");
            notifyIcon1.Visible = true;
            try
            {
                String activeWindowTitle = GetActiveWindowTitle();
                if (activeWindowTitle != null)
                    filename += "  " + CorrectFileNames(activeWindowTitle);
                ScreenShot(path2, filename, extension);
                notifyIcon1.ShowBalloonTip(0, "Success !\r\n", filename, ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                notifyIcon1.ShowBalloonTip(0, "Save failed !\r\n" + filename, ex.Message, ToolTipIcon.Error);
            }
        }

        Form2 form2 = new Form2();
        private bool Mode2;

        private void hotkeyShot2()
        {
            if (Mode2)
            {
                form2.Hide();
                //notifyIcon1.Icon = Resources.valloon;
                Mode2 = false;
            }
            else
            {
                form2.Owner = this;
                form2.Show();
                //notifyIcon1.Icon = Resources.valloon_gray;
                Mode2 = true;
            }
            if (!Mode3)
            {
                Bitmap bitmap = new Bitmap(Form3.R.Width, Form3.R.Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(Form3.R.X, Form3.R.Y, 0, 0, new Size(Form3.R.Width, Form3.R.Height));
                graphics.Flush();
                form3.BackgroundImage = bitmap;
            }
        }

        Form3 form3 = new Form3();
        private bool Mode3;

        private void hotkeyShot3()
        {
            if (Mode3)
            {
                form3.Hide();
                Mode3 = false;
            }
            else
            {
                form3.Owner = this;
                form3.Show();
                Mode3 = true;
            }
        }

        //[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //public static IntPtr FindExcelErrorPopup(string title)
        //{
        //    return FindWindow(null, title);
        //}

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 0:
                            hotkeyShot();
                            break;
                        case 2:
                            hotkeyShot2();
                            break;
                        case 3:
                            hotkeyShot3();
                            break;
                        case 4:
                            try
                            {
                                IntPtr handle = GetForegroundWindow();
                                GetWindowThreadProcessId(handle, out uint pid);
                                string title = GetWindowTitle(handle);
                                if (MessageBoc.Show(pid.ToString(), title) == DialogResult.OK)
                                {
                                    var process = Process.Start(new ProcessStartInfo()
                                    {
                                        WindowStyle = ProcessWindowStyle.Hidden,
                                        CreateNoWindow = true,
                                        UseShellExecute = false,
                                        RedirectStandardError = true,
                                        RedirectStandardOutput = true,
                                        FileName = "cmd.exe",
                                        Arguments = $"/c Invisiwind.exe -h {pid}",
                                        WorkingDirectory = @"C:\Program Files (x86)\VMware\VMware Workstation\bin"
                                    });
                                    StringBuilder output = new StringBuilder();
                                    process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                                        output.Append(">>" + e.Data);
                                    process.BeginOutputReadLine();

                                    process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                                        output.Append(">>" + e.Data);
                                    process.BeginErrorReadLine();

                                    process.WaitForExit();

                                    output.Append("ExitCode: " + process.ExitCode);
                                    process.Close();

                                    var result = output.ToString();
                                    if (!result.Contains("Success!"))
                                        MessageBoc.Show(pid.ToString(), result);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBoc.Show(ex.Message, "Error");
                            }
                            break;
                        case 5:
                            try
                            {
                                IntPtr handle = GetForegroundWindow();
                                GetWindowThreadProcessId(handle, out uint pid);
                                string title = GetWindowTitle(handle);
                                if (MessageBoc.Show(pid.ToString(), title) == DialogResult.OK)
                                {
                                    var process = Process.Start(new ProcessStartInfo()
                                    {
                                        WindowStyle = ProcessWindowStyle.Hidden,
                                        CreateNoWindow = true,
                                        UseShellExecute = false,
                                        RedirectStandardError = true,
                                        RedirectStandardOutput = true,
                                        FileName = "cmd.exe",
                                        Arguments = $"/c Invisiwind.exe -u {pid}",
                                        WorkingDirectory = @"C:\Program Files (x86)\VMware\VMware Workstation\bin"
                                    });
                                    StringBuilder output = new StringBuilder();
                                    process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                                        output.Append(">>" + e.Data);
                                    process.BeginOutputReadLine();

                                    process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                                        output.Append(">>" + e.Data);
                                    process.BeginErrorReadLine();

                                    process.WaitForExit();

                                    output.Append("ExitCode: " + process.ExitCode);
                                    process.Close();

                                    var result = output.ToString();
                                    if (!result.Contains("Success!"))
                                        MessageBoc.Show(pid.ToString(), result);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBoc.Show(ex.Message, "Error");
                            }
                            break;
                        case 9:
                            notifyIcon1.Visible = !notifyIcon1.Visible;
                            break;
                    }
                    break;
                case WM_DESTROY:
                    UnregisterHotKey(this.Handle, 0);
                    UnregisterHotKey(this.Handle, 2);
                    UnregisterHotKey(this.Handle, 3);
                    UnregisterHotKey(this.Handle, 4);
                    UnregisterHotKey(this.Handle, 5);
                    UnregisterHotKey(this.Handle, 9);
                    break;
            }
            if (m.Msg == WM_HOTKEY && m.WParam == (IntPtr)0)
            {
                hotkeyShot();
            }
            base.WndProc(ref m);
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            this.Visible = false;
            if (!RegisterHotKey(this.Handle, 0, MOD_WIN + MOD_ALT, hotkey) && !StealthMode)
                MessageBox.Show("Set hotkey failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (!RegisterHotKey(this.Handle, 4, MOD_WIN + MOD_ALT, (int)Keys.Z) && !StealthMode)
                MessageBox.Show("Set hotkey failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (!RegisterHotKey(this.Handle, 5, MOD_WIN + MOD_ALT, (int)Keys.X) && !StealthMode)
                MessageBox.Show("Set hotkey failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (!RegisterHotKey(this.Handle, 2, MOD_WIN + MOD_ALT, (int)Keys.C) && !StealthMode)
                MessageBox.Show("Set hotkey failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (!RegisterHotKey(this.Handle, 3, MOD_WIN + MOD_ALT, (int)Keys.V) && !StealthMode)
                MessageBox.Show("Set hotkey failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (!RegisterHotKey(this.Handle, 9, MOD_WIN + MOD_CONTROL + MOD_SHIFT, (int)Keys.Q))
                MessageBox.Show("Set hotkey failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            startToolStripMenuItem_Click(sender, e);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 0);
            UnregisterHotKey(this.Handle, 2);
            UnregisterHotKey(this.Handle, 3);
            UnregisterHotKey(this.Handle, 4);
            UnregisterHotKey(this.Handle, 5);
            UnregisterHotKey(this.Handle, 9);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            String filename = time.ToString("yyyy-MM-dd  HH.mm.ss");
            try
            {
                String activeWindowTitle = GetActiveWindowTitle();
                if (ExceptThis(activeWindowTitle)) return;
                if (activeWindowTitle != null)
                    filename += "  " + CorrectFileNames(activeWindowTitle);
                ScreenShot(path + time.ToString("yyyy-MM-dd"), filename, extension);
            }
            catch (Exception ex)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(0, "Save failed !\r\n" + filename, ex.Message, ToolTipIcon.Error);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            String filename = time.ToString("yyyy-MM-dd  HH.mm.ss");
            try
            {
                String activeWindowTitle = GetActiveWindowTitle();
                if (ExceptThis(activeWindowTitle)) return;
                if (activeWindowTitle != null)
                    filename += "  " + CorrectFileNames(activeWindowTitle);
                ScreenShot(path + time.ToString("yyyy"), filename, extension);
            }
            catch (Exception ex)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(0, "Save failed !\r\n" + filename, ex.Message, ToolTipIcon.Error);
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                String filename = path + "config.cfg";
                FileInfo fileInfo = new FileInfo(filename);
                if (fileInfo.Exists)
                {
                    StreamReader reader = File.OpenText(filename);
                    String line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("//")) continue;
                        String[] var = line.Split(new Char[] { '=' }, 2);
                        if (var[0] == "interval1")
                        {
                            interval1 = Convert.ToInt16(var[1]);
                        }
                        else if (var[0] == "interval2")
                        {
                            interval2 = Convert.ToInt16(var[1]);
                        }
                        else if (var[0] == "extension")
                        {
                            extension = var[1].Trim();
                        }
                        else if (var[0] == "expect")
                        {
                            exceptWindowTitles = var[1].Trim().Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        }
                        else if (var[0] == "stealth")
                        {
                            StealthMode = var[1].Trim() == "1";
                            this.notifyIcon1.Visible = !StealthMode;
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                }
                else
                {
                    StreamWriter writer = new StreamWriter("config.cfg");
                    writer.Write(
@"interval1=10
interval2=1199
extension=png
expect=Program Manager");
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                if (!StealthMode)
                    MessageBox.Show("Read config failed. " + ex.Message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(0, "I am alive !", "Interval1=" + interval1 + ", Interval2=" + interval2, ToolTipIcon.Info);
            notifyIcon1.Visible = false;
            timer1.Interval = 1000 * interval1;
            timer2.Interval = 1000 * interval2;
            timer1.Enabled = true;
            timer2.Enabled = true;
            notifyIcon1.Icon = Resources.valloon;
            startToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = true;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            //timer2.Enabled=false;
            notifyIcon1.Icon = Resources.valloon_gray;
            startToolStripMenuItem.Enabled = true;
            stopToolStripMenuItem.Enabled = false;
        }

        private void openConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Notepad.exe", "config.cfg");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnregisterHotKey(this.Handle, 0);
            UnregisterHotKey(this.Handle, 2);
            UnregisterHotKey(this.Handle, 3);
            UnregisterHotKey(this.Handle, 4);
            UnregisterHotKey(this.Handle, 5);
            this.Close();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (startToolStripMenuItem.Enabled)
                startToolStripMenuItem_Click(sender, e);
            else
                stopToolStripMenuItem_Click(sender, e);
        }

        private void openTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", @"file:///" + path);
        }
    }
}
