# 🤖 C# ile 20 Yapay Zeka Entegrasyonu

.NET / C# kullanarak **20 farklı yapay zeka servisini** doğrudan REST API'ler üzerinden entegre eden konsol uygulamaları koleksiyonu. Metin analizi, özetleme, sohbet, görüntü ve video üretimi, sesli asistan ve daha fazlası — her biri ayrı bir proje olarak.

> **AI Masterclass: C# ile 20 Yapay Zeka Entegrasyonu (Part 2)** Udemy kursu kapsamında geliştirilmiştir.
> 👨‍🏫 Eğitmen: **Murat Yücedağ** — M&Y Yazılım Eğitim Akademi Danışmanlık

---

## 🧠 Entegre Edilen Yapay Zeka Servisleri

| Servis | Proje Sayısı | Kullanım Alanları |
|--------|:---:|-------------------|
| 🤗 **Hugging Face** | 5 | Duygu analizi, özetleme, NER, soru-cevap, toksisite tespiti |
| 🧡 **Anthropic Claude** | 3 | Sohbet, PDF özetleme, e-posta üretimi |
| 🔷 **Google Gemini** | 3 | Soru-cevap, rol simülasyonu, otonom ajan |
| 🌩️ **Microsoft Azure** | 3 | Metinden sese, görüntü açıklama, nesne tespiti |
| 🎨 **Replicate** | 2 | Metinden görsel, metinden video |
| 🟢 **OpenAI** | 2 | Kod asistanı, sesli chatbot |
| 🎙️ **DeepGram** | 1 | Sesten metne (transkripsiyon) |
| 🖼️ **Stability AI** | 1 | Metinden görsel (Stable Diffusion) |

---

## 📚 Projeler

### 🤗 Hugging Face

| # | Proje | Açıklama | Model |
|---|-------|----------|-------|
| 02 | [Sentiment Analysis](NetCoreAINew.Project02_HuggingFaceSentimentAnalysis) | Metnin duygu durumunu analiz eder (olumlu/nötr/olumsuz) | `twitter-roberta-base-sentiment` |
| 03 | [Summarize Text](NetCoreAINew.Project03_HuggingFaceSummarizeText) | Uzun metinleri özetler | `distilbart-cnn-12-6` |
| 04 | [Named Entity Recognition](NetCoreAINew.Project04_HuggingFaceNamedEntityRecognition) | Metindeki varlıkları (kişi, yer, kurum) tanır | `bert-base-NER` |
| 05 | [Roberta Base QA](NetCoreAINew.Project05_HuggingFaceRobertaBaseQA) | Verilen bağlamdan soruları yanıtlar | `roberta-base-squad2` |
| 06 | [Toxic Bert](NetCoreAINew.Project06_HuggingFaceToxicBert) | Metindeki toksik/saldırgan içeriği tespit eder | `unitary/toxic-bert` |

### 🧡 Anthropic Claude

| # | Proje | Açıklama |
|---|-------|----------|
| 07 | [Claude Chat](NetCoreAINew.Project07_AnthropicClaudeChat) | Genel amaçlı soru-cevap sohbeti |
| 08 | [Claude PDF Summary](NetCoreAINew.Project08_AnthropicClaudePdfSummary) | PDF dosyasından metni çıkarıp özetler (PdfPig) |
| 09 | [Claude Job Email](NetCoreAINew.Project09_AnthropicClaudeJobEmail) | Profesyonel iş başvuru e-postası üretir |

### 🔷 Google Gemini

| # | Proje | Açıklama |
|---|-------|----------|
| 14 | [Question Answer](NetCoreAINew.Project14_GoogleGeminiQuestionAnswer) | Kullanıcının sorularını yanıtlar |
| 15 | [Role Simulation](NetCoreAINew.Project15_GoogleGeminiRoleSimulation) | Seçilen role (psikolog, tarihçi, rehber...) bürünerek yanıt verir |
| 16 | [Auto Agent Prompt](NetCoreAINew.Project15_GoogleGeminiAutoAgentPrompt) | Kullanıcıya adım adım sorular sorup içerik planı üreten otonom ajan |

### 🌩️ Microsoft Azure

| # | Proje | Açıklama |
|---|-------|----------|
| 11 | [Text To Speech](NetCoreAINew.Project11_MicrosoftAzureTextToSpeech) | Metni SSML ile seslendirip WAV dosyası oluşturur |
| 12 | [Computer Vision](NetCoreAINew.Project12_MicrosoftAzureComputerVision) | Görseli açıklar ve güven skoru döner |
| 13 | [Vision Object & Details](NetCoreAINew.Project13_MicrosoftAzureComputerVisionObjectAndDetails) | Görseldeki nesneleri ve detayları (etiket, yüz, marka) tespit eder |

### 🎨 Görsel & Video Üretimi

| # | Proje | Servis | Açıklama |
|---|-------|--------|----------|
| 10 | [Draw Image](NetCoreAINew.Project10_ReplicateAIDrawImage) | Replicate | Metinden görsel üretir |
| 18 | [Stable Diffusion](NetCoreAINew.Project18_StabilityAIStableDiffusion) | Stability AI | Metinden görsel üretir (`stable-diffusion-v1-6`) |
| 19 | [Create Video From Prompt](NetCoreAINew.Project19_ReplicateAICreateVideoFromPrompt) | Replicate | Metinden video üretir |

### 🎙️ Ses & OpenAI

| # | Proje | Servis | Açıklama |
|---|-------|--------|----------|
| 01 | [DeepGram AI Voice](NetCoreAINew.Project01_DeepGramAIVoice) | DeepGram | MP3 ses dosyasını metne çevirir |
| 17 | [Code Editor](NetCoreAINew.Project17_OpenAICodeEditor) | OpenAI | C# kodunu açıklar, refactor eder, birim testi üretir |
| 20 | [Speech ChatBot](NetCoreAINew.Project20_OpenAISpeechChatBot) | OpenAI | Sesli sohbet: kaydet → Whisper ile yaz → ChatGPT → sesli yanıt |

---

## 🛠️ Kullanılan Teknolojiler

- **.NET / C#** — Konsol uygulamaları
- **HttpClient + System.Text.Json** — REST API çağrıları (projelerin çoğu harici paket kullanmadan)
- **NuGet Paketleri:**
  - `PdfPig` — PDF metin çıkarımı (Proje 08)
  - `NAudio` — Mikrofon ses kaydı (Proje 20)
  - `System.Speech` — Metinden sese, TTS (Proje 20)

---

## 🎓 Eğitim Hakkında

Bu koleksiyon, **Udemy** üzerindeki **"AI Masterclass: C# ile 20 Yapay Zeka Entegrasyonu Part 2"** kursunun tamamlanmasıyla ortaya çıkmıştır. Kurs, C# geliştiricilerine modern yapay zeka servislerini gerçek dünya senaryolarıyla entegre etmeyi öğretir.

---

⭐ Faydalı bulduysanız repoya yıldız vermeyi unutmayın!
