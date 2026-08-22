using System.Net.Http;
using System.Reflection;
using System.Text.Json;
namespace MyGM.Companion;
public sealed record UpdateAvailability(bool Available,Version Current,Version? Latest,string Message);
public static class UpdateCheckService {
 const string LatestApi="https://api.github.com/repos/DeanRotations/WWE-2K26-MyGM-Companion/releases/latest";
 public static async Task<UpdateAvailability> CheckAsync(CancellationToken token){var current=Assembly.GetExecutingAssembly().GetName().Version??new Version(0,0,0);try{using var http=new HttpClient{Timeout=TimeSpan.FromSeconds(6)};http.DefaultRequestHeaders.UserAgent.ParseAdd("MyGM-Companion/11.0");http.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");using var response=await http.GetAsync(LatestApi,HttpCompletionOption.ResponseHeadersRead,token);response.EnsureSuccessStatusCode();using var doc=JsonDocument.Parse(await response.Content.ReadAsStreamAsync(token));var tag=doc.RootElement.GetProperty("tag_name").GetString()?.TrimStart('v','V');if(!Version.TryParse(tag,out var latest))return new(false,current,null,"Release-Version nicht lesbar");return new(latest>current,current,latest,latest>current?$"Update {latest} verfügbar":$"Aktuell · {current.ToString(3)}");}catch(OperationCanceledException){return new(false,current,null,"Updateprüfung abgebrochen");}catch{return new(false,current,null,"Updateprüfung momentan nicht erreichbar");}}
}

