# 全屏水印程序

## 功能特性

- 全屏透明水印显示
- 显示设备位置信息 + 当前日期
- 半透明倾斜文字样式
- 密码验证关闭
- 开机自启动
- 系统托盘图标

## 文件说明

```
WatermarkApp/
├── Program.cs              # 程序入口
├── MainForm.cs             # 主水印窗口
├── ConfigManager.cs        # 配置文件管理
├── PasswordDialog.cs       # 密码验证对话框
├── AutoStartManager.cs     # 开机自启管理
├── WatermarkApp.csproj     # 项目文件
├── config.ini              # 配置文件模板
├── setup.iss               # Inno Setup安装脚本
└── README.md               # 本文件
```

## 编译方法

### 方式一：Visual Studio

1. 用 Visual Studio 2010+ 打开 `WatermarkApp.csproj`
2. 选择 Release 配置
3. 生成解决方案
4. 输出文件在 `bin\Release\` 目录

### 方式二：命令行编译

```batch
# 打开 VS 开发人员命令提示符
cd WatermarkApp
msbuild WatermarkApp.csproj /p:Configuration=Release
```

## 打包安装程序

### 安装 Inno Setup

1. 下载 Inno Setup：https://jrsoftware.org/isinfo.php
2. 安装 Inno Setup 6.x

### 制作安装包

1. 编译 Release 版本
2. 用 Inno Setup 打开 `setup.iss`
3. 点击 编译 -> 编译
4. 安装包生成在 `Output` 目录

## 配置文件说明

配置文件 `config.ini` 位于程序目录：

```ini
[Settings]
Location=北京市海淀区XX大厦A座3层
Password=e10adc3949ba59abbe56e057f20f883e
```

- `Location`：设备位置信息，手动修改
- `Password`：MD5加密的密码，默认密码为 `123456`

## 使用说明

1. **安装后首次运行**
   - 程序自动创建配置文件
   - 自动设置开机自启
   - 桌面显示全屏水印

2. **修改位置信息**
   - 编辑程序目录下的 `config.ini`
   - 修改 `Location` 的值
   - 重启程序生效

3. **关闭水印**
   - 双击系统托盘图标
   - 输入密码验证
   - 验证成功后水印隐藏

4. **退出程序**
   - 右键系统托盘图标
   - 选择"退出水印"
   - 输入密码验证

5. **修改密码**
   - 编辑 `config.ini`
   - 将新密码进行MD5加密后填入
   - 或使用在线MD5工具转换

## 系统要求

- Windows 7 / 8 / 10 / 11
- .NET Framework 4.0 或更高版本

## 注意事项

- 程序运行时会在系统托盘显示图标
- 水印窗口置顶显示，鼠标可穿透
- 默认密码为 `123456`，建议修改
- 配置文件手动编辑后需重启程序
