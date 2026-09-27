using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bastion.Server.Data.Entities;
using Bastion.Server.Data.Repositories;

namespace Bastion.Server.Services.Tests.Fakes;

public sealed class FakeGameModeRepository : IGameModeRepository
{
    public const byte ClassicId = 1;

    public List<GameMode> GameModes { get; } =
    [
        new GameMode { GameModeId = ClassicId, Code = "CLASSIC", IsActive = true },
    ];

    public Task<IReadOnlyList<GameMode>> GetActiveAsync()
    {
        IReadOnlyList<GameMode> active = GameModes.Where(gameMode => gameMode.IsActive).ToList();
        return Task.FromResult(active);
    }

    public Task<GameMode?> FindByCodeAsync(string code)
    {
        return Task.FromResult(GameModes.FirstOrDefault(gameMode => gameMode.Code == code));
    }
}
