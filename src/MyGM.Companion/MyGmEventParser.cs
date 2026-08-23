using System.Text.RegularExpressions;
namespace MyGM.Companion;
public sealed record MyGmEventContext(string Name,int? Week,string? PreferredMatchType,string PlanningNote);
public static class MyGmEventParser {
 static readonly (string Name,string? Type,string Note)[] Events=[
  ("Hell in a Cell","Hell in a Cell","Stufe-3/4-Rivalitäten und ausdauerstarke Stars für Hell in a Cell reservieren."),
  ("Night of Champions",null,"Champions, Titelrivalitäten und höchste Popularität priorisieren."),
  ("WrestleMania","Hell in a Cell","Saisonhöhepunkt: stärkste Rivalitäten abschließen und Budget einsetzen."),
  ("SummerSlam",null,"Große Rivalitäten und höchste Popularität im Auftakt und Hauptevent einsetzen."),
  ("Survivor Series",null,"Brand- und Teamkonflikte priorisieren."),
  ("Royal Rumble",null,"Ausdauerstarke, populäre Superstars für das Event schonen."),
  ("Money in the Bank","TLC","Ausdauerstarke Superstars für TLC reservieren."),
  ("Extreme Rules","Extreme Rules","Rivalitäten mit ausreichender Ausdauer als Extreme Rules abschließen.")];
 public static MyGmEventContext? Parse(string text,int currentWeek){foreach(var e in Events)if(text.Contains(e.Name,StringComparison.OrdinalIgnoreCase)){var near=Regex.Match(text,$@"{Regex.Escape(e.Name)}[^0-9]{{0,30}}(?:WOCHE|SHOW)?\s*(\d+)",RegexOptions.IgnoreCase);return new(e.Name,near.Success&&int.TryParse(near.Groups[1].Value,out var w)?w:null,e.Type,e.Note);}return null;}
}
