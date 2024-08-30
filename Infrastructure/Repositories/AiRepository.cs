using System.Text;
using System.Text.Json;
using Azure;
using Azure.AI.Translation.Text;
using Core.Interfaces;
using Core.Models.ApiModels;
using Core.Models.DataModels;

namespace Infrastructure.Repositories;

public class AiRepository(HttpClient httpClient, TextTranslationClient textTranslationClient) : IAiRepository
{
    private string LocalizedSystemPrompt(string language, string region)
    {
        return language switch
        {
            "Spanish" => $"Eres un asistente útil que genera frases de ejemplo. Cuando se te proporcione una palabra, escribe una frase corta de ejemplo usando esa palabra. Utiliza español de {(region == "Spain" ? "España" : "América Latina")}.",
            "Portuguese" => $"Você é um assistente útil que gera frases de exemplo. Quando uma palavra for fornecida, escreva uma frase curta de exemplo usando essa palavra. Use português de {region switch
            {
                "Brazil" => "Brasil",
                "Portugal" => "Portugal",
                _ => "Portugal"
            }}.",
            "Japanese" => "あなたは例文を生成する役立つアシスタントです。単語が提供されたら、その単語を使用して10語以下の短い文を書いてください。",
            "German" => "Sie sind ein hilfreicher Assistent, der Beispielsätze generiert. Wenn Ihnen ein Wort gegeben wird, schreiben Sie einen kurzen Beispielsatz mit diesem Wort.",
            "Italian" => "Sei un assistente utile che genera frasi di esempio. Quando ti viene fornita una parola, scrivi una breve frase di esempio usando quella parola.",
            "French" => $"Vous êtes un assistant utile qui génère des phrases d'exemple. Lorsqu'un mot vous est fourni, écrivez une courte phrase d'exemple en utilisant ce mot. Utilisez le français de {region switch
            {
                "France" => "France",
                "Canada" => "Canada",
                _ => "France"
            }}.",
            _ => throw new ArgumentException($"Unsupported language: {language}")
        };
    }
    
    public async Task<string> GenerateSentenceAsync(string word, string language, string country)
    {
        var systemPrompt = LocalizedSystemPrompt(language, country);
        
        var requestBody = new ChatGptRequest
        {
            Model = "gpt-4o-mini",
            Messages =
            [
                new ChatGptMessage { Role = "system", Content = systemPrompt },
                new ChatGptMessage { Role = "user", Content = word }
            ],
            Temperature = 0.8
        };
        
        var requestJson = JsonSerializer.Serialize(requestBody, CustomJsonSerializerContext.Default.ChatGptRequest);
        
        var request = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions")
        {
            Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
        };
        
        var response = await httpClient.SendAsync(request);
        Console.WriteLine($"content: {await response.Content.ReadAsStringAsync()}");
        
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        
        var jsonResponse = JsonSerializer.Deserialize(responseBody, CustomJsonSerializerContext.Default.ChatGptResponse);
        
        return jsonResponse?.Choices[0].Message.Content ?? string.Empty;
    }
    
    public async Task<TranslatedTextItem?> TranslateSentenceAsync(string sentence, string sourceLanguage)
    {
        var sourceLanguageCode = sourceLanguage switch
        {
            "Spanish" => "es",
            "Portuguese" => "pt",
            "Japanese" => "ja",
            "German" => "de",
            "Italian" => "it",
            "French" => "fr",
            _ => throw new ArgumentException($"Unsupported language: {sourceLanguage}")
        };

        Response<IReadOnlyList<TranslatedTextItem>> response =
            await textTranslationClient.TranslateAsync("en", sentence, sourceLanguageCode);
        IReadOnlyList<TranslatedTextItem?> translations = response.Value;
        var translation = translations.FirstOrDefault();
        
        Console.WriteLine($"Text was translated to: '{translation?.Translations?.FirstOrDefault()?.TargetLanguage}' and the result is: '{translation?.Translations?.FirstOrDefault()?.Text}'.");

        return translation;
    }
}