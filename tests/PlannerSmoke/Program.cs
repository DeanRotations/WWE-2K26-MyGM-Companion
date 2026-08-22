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
Console.WriteLine(plan.Render());

