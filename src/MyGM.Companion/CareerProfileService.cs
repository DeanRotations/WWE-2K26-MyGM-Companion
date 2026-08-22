using System.IO;
using System.Text.Json;

namespace MyGM.Companion;

public sealed record CareerProfile(string Id,string Name,string Brand,int Season,int Week,int MatchSlots,int PromoSlots,string[] RosterRows,string? RosterCaptureHash,DateTimeOffset UpdatedAt);

public static class CareerProfileService {
 public static string ProfilesDirectory(string dataDir)=>Path.Combine(dataDir,"careers");
 public static string ActivePath(string dataDir)=>Path.Combine(dataDir,"active-career.json");
 public static CareerProfile? LoadActive(string dataDir){try{return File.Exists(ActivePath(dataDir))?JsonSerializer.Deserialize<CareerProfile>(File.ReadAllText(ActivePath(dataDir))):null;}catch{return null;}}
 public static void SaveActive(string dataDir,CareerProfile profile){var dir=ProfilesDirectory(dataDir);Directory.CreateDirectory(dir);var json=JsonSerializer.Serialize(profile,new JsonSerializerOptions{WriteIndented=true});File.WriteAllText(Path.Combine(dir,profile.Id+".json"),json);File.WriteAllText(ActivePath(dataDir),json);}
 public static CareerProfile CurrentEcwCalibration()=>new("ecw-s1","ECW · Saison 1","ECW",1,13,4,3,[
  "Brock Lesnar | M | Heel | Bruiser | 100 | 27 | LA Knight | 0",
  "Seth Rollins | M | Heel | Specialist | 93 | 17 | – | 0",
  "Kevin Owens | M | Heel | Bruiser | 92 | 3 | Jimmy Uso | 0",
  "Cody Rhodes | M | Face | Specialist | 78 | 19 | Kevin Owens | 0",
  "Jimmy Uso | M | Face | Cruiser | 73 | 30 | Kevin Owens | 0",
  "Drew McIntyre | M | Face | Bruiser | 68 | 48 | – | 0",
  "Randy Orton | M | Face | Fighter | 68 | 36 | Nick Aldis | 0",
  "Shinsuke Nakamura | M | Face | Fighter | 68 | 32 | Liv Morgan | 0",
  "Liv Morgan | W | Heel | Cruiser | 67 | 57 | Shinsuke Nakamura | 0",
  "LA Knight | M | Heel | Bruiser | 66 | 41 | Brock Lesnar | 0",
  "Roman Reigns | M | Face | Bruiser | 65 | 0 | – | 0",
  "Finn Bálor | M | Heel | Fighter | 61 | 0 | – | 0",
  "Nick Aldis | M | Heel | Fighter | 58 | 67 | Randy Orton | 0",
  "Omos | M | Heel | Giant | 55 | 0 | – | 0",
  "Montez Ford | M | Face | Specialist | 54 | 90 | – | 0"
 ],null,DateTimeOffset.Now);
}
