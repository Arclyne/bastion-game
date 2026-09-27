namespace Bastion.Server.Data.Entities;

// Mirrors CK_AccessLog_Result; only the values the server writes today are listed.
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
