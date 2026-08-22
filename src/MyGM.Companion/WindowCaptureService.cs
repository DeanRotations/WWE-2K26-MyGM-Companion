using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;
namespace MyGM.Companion;
public sealed record WindowInfo(IntPtr Handle, int ProcessId, string ProcessName, string Title, bool IsForeground, string Mode, int Width, int Height);
public sealed record CaptureResult(BitmapImage Preview, string Hash, string SavedPath, WindowInfo Window);
public sealed class WindowCaptureService {
 public Task<WindowInfo?> FindWweWindowAsync(CancellationToken token) => Task.Run(() => {
   token.ThrowIfCancellationRequested(); WindowInfo? found=null;
   var ownPid=Environment.ProcessId;
   NativeMethods.EnumWindows((h,_) => { token.ThrowIfCancellationRequested(); if(!NativeMethods.IsWindowVisible(h)) return true; NativeMethods.GetWindowThreadProcessId(h,out var pid); if(pid==ownPid)return true; var b=new StringBuilder(512); NativeMethods.GetWindowText(h,b,b.Capacity); var t=b.ToString();
     if(t.Contains("Companion",StringComparison.OrdinalIgnoreCase)||t.Contains("Setup",StringComparison.OrdinalIgnoreCase))return true;
     string processName;try{processName=Process.GetProcessById((int)pid).ProcessName;}catch{return true;}var normalized=processName.Replace("_","").Replace("-","").Replace(" ","");var realGameProcess=normalized.Contains("WWE2K26",StringComparison.OrdinalIgnoreCase);var matchingTitle=t.Contains("WWE 2K26",StringComparison.OrdinalIgnoreCase)||t.Contains("WWE2K26",StringComparison.OrdinalIgnoreCase);
     if(realGameProcess&&matchingTitle) { NativeMethods.GetWindowRect(h,out var r); var w=r.Right-r.Left; var he=r.Bottom-r.Top; var sw=NativeMethods.GetSystemMetrics(0); var sh=NativeMethods.GetSystemMetrics(1); var mode=(r.Left==0&&r.Top==0&&w>=sw&&he>=sh)?"Vollbild / Randlos":"Fenstermodus"; found=new(h,(int)pid,processName,t,NativeMethods.GetForegroundWindow()==h,mode,w,he); return false;} return true;},IntPtr.Zero); return found;
 },token);
 public async Task<CaptureResult> CaptureAsync(WindowInfo info,string cacheDir,CancellationToken token) {
   return await Task.Run(() => { token.ThrowIfCancellationRequested(); NativeMethods.GetWindowRect(info.Handle,out var r); if(info.Width<100||info.Height<100) throw new InvalidOperationException("WWE-Fenster ist minimiert oder nicht erfassbar.");
     using var bmp=new Bitmap(info.Width,info.Height,PixelFormat.Format24bppRgb); using(var g=Graphics.FromImage(bmp)) g.CopyFromScreen(r.Left,r.Top,0,0,bmp.Size,CopyPixelOperation.SourceCopy);
     token.ThrowIfCancellationRequested(); using var ms=new MemoryStream(); bmp.Save(ms,ImageFormat.Png); var bytes=ms.ToArray(); var hash=Convert.ToHexString(SHA256.HashData(bytes))[..16]; Directory.CreateDirectory(cacheDir); var path=Path.Combine(cacheDir,$"capture-{DateTime.Now:yyyyMMdd-HHmmss}-{hash}.png"); File.WriteAllBytes(path,bytes);
     var image=new BitmapImage(); image.BeginInit(); image.CacheOption=BitmapCacheOption.OnLoad; image.StreamSource=new MemoryStream(bytes); image.EndInit(); image.Freeze(); return new CaptureResult(image,hash,path,info); },token).WaitAsync(TimeSpan.FromSeconds(8),token);
 }
}
