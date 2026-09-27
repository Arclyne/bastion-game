using Bastion.Contracts.Accounts;

namespace Bastion.Server.Services.Accounts;

public sealed record RegistrationOutcome(RegistrationResultCode Code, int? AccountId);
