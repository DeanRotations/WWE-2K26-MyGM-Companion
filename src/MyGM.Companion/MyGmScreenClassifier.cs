using System.Text.RegularExpressions;
namespace MyGM.Companion;
public enum MyGmScreen {Unknown,Home,Roster,Protocol,Booking,Career}
public static class MyGmScreenClassifier {
 public static MyGmScreen Classify(string text){var t=Regex.Replace(text.Replace('–','-'),@"\s+"," ").ToUpperInvariant();if(Has(t,"SHOW BUCHEN")&&(Has(t,"AUFTAKT")||Has(t,"HAUPTEVENT")))return MyGmScreen.Booking;if(Has(t,"ROSTER VERWALTEN")||Regex.Matches(t,@"\bPOP\b").Count>=4)return MyGmScreen.Roster;if(Has(t,"PROTOKOLL")||Has(t,"HERAUSFORDERUNGEN"))return MyGmScreen.Protocol;if(Has(t,"KARRIERE")&&(Has(t,"SAISON")||Has(t,"PLE")||Has(t,"EVENT")))return MyGmScreen.Career;if(Has(t,"STARTSEITE")&&Has(t,"BUDGET"))return MyGmScreen.Home;return MyGmScreen.Unknown;}
 static bool Has(string text,string word)=>text.Contains(word,StringComparison.OrdinalIgnoreCase);
}
