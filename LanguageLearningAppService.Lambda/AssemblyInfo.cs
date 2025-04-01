using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Core.Models.DataModels;
using CustomJsonSerializerContext = LanguageLearningAppService.Lambda.CustomJsonSerializerContext;

[assembly: LambdaGlobalProperties(GenerateMain = true)]
[assembly: LambdaSerializer(typeof(CamelCaseLambdaJsonSerializer))]
[assembly: LambdaSerializer(typeof(SourceGeneratorLambdaJsonSerializer<CustomJsonSerializerContext>))]