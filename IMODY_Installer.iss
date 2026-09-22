; =====================================================================
; IMODY AI Travel Companion - Professional Windows Setup Script (Inno Setup)
; Developed by: İklim Düzen
; =====================================================================

#define MyAppName "IMODY AI Travel Companion"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "İklim Düzen"
#define MyAppExeName "IMODY.exe"
#define MyAppIcon "c:\Users\User\OneDrive\Desktop\IMODY\IMODY\Resources\app_icon.ico"
#define SourceDir "c:\Users\User\OneDrive\Desktop\IMODY_Uygulama"

[Setup]
AppId={{D82F7A1E-4819-4A73-98C3-B48E0F3795C4}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\IMODY
DefaultGroupName=IMODY AI
AllowNoIcons=yes
OutputDir=c:\Users\User\OneDrive\Desktop
OutputBaseFilename=IMODY_Kurulum_Setup
SetupIconFile={#MyAppIcon}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
DisableWelcomePage=no
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName}

[Languages]
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Resources\app_icon.ico"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\IMODY AI"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Resources\app_icon.ico"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
