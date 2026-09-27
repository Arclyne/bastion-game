using Bastion.Server.Data.Entities;

namespace Bastion.Server.Services.Auditing;

public sealed record AccessAttempt(AccessLogResult Result, int? AccountId, string? EnteredIdentifier);
