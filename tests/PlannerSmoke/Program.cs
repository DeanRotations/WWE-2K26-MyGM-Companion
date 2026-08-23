using MyGM.Companion;
var roster=new[]{
 "Cody Rhodes | M | Face | Fighter | 88 | 74 | – | 3","Randy Orton | M | Heel | Bruiser | 84 | 70 | – | 3",
 "Kevin Owens | M | Face | Fighter | 80 | 67 | – | 2","Jimmy Uso | M | Heel | Bruiser | 75 | 64 | – | 2",
 "LA Knight | M | Face | Cruiser | 81 | 70 | – | 1","Bronson Reed | M | Heel | Giant | 70 | 68 | – | 1",
 "Seth Rollins | M | Face | Specialist | 86 | 52 | – | 2","Drew McIntyre | M | Heel | Fighter | 82 | 58 | – | 2",
 "Liv Morgan | W | Heel | Cruiser | 79 | 69 | – | 2","Rhea Ripley | W | Face | Giant | 88 | 72 | – | 2",
 "Montez Ford | M | Face | Cruiser | 68 | 61 | – | 0"};
var objectives=new[]{"Matchtyp | Allgemein | Plane diese Woche ein Tables Match | 12 | Mittel | Offen"};
var plan=PlannerEngine.Generate(roster,objectives,50_000);
if(plan.Matches.Count!=4)throw new Exception("Vier Matches erwartet.");
if(plan.Promos.Count!=3)throw new Exception("Drei Promos erwartet.");
if(!plan.Matches.Any(x=>x.MatchType=="Tables"))throw new Exception("Tables-Ziel nicht eingeplant.");
if(plan.Matches.SelectMany(x=>new[]{x.A.Name,x.B.Name}).Distinct().Count()!=8)throw new Exception("Superstar doppelt gebucht.");
if(plan.Matches.Any(x=>x.A.Gender!=x.B.Gender))throw new Exception("Ungültige Geschlechterpaarung.");
var learned=LearningEngine.Learn([new ShowOutcome(1,[new MatchOutcome("Cody Rhodes","Randy Orton","Tables",5.0)],[],4.5,"",DateTimeOffset.Now)]);
if(learned.Profile.MatchBonus("Cody Rhodes","Randy Orton","Tables")<=0)throw new Exception("Positives Ergebnis wurde nicht gelernt.");
var learnedPlan=PlannerEngine.Generate(roster,objectives,50_000,learned.Profile);
if(!learnedPlan.Matches.Any(x=>x.A.Name.Contains("Cody")&&x.B.Name.Contains("Randy")||x.A.Name.Contains("Randy")&&x.B.Name.Contains("Cody")))throw new Exception("Starke gelernte Paarung fehlt.");
Console.WriteLine(plan.Render());
var ecw=CareerProfileService.CurrentEcwCalibration();
var ecwPlan=PlannerEngine.Generate(ecw.RosterRows,[],1_042_425,null,ecw.MatchSlots,ecw.PromoSlots);
if(ecwPlan.Matches.Count!=ecw.MatchSlots)throw new Exception("ECW-Spielstand hat nicht vier sichere Matches erhalten.");
if(ecwPlan.Promos.Count!=2)throw new Exception("ECW-Spielstand muss wegen fünf Verletzungen genau zwei sichere Promos erhalten.");
if(ecwPlan.Matches.SelectMany(x=>new[]{x.A.Name,x.B.Name}).Any(x=>x is "Roman Reigns" or "Finn Bálor" or "Omos"))throw new Exception("Verletzter Superstar wurde gebucht.");
if(ecwPlan.Promos.Any(x=>x.Star.Name is "Kevin Owens" or "Cody Rhodes" or "Roman Reigns" or "Finn Bálor" or "Omos"))throw new Exception("Verletzter Superstar wurde für eine Promo gebucht.");
var fiveMatchPlan=PlannerEngine.Generate(roster,objectives,50_000,null,5,1);
if(fiveMatchPlan.Matches.Count!=5||fiveMatchPlan.Promos.Count!=1)throw new Exception("Dynamische Showplätze wurden nicht beachtet.");
var metadata=CareerMetadata.Empty() with{InjuryWeeks=new(){{"Kevin Owens",2}},ContractWeeks=new(){{"Seth Rollins",2}},NextPleWeek=16,Budget=900_000,Fans=1_600_000,Business=[new(12,850_000,1_550_000,80_000,40_000),new(13,900_000,1_600_000,100_000,35_000)],FreeAgents=[new("Test Giant","M","Heel","Giant",70,80,40_000)]};
var repeated=new[]{new ShowOutcome(12,[new("Cody Rhodes","Randy Orton","Normal",4.0)],[],4,"",DateTimeOffset.Now),new ShowOutcome(13,[new("Cody Rhodes","Randy Orton","Normal",4.5)],[],4.2,"",DateTimeOffset.Now)};
var career=CareerIntelligenceEngine.Build(ecw,metadata,repeated,["Versprechen | Cody Rhodes | Gewinne dein nächstes Match | 12 | Hoch | Offen"],"Erwartung: 3,5–4,0 ★",Path.GetTempPath());
if(!career.Notices.Any(x=>x.Group=="VERLETZUNG"&&x.Title=="Kevin Owens"))throw new Exception("Verletzungskalender fehlt.");
if(!career.Notices.Any(x=>x.Group=="WIEDERHOLUNG"))throw new Exception("Wiederholungswarnung fehlt.");
if(!career.Notices.Any(x=>x.Group=="PROGNOSE-CHECK"))throw new Exception("Sternevergleich fehlt.");
if(!career.Notices.Any(x=>x.Group=="BIS ZUM PLE"&&x.Title=="Woche 16"))throw new Exception("PLE-Plan fehlt.");
if(MyGmScreenClassifier.Classify("STARTSEITE SHOW 13 BUDGET 1.000.000 $")!=MyGmScreen.Home)throw new Exception("Startseite nicht erkannt.");
if(MyGmScreenClassifier.Classify("SHOW BUCHEN AUFTAKT MIDCARD HAUPTEVENT")!=MyGmScreen.Booking)throw new Exception("Buchungsseite nicht erkannt.");
if(MyGmScreenClassifier.Classify("zufälliger unbekannter Text")!=MyGmScreen.Unknown)throw new Exception("Unbekannter Bildschirm muss die Automatik stoppen.");
var ple=MyGmEventParser.Parse("NÄCHSTES EVENT HELL IN A CELL WOCHE 16",13);
if(ple?.PreferredMatchType!="Hell in a Cell"||ple.Week!=16)throw new Exception("PLE-Kontext nicht erkannt.");
var statuses=ObjectiveParser.Parse("Cody Rhodes: nächstes Match gewinnen – ERFÜLLT\nRandy Orton: nächstes Match gewinnen – GESCHEITERT",13);
if(!statuses.Any(x=>x.Status=="Erfüllt")||!statuses.Any(x=>x.Status=="Gescheitert"))throw new Exception("Versprechenstatus nicht erkannt.");
