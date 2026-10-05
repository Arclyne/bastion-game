namespace Bastion.Server.Data.Entities;

public enum AccessLogResult
{
    LoginSucceeded,
    AccountNotFound,
    AccountLocked,
    WrongPassword,
    AccountPending,
    AccountSanctioned,
    RegistrationSucceeded,
    RegistrationRejected,
}
