using System.Collections.Generic;
using Bastion.Client.Screens;

namespace Bastion.Client.Tests.Navigation;

public sealed record PressOutcome(IReadOnlyList<ScreenId> Destinations, bool HasGoneBack);
