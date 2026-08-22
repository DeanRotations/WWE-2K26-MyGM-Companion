using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
namespace MyGM.Companion;
public partial class OverlayWindow : Window {
 const int GWL_EXSTYLE=-20, WS_EX_TRANSPARENT=0x20, WS_EX_TOOLWINDOW=0x80, WS_EX_NOACTIVATE=0x08000000;
 [DllImport("user32.dll")] static extern int GetWindowLong(IntPtr h,int i); [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr h,int i,int v);
 public OverlayWindow(){InitializeComponent(); Loaded+=(_,_)=>{Left=SystemParameters.WorkArea.Right-Width-24;Top=24;};}
 public void SetSummary(string text)=>OverlaySummary.Text=text;
 public void SetPlan(ShowRecommendation plan,CareerProfile? profile){OverlayCareer.Text=profile is null?"Lokaler Spielstand":$"{profile.Name} · Woche {profile.Week} · {profile.MatchSlots} Matches / {profile.PromoSlots} Promos";OverlaySummary.Text=plan.Render();}
 protected override void OnSourceInitialized(EventArgs e){base.OnSourceInitialized(e);var h=new WindowInteropHelper(this).Handle;SetWindowLong(h,GWL_EXSTYLE,GetWindowLong(h,GWL_EXSTYLE)|WS_EX_TRANSPARENT|WS_EX_TOOLWINDOW|WS_EX_NOACTIVATE);}
 protected override void OnClosing(System.ComponentModel.CancelEventArgs e){if(Application.Current.MainWindow?.IsVisible==true){e.Cancel=true;Hide();}base.OnClosing(e);}
}
