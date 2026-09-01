using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "";
        string prompt = "Bana 'Yazılım Geliştirici' pozisyonu için hazırlanan, profesyonel ama samimi tonda bir iş başvuru e-postası yazar mısın? Adım Murat, 5 yıldır .Net geliştiricisiyim, ekip çalışmasına yatkınım, ve uzaktan çalışmaya uygunum.";

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

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("v1/messages", jsonContent);
        var responseString = await response.Content.ReadAsStringAsync();
        var json = JsonNode.Parse(responseString); // JSON yanıtını JsonNode olarak ayrıştırıyoruz, böylece içeriğe kolayca erişebiliriz.
        string? textContent = json?["content"]?[0]?["text"]?.ToString(); // JSON yanıtındaki "content" alanının ilk öğesinin "text" alanını alıyoruz. Eğer bu alan yoksa null dönecektir.

        // Renkli çıktı için konsol ayarları
        Console.OutputEncoding = Encoding.UTF8;

        // Çerçeve ve başlık
        PrintColoredFrame();
        PrintColoredTitle("Oluşturulan E-Posta");
        PrintColoredFrame();

        // E-posta içeriğini renkli yazdır
        if (!string.IsNullOrEmpty(textContent))
        {
            PrintColoredContent(textContent);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("E-posta içeriği alınamadı.");
            Console.ResetColor();
        }

        PrintColoredFrame();

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("\nİşlem tamamlandı. Devam etmek için bir tuşa basın...");
        Console.ResetColor();
        Console.ReadKey();
    }

    static void PrintColoredFrame()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        string frame = new string('-', 80);
        Console.WriteLine(frame);
        Console.ResetColor();
    }

    static void PrintColoredTitle(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.BackgroundColor = ConsoleColor.DarkBlue;

        int padding = (80 - title.Length) / 2;
        string centeredTitle = title.PadLeft(title.Length + padding).PadRight(80);

        Console.WriteLine(centeredTitle);
        Console.ResetColor();
    }

    static void PrintColoredContent(string content)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkGreen;

        // İçeriği satırlara böl ve her satırı çerçeve içinde yazdır
        string[] lines = content.Split('\n');

        foreach (string line in lines)
        {
            // Her satırı 76 karakterde sınırla (çerçeve için 2 karakter boşluk)
            if (line.Length <= 76)
            {
                Console.WriteLine($" {line.PadRight(78)} ");
            }
            else
            {
                // Uzun satırları böl
                var words = line.Split(' ');
                var currentLine = "";

                foreach (var word in words)
                {
                    if ((currentLine + " " + word).Length <= 76)
                    {
                        currentLine += (currentLine.Length > 0 ? " " : "") + word;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(currentLine))
                        {
                            Console.WriteLine($" {currentLine.PadRight(78)} ");
                        }
                        currentLine = word;
                    }
                }

                if (!string.IsNullOrEmpty(currentLine))
                {
                    Console.WriteLine($" {currentLine.PadRight(78)} ");
                }
            }
        }

        Console.ResetColor();
    }
}