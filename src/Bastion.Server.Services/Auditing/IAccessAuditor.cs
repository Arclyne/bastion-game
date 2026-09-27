using System.Threading.Tasks;

namespace Bastion.Server.Services.Auditing;

public interface IAccessAuditor
{
    Task RecordAsync(AccessAttempt attempt);
}
