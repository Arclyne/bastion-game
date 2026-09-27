using System.Threading.Tasks;
using Bastion.Client.Localization;
using Bastion.Client.Screens.Login;
using Bastion.Client.Session;
using Bastion.Client.Tests.Fakes;
using Bastion.Contracts.Accounts;
using Xunit;

namespace Bastion.Client.Tests;

public sealed class TestLoginController
{
    private readonly FakeAccountClient _accounts = new FakeAccountClient();
    private readonly SessionContext _session = new SessionContext();
    private readonly LoginController _controller;

    public TestLoginController()
    {
        Language.Apply(Language.English);
        _controller = new LoginController(_accounts, _session);
    }

    [Fact]
    public void Validate_EmptyIdentifier_ReturnsRequiredWarning()
    {
        string identifier = "   ";

        LoginWarnings warnings = LoginController.Validate(identifier, "secret");

        Assert.Equal(TextCatalog.LoginIdentifierRequired, warnings.Identifier);
    }

    [Fact]
    public void Validate_EmptyPassword_ReturnsRequiredWarning()
    {
        string password = string.Empty;

        LoginWarnings warnings = LoginController.Validate("luis_quo", password);

        Assert.Equal(TextCatalog.LoginPasswordRequired, warnings.Password);
    }

    [Fact]
    public void Validate_BothFieldsFilled_HasNoWarnings()
    {
        string identifier = "luis_quo";

        LoginWarnings warnings = LoginController.Validate(identifier, "secret");

        Assert.False(warnings.HasAny);
    }

    [Fact]
    public void GetMessage_InvalidCredentials_ReturnsTheGenericMessage()
    {
        var result = new LoginResult { Code = LoginResultCode.InvalidCredentials };

        string message = LoginController.GetMessage(result);

        Assert.Equal(TextCatalog.LoginCredentialsRejected, message);
    }

    [Fact]
    public void GetMessage_ServiceUnavailable_ReturnsTheServerMessage()
    {
        var result = new LoginResult { Code = LoginResultCode.ServiceUnavailable };

        string message = LoginController.GetMessage(result);

        Assert.Equal(TextCatalog.CommonServerUnreachable, message);
    }

    [Fact]
    public async Task LogInAsync_Success_StartsTheSession()
    {
        _accounts.LoginResult = new LoginResult
        {
            Code = LoginResultCode.Success,
            SessionToken = "token",
            Nickname = "luis_quo",
        };

        await _controller.LogInAsync("luis_quo", "Luis#Quo2026");

        Assert.True(_session.IsSignedIn);
    }

    [Fact]
    public async Task LogInAsync_InvalidCredentials_KeepsTheSessionClosed()
    {
        _accounts.LoginResult = new LoginResult { Code = LoginResultCode.InvalidCredentials };

        await _controller.LogInAsync("luis_quo", "wrong");

        Assert.False(_session.IsSignedIn);
    }
}
