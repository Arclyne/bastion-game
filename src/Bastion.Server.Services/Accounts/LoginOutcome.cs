using Bastion.Contracts.Accounts;
using Bastion.Server.Services.Auditing;

namespace Bastion.Server.Services.Accounts;

public sealed record LoginOutcome(LoginResult Result, AccessAttempt Attempt);
