using System.Reflection;
using Azure.AI.Translation.Text;
using Core.Interfaces;

namespace LanguageLearningAppService.Tests.TestRepositories;

internal class TestTranslationRepository : ITranslationRepository
{
    public Task<TranslatedTextItem?> TranslateSentenceAsync(string sentence, string sourceLanguageCode)
    {
        // Create an instance of TranslationText using its non-public constructor.
        var translationText = (TranslationText)Activator.CreateInstance(typeof(TranslationText), nonPublic: true)!;
        SetBackingField(translationText, "Text", sentence + " translated");

        // Create an instance of TranslatedTextAlignment and set its Projections as a string.
        var alignment =
            (TranslatedTextAlignment)Activator.CreateInstance(typeof(TranslatedTextAlignment), nonPublic: true)!;
        // Instead of assigning a List<string>, assign a string value.
        SetBackingField(alignment, "Projections", "dummy-alignment");
        SetBackingField(translationText, "Alignment", alignment);

        // Create a list of translations.
        var translations = new List<TranslationText> { translationText };

        // Create an instance of TranslatedTextItem and set its Translations property.
        var translatedTextItem =
            (TranslatedTextItem)Activator.CreateInstance(typeof(TranslatedTextItem), nonPublic: true)!;
        SetBackingField(translatedTextItem, "Translations", translations);

        return Task.FromResult<TranslatedTextItem?>(translatedTextItem);
    }

    private void SetBackingField<T>(object obj, string propertyName, T value)
    {
        // Backing field name for auto-properties is usually in the form: <PropertyName>k__BackingField
        var fieldName = $"<{propertyName}>k__BackingField";
        var field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new Exception($"Backing field '{fieldName}' not found on {obj.GetType().FullName}.");
        }

        field.SetValue(obj, value);
    }
}