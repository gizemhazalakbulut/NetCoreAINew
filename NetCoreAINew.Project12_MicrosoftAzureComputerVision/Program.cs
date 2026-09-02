using System.Net.Http.Headers;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string imagePath = "C:\\Users\\gizem.akbulut\\Downloads\\04.jpg";
        string subscriptionKey = "";
        string endpoint = "https://murat-vision-ai.cognitiveservices.azure.com";

        string apiUrl = $"{endpoint}/vision/v3.2/analyze";

        string requestParameters = "visualFeatures=Categories,Description,Tags,Color&language=en"; // İstek parametreleri, görselin hangi özelliklerinin analiz edileceğini belirler. Bu örnekte, kategoriler, açıklamalar, etiketler ve renk bilgisi isteniyor.
        string uri = apiUrl + "?" + requestParameters; // Tam API URL'si, istek parametreleri ile birleştirilir.

        if (!File.Exists(imagePath)) // Görsel dosyasının varlığını kontrol eder. Eğer dosya yoksa, kullanıcıya bilgi verir ve işlemi sonlandırır.
        {
            Console.WriteLine("Görsel dosyası bulunamadı!" + imagePath);
            return;
        }

        byte[] imageBytes = await File.ReadAllBytesAsync(imagePath); // Görsel dosyasını byte dizisine dönüştürür. Bu, görselin API'ye gönderilmesi için gereklidir.

        using (HttpClient client = new HttpClient())
        using (ByteArrayContent content = new ByteArrayContent(imageBytes)) // ByteArrayContent, görselin byte dizisini HTTP isteği için uygun bir içerik türüne dönüştürür.
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            HttpResponseMessage response = await client.PostAsync(uri, content);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Azure Yanıtı: ");
                JsonDocument json = JsonDocument.Parse(result); // JSON yanıtını ayrıştırır ve analiz edilen görselin açıklama ve güven skorunu alır.
                var description = json.RootElement.GetProperty("description").GetProperty("captions")[0]; // Görselin açıklama kısmını alır. "captions" dizisinin ilk öğesi, görselin en iyi açıklamasını içerir.
                string text = description.GetProperty("text").GetString(); // Görselin açıklama metnini alır.
                double confidence = description.GetProperty("confidence").GetDouble(); // Açıklamanın güven skorunu alır. Bu, modelin açıklamanın doğruluğuna olan güvenini temsil eder.

                Console.WriteLine($"Açıklama: {text} (Güven: %{confidence * 100:0.00})");
            }
            else
            {
                Console.WriteLine("bir hata oluştu!");
                Console.WriteLine($"Status {response.StatusCode}");
                Console.WriteLine("Yanıt: " + result);
            }
        }
    }
}