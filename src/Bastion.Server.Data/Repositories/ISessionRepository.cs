using System;
using System.Threading.Tasks;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Repositories;

public interface ISessionRepository
{
    Task AddAsync(Session session);

    Task<Session?> FindOpenAsync(byte[] tokenHash, DateTime now);
}
