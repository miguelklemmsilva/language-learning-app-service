using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.DataModels;
using Table = Core.Helpers.Table;

namespace LanguageLearningAppService.Infrastructure.Repositories;

public class VocabularyRepository(IDynamoDBContext context) : IVocabularyRepository
{
    public async Task<Vocabulary> UpdateVocabularyAsync(Vocabulary vocabulary)
    {
        await context.SaveAsync(vocabulary);

        return await GetVocabularyAsync(vocabulary.UserId, vocabulary.Language, vocabulary.Word);
    }

    public async Task<Vocabulary> GetVocabularyAsync(string userId, Language language, string word)
    {
        // DynamoDBContext will automatically use the composite key.
        return await context.LoadAsync<Vocabulary>(userId, $"{language}#{word}");
    }

    public async Task<IEnumerable<Vocabulary>> GetUserVocabularyAsync(string userId, Language language)
    {
        // Our composite key is "Language#Word", so to retrieve all items for a language,
        // we need to filter where the range key begins with "Language#".
        var prefix = $"{language}#";
        
        // QueryAsync<T> has an overload that accepts a QueryOperator.
        // Here, we use QueryOperator.BeginsWith to restrict the range key.
        var query = context.QueryAsync<Vocabulary>(
            hashKeyValue: userId,
            op: QueryOperator.BeginsWith,
            values: [prefix]
        );

        return await query.GetRemainingAsync();
    }

    public async Task RemoveVocabularyAsync(string userId, Language language, string word)
    {
        await context.DeleteAsync<Vocabulary>(userId, $"{language}#{word}");
    }
}