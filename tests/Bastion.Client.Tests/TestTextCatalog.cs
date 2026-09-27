using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using Bastion.Client.Localization;
using Xunit;

namespace Bastion.Client.Tests;

public sealed class TestTextCatalog
{
    private const string ResourceBaseName = "Bastion.Client.Localization.Strings";

    // A property whose key is missing returns the key itself, such as "Login.Subtitle".
    private static readonly Regex _keyPattern = new Regex(@"^[A-Z][A-Za-z]*\.[A-Za-z_]+$");

    private static readonly ResourceManager _manager = new ResourceManager(
        ResourceBaseName,
        typeof(TextCatalog).Assembly);

    [Fact]
    public void Strings_SpanishCatalog_HasTheSameKeysAsTheNeutralCatalog()
    {
        IReadOnlyCollection<string> neutralKeys = ReadKeys(CultureInfo.InvariantCulture);

        IReadOnlyCollection<string> spanishKeys = ReadKeys(Language.SpanishMexico);

        Assert.Equal(neutralKeys, spanishKeys);
    }

    [Fact]
    public void Strings_EveryKey_FollowsScreenElementFormat()
    {
        IReadOnlyCollection<string> keys = ReadKeys(CultureInfo.InvariantCulture);

        var malformed = keys.Where(key => key.Split('.').Length != 2).ToList();

        Assert.Empty(malformed);
    }

    [Fact]
    public void Strings_SpanishCatalog_HasNoEmptyValues()
    {
        ResourceSet resources = ReadSet(Language.SpanishMexico);

        var emptyKeys = resources.Cast<DictionaryEntry>()
            .Where(entry => string.IsNullOrWhiteSpace(entry.Value as string))
            .Select(entry => (string)entry.Key)
            .ToList();

        Assert.Empty(emptyKeys);
    }

    [Fact]
    public void LoginSubtitle_SpanishCulture_ReturnsSpanishText()
    {
        Language.Apply(Language.SpanishMexico);

        string subtitle = TextCatalog.LoginSubtitle;

        Assert.Equal("ACCEDE A TU CUENTA", subtitle);
    }

    [Fact]
    public void LoginSubtitle_EnglishCulture_ReturnsEnglishText()
    {
        Language.Apply(Language.English);

        string subtitle = TextCatalog.LoginSubtitle;

        Assert.Equal("SIGN IN TO YOUR ACCOUNT", subtitle);
    }

    [Fact]
    public void Properties_EveryProperty_ReturnsTextInsteadOfItsKey()
    {
        Language.Apply(Language.English);

        var missing = typeof(TextCatalog).GetProperties()
            .Select(property => (string)(property.GetValue(null) ?? string.Empty))
            .Where(text => _keyPattern.IsMatch(text))
            .ToList();

        Assert.Empty(missing);
    }

    private static IReadOnlyCollection<string> ReadKeys(CultureInfo culture)
    {
        return ReadSet(culture)
            .Cast<DictionaryEntry>()
            .Select(entry => (string)entry.Key)
            .OrderBy(key => key, StringComparer.Ordinal)
            .ToList();
    }

    private static ResourceSet ReadSet(CultureInfo culture)
    {
        return _manager.GetResourceSet(culture, true, false)
            ?? throw new MissingManifestResourceException($"Missing resources. Culture={culture.Name}");
    }
}
