using System.Diagnostics;
using System.Text.Json;
using System.IO;
namespace MyGM.Companion;
public sealed record OcrWorkerResponse(bool Ok,string Text,string Language,string? Error);
public sealed class OcrProcessService {
 public async Task<OcrWorkerResponse> RecognizeAsync(string imagePath,CancellationToken token){var worker=Path.Combine(AppContext.BaseDirectory,"MyGM.OcrWorker.exe");if(!File.Exists(worker))throw new FileNotFoundException("OCR-Worker fehlt.");var id=Guid.NewGuid().ToString("N");var requestPath=Path.Combine(Path.GetTempPath(),$"mygm-ocr-{id}.json");var responsePath=Path.Combine(Path.GetTempPath(),$"mygm-ocr-{id}-response.json");await File.WriteAllTextAsync(requestPath,JsonSerializer.Serialize(new{ImagePath=imagePath,ResponsePath=responsePath}),token);using var process=new Process{StartInfo=new ProcessStartInfo(worker,$"\"{requestPath}\""){UseShellExecute=false,CreateNoWindow=true,WindowStyle=ProcessWindowStyle.Hidden}};try{process.Start();try{await process.WaitForExitAsync(token).WaitAsync(TimeSpan.FromSeconds(15),token);}catch{if(!process.HasExited)process.Kill(true);throw;}if(!File.Exists(responsePath))throw new InvalidDataException("OCR-Worker lieferte keine Antwort.");return JsonSerializer.Deserialize<OcrWorkerResponse>(await File.ReadAllTextAsync(responsePath,token),new JsonSerializerOptions{PropertyNameCaseInsensitive=true})??throw new InvalidDataException("OCR-Antwort ist ungültig.");}finally{try{File.Delete(requestPath);}catch{}try{File.Delete(responsePath);}catch{}}}
}

