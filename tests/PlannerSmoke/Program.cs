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
if(ecwPlan.Promos.Count!=ecw.PromoSlots)throw new Exception("ECW-Spielstand hat nicht drei Promos erhalten.");
if(ecwPlan.Matches.SelectMany(x=>new[]{x.A.Name,x.B.Name}).Any(x=>x is "Roman Reigns" or "Finn Bálor" or "Omos"))throw new Exception("Verletzter Superstar wurde gebucht.");
var fiveMatchPlan=PlannerEngine.Generate(roster,objectives,50_000,null,5,1);
if(fiveMatchPlan.Matches.Count!=5||fiveMatchPlan.Promos.Count!=1)throw new Exception("Dynamische Showplätze wurden nicht beachtet.");
