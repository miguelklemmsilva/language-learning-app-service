using System.Text;
using System.Text.Json;
using Azure;
using Azure.AI.Translation.Text;
using Core.Interfaces;
using Core.Models.ApiModels;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Services;

public class ChatGptService(IChatGptRepository chatGptRepository) : IChatGptService
{
    public async Task<VerifySentenceResponse> VerifySentenceAsync(VerifySentenceRequest request)
    {
        var chatGptRequest = new ChatGptRequest
        {
            Model = "gpt-4o-mini",
            Messages =
            [
                new ChatGptMessage
                {
                    Role = "system",
                    Content =
                        "Evaluate whether a translation is correct or not, don't be too harsh. If not, explain why to the user. If it is correct return null."
                },
                new ChatGptMessage
                {
                    Role = "user",
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

        var chatGptResponse = await chatGptRepository.SendChatGptRequestAsync(chatGptRequest);

        var messageResponse = JsonSerializer.Deserialize(chatGptResponse.Choices.First().Message.Content,
            CustomJsonSerializerContext.Default.VerifySentenceResponse);

        return messageResponse ?? new VerifySentenceResponse { IsCorrect = false, Explanation = "No response from AI" };
    }

    public async Task<string> GenerateSentenceAsync(string word, string language, string country)
    {
        string systemPrompt = LocalizedSystemPrompt(language, country);

        var chatGptRequest = new ChatGptRequest
        {
            Model = "gpt-4o-mini",
            Messages =
            [
                new ChatGptMessage { Role = "system", Content = systemPrompt },
                new ChatGptMessage { Role = "user", Content = word }
            ],
            Temperature = 0.8
        };

        var chatGptResponse = await chatGptRepository.SendChatGptRequestAsync(chatGptRequest);
        return chatGptResponse.Choices.First().Message.Content;
    }

    private static string LocalizedSystemPrompt(string language, string region)
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

public class TranslationService(ITranslationRepository translationRepository) : ITranslationService
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
        
        return await translationRepository.TranslateSentenceAsync(sentence, sourceLanguageCode);
    }
}