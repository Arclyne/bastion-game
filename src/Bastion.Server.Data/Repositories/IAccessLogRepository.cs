using System.Threading.Tasks;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Repositories;

public interface IAccessLogRepository
{
    Task AddAsync(AccessLog entry);
}
