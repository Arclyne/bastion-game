using Bastion.Client.Localization;
using Xunit;

namespace Bastion.Client.Tests;

public sealed class TestLanguage
{
    [Fact]
    public void Default_GameStart_IsEnglish()
    {
        string expected = "en-US";

        string cultureName = Language.Default.Name;

        Assert.Equal(expected, cultureName);
    }
}
