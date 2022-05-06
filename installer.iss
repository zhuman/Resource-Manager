; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Resource Manager"
#define MyAppVersion "0.4.8"
#define MyAppPublisher "VladTheJunior"
#define MyAppExeName "ResourceManagerUpdater.exe"
#define MyAppMainExeName "Resource Manager.exe"
#define MyAppAssocName "Age of Empires 3 Bar File"
#define MyAppAssocExt ".bar"
#define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{26C15E18-A394-4431-9B4E-BB582193501A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
PrivilegesRequired=admin
OutputBaseFilename=Resource Manager
SetupIconFile=Resource Manager\icon.ico
UninstallDisplayIcon=Resource Manager\icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ChangesAssociations=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "publish\Release\net6.0-windows\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\libwebp_x64.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\libwebp_x86.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\SixLabors.ImageSharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\BCnEncoder.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\BCnEncoder.NET.ImageSharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\pngquant.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\Microsoft.Toolkit.HighPerformance.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\Microsoft.Xaml.Behaviors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\ColorPicker.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\K4os.Hash.xxHash.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\DiffPlex.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\DiffPlex.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\K4os.Compression.LZ4.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\NAudio.Asio.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\NAudio.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\NAudio.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\NAudio.Midi.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\NAudio.Wasapi.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\NAudio.WinForms.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\NAudio.WinMM.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\Pfim.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\Resource Manager.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\Resource Manager.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\Resource Manager.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\Resource Manager.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\Resource Manager.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\ResourceManagerUpdater.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\ResourceManagerUpdater.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\ResourceManagerUpdater.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\ResourceManagerUpdater.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\ResourceManagerUpdater.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\Release\net6.0-windows\fr\*"; DestDir: "{app}\fr"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "publish\Release\net6.0-windows\zh-Hans\*"; DestDir: "{app}\zh-Hans"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "publish\Release\net6.0-windows\zh-Hant\*"; DestDir: "{app}\zh-Hant"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "publish\netcorecheck_x64.exe"; DestDir: "{tmp}"
Source: "publish\netcorecheck.exe"; DestDir: "{tmp}"
Source: "publish\windowsdesktop-runtime-6.0.3-win-x64.exe"; DestDir: "{tmp}"
Source: "publish\windowsdesktop-runtime-6.0.3-win-x86.exe"; DestDir: "{tmp}"
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Registry]
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocExt}\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppAssocKey}"; ValueData: ""; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppAssocName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppMainExeName},0"
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppMainExeName}"" ""%1"""
Root: HKA; Subkey: "Software\Classes\Applications\{#MyAppMainExeName}\SupportedTypes"; ValueType: string; ValueName: ".bar"; ValueData: ""


[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{tmp}\windowsdesktop-runtime-6.0.3-win-x86.exe"; Flags: runascurrentuser skipifdoesntexist; Check: (not IsWin64) and NotIsNetCoreInstalled86('Microsoft.NETCore.App 6.0.3')
Filename: "{tmp}\windowsdesktop-runtime-6.0.3-win-x64.exe"; Flags: runascurrentuser skipifdoesntexist; Check: IsWin64 and NotIsNetCoreInstalled64('Microsoft.NETCore.App 6.0.3')
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: runascurrentuser nowait postinstall skipifsilent

[Code]
function NotIsNetCoreInstalled86(const Version: String): Boolean;
var
  ResultCode: Integer;
begin
  if not FileExists(ExpandConstant('{tmp}{\}') + 'netcorecheck.exe') then begin
    ExtractTemporaryFile('netcorecheck.exe');
  end;
  Result := ShellExec('', ExpandConstant('{tmp}{\}') + 'netcorecheck.exe', Version, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) and (ResultCode = 0);
  Result := not Result;
 end;

function NotIsNetCoreInstalled64(const Version: String): Boolean;
var
  ResultCode: Integer;
begin
  if not FileExists(ExpandConstant('{tmp}{\}') + 'netcorecheck_x64.exe') then begin
    ExtractTemporaryFile('netcorecheck_x64.exe');
  end;
  Result := ShellExec('', ExpandConstant('{tmp}{\}') + 'netcorecheck_x64.exe', Version, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) and (ResultCode = 0);
  Result := not Result;
 end;