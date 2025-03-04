using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Infrastructure.Repositories;
using Core.Interfaces;
using Core.Models.DataModels;
using Core.Services;

namespace Lambda.PreSignUp;

public class Function
{
    private static readonly AmazonDynamoDBClient DynamoDbClient = new(RegionEndpoint.EUWest2);

    private static async Task Main()
    {
        Func<JsonObject, ILambdaContext, Task<JsonObject>> handler = FunctionHandler;
        await LambdaBootstrapBuilder.Create(handler,
                new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
            .Build()
            .RunAsync();
    }

    public static async Task<JsonObject> FunctionHandler(JsonObject input, ILambdaContext context)
    {
        var email = input["request"]!["userAttributes"]!["email"]!.ToString().ToLower();
        var sub = input["userName"]!.ToString();


        return input;
    }
}

[JsonSerializable(typeof(JsonObject))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
    // By using this partial class derived from JsonSerializerContext, we can generate reflection free JSON Serializer code at compile time
    // which can deserialize our class and properties. However, we must attribute this class to tell it what types to generate serialization code for.
    // See https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation
}