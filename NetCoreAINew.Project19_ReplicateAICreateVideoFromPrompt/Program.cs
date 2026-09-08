using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;// Emojileri desteklemek için çıktı kodlamasını UTF-8 olarak ayarlar
        Console.WriteLine("🎬 Replicate AI ile Video Üretici Uygulaması");

        Console.Write("Please Input Here Prompt Text:");
        string prompt = Console.ReadLine();

        string apiKey = "";
        string version = "9f747673945c62801b13b84701c783929c0ee784e4748ec062204894dda1a351";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", apiKey);

        // Bölüm 1-Üretim İsteği 
        var body = new 
        {
            version, // Model sürümü
            input = new // Girdi parametreleri
            {
                prompt, // Kullanıcıdan alınan metin girdisi
                num_frames = 24, // Video uzunluğu (saniye cinsinden)
                fps = 8, // Video kare hızı
                guidance_scale = 12.5, // Yaratıcılık seviyesi
                num_inference_steps = 50, // Modelin üretim adım sayısı
                width = 576, // Video genişliği (piksel)
                height = 320 // Video yüksekliği (piksel)
            }
        };

        var json = JsonSerializer.Serialize(body);
        var response = await client.PostAsync("https://api.replicate.com/v1/predictions", new StringContent(json, Encoding.UTF8, "application/json"));
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("API Hatası: " + await response.Content.ReadAsStringAsync());
            return;
        }

        var pred = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        string id = pred.RootElement.GetProperty("id").GetString(); // Üretim isteği ID'si
        Console.WriteLine("🎨 Video üretiliyor...");

        //Bölüm 2 Durum Sorgulama Döngüsü
        string status = "";
        string videoUrl = "";
        while (status != "succeeded") // Üretim tamamlanana kadar döngü
        {
            await Task.Delay(5000); // 5 saniye bekle
            var chk = await client.GetAsync($"https://api.replicate.com/v1/predictions/{id}"); // Üretim durumunu sorgula
            var chkJson = JsonDocument.Parse(await chk.Content.ReadAsStringAsync()); // JSON yanıtını ayrıştır
            status = chkJson.RootElement.GetProperty("status").GetString(); // Üretim durumunu al
            Console.WriteLine($"⌛ Durum: {status}"); // Durumu konsola yazdır
            if (status == "failed") // Üretim başarısız olduysa döngüden çık
            {
                Console.WriteLine("Üretim başarısız oldu");
                return;
            }
            if (status == "succeeded") // Üretim başarılı olduysa video URL'sini al
            {
                var output = chkJson.RootElement.GetProperty("output"); // Üretim çıktısını al
                videoUrl = output.ValueKind == JsonValueKind.Array ? output[0].GetString() : output.GetString(); // Video URL'sini al
            }
        }

        Console.WriteLine($"💎 Video hazır: {videoUrl}");

        //Bölüm 3 İndirme
        using var stream = await client.GetStreamAsync(videoUrl); // Video akışını al
        await using var file = File.Create("generated_video.mp4"); // Video dosyasını oluştur
        await stream.CopyToAsync(file); // Video akışını dosyaya yaz
        Console.WriteLine("🎉 Video indirildi -> generated_video.mp4");

        //Bölüm 4 otomatik aç
        Process.Start(new ProcessStartInfo // Video dosyasını varsayılan oynatıcı ile aç
        {
            FileName = "generated_video.mp4", // Açılacak dosya adı
            UseShellExecute = true           // Varsayılan uygulama ile aç
        });


    }
}