using System;

namespace Bastion.Server.Services.Accounts;

// The client works with culture names (en-US, es-MX); the database stores 'en' or 'es-MX'
// (CK_Account_PreferredLanguage).
public static class LanguageCode
{
    public const string English = "en";
    public const string SpanishMexico = "es-MX";

    public static string FromCultureName(string? cultureName)
    {
        bool isEnglish = cultureName is not null
            && cultureName.StartsWith(English, StringComparison.OrdinalIgnoreCase);
        return isEnglish ? English : SpanishMexico;
    }
}
