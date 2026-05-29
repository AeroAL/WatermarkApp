; Inno Setup 安装脚本
; 用于打包全屏水印程序

[Setup]
AppName=全屏水印程序
AppVersion=1.0
AppPublisher=WatermarkApp
DefaultDirName={pf}\WatermarkApp
DefaultGroupName=全屏水印
OutputBaseFilename=WatermarkApp_Setup
Compression=lzma
SolidCompression=yes
; Win7 兼容性
MinVersion=6.1
; 安装图标（可选）
; SetupIconFile=app.ico

[Files]
; 主程序
Source: "bin\Release\WatermarkApp.exe"; DestDir: "{app}"; Flags: ignoreversion
; 配置文件模板（首次安装时复制）
Source: "config.ini"; DestDir: "{app}"; Flags: onlyifdoesntexist uninsneveruninstall

[Icons]
; 开始菜单快捷方式
Name: "{group}\全屏水印"; Filename: "{app}\WatermarkApp.exe"
Name: "{group}\卸载全屏水印"; Filename: "{uninstallexe}"
; 桌面快捷方式（可选）
Name: "{commondesktop}\全屏水印"; Filename: "{app}\WatermarkApp.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "创建桌面快捷方式"; GroupDescription: "附加图标:"

[Run]
; 安装完成后运行程序
Filename: "{app}\WatermarkApp.exe"; Description: "启动全屏水印程序"; Flags: nowait postinstall skipifsilent

[Registry]
; 开机自启（程序自身也会设置，这里双重保障）
Root: HKCU; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "WatermarkApp"; ValueData: """{app}\WatermarkApp.exe"""; Flags: uninsdeletevalue

[Code]
// 卸载前确认
function InitializeUninstall(): Boolean;
begin
  Result := MsgBox('确定要卸载全屏水印程序吗？', mbConfirmation, MB_YESNO) = IDYES;
end;

// 安装完成后显示说明
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    MsgBox('安装完成！' + #13#10 + #13#10 +
           '配置文件位置：' + ExpandConstant('{app}\config.ini') + #13#10 +
           '默认密码：123456' + #13#10 + #13#10 +
           '请修改配置文件中的设备位置信息。',
           mbInformation, MB_OK);
  end;
end;
