# 全屏水印程序

## 功能特性

- 全屏透明水印显示（支持多显示器）
- 显示设备位置信息 + 当前日期
- 半透明倾斜文字样式，鼠标穿透
- 密码验证（SHA-256 + 随机盐值加密）
- 开机自启动（可在设置中开关）
- 系统托盘图标与菜单
- 图形化设置界面

## 文件说明

```
WatermarkApp/
├── Program.cs              # 程序入口（单实例互斥 + 自启策略）
├── MainForm.cs             # 主水印窗口（分层透明 + 多屏平铺绘制）
├── ConfigManager.cs        # 配置文件读写（section 感知 INI 解析）
├── PasswordDialog.cs       # 密码验证对话框
├── SettingsDialog.cs       # 设置对话框（位置 / 密码 / 自启）
├── AutoStartManager.cs     # 注册表 Run 键管理
├── WatermarkApp.csproj     # .NET Framework 4.8 项目文件
├── config.ini              # 配置文件模板
├── setup.iss               # Inno Setup 安装脚本
├── build.bat               # 一键编译脚本
└── README.md               # 本文件
```

## 编译方法

### 方式一：Visual Studio

1. 用 Visual Studio 2010+ 打开 `WatermarkApp.csproj`
2. 选择 Release 配置
3. 生成解决方案 → 输出 `bin\Release\WatermarkApp.exe`

### 方式二：命令行编译

```batch
# 打开 "VS 开发人员命令提示符" 或 "Developer PowerShell"
cd WatermarkApp
msbuild WatermarkApp.csproj /p:Configuration=Release
```

## 打包安装程序

1. 下载安装 [Inno Setup](https://jrsoftware.org/isinfo.php) 6.x
2. 编译 Release 版本
3. 用 Inno Setup 打开 `setup.iss` → 编译
4. 安装包生成在 `Output` 目录

## 配置文件说明

配置文件 `config.ini` 位于程序所在目录，首次运行自动创建：

```ini
[Settings]
Location=请配置设备位置
Password=<salt>:<sha256_hash>
AutoStart=true
```

| 键 | 说明 |
|---|------|
| `Location` | 设备位置信息，用于水印显示内容 |
| `Password` | SHA-256 + 随机盐值哈希；兼容旧版 MD5 格式（首次验证后自动升级） |
| `AutoStart` | `true`/`false`，控制是否开机自启 |

> 建议通过托盘菜单 → **设置** 修改配置，无需手动编辑文件。

## 使用说明

### 首次运行

- 程序自动创建配置文件并写入默认密码 `123456`
- 根据 `AutoStart` 配置决定是否注册开机自启
- 桌面显示全屏倾斜水印（位置 + 日期）

### 托盘菜单

右键系统托盘图标：

| 菜单项 | 说明 |
|--------|------|
| 设置 | 密码验证后打开设置面板（修改位置、密码、开机自启） |
| 退出水印 | 密码验证后完全退出程序 |
| 关于 | 版本信息 |

双击托盘图标可临时隐藏 / 恢复水印窗口。

### 设置面板

通过托盘菜单 → **设置**（需验证密码）进入：

- **设备位置**：修改水印中显示的位置文字，保存后即时刷新
- **开机自启**：勾选 / 取消，保存时即时写入注册表
- **修改密码**：留空则不修改；新密码需 ≥4 位，两次输入一致

## 系统要求

- Windows 7 / 8 / 10 / 11
- .NET Framework 4.8

## 注意事项

- 水印窗口为全屏透明穿透层，鼠标操作直达下层窗口
- 默认密码为 `123456`，首次使用请通过设置修改
- 多显示器环境下每个屏幕均独立平铺水印
- 密码以 SHA-256 + 随机盐值哈希存储，不可逆向还原
