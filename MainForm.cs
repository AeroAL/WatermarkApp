using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WatermarkApp
{
    public class MainForm : Form
    {
        // Win32 API
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int LWA_ALPHA = 0x00000002;

        private Timer refreshTimer;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private string watermarkText;

        public MainForm()
        {
            InitializeComponent();
            SetupTrayIcon();
            UpdateWatermarkText();

            // 定时刷新（每分钟更新日期）
            refreshTimer = new Timer();
            refreshTimer.Interval = 60000; // 60秒
            refreshTimer.Tick += (s, e) => UpdateWatermarkText();
            refreshTimer.Start();
        }

        private void InitializeComponent()
        {
            // 窗口设置
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.DoubleBuffered = true;

            // 窗口穿透
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 设置窗口穿透
            int style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_TRANSPARENT | WS_EX_LAYERED);
            SetLayeredWindowAttributes(this.Handle, 0, 255, LWA_ALPHA);
        }

        private void SetupTrayIcon()
        {
            // 创建托盘菜单
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("退出水印", null, OnExitClick);
            trayMenu.Items.Add("关于", null, OnAboutClick);

            // 创建托盘图标
            trayIcon = new NotifyIcon();
            trayIcon.Text = "全屏水印";
            trayIcon.ContextMenuStrip = trayMenu;

            // 使用默认图标
            trayIcon.Icon = SystemIcons.Application;

            trayIcon.Visible = true;
            trayIcon.DoubleClick += (s, e) => ShowPasswordDialog();
        }

        private void UpdateWatermarkText()
        {
            string location = ConfigManager.GetLocation();
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            watermarkText = $"{location}\n{date}";
            this.Invalidate(); // 触发重绘
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (string.IsNullOrEmpty(watermarkText))
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // 获取屏幕尺寸
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

            // 绘制平铺水印
            using (Font font = new Font("微软雅黑", 18, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(25, 200, 200, 200)))
            {
                // 计算文字尺寸
                SizeF textSize = g.MeasureString(watermarkText, font);

                // 平铺间距
                float spacingX = textSize.Width + 150;
                float spacingY = textSize.Height + 100;

                // 旋转角度
                float angle = -30;
                g.RotateTransform(angle);

                // 计算旋转后的覆盖范围
                float diagonal = (float)Math.Sqrt(screenBounds.Width * screenBounds.Width + screenBounds.Height * screenBounds.Height);
                float startX = -diagonal / 2;
                float startY = -diagonal / 2;

                // 平铺绘制
                for (float y = startY; y < diagonal; y += spacingY)
                {
                    for (float x = startX; x < diagonal; x += spacingX)
                    {
                        g.DrawString(watermarkText, font, textBrush, x, y);
                    }
                }
            }
        }

        private void ShowPasswordDialog()
        {
            using (PasswordDialog dialog = new PasswordDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 密码验证成功，显示主窗口
                    this.Show();
                    this.WindowState = FormWindowState.Maximized;
                }
            }
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            ShowPasswordDialogForExit();
        }

        private void ShowPasswordDialogForExit()
        {
            using (PasswordDialog dialog = new PasswordDialog())
            {
                dialog.Message = "请输入密码以退出水印程序：";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 密码验证成功，退出程序
                    trayIcon.Visible = false;
                    Application.Exit();
                }
            }
        }

        private void OnAboutClick(object sender, EventArgs e)
        {
            MessageBox.Show(
                "全屏水印程序 v1.0\n\n" +
                "功能：在桌面显示全屏水印\n" +
                "内容：设备位置 + 当前日期\n\n" +
                "配置文件：config.ini\n" +
                "默认密码：123456",
                "关于",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 点击关闭按钮时，最小化到托盘而不是退出
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                trayIcon.Visible = false;
                base.OnFormClosing(e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                refreshTimer?.Dispose();
                trayIcon?.Dispose();
                trayMenu?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
