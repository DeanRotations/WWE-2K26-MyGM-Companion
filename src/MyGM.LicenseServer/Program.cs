using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

var builder=WebApplication.CreateBuilder(args);builder.Logging.ClearProviders();builder.Logging.AddConsole();var app=builder.Build();
var store=new LicenseStore(Path.Combine(AppContext.BaseDirectory,"data","licenses.json"));
var signer=new ReceiptSigner(Environment.GetEnvironmentVariable("MYGM_LICENSE_PRIVATE_KEY_PEM")??"");
var adminKey=Environment.GetEnvironmentVariable("MYGM_LICENSE_ADMIN_KEY")??"";
app.UseHttpsRedirection();
app.MapPost("/api/license/login",(LoginRequest req)=>{var result=store.Login(req);return result.Ok?Results.Ok(signer.Sign(result)):Results.Json(result,statusCode:401);});
app.MapPost("/api/license/refresh",(RefreshRequest req)=>{var result=store.Refresh(req);return result.Ok?Results.Ok(signer.Sign(result)):Results.Json(result,statusCode:401);});
app.MapPost("/api/admin/users",(HttpRequest http,CreateUserRequest req)=>{if(string.IsNullOrWhiteSpace(adminKey)||http.Headers["X-Admin-Key"]!=adminKey)return Results.Unauthorized();return Results.Ok(store.Create(req));});
app.MapPost("/api/admin/licenses/extend",(HttpRequest http,ExtendRequest req)=>{if(string.IsNullOrWhiteSpace(adminKey)||http.Headers["X-Admin-Key"]!=adminKey)return Results.Unauthorized();return store.Extend(req)?Results.Ok():Results.NotFound();});
app.MapGet("/health",()=>Results.Ok(new{status="ok"}));app.Run();

record LoginRequest(string Username,string Password,string LicenseCode,string DeviceId);record RefreshRequest(string Token,string DeviceId);record CreateUserRequest(string Username,string Password,string LicenseCode,int Days);record ExtendRequest(string LicenseCode,int Days);
record ServerResponse(bool Ok,string? Error,string Username,string LicenseCode,string DeviceId,DateTimeOffset ValidUntil,string Token,string Signature="");
sealed record StoredUser(string Username,string PasswordHash,string LicenseCode,string DeviceId,DateTimeOffset ValidUntil,string Token,bool Revoked);
sealed class LicenseStore {
 readonly string path;readonly object gate=new();readonly PasswordHasher<string> hasher=new();List<StoredUser> users;
 public LicenseStore(string path){this.path=path;Directory.CreateDirectory(Path.GetDirectoryName(path)!);users=Load();}
 public ServerResponse Login(LoginRequest r){lock(gate){var u=users.FirstOrDefault(x=>x.Username.Equals(r.Username.Trim(),StringComparison.OrdinalIgnoreCase)&&x.LicenseCode.Equals(r.LicenseCode.Trim(),StringComparison.OrdinalIgnoreCase));if(u is null||u.Revoked||u.ValidUntil<=DateTimeOffset.UtcNow)return Fail("Lizenz ungültig oder abgelaufen.");if(hasher.VerifyHashedPassword(u.Username,u.PasswordHash,r.Password)==PasswordVerificationResult.Failed)return Fail("Anmeldedaten ungültig.");if(!string.IsNullOrWhiteSpace(u.DeviceId)&&u.DeviceId!=r.DeviceId)return Fail("Lizenz ist bereits an ein anderes Gerät gebunden.");u=u with{DeviceId=r.DeviceId,Token=Convert.ToHexString(RandomNumberGenerator.GetBytes(32))};Replace(u);return Ok(u);}}
 public ServerResponse Refresh(RefreshRequest r){lock(gate){var u=users.FirstOrDefault(x=>x.Token==r.Token&&x.DeviceId==r.DeviceId);return u is null||u.Revoked||u.ValidUntil<=DateTimeOffset.UtcNow?Fail("Lizenz abgelaufen oder gesperrt."):Ok(u);}}
 public object Create(CreateUserRequest r){lock(gate){if(users.Any(x=>x.Username.Equals(r.Username,StringComparison.OrdinalIgnoreCase)||x.LicenseCode.Equals(r.LicenseCode,StringComparison.OrdinalIgnoreCase)))return new{ok=false,error="Benutzer oder Lizenzcode existiert bereits."};var u=new StoredUser(r.Username,hasher.HashPassword(r.Username,r.Password),r.LicenseCode.ToUpperInvariant(),"",DateTimeOffset.UtcNow.AddDays(Math.Clamp(r.Days,1,3650)),"",false);users.Add(u);Save();return new{ok=true,u.Username,u.LicenseCode,u.ValidUntil};}}
 public bool Extend(ExtendRequest r){lock(gate){var u=users.FirstOrDefault(x=>x.LicenseCode.Equals(r.LicenseCode,StringComparison.OrdinalIgnoreCase));if(u is null)return false;var from=u.ValidUntil>DateTimeOffset.UtcNow?u.ValidUntil:DateTimeOffset.UtcNow;Replace(u with{ValidUntil=from.AddDays(Math.Clamp(r.Days,1,3650)),Revoked=false});return true;}}
 void Replace(StoredUser u){users.RemoveAll(x=>x.Username.Equals(u.Username,StringComparison.OrdinalIgnoreCase));users.Add(u);Save();}
 List<StoredUser> Load(){try{return File.Exists(path)?JsonSerializer.Deserialize<List<StoredUser>>(File.ReadAllText(path))??[]:[];}catch{return[];}}
 void Save(){var tmp=path+".tmp";File.WriteAllText(tmp,JsonSerializer.Serialize(users,new JsonSerializerOptions{WriteIndented=true}));File.Move(tmp,path,true);}
 static ServerResponse Ok(StoredUser u)=>new(true,null,u.Username,u.LicenseCode,u.DeviceId,u.ValidUntil,u.Token);static ServerResponse Fail(string e)=>new(false,e,"","","",DateTimeOffset.MinValue,"");
}
sealed class ReceiptSigner {
 readonly ECDsa? key;public ReceiptSigner(string pem){try{if(!string.IsNullOrWhiteSpace(pem)){key=ECDsa.Create();key.ImportFromPem(pem);}}catch{key=null;}}
 public ServerResponse Sign(ServerResponse r){if(key is null)throw new InvalidOperationException("MYGM_LICENSE_PRIVATE_KEY_PEM ist nicht konfiguriert.");var canonical=string.Join('\n',r.Username,r.LicenseCode,r.DeviceId,r.ValidUntil.ToUniversalTime().ToString("O"),r.Token);return r with{Signature=Convert.ToBase64String(key.SignData(Encoding.UTF8.GetBytes(canonical),HashAlgorithmName.SHA256))};}
}
