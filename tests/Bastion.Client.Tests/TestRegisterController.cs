using System;
using System.Threading.Tasks;
using Bastion.Client.Localization;
using Bastion.Client.Screens.Register;
using Bastion.Client.Tests.Fakes;
using Bastion.Contracts.Accounts;
using Xunit;

namespace Bastion.Client.Tests;

public sealed class TestRegisterController
{
    private readonly FakeAccountClient _accounts = new FakeAccountClient();
    private readonly RegisterController _controller;

    public TestRegisterController()
    {
        Language.Apply(Language.English);
        _controller = new RegisterController(_accounts);
    }

    [Fact]
    public void Validate_ValidForm_HasNoWarnings()
    {
        RegistrationForm form = CreateValidForm();

        RegistrationWarnings warnings = RegisterController.Validate(form);

        Assert.False(warnings.HasAny);
    }

    [Fact]
    public void Validate_ShortNickname_WarnsAboutLength()
    {
        RegistrationForm form = CreateValidForm();
        form.Nickname = "ab";

        RegistrationWarnings warnings = RegisterController.Validate(form);

        Assert.Equal(TextCatalog.RegisterNicknameLength, warnings.Nickname);
    }

    [Fact]
    public void Validate_NicknameWithSpaces_WarnsAboutCharacters()
    {
        RegistrationForm form = CreateValidForm();
        form.Nickname = "nora north";

        RegistrationWarnings warnings = RegisterController.Validate(form);

        Assert.Equal(TextCatalog.RegisterNicknameInvalid, warnings.Nickname);
    }

    [Fact]
    public void Validate_MalformedEmail_WarnsAboutEmail()
    {
        RegistrationForm form = CreateValidForm();
        form.Email = "nora.example.test";

        RegistrationWarnings warnings = RegisterController.Validate(form);

        Assert.Equal(TextCatalog.RegisterEmailInvalid, warnings.Email);
    }

    [Fact]
    public void Validate_ShortPassword_WarnsAboutLength()
    {
        RegistrationForm form = CreateValidForm();
        form.Password = "Ab#1";
        form.Confirmation = "Ab#1";

        RegistrationWarnings warnings = RegisterController.Validate(form);

        Assert.Equal(TextCatalog.RegisterPasswordTooShort, warnings.Password);
    }

    [Fact]
    public void Validate_WeakPassword_WarnsAboutPolicy()
    {
        RegistrationForm form = CreateValidForm();
        form.Password = "onlylowercase";
        form.Confirmation = "onlylowercase";

        RegistrationWarnings warnings = RegisterController.Validate(form);

        Assert.Equal(TextCatalog.RegisterPasswordPolicy, warnings.Password);
    }

    [Fact]
    public void Validate_DifferentConfirmation_WarnsAboutMismatch()
    {
        RegistrationForm form = CreateValidForm();
        form.Confirmation = "Other#Pass26";

        RegistrationWarnings warnings = RegisterController.Validate(form);

        Assert.Equal(TextCatalog.RegisterConfirmationMismatch, warnings.Confirmation);
    }

    [Fact]
    public void Validate_FebruaryThirtieth_WarnsAboutDate()
    {
        RegistrationForm form = CreateValidForm();
        form.Day = "30";
        form.Month = "2";

        RegistrationWarnings warnings = RegisterController.Validate(form);

        Assert.Equal(TextCatalog.RegisterBirthDateInvalid, warnings.BirthDate);
    }

    [Fact]
    public void Validate_BornLastYear_WarnsAboutAge()
    {
        RegistrationForm form = CreateValidForm();
        form.Year = (DateTime.Today.Year - 1).ToString(System.Globalization.CultureInfo.InvariantCulture);

        RegistrationWarnings warnings = RegisterController.Validate(form);

        Assert.Equal(TextCatalog.RegisterUnderage, warnings.BirthDate);
    }

    [Fact]
    public void Validate_TermsNotAccepted_WarnsAboutTerms()
    {
        RegistrationForm form = CreateValidForm();
        form.HasAcceptedTerms = false;

        RegistrationWarnings warnings = RegisterController.Validate(form);

        Assert.Equal(TextCatalog.RegisterTermsRequired, warnings.Terms);
    }

    [Fact]
    public void GetFieldWarnings_NicknameTaken_WarnsOnTheNickname()
    {
        RegistrationResultCode code = RegistrationResultCode.NicknameTaken;

        RegistrationWarnings? warnings = RegisterController.GetFieldWarnings(code);

        Assert.Equal(TextCatalog.RegisterNicknameTaken, warnings?.Nickname);
    }

    [Fact]
    public async Task RegisterAsync_ValidForm_SendsTheCurrentLanguage()
    {
        RegistrationForm form = CreateValidForm();

        await _controller.RegisterAsync(form);

        Assert.Equal(Language.English.Name, _accounts.LastRegistration?.PreferredLanguage);
    }

    private static RegistrationForm CreateValidForm()
    {
        return new RegistrationForm
        {
            Nickname = "nora_north",
            Email = "nora@example.test",
            Password = "Nora#North26",
            Confirmation = "Nora#North26",
            Day = "15",
            Month = "3",
            Year = "2008",
            HasAcceptedTerms = true,
        };
    }
}
