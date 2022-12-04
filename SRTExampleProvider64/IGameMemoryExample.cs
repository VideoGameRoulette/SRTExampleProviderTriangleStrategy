namespace SRTExampleProvider64
{
    public interface IGameMemoryExample
    {
        string GameName { get; }
        string VersionInfo { get; }
        bool IsGameplay { get; }
        bool IsPaused { get; }
        int Money { get; set; }
        int Kudos { get; set; }
        int Liberty { get; set; }
        int Utility { get; set; }
        int Morality { get; set; }
    }
}
