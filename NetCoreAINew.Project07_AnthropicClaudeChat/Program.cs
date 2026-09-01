using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "";
        Console.Write("Lütfen sormak istediğiniz soruyu yazınız: ");
        string prompt = Console.ReadLine();

        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.anthropic.com"); // BaseAddress: İstekler bu köke ek path getirerek yapılacak.
        client.DefaultRequestHeaders.Add("x-api-key", apiKey); // x-api-key: Anthropic'in beklediği kimlik başlığı.
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01"); // anthropic-version: API sürümünü belirtir. Sürüm sabitleme: API'nin gelecekteki değişikliklerinden etkilenmemek için.
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); // Accept: Sunucudan JSON formatında yanıt beklediğimizi belirtir.

        // İstek gövdesi oluşturuluyor. Gönderilecek JSON gövdesini oluşturma
        var requestBody = new
        {
            model = "claude-3-opus-20240229", // Kullanılacak Claude modelini belirtir. Claude-3-opus-20240229, Claude 3'ün belirli bir sürümüdür.
            max_tokens = 1000, // max_tokens: Yanıtın maksimum token sayısını belirtir. Token, kelime veya kelime parçacığı olabilir.
            temperature = 0.7, // temperature: Yanıtın rastgeleliğini kontrol eder. 0.7, orta derecede rastgelelik sağlar. Yaratıcılık derecesi.
            messages = new[]
            {
                new // messages: Kullanıcı ve model arasındaki konuşmayı temsil eder. Claude, bu mesajları kullanarak yanıt üretir. İlk kullanıcı mesajı, modelin yanıtını tetikler. Bu örnekte tek bir kullanıcı mesajı var.
                { 
                    role="user",
                    content=prompt
                }
            }
        };
        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"); // JsonSerializer.Serialize : Nesneyi JSON stringine çevirir. StringContent: HTTP isteği gövdesi olarak JSON stringini ayarlar.

        var response = await client.PostAsync("v1/messages", jsonContent); // PostAsync: HTTP POST isteği gönderir. "v1/messages" path'i, Claude modeline mesaj göndermek için kullanılır.
        var responseString = await response.Content.ReadAsStringAsync(); // ReadAsStringAsync: Yanıt gövdesini string olarak okur. Claude modelinin yanıtını JSON formatında alır.

        var doc = JsonDocument.Parse(responseString);
        var contentElement = doc.RootElement.GetProperty("content")[0];
        var text = contentElement.GetProperty("text").GetString();

        // ANSI renk kodları
        string yellow = "\u001b[33m";
        string green = "\u001b[32m";
        string cyan = "\u001b[36m";
        string reset = "\u001b[0m";

        Console.WriteLine();
        Console.WriteLine($"{cyan}{new string('-', 60)}{reset}");
        Console.WriteLine($"{yellow} CLAUDE YANITI{reset}");
        Console.WriteLine($"{cyan}{new string('-', 60)}{reset}");
        Console.WriteLine();
        Console.WriteLine($"{green}{text}{reset}");
        Console.WriteLine();
        Console.WriteLine($"{cyan}{new string('-', 60)}{reset}");
        Console.WriteLine($"{yellow} Cevap tamamlandı.{reset}");
        Console.WriteLine($"{cyan}{new string('-', 60)}{reset}");
    }
}

