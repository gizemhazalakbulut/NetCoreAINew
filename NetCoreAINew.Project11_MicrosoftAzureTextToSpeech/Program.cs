using System.Net.Http.Headers;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        string subscriptonKey = "";
        string region = "westeurope"; // Azure bölgesi (batıavrupa)
        string tokenEndPoint = $"https://{region}.api.cognitive.microsoft.com/sts/v1.0/issuetoken";
    

        var token = await GetTokenAsync(subscriptonKey, tokenEndPoint);
        string userText = "Merhaba arkadaşlar, bu bir deneme mesajıdır. Amacımız Microsoft Azure kullanarak metni sese dönüştürmektir. Umarım başarılı olabiliriz.";
        await SynthesizeSpeechAsync(token, region, userText); // Metni sese dönüştürme işlemi metodu çağrılıyor.
    }

    static async Task<string> GetTokenAsync(string key, string endPoint)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
        var response = await client.PostAsync(endPoint, null);
        return await response.Content.ReadAsStringAsync();
    }

    static async Task SynthesizeSpeechAsync(string token, string region, string text) // Metni sese dönüştürme işlemi metodu.
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("User-Agent", "AzureTTSClient"); // Kullanıcı ajanı başlığı ekleniyor
        client.DefaultRequestHeaders.Add("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm"); // Çıkış formatı başlığı ekleniyor (16kHz, 16 bit, mono PCM)

        string ssml = $@" 
<speak version='1.0' xml:lang='en-US'>
  <voice xml:lang='tr-TR' name='tr-TR-AhmetNeural'>{text}</voice>
</speak>";  // SSML (Speech Synthesis Markup Language) formatında metin oluşturuluyor. Burada seslendirme dili ve sesi belirleniyor.

        var content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml"); // SSML içeriği oluşturuluyor ve HTTP isteği için hazır hale getiriliyor.
        var result = await client.PostAsync($"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1", content); // Azure TTS API'sine POST isteği gönderiliyor ve yanıt alınıyor.

        if (result.IsSuccessStatusCode)
        {
            var audioBytes = await result.Content.ReadAsByteArrayAsync(); // Yanıt içeriği byte dizisine dönüştürülüyor (ses verisi).
            File.WriteAllBytes("output2.wav", audioBytes); // Ses verisi "output.wav" dosyasına yazdırılıyor.
            Console.WriteLine("Ses dosyası oluşturuldu: output.wav"); // Başarılı işlem mesajı yazdırılıyor.
        }
        else
        {
            Console.WriteLine("Hata: " + result.StatusCode);
            Console.WriteLine(await result.Content.ReadAsStringAsync());
        }
    }
}