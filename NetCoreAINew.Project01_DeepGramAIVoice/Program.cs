using System.Net.Http.Headers;
using System.Text.Json;

var apiKey = "";
var filePath = "testeng.mp3";

if(!File.Exists(filePath))
{
    Console.WriteLine("Dosya bulunamadı!");
    return;
}

using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Token",apiKey);
using var fileStream = File.OpenRead(filePath);

var content = new StreamContent(fileStream);
content.Headers.ContentType = new MediaTypeHeaderValue("audio/mp3");

var response = await client.PostAsync("https://api.deepgram.com/v1/listen?model=general&language=en", content);
var json=await response.Content.ReadAsStringAsync();

try
{
    var doc=JsonDocument.Parse(json);
    var transcript=doc.RootElement.GetProperty("results").GetProperty("channels")[0].GetProperty("alternatives")[0].GetProperty("transcript").GetString();
    Console.WriteLine();
    Console.WriteLine("Transkript: " + transcript);
}
catch (Exception ex)
{
    Console.WriteLine("Hata oluştu: " + ex.Message);
    Console.WriteLine("JSON yanıtı: " + json);
    throw;
}