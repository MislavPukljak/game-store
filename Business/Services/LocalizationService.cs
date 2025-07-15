using System.Globalization;
using System.Reflection;
using Business.Interfaces;
using Microsoft.Extensions.Localization;

namespace Business.Services;

public class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer _localizer;

    public LocalizationService(string culture, IStringLocalizerFactory localizer)
    {
        var type = typeof(LocalizationService);
#pragma warning disable CS8604 // Possible null reference argument.
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
        _localizer = localizer.Create("Resource", assemblyName.Name);
#pragma warning restore CS8604 // Possible null reference argument.
        CultureInfo.CurrentCulture = new CultureInfo(culture);
        CultureInfo.CurrentUICulture = new CultureInfo(culture);
    }

    public string Localize(string key)
    {
        return _localizer[key];
    }
}
