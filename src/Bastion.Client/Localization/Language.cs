using System.Globalization;

namespace Bastion.Client.Localization;

// Exactly two languages are supported (D-21). The database enforces the same pair through
// CK_Account_PreferredLanguage, so adding a third one also changes that check.
public static class Language
{
    private const string EnglishCode = "en-US";
    private const string SpanishMexicoCode = "es-MX";
    private const string EnglishTwoLetterCode = "en";

    public static readonly CultureInfo SpanishMexico = new CultureInfo(SpanishMexicoCode);

    public static readonly CultureInfo English = new CultureInfo(EnglishCode);

    // D-21 makes es-MX the language the game starts in; en-US stays the neutral resource.
    public static CultureInfo Default => SpanishMexico;

    public static CultureInfo Current => CultureInfo.CurrentUICulture;

    // Compared by two-letter code because "en" and "en-US" are different cultures and both count as English.
    public static bool IsEnglish => Current.TwoLetterISOLanguageName == EnglishTwoLetterCode;

    public static void Apply(CultureInfo culture)
    {
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
    }
}
