using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace WatermarkApp
{
    public static class ConfigManager
    {
        private static string configFilePath;
        private static string defaultLocation = "请配置设备位置";
        private static string defaultPassword = "123456"; // 默认密码

        public static void Initialize()
        {
            // 配置文件路径：程序目录下的config.ini
            configFilePath = Path.Combine(Application.StartupPath, "config.ini");

            // 如果配置文件不存在，创建默认配置
            if (!File.Exists(configFilePath))
            {
                CreateDefaultConfig();
            }
        }

        private static void CreateDefaultConfig()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[Settings]");
                sb.AppendLine($"Location={defaultLocation}");
                sb.AppendLine($"Password={EncryptPassword(defaultPassword)}");

                File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建配置文件失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string GetLocation()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string[] lines = File.ReadAllLines(configFilePath, Encoding.UTF8);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Location="))
                        {
                            return line.Substring("Location=".Length).Trim();
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 读取失败返回默认值
            }
            return defaultLocation;
        }

        public static bool VerifyPassword(string inputPassword)
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string[] lines = File.ReadAllLines(configFilePath, Encoding.UTF8);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Password="))
                        {
                            string storedHash = line.Substring("Password=".Length).Trim();
                            string inputHash = EncryptPassword(inputPassword);
                            return storedHash.Equals(inputHash, StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 验证失败
            }
            return false;
        }

        public static string EncryptPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static bool SaveLocation(string location)
        {
            try
            {
                if (!File.Exists(configFilePath))
                {
                    CreateDefaultConfig();
                }

                string[] lines = File.ReadAllLines(configFilePath, Encoding.UTF8);
                StringBuilder sb = new StringBuilder();
                bool locationFound = false;

                foreach (string line in lines)
                {
                    if (line.StartsWith("Location="))
                    {
                        sb.AppendLine($"Location={location}");
                        locationFound = true;
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }

                if (!locationFound)
                {
                    sb.AppendLine($"Location={location}");
                }

                File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存位置信息失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool SavePassword(string newPassword)
        {
            try
            {
                if (!File.Exists(configFilePath))
                {
                    CreateDefaultConfig();
                }

                string[] lines = File.ReadAllLines(configFilePath, Encoding.UTF8);
                StringBuilder sb = new StringBuilder();
                bool passwordFound = false;

                foreach (string line in lines)
                {
                    if (line.StartsWith("Password="))
                    {
                        sb.AppendLine($"Password={EncryptPassword(newPassword)}");
                        passwordFound = true;
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }

                if (!passwordFound)
                {
                    sb.AppendLine($"Password={EncryptPassword(newPassword)}");
                }

                File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存密码失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
