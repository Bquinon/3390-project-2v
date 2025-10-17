using Godot;
using ConnectMore.Scripts.Data;

public partial class GameData : Node
{
	public LeaderboardEntry PendingScoreEntry { get; set; }

	public string           WinnerString      { get; set; }
}
