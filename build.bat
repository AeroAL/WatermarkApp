@echo off
echo ========================================
echo 全屏水印程序 - 编译脚本
echo ========================================
echo.

REM 检查是否有 MSBuild
where msbuild >nul 2>&1
if %errorlevel% neq 0 (
    echo 错误：未找到 MSBuild
    echo 请在 Visual Studio 开发人员命令提示符中运行此脚本
    echo 或者安装 .NET Framework SDK
    pause
    exit /b 1
)

echo 正在编译 Release 版本...
echo.

msbuild WatermarkApp.csproj /p:Configuration=Release /v:minimal

if %errorlevel% equ 0 (
    echo.
    echo ========================================
    echo 编译成功！
    echo 输出目录：bin\Release\
    echo ========================================
    echo.
    echo 下一步：
    echo 1. 用 Inno Setup 打开 setup.iss 制作安装包
    echo 2. 或直接运行 bin\Release\WatermarkApp.exe 测试
) else (
    echo.
    echo ========================================
    echo 编译失败！请检查错误信息
    echo ========================================
)

echo.
pause
