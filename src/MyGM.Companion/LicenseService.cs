using System.IO;
using System.Net.Http.Json;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MyGM.Companion;

public sealed record LicenseSettings(string Mode,string Endpoint,string PublicKeyPem,int OfflineGraceHours){public static LicenseSettings Development()=>new("development","","",72);}
public sealed record LicenseSession(string Username,string LicenseCode,string DeviceId,DateTimeOffset ValidUntil,DateTimeOffset LastChecked,string Token,string Signature,string Mode){public TimeSpan Remaining=>ValidUntil-DateTimeOffset.Now;public bool Active=>Remaining>TimeSpan.Zero;}
public sealed record LicenseLoginRequest(string Username,string Password,string LicenseCode,string DeviceId);
public sealed record LicenseRefreshRequest(string Token,string DeviceId);
public sealed record LicenseServerResponse(bool Ok,string? Error,string Username,string LicenseCode,string DeviceId,DateTimeOffset ValidUntil,string Token,string Signature);

public sealed class LicenseService {
 readonly string configPath,sessionPath;readonly HttpClient http=new(){Timeout=TimeSpan.FromSeconds(10)};
 public LicenseService(string root){configPath=Path.Combine(root,"config","license.json");sessionPath=Path.Combine(root,"data","license-session.json");}
 public LicenseSettings LoadSettings(){try{return File.Exists(configPath)?JsonSerializer.Deserialize<LicenseSettings>(File.ReadAllText(configPath),Options())??LicenseSettings.Development():LicenseSettings.Development();}catch{return LicenseSettings.Development();}}
 public LicenseSession? LoadCached(){try{return File.Exists(sessionPath)?JsonSerializer.Deserialize<LicenseSession>(File.ReadAllText(sessionPath),Options()):null;}catch{return null;}}
 public async Task<(LicenseSession? Session,string Error)> LoginAsync(string username,string password,string code,CancellationToken token){var cfg=LoadSettings();username=username.Trim();code=code.Trim().ToUpperInvariant();if(string.IsNullOrWhiteSpace(username)||string.IsNullOrWhiteSpace(password)||string.IsNullOrWhiteSpace(code))return(null,"Benutzername, Passwort und Lizenzcode werden benötigt.");var device=DeviceId();if(cfg.Mode.Equals("development",StringComparison.OrdinalIgnoreCase)){var owner=new LicenseSession(username,code,device,DateTimeOffset.Now.AddYears(10),DateTimeOffset.Now,"development-owner","","development");Save(owner);return(owner,"");}try{var url=cfg.Endpoint.TrimEnd('/')+"/api/license/login";using var response=await http.PostAsJsonAsync(url,new LicenseLoginRequest(username,password,code,device),token);var body=await response.Content.ReadFromJsonAsync<LicenseServerResponse>(Options(),token);if(body is null||!response.IsSuccessStatusCode||!body.Ok)return(null,body?.Error??"Lizenzserver nicht erreichbar.");if(!Verify(body,cfg))return(null,"Die Antwort des Lizenzservers besitzt keine gültige Signatur.");var session=ToSession(body,"server");Save(session);return(session,"");}catch(Exception ex)when(ex is HttpRequestException or TaskCanceledException){return TryOffline(cfg,out var offline)?(offline,""):(null,"Lizenzserver nicht erreichbar und keine gültige Offline-Lizenz vorhanden.");}}
 public async Task<(LicenseSession? Session,string Error)> ValidateAsync(CancellationToken token){var cfg=LoadSettings();var cached=LoadCached();if(cached is null)return(null,"Noch nicht angemeldet.");if(cfg.Mode.Equals("development",StringComparison.OrdinalIgnoreCase))return(cached.Active?(cached,""):(null,"Entwicklungslizenz abgelaufen."));try{using var response=await http.PostAsJsonAsync(cfg.Endpoint.TrimEnd('/')+"/api/license/refresh",new LicenseRefreshRequest(cached.Token,cached.DeviceId),token);var body=await response.Content.ReadFromJsonAsync<LicenseServerResponse>(Options(),token);if(body is null||!response.IsSuccessStatusCode||!body.Ok)return(null,body?.Error??"Lizenz ungültig.");if(!Verify(body,cfg))return(null,"Ungültige Serversignatur.");var session=ToSession(body,"server");Save(session);return(session,"");}catch(Exception ex)when(ex is HttpRequestException or TaskCanceledException){return TryOffline(cfg,out var offline)?(offline,""):(null,"Offline-Zeitraum abgelaufen.");}}
 bool TryOffline(LicenseSettings cfg,out LicenseSession? session){session=LoadCached();if(session is null||!session.Active||session.Mode!="server"||DateTimeOffset.Now-session.LastChecked>TimeSpan.FromHours(Math.Clamp(cfg.OfflineGraceHours,0,168)))return false;var response=new LicenseServerResponse(true,null,session.Username,session.LicenseCode,session.DeviceId,session.ValidUntil,session.Token,session.Signature);return Verify(response,cfg);}
 static LicenseSession ToSession(LicenseServerResponse r,string mode)=>new(r.Username,r.LicenseCode,r.DeviceId,r.ValidUntil,DateTimeOffset.Now,r.Token,r.Signature,mode);
 static bool Verify(LicenseServerResponse r,LicenseSettings cfg){try{if(string.IsNullOrWhiteSpace(cfg.PublicKeyPem)||string.IsNullOrWhiteSpace(r.Signature))return false;using var ecdsa=ECDsa.Create();ecdsa.ImportFromPem(cfg.PublicKeyPem);return ecdsa.VerifyData(Encoding.UTF8.GetBytes(Canonical(r)),Convert.FromBase64String(r.Signature),HashAlgorithmName.SHA256);}catch{return false;}}
 internal static string Canonical(LicenseServerResponse r)=>string.Join('\n',r.Username,r.LicenseCode,r.DeviceId,r.ValidUntil.ToUniversalTime().ToString("O"),r.Token);
 void Save(LicenseSession session){Directory.CreateDirectory(Path.GetDirectoryName(sessionPath)!);File.WriteAllText(sessionPath,JsonSerializer.Serialize(session,Options()));}
 public static string RemainingText(LicenseSession s){var left=s.Remaining;if(left<=TimeSpan.Zero)return"Abgelaufen";if(left.TotalDays>=60)return$"{Math.Floor(left.TotalDays/30)} Monate";if(left.TotalDays>=14)return$"{Math.Floor(left.TotalDays/7)} Wochen";return$"{Math.Max(1,Math.Ceiling(left.TotalDays))} Tage";}
 static string DeviceId(){var raw=$"{Environment.MachineName}|{Environment.UserName}|{Environment.OSVersion.VersionString}";return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)))[..24];}
 static JsonSerializerOptions Options()=>new(){PropertyNameCaseInsensitive=true,WriteIndented=true};
}
