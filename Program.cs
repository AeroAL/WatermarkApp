using System;
using System.Threading;
using System.Windows.Forms;

namespace WatermarkApp
{
    static class Program
    {
        private static Mutex mutex = null;

        [STAThread]
        static void Main()
        {
            // 确保只有一个实例运行
            bool createdNew;
            mutex = new Mutex(true, "WatermarkAppMutex", out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("程序已在运行中！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 初始化配置
            ConfigManager.Initialize();

            // 根据配置决定开机自启（同时清理注册表残留）
            if (ConfigManager.GetAutoStart())
                AutoStartManager.EnableAutoStart();
            else
                AutoStartManager.DisableAutoStart();

            // 运行主程序
            Application.Run(new MainForm());

            GC.KeepAlive(mutex);
        }
    }
}
