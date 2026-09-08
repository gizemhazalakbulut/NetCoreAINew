using NAudio.Wave;
using System.Threading;
using System.Net.Http.Headers;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("🤖 Sesli Chatbot Başladı. Konuşmak için Enter tuşuna basınız...");

        while (true)
        {
            Console.ReadLine(); // Kullanıcı Enter tuşuna basana kadar bekle
            string audioFilePath = "recorded.wav"; // Kullanıcının sesli girdisinin kaydedileceği dosya yolu

            //1.Mikrofon kaydı al 
            Console.WriteLine("🎤 Konuşmaya başlayın...");

            RecordAudio(audioFilePath); // Ses kaydını başlat
            Console.WriteLine("⏹️ Kayıt tamamlandı...");

            //2. Ses dosyasını OpenAI'ye gönder ve metin olarak al. Whisper modelini kullanarak ses dosyasını metne dönüştür
            string transcription = await TranscribeAudioAsync(audioFilePath);
            Console.WriteLine($"📝 Transkripsiyon: {transcription}");

            //3. Transkripsiyonu OpenAI ChatGPT modeline gönder ve yanıt al
            string reply = await AskChatGptAsync(transcription);
            Console.WriteLine($"💬 ChatGPT Yanıtı: {reply}");

            //4. Yanıtı sesli olarak çal
            var synth= new SpeechSynthesizer();
            synth.Speak(reply); // Yanıtı sesli olarak çal


        }
    }
     
    // Mikrofonla 10 saniyelik bir ses kaydı yapıyor ve bunu .wav dosyası olarak belirttiğin outputFilePath'e kaydediyor.
    static void RecordAudio(string outputFilePath)
    {

        using var waveIn = new WaveInEvent();
        waveIn.WaveFormat=new WaveFormat(16000,1);
        using var writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
        waveIn.DataAvailable += (s, a) => writer.Write(a.Buffer, 0, a.BytesRecorded);
        waveIn.StartRecording();
        Thread.Sleep(1000);
        waveIn.StopRecording();
    }

    static async Task<string> TranscribeAudioAsync(string audioFilePath)
    {
        string apiKey = "";
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        using var form = new MultipartFormDataContent(); // Ses dosyasını ve model adını form-data olarak ekle
        using var fs = File.OpenRead(audioFilePath); // Ses dosyasını aç
        form.Add(new StreamContent(fs), "file", Path.GetFileName(audioFilePath)); // Ses dosyasını form-data'ya ekle
        form.Add(new StringContent("whisper-1"), "model"); // Whisper modelini belirt

        var response  = await httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", form); // OpenAI API'ye POST isteği gönder
        var result = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(result); // JSON yanıtını ayrıştır
        return doc.RootElement.GetProperty("text").GetString(); // Transkripsiyonu döndür

    }

    static async Task<string> AskChatGptAsync(string userMessage)
    {
        string apiKey = "";
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        var payload = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                //new { role = "system", content = "Sen bir yardımcı asistan olarak görev yapıyorsun." },
                new { role = "user", content = userMessage }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload),Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var result = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(result);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();



    }
}