using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;

class Program
{
    static async Task Main(string[] args)
    {
        string pdfPath = "C:\\Users\\gizem.akbulut\\Downloads\\kitap.pdf";
        string apiKey = "";

        if (!File.Exists(pdfPath))
        {
            Console.WriteLine("Pdf Dosyası bulunamadı!");
            return;
        }

        string pdfText = "";
        using (var document = PdfDocument.Open(pdfPath))
        {
            foreach (var page in document.GetPages())
            {
                pdfText += page.Text + "\n";
            }
        }

        string prompt = $"Aşağıdaki metni detaylıca özetler misin?\n\n{pdfText}";

        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.anthropic.com/");
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var requestBody = new
        {
            model = "claude-3-opus-20240229",
            max_tokens = 1000,
            temperature = 0.5,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            }
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"); // Bu satır, requestBody nesnesini JSON'a çevirip HTTP isteğinde gönderilebilecek bir body/content haline getiriyor. Yani gönderilecek paketin içeriği jsonContent değişkeninde tutuluyor. Bu, Claude API'sine gönderilecek olan mesajın içeriğini temsil ediyor.

        var response = await client.PostAsync("v1/messages", jsonContent);
        var reponseString = await response.Content.ReadAsStringAsync(); //API’nin döndürdüğü body’yi string olarak okuyorsun

        Console.WriteLine("Claude Pdf Özeti: ");
        Console.WriteLine(reponseString);

    }
}
//C:\Users\gizem.akbulut\Downloads\kitap.pdf

//Burada en çok karışan nokta şu oluyor:
//jsonContent = senin API'ye gönderdiğin şey.
//response.Content = API'nin sana geri gönderdiği şey.

// Serialize = C# → JSON
// Deserialize = JSON → C#

/*
Yani en basit haliyle şu:

requestBody
   ↓
C# nesnesi

JsonSerializer.Serialize
   ↓
JSON

StringContent
   ↓
HTTP body

PostAsync
   ↓
API'ye gönder

response
   ↓
API'nin HTTP cevabı

ReadAsStringAsync
   ↓
API'nin döndürdüğü JSON/string
 
 * 
 */



/* Şöyle düşünmen bence daha kolay:
  // 1. API'ye ne göndereceğim?
var requestBody = new
{
    Phone = "5551234567",
    Message = "Merhaba"
};

// 2. Göndereceğim şeyi API'nin anlayacağı JSON'a çevir
var json = JsonSerializer.Serialize(requestBody);

// 3. JSON'u HTTP body haline getir
var jsonContent = new StringContent(
    json,
    Encoding.UTF8,
    "application/json"
);

// 4. API'ye gönder
var response = await client.PostAsync("v1/messages", jsonContent);

// 5. API başarılı cevap verdi mi?
if (response.IsSuccessStatusCode)
{
    // 6. API'nin verdiği cevabı oku
    var responseString = await response.Content.ReadAsStringAsync();
}
*/