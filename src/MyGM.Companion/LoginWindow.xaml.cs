using System.Windows;
namespace MyGM.Companion;
public partial class LoginWindow:Window {
 readonly LicenseService licenses=new(AppContext.BaseDirectory);
 public LoginWindow(){InitializeComponent();var cached=licenses.LoadCached();if(cached is not null){UsernameInput.Text=cached.Username;LicenseInput.Text=cached.LicenseCode;LoginStatus.Text=$"Gespeicherte Lizenz: {LicenseService.RemainingText(cached)} verbleibend";Loaded+=async(_,_)=>await TryCachedAsync();}else if(licenses.LoadSettings().Mode.Equals("development",StringComparison.OrdinalIgnoreCase)){UsernameInput.Text="Owner";PasswordInput.Password="owner";LicenseInput.Text="OWNER-DEVELOPMENT";LoginStatus.Text="Entwicklermodus · vor dem Verkauf mit deinem Lizenzserver verbinden.";}}
 async Task TryCachedAsync(){LoginButton.IsEnabled=false;LoginStatus.Text="Gespeicherte Lizenz wird geprüft …";var(session,error)=await licenses.ValidateAsync(CancellationToken.None);if(session is not null){Open(session);return;}LoginStatus.Text=error;LoginButton.IsEnabled=true;}
 async void Login_Click(object sender,RoutedEventArgs e){LoginButton.IsEnabled=false;LoginStatus.Text="Lizenz wird geprüft …";try{var(result,error)=await licenses.LoginAsync(UsernameInput.Text,PasswordInput.Password,LicenseInput.Text,CancellationToken.None);if(result is null){LoginStatus.Text=error;return;}Open(result);}finally{LoginButton.IsEnabled=true;}}
 void Open(LicenseSession session){var main=new MainWindow(session);Application.Current.MainWindow=main;main.Show();Close();}
}
