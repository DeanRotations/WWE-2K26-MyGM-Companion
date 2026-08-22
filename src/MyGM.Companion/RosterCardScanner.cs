using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace MyGM.Companion;

public sealed record ScannedRosterCard(int Index,string ImagePath,string RawText,string? RosterRow,bool NeedsReview);
public sealed record RosterScanResult(string SourceHash,ScannedRosterCard[] Cards,string[] Rows,int ReviewCount);

public static class RosterCardScanner {
 static readonly double[] Columns=[.058,.238,.417,.596,.775];
 static readonly double[] Rows=[.245,.422,.586];
 static readonly HashSet<string> Women=new(StringComparer.OrdinalIgnoreCase){"Liv Morgan","Rhea Ripley","Bianca Belair","Becky Lynch","Bayley","Charlotte Flair","Iyo Sky","Asuka","Alexa Bliss","Nikki Bella","Naomi","Jade Cargill","Tiffany Stratton"};
 public static async Task<RosterScanResult> ScanAsync(string screenshot,string profileDir,OcrProcessService ocr,Action<int,int>? progress,CancellationToken token){var cardsDir=Path.Combine(profileDir,"superstars");Directory.CreateDirectory(cardsDir);var crops=CropCards(screenshot,cardsDir);var output=new ScannedRosterCard[crops.Count];using var gate=new SemaphoreSlim(3);var done=0;await Task.WhenAll(crops.Select(async item=>{await gate.WaitAsync(token);try{var response=await ocr.RecognizeAsync(item.Path,token);var text=response.Ok?response.Text:"";var parsed=Parse(item.Index,item.Path,text);output[item.Index]=parsed;}finally{gate.Release();progress?.Invoke(Interlocked.Increment(ref done),crops.Count);}}));var bytes=await File.ReadAllBytesAsync(screenshot,token);var hash=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes))[..16];return new(hash,output,output.Where(x=>x.RosterRow is not null).Select(x=>x.RosterRow!).ToArray(),output.Count(x=>x.NeedsReview));}
 static List<(int Index,string Path)> CropCards(string source,string target){using var image=new Bitmap(source);var result=new List<(int,string)>();var index=0;foreach(var y in Rows)foreach(var x in Columns){var rect=new Rectangle((int)(x*image.Width),(int)(y*image.Height),(int)(.171*image.Width),(int)(.148*image.Height));rect.Intersect(new Rectangle(0,0,image.Width,image.Height));using var crop=image.Clone(rect,PixelFormat.Format24bppRgb);var path=Path.Combine(target,$"card-{index+1:00}.png");crop.Save(path,ImageFormat.Png);result.Add((index,path));index++;}return result;}
 static ScannedRosterCard Parse(int index,string image,string raw){var clean=raw.Replace("\r","");var lines=clean.Split('\n',StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);var name=lines.AsEnumerable().Reverse().Select(NormalizeName).FirstOrDefault(IsLikelyName);var pop=FindNumber(clean,@"(?:POP|POPULARITÄT)\D{0,8}(\d{1,3})")??FindStandalone(lines,0);var stamina=FindNumber(clean,@"(?:AUS|AUSDAUER)\D{0,8}(\d{1,3})")??FindStandalone(lines,1);var style=FindStyle(clean);if(name is null||pop is null||stamina is null||style is null)return new(index,image,clean,null,true);var gender=Women.Contains(name)||clean.Contains("CRUISERIN",StringComparison.OrdinalIgnoreCase)?"W":"M";var role=DetectRoleFromImage(image);var review=role=="Unbekannt";return new(index,image,clean,$"{name} | {gender} | {role} | {style} | {pop} | {stamina} | – | 0",review);}
 static string DetectRoleFromImage(string path){using var bmp=new Bitmap(path);long red=0,green=0;for(var y=0;y<Math.Min(bmp.Height,bmp.Height/2);y+=3)for(var x=0;x<Math.Min(bmp.Width,bmp.Width/2);x+=3){var c=bmp.GetPixel(x,y);if(c.R>c.G*1.35&&c.R>90)red++;if(c.G>c.R*1.25&&c.G>75)green++;}return green>red*1.15?"Face":red>green*1.15?"Heel":"Unbekannt";}
 static int? FindNumber(string text,string pattern){var m=Regex.Match(text,pattern,RegexOptions.IgnoreCase);return m.Success&&int.TryParse(m.Groups[1].Value,out var n)?Math.Clamp(n,0,100):null;}
 static int? FindStandalone(string[] lines,int ordinal){var values=lines.SelectMany(x=>Regex.Matches(x,@"\b\d{1,3}\b").Select(m=>int.Parse(m.Value))).Where(x=>x<=100).ToArray();return values.Length>ordinal?values[ordinal]:null;}
 static string? FindStyle(string text){foreach(var p in new[]{("SPEZIALIST","Specialist"),("SPECIALIST","Specialist"),("BRUISER","Bruiser"),("KÄMPFER","Fighter"),("KAMPFER","Fighter"),("FIGHTER","Fighter"),("CRUISERIN","Cruiser"),("CRUISER","Cruiser"),("RIESE","Giant"),("GIANT","Giant")})if(text.Contains(p.Item1,StringComparison.OrdinalIgnoreCase))return p.Item2;return null;}
 static string NormalizeName(string value)=>Regex.Replace(value.Replace('“',' ').Replace('”',' '),@"[^\p{L}\p{M}'\- ]","").Trim();
 static bool IsLikelyName(string? value)=>!string.IsNullOrWhiteSpace(value)&&value.Length>=4&&value.Any(char.IsLetter)&&!new[]{"POP","AUS","MOR","BRUISER","FIGHTER","SPECIALIST","SPEZIALIST","CRUISER","RIESE"}.Any(x=>value.Equals(x,StringComparison.OrdinalIgnoreCase));
}

