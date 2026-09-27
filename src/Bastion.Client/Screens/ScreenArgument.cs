namespace Bastion.Client.Screens;

// Values one screen passes to another through INavigator.GoTo; shared here because screens never reference each
// other's types.
public static class ScreenArgument
{
    public const string HostRole = "host";
    public const string GuestRole = "guest";
    public const string VictoryOutcome = "victory";
    public const string DefeatOutcome = "defeat";
    public const string AiOpponent = "ai";
}
