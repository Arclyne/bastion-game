using Bastion.Server.Data.Entities;

namespace Bastion.Server.Services.Accounts;

public sealed record AuthenticationOutcome(AccessLogResult Result, Account? Account, int LockMinutes);
