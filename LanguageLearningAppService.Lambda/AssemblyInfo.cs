using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using LanguageLearningAppService.Lambda;

[assembly: LambdaGlobalProperties(GenerateMain = true)]
[assembly: LambdaSerializer(typeof(SourceGeneratorLambdaJsonSerializer<CustomJsonSerializerContext>))]