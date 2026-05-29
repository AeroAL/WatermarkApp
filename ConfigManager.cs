using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace WatermarkApp
{
    public static class ConfigManager
    {
        private static string configFilePath;
        private static readonly string defaultLocation = "请配置设备位置";
        private static readonly string defaultPassword = "123456";
        private static readonly string sectionName = "Settings";

        public static void Initialize()
        {
            configFilePath = Path.Combine(Application.StartupPath, "config.ini");
            if (!File.Exists(configFilePath))
            {
                CreateDefaultConfig();
            }
        }

        private static void CreateDefaultConfig()
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("[Settings]");
                sb.AppendLine($"Location={defaultLocation}");
                sb.AppendLine($"Password={HashPassword(defaultPassword)}");
                sb.AppendLine("AutoStart=true");
                File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WatermarkApp] 创建配置文件失败：{ex.Message}");
                MessageBox.Show($"创建配置文件失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════
        //  INI 解析：section 感知 + trim + 容错
        // ═══════════════════════════════════════════

        /// <summary>读取指定 section 下所有 key=value，返回字典。</summary>
        private static Dictionary<string, string> ReadSection(string section)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(configFilePath)) return result;

            try
            {
                string[] lines = File.ReadAllLines(configFilePath, Encoding.UTF8);
                bool inSection = false;
                string targetHeader = $"[{section}]";

                foreach (string rawLine in lines)
                {
                    string line = rawLine.Trim();

                    // 跳过空行和注释（; 或 # 开头）
                    if (string.IsNullOrEmpty(line) || line[0] == ';' || line[0] == '#')
                        continue;

                    // Section 头切换
                    if (line[0] == '[' && line[line.Length - 1] == ']')
                    {
                        inSection = string.Equals(line, targetHeader, StringComparison.OrdinalIgnoreCase);
                        continue;
                    }

                    if (!inSection) continue;

                    // 解析 Key=Value
                    int eqIdx = line.IndexOf('=');
                    if (eqIdx > 0)
                    {
                        string key = line.Substring(0, eqIdx).Trim();
                        string value = line.Substring(eqIdx + 1).Trim();
                        result[key] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WatermarkApp] 读取 INI 失败：{ex.Message}");
            }
            return result;
        }

        /// <summary>在指定 section 下写入（或更新）一个键值。保留原文件结构。</summary>
        private static bool WriteSectionKey(string key, string value)
        {
            try
            {
                if (!File.Exists(configFilePath))
                    CreateDefaultConfig();

                string[] lines = File.ReadAllLines(configFilePath, Encoding.UTF8);
                var sb = new StringBuilder();
                bool inSection = false;
                bool keyWritten = false;
                bool sectionSeen = false;
                string targetHeader = $"[{sectionName}]";

                for (int i = 0; i < lines.Length; i++)
                {
                    string rawLine = lines[i];
                    string line = rawLine.Trim();

                    if (line.Length > 0 && line[0] == '[' && line[line.Length - 1] == ']')
                    {
                        // 即将离开当前 section：如果 key 还没写，追加
                        if (inSection && !keyWritten)
                        {
                            sb.AppendLine($"{key}={value}");
                            keyWritten = true;
                        }
                        inSection = string.Equals(line, targetHeader, StringComparison.OrdinalIgnoreCase);
                        if (inSection) sectionSeen = true;
                        sb.AppendLine(rawLine);
                        continue;
                    }

                    if (inSection && !keyWritten)
                    {
                        int eqIdx = line.IndexOf('=');
                        if (eqIdx > 0)
                        {
                            string lineKey = line.Substring(0, eqIdx).Trim();
                            if (string.Equals(lineKey, key, StringComparison.OrdinalIgnoreCase))
                            {
                                sb.AppendLine($"{key}={value}");
                                keyWritten = true;
                                continue;
                            }
                        }
                    }

                    sb.AppendLine(rawLine);
                }

                // 文件末尾：section 是最后一个
                if (inSection && !keyWritten)
                {
                    sb.AppendLine($"{key}={value}");
                    keyWritten = true;
                }

                // section 从未出现过
                if (!keyWritten)
                {
                    if (!sectionSeen)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"[{sectionName}]");
                    }
                    sb.AppendLine($"{key}={value}");
                }

                File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WatermarkApp] 保存 INI 失败：{ex.Message}");
                MessageBox.Show($"保存配置失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ═══════════════════════════════════════════
        //  密码：SHA-256 + 随机 Salt（兼容旧 MD5）
        // ═══════════════════════════════════════════

        /// <summary>生成新格式密码串：{salt_hex}:{sha256_hex}</summary>
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string saltHex = BitConverter.ToString(salt).Replace("-", "").ToLowerInvariant();
            string hash = ComputeSHA256(salt, password);
            return $"{saltHex}:{hash}";
        }

        private static string ComputeSHA256(byte[] salt, string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] pwBytes = Encoding.UTF8.GetBytes(password);
                byte[] combined = new byte[salt.Length + pwBytes.Length];
                Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
                Buffer.BlockCopy(pwBytes, 0, combined, salt.Length, pwBytes.Length);
                byte[] hash = sha.ComputeHash(combined);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private static string ComputeMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));
                return sb.ToString();
            }
        }

        private static byte[] HexToBytes(string hex)
        {
            int len = hex.Length;
            byte[] bytes = new byte[len / 2];
            for (int i = 0; i < len; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static bool VerifyPassword(string inputPassword)
        {
            try
            {
                var settings = ReadSection(sectionName);
                if (settings.TryGetValue("Password", out string stored))
                {
                    int colonIdx = stored.IndexOf(':');
                    if (colonIdx > 0)
                    {
                        // 新格式 salt:sha256
                        string saltHex = stored.Substring(0, colonIdx);
                        string expectedHash = stored.Substring(colonIdx + 1);
                        byte[] salt = HexToBytes(saltHex);
                        string actualHash = ComputeSHA256(salt, inputPassword);
                        return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        // 旧格式：裸 MD5  —— 兼容验证，通过后自动迁移
                        string md5Hash = ComputeMD5(inputPassword);
                        if (string.Equals(md5Hash, stored, StringComparison.OrdinalIgnoreCase))
                        {
                            SavePassword(inputPassword); // 自动迁移到新格式
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WatermarkApp] 密码验证异常：{ex.Message}");
            }
            return false;
        }

        // ═══════════════════════════════════════════
        //  Location
        // ═══════════════════════════════════════════

        public static string GetLocation()
        {
            try
            {
                var settings = ReadSection(sectionName);
                if (settings.TryGetValue("Location", out string loc) && !string.IsNullOrWhiteSpace(loc))
                    return loc;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WatermarkApp] 读取位置失败：{ex.Message}");
            }
            return defaultLocation;
        }

        public static bool SaveLocation(string location)
        {
            return WriteSectionKey("Location", location);
        }

        // ═══════════════════════════════════════════
        //  Password Save
        // ═══════════════════════════════════════════

        public static bool SavePassword(string newPassword)
        {
            return WriteSectionKey("Password", HashPassword(newPassword));
        }

        // ═══════════════════════════════════════════
        //  AutoStart
        // ═══════════════════════════════════════════

        public static bool GetAutoStart()
        {
            try
            {
                var settings = ReadSection(sectionName);
                if (settings.TryGetValue("AutoStart", out string val))
                    return string.Equals(val, "true", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WatermarkApp] 读取开机自启失败：{ex.Message}");
            }
            return true; // 默认开启
        }

        public static bool SaveAutoStart(bool enabled)
        {
            return WriteSectionKey("AutoStart", enabled ? "true" : "false");
        }
    }
}
