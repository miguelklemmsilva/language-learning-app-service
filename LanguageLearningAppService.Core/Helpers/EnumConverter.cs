using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Core.Models.DataModels;

namespace Core.Helpers;

public class EnumConverter<T> : IPropertyConverter
{
    public DynamoDBEntry ToEntry(object? value)
    {
        if (value == null)
        {
            return new DynamoDBNull();
        }
        return new Primitive(value.ToString());
    }

    public object? FromEntry(DynamoDBEntry entry)
    {
        return entry switch
        {
            DynamoDBNull => null,
            Primitive primitive when Enum.TryParse(typeof(T), primitive.Value.ToString(), out var result) =>
                result,
            _ => throw new InvalidOperationException("Unable to convert DynamoDB entry to enum.")
        };
    }
}