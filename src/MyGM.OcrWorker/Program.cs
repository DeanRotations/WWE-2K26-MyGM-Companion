using System.Text.Json;
// Isolierter, standardmäßig ungenutzter Worker. Spätere OCR erhält genau eine Datei und endet danach.
if(args.Length<1){Environment.ExitCode=2;return;}
var request=args[0]; using var cts=new CancellationTokenSource(TimeSpan.FromSeconds(12));
try { var json=await File.ReadAllTextAsync(request,cts.Token); using var doc=JsonDocument.Parse(json); var response=new { ok=true, engine="disabled", text="", message="OCR ist in V10.6 Diagnosephase bewusst deaktiviert." }; Console.Write(JsonSerializer.Serialize(response)); }
catch(OperationCanceledException){Environment.ExitCode=124;} catch{Environment.ExitCode=1;}
