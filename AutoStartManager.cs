using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WatermarkApp
{
    public static class AutoStartManager
    {
        private const string AppName = "WatermarkApp";
        private const string RunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static void EnableAutoStart()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                {
                    if (key != null)
                    {
                        string appPath = Application.ExecutablePath;
                        key.SetValue(AppName, $"\"{appPath}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                // 静默失败，不影响主程序运行
                System.Diagnostics.Debug.WriteLine($"设置开机自启失败：{ex.Message}");
            }
        }

        public static void DisableAutoStart()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue(AppName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"取消开机自启失败：{ex.Message}");
            }
        }

        public static bool IsAutoStartEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false))
                {
                    if (key != null)
                    {
                        string value = key.GetValue(AppName) as string;
                        return !string.IsNullOrEmpty(value);
                    }
                }
            }
            catch (Exception)
            {
                // 读取失败
            }
            return false;
        }
    }
}
