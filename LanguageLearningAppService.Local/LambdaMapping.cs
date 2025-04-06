using System.Reflection;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using LanguageLearningAppService.Lambda;

namespace LanguageLearningAppService.Local;

public static class LambdaMapping
{
    public static void RegisterLambdaAnnotatedMethods(WebApplication app, Functions functionsInstance)
    {
        // Get all public instance methods in the `Functions` class
        var methods = typeof(Functions).GetMethods(BindingFlags.Instance | BindingFlags.Public);

        foreach (var method in methods)
        {
            // Check for [LambdaFunction] and [HttpApi(...)] attributes
            var lambdaFuncAttr = method.GetCustomAttribute<LambdaFunctionAttribute>();
            var httpApiAttr = method.GetCustomAttribute<HttpApiAttribute>();
            if (lambdaFuncAttr == null || httpApiAttr == null)
                continue;

            // Extract the HTTP method + route
            var httpMethod = httpApiAttr.Method; // e.g. LambdaHttpMethod.Get
            var path = httpApiAttr.Template; // e.g. /user

            // Map to ASP.NET Core endpoint
            switch (httpMethod)
            {
                case LambdaHttpMethod.Get:
                    app.MapGet(path, async context =>
                    {
                        var result = await InvokeLambdaMethodAsync(method, functionsInstance, context);
                        await WriteResultAsync(context, result);
                    });
                    break;

                case LambdaHttpMethod.Post:
                    app.MapPost(path, async context =>
                    {
                        var result = await InvokeLambdaMethodAsync(method, functionsInstance, context);
                        await WriteResultAsync(context, result);
                    });
                    break;

                case LambdaHttpMethod.Put:
                    app.MapPut(path, async context =>
                    {
                        var result = await InvokeLambdaMethodAsync(method, functionsInstance, context);
                        await WriteResultAsync(context, result);
                    });
                    break;

                case LambdaHttpMethod.Delete:
                    app.MapDelete(path, async context =>
                    {
                        var result = await InvokeLambdaMethodAsync(method, functionsInstance, context);
                        await WriteResultAsync(context, result);
                    });
                    break;
            }
        }
    }

    private static async Task<APIGatewayHttpApiV2ProxyResponse?> InvokeLambdaMethodAsync(MethodInfo method,
        Functions instance, HttpContext context)
    {
        // 1. Build up the parameter list based on method signature 
        //    and custom attributes ([FromHeader], [FromBody], etc.).

        var parameters = method.GetParameters();
        var argumentValues = new object?[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var paramInfo = parameters[i];

            // Check for [FromHeader]
            var fromHeaderAttr =
                paramInfo.GetCustomAttribute<FromHeaderAttribute>();
            if (fromHeaderAttr != null)
            {
                string? headerName = fromHeaderAttr.Name ?? paramInfo.Name;
                // if you wanted to handle the case `[FromHeader] string authorization`, for example:
                // Or just default to paramInfo.Name if fromHeaderAttr.Name is null
                if (headerName != null) argumentValues[i] = context.Request.Headers[headerName].ToString();
                continue;
            }

            // Check for [FromQuery]
            var fromQueryAttr = paramInfo.GetCustomAttribute<FromQueryAttribute>();
            if (fromQueryAttr != null)
            {
                string? queryName = fromQueryAttr.Name ?? paramInfo.Name;
                if (queryName == null) throw new ArgumentNullException(nameof(queryName));
                argumentValues[i] = context.Request.Query[queryName].ToString();
                continue;
            }

            // Check for [FromBody]
            var fromBodyAttr = paramInfo.GetCustomAttribute<FromBodyAttribute>();
            if (fromBodyAttr != null)
            {
                // read the JSON body and deserialize to paramInfo.ParameterType
                using var reader = new StreamReader(context.Request.Body);
                var bodyString = await reader.ReadToEndAsync();

                // You can use System.Text.Json or Newtonsoft.Json here:
                var deserialized = System.Text.Json.JsonSerializer.Deserialize(bodyString, paramInfo.ParameterType);
                argumentValues[i] = deserialized;
                continue;
            }

            // If nothing else matched, you could pass null or some default. 
            // Or handle custom logic. For example, the "authorization" header 
            // might not be annotated with [FromHeader].
            argumentValues[i] = null;
        }

        // 2. Invoke the method
        var result = method.Invoke(instance, argumentValues);

        // If the method returns a Task (async), await it to get the actual result
        if (result is Task taskResult)
        {
            await taskResult.ConfigureAwait(false);

            // Check if it's a generic Task<T>
            if (taskResult.GetType().IsGenericType)
            {
                return (APIGatewayHttpApiV2ProxyResponse?)taskResult.GetType().GetProperty("Result")!.GetValue(taskResult);
            }

            return null;
        }

        return (APIGatewayHttpApiV2ProxyResponse?)result;
    }

    private static async Task WriteResultAsync(HttpContext context, APIGatewayHttpApiV2ProxyResponse? result)
    {
        // If your AWS Lambda method uses `APIGatewayHttpApiV2ProxyResponse` or returns something else,
        // you can adapt accordingly. Here’s a simple approach:

        if (result == null)
        {
            context.Response.StatusCode = 204; // No Content
            return;
        }

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result.Body);
    }
}