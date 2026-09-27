using Bastion.Server.Services.Accounts;
using Xunit;

namespace Bastion.Server.Services.Tests;

public sealed class TestLanguageCode
{
    [Fact]
    public void FromCultureName_SpanishMexico_ReturnsSpanishMexico()
    {
        string cultureName = "es-MX";

        string code = LanguageCode.FromCultureName(cultureName);

        Assert.Equal(LanguageCode.SpanishMexico, code);
    }

    [Fact]
    public void FromCultureName_EnglishUnitedStates_ReturnsEnglish()
    {
        string cultureName = "en-US";

        string code = LanguageCode.FromCultureName(cultureName);

        Assert.Equal(LanguageCode.English, code);
    }

    [Fact]
    public void FromCultureName_MissingCulture_ReturnsTheDefaultEnglish()
    {
        string? cultureName = null;

        string code = LanguageCode.FromCultureName(cultureName);

        Assert.Equal(LanguageCode.English, code);
    }
}
