using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Core;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.DataModels;

namespace LanguageLearningAppService.Infrastructure.Repositories;

public class AllowedVocabularyRepository(IDynamoDBContext context) : IAllowedVocabularyRepository
{
    public async Task<bool> IsVocabularyAllowedAsync(Language language, string word)
    {
        var allowedVocabulary = await context.LoadAsync<AllowedVocabulary>(word, language.ToString());
        return allowedVocabulary != null;
    }

    public async Task<IEnumerable<AllowedVocabulary>> GetWordsByCategoryAsync(Language language)
    {
        // Set up the DynamoDBOperationConfig to use the secondary index.
        var config = new QueryConfig
        {
            IndexName = "CategoryIndex"
        };

        // Query the GSI where the partition key is "Language". 
        var query = context.QueryAsync<AllowedVocabulary>(language.ToString(), config);
        return await query.GetRemainingAsync();
    }
}