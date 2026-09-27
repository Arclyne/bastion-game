using System.Threading.Tasks;
using Bastion.Contracts.Accounts;
using Bastion.Server.Data.Entities;
using Bastion.Server.Services.Accounts;
using Bastion.Server.Services.Security;
using Bastion.Server.Services.Tests.Fakes;
using Xunit;

namespace Bastion.Server.Services.Tests;

public sealed class TestLoginService
{
    private readonly FakeAccountRepository _accounts = new FakeAccountRepository();
    private readonly LoginService _service;

    public TestLoginService()
    {
        var authentication = new AuthenticationService(_accounts, new PasswordHasher(), new FakeCallContext());
        _service = new LoginService(authentication, new FakeSessionService());
    }

    [Fact]
    public async Task LogInAsync_ActiveAccount_ReturnsSessionToken()
    {
        _accounts.Accounts.Add(AccountBuilder.Create(AccountStatus.Active));
        LoginRequest request = CreateRequest(AccountBuilder.Nickname, AccountBuilder.Password);

        LoginOutcome outcome = await _service.LogInAsync(request);

        Assert.Equal(FakeSessionService.IssuedToken, outcome.Result.SessionToken);
    }

    [Fact]
    public async Task LogInAsync_Email_SignsIn()
    {
        _accounts.Accounts.Add(AccountBuilder.Create(AccountStatus.Active));
        LoginRequest request = CreateRequest(AccountBuilder.Email, AccountBuilder.Password);

        LoginOutcome outcome = await _service.LogInAsync(request);

        Assert.Equal(LoginResultCode.Success, outcome.Result.Code);
    }

    [Fact]
    public async Task LogInAsync_UnknownIdentifier_ReturnsInvalidCredentials()
    {
        _accounts.Accounts.Add(AccountBuilder.Create(AccountStatus.Active));

        LoginOutcome outcome = await _service.LogInAsync(CreateRequest("nobody", AccountBuilder.Password));

        Assert.Equal(LoginResultCode.InvalidCredentials, outcome.Result.Code);
    }

    [Fact]
    public async Task LogInAsync_WrongPassword_ReturnsInvalidCredentials()
    {
        _accounts.Accounts.Add(AccountBuilder.Create(AccountStatus.Active));

        LoginOutcome outcome = await _service.LogInAsync(CreateRequest(AccountBuilder.Nickname, "Wrong#Pass1"));

        Assert.Equal(LoginResultCode.InvalidCredentials, outcome.Result.Code);
    }

    [Fact]
    public async Task LogInAsync_WrongPassword_RecordsWrongPassword()
    {
        _accounts.Accounts.Add(AccountBuilder.Create(AccountStatus.Active));

        LoginOutcome outcome = await _service.LogInAsync(CreateRequest(AccountBuilder.Nickname, "Wrong#Pass1"));

        Assert.Equal(AccessLogResult.WrongPassword, outcome.Attempt.Result);
    }

    [Fact]
    public async Task LogInAsync_ThirdWrongPassword_ReturnsAccountLocked()
    {
        Account account = AccountBuilder.Create(AccountStatus.Active);
        account.FailedAttempts = 2;
        _accounts.Accounts.Add(account);

        LoginOutcome outcome = await _service.LogInAsync(CreateRequest(AccountBuilder.Nickname, "Wrong#Pass1"));

        Assert.Equal(LoginResultCode.AccountLocked, outcome.Result.Code);
    }

    [Fact]
    public async Task LogInAsync_PendingAccount_ReturnsAccountPending()
    {
        _accounts.Accounts.Add(AccountBuilder.Create(AccountStatus.Pending));
        LoginRequest request = CreateRequest(AccountBuilder.Nickname, AccountBuilder.Password);

        LoginOutcome outcome = await _service.LogInAsync(request);

        Assert.Equal(LoginResultCode.AccountPending, outcome.Result.Code);
    }

    [Fact]
    public async Task LogInAsync_BannedAccount_ReturnsAccountBanned()
    {
        _accounts.Accounts.Add(AccountBuilder.Create(AccountStatus.Banned));
        LoginRequest request = CreateRequest(AccountBuilder.Nickname, AccountBuilder.Password);

        LoginOutcome outcome = await _service.LogInAsync(request);

        Assert.Equal(LoginResultCode.AccountBanned, outcome.Result.Code);
    }

    [Fact]
    public async Task LogInAsync_EmptyIdentifier_ReturnsInvalidCredentials()
    {
        LoginOutcome outcome = await _service.LogInAsync(CreateRequest(string.Empty, AccountBuilder.Password));

        Assert.Equal(LoginResultCode.InvalidCredentials, outcome.Result.Code);
    }

    private static LoginRequest CreateRequest(string identifier, string password)
    {
        return new LoginRequest { Identifier = identifier, Password = password };
    }
}
