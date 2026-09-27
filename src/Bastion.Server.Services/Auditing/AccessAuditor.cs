using System;
using System.Threading.Tasks;
using Bastion.Server.Data.Entities;
using Bastion.Server.Data.Repositories;
using Bastion.Server.Services.Common;

namespace Bastion.Server.Services.Auditing;

/// <summary>
/// Writes every sign-in and sign-up attempt to the access log, never with the password (CU-01 RN-02, CU-48).
/// </summary>
public sealed class AccessAuditor : IAccessAuditor
{
    private readonly IAccessLogRepository _accessLogs;
    private readonly ICallContext _callContext;

    /// <summary>
    /// Creates the auditor.
    /// </summary>
    /// <param name="accessLogs">Store of access log entries.</param>
    /// <param name="callContext">Time and address of the current call.</param>
    /// <exception cref="ArgumentNullException">A dependency is null.</exception>
    public AccessAuditor(IAccessLogRepository accessLogs, ICallContext callContext)
    {
        ArgumentNullException.ThrowIfNull(accessLogs);
        ArgumentNullException.ThrowIfNull(callContext);

        _accessLogs = accessLogs;
        _callContext = callContext;
    }

    /// <summary>
    /// Records one access attempt.
    /// </summary>
    /// <param name="attempt">The result and whatever identified the account.</param>
    /// <returns>A task that completes when the entry is stored.</returns>
    /// <exception cref="ArgumentNullException">The attempt is null.</exception>
    public Task RecordAsync(AccessAttempt attempt)
    {
        ArgumentNullException.ThrowIfNull(attempt);

        var entry = new AccessLog
        {
            AccountId = attempt.AccountId,
            EnteredIdentifier = attempt.EnteredIdentifier,
            Result = attempt.Result,
            IpAddress = _callContext.ClientAddress,
            OccurredAt = _callContext.UtcNow,
        };
        return _accessLogs.AddAsync(entry);
    }
}
