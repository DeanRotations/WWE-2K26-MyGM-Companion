using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;
ApplicationConfiguration.Initialize();
if(MessageBox.Show("WWE 2K26 MyGM Companion V10.6 wirklich deinstallieren?\n\nEigene Daten in 'data' bleiben standardmäßig erhalten.","Deinstallation",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
try {
 var install=AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar); var desktop=Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); var link=Path.Combine(desktop,"WWE 2K26 MyGM Companion.lnk"); if(File.Exists(link))File.Delete(link);
 Registry.CurrentUser.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\MyGMCompanion106",false);
 var script=Path.Combine(Path.GetTempPath(),"mygm-remove-"+Guid.NewGuid().ToString("N")+".cmd");
 File.WriteAllText(script,$"@echo off\r\ntimeout /t 2 /nobreak >nul\r\nfor /d %%D in (\"{install}\\*\") do if /I not \"%%~nxD\"==\"data\" rd /s /q \"%%D\"\r\nfor %%F in (\"{install}\\*\") do del /q \"%%F\"\r\nif not exist \"{install}\\data\" rd /q \"{install}\"\r\ndel /q \"%~f0\"\r\n");
 Process.Start(new ProcessStartInfo("cmd.exe",$"/c \"{script}\""){CreateNoWindow=true,UseShellExecute=false}); MessageBox.Show("Deinstallation wurde gestartet.","MyGM Companion");
} catch(Exception ex){MessageBox.Show("Deinstallation fehlgeschlagen: "+ex.Message,"MyGM Companion",MessageBoxButtons.OK,MessageBoxIcon.Error);}
