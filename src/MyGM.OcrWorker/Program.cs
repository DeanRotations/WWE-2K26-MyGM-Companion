using System.Text.Json;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;

internal sealed record OcrRequest(string ImagePath,string ResponsePath);
internal sealed record OcrResponse(bool Ok,string Text,string Language,string? Error);

internal static class Program {
 [STAThread]
 static async Task Main(string[] args){OcrRequest? request=null;try{if(args.Length!=1)throw new ArgumentException("OCR-Anfrage fehlt.");request=JsonSerializer.Deserialize<OcrRequest>(await File.ReadAllTextAsync(args[0]))??throw new InvalidDataException("OCR-Anfrage ist ungültig.");using var cts=new CancellationTokenSource(TimeSpan.FromSeconds(12));var file=await StorageFile.GetFileFromPathAsync(Path.GetFullPath(request.ImagePath)).AsTask(cts.Token);await using var stream=await file.OpenStreamForReadAsync();var decoder=await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream()).AsTask(cts.Token);using var bitmap=await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8,BitmapAlphaMode.Premultiplied).AsTask(cts.Token);var engine=OcrEngine.TryCreateFromUserProfileLanguages()??throw new InvalidOperationException("Keine Windows-OCR-Sprache installiert.");var result=await engine.RecognizeAsync(bitmap).AsTask(cts.Token);await File.WriteAllTextAsync(request.ResponsePath,JsonSerializer.Serialize(new OcrResponse(true,result.Text,engine.RecognizerLanguage.LanguageTag,null)),cts.Token);}catch(Exception ex){if(request?.ResponsePath is not null)try{await File.WriteAllTextAsync(request.ResponsePath,JsonSerializer.Serialize(new OcrResponse(false,"","",ex.Message)));}catch{}Environment.ExitCode=1;}}
}

