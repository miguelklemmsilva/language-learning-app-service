using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Azure;
using Azure.AI.Translation.Text;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.ApiModels;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Core.Services;

public class ChatGptService(HttpClient httpClient) : IChatGptService
{
    public async Task<VerifySentenceResponse> VerifySentenceAsync(VerifySentenceRequest request)
    {
        var requestBody = new ChatGptRequest
        {
            Model = "gpt-4o-mini",
            Messages =
            [
                new ChatGptMessage
                {
                    Role = "system",
                    Content =
                        "Evaluate whether a translation is correct or not. If not, explain why to the user. If it is correct return null."
                },
                new ChatGptMessage
                {
                    Role  = "user",
                    Content = $"English to {request.Language}"
                },
                new ChatGptMessage
                {
                    Role = "user",
                    Content = $"Original sentence: {request.Original}"
                },
                new ChatGptMessage
                {
                    Role = "user",
                    Content = $"Translation: {request.Translation}"
                }
            ],
            Temperature = 0.2,
            ResponseFormat = new ResponseFormat
            {
                Type = "json_schema",
                JsonSchema = new JsonSchema
                {
                    Schema = new Schema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, JsonSchemaProperty>
                        {
                            ["isCorrect"] = new() { Type = "boolean" },
                            ["explanation"] = new() { Type = "string" }
                        },
                        Required = ["isCorrect", "explanation"]
                    }
                }
            }
        };

        var requestJson = JsonSerializer.Serialize(requestBody, CustomJsonSerializerContext.Default.ChatGptRequest);

        Console.WriteLine(requestJson);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions")
        {
            Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(httpRequest);
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        var jsonResponse =
            JsonSerializer.Deserialize(responseBody, CustomJsonSerializerContext.Default.ChatGptResponse);
        
        var messageResponse = JsonSerializer.Deserialize(jsonResponse!.Choices.First().Message.Content,
            CustomJsonSerializerContext.Default.VerifySentenceResponse);
        
        return messageResponse ?? new VerifySentenceResponse { IsCorrect = false, Explanation = "No response from AI" };
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

        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        var jsonResponse =
            JsonSerializer.Deserialize(responseBody, CustomJsonSerializerContext.Default.ChatGptResponse);

        return jsonResponse!.Choices.First().Message.Content;
    }

    private string LocalizedSystemPrompt(string language, string region)
    {
        return language switch
        {
            "Spanish" =>
                $"Eres un asistente útil que genera frases de ejemplo. Cuando se te proporcione una palabra, escribe una frase corta de ejemplo usando esa palabra. Utiliza español de {(region == "Spain" ? "España" : "América Latina")}.",
            "Portuguese" =>
                $"Você é um assistente útil que gera frases de exemplo. Quando uma palavra for fornecida, escreva uma frase curta de exemplo usando essa palavra. Use português de {region switch { "Brazil" => "Brasil", "Portugal" => "Portugal", _ => "Portugal" }}.",
            "Japanese" => "あなたは例文を生成する役立つアシスタントです。単語が提供されたら、その単語を使用して10語以下の短い文を書いてください。",
            "German" =>
                "Sie sind ein hilfreicher Assistent, der Beispielsätze generiert. Wenn Ihnen ein Wort gegeben wird, schreiben Sie einen kurzen Beispielsatz mit diesem Wort.",
            "Italian" =>
                "Sei un assistente utile che genera frasi di esempio. Quando ti viene fornita una parola, scrivi una breve frase di esempio usando quella parola.",
            "French" =>
                $"Vous êtes un assistant utile qui génère des phrases d'exemple. Lorsqu'un mot vous est fourni, écrivez une courte phrase d'exemple en utilisant ce mot. Utilisez le français de {region switch { "France" => "France", "Canada" => "Canada", _ => "France" }}.",
            _ => throw new ArgumentException($"Unsupported language: {language}")
        };
    }
}

public class TranslationService(TextTranslationClient textTranslationClient) : ITranslationService
{
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

        var options = new TextTranslationTranslateOptions(targetLanguages: ["en"], content: [sentence])
        {
            SourceLanguage = sourceLanguageCode,
            ProfanityAction = ProfanityAction.NoAction,
            IncludeAlignment = true
        };

        Response<IReadOnlyList<TranslatedTextItem>> response = await textTranslationClient.TranslateAsync(options);
        IReadOnlyList<TranslatedTextItem?> translations = response.Value;
        var translation = translations.FirstOrDefault();

        return translation;
    }
}

public class TokenService(HttpClient httpClient) : ITokenService
{
    public async Task<string> GetIssueTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/sts/v1.0/issueToken");
        var response = await httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }
}