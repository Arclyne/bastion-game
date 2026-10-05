using System;

namespace Bastion.Server.Services.Accounts;

// The client works with culture names (en-US, es-MX); the database stores 'en' or 'es-MX'
// (CK_Account_PreferredLanguage). English is the default language, so any culture that is not Spanish maps to it.
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
