using System.Collections.Generic;
using System.Threading.Tasks;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Repositories;

public interface IGameModeRepository
{
    Task<IReadOnlyList<GameMode>> GetActiveAsync();

    Task<GameMode?> FindByCodeAsync(string code);
}
