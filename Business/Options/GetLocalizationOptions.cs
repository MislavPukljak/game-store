using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Business.Options;

public class GetLocalizationOptions
{
    private readonly IConfiguration _configuration;

    public GetLocalizationOptions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public RequestLocalizationOptions LocalizationOptions()
    {
        var cultures = _configuration.GetSection("Localization").GetSection("SupportedCultures")
            .GetChildren().ToDictionary(x => x.Key, x => x.Value);

        var supportedCultures = cultures.Keys.ToArray();

        var localizationOptions = new RequestLocalizationOptions()
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        return localizationOptions;
    }
}
