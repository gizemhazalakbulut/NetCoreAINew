using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("🤖 Prompt'tan Görsel Üretici V1 - Stability AI");
        Console.Write("Lütfen prompt girin (örn: a wearing sunglasses on a beach): ");
        string prompt = Console.ReadLine();

        string apiKey = "";
        string engineId = "stable-diffusion-v1-6";
        string apiUrl = $"https://api.stability.ai/v1/generation/{engineId}/text-to-image";

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var requestBody = new // JSON isteği için gövde oluşturuluyor. Bu yapı, Stability AI API'sine gönderilecek parametreleri içerir.
        {
            text_prompts = new[] // Promptları içeren dizi
            {
                new
                {
                    text=prompt // Kullanıcının girdiği prompt
                }
            },
            cfg_scale = 12, // Daha yüksek değerler, modelin prompta daha sıkı uymasını sağlar. Önerilen aralık: 7-15
            height = 512, // Görselin yüksekliği (piksel cinsinden)
            width = 512, // Görselin genişliği (piksel cinsinden)
            steps = 30, // Daha yüksek değerler, daha fazla detay ve kalite sağlar. Önerilen aralık: 20-50
            samples = 1 // Üretilen görsel sayısı
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(apiUrl, jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Hata: " + response.StatusCode);
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine(error);
            return;
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var responseJson = JsonDocument.Parse(responseString);

        string base64İmage = responseJson // JSON yanıtını ayrıştırır ve yanıtın kök öğesini temsil eden bir JsonDocument nesnesi oluşturur. Bu, yanıtın içeriğine erişmek için kullanılır.
            .RootElement
            .GetProperty("artifacts")[0] // "artifacts" dizisinin ilk öğesini alır. Bu, üretilen görselin verilerini temsil eder.
            .GetProperty("base64") // "base64" alanını alır. Bu, üretilen görselin Base64 kodlu verisini temsil eder.
            .GetString();

        byte[] imageBytes = Convert.FromBase64String(base64İmage); // Base64 kodlu görsel verisini byte dizisine dönüştürür.
        string fileName = $"generated_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
        await File.WriteAllBytesAsync(fileName, imageBytes);

        Console.WriteLine($"🎈 Görsel başarıyla oluşturuldu ve kaydedildi: {fileName}");
    }
}