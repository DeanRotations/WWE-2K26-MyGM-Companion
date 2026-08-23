using System.Runtime.InteropServices;
using System.Text;
namespace MyGM.Companion;
internal static class NativeMethods {
 [DllImport("user32.dll")] internal static extern bool EnumWindows(EnumWindowsProc callback, IntPtr lParam);
 internal delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);
 [DllImport("user32.dll")] internal static extern int GetWindowText(IntPtr hwnd, StringBuilder text, int count);
 [DllImport("user32.dll")] internal static extern bool IsWindowVisible(IntPtr hwnd);
 [DllImport("user32.dll")] internal static extern IntPtr GetForegroundWindow();
 [DllImport("user32.dll")] internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId);
 [DllImport("user32.dll")] internal static extern bool GetWindowRect(IntPtr hwnd, out Rect rect);
 [DllImport("user32.dll")] internal static extern bool PrintWindow(IntPtr hwnd,IntPtr hdc,uint flags);
 [DllImport("user32.dll")] internal static extern int GetSystemMetrics(int index);
 [DllImport("user32.dll")] internal static extern bool RegisterHotKey(IntPtr hwnd, int id, uint modifiers, uint key);
 [DllImport("user32.dll")] internal static extern bool UnregisterHotKey(IntPtr hwnd, int id);
 [DllImport("user32.dll")] internal static extern bool SetForegroundWindow(IntPtr hwnd);
 [DllImport("user32.dll",SetLastError=true)] internal static extern uint SendInput(uint count,Input[] inputs,int size);
 [StructLayout(LayoutKind.Sequential)] internal struct Rect { public int Left, Top, Right, Bottom; }
 [StructLayout(LayoutKind.Sequential)] internal struct Input {public uint Type;public InputUnion Union;}
 [StructLayout(LayoutKind.Explicit)] internal struct InputUnion {[FieldOffset(0)]public KeyboardInput Keyboard;}
 [StructLayout(LayoutKind.Sequential)] internal struct KeyboardInput {public ushort VirtualKey;public ushort ScanCode;public uint Flags;public uint Time;public UIntPtr ExtraInfo;}
}
