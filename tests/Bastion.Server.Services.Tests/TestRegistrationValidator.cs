using System;
using System.Threading.Tasks;
using Bastion.Contracts.Accounts;
using Bastion.Server.Data.Entities;
using Bastion.Server.Services.Accounts;
using Bastion.Server.Services.Tests.Fakes;
using Xunit;

namespace Bastion.Server.Services.Tests;

public sealed class TestRegistrationValidator
{
    private readonly FakeAccountRepository _accounts = new FakeAccountRepository();
    private readonly FakeCallContext _callContext = new FakeCallContext();
    private readonly RegistrationValidator _validator;

    public TestRegistrationValidator()
    {
        _validator = new RegistrationValidator(_accounts, _callContext);
    }

    [Fact]
    public async Task FindProblemAsync_ValidRequest_ReturnsNull()
    {
        RegistrationRequest request = CreateValidRequest();

        RegistrationResultCode? problem = await _validator.FindProblemAsync(request);

        Assert.Null(problem);
    }

    [Fact]
    public async Task FindProblemAsync_ShortNickname_ReturnsInvalidData()
    {
        RegistrationRequest request = CreateValidRequest();
        request.Nickname = "ab";

        RegistrationResultCode? problem = await _validator.FindProblemAsync(request);

        Assert.Equal(RegistrationResultCode.InvalidData, problem);
    }

    [Fact]
    public async Task FindProblemAsync_MalformedEmail_ReturnsInvalidData()
    {
        RegistrationRequest request = CreateValidRequest();
        request.Email = "nora@mail";

        RegistrationResultCode? problem = await _validator.FindProblemAsync(request);

        Assert.Equal(RegistrationResultCode.InvalidData, problem);
    }

    [Fact]
    public async Task FindProblemAsync_TwoPasswordGroups_ReturnsInvalidData()
    {
        RegistrationRequest request = CreateValidRequest();
        request.Password = "onlylowercase1";

        RegistrationResultCode? problem = await _validator.FindProblemAsync(request);

        Assert.Equal(RegistrationResultCode.InvalidData, problem);
    }

    [Fact]
    public async Task FindProblemAsync_SevenYearsOld_ReturnsUnderage()
    {
        RegistrationRequest request = CreateValidRequest();
        request.BirthDate = _callContext.UtcNow.AddYears(-7);

        RegistrationResultCode? problem = await _validator.FindProblemAsync(request);

        Assert.Equal(RegistrationResultCode.Underage, problem);
    }

    [Fact]
    public async Task FindProblemAsync_TermsNotAccepted_ReturnsTermsNotAccepted()
    {
        RegistrationRequest request = CreateValidRequest();
        request.HasAcceptedTerms = false;

        RegistrationResultCode? problem = await _validator.FindProblemAsync(request);

        Assert.Equal(RegistrationResultCode.TermsNotAccepted, problem);
    }

    [Fact]
    public async Task FindProblemAsync_TakenNickname_ReturnsNicknameTaken()
    {
        _accounts.Accounts.Add(new Account { Nickname = "nora_north", Email = "other@example.test" });

        RegistrationResultCode? problem = await _validator.FindProblemAsync(CreateValidRequest());

        Assert.Equal(RegistrationResultCode.NicknameTaken, problem);
    }

    [Fact]
    public async Task FindProblemAsync_TakenEmailInOtherCase_ReturnsEmailTaken()
    {
        _accounts.Accounts.Add(new Account { Nickname = "someone", Email = "nora@example.test" });
        RegistrationRequest request = CreateValidRequest();
        request.Email = "Nora@Example.test";

        RegistrationResultCode? problem = await _validator.FindProblemAsync(request);

        Assert.Equal(RegistrationResultCode.EmailTaken, problem);
    }

    private RegistrationRequest CreateValidRequest()
    {
        return new RegistrationRequest
        {
            Nickname = "nora_north",
            Email = "nora@example.test",
            Password = "Nora#North26",
            BirthDate = new DateTime(2008, 3, 15),
            PreferredLanguage = "es-MX",
            HasAcceptedTerms = true,
        };
    }
}
