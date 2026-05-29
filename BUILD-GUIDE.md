# Linux Mint 编译指南

## 方案说明

由于项目使用 Windows 特有技术（WinForms、Windows API），在 Linux 上无法直接编译。
使用 GitHub Actions 在云端 Windows 环境自动编译和打包。

## 操作步骤

### 第一步：安装 Git（如果未安装）

```bash
sudo apt update
sudo apt install git
```

### 第二步：初始化并推送到 GitHub

```bash
# 进入项目目录
cd WatermarkApp

# 运行配置脚本
chmod +x setup-git.sh
./setup-git.sh

# 提交代码
git commit -m "Initial commit"
```

### 第三步：在 GitHub 创建仓库

1. 访问 https://github.com/new
2. 填写仓库信息：
   - Repository name: `WatermarkApp`
   - 选择 Public 或 Private
3. 点击 Create repository

### 第四步：推送代码

```bash
# 添加远程仓库（替换为你的用户名）
git remote add origin https://github.com/你的用户名/WatermarkApp.git

# 推送代码
git branch -M main
git push -u origin main
```

### 第五步：等待自动编译

1. 推送后，GitHub Actions 自动开始编译
2. 在仓库页面点击 **Actions** 标签查看进度
3. 编译通常需要 2-3 分钟

### 第六步：下载安装包

1. 编译完成后，点击对应的 Workflow 运行记录
2. 在 **Artifacts** 部分下载：
   - `WatermarkApp-Setup`：安装包（推荐）
   - `WatermarkApp-Debug`：绿色版（免安装）

## 配置文件说明

安装后需要手动编辑配置文件：

```ini
[Settings]
Location=你的设备位置    # 修改为实际位置
Password=e10adc3949ba59abbe56e057f20f883e  # 默认密码123456
```

配置文件位置：
- 安装版：`C:\Program Files\WatermarkApp\config.ini`
- 绿色版：与 exe 同目录下的 `config.ini`

## 常见问题

### Q: 编译失败怎么办？

A: 在 Actions 页面查看错误日志，常见原因：
- 代码语法错误
- 依赖项缺失

### Q: 如何修改代码后重新编译？

A: 修改代码后提交推送即可：
```bash
git add .
git commit -m "Update code"
git push
```

### Q: 如何在本地测试？

A: 需要在 Windows 环境：
- 安装 Visual Studio 2010+
- 或安装 .NET Framework 4.0 SDK + MSBuild

### Q: 能否使用其他 CI 服务？

A: 可以，类似配置可用于：
- Azure Pipelines
- GitLab CI
- Jenkins（需 Windows Agent）

## 技术架构

```
Linux Mint (开发)
    ↓ git push
GitHub (代码托管)
    ↓ 自动触发
GitHub Actions (Windows Runner)
    ↓ MSBuild 编译
    ↓ Inno Setup 打包
GitHub Artifacts (安装包)
    ↓ 下载
Windows 7+ (部署运行)
```
