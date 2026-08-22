using System.Text.RegularExpressions;
namespace MyGM.Companion;

public sealed record ParsedObjective(string Kind,string Subject,string Requirement,int? DeadlineWeek,string Priority,string SourceText){
 public string ToEditorLine()=>string.Join(" | ",[Kind,Subject,Requirement,DeadlineWeek?.ToString()??"–",Priority,"Offen"]);
}

public static partial class ObjectiveParser {
 static readonly (string Kind,string[] Words,string Priority)[] Rules=[
  ("Eingriff",["eingriff","interference","run-in"],"Hoch"),
  ("Sieg",["gewinnen","gewinnt","win next","must win","nächstes match gewinnen"],"Hoch"),
  ("Matchtyp",["tables","extreme rules","hell in a cell","tlc","submission","falls count","steel cage","last man standing"],"Mittel"),
  ("Rollenwechsel",["rollenwechsel","change role","turn face","turn heel","good guy","bad guy"],"Hoch"),
  ("Ruhe",["ruhe","rest","nicht antreten","do not book"],"Hoch"),
  ("Promo",["promo","eigenwerbung","self promo","charity","call out"],"Mittel"),
  ("Buchen",["buche","book ","schedule","match bestreiten"],"Mittel")];
 public static IReadOnlyList<ParsedObjective> Parse(string text,int currentWeek){var lines=text.Replace("\r","").Split('\n',StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);var result=new List<ParsedObjective>();foreach(var line in lines){if(line.Length<5)continue;var lower=line.ToLowerInvariant();var rule=Rules.FirstOrDefault(r=>r.Words.Any(lower.Contains));if(rule.Words is null)continue;var deadline=ExtractDeadline(lower,currentWeek);var subject=ExtractSubject(line);result.Add(new(rule.Kind,subject,line,deadline,rule.Priority,line));}return result.DistinctBy(x=>x.SourceText,StringComparer.OrdinalIgnoreCase).ToList();}
 static int? ExtractDeadline(string line,int week){var match=WeekRegex().Match(line);if(match.Success&&int.TryParse(match.Groups[1].Value,out var explicitWeek))return explicitWeek;if(line.Contains("diese woche")||line.Contains("this week"))return week;if(line.Contains("nächste woche")||line.Contains("next week"))return week+1;var inWeeks=InWeeksRegex().Match(line);return inWeeks.Success&&int.TryParse(inWeeks.Groups[1].Value,out var count)?week+count:null;}
 static string ExtractSubject(string line){var cleaned=Regex.Replace(line,@"\s+"," ").Trim();var before=cleaned.Split([':', '-', '–'],2)[0].Trim();return before.Length is >=3 and <=35?before:"Allgemein";}
 [GeneratedRegex(@"(?:woche|week)\s*(\d+)",RegexOptions.IgnoreCase)]private static partial Regex WeekRegex();
 [GeneratedRegex(@"(?:in|innerhalb)\s+(\d+)\s+(?:wochen|weeks)",RegexOptions.IgnoreCase)]private static partial Regex InWeeksRegex();
}

