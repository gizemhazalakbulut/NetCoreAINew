using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "";
        string model = "gemini-1.5-pro";
        string endpoint = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";
        Console.Write("Sormak istediğiniz soruyu yazın: ");
        string question = Console.ReadLine();

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = question }
                    }
                }
            }
        };
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.PostAsync(endpoint, content);
        var responseText = await response.Content.ReadAsStringAsync();

        try
        {
            var doc = JsonDocument.Parse(responseText); // JSON yanıtını ayrıştırır ve yanıtın kök öğesini temsil eden bir JsonDocument nesnesi oluşturur. Bu, yanıtın içeriğine erişmek için kullanılır.
            string answer = doc.RootElement // Yanıtın kök öğesini temsil eden JsonElement nesnesi üzerinden yanıtın içeriğine erişir. "candidates" dizisinin ilk öğesinin "content" alanının "parts" dizisinin ilk öğesinin "text" alanını alır. Bu, modelin verdiği yanıt metnini temsil eder.
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            Console.WriteLine("Gemini Cevap: " + answer);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Yanıt çözümlemesi başarısız oldu");
            Console.WriteLine("Gelen Yanıt: " + responseText + ex);
        }
    }
}
