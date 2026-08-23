using System.Runtime.InteropServices;
namespace MyGM.Companion;
public sealed class KeyboardAutomationService {
 public async Task SendAsync(WindowInfo window,IEnumerable<string> keys,CancellationToken token){foreach(var name in keys){token.ThrowIfCancellationRequested();if(!NativeMethods.SetForegroundWindow(window.Handle))throw new InvalidOperationException("Das WWE-Fenster konnte nicht aktiviert werden.");await Task.Delay(250,token);if(NativeMethods.GetForegroundWindow()!=window.Handle)throw new InvalidOperationException("AUTOMATIK GESTOPPT: WWE 2K26 ist nicht im Vordergrund.");var key=VirtualKey(name);var input=new[]{New(key,0),New(key,2)};var sent=NativeMethods.SendInput((uint)input.Length,input,Marshal.SizeOf<NativeMethods.Input>());if(sent!=input.Length)throw new InvalidOperationException($"Tastaturbefehl {name} wurde von Windows blockiert.");await Task.Delay(650,token);}}
 static NativeMethods.Input New(ushort key,uint flags)=>new(){Type=1,Union=new NativeMethods.InputUnion{Keyboard=new NativeMethods.KeyboardInput{VirtualKey=key,Flags=flags}}};
 static ushort VirtualKey(string name)=>name.ToUpperInvariant() switch{"PAGEUP"=>0x21,"PAGEDOWN"=>0x22,"ENTER"=>0x0D,"ESCAPE"=>0x1B,"UP"=>0x26,"DOWN"=>0x28,"LEFT"=>0x25,"RIGHT"=>0x27,"F1"=>0x70,_=>throw new InvalidOperationException("Unbekannte Automationstaste: "+name)};
}
