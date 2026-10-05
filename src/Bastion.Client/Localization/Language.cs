using System.Globalization;

namespace Bastion.Client.Localization;

public static class Language
{
    private const string EnglishCode = "en-US";
    private const string SpanishMexicoCode = "es-MX";
    private const string EnglishTwoLetterCode = "en";

    public static readonly CultureInfo SpanishMexico = new CultureInfo(SpanishMexicoCode);

    public static readonly CultureInfo English = new CultureInfo(EnglishCode);

    public static CultureInfo Default => English;

    public static CultureInfo Current => CultureInfo.CurrentUICulture;

    public static bool IsEnglish => Current.TwoLetterISOLanguageName == EnglishTwoLetterCode;

    public static void Apply(CultureInfo culture)
    {
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
    }
}
