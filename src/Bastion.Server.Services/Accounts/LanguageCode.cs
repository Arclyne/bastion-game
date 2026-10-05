using System;

namespace Bastion.Server.Services.Accounts;

public static class LanguageCode
{
    public const string English = "en";
    public const string SpanishMexico = "es-MX";

    private const string SpanishPrefix = "es";

    public static string FromCultureName(string? cultureName)
    {
        bool isSpanish = cultureName is not null
            && cultureName.StartsWith(SpanishPrefix, StringComparison.OrdinalIgnoreCase);
        return isSpanish ? SpanishMexico : English;
    }
}
