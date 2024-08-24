using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using AWS.Repositories;
using Core.Models.DataModels;

namespace Lambda.PostSignUp;

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
        // line by line debugging
        Console.WriteLine("PostSignUp Lambda function called");
        Console.WriteLine("Input: " + input.ToString());
        Console.WriteLine("request: " + input["request"].ToString());
        Console.WriteLine("userAttributes: " + input["request"]["userAttributes"].ToString());
        Console.WriteLine("email: " + input["request"]["userAttributes"]["email"].ToString());
        Console.WriteLine("sub: " + input["request"]["userAttributes"]["sub"].ToString());
        
        var email = input["request"]!["userAttributes"]!["email"]!.ToString().ToLower();
        var sub = input["request"]!["userAttributes"]!["sub"]!.ToString().ToLower();

        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email not provided in the request.");

        var userRepository = new UserRepository(DynamoDbClient);

        await userRepository.CreateUserAsync(new User
        {
            UserId = sub,
            Email = email
        });

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