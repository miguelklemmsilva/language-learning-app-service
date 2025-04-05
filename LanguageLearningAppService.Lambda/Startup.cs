using Amazon.Lambda.Annotations;
using LanguageLearningAppService.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageLearningAppService.Lambda;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCommonServices();
    }
}